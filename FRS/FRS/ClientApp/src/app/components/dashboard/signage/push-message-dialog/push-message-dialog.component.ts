import { Component, ViewChild, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { SignageDashboardFilter } from 'src/app/models/dashboard.model';
import { DevicePushMessage } from 'src/app/models/device.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { DeviceService } from 'src/app/services/device.service';
import { Utilities } from 'src/app/services/utilities';


@Component({
  selector: 'device-push-message-dialog',
  templateUrl: './push-message-dialog.component.html',
  styleUrls: ['./push-message-dialog.component.css']
})
export class DevicePushMessageDialog {

  private isNewDepartment = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingDepartmentName: string;
  private message: DevicePushMessage = new DevicePushMessage();

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  sgnFilter: SignageDashboardFilter;
  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private deviceService: DeviceService, 
    public dialogRef: MatDialogRef<DevicePushMessageDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (data && data.sgnFilter) {
      this.sgnFilter = data.sgnFilter;
    }

  }
  private pushMessage() {
    let dto = new DevicePushMessage();
    dto.filter = this.sgnFilter;
    dto.emergencyMessage1 = this.message.emergencyMessage1;
    dto.emergencyMessage2 = this.message.emergencyMessage2;
    dto.emergencyMessage3 = this.message.emergencyMessage3;
    dto.emergencyMessage4 = this.message.emergencyMessage4;

    this.alertService.startLoadingMessage("Pushing messages to the devices...");

    this.deviceService.pushMessages(dto)
      .subscribe(results => {
        console.log(results);
        this.alertService.stopLoadingMessage();
        if (results.isSuccess) {
          this.alertService.showMessage("Successfully pushed to all devices!", "", MessageSeverity.success);
        } else {
          this.alertService.showMessage(results.message, "", MessageSeverity.error);
        }

        this.dialogRef.close(results);
      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Push Message Error", `An error occured while pusing the messages to the devices.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
    
  }


  private cancel() {
    this.dialogRef.close();
  }
}
