import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ImageReferenceColor } from 'src/app/models/image-reference-color.model';
import { ImageReferenceColorService } from 'src/app/services/image-reference-color.service';


@Component({
  selector: 'image-reference-color-editor',
  templateUrl: './image-reference-color-editor.component.html',
  styleUrls: ['./image-reference-color-editor.component.css']
})
export class ImageReferenceColorEditorComponent {

  private isNewImageReferenceColor = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingImageReferenceColorName: string;
  private imageReferenceColorEdit: ImageReferenceColor = new ImageReferenceColor();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private imageReferenceColorService: ImageReferenceColorService, private accountService: AccountService,
    public dialogRef: MatDialogRef<ImageReferenceColorEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.imageReferenceColor) != typeof (undefined)) {
      if (data.imageReferenceColor.id) {
        this.editImageReferenceColor(data.imageReferenceColor);
      } else {
        this.newImageReferenceColor();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.imageReferenceColorEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewImageReferenceColor) {
      this.imageReferenceColorService.newImageReferenceColor(this.imageReferenceColorEdit).subscribe(imageReferenceColor => this.saveSuccessHelper(imageReferenceColor), error => this.saveFailedHelper(error));
    }
    else {
      this.imageReferenceColorService.updateImageReferenceColor(this.imageReferenceColorEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(imageReferenceColor?: ImageReferenceColor) {
    if (imageReferenceColor)
      Object.assign(this.imageReferenceColorEdit, imageReferenceColor);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewImageReferenceColor)
      this.alertService.showMessage("Success", `ImageReferenceColor \"${this.imageReferenceColorEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to imageReferenceColor \"${this.imageReferenceColorEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.imageReferenceColorEdit = new ImageReferenceColor();
    this.resetForm();


    //if (!this.isNewImageReferenceColor && this.accountService.currentUser.facilities.some(r => r == this.editingImageReferenceColorName))
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
    this.imageReferenceColorEdit = new ImageReferenceColor();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.alertService.resetStickyMessage();
    this.alertService.resetToastMessage();
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


  newImageReferenceColor() {
    this.isNewImageReferenceColor = true;
    this.showValidationErrors = true;

    this.editingImageReferenceColorName = null;
    this.selectedValues = {};
    this.imageReferenceColorEdit = new ImageReferenceColor();

    return this.imageReferenceColorEdit;
  }

  editImageReferenceColor(imageReferenceColor: ImageReferenceColor) {
    if (imageReferenceColor) {
      this.isNewImageReferenceColor = false;
      this.showValidationErrors = true;

      this.editingImageReferenceColorName = imageReferenceColor.name;
      this.selectedValues = {};
      this.imageReferenceColorEdit = new ImageReferenceColor();
      Object.assign(this.imageReferenceColorEdit, imageReferenceColor);

      return this.imageReferenceColorEdit;
    }
    else {
      return this.newImageReferenceColor();
    }
  }



  get canManageImageReferenceColors() {
    return this.accountService.userHasPermission(Permission.manageImageReferenceColorsPermission)
  }
}
