import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { FacilityTypeService } from "../../../services/facility-type.service";
import { FacilityType } from '../../../models/facility-type.model';
import { Permission } from '../../../models/permission.model';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';


@Component({
  selector: 'facility-type-editor',
  templateUrl: './facility-type-editor.component.html',
  styleUrls: ['./facility-type-editor.component.css']
})
export class FacilityTypeEditorComponent {

  private isNewFacilityType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingFacilityTypeName: string;
  private facilityTypeEdit: FacilityType = new FacilityType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private facilityTypeService: FacilityTypeService, private accountService: AccountService,
    public dialogRef: MatDialogRef<FacilityTypeEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.facilityType) != typeof (undefined)) {
      if (data.facilityType.id) {
        this.editFacilityType(data.facilityType);
      } else {
        this.newFacilityType();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewFacilityType) {
      this.facilityTypeEdit.institutionId = this.accountService.currentUser.institutionId;
      this.facilityTypeService.newFacilityType(this.facilityTypeEdit).subscribe(facilityType => this.saveSuccessHelper(facilityType), error => this.saveFailedHelper(error));
    }
    else {
      this.facilityTypeService.updateFacilityType(this.facilityTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(facilityType?: FacilityType) {
    if (facilityType)
      Object.assign(this.facilityTypeEdit, facilityType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewFacilityType)
      this.alertService.showMessage("Success", `Facility Type \"${this.facilityTypeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to facility type \"${this.facilityTypeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.facilityTypeEdit = new FacilityType();
    this.resetForm();


    //if (!this.isNewFacilityType && this.accountService.currentUser.facilities.some(r => r == this.editingFacilityTypeName))
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

    this.dialogRef.close();
  }


  private cancel() {
    this.facilityTypeEdit = new FacilityType();

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


  newFacilityType() {
    this.isNewFacilityType = true;
    this.showValidationErrors = true;

    this.editingFacilityTypeName = null;
    this.selectedValues = {};
    this.facilityTypeEdit = new FacilityType();
    this.facilityTypeEdit.institutionId = this.accountService.currentUser.institutionId;
    return this.facilityTypeEdit;
  }

  editFacilityType(facilityType: FacilityType) {
    if (facilityType) {
      this.isNewFacilityType = false;
      this.showValidationErrors = true;

      this.editingFacilityTypeName = facilityType.name;
      this.selectedValues = {};
      this.facilityTypeEdit = new FacilityType();
      Object.assign(this.facilityTypeEdit, facilityType);

      return this.facilityTypeEdit;
    }
    else {
      return this.newFacilityType();
    }
  }



  get canManageFacilityTypes() {
    return this.accountService.userHasPermission(Permission.manageFacilityTypesPermission)
  }
}
