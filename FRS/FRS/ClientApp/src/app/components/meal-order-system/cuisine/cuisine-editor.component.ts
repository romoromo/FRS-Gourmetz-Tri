import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Cuisine } from 'src/app/models/meal-order/cuisine.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { DishService } from '../../../services/meal-order/dish.service';


@Component({
  selector: 'cuisine-editor',
  templateUrl: './cuisine-editor.component.html',
  styleUrls: ['./cuisine-editor.component.css']
})
export class CuisineEditorComponent {

  private isNewCuisine = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingCuisineCode: string;
  private cuisineEdit: Cuisine = new Cuisine();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private dishService: DishService, private accountService: AccountService,
    public dialogRef: MatDialogRef<CuisineEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.cuisine) != typeof (undefined)) {
      if (data.cuisine.id) {
        this.editCuisine(data.cuisine);
      } else {
        this.newCuisine();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewCuisine) {
      this.dishService.newCuisine(this.cuisineEdit).subscribe(cuisine => this.saveSuccessHelper(cuisine), error => this.saveFailedHelper(error));
    }
    else {
      this.dishService.updateCuisine(this.cuisineEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(cuisine?: Cuisine) {
    if (cuisine)
      Object.assign(this.cuisineEdit, cuisine);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewCuisine)
      this.alertService.showMessage("Success", `\"${this.cuisineEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.cuisineEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.cuisineEdit = new Cuisine();
    this.resetForm();

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
    this.cuisineEdit = new Cuisine();

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


  newCuisine() {
    this.isNewCuisine = true;
    this.showValidationErrors = true;

    this.editingCuisineCode = null;
    this.selectedValues = {};
    this.cuisineEdit = new Cuisine();

    return this.cuisineEdit;
  }

  editCuisine(cuisine: Cuisine) {
    if (cuisine) {
      this.isNewCuisine = false;
      this.showValidationErrors = true;

      this.editingCuisineCode = cuisine.name;
      this.selectedValues = {};
      this.cuisineEdit = new Cuisine();
      Object.assign(this.cuisineEdit, cuisine);

      return this.cuisineEdit;
    }
    else {
      return this.newCuisine();
    }
  }



  get canManageCuisines() {
    return true; //this.accountService.userHasPermission(Permission.manageCuisinesPermission)
  }
}
