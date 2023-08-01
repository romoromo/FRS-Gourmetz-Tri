import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AssetModel } from 'src/app/models/asset-model.model';
import { AssetService } from 'src/app/services/asset.service';
import { FileService } from 'src/app/services/file.service';
import { AssetType } from 'src/app/models/asset-type.model';
import { Filter } from 'src/app/models/sieve-filter.model';


@Component({
  selector: 'asset-model-editor',
  templateUrl: './asset-model-editor.component.html',
  styleUrls: ['./asset-model-editor.component.css']
})
export class AssetModelEditorComponent {

  private isNewAssetModel = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingAssetModelName: string;
  private assetModelEdit: AssetModel = new AssetModel();
  private allPermissions: Permission[] = [];
  private assetTypes: AssetType[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private assetService: AssetService, private accountService: AccountService,
    private fileService: FileService,
    public dialogRef: MatDialogRef<AssetModelEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.loadAssetTypes();
    if (typeof (data.assetModel) != typeof (undefined)) {
      if (data.assetModel.id) {
        this.editAssetModel(data.assetModel);
      } else {
        this.newAssetModel();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.assetModelEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewAssetModel) {
      this.assetService.newAssetModel(this.assetModelEdit).subscribe(assetModel => this.saveSuccessHelper(assetModel), error => this.saveFailedHelper(error));
    }
    else {
      this.assetService.updateAssetModel(this.assetModelEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(assetModel?: AssetModel) {
    if (assetModel)
      Object.assign(this.assetModelEdit, assetModel);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewAssetModel)
      this.alertService.showMessage("Success", `Asset Model \"${this.assetModelEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to asset model \"${this.assetModelEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.assetModelEdit = new AssetModel();
    this.resetForm();


    //if (!this.isNewAssetModel && this.accountService.currentUser.facilities.some(r => r == this.editingAssetModelName))
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
    this.assetModelEdit = new AssetModel();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  private toggleGroup(groupName: string) {
    let firstMemberValue: boolean;

    this.allPermissions.forEach(p => {
      if (p.groupName != groupName)
        return;

      if (firstMemberValue == null)
        firstMemberValue = this.selectedValues[p.value] == true;

      this.selectedValues[p.value] = !firstMemberValue;
    });
  }

  loadAssetTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true,(InstitutionId)==' + this.accountService.currentUser.institutionId;
    this.assetService.getAssetTypesByFilter(filter)
      .subscribe(results => {
        this.assetTypes = results.pagedData;

      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getAssetTypesByFilter failed", "An error occured while trying to get asset types from the server", MessageSeverity.error);
        });
  }

  private getSelectedPermissions() {
    return this.allPermissions.filter(p => this.selectedValues[p.value] == true);
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


  newAssetModel() {
    this.isNewAssetModel = true;
    this.showValidationErrors = true;

    this.editingAssetModelName = null;
    this.selectedValues = {};
    this.assetModelEdit = new AssetModel();

    return this.assetModelEdit;
  }

  editAssetModel(assetModel: AssetModel) {
    if (assetModel) {
      this.isNewAssetModel = false;
      this.showValidationErrors = true;

      this.editingAssetModelName = assetModel.name;
      this.selectedValues = {};
      this.assetModelEdit = new AssetModel();
      Object.assign(this.assetModelEdit, assetModel);

      return this.assetModelEdit;
    }
    else {
      return this.newAssetModel();
    }
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.assetModelEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageAssetModels() {
    return this.accountService.userHasPermission(Permission.manageAssetModelsPermission)
  }
}
