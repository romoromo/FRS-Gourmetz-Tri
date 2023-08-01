import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import { Permission } from '../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ServiceContract } from 'src/app/models/service-contract.model';
import { ServiceContractService } from 'src/app/services/service-contract.service';
import { FileService } from 'src/app/services/file.service';
import { AssetSelectorComponent } from '../common/asset-selector/asset-selector.component';


@Component({
  selector: 'service-contract-editor',
  templateUrl: './service-contract-editor.component.html',
  styleUrls: ['./service-contract-editor.component.css']
})
export class ServiceContractEditorComponent {

  private isNewServiceContract = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingServiceContractName: string;
  private serviceContractEdit: ServiceContract = new ServiceContract();
  private allPermissions: Permission[] = [];
  private alerts = [7, 30, 60];
  private selectedValues: { [key: string]: boolean; } = {};
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private serviceContractService: ServiceContractService, private accountService: AccountService,
    private fileService: FileService,
    public dialogRef: MatDialogRef<ServiceContractEditorComponent>, public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.serviceContract) != typeof (undefined)) {
      if (data.serviceContract.id) {
        this.editServiceContract(data.serviceContract);
      } else {
        this.newServiceContract();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    if (this.isNewServiceContract) {
      this.serviceContractService.newServiceContract(this.serviceContractEdit).subscribe(serviceContract => this.saveSuccessHelper(serviceContract), error => this.saveFailedHelper(error));
    }
    else {
      this.serviceContractService.updateServiceContract(this.serviceContractEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(serviceContract?: ServiceContract) {
    if (serviceContract)
      Object.assign(this.serviceContractEdit, serviceContract);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewServiceContract)
      this.alertService.showMessage("Success", `Service Contract \"${this.serviceContractEdit.identifier}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.serviceContractEdit.identifier}\" was saved successfully`, MessageSeverity.success);


    this.serviceContractEdit = new ServiceContract();
    this.resetForm();


    //if (!this.isNewServiceContract && this.accountService.currentUser.facilities.some(r => r == this.editingServiceContractName))
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
    this.serviceContractEdit = new ServiceContract();

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


  newServiceContract() {
    this.isNewServiceContract = true;
    this.showValidationErrors = true;

    this.editingServiceContractName = null;
    this.selectedValues = {};
    this.serviceContractEdit = new ServiceContract();
    this.serviceContractEdit.renewalAlert = 7;
    this.serviceContractEdit.startDate = this.serviceContractEdit.endDate = new Date();
    this.serviceContractEdit.serviceContractAssets = [];
    return this.serviceContractEdit;
  }

  editServiceContract(serviceContract: ServiceContract) {
    if (serviceContract) {
      this.isNewServiceContract = false;
      this.showValidationErrors = true;

      this.editingServiceContractName = serviceContract.identifier;
      this.selectedValues = {};
      this.serviceContractEdit = new ServiceContract();
      Object.assign(this.serviceContractEdit, serviceContract);

      return this.serviceContractEdit;
    }
    else {
      return this.newServiceContract();
    }
  }



  addAssetFromSelector() {
    let assets = [];
    this.serviceContractEdit.serviceContractAssets.forEach((ir, index, l) => {
      ir.assetId = ir.assetId;
      ir.serviceContractId = this.serviceContractEdit.id;
      assets.push(ir);
    });

    const dialogRef = this.dialog.open(AssetSelectorComponent, {
      data: { header: "Assets", assets: assets },
      disableClose: true,
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result.isCancel) {
        console.log(result.selectedAssets);
        let assets = [];
        if (result.selectedAssets) {
          result.selectedAssets.forEach((ir, index, l) => {
            ir.assetId = ir.assetId;
            ir.serviceContractId = this.serviceContractEdit.id;
            ir.id = 0;
            assets.push(ir);
          });
        }
        this.serviceContractEdit.serviceContractAssets = assets;
      }
    });
  }

  get canManageServiceContracts() {
    return this.accountService.userHasPermission(Permission.manageServiceContractsPermission)
  }
}
