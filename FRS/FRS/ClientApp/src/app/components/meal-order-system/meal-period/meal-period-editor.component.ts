import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { DateAdapter, MatDatepickerInputEvent, MatDialogRef, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MAT_DIALOG_DATA } from '@angular/material';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';


@Component({
  selector: 'meal-period-editor',
  templateUrl: './meal-period-editor.component.html',
  styleUrls: ['./meal-period-editor.component.css'],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class MealPeriodEditorComponent {

  private isNewMealPeriod = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingMealPeriodCode: string;
  private mealPeriodEdit: MealPeriod = new MealPeriod();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  start = new Date();
  end = new Date();
  outletProfiles = [];
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public catererId: string;

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private mealService: MealService, private accountService: AccountService,
    public dialogRef: MatDialogRef<MealPeriodEditorComponent>,private deliveryService: DeliveryService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.mealPeriod) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.mealPeriod.id) {
        this.editMealPeriod(data.mealPeriod);
      } else {
        this.newMealPeriod();
      }
    }

    this.getOutletProfiles();
  }


  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
      this.mealPeriodEdit.startDate = new Date(event.value);
    }
    if (type == 'end') {
      this.end = new Date(event.value);
      this.mealPeriodEdit.endDate = new Date(event.value);
    }
  }

  getOutletProfiles() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.deliveryService.getOutletProfilesByFilter(filter)
      .subscribe(results => {
        this.outletProfiles = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving outlets.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.mealPeriodEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewMealPeriod) {
      this.mealService.newMealPeriod(this.mealPeriodEdit).subscribe(mealPeriod => this.saveSuccessHelper(mealPeriod), error => this.saveFailedHelper(error));
    }
    else {
      this.mealService.updateMealPeriod(this.mealPeriodEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(mealPeriod?: MealPeriod) {
    if (mealPeriod)
      Object.assign(this.mealPeriodEdit, mealPeriod);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewMealPeriod)
      this.alertService.showMessage("Success", `Restriction Type \"${this.mealPeriodEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.mealPeriodEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.mealPeriodEdit = new MealPeriod();
    this.resetForm();


    //if (!this.isNewMealPeriod && this.accountService.currentUser.facilities.some(r => r == this.editingMealPeriodCode))
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
    this.mealPeriodEdit = new MealPeriod();

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


  newMealPeriod() {
    this.isNewMealPeriod = true;
    this.showValidationErrors = true;

    this.editingMealPeriodCode = null;
    this.selectedValues = {};
    this.mealPeriodEdit = new MealPeriod();
    this.start = new Date();
    this.end = new Date();
    this.mealPeriodEdit.startDate = this.start;
    this.mealPeriodEdit.endDate = this.end;
    this.mealPeriodEdit.catererId = this.catererId;
    return this.mealPeriodEdit;
  }

  editMealPeriod(mealPeriod: MealPeriod) {
    if (mealPeriod) {
      this.isNewMealPeriod = false;
      this.showValidationErrors = true;

      this.editingMealPeriodCode = mealPeriod.name;
      this.selectedValues = {};
      this.mealPeriodEdit = new MealPeriod();
      Object.assign(this.mealPeriodEdit, mealPeriod);
      this.start = this.mealPeriodEdit.startDate;
      this.end = this.mealPeriodEdit.endDate;
      return this.mealPeriodEdit;
    }
    else {
      return this.newMealPeriod();
    }
  }

  get canManageMealPeriods() {
    return true; //this.accountService.userHasPermission(Permission.manageMealPeriodsPermission)
  }
}
