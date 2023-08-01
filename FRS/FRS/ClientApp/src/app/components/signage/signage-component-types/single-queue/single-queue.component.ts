import { Component, Input, ChangeDetectorRef } from '@angular/core'
import { ActivatedRoute } from '@angular/router';
import { ConfigurationService } from '../../../../services/configuration.service';
import { SignageComponent } from '../../../../models/signage-component.model';
import { AccountService } from '../../../../services/account.service';
import { AuthService } from '../../../../services/auth.service';
import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'single-queue',
  template: `<div *ngFor="let t of configurations.configurationsObj.texts | keyvalue" [style.visibility]="t.value.hide ? 'hidden': 'visible'"
[style.position]="'absolute'"

[style.display]="'flex'"
[style.align-items]="t.value.vAlign == 'middle' ? 'center' : (t.value.vAlign == 'top' ? 'flex-start' : 'flex-end')"

[style.font-family]="t.value.fontFamily"
[style.font-size.px]="t.value.textSize"
[style.color]="t.value.textColor"
[style.left.px]="t.value.x"
[style.top.px]="t.value.y"
[style.width.px]="t.value.width"
[style.height.px]="t.value.height"
[style.background-color]="t.value.fieldColorLocation && locationColorTheme ? locationColorTheme : t.value.fieldColor"
[style.font-weight]="t.value.bold ? 'bold' : 'normal'"
[style.text-decoration]="t.value.underline ? 'underline' : 'none'"
[style.font-style]="t.value.italic ? 'italic' : 'normal'"
>
<div [style.width.%]="100" [style.text-align]="t.value.textAlign">
<span *ngIf="preview">{{t.value.label}}</span>
<span *ngIf="!preview">{{t.value.value}}</span>
</div>
</div>`

})
export class SingleQueue {
  @Input() configurations: any;
  @Input() preview: boolean;

  mac_address: any;

  private signalRCoreconnection: signalR.HubConnection;
  device: any;
    locationColorTheme: any;
    locationName: any;

  constructor(private changeDetectorRef: ChangeDetectorRef, private authService: AuthService, private route: ActivatedRoute, private deviceService: DeviceService,
    private configurationService: ConfigurationService  ) {

  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    console.log("initqueue");

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
          this.configurations.configurationsObj.texts.queueNo.value = param.callAction == "call" || param.callAction == "silent call" ? param.queueNo : '';

          if (!this.configurations.configurationsObj.roomInfoLocationName) this.configurations.configurationsObj.texts.roomInfo.value = param.roomInfo;

          this.configurations.configurationsObj.texts.servingInfo.value = param.servingInfo;

          this.configurations.configurationsObj.texts.doctorInfo.value = param.doctorInfo;

          this.configurations.configurationsObj.texts.assistantInfo.value = param.assistantInfo;

          param.currentDisplay = this.configurations.configurationsObj.texts.queueNo.value;
          param.deviceId = this.mac_address;

          this.signalRCoreconnection.send("queueReturn", param);
        });
      }

      this.getDeviceInfo();
    }
  }

  getDeviceInfo() {
    if (this.mac_address) {
      this.deviceService.getDeviceById(null, this.mac_address, true)
        .subscribe(results => {

          this.device = results.data;
          if (this.device) {
            this.locationColorTheme = this.device.locationColorTheme;
            this.locationName = this.device.locationName;

            if (this.configurations.configurationsObj.roomInfoLocationName) this.configurations.configurationsObj.texts.roomInfo.value = this.locationName;
          }
        });
    }
  }
}
