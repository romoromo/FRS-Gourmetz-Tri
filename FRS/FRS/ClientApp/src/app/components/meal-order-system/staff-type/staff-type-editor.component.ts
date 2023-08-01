import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { StaffType } from 'src/app/models/meal-order/staff-type.model';
import { UserService } from 'src/app/services/meal-order/user.service';


@Component({
  selector: 'staff-type-editor',
  templateUrl: './staff-type-editor.component.html',
  styleUrls: ['./staff-type-editor.component.css']
})
export class StaffTypeEditorComponent {

  private isNewStaffType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingStaffTypeName: string;
  private staffTypeEdit: StaffType = new StaffType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private userService: UserService, private accountService: AccountService,
    public dialogRef: MatDialogRef<StaffTypeEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.staffType) != typeof (undefined)) {
      if (data.staffType.id) {
        this.editStaffType(data.staffType);
      } else {
        this.newStaffType();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.staffTypeEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewStaffType) {
      this.userService.newStaffType(this.staffTypeEdit).subscribe(staffType => this.saveSuccessHelper(staffType), error => this.saveFailedHelper(error));
    }
    else {
      this.userService.updateStaffType(this.staffTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(staffType?: StaffType) {
    if (staffType)
      Object.assign(this.staffTypeEdit, staffType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewStaffType)
      this.alertService.showMessage("Success", `User Type \"${this.staffTypeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to user type \"${this.staffTypeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.staffTypeEdit = new StaffType();
    this.resetForm();


    //if (!this.isNewStaffType && this.accountService.currentUser.facilities.some(r => r == this.editingStaffTypeName))
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
    this.staffTypeEdit = new StaffType();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
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


  newStaffType() {
    this.isNewStaffType = true;
    this.showValidationErrors = true;

    this.editingStaffTypeName = null;
    this.selectedValues = {};
    this.staffTypeEdit = new StaffType();

    return this.staffTypeEdit;
  }

  editStaffType(staffType: StaffType) {
    if (staffType) {
      this.isNewStaffType = false;
      this.showValidationErrors = true;

      this.editingStaffTypeName = staffType.name;
      this.selectedValues = {};
      this.staffTypeEdit = new StaffType();
      Object.assign(this.staffTypeEdit, staffType);

      return this.staffTypeEdit;
    }
    else {
      return this.newStaffType();
    }
  }

  get canManageStaffTypes() {
    return true; //this.accountService.userHasPermission(Permission.manageStaffTypesPermission)
  }
}
