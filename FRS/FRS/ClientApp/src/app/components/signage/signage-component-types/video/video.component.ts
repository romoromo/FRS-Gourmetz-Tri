import { Component, Input } from '@angular/core'
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../../services/auth.service';
import { ConfigurationService } from '../../../../services/configuration.service';

import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'signage-video',
  template: `<app-vjs-player [options]="{ autoplay: 'any', loop: true, controls: false, preload: 'auto', muted: false, volume: defaultVolume, fluid: true, sources: [{ src: configurations.configurationsObj.url, type: configurations.configurationsObj.type }]}"></app-vjs-player>`

})
export class Video {
  @Input() configurations: any;
  @Input() preview: boolean;

  mac_address: any;
  device: any;
  locationColorTheme: any;
  locationName: any;

  private signalRCoreconnection: signalR.HubConnection;
    defaultVolume: any = 0;

  constructor(private sanitizer: DomSanitizer, private route: ActivatedRoute, private authService: AuthService,
    private configurationService: ConfigurationService, private deviceService: DeviceService) {

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

      

      this.getDeviceInfo();
    }
  }

  ngOnChanges(changes: any) {
    console.log("HTML Changes");
  }

  getDeviceInfo() {
    if (this.mac_address) {
      this.deviceService.getDeviceById(null, this.mac_address, true)
        .subscribe(results => {

          this.device = results.data;
          if (this.device) {
            this.locationColorTheme = this.device.locationColorTheme;
            this.locationName = this.device.locationName;
            this.defaultVolume = this.device.defaultVolume || 0;

            this.signalRCoreconnection = this.authService.signalRConnection(this.configurationService.baseUrl + "/hub/frsdevice?device_id=" + this.device.id + "&source=server", true, this.signalRCoreconnection);
            if (this.signalRCoreconnection != null) {
              this.signalRCoreconnection.on("RefreshDeviceData", (deviceVM) => {
                console.log('Refreshing Device Display');
                this.defaultVolume = deviceVM.defaultVolume || 0;
              });
            }
          }
        });
    }
  }
}
