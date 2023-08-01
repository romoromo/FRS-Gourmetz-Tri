import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { OutletProfile } from 'src/app/models/meal-order/outlet.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';


@Component({
  selector: 'outlet-profile-editor',
  templateUrl: './outlet-profile-editor.component.html',
  styleUrls: ['./outlet-profile-editor.component.css']
})
export class OutletProfileEditorComponent {

  private isNewOutletProfile = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingOutletProfileCode: string;
  private outletProfileEdit: OutletProfile = new OutletProfile();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<OutletProfileEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outletProfile) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.outletProfile.id) {
        this.editOutletProfile(data.outletProfile);
      } else {
        this.newOutletProfile();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewOutletProfile) {
      this.deliveryService.newOutletProfile(this.outletProfileEdit).subscribe(outletProfile => this.saveSuccessHelper(outletProfile), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateOutletProfile(this.outletProfileEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(outletProfile?: OutletProfile) {
    if (outletProfile)
      Object.assign(this.outletProfileEdit, outletProfile);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewOutletProfile)
      this.alertService.showMessage("Success", `Outlet \"${this.outletProfileEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.outletProfileEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.outletProfileEdit = new OutletProfile();
    this.resetForm();


    //if (!this.isNewOutletProfile && this.accountService.currentUser.facilities.some(r => r == this.editingOutletProfileCode))
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
    this.outletProfileEdit = new OutletProfile();

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


  newOutletProfile() {
    this.isNewOutletProfile = true;
    this.showValidationErrors = true;

    this.editingOutletProfileCode = null;
    this.selectedValues = {};
    this.outletProfileEdit = new OutletProfile();
    this.outletProfileEdit.catererId = this.catererId;
    return this.outletProfileEdit;
  }

  editOutletProfile(outletProfile: OutletProfile) {
    if (outletProfile) {
      this.isNewOutletProfile = false;
      this.showValidationErrors = true;

      this.editingOutletProfileCode = outletProfile.label;
      this.selectedValues = {};
      this.outletProfileEdit = new OutletProfile();
      Object.assign(this.outletProfileEdit, outletProfile);

      return this.outletProfileEdit;
    }
    else {
      return this.newOutletProfile();
    }
  }



  get canManageOutletProfiles() {
    return true; //this.accountService.userHasPermission(Permission.manageOutletProfilesPermission)
  }
}
