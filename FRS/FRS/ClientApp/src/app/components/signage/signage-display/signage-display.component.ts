import { Component } from '@angular/core'
import { ActivatedRoute } from '@angular/router';
import { PIBService } from '../../../services/pib.service';
import { DeviceService } from '../../../services/device.service';
import { SignagePublicationService } from '../../../services/signage-publication.service';
import { SignagePublication } from '../../../models/signage-publication.model';
import { SignageSchedule } from '../../../models/signage-schedule.model';
import { SignageCompilation } from '../../../models/signage-compilation.model';
import { SignageComponentTypeService } from '../../../services/signage-component-type.service';
import { AuthService } from '../../../services/auth.service';
import { ConfigurationService } from '../../../services/configuration.service';
import { Subscription } from 'rxjs';
import { ApplicationSettingService } from '../../../services/application-setting.service';

@Component({
  selector: 'signage-display',
  templateUrl: `./signage-display.component.html`,
  styleUrls: ['./signage-display.component.css']
})
export class SignageDisplayComponent {
  private subscription: Subscription = new Subscription();
  private mac_address: string = '';
  private publication_id: string = '';

  message: string = "Unable to load display.";
  device: any;
  publication: SignagePublication;
  schedule: SignageSchedule;
  compilation: SignageCompilation;
  compilationList: SignageCompilation[];
  //now: Date;
  componentTypeMap: any;

  private signalRCoreconnection: signalR.HubConnection;
  private pibSignalRCoreconnection: signalR.HubConnection;
    preview: boolean;

  constructor(private route: ActivatedRoute, private deviceService: DeviceService, private signagePublicationService: SignagePublicationService, private signageComponentTypeService: SignageComponentTypeService, private authService: AuthService, private configurations: ConfigurationService, private applicationSetting: ApplicationSettingService,) {
  }

  getTimeString(time) {
    return ("0" + time.getHours()).slice(-2) + ":" +
      ("0" + time.getMinutes()).slice(-2) + ":" +
      ("0" + time.getSeconds()).slice(-2);
  }

  getEndTimeSeconds() {
    if (this.schedule.endTime) {
      let endTimeObj = new Date(this.schedule.endTime);
      let now = new Date();

      let end = new Date(now.getFullYear(), now.getMonth(), now.getDate(), endTimeObj.getHours(), endTimeObj.getMinutes(), endTimeObj.getSeconds());

      let dif = end.getTime() - new Date().getTime();

      let sec = Math.floor(dif / 1000);

      if (sec > 0) return sec;
    }
  }

  startInterval(index) {
    if (index > this.compilationList.length - 1) index = 0;

    this.compilation = this.compilationList[index];

    let interval = null;

    if (this.compilation.interval) interval = this.compilation.interval;

    let sec = this.getEndTimeSeconds();
    if ((sec && !interval) || (sec && interval && interval > sec)) {
      setTimeout(() => {
        this.loadData();
      }, sec * 1000);
    } else if (interval) {
      setTimeout(() => {
        this.startInterval(index + 1);
      }, interval * 1000);
    }
  }

  start(publication: SignagePublication) {
    if (publication.schedules) {
      for (let s of publication.schedules) {
        let now = new Date();

        if (s.effectiveDate && s.ineffectiveDate) {
          s.effectiveDateObj = new Date(s.effectiveDate.split("T")[0]);
          s.ineffectiveDateObj = new Date(s.ineffectiveDate.split("T")[0]);
          if (s.effectiveDateObj <= now && s.ineffectiveDateObj >= now) {

            if (s.startTime && s.endTime) {
              s.startTimeObj = s.startTime.split("T")[1];
              s.endTimeObj = s.endTime.split("T")[1];
              let nowTime = this.getTimeString(now);

              if (s.startTimeObj <= nowTime && s.endTimeObj >= nowTime) {
                let day = now.getDay();

                if ((day == 0 && s.sunday) ||
                  (day == 1 && s.monday) ||
                  (day == 2 && s.tuesday) ||
                  (day == 3 && s.wednesday) ||
                  (day == 4 && s.thursday) ||
                  (day == 5 && s.friday) ||
                  (day == 6 && s.saturday)
                ) {
                  this.schedule = s;
                }
              }
            }
          }
        }
      }
    }

    if (this.schedule || this.preview) {

      if (this.preview && !this.schedule && publication.schedules && publication.schedules[0]) this.schedule = publication.schedules[0];

      if (this.schedule.compilations && this.schedule.compilations[0]) {
        this.compilationList = this.schedule.compilations;

        for (let c of this.compilationList) {
          if (c.backgroundImage) c.backgroundImage = c.backgroundImage.replace(/\\/g, '/');

          for (let p of c.components) {
            if (p.backgroundImage) p.backgroundImage = p.backgroundImage.replace(/\\/g, '/');
          }
        }

        this.startInterval(0);
      } else {
        this.message = "Current schedule does not have any compilation.";

        setTimeout(() => this.start(this.publication), 5 * 60 * 1000);
      }
    } else {
      this.message = "No schedule match for current time.";

      setTimeout(() => this.start(this.publication), 5 * 60 * 1000);
    }
  }

  getPublication(publicationId) {
    this.publication_id = publicationId;

    this.signagePublicationService.getSignagePublication(publicationId).subscribe(p => {
      this.publication = p;

      if (this.publication) {
        if (this.publication.approved || this.preview) this.start(this.publication);
        else this.message = "This publication '" + this.publication.name + "' has not been approved.";
      } else {
        this.message = "No publication data found."
      }
    });
  }

  loadData() {
    if (this.publication_id) this.getPublication(this.publication_id);

    this.deviceService.getDeviceById(null, this.mac_address, true)
      .subscribe(results => {

        this.device = results.data;

        if (!this.publication_id) {
          if (this.device) {
            if (this.device.publicationId) {
              this.getPublication(this.device.publicationId);
            } else {
              this.message = "This device has not been assigned publication."
            }


          } else {
            this.message = "No device data found."
          }
        }

        this.initSignalR()
      },
        error => {
          this.message = "Error Fetching Device.";
        });
  }

  initSignalR() {
    if (this.device) {
      this.signalRCoreconnection = this.authService.signalRConnection(this.configurations.baseUrl + "/hub/frsdevice?device_id=" + this.device.id + "&source=server", true, this.signalRCoreconnection);
      if (this.signalRCoreconnection != null) {
        this.signalRCoreconnection.on("RefreshDeviceData", (deviceVM) => {
          if (this.device.moduleId != deviceVM.moduleId)
            window.location.replace(window.location.origin + '/display/' + this.mac_address);
          else if (this.device.publicationId != deviceVM.publicationId || this.device.locationId != deviceVM.locationId)
            window.location.replace(window.location.origin + '/display/' + this.mac_address);
        });

        this.signalRCoreconnection.on("RefreshPanel", (deviceVM) => {
          console.log('Refreshing Panel');
          window.location.reload(true);
        });
      }
    } else {
      this.subscription.add(this.applicationSetting.getApplicationSettingByKey(null, 'URL_PIB_SERVER').subscribe(res => {
        if (res) {
          this.subscription.add(this.deviceService.getDeviceById(null, this.mac_address, false, res.value + '/api/pib/get/device').subscribe(result => {
            if (result && result.data) {
              let device = result.data;
              if (!device.module_path_value) {
                location.replace(device.module_path);
              }

              this.pibSignalRCoreconnection = this.authService.signalRConnection(res.value + "/hub/pibdevice?device_id=" + device.id, true, this.pibSignalRCoreconnection);
              if (this.pibSignalRCoreconnection != null) {


                this.pibSignalRCoreconnection.on("RefreshDeviceData", (deviceVM) => {
                  if (device.moduleId != deviceVM.moduleId)
                    window.location.replace(res.value + '/display/' + this.mac_address);
                  else if (device.publicationId != deviceVM.publicationId || device.locationId != deviceVM.locationId)
                    window.location.replace(window.location.origin + '/display/' + this.mac_address);

                });

                this.pibSignalRCoreconnection.on("RefreshPanel", (deviceVM) => {
                  console.log('Refreshing Panel');
                  window.location.reload(true);
                });
              }

            }

          }, error => { }));

        }
      }, error => function (error) {
        this.alertService.stopLoadingMessage();
        console.log(error);
      }));
    }
  }

  ngOnInit() {
    try {
      (document.body as HTMLElement).style.overflow = 'hidden';
    } catch (ex) { }

    this.componentTypeMap = this.signageComponentTypeService.getComponentTypeMap();

    this.route.params.subscribe(queryParams => {
      this.mac_address = queryParams["mac_address"];
    });

    if (!this.mac_address) {
      this.route.queryParams.subscribe(queryParams => {
        this.mac_address = queryParams["mac_address"];
      });
    }

    if (this.mac_address == 'serverpreview') this.preview = true;

    this.route.params.subscribe(queryParams => {
      this.publication_id = queryParams["publication_id"];
    });

    if (!this.publication_id) {
      this.route.queryParams.subscribe(queryParams => {
        this.publication_id = queryParams["publication_id"];
      });
    }

    this.loadData();
  }
}
