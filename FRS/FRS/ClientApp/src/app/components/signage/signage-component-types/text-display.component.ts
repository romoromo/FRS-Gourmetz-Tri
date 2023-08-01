import { Component, Input, ChangeDetectorRef } from '@angular/core'
import { ActivatedRoute } from '@angular/router';
import { DeviceService } from '../../../services/device.service';

@Component({
  selector: 'text-display',
  template: `<div
[style.visibility]="textStyle?.hide ? 'hidden': 'visible'"
[style.position]="'absolute'"

[style.display]="'flex'"
[style.align-items]="textStyle?.vAlign == 'middle' ? 'center' : (textStyle?.vAlign == 'top' ? 'flex-start' : 'flex-end')"

[style.font-family]="textStyle?.fontFamily"
[style.font-size.px]="textStyle?.textSize"
[style.color]="textStyle?.textColor"
[style.left.px]="textStyle?.x"
[style.top.px]="textStyle?.y"
[style.width.px]="textStyle?.width"
[style.height.px]="textStyle?.height"
[style.background-color]="textStyle?.fieldColorLocation && locationColorTheme ? locationColorTheme : textStyle?.fieldColor"
[style.font-weight]="textStyle?.bold ? 'bold' : 'normal'"
[style.text-decoration]="textStyle?.underline ? 'underline' : 'none'"
[style.font-style]="textStyle?.italic ? 'italic' : 'normal'"
>
<div [style.width.%]="100" [style.text-align]="textStyle?.textAlign">
<span *ngIf="preview">{{previewText}}</span>
<span *ngIf="!preview">
  <span >{{actualText}}</span>
</span>
</div>
</div>`

})
export class TextDisplay {
  //@Input() configurations: any;
  @Input() preview: boolean;
  @Input() textStyle: any;
  @Input() previewText: string;
  @Input() actualText: string;

  mac_address: any;

  device: any;
  locationColorTheme: any;
  locationName: any;

  constructor(private changeDetectorRef: ChangeDetectorRef, private route: ActivatedRoute, private deviceService: DeviceService) {

  }

  ngOnInit() {
   /* if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);*/

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

           /* if (this.configurations.configurationsObj.roomInfoLocationName) this.configurations.configurationsObj.texts.roomInfo.value = this.locationName;*/
          }
        });
    }
  }
}
