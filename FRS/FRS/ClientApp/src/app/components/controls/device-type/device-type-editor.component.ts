import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DeviceType } from 'src/app/models/device-type.model';
import { DeviceService } from 'src/app/services/device.service';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'device-type-editor',
  templateUrl: './device-type-editor.component.html',
  styleUrls: ['./device-type-editor.component.css']
})
export class DeviceTypeEditorComponent {

  private isNewDeviceType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingDeviceTypeName: string;
  private deviceTypeEdit: DeviceType = new DeviceType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private deviceService: DeviceService, private accountService: AccountService,
    private fileService: FileService,
    public dialogRef: MatDialogRef<DeviceTypeEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.deviceType) != typeof (undefined)) {
      if (data.deviceType.id) {
        this.editDeviceType(data.deviceType);
      } else {
        this.newDeviceType();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.deviceTypeEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewDeviceType) {
      this.deviceService.newDeviceType(this.deviceTypeEdit).subscribe(deviceType => this.saveSuccessHelper(deviceType), error => this.saveFailedHelper(error));
    }
    else {
      this.deviceService.updateDeviceType(this.deviceTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(deviceType?: DeviceType) {
    if (deviceType)
      Object.assign(this.deviceTypeEdit, deviceType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDeviceType)
      this.alertService.showMessage("Success", `Device Type \"${this.deviceTypeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to device type \"${this.deviceTypeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.deviceTypeEdit = new DeviceType();
    this.resetForm();


    //if (!this.isNewDeviceType && this.accountService.currentUser.facilities.some(r => r == this.editingDeviceTypeName))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }


  private cancel() {
    this.deviceTypeEdit = new DeviceType();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  private toggleGroup(groupName: string) {
    let firstMemberValue: boolean;

    this.allPermissions.forEach(p => {
      if (p.groupName != groupName)
        return;

      if (firstMemberValue == null)
        firstMemberValue = this.selectedValues[p.value] == true;

      this.selectedValues[p.value] = !firstMemberValue;
    });
  }


  private getSelectedPermissions() {
    return this.allPermissions.filter(p => this.selectedValues[p.value] == true);
  }


  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }


  newDeviceType() {
    this.isNewDeviceType = true;
    this.showValidationErrors = true;

    this.editingDeviceTypeName = null;
    this.selectedValues = {};
    this.deviceTypeEdit = new DeviceType();

    return this.deviceTypeEdit;
  }

  editDeviceType(deviceType: DeviceType) {
    if (deviceType) {
      this.isNewDeviceType = false;
      this.showValidationErrors = true;

      this.editingDeviceTypeName = deviceType.name;
      this.selectedValues = {};
      this.deviceTypeEdit = new DeviceType();
      Object.assign(this.deviceTypeEdit, deviceType);

      return this.deviceTypeEdit;
    }
    else {
      return this.newDeviceType();
    }
  }

  get canManageDeviceTypes() {
    return this.accountService.userHasPermission(Permission.manageDeviceTypesPermission)
  }
}
