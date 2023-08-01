import { Component, Input, ChangeDetectorRef } from '@angular/core'
import { ActivatedRoute } from '@angular/router';
import { EmployeeData } from '../../../../models/employee-data.model';
import { SignageComponent } from '../../../../models/signage-component.model';
import { AccountService } from '../../../../services/account.service';
import { AuthService } from '../../../../services/auth.service';
import { ConfigurationService } from '../../../../services/configuration.service';
import { DeviceService } from '../../../../services/device.service';
import { EmployeeDataService } from '../../../../services/employee-data.service';

@Component({
  selector: 'employee-name',
  template: `<div
[style.position]="'absolute'"

[style.display]="'flex'"
[style.align-items]="configurations.configurationsObj.name.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.name.vAlign == 'top' ? 'flex-start' : 'flex-end')"

[style.font-family]="configurations.configurationsObj.name.fontFamily"
[style.font-size.px]="configurations.configurationsObj.name.textSize"
[style.color]="configurations.configurationsObj.name.textColor"
[style.left.px]="configurations.configurationsObj.name.x"
[style.top.px]="configurations.configurationsObj.name.y"
[style.width.px]="configurations.configurationsObj.name.width"
[style.height.px]="configurations.configurationsObj.name.height"
[style.background-color]="configurations.configurationsObj.name.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.name.fieldColor"
[style.font-weight]="configurations.configurationsObj.name.bold ? 'bold' : 'normal'"
[style.text-decoration]="configurations.configurationsObj.name.underline ? 'underline' : 'none'"
[style.font-style]="configurations.configurationsObj.name.italic ? 'italic' : 'normal'"
>
<div [style.width.%]="100" [style.text-align]="configurations.configurationsObj.name.textAlign">
<span *ngIf="preview">Employee Name</span>
<span *ngIf="!preview">
  <span *ngFor="let e of employees; let i = index"><span *ngIf="i > 0"> | </span>{{e.displayName || e.name}}</span>
</span>
</div>
</div>`

})
export class EmployeeName {
  @Input() configurations: any;
  @Input() preview: boolean;

  mac_address: any;

  private signalRCoreconnection: signalR.HubConnection;
  device: any;
  locationColorTheme: any;
  locationName: any;
  employees: EmployeeData[];

  constructor(private changeDetectorRef: ChangeDetectorRef, private authService: AuthService, private route: ActivatedRoute, private deviceService: DeviceService, private employeeDataService: EmployeeDataService, private configurationService: ConfigurationService) {

  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    console.log('employee')

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

      this.start();
    }
  }

  start() {
    this.getEmployeeName();
    
    setTimeout(() => {
      this.start();
    }, 5 * 60 * 1000);
  }

  getEmployeeName() {
    if (!this.device || !this.device.locationId) return;

    this.employeeDataService.getCurrentEmployeeDataByLocation(this.device.locationId)
      .subscribe(results => {
        this.employees = results;

      },
        error => {
        });
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

            this.getEmployeeName();

            this.signalRCoreconnection = this.authService.signalRConnection(this.configurationService.baseUrl + "/hub/employeeschedule?location_id=" + this.device.locationId, true, this.signalRCoreconnection);
            if (this.signalRCoreconnection != null) {
              this.signalRCoreconnection.on("UpdateSchedule", (param) => {
                this.getEmployeeName();
              });
            }
          }
        });
    }
  }
}
