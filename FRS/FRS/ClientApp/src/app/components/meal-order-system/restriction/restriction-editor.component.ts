import { Component, ViewChild, Inject, OnInit, OnDestroy } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Restriction } from 'src/app/models/meal-order/restriction.model';
import { RestrictionService } from 'src/app/services/meal-order/restriction.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { Subscription } from 'rxjs';


@Component({
  selector: 'restriction-editor',
  templateUrl: './restriction-editor.component.html',
  styleUrls: ['./restriction-editor.component.css']
})
export class RestrictionEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private isNewRestriction = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingRestrictionCode: string;
  private restrictionEdit: Restriction = new Restriction();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public restrictionTypes = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private restrictionService: RestrictionService, private accountService: AccountService,
    public dialogRef: MatDialogRef<RestrictionEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.restriction) != typeof (undefined) && data.restriction.id) {
      this.editRestriction(data.restriction);
    } else {
      this.newRestriction();
    }

    this.getRestrictionTypes();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  getRestrictionTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.restrictionService.getRestrictionTypesByFilter(filter)
      .subscribe(results => {
        this.restrictionTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving restriction types.\r\n"`,
            MessageSeverity.error);
        }));
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    
    if (this.isNewRestriction) {
      this.restrictionService.newRestriction(this.restrictionEdit).subscribe(restriction => this.saveSuccessHelper(restriction), error => this.saveFailedHelper(error));
    }
    else {
      this.restrictionService.updateRestriction(this.restrictionEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  private invalidate() {
    return false;
  }

  private saveSuccessHelper(restriction?: Restriction) {
    if (restriction)
      Object.assign(this.restrictionEdit, restriction);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewRestriction)
      this.alertService.showMessage("Success", `Restriction \"${this.restrictionEdit.code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to restriction \"${this.restrictionEdit.code}\" was saved successfully`, MessageSeverity.success);


    this.restrictionEdit = new Restriction();
    this.resetForm();


    //if (!this.isNewRestriction && this.accountService.currentUser.facilities.some(r => r == this.editingRestrictionCode))
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
    this.restrictionEdit = new Restriction();

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


  newRestriction() {
    this.isNewRestriction = true;
    this.showValidationErrors = true;

    this.editingRestrictionCode = null;
    this.selectedValues = {};
    this.restrictionEdit = new Restriction();

    return this.restrictionEdit;
  }

  editRestriction(restriction: Restriction) {
    if (restriction) {
      this.isNewRestriction = false;
      this.showValidationErrors = true;

      this.editingRestrictionCode = restriction.code;
      this.selectedValues = {};
      this.restrictionEdit = new Restriction();
      Object.assign(this.restrictionEdit, restriction);

      return this.restrictionEdit;
    }
    else {
      return this.newRestriction();
    }
  }

  get canManageRestrictions() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtRestrictionsPermission)
  }
}
