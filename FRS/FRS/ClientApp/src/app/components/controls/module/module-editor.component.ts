import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { ModuleService } from "../../../services/module.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Module } from 'src/app/models/module.model';


@Component({
  selector: 'module-editor',
  templateUrl: './module-editor.component.html',
  styleUrls: ['./module-editor.component.css']
})
export class ModuleEditorComponent {

  private isNewModule = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingModuleName: string;
  private moduleEdit: Module = new Module();
  private allPermissions: Permission[] = [];
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private moduleService: ModuleService, private accountService: AccountService,
    public dialogRef: MatDialogRef<ModuleEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.module) != typeof (undefined)) {
      if (data.module.id) {
        this.editModule(data.module);
      } else {
        this.newModule();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewModule) {
      this.moduleService.newModule(this.moduleEdit).subscribe(module => this.saveSuccessHelper(module), error => this.saveFailedHelper(error));
    }
    else {
      this.moduleService.updateModule(this.moduleEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(module?: Module) {
    if (module)
      Object.assign(this.moduleEdit, module);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewModule)
      this.alertService.showMessage("Success", `Module \"${this.moduleEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to module \"${this.moduleEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.moduleEdit = new Module();
    this.resetForm();


    //if (!this.isNewModule && this.accountService.currentUser.facilities.some(r => r == this.editingModuleName))
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
    this.moduleEdit = new Module();

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


  newModule() {
    this.isNewModule = true;
    this.showValidationErrors = true;

    this.editingModuleName = null;
    this.moduleEdit = new Module();
    return this.moduleEdit;
  }

  editModule(module: Module) {
    if (module) {
      this.isNewModule = false;
      this.showValidationErrors = true;

      this.editingModuleName = module.name;
      this.moduleEdit = new Module();
      Object.assign(this.moduleEdit, module);

      return this.moduleEdit;
    }
    else {
      return this.newModule();
    }
  }

  private fieldArray: Array<any> = [];
  private newParameter: any = {};

  addParameter() {
    if (!this.moduleEdit.moduleParameters) {
      this.moduleEdit.moduleParameters = [];
    }
    this.newParameter.moduleId = this.moduleEdit.id;
    this.moduleEdit.moduleParameters.push(this.newParameter)
    this.newParameter = {};
  }

  deleteParameter(index) {
    this.moduleEdit.moduleParameters.splice(index, 1);
  }

  get canManageModules() {
    return true; //this.accountService.userHasPermission(Permission.manageModulesPermission)
  }
}
