import { Component, Input, ChangeDetectorRef, ElementRef, ViewChild } from '@angular/core'
import { SignageComponent } from '../../../../models/signage-component.model';
import { style, animate, AnimationBuilder, AnimationPlayer } from '@angular/animations';
import { ActivatedRoute } from '@angular/router';
import { DeviceService } from '../../../../services/device.service';
import { AccountService } from '../../../../services/account.service';
import { AuthService } from '../../../../services/auth.service';
import { ConfigurationService } from '../../../../services/configuration.service';

@Component({
  selector: 'emergency-message',
  template: `<div
[style.visibility]="!configurations.configurationsObj.textStyle?.hide && !message4 ? 'visible' : 'hidden'"
[style.position]="'absolute'"
[style.overflow]="'hidden'"
[style.display]="'flex'"
[style.align-items]="configurations.configurationsObj.textStyle?.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.textStyle?.vAlign == 'top' ? 'flex-start' : 'flex-end')"
[style.font-family]="configurations.configurationsObj.textStyle?.fontFamily"
[style.text-align]="configurations.configurationsObj.textStyle?.textAlign"
[style.font-size.px]="configurations.configurationsObj.textStyle?.textSize"
[style.color]="configurations.configurationsObj.textStyle?.textColor"
[style.left.px]="configurations.configurationsObj.textStyle?.x"
[style.top.px]="configurations.configurationsObj.textStyle?.y"
[style.width.px]="configurations.configurationsObj.textStyle?.width"
[style.height.px]="configurations.configurationsObj.textStyle?.height"
[style.background-color]="configurations.configurationsObj.textStyle?.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.textStyle?.fieldColor"
[style.font-weight]="configurations.configurationsObj.textStyle?.bold ? 'bold' : 'normal'"
[style.text-decoration]="configurations.configurationsObj.textStyle?.underline ? 'underline' : 'none'"
[style.font-style]="configurations.configurationsObj.textStyle?.italic ? 'italic' : 'normal'">
  <div
     
    [style.position]="'absolute'"
    id="el" #el
    [style.white-space]="'nowrap'">
      <div *ngIf="preview">Sample Message!</div><div *ngIf="!preview">{{message}}</div>
  </div>
</div>
<div
[style.visibility]="!configurations.configurationsObj.emergencyStyle?.hide && (preview || message4) ? 'visible' : 'hidden'"
[style.position]="'absolute'"
[style.overflow]="'hidden'"
[style.display]="'flex'"
[style.align-items]="configurations.configurationsObj.emergencyStyle?.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.emergencyStyle?.vAlign == 'top' ? 'flex-start' : 'flex-end')"
[style.font-family]="configurations.configurationsObj.emergencyStyle?.fontFamily"
[style.text-align]="configurations.configurationsObj.emergencyStyle?.textAlign"
[style.font-size.px]="configurations.configurationsObj.emergencyStyle?.textSize"
[style.color]="configurations.configurationsObj.emergencyStyle?.textColor"
[style.left.px]="configurations.configurationsObj.emergencyStyle?.x"
[style.top.px]="configurations.configurationsObj.emergencyStyle?.y"
[style.width.px]="configurations.configurationsObj.emergencyStyle?.width"
[style.height.px]="configurations.configurationsObj.emergencyStyle?.height"
[style.background-color]="configurations.configurationsObj.emergencyStyle?.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.emergencyStyle?.fieldColor"
[style.font-weight]="configurations.configurationsObj.emergencyStyle?.bold ? 'bold' : 'normal'"
[style.text-decoration]="configurations.configurationsObj.emergencyStyle?.underline ? 'underline' : 'none'"
[style.font-style]="configurations.configurationsObj.emergencyStyle?.italic ? 'italic' : 'normal'">
  <div
     [style.width.%]="100" [style.text-align]="configurations.configurationsObj.emergencyStyle?.textAlign"
    [style.position]="'absolute'"
    id="em" #em>
      <div
        [style.animation]="configurations.configurationsObj.emergencyStyle?.disableBlinking ? 'none' : 'blinker ' + (configurations.configurationsObj.blinkSpeed || 0.5) + 's linear infinite'"
        *ngIf="preview">
        Emergency Message!
      </div>
      <div
         [style.animation]="configurations.configurationsObj.emergencyStyle?.disableBlinking ? 'none' : 'blinker ' + (configurations.configurationsObj.blinkSpeed || 0.5) + 's linear infinite'"
         *ngIf="!preview">
        {{message}}
      </div>
  </div>
</div>
<style>
.blink_me {
  animation: blinker 0.5s linear infinite;
}

@keyframes blinker {  
  50% { opacity: 0.2; }
}
</style>
`

})
export class EmergencyMessage {
  @Input() configurations: any;
  @Input() preview: boolean;
  device: any;
  message: string;
  message1: string;
  message2: string;
  message3: string;
  message4: string;

  private signalRCoreconnection: signalR.HubConnection;

  directionMap = {
    left: 'right',
    right: 'left',
    top: 'bottom',
    bottom: 'top'
  }

  regularstyle = {};
  regularanimStyle = {};

  regularleft: any;

  emergencystyle = {};
  emergencyanimStyle = {};

  emergencyleft: any;

  @ViewChild('el') el: ElementRef;
  @ViewChild('em') em: ElementRef;
  mac_address: any;
  started: any;
  to: NodeJS.Timer;
  locationName: any;
  locationColorTheme: any;

  constructor(private changeDetectorRef: ChangeDetectorRef, private builder: AnimationBuilder, private route: ActivatedRoute, private deviceService: DeviceService, private authService: AuthService,
    private configurationService: ConfigurationService) {

  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
  }

  ngAfterViewInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    if (this.configurations.configurationsObj.textStyle && this.configurations.configurationsObj.textStyle.enableScrolling) this.startAnimation();

    if (this.configurations.configurationsObj.emergencyStyle && this.configurations.configurationsObj.emergencyStyle.enableScrolling) this.startEmergencyAnimation();

    this.route.params.subscribe(queryParams => {
      this.mac_address = queryParams["mac_address"];
    });

    if (!this.mac_address) {
      this.route.queryParams.subscribe(queryParams => {
        this.mac_address = queryParams["mac_address"];
      });
    }

    console.log("MACS", this.mac_address);

    this.startMessage();

    this.deviceService.getDeviceById(null, this.mac_address, true)
      .subscribe(results => {

        this.device = results.data;

        if (this.device) {
          this.locationColorTheme = this.device.locationColorTheme;
          this.locationName = this.device.locationName;

          this.message1 = this.device.emergencyMessage1;
          this.message2 = this.device.emergencyMessage2;
          this.message3 = this.device.emergencyMessage3;
          this.message4 = this.device.emergencyMessage4;

          if (this.to) clearTimeout(this.to);
          this.startMessage();

          this.signalRCoreconnection = this.authService.signalRConnection(this.configurationService.baseUrl + "/hub/frsdevice?device_id=" + this.device.id + "&source=server", true, this.signalRCoreconnection);
          if (this.signalRCoreconnection != null) {
            this.signalRCoreconnection.on("RefreshDeviceData", (deviceVM) => {
              console.log('Refreshing Device Display');

              this.message1 = deviceVM.emergencyMessage1;
              this.message2 = deviceVM.emergencyMessage2;
              this.message3 = deviceVM.emergencyMessage3;
              this.message4 = deviceVM.emergencyMessage4;

              if (this.to) clearTimeout(this.to);
              this.startMessage();
            });
          }
        }

      },
        error => {

        });
  }

  defaultDuration = 30;

  startMessage(queue?: number) {
    queue = queue || 1;
    if (queue > 3) queue = 1;

    if (this.message4) this.message = this.message4;
    else if (this.message1 || this.message2 || this.message3) {
      this.configurations.configurationsObj.message1Duration = this.configurations.configurationsObj.message1Duration || this.defaultDuration;
      this.configurations.configurationsObj.message2Duration = this.configurations.configurationsObj.message2Duration || this.defaultDuration;
      this.configurations.configurationsObj.message3Duration = this.configurations.configurationsObj.message3Duration || this.defaultDuration;

      if (queue == 1) {
        if (this.message1) {
          this.message = this.message1;
        }

        this.setDelay(queue, this.configurations.configurationsObj.message1Duration);
        return;
      }

      if (queue == 2) {
        if (this.message2) {
          this.message = this.message2;
        }

        this.setDelay(queue, this.configurations.configurationsObj.message2Duration);
        return;
      }

      if (queue == 3) {
        if (this.message3) {
          this.message = this.message3;
        }

        this.setDelay(queue, this.configurations.configurationsObj.message3Duration);
        return;
      }
    }

    this.setDelay(queue, this.defaultDuration);
  }

  setDelay(queue, duration) {
    this.to = setTimeout(() => this.startMessage(++queue), duration * 1000);
  }

  private regularfactory;
  private regularplayer;

  private emergencyfactory;
  private emergencyplayer;


  private startAnimation() {
    this.regularleft = this.el.nativeElement.offsetWidth;

    this.regularstyle[this.directionMap[this.configurations.configurationsObj.direction || 'left']] = `-${this.regularleft}px`;
    this.regularanimStyle[this.directionMap[this.configurations.configurationsObj.direction || 'left']] = '100%';

    let s = this.configurations.configurationsObj.speed || 10;

    let ss = s + (this.regularleft / (this.configurations.configurationsObj.width / s));

    this.regularfactory = this.builder.build([
      style(this.regularstyle),
      animate((ss || s) * 1000, style(this.regularanimStyle))
    ]);

    this.regularplayer = this.regularfactory.create(this.el.nativeElement, {});

    this.regularplayer.reset();

    this.regularplayer.onDone(() => {
      this.startAnimation();
    });

    this.regularplayer.play();
  }

  private startEmergencyAnimation() {
    this.emergencyleft = this.em.nativeElement.offsetWidth;

    this.emergencystyle[this.directionMap[this.configurations.configurationsObj.direction || 'left']] = `-${this.emergencyleft}px`;
    this.emergencyanimStyle[this.directionMap[this.configurations.configurationsObj.direction || 'left']] = '100%';

    let s = this.configurations.configurationsObj.speed || 10;

    let ss = s + (this.emergencyleft / (this.configurations.configurationsObj.width / s));

    this.emergencyfactory = this.builder.build([
      style(this.emergencystyle),
      animate((ss || s) * 1000, style(this.emergencyanimStyle))
    ]);

    this.emergencyplayer = this.emergencyfactory.create(this.em.nativeElement, {});

    this.emergencyplayer.reset();

    this.emergencyplayer.onDone(() => {
      this.startEmergencyAnimation();
    });

    this.emergencyplayer.play();
  }
}
