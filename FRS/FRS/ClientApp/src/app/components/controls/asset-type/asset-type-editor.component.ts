import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AssetType } from 'src/app/models/asset-type.model';
import { AssetService } from 'src/app/services/asset.service';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'asset-type-editor',
  templateUrl: './asset-type-editor.component.html',
  styleUrls: ['./asset-type-editor.component.css']
})
export class AssetTypeEditorComponent {

  private isNewAssetType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingAssetTypeName: string;
  private assetTypeEdit: AssetType = new AssetType();
  private allPermissions: Permission[] = [];
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
    public dialogRef: MatDialogRef<AssetTypeEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.assetType) != typeof (undefined)) {
      if (data.assetType.id) {
        this.editAssetType(data.assetType);
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
    this.assetTypeEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewAssetType) {
      this.assetService.newAssetType(this.assetTypeEdit).subscribe(assetType => this.saveSuccessHelper(assetType), error => this.saveFailedHelper(error));
    }
    else {
      this.assetService.updateAssetType(this.assetTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(assetType?: AssetType) {
    if (assetType)
      Object.assign(this.assetTypeEdit, assetType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewAssetType)
      this.alertService.showMessage("Success", `Asset Type \"${this.assetTypeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to asset type \"${this.assetTypeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.assetTypeEdit = new AssetType();
    this.resetForm();


    //if (!this.isNewAssetType && this.accountService.currentUser.facilities.some(r => r == this.editingAssetTypeName))
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
    this.assetTypeEdit = new AssetType();

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


  newAssetType() {
    this.isNewAssetType = true;
    this.showValidationErrors = true;

    this.editingAssetTypeName = null;
    this.selectedValues = {};
    this.assetTypeEdit = new AssetType();

    return this.assetTypeEdit;
  }

  editAssetType(assetType: AssetType) {
    if (assetType) {
      this.isNewAssetType = false;
      this.showValidationErrors = true;

      this.editingAssetTypeName = assetType.name;
      this.selectedValues = {};
      this.assetTypeEdit = new AssetType();
      Object.assign(this.assetTypeEdit, assetType);

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

  get canManageAssetTypes() {
    return this.accountService.userHasPermission(Permission.manageAssetTypesPermission)
  }
}
