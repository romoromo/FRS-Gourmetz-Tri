import { Component, Input, ChangeDetectorRef } from '@angular/core'
import { ActivatedRoute } from '@angular/router';
import { SignageComponent } from '../../../../models/signage-component.model';
import { AccountService } from '../../../../services/account.service';
import { AuthService } from '../../../../services/auth.service';
import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'location-name',
  template: `<div [style.position]="'absolute'" [style.display]="'flex'"
    [style.align-items]="configurations.configurationsObj.name.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.name.vAlign == 'top' ? 'flex-start' : 'flex-end')"
    [style.font-family]="configurations.configurationsObj.name.fontFamily"
    [style.font-size.px]="configurations.configurationsObj.name.textSize"
    [style.color]="configurations.configurationsObj.name.textColor"
    [style.left.px]="configurations.configurationsObj.name.x" [style.top.px]="configurations.configurationsObj.name.y"
    [style.width.px]="configurations.configurationsObj.name.width"
    [style.height.px]="configurations.configurationsObj.name.height"
    [style.background-color]="configurations.configurationsObj.name.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.name.fieldColor"
    [style.font-weight]="configurations.configurationsObj.name.bold ? 'bold' : 'normal'"
    [style.text-decoration]="configurations.configurationsObj.name.underline ? 'underline' : 'none'"
    [style.font-style]="configurations.configurationsObj.name.italic ? 'italic' : 'normal'">
    <div [style.width.%]="100" [style.text-align]="configurations.configurationsObj.name.textAlign">
        <span *ngIf="preview">Location Name</span>
        <span *ngIf="!preview">{{locationName}}
        </span>
    </div>
</div>

<div [style.position]="'absolute'" [style.display]="'flex'"
    [style.align-items]="configurations.configurationsObj.group.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.group.vAlign == 'top' ? 'flex-start' : 'flex-end')"
    [style.font-family]="configurations.configurationsObj.group.fontFamily"
    [style.font-size.px]="configurations.configurationsObj.group.textSize"
    [style.color]="configurations.configurationsObj.group.textColor"
    [style.left.px]="configurations.configurationsObj.group.x" [style.top.px]="configurations.configurationsObj.group.y"
    [style.width.px]="configurations.configurationsObj.group.width"
    [style.height.px]="configurations.configurationsObj.group.height"
    [style.background-color]="configurations.configurationsObj.group.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.group.fieldColor"
    [style.font-weight]="configurations.configurationsObj.group.bold ? 'bold' : 'normal'"
    [style.text-decoration]="configurations.configurationsObj.group.underline ? 'underline' : 'none'"
    [style.font-style]="configurations.configurationsObj.group.italic ? 'italic' : 'normal'">
    <div [style.width.%]="100" [style.text-align]="configurations.configurationsObj.group.textAlign">
        <span *ngIf="preview">Location Group</span>
        <span *ngIf="!preview">{{locationGroup}}
        </span>
    </div>
</div>
`

})
export class LocationName {
  @Input() configurations: any;
  @Input() preview: boolean;

  mac_address: any;

  private signalRCoreconnection: signalR.HubConnection;
  device: any;
  locationColorTheme: any;
  locationName: any;
    locationGroup: any;

  constructor(private changeDetectorRef: ChangeDetectorRef, private authService: AuthService, private route: ActivatedRoute, private deviceService: DeviceService) {

  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    console.log('location')

    if (!this.preview) {
      this.route.params.subscribe(queryParams => {
        this.mac_address = queryParams["mac_address"];
      });

      if (!this.mac_address) {
        this.route.queryParams.subscribe(queryParams => {
          this.mac_address = queryParams["mac_address"];
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
            this.locationGroup = this.device.locationGroup;

            if (this.configurations.configurationsObj.roomInfoLocationName) this.configurations.configurationsObj.texts.roomInfo.value = this.locationName;
          }
        });
    }
  }
}
