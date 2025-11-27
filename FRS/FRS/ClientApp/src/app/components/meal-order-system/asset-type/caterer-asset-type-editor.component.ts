import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { DishService } from '../../../services/meal-order/dish.service';
import { FileService } from 'src/app/services/file.service';
import { CatererAssetType } from 'src/app/models/meal-order/asset-type.model';


@Component({
  selector: 'caterer-asset-type-editor',
  templateUrl: './caterer-asset-type-editor.component.html',
  styleUrls: ['./caterer-asset-type-editor.component.css']
})
export class CatererAsssetTypeEditorComponent {

  private isNewAssetType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingAssetTypeCode: string;
  private assetTypeEdit: CatererAssetType = new CatererAssetType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  private catererInfoId: string;

  constructor(private alertService: AlertService, private dishService: DishService, private accountService: AccountService,
    public dialogRef: MatDialogRef<CatererAsssetTypeEditorComponent>, private mealService: MealService, private fileService: FileService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.AssetType) != typeof (undefined)) {
      if(data.AssetType.catererInfoId)
        this.catererInfoId = data.AssetType.catererInfoId;

      if (data.AssetType.id) {
        this.editAssetType(data.AssetType);
      } else {
        this.newAssetType();
      }
    }
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewAssetType) {
      this.dishService.newAssetType(this.assetTypeEdit).subscribe(AssetType => this.saveSuccessHelper(AssetType), error => this.saveFailedHelper(error));
    }
    else {
      this.dishService.updateAssetType(this.assetTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(AssetType?: CatererAssetType) {
    if (AssetType)
      Object.assign(this.assetTypeEdit, AssetType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewAssetType)
      this.alertService.showMessage("Success", `\"${this.assetTypeEdit.code}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.assetTypeEdit.code}\" was saved successfully`, MessageSeverity.success);

    this.assetTypeEdit = new CatererAssetType();
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
    this.assetTypeEdit = new CatererAssetType();

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


  newAssetType() {
    this.isNewAssetType = true;
    this.showValidationErrors = true;

    this.editingAssetTypeCode = null;
    this.selectedValues = {};
    this.assetTypeEdit = new CatererAssetType();
    this.assetTypeEdit.catererInfoId = this.catererInfoId;
    return this.assetTypeEdit;
  }

  editAssetType(AssetType: CatererAssetType) {
    if (AssetType) {
      this.isNewAssetType = false;
      this.showValidationErrors = true;

      this.editingAssetTypeCode = AssetType.code;
      this.selectedValues = {};
      this.assetTypeEdit = new CatererAssetType();
      Object.assign(this.assetTypeEdit, AssetType);
      this.assetTypeEdit.catererInfoId = this.catererInfoId;
      return this.assetTypeEdit;
    }
    else {
      return this.newAssetType();
    }
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.assetTypeEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  removePhoto() {
    this.assetTypeEdit.filePath = null;
    this.assetTypeEdit.fileId = null;
    this.assetTypeEdit.fileName = null;
  }



  get canManageAssetTypes() {
    return true; //this.accountService.userHasPermission(Permission.manageAssetTypesPermission)
  }
}
