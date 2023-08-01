///// <reference path="../../../../../node_modules/@types/jquery/dist/jquery.slim.d.ts" />
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from "@angular/material";
import { Component, Inject, OnInit, AfterViewInit, ChangeDetectorRef, HostListener, OnDestroy, ViewEncapsulation } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { PIBTemplate } from "src/app/models/department.model";
import { AlertService, MessageSeverity } from "src/app/services/alert.service";
import { ShapeService } from "src/app/services/shape.service";
import { TextNodeService } from "src/app/services/text-node.service";
import { AuthService } from "src/app/services/auth.service";
import { AccountService } from "src/app/services/account.service";
import { DomSanitizer } from "@angular/platform-browser";
import { Utilities } from "src/app/services/utilities";
import { Subscription } from "rxjs";
import * as signalR from '@aspnet/signalr';
import { DeviceService } from "src/app/services/device.service";
import { Reservation } from "src/app/models/reservation.model";
import { ReservationService } from "src/app/services/reservation.service";
import { ConfigurationService } from "src/app/services/configuration.service";
import { ApplicationSettingService } from "src/app/services/application-setting.service";

@Component({
  selector: 'meeting-room-display',
  templateUrl: './meeting-room.component.html',
  styleUrls: ['./meeting-room.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class MeetingRoomDisplayComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private connection: signalR.HubConnection;
  mac_address: string;
  server: string;
  detail: any;
  nextTimeRun: number;
  facilities: any[];
  server_time: string;
  time: Date;
  prevNowPlaying: any;
  isSignalRManuallyStopped = false;
  original_module_path: string = '';

  private signalRCoreconnection: signalR.HubConnection;
  private pibSignalRCoreconnection: signalR.HubConnection;
  constructor(
    private configurationService: ConfigurationService,
    private deviceService: DeviceService,
    private reservationService: ReservationService,
    private alertService: AlertService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private accountService: AccountService,
    private applicationSetting: ApplicationSettingService,
    private configurations: ConfigurationService,
    private router: Router,
    private domSanitizer: DomSanitizer,
    private cd: ChangeDetectorRef,
    private dialog: MatDialog) {
    this.isSignalRManuallyStopped = false;

    this.route.params.subscribe(queryParams => {
      this.mac_address = queryParams["mac_address"];
    });

    this.route.params.subscribe(queryParams => {
      this.server = queryParams["server"];
    });

    if (!this.mac_address) {
      this.route.queryParams.subscribe(queryParams => {
        this.mac_address = queryParams["mac_address"];
      });
    }

    if (!this.server) {
      this.route.queryParams.subscribe(queryParams => {
        this.server = queryParams["server"];
      });
    }

  }

  ngOnInit() {
    if (this.router.url.indexOf('/meetingroom') > -1) {
      this.authService.reLoginDelegate = () => null;
      if (this.mac_address) {
        this.alertService.startLoadingMessage("Updating display...");
        if (this.server) {
          this.subscription.add(this.applicationSetting.getApplicationSettingByKey(null, 'URL_PIB_SERVER').subscribe(res => {
            if (res) {
              this.subscription.add(this.deviceService.getDeviceById(null, this.mac_address, false, res.value + '/api/pib/get/device').subscribe(result => {
                this.alertService.stopLoadingMessage();
                if (result && result.data) {
                  let device = result.data;
                  this.original_module_path = device.module_path;
                  this.getCurrentDetail(device.location_id);
                  //if (this.original_module_path != device.module_path) {
                  //  if (device.module_path && !(device.module_path.indexOf('/meetingroom') > -1 && device.module_path.indexOf('/signagedisplay') > -1)) {
                  //    window.location.href = res.value + device.module_path;
                  //  } else {
                  //    location.replace(device.module_path);
                  //  }

                    

                  //} else {
                  //  this.getCurrentDetail(device.location_id);
                  //}

                  //this.original_module_path = device.module_path;
                  //if (!device.module_path_value) {
                  //  location.replace(device.module_path);
                  //}
                  

                  this.signalRCoreconnection = this.authService.signalRConnection(this.configurations.baseUrl + "/hub/meetingroom?location_id=" + device.location_id, true, this.signalRCoreconnection);
                  if (this.signalRCoreconnection != null) {

                    this.signalRCoreconnection.onclose(async () => {
                      setTimeout(() => {
                        if (!this.isSignalRManuallyStopped) {
                          console.log("reconnecting");
                          this.signalRCoreconnection = this.authService.signalRConnection(this.configurations.baseUrl + "/hub/meetingroom?location_id=" + device.location_id, true, this.signalRCoreconnection);
                        }
                      }, 5000);
                    });

                    this.signalRCoreconnection.on("RefreshDisplay", (locationId) => {
                      console.log('Refreshing Device Display');
                      //if (locationId && locationId > 0 && locationId != device.location_id) {
                      //  device = deviceVM;
                      //}

                      this.getCurrentDetail(locationId);
                    });
                  }

                  this.pibSignalRCoreconnection = this.authService.signalRConnection(res.value + "/hub/pibdevice?device_id=" + device.id, true, this.signalRCoreconnection);
                  if (this.pibSignalRCoreconnection != null) {
                    this.pibSignalRCoreconnection.onclose(async () => {
                      setTimeout(() => {
                        if (!this.isSignalRManuallyStopped) {
                          console.log("reconnecting");
                          this.signalRCoreconnection = this.authService.signalRConnection(res.value + "/hub/pibdevice?device_id=" + device.id, true, this.signalRCoreconnection);
                        }
                      }, 5000);
                    });

                    this.pibSignalRCoreconnection.on("RefreshDeviceData", (device) => {
                      console.log('Refreshing Device Display');
                      //if (locationId && locationId > 0 && locationId != device.location_id) {
                      //  device = deviceVM;
                      //}
                      if (device) {
                        if (this.original_module_path != device.module_path) {
                          this.original_module_path = device.module_path;
                          location.replace(res.value + '/Display/' + device.mac_address);
                        } else {
                          this.getCurrentDetail(device.location_id);
                        }

                       
                      }
                      
                    });
                  }

                }

              }, error => { }));

            }
          }, error => function (error) {
            this.alertService.stopLoadingMessage();
            console.log(error);
          }));

          
        } else {
          this.subscription.add(this.deviceService.getDeviceById(null, this.mac_address).subscribe(result => {
            this.alertService.stopLoadingMessage();
            if (result && result.data) {
              let device = result.data;
              console.log(device);
              if (!device.module_path_value) {
                location.replace(device.module_path);
              }
              this.getCurrentDetail(device.locationId);

              this.signalRCoreconnection = this.authService.signalRConnection(this.configurations.baseUrl + "/hub/frsdevice?device_id=" + device.id, true, this.signalRCoreconnection);
              if (this.signalRCoreconnection != null) {
                this.signalRCoreconnection.on("RefreshDeviceData", (deviceVM) => {
                  console.log('Refreshing Device Display');
                  if (this.original_module_path != device.module_path) {
                    this.original_module_path = device.module_path;
                    location.replace(this.configurations.baseUrl + '/Display/' + device.macAddress);
                  } else {
                    this.getCurrentDetail(device.location_id);
                  }
                });
              }

            }

          }, error => { }));
        }
      }
    }
  }

  getCurrentDetail(locationId) {
    this.subscription.add(this.reservationService.getCurrentLocationDetail(locationId, null, null)
      .subscribe(results => {
        this.detail = results;
        if (this.detail) {
          //compute next run time (next meeting if available)
          // create Date object for current location
          this.deviceService.getServerTime().subscribe(results => {
            var currentTime = new Date(results);
            console.log(currentTime);
            // convert to msec, add local time zone offsetand  get UTC time in msec
            //var utc = d.getTime() + (d.getTimezoneOffset() * 60000);

            // create new Date object with supplied offset
            //var currentTime = new Date(utc + (3600000 * 8));
            //get current date time, you can set your own Date() over here
            var stationdate = new Date(results);

            //clear Interval, it's my system requirement
            if (this.prevNowPlaying) {
              clearInterval(this.prevNowPlaying);
            }
            this.server_time = stationdate.toLocaleString();
            // set clock and Interval
            this.prevNowPlaying = setInterval(() => {
              stationdate = new Date(stationdate.setSeconds(stationdate.getSeconds() + 1));
              this.time = stationdate;
              this.server_time = this.time.toLocaleString();
            }, 1000);

            // return time as a string
            //this.server_time = currentTime.toLocaleString();
            //console.log("The Server time is " + currentTime.toLocaleString());

            if (this.detail.current_reservation_start_time) {
              var currentResStartTime = new Date(this.detail.current_reservation_start_time);
              var currentResEndTime = new Date(this.detail.current_reservation_end_time);
              console.log(currentTime);
              console.log(currentResStartTime);
              this.nextTimeRun = currentResStartTime.getTime() - currentTime.getTime();
              if (this.nextTimeRun <= 0) {
                this.nextTimeRun = currentResEndTime.getTime() - currentTime.getTime();
              }
              console.log(this.nextTimeRun);
              if (this.nextTimeRun > 0) {
                var timerId = setTimeout(() =>
                  this.getCurrentDetail(locationId), this.nextTimeRun + 2000);
                //clearTimeout(timerId);
              }
            } else if (this.detail.next_reservation_start_time) {
              var nextResStartTime = new Date(this.detail.next_reservation_start_time);
              var nextResEndTime = new Date(this.detail.next_reservation_end_time);
              console.log(currentTime);
              console.log(currentResStartTime);
              this.nextTimeRun = nextResStartTime.getTime() - currentTime.getTime();
              if (this.nextTimeRun <= 0) {
                this.nextTimeRun = nextResEndTime.getTime() - currentTime.getTime();
              }
              console.log(this.nextTimeRun);
              if (this.nextTimeRun > 0) {
                var timerId = setTimeout(() =>
                  this.getCurrentDetail(locationId), this.nextTimeRun + 2000);
                //clearTimeout(timerId);
              }
            }

            this.facilities = [];
            this.facilities.push({ name: 'Max Capacity', value: this.detail.location_capacity });
            if (this.detail.facilities && this.detail.facilities) {
              this.detail.facilities.forEach((facility, index, facilities) => {
                let fullPath = this.configurationService.baseUrl + '/' + facility.path.replace(/\\/g, '/');
                this.facilities.push({ name: facility.name, path: fullPath });
              });
            }
          },
            error => { });
        }
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the information.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getNextEventDate(d: string) {
    var date1 = new Date(),
      date2 = new Date(d);
    date1.setHours(0, 0, 0, 0);
    date2.setHours(0, 0, 0, 0);

    return date1.getTime() === date2.getTime() ? "" : date2.toLocaleDateString();
  }

  ngAfterViewInit() {
  }

  ngOnDestroy() {
    //this.accountService.disconnectSignalRConnection(this.connection);
    this.isSignalRManuallyStopped = true;
    this.signalRCoreconnection.stop();
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }
}
