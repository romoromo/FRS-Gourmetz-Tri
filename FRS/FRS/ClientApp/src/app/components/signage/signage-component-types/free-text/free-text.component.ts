import { Component, Input, ChangeDetectorRef } from '@angular/core'
import { ActivatedRoute } from '@angular/router';
import { SignageComponent } from '../../../../models/signage-component.model';
import { AccountService } from '../../../../services/account.service';
import { AuthService } from '../../../../services/auth.service';
import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'free-text',
  template: `
<text-display [preview]="preview" [textStyle]="configurations.configurationsObj.name" [previewText]="configurations.configurationsObj.freetext || 'Free Text'" [actualText]="configurations.configurationsObj.freetext"></text-display>
`

})
export class FreeText {
  @Input() configurations: any;
  @Input() preview: boolean;

  mac_address: any;

  private signalRCoreconnection: signalR.HubConnection;
  device: any;
  locationColorTheme: any;

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
          }
        });
    }
  }
}
