import { Component, ViewChild, Inject, OnInit, OnDestroy } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { CartonType } from 'src/app/models/meal-order/carton-type.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { CatererInfo } from '../../../models/meal-order/caterer-info.model';
import { Subscription } from 'rxjs';


@Component({
  selector: 'carton-type-editor',
  templateUrl: './carton-type-editor.component.html',
  styleUrls: ['./carton-type-editor.component.css']
})
export class CartonTypeEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();

  private isNewCartonType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingCartonTypeCode: string;
  private cartonTypeEdit: CartonType = new CartonType();
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
    public dialogRef: MatDialogRef<CartonTypeEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.cartonType) != typeof (undefined)) {
      if (data.cartonType.id) {
        this.editCartonType(data.cartonType);
      } else {
        this.newCartonType();
      }
    }

    this.getCatererInfos();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
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
    this.cartonTypeEdit.institutionId = this.accountService.currentUser.institutionId;
    console.log(this.periods);
    let selectedPeriods = this.periods.filter(f => f.checked);
    

    if (this.isNewCartonType) {
      this.deliveryService.newCartonType(this.cartonTypeEdit).subscribe(cartonType => this.saveSuccessHelper(cartonType), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateCartonType(this.cartonTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(cartonType?: CartonType) {
    if (cartonType)
      Object.assign(this.cartonTypeEdit, cartonType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewCartonType)
      this.alertService.showMessage("Success", `Carton type \"${this.cartonTypeEdit.code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Carton type \"${this.cartonTypeEdit.code}\" was saved successfully`, MessageSeverity.success);


    this.cartonTypeEdit = new CartonType();
    this.resetForm();


    //if (!this.isNewCartonType && this.accountService.currentUser.facilities.some(r => r == this.editingCartonTypeCode))
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
    this.cartonTypeEdit = new CartonType();

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


  newCartonType() {
    this.isNewCartonType = true;
    this.showValidationErrors = true;

    this.editingCartonTypeCode = null;
    this.selectedValues = {};
    this.cartonTypeEdit = new CartonType();

    return this.cartonTypeEdit;
  }

  editCartonType(cartonType: CartonType) {
    if (cartonType) {
      this.isNewCartonType = false;
      this.showValidationErrors = true;

      this.editingCartonTypeCode = cartonType.code;
      this.selectedValues = {};
      this.cartonTypeEdit = new CartonType();
      Object.assign(this.cartonTypeEdit, cartonType);

      return this.cartonTypeEdit;
    }
    else {
      return this.newCartonType();
    }
  }

  getCatererInfos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.deliveryService.getCatererInfosByFilter(filter)
      .subscribe(results => {
        this.catererInfos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving caterers.\r\n"`,
            MessageSeverity.error);
        }));
  }



  get canManageCartonTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCartonTypesPermission)
  }
}
