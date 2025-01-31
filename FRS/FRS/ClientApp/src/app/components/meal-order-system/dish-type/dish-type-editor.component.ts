import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DishType, DishTypePeriod } from 'src/app/models/meal-order/dish-type.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { NgForm } from '@angular/forms';


@Component({
  selector: 'dish-type-editor',
  templateUrl: './dish-type-editor.component.html',
  styleUrls: ['./dish-type-editor.component.css']
})
export class DishTypeEditorComponent {

  private isNewDishType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingDishTypeCode: string;
  private dishTypeEdit: DishType = new DishType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form: NgForm;

  constructor(private alertService: AlertService, private dishService: DishService, private accountService: AccountService,
    public dialogRef: MatDialogRef<DishTypeEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.dishType) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.dishType.id) {
        this.editDishType(data.dishType);
      } else {
        this.newDishType();
      }
    }

    this.getMealPeriods();
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save(formData: NgForm) {
    if (formData.form.invalid) {
      const dishTypeName = formData.form.value.dishTypeName;
      const orderNumber = formData.form.value.orderNumber;
      if (!dishTypeName) {
        this.alertService.showMessage('Name is required', 'Please enter name (minimum of 2 and maximum of 200 characters)', MessageSeverity.error);
      } else if (!orderNumber) {
        this.alertService.showMessage('Order Number is required', 'Please enter order number', MessageSeverity.error);
      }
    } else {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");
      this.dishTypeEdit.institutionId = this.accountService.currentUser.institutionId;
      let selectedPeriods = this.periods.filter(f => f.checked);
      this.dishTypeEdit.dishTypePeriods = [];
      this.periods.forEach((p, index, ps) => {
        if (p.checked) {
          let dtPeriod = new DishTypePeriod();
          dtPeriod.dishTypeId = this.dishTypeEdit.id;
          dtPeriod.periodId = p.id;
          this.dishTypeEdit.dishTypePeriods.push(dtPeriod);
        }

      });
      if (this.isNewDishType) {
        this.dishService.newDishType(this.dishTypeEdit).subscribe(dishType => this.saveSuccessHelper(dishType), error => this.saveFailedHelper(error));
      }
      else {
        this.dishService.updateDishType(this.dishTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
    }
  }


  private saveSuccessHelper(dishType?: DishType) {
    if (dishType)
      Object.assign(this.dishTypeEdit, dishType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDishType)
      this.alertService.showMessage("Success", `Dish Type \"${this.dishTypeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to dish type \"${this.dishTypeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.dishTypeEdit = new DishType();
    this.resetForm();


    //if (!this.isNewDishType && this.accountService.currentUser.facilities.some(r => r == this.editingDishTypeCode))
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
    this.dishTypeEdit = new DishType();

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


  newDishType() {
    this.isNewDishType = true;
    this.showValidationErrors = true;

    this.editingDishTypeCode = null;
    this.selectedValues = {};
    this.dishTypeEdit = new DishType();
    this.dishTypeEdit.catererId = this.catererId;
    return this.dishTypeEdit;
  }

  editDishType(dishType: DishType) {
    if (dishType) {
      this.isNewDishType = false;
      this.showValidationErrors = true;

      this.editingDishTypeCode = dishType.name;
      this.selectedValues = {};
      this.dishTypeEdit = new DishType();
      Object.assign(this.dishTypeEdit, dishType);

      return this.dishTypeEdit;
    }
    else {
      return this.newDishType();
    }
  }

  getMealPeriods() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.mealService.getMealPeriodsByFilter(filter)
      .subscribe(results => {
        this.periods = results.pagedData;
        this.periods.forEach((p, index, ps) => {
          (<any>p).checked = this.dishTypeEdit.dishTypePeriods != null && this.dishTypeEdit.dishTypePeriods.findIndex(f => f.periodId == p.id) > -1;
        });
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving cperiods.\r\n"`,
            MessageSeverity.error);
        })
  }

  get canManageDishTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCatererDishTypesMenu)
  }
}
