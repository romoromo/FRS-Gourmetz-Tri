import {
  Component,
  ViewChild,
  Inject,
  OnInit,
  OnDestroy,
  Input,
} from "@angular/core";

import { AlertService, MessageSeverity } from "../../../services/alert.service";
import { AccountService } from "../../../services/account.service";
import { Permission } from "../../../models/permission.model";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { MealService } from "src/app/services/meal-order/meal.service";
import { CatererAssetFilter, Filter } from "src/app/models/sieve-filter.model";
import { MealPeriod } from "src/app/models/meal-order/meal-period.model";
import { StaffService } from "../../../services/meal-order/staff.service";
import { DeliveryService } from "../../../services/meal-order/delivery.service";
import { DishService } from "../../../services/meal-order/dish.service";
import { Subscription } from "rxjs";
import { FileService } from "src/app/services/file.service";
import { AssetComponent } from "src/app/models/meal-order/asset-component.model";
import { CatererAsset } from "src/app/models/meal-order/caterer-asset.model";

@Component({
  selector: "asset-component-editor",
  templateUrl: "./asset-component-editor.component.html",
  styleUrls: ["./asset-component-editor.component.css"],
})
export class AssetComponentEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();

  private isNewAssetComponent = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private assetComponentEdit: AssetComponent = new AssetComponent();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];

  private catererAssets: CatererAsset[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  dishs: any;
  storeInfos: any;

  @ViewChild("f")
  private form;

  private catererId: string;
  public fileUploadResponse: { dbPath: ""; fileId: null; fileName: "" };

  constructor(
    private alertService: AlertService,
    private deliveryService: DeliveryService,
    private accountService: AccountService,
    public dialogRef: MatDialogRef<AssetComponentEditorComponent>,
    private mealService: MealService,
    private dishService: DishService,
    private fileService: FileService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (typeof data.catererAsset != typeof undefined) {
      if (data.catererAsset.catererId)
        this.catererId = data.catererAsset.catererId;
      if (data.catererAsset.id) {
        this.editAssetComponent(data.catererAsset);
      } else {
        this.newAssetComponent();
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
    if (this.isNewAssetComponent) {
      this.deliveryService.newAssetComponent(this.assetComponentEdit).subscribe(
        (catererAsset) => this.saveSuccessHelper(catererAsset),
        (error) => this.saveFailedHelper(error)
      );
    } else {
      this.deliveryService
        .updateAssetComponent(this.assetComponentEdit)
        .subscribe(
          (response) => this.saveSuccessHelper(),
          (error) => this.saveFailedHelper(error)
        );
    }
  }

  private saveSuccessHelper(catererAsset?: AssetComponent) {
    if (catererAsset) Object.assign(this.assetComponentEdit, catererAsset);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewAssetComponent)
      this.alertService.showMessage(
        "Success",
        `Caterer Asset \"${this.assetComponentEdit.description}\" was created successfully`,
        MessageSeverity.success
      );
    else
      this.alertService.showMessage(
        "Success",
        `Changes to Caterer Asset \"${this.assetComponentEdit.description}\" was saved successfully`,
        MessageSeverity.success
      );

    this.assetComponentEdit = new AssetComponent();
    this.resetForm();
    if (this.changesSavedCallback) this.changesSavedCallback();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage(
      "Save Error",
      "The below errors occured while saving your changes:",
      MessageSeverity.error
    );
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback) this.changesFailedCallback();
  }

  private cancel() {
    this.assetComponentEdit = new AssetComponent();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback) this.changesCancelledCallback();

    this.dialogRef.close({ isCancel: true });
  }

  resetForm(replace = false) {
    if (!replace) {
      this.form.reset();
    } else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  newAssetComponent() {
    this.isNewAssetComponent = true;
    this.showValidationErrors = true;

    this.selectedValues = {};
    this.assetComponentEdit = new AssetComponent();

    return this.assetComponentEdit;
  }

  editAssetComponent(catererAsset: AssetComponent) {
    if (catererAsset) {
      this.isNewAssetComponent = false;
      this.showValidationErrors = true;

      this.selectedValues = {};
      this.assetComponentEdit = new AssetComponent();
      Object.assign(this.assetComponentEdit, catererAsset);

      return this.assetComponentEdit;
    } else {
      return this.newAssetComponent();
    }
  }

  getCatererTypes() {
    let filter = new CatererAssetFilter();
    filter.filters = "(IsActive)==true";
    filter.catererInfoId = this.catererId;
    this.subscription.add(
      this.deliveryService.getCatererAssetsByFilter(filter).subscribe(
        (results) => {
          this.catererAssets = results.pagedData;
        },
        (error) => {
          this.alertService.showStickyMessage(
            "Get Error",
            `An error occured while retrieving caterer types.\r\n"`,
            MessageSeverity.error
          );
        }
      )
    );
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.assetComponentEdit.filePath = this.fileUploadResponse
      ? this.fileUploadResponse.dbPath
      : null;
  };

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  removePhoto() {
    this.assetComponentEdit.filePath = null;
    this.assetComponentEdit.fileId = null;
    this.assetComponentEdit.fileName = null;
  }
}
