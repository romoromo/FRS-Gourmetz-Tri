import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { CatererInfo } from '../../../models/meal-order/caterer-info.model';


@Component({
  selector: 'store-info-editor',
  templateUrl: './store-info-editor.component.html',
  styleUrls: ['./store-info-editor.component.css']
})
export class StoreInfoEditorComponent {

  private isNewStoreInfo = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingStoreInfoCode: string;
  private storeInfoEdit: StoreInfo = new StoreInfo();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];
  private catererInfos: CatererInfo[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<StoreInfoEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.storeInfo) != typeof (undefined)) {
      if (data.storeInfo.id) {
        this.editStoreInfo(data.storeInfo);
      } else {
        this.newStoreInfo();
      }
    }

    this.getCatererInfos();
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.storeInfoEdit.institutionId = this.accountService.currentUser.institutionId;
    console.log(this.periods);
    let selectedPeriods = this.periods.filter(f => f.checked);
    

    if (this.isNewStoreInfo) {
      this.deliveryService.newStoreInfo(this.storeInfoEdit).subscribe(storeInfo => this.saveSuccessHelper(storeInfo), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateStoreInfo(this.storeInfoEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(storeInfo?: StoreInfo) {
    if (storeInfo)
      Object.assign(this.storeInfoEdit, storeInfo);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewStoreInfo)
      this.alertService.showMessage("Success", `Store Info \"${this.storeInfoEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Store Info \"${this.storeInfoEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.storeInfoEdit = new StoreInfo();
    this.resetForm();


    //if (!this.isNewStoreInfo && this.accountService.currentUser.facilities.some(r => r == this.editingStoreInfoCode))
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
    this.storeInfoEdit = new StoreInfo();

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


  newStoreInfo() {
    this.isNewStoreInfo = true;
    this.showValidationErrors = true;

    this.editingStoreInfoCode = null;
    this.selectedValues = {};
    this.storeInfoEdit = new StoreInfo();

    return this.storeInfoEdit;
  }

  editStoreInfo(storeInfo: StoreInfo) {
    if (storeInfo) {
      this.isNewStoreInfo = false;
      this.showValidationErrors = true;

      this.editingStoreInfoCode = storeInfo.name;
      this.selectedValues = {};
      this.storeInfoEdit = new StoreInfo();
      Object.assign(this.storeInfoEdit, storeInfo);

      return this.storeInfoEdit;
    }
    else {
      return this.newStoreInfo();
    }
  }

  getCatererInfos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getCatererInfosSimpleByFilter(filter)
      .subscribe(results => {
        this.catererInfos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving caterers.\r\n"`,
            MessageSeverity.error);
        })
  }



  get canManageStoreInfos() {
    return true; //this.accountService.userHasPermission(Permission.manageStoreInfosPermission)
  }
}
