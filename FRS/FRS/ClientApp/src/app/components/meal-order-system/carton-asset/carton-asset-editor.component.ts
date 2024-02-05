import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { CartonAsset } from 'src/app/models/meal-order/carton-asset.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { CartonType } from '../../../models/meal-order/carton-type.model';
import { DishService } from '../../../services/meal-order/dish.service';


@Component({
  selector: 'carton-asset-editor',
  templateUrl: './carton-asset-editor.component.html',
  styleUrls: ['./carton-asset-editor.component.css']
})
export class CartonAssetEditorComponent {

  private isNewCartonAsset = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingCartonAssetCode: string;
  private cartonAssetEdit: CartonAsset = new CartonAsset();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];

  private cartonTypes: CartonType[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  dishs: any;
  storeInfos: any;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<CartonAssetEditorComponent>, private mealService: MealService, private dishService: DishService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.cartonAsset) != typeof (undefined)) {
      if (data.cartonAsset.id) {
        this.editCartonAsset(data.cartonAsset);
      } else {
        this.newCartonAsset();
      }
    }

    this.getCartonTypes();
    this.getDishs();
    this.getStoreInfos();
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.cartonAssetEdit.institutionId = this.accountService.currentUser.institutionId;
    console.log(this.periods);
    let selectedPeriods = this.periods.filter(f => f.checked);
    

    if (this.isNewCartonAsset) {
      this.deliveryService.newCartonAsset(this.cartonAssetEdit).subscribe(cartonAsset => this.saveSuccessHelper(cartonAsset), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateCartonAsset(this.cartonAssetEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(cartonAsset?: CartonAsset) {
    if (cartonAsset)
      Object.assign(this.cartonAssetEdit, cartonAsset);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewCartonAsset)
      this.alertService.showMessage("Success", `Carton Asset \"${this.cartonAssetEdit.code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Carton Asset \"${this.cartonAssetEdit.code}\" was saved successfully`, MessageSeverity.success);


    this.cartonAssetEdit = new CartonAsset();
    this.resetForm();


    //if (!this.isNewCartonAsset && this.accountService.currentUser.facilities.some(r => r == this.editingCartonAssetCode))
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
    this.cartonAssetEdit = new CartonAsset();

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


  newCartonAsset() {
    this.isNewCartonAsset = true;
    this.showValidationErrors = true;

    this.editingCartonAssetCode = null;
    this.selectedValues = {};
    this.cartonAssetEdit = new CartonAsset();

    return this.cartonAssetEdit;
  }

  editCartonAsset(cartonAsset: CartonAsset) {
    if (cartonAsset) {
      this.isNewCartonAsset = false;
      this.showValidationErrors = true;

      this.editingCartonAssetCode = cartonAsset.code;
      this.selectedValues = {};
      this.cartonAssetEdit = new CartonAsset();
      Object.assign(this.cartonAssetEdit, cartonAsset);

      return this.cartonAssetEdit;
    }
    else {
      return this.newCartonAsset();
    }
  }

  getCartonTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getCartonTypesByFilter(filter)
      .subscribe(results => {
        this.cartonTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving carton types.\r\n"`,
            MessageSeverity.error);
        })
  }

  getStoreInfos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getStoreInfosByFilter(filter)
      .subscribe(results => {
        this.storeInfos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving bento box types.\r\n"`,
            MessageSeverity.error);
        })
  }

  getDishs() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.dishService.getDishesByFilter(filter)
      .subscribe(results => {
        this.dishs = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dishs.\r\n"`,
            MessageSeverity.error);
        })
  }



  get canManageCartonAssets() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCartonAssetsPermission)
  }
}
