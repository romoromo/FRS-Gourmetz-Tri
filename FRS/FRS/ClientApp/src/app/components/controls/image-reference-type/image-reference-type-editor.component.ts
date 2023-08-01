import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ImageReferenceType } from 'src/app/models/image-reference-type.model';
import { ImageReferenceTypeService } from 'src/app/services/image-reference-type.service';


@Component({
  selector: 'image-reference-type-editor',
  templateUrl: './image-reference-type-editor.component.html',
  styleUrls: ['./image-reference-type-editor.component.css']
})
export class ImageReferenceTypeEditorComponent {

  private isNewImageReferenceType = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingImageReferenceTypeName: string;
  private imageReferenceTypeEdit: ImageReferenceType = new ImageReferenceType();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private imageReferenceTypeService: ImageReferenceTypeService, private accountService: AccountService,
    public dialogRef: MatDialogRef<ImageReferenceTypeEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.imageReferenceType) != typeof (undefined)) {
      if (data.imageReferenceType.id) {
        this.editImageReferenceType(data.imageReferenceType);
      } else {
        this.newImageReferenceType();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.imageReferenceTypeEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewImageReferenceType) {
      this.imageReferenceTypeService.newImageReferenceType(this.imageReferenceTypeEdit).subscribe(imageReferenceType => this.saveSuccessHelper(imageReferenceType), error => this.saveFailedHelper(error));
    }
    else {
      this.imageReferenceTypeService.updateImageReferenceType(this.imageReferenceTypeEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(imageReferenceType?: ImageReferenceType) {
    if (imageReferenceType)
      Object.assign(this.imageReferenceTypeEdit, imageReferenceType);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewImageReferenceType)
      this.alertService.showMessage("Success", `ImageReferenceType \"${this.imageReferenceTypeEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to imageReferenceType \"${this.imageReferenceTypeEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.imageReferenceTypeEdit = new ImageReferenceType();
    this.resetForm();


    //if (!this.isNewImageReferenceType && this.accountService.currentUser.facilities.some(r => r == this.editingImageReferenceTypeName))
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
    this.imageReferenceTypeEdit = new ImageReferenceType();

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


  newImageReferenceType() {
    this.isNewImageReferenceType = true;
    this.showValidationErrors = true;

    this.editingImageReferenceTypeName = null;
    this.selectedValues = {};
    this.imageReferenceTypeEdit = new ImageReferenceType();

    return this.imageReferenceTypeEdit;
  }

  editImageReferenceType(imageReferenceType: ImageReferenceType) {
    if (imageReferenceType) {
      this.isNewImageReferenceType = false;
      this.showValidationErrors = true;

      this.editingImageReferenceTypeName = imageReferenceType.name;
      this.selectedValues = {};
      this.imageReferenceTypeEdit = new ImageReferenceType();
      Object.assign(this.imageReferenceTypeEdit, imageReferenceType);

      return this.imageReferenceTypeEdit;
    }
    else {
      return this.newImageReferenceType();
    }
  }



  get canManageImageReferenceTypes() {
    return this.accountService.userHasPermission(Permission.manageImageReferenceTypesPermission)
  }
}
