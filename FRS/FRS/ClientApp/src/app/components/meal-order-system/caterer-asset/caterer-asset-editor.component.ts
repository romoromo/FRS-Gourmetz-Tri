import { Component, ViewChild, Inject, OnInit, OnDestroy, Input } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { DishService } from '../../../services/meal-order/dish.service';
import { Subscription } from 'rxjs';
import { CatererAssetType } from 'src/app/models/meal-order/asset-type.model';
import { CatererAsset } from 'src/app/models/meal-order/caterer-asset.model';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'caterer-asset-editor',
  templateUrl: './caterer-asset-editor.component.html',
  styleUrls: ['./caterer-asset-editor.component.css']
})
export class CatererAssetEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();

  private isNewCatererAsset = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private catererAssetEdit: CatererAsset = new CatererAsset();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];

  private catererAssetTypes: CatererAssetType[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  dishs: any;
  storeInfos: any;


  @ViewChild('f')
  private form;

  private catererId: string;
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<CatererAssetEditorComponent>, private mealService: MealService, private dishService: DishService,  private fileService: FileService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.catererAsset) != typeof (undefined)) {
      if(data.catererAsset.catererId)
        this.catererId = data.catererAsset.catererId;
      if (data.catererAsset.id) {
        this.editCatererAsset(data.catererAsset);
      } else {
        this.newCatererAsset();
      }
    }

    this.getCatererTypes();
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
    if (this.isNewCatererAsset) {
      this.deliveryService.newCatererAsset(this.catererAssetEdit).subscribe(catererAsset => this.saveSuccessHelper(catererAsset), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateCatererAsset(this.catererAssetEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(catererAsset?: CatererAsset) {
    if (catererAsset)
      Object.assign(this.catererAssetEdit, catererAsset);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewCatererAsset)
      this.alertService.showMessage("Success", `Caterer Asset \"${this.catererAssetEdit.assetQRCode}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Caterer Asset \"${this.catererAssetEdit.assetQRCode}\" was saved successfully`, MessageSeverity.success);


    this.catererAssetEdit = new CatererAsset();
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
    this.catererAssetEdit = new CatererAsset();

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


  newCatererAsset() {
    this.isNewCatererAsset = true;
    this.showValidationErrors = true;

    this.selectedValues = {};
    this.catererAssetEdit = new CatererAsset();

    this.deliveryService.getAssetQRCode(this.catererId)
    .subscribe(response => {
      this.catererAssetEdit.assetQRCode = response.result;
    })

    return this.catererAssetEdit;
  }

  editCatererAsset(catererAsset: CatererAsset) {
    if (catererAsset) {
      this.isNewCatererAsset = false;
      this.showValidationErrors = true;

      this.selectedValues = {};
      this.catererAssetEdit = new CatererAsset();
      Object.assign(this.catererAssetEdit, catererAsset);

      return this.catererAssetEdit;
    }
    else {
      return this.newCatererAsset();
    }
  }

  getCatererTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true'+ ',(CatererInfoId)==' + this.catererId;
    this.subscription.add(this.dishService.getAssetTypeByFilter(filter)
      .subscribe(results => {
        this.catererAssetTypes = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving caterer types.\r\n"`,
            MessageSeverity.error);
        }));
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.catererAssetEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  removePhoto() {
    this.catererAssetEdit.filePath = null;
    this.catererAssetEdit.fileId = null;
    this.catererAssetEdit.fileName = null;
  }

}
