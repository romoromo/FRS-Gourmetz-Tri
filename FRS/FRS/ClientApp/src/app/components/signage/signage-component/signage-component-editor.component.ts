import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { SignageComponentService } from "../../../services/signage-component.service";
import { SignageComponent } from '../../../models/signage-component.model';
import { Permission } from '../../../models/permission.model';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { SignageComponentType } from '../../../models/signage-component-type.model';
import { SignageComponentTypeService } from '../../../services/signage-component-type.service';
import { SignageComponentTypeWrapperComponent } from '../signage-component-types/signage-component-type-wrapper.component';


@Component({
  selector: 'signage-component-editor',
  templateUrl: './signage-component-editor.component.html',
  styleUrls: ['./signage-component-editor.component.css']
})
export class SignageComponentEditorComponent {

  private isNewSignageComponent = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingSignageComponentName: string;
  private signageComponentEdit: SignageComponent = new SignageComponent();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  public componentTypes: SignageComponentType[];

  @ViewChild('f')
  private form;

  @ViewChild(SignageComponentTypeWrapperComponent)
  wrapper: SignageComponentTypeWrapperComponent;

  @ViewChild('signageDisplayTypeWrapper')
  wrapperDisplay: SignageComponentTypeWrapperComponent;



  constructor(private alertService: AlertService, private signageComponentService: SignageComponentService, private accountService: AccountService, private signageComponentTypeService: SignageComponentTypeService,
    //public dialogRef: MatDialogRef<SignageComponentEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.componentTypes = signageComponentTypeService.getComponentTypes();

    if (typeof (data.signageComponent) != typeof (undefined)) {
      if (data.signageComponent.id) {
        this.editSignageComponent(data.signageComponent);
      } else {
        this.newSignageComponent();
      }
    }
  }

  componentTypeChange = (event, preview) => {
    if (this.signageComponentEdit.componentType) {
      this.signageComponentEdit.componentTypeObj = this.signageComponentTypeService.getComponentTypeById(this.signageComponentEdit.componentType);

      if (!preview) {
        if (this.signageComponentEdit.prevComponentType == this.signageComponentEdit.componentType) {
          this.signageComponentEdit.configurationsObj = JSON.parse(this.signageComponentEdit.configurations);
        } else {
          this.signageComponentEdit.configurationsObj = {};
        }
      }

      if (this.wrapper && !preview) this.wrapper.loadComponent(this.signageComponentEdit.componentTypeObj.configuration, this.signageComponentEdit.configurationsObj);

      if (this.wrapperDisplay) this.wrapperDisplay.loadComponent(this.signageComponentEdit.componentTypeObj.component, this.signageComponentEdit, true);
    }
  }

  public uploadFinished = (event) => {
    this.signageComponentEdit.backgroundImage = event ? event.dbPath : null;

    if (this.signageComponentEdit.backgroundImage) this.signageComponentEdit.backgroundImage = this.signageComponentEdit.backgroundImage.replace(/\\/g, '/');
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.wrapper) this.signageComponentEdit.configurations = JSON.stringify(this.wrapper.getConfigurations());

    if (this.isNewSignageComponent) {
      this.signageComponentService.newSignageComponent(this.signageComponentEdit).subscribe(signageComponent => this.saveSuccessHelper(signageComponent), error => this.saveFailedHelper(error));
    }
    else {
      this.signageComponentService.updateSignageComponent(this.signageComponentEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(signageComponent?: SignageComponent) {
    if (signageComponent)
      Object.assign(this.signageComponentEdit, signageComponent);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewSignageComponent)
      this.alertService.showMessage("Success", `Signage Component \"${this.signageComponentEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to signage component \"${this.signageComponentEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.signageComponentEdit = new SignageComponent();
    this.resetForm();


    if (this.changesSavedCallback)
      this.changesSavedCallback();

    //this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();

    //this.dialogRef.close();
  }


  private cancel() {
    this.signageComponentEdit = new SignageComponent();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    //this.dialogRef.close();
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


  newSignageComponent() {
    this.isNewSignageComponent = true;
    this.showValidationErrors = true;

    this.editingSignageComponentName = null;
    this.selectedValues = {};
    this.signageComponentEdit = new SignageComponent();


    this.signageComponentEdit.previewWidth = this.signageComponentEdit.previewWidth || 720;
    this.signageComponentEdit.previewHeight = this.signageComponentEdit.previewHeight || 480;
    return this.signageComponentEdit;
  }

  editSignageComponent(signageComponent: SignageComponent) {
    if (signageComponent) {
      this.isNewSignageComponent = false;
      this.showValidationErrors = true;

      this.editingSignageComponentName = signageComponent.name;
      this.selectedValues = {};
      this.signageComponentEdit = new SignageComponent();
      Object.assign(this.signageComponentEdit, signageComponent);

      if (this.signageComponentEdit.componentType) {
        this.signageComponentEdit.componentTypeObj = this.signageComponentTypeService.getComponentTypeById(this.signageComponentEdit.componentType);

        this.signageComponentEdit.prevComponentType = this.signageComponentEdit.componentType;

        this.signageComponentEdit.configurationsObj = JSON.parse(this.signageComponentEdit.configurations);
      }

      this.signageComponentEdit.previewWidth = this.signageComponentEdit.previewWidth || 720;
      this.signageComponentEdit.previewHeight = this.signageComponentEdit.previewHeight || 480;

      return this.signageComponentEdit;
    }
    else {
      return this.newSignageComponent();
    }
  }



  get canManageSignageComponents() {
    return true;//this.accountService.userHasPermission(Permission.manageSignageComponentsPermission)
  }
}
