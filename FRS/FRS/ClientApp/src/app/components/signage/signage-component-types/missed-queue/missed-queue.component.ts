import { Component, Input, ChangeDetectorRef } from '@angular/core'
import { ActivatedRoute } from '@angular/router';
import { ConfigurationService } from 'src/app/services/configuration.service';

import { SignageComponent } from '../../../../models/signage-component.model';
import { AuthService } from '../../../../services/auth.service';
import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'missed-queue',
  templateUrl: './missed-queue.component.html'

})
export class MissedQueue {
  @Input() configurations: any;
  @Input() preview: boolean;

  mac_address: any;

  device: any;
  locationColorTheme: any;
  locationName: any;

  page: number = 0;

  math = Math;

  private signalRCoreconnection: signalR.HubConnection;
  data: any = [];

  constructor(private changeDetectorRef: ChangeDetectorRef, private route: ActivatedRoute, private authService: AuthService, private deviceService: DeviceService,
    private configurationService: ConfigurationService) {

  }

  start(first?) {
    if (!first) {
      this.page++;

      if (this.page * this.configurations.configurationsObj.noOfRows * this.configurations.configurationsObj.noOfColumns >= this.data.length) this.page = 0;
    }

    setTimeout(() => {
      this.start();
    }, (this.configurations.configurationsObj.interval || 5) * 1000);
  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    if (!this.preview) {
      this.route.params.subscribe(queryParams => {
        this.mac_address = queryParams["mac_address"];
      });

      if (!this.mac_address) {
        this.route.queryParams.subscribe(queryParams => {
          this.mac_address = queryParams["mac_address"];
        });
      }

      this.signalRCoreconnection = this.authService.signalRConnection(this.configurationService.baseUrl + "/hub/queue?device_id=" + this.mac_address, true, this.signalRCoreconnection);
      if (this.signalRCoreconnection != null) {
        this.signalRCoreconnection.on("UpdateQueue", (param) => {
          this.updateQueue(param);
        });
      }

      this.getDeviceInfo();

      this.start(true);
    }
  }

  updateQueue(param) {
    this.data = this.data.filter(d => d.queueNo != param.queueNo);

    if (param.callAction == "missed queue") {
      this.data.unshift(this.assign(param));
    }

    param.deviceId = this.mac_address;

    //this.signalRCoreconnection.send("queueReturn", param);
  }

  assign(param) {
    let target: any = {};

    target.roomInfo = param.roomInfo;

    target.servingInfo = param.servingInfo;

    target.doctorInfo = param.doctorInfo;

    target.assistantInfo = param.assistantInfo;

    target.queueNo = param.queueNo;

    return target;
  }

  getDeviceInfo() {
    if (this.mac_address) {
      this.deviceService.getDeviceById(null, this.mac_address, true)
        .subscribe(results => {

          this.device = results.data;
          if (this.device) {
            this.locationColorTheme = this.device.locationColorTheme;
            this.locationName = this.device.locationName;
          }
        });
    }
  }

  getArrays(num) {
    if (num && num > 0)
      return new Array(num);
  }
}
