import { Component, ViewChild, Inject, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { RestrictionType } from 'src/app/models/meal-order/restriction-type.model';
import { RestrictionService } from 'src/app/services/meal-order/restriction.service';


@Component({
  selector: 'restriction-type-editor',
  templateUrl: './restriction-type-editor.component.html',
  styleUrls: ['./restriction-type-editor.component.css']
})
export class RestrictionTypeEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private isNewRestrictionType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingRestrictionTypeCode: string;
  private restrictionTypeEdit: RestrictionType = new RestrictionType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private restrictionService: RestrictionService, private accountService: AccountService,
    public dialogRef: MatDialogRef<RestrictionTypeEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.restrictionType) != typeof (undefined)) {
      if (data.restrictionType.id) {
        this.editRestrictionType(data.restrictionType);
      } else {
        this.newRestrictionType();
      }
    }
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.restrictionTypeEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewRestrictionType) {
      this.restrictionService.newRestrictionType(this.restrictionTypeEdit).subscribe(restrictionType => this.saveSuccessHelper(restrictionType), error => this.saveFailedHelper(error));
    }
    else {
      this.restrictionService.updateRestrictionType(this.restrictionTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(restrictionType?: RestrictionType) {
    if (restrictionType)
      Object.assign(this.restrictionTypeEdit, restrictionType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewRestrictionType)
      this.alertService.showMessage("Success", `Restriction Type \"${this.restrictionTypeEdit.code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to restriction type \"${this.restrictionTypeEdit.code}\" was saved successfully`, MessageSeverity.success);


    this.restrictionTypeEdit = new RestrictionType();
    this.resetForm();


    //if (!this.isNewRestrictionType && this.accountService.currentUser.facilities.some(r => r == this.editingRestrictionTypeCode))
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
    this.restrictionTypeEdit = new RestrictionType();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close({ isCancel: true });
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


  newRestrictionType() {
    this.isNewRestrictionType = true;
    this.showValidationErrors = true;

    this.editingRestrictionTypeCode = null;
    this.selectedValues = {};
    this.restrictionTypeEdit = new RestrictionType();

    return this.restrictionTypeEdit;
  }

  editRestrictionType(restrictionType: RestrictionType) {
    if (restrictionType) {
      this.isNewRestrictionType = false;
      this.showValidationErrors = true;

      this.editingRestrictionTypeCode = restrictionType.code;
      this.selectedValues = {};
      this.restrictionTypeEdit = new RestrictionType();
      Object.assign(this.restrictionTypeEdit, restrictionType);

      return this.restrictionTypeEdit;
    }
    else {
      return this.newRestrictionType();
    }
  }

  get canManageRestrictionTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtRestrictionTypesPermission)
  }
}
