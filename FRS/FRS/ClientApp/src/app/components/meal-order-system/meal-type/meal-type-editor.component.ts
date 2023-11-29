import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealType, MealTypeDish } from 'src/app/models/meal-order/meal-type.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { Dish } from 'src/app/models/meal-order/dish.model';
import { Cuisine } from 'src/app/models/meal-order/cuisine.model';


@Component({
  selector: 'meal-type-editor',
  templateUrl: './meal-type-editor.component.html',
  styleUrls: ['./meal-type-editor.component.css']
})
export class MealTypeEditorComponent {

  private isNewMealType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingMealTypeCode: string;
  private mealTypeEdit: MealType = new MealType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private dishes: Dish[] = [];
  private cuisines: Cuisine[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public catererId: string;

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private mealService: MealService, private accountService: AccountService,
    public dialogRef: MatDialogRef<MealTypeEditorComponent>, private dishService: DishService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.mealType) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.mealType.id) {
        this.editMealType(data.mealType);
      } else {
        this.newMealType();
      }
    }

    this.getDishes();
    this.getCuisines();
  }

  getCuisines() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.dishService.getCuisinesByFilter(filter)
      .subscribe(results => {
        this.cuisines = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving cuisines.\r\n"`,
            MessageSeverity.error);
        })
  }

  getDishes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.dishService.getDishesByFilter(filter)
      .subscribe(results => {
        this.dishes = results.pagedData;
        this.dishes.forEach((p, index, ps) => {
          (<any>p).checked = this.mealTypeEdit.dishes != null && this.mealTypeEdit.dishes.findIndex(f => f.dishId == p.id) > -1;
        });
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dishes.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.mealTypeEdit.dishes = [];
    this.dishes.forEach((p, index, ps) => {
      if (p.checked) {
        let dtDish = new MealTypeDish();
        dtDish.mealTypeId = this.mealTypeEdit.id;
        dtDish.dishId = p.id;
        this.mealTypeEdit.dishes.push(dtDish);
      }

    });

    this.mealTypeEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewMealType) {
      this.mealService.newMealType(this.mealTypeEdit).subscribe(mealType => this.saveSuccessHelper(mealType), error => this.saveFailedHelper(error));
    }
    else {
      this.mealService.updateMealType(this.mealTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(mealType?: MealType) {
    if (mealType)
      Object.assign(this.mealTypeEdit, mealType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewMealType)
      this.alertService.showMessage("Success", `Meal Type \"${this.mealTypeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to meal type \"${this.mealTypeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.mealTypeEdit = new MealType();
    this.resetForm();


    //if (!this.isNewMealType && this.accountService.currentUser.facilities.some(r => r == this.editingMealTypeCode))
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
    this.mealTypeEdit = new MealType();

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


  newMealType() {
    this.isNewMealType = true;
    this.showValidationErrors = true;

    this.editingMealTypeCode = null;
    this.selectedValues = {};
    this.mealTypeEdit = new MealType();
    this.mealTypeEdit.catererId = this.catererId;
    return this.mealTypeEdit;
  }

  editMealType(mealType: MealType) {
    if (mealType) {
      this.isNewMealType = false;
      this.showValidationErrors = true;

      this.editingMealTypeCode = mealType.name;
      this.selectedValues = {};
      this.mealTypeEdit = new MealType();
      Object.assign(this.mealTypeEdit, mealType);

      return this.mealTypeEdit;
    }
    else {
      return this.newMealType();
    }
  }

  get canManageMealTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCatererMealTypesMenu)
  }
}
