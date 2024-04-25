import { Component, ViewChild, Inject, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { BentoAsset } from 'src/app/models/meal-order/bento-asset.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { BentoBoxType } from '../../../models/meal-order/bento-box-type.model';
import { DishService } from '../../../services/meal-order/dish.service';


@Component({
  selector: 'bento-asset-editor',
  templateUrl: './bento-asset-editor.component.html',
  styleUrls: ['./bento-asset-editor.component.css']
})
export class BentoAssetEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private isNewBentoAsset = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingBentoAssetCode: string;
  private bentoAssetEdit: BentoAsset = new BentoAsset();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];

  private bentoBoxTypes: BentoBoxType[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  cartonAssets: any;
  dishs: any;
  storeInfos: any;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private dishService: DishService,  private accountService: AccountService,
    public dialogRef: MatDialogRef<BentoAssetEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.bentoAsset) != typeof (undefined)) {
      if (data.bentoAsset.id) {
        this.editBentoAsset(data.bentoAsset);
      } else {
        this.newBentoAsset();
      }
    }

    this.getBentoBoxTypes();
    this.getCartonAssets();
    this.getDishs();
    this.getStoreInfos();
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
    this.bentoAssetEdit.institutionId = this.accountService.currentUser.institutionId;
    console.log(this.periods);
    let selectedPeriods = this.periods.filter(f => f.checked);
    

    if (this.isNewBentoAsset) {
      this.deliveryService.newBentoAsset(this.bentoAssetEdit).subscribe(bentoAsset => this.saveSuccessHelper(bentoAsset), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateBentoAsset(this.bentoAssetEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(bentoAsset?: BentoAsset) {
    if (bentoAsset)
      Object.assign(this.bentoAssetEdit, bentoAsset);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewBentoAsset)
      this.alertService.showMessage("Success", `Bento Asset \"${this.bentoAssetEdit.code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Bento Asset \"${this.bentoAssetEdit.code}\" was saved successfully`, MessageSeverity.success);


    this.bentoAssetEdit = new BentoAsset();
    this.resetForm();


    //if (!this.isNewBentoAsset && this.accountService.currentUser.facilities.some(r => r == this.editingBentoAssetCode))
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
    this.bentoAssetEdit = new BentoAsset();

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


  newBentoAsset() {
    this.isNewBentoAsset = true;
    this.showValidationErrors = true;

    this.editingBentoAssetCode = null;
    this.selectedValues = {};
    this.bentoAssetEdit = new BentoAsset();

    return this.bentoAssetEdit;
  }

  editBentoAsset(bentoAsset: BentoAsset) {
    if (bentoAsset) {
      this.isNewBentoAsset = false;
      this.showValidationErrors = true;

      this.editingBentoAssetCode = bentoAsset.code;
      this.selectedValues = {};
      this.bentoAssetEdit = new BentoAsset();
      Object.assign(this.bentoAssetEdit, bentoAsset);

      return this.bentoAssetEdit;
    }
    else {
      return this.newBentoAsset();
    }
  }

  getStoreInfos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.deliveryService.getStoreInfosByFilter(filter)
      .subscribe(results => {
        this.storeInfos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving bento box types.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getBentoBoxTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.deliveryService.getBentoBoxTypesByFilter(filter)
      .subscribe(results => {
        this.bentoBoxTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving bento box types.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getCartonAssets() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.deliveryService.getCartonAssetsByFilter(filter)
      .subscribe(results => {
        this.cartonAssets = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving carton assets.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getDishs() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.dishService.getDishesByFilter(filter)
      .subscribe(results => {
        this.dishs = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dishs.\r\n"`,
            MessageSeverity.error);
        }));
  }


  get canManageBentoAssets() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtBentoAssetsPermission)
  }
}
