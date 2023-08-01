import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { MediaService } from "../../../services/media.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Media } from 'src/app/models/media.model';
import { Role } from '../../../models/role.model';
import { UserGroupService } from '../../../services/userGroup.service';


@Component({
  selector: 'media-editor',
  templateUrl: './media-editor.component.html',
  styleUrls: ['./media-editor.component.css']
})
export class MediaEditorComponent {

  private isNewMedia = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingMediaName: string;
  private mediaEdit: Media = new Media();
  private allPermissions: Permission[] = [];
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  private allRoles: Role[] = [];


  @ViewChild('f')
  private form;
    filter: any = {};
    allUserGroups: any;



  constructor(private alertService: AlertService, private mediaService: MediaService, private accountService: AccountService, private userGroupService: UserGroupService, 
    public dialogRef: MatDialogRef<MediaEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.media) != typeof (undefined)) {
      if (data.media.id) {
        this.editMedia(data.media);
      } else {
        this.newMedia();
      }
    }

    this.accountService.getRoles()
      .subscribe(results => {
        this.allRoles = results;
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving roles.`,
            MessageSeverity.error);
        });

    this.filter.filters = '(InstitutionId)==' + this.accountService.currentUser.institutionId;
    this.userGroupService.getUserGroupsByFilter(this.filter)
      .subscribe(results => {

        this.allUserGroups = results.pagedData;

      },
        error => {
        });
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewMedia) {
      this.mediaService.newMedia(this.mediaEdit).subscribe(media => this.saveSuccessHelper(media), error => this.saveFailedHelper(error));
    }
    else {
      this.mediaService.updateMedia(this.mediaEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(media?: Media) {
    if (media)
      Object.assign(this.mediaEdit, media);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewMedia)
      this.alertService.showMessage("Success", `Media \"${this.mediaEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to media \"${this.mediaEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.mediaEdit = new Media();
    this.resetForm();


    //if (!this.isNewMedia && this.accountService.currentUser.facilities.some(r => r == this.editingMediaName))
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
    this.mediaEdit = new Media();

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


  newMedia() {
    this.isNewMedia = true;
    this.showValidationErrors = true;

    this.editingMediaName = null;
    this.mediaEdit = new Media();
    this.mediaEdit.institutionId = this.accountService.currentUser.institutionId;
    return this.mediaEdit;
  }

  editMedia(media: Media) {
    if (media) {
      this.isNewMedia = false;
      this.showValidationErrors = true;

      this.editingMediaName = media.name;
      this.mediaEdit = new Media();
      Object.assign(this.mediaEdit, media);

      return this.mediaEdit;
    }
    else {
      return this.newMedia();
    }
  }



  get canManageMedias() {
    return true; //this.accountService.userHasPermission(Permission.manageMediasPermission)
  }
}
