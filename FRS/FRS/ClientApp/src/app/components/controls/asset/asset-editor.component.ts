import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { Asset } from 'src/app/models/asset.model';
import { AssetService } from 'src/app/services/asset.service';
import { FileService } from 'src/app/services/file.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { LocationService } from '../../../services/location.service';
import { InstitutionService } from '../../../services/institution.service';



@Component({
  selector: 'asset-editor',
  templateUrl: './asset-editor.component.html',
  styleUrls: ['./asset-editor.component.css'],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class AssetEditorComponent {

  private isNewAsset = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingAssetName: string;
  private assetEdit: Asset = new Asset();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  public allLocations = [];
  public allInstitutions = [];

  public assetModels = [];
  public assetTypes = [];
    filter: Filter;

  constructor(private alertService: AlertService, private assetService: AssetService, private accountService: AccountService, private locationService: LocationService, private institutionService: InstitutionService,
    private fileService: FileService,
    public dialogRef: MatDialogRef<AssetEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.asset) != typeof (undefined)) {
      if (data.asset.id) {
        this.editAsset(data.asset);
      } else {
        this.newAsset();
      }

      this.loadAssetModels();
      this.loadAssetTypes();
      this.getLocations();
      this.loadInstitutions();
    }
  }

  onChangeDatePurchase(type: string, event: MatDatepickerInputEvent<Date>) {
    this.assetEdit.purchaseDate = new Date(event.value);
  }

  onChangeDateStart(type: string, event: MatDatepickerInputEvent<Date>) {
    this.assetEdit.warrantyStart = new Date(event.value);
  }

  onChangeDateEnd(type: string, event: MatDatepickerInputEvent<Date>) {
    this.assetEdit.warrantyEnd = new Date(event.value);
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    if (this.isNewAsset) {
      this.assetService.newAsset(this.assetEdit).subscribe(asset => this.saveSuccessHelper(asset), error => this.saveFailedHelper(error));
    }
    else {
      this.assetService.updateAsset(this.assetEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(asset?: Asset) {
    if (asset)
      Object.assign(this.assetEdit, asset);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewAsset)
      this.alertService.showMessage("Success", `Asset \"${this.assetEdit.serialNumber}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to asset type \"${this.assetEdit.serialNumber}\" was saved successfully`, MessageSeverity.success);


    this.assetEdit = new Asset();
    this.resetForm();


    //if (!this.isNewAsset && this.accountService.currentUser.facilities.some(r => r == this.editingAssetName))
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
    this.assetEdit = new Asset();

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


  newAsset() {
    this.isNewAsset = true;
    this.showValidationErrors = true;

    this.editingAssetName = null;
    this.selectedValues = {};
    this.assetEdit = new Asset();

    return this.assetEdit;
  }

  editAsset(asset: Asset) {
    if (asset) {
      this.isNewAsset = false;
      this.showValidationErrors = true;

      this.editingAssetName = asset.serialNumber;
      this.selectedValues = {};
      this.assetEdit = new Asset();
      Object.assign(this.assetEdit, asset);

      return this.assetEdit;
    }
    else {
      return this.newAsset();
    }
  }

  loadAssetModels() {
    this.filter = new Filter();
    this.filter.sorts = 'Name';
    this.filter.filters = '';
    this.filter.page = 0;
    this.filter.pageSize = 0;
    this.filter.filters = '(IsActive)==true,(InstitutionId)==' + this.accountService.currentUser.institutionId;

    this.assetService.getAssetModelsByFilter(this.filter)
      .subscribe(results => {
        this.assetModels = results.pagedData;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getAssetModelsByFilter failed", "An error occured while trying to get asset model from the server", MessageSeverity.error);
        });
  }

  loadAssetTypes() {
    this.filter = new Filter();
    this.filter.sorts = 'Name';
    this.filter.filters = '';
    this.filter.page = 0;
    this.filter.pageSize = 0;
    this.filter.filters = '(IsActive)==true,(InstitutionId)==' + this.accountService.currentUser.institutionId;

    this.assetService.getAssetTypesByFilter(this.filter)
      .subscribe(results => {
        this.assetTypes = results.pagedData;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getAssetTypesByFilter failed", "An error occured while trying to get asset types from the server", MessageSeverity.error);
        });
  }

  getAssetTypeName(id, row) {
    if (!this.assetModels) this.assetModels = [];
    let modelIndx = this.assetModels.findIndex(e => e.id == id);
    if (modelIndx > -1) {
      if (!this.assetTypes) this.assetTypes = [];
      let typeIndx = this.assetTypes.findIndex(e => e.id == this.assetModels[modelIndx].assetTypeId);

      return this.assetTypes[typeIndx] ? this.assetTypes[typeIndx].name : '';
    }

    return '';
  }

  getLocations() {

    this.locationService.getLocations(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.allLocations = results;
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.`,
            MessageSeverity.error);
        });
  }

  loadInstitutions() {
    this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();

        this.allInstitutions = results[0];

      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving institutions.`,
            MessageSeverity.error);
        });
  }

  get canManageAssets() {
    return this.accountService.userHasPermission(Permission.manageAssetsPermission)
  }
}
