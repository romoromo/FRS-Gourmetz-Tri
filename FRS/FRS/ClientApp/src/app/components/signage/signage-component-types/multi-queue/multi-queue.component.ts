import { Component, Input, ChangeDetectorRef, Directive } from '@angular/core'
import { ActivatedRoute } from '@angular/router';
import { ConfigurationService } from '../../../../services/configuration.service';
import { SignageComponent } from '../../../../models/signage-component.model';
import { AuthService } from '../../../../services/auth.service';
import { DeviceService } from '../../../../services/device.service';
import { animate, state, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'multi-queue',
  templateUrl: './multi-queue.component.html',
  animations: [
    trigger("changeBodyColor", [
      state('off', style({
        color: '{{default}}'
      }), { params: { default: 'black' } }),
      state('on', style({
        color: '{{color}}'
      }), { params: { color: 'red' } }),
      transition('off => on', animate('0.5s')),
      transition('on => off', animate('0.5s'))
    ])
  ]

})
export class MultiQueue {
  @Input() configurations: any;
  @Input() preview: boolean;

  mac_address: any;

  device: any;
  locationColorTheme: any;
  locationName: any;

  private signalRCoreconnection: signalR.HubConnection;
  data: any = {};
  dataFifo: any = [];
  obj: any;

  swap = true;

  constructor(private changeDetectorRef: ChangeDetectorRef, private route: ActivatedRoute, private authService: AuthService, private deviceService: DeviceService,
    private configurationService: ConfigurationService) {

  }

  checkEven(j) {
    return (this.configurations.configurationsObj.swapLabel && j % 2 != 0) || (!this.configurations.configurationsObj.swapLabel && j % 2 == 0);
  }

  checkOdd(j) {
    return (this.configurations.configurationsObj.swapLabel && j % 2 == 0) || (!this.configurations.configurationsObj.swapLabel && j % 2 != 0);
  }

  floor(j) {
    return Math.floor(j);
  }

  parseInteger(str: string) {
    try {
      return parseInt(str);
    } catch (ex) {
      return str;
    }
  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    this.configurations.configurationsObj.stationlabels = this.configurations.configurationsObj.stationlabels || {};

    this.configurations.configurationsObj.headerlabels = this.configurations.configurationsObj.headerlabels || {};
    this.configurations.configurationsObj.queuenolabels = this.configurations.configurationsObj.queuenolabels || {};

    this.obj = this.configurations.configurationsObj;

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
          this.updateStationId(param);
        });
      }

      this.getDeviceInfo();
    }
  }

  updateStationId(param) {
    let dataidx = null;

    let idx = this.findStationId(param);

    if (!idx) return;

    if (!this.configurations.configurationsObj.fifoMode) {
      if (!this.data[idx]) this.data[idx] = {};

      dataidx = this.data[idx];
    } else {
      let exist = this.dataFifo.find((d) => d ? d.roomName === param.roomName : false);

      if (!exist) {
        this.dataFifo.unshift({});
        dataidx = this.dataFifo[0];

        this.dataFifo.length = this.configurations.configurationsObj.noOfRows * this.configurations.configurationsObj.noOfColumns;
      } else {
        dataidx = exist;
      }
    }

    if (!dataidx) return;

    dataidx.stationId = param.stationId;
    dataidx.roomName = param.callAction == "call" || param.callAction == "silent call" ? param.roomName : '';

    dataidx.queueNo = param.callAction == "call" || param.callAction == "silent call" ? param.queueNo : '';

    dataidx.roomInfo = param.roomInfo;

    dataidx.servingInfo = param.servingInfo;

    dataidx.doctorInfo = param.doctorInfo;

    dataidx.assistantInfo = param.assistantInfo;

    param.currentDisplay = dataidx.queueNo;
    param.deviceId = this.mac_address;

    if (dataidx.queueNo && !this.configurations.configurationsObj.noSound && param.callAction == "call") {
      let audio = new Audio(this.configurations.configurationsObj.queueSound || 'Resources/queue-bell.mp3') //this.configurations.configurationsObj.queueSound);
      audio.play();
    }

    if (dataidx.queueNo) {
      dataidx.isFlashing = true;
      dataidx.isOn = true;

      this.toggle(dataidx);

      setTimeout(() => dataidx.isFlashing = false, (this.configurations.configurationsObj.flashDuration || 7) * 1000);
    }

    this.signalRCoreconnection.send("queueReturn", param);

    if (this.configurations.configurationsObj.fifoMode) this.dataFifo = this.dataFifo.filter(d => d.queueNo && d.roomName);
  }

  toggle(dataidx) {
    setTimeout(() => {
      dataidx.isOn = !dataidx.isOn;
      if (dataidx.isFlashing) this.toggle(dataidx);
    }, 500);
  }

  findStationId(param) {
    let stationids = this.configurations.configurationsObj.stationids;

    for (const key of Object.keys(stationids)) {
      if (stationids[key] == param.stationId) return key;
    }

    return null;
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

@Directive({
  selector: 'ngInit',
  exportAs: 'ngInit'
})
export class NgInit {
  @Input() values: any = {};

  @Input() ngInit;
  ngOnInit() {
    if (this.ngInit) { this.ngInit(); }
  }
}
