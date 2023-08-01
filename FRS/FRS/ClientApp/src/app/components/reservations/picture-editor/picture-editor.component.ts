import { Component, ViewChild, Inject, ViewEncapsulation, OnDestroy } from '@angular/core';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Permission } from 'src/app/models/permission.model';
import { Reservation, ReservationPicture } from 'src/app/models/reservation.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AccountService } from 'src/app/services/account.service';
import { UserCardId } from 'src/app/models/usercardid.model';
import { User } from 'src/app/models/user.model';
import { Utilities } from 'src/app/services/utilities';
import { Subscription } from 'rxjs';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'picture-editor',
  templateUrl: './picture-editor.component.html',
  styleUrls: ['./picture-editor.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class PictureEditorComponent implements OnDestroy{
  private isNewUserCardId = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private pictureEdit: ReservationPicture = new ReservationPicture();
  private allPermissions: Permission[] = [];
  private allUsers: User[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public isUserInfo = true;
  private loadingIndicator: boolean;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };



  constructor(private alertService: AlertService, private accountService: AccountService, private fileService: FileService,
    public dialogRef: MatDialogRef<PictureEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.reservationPicture) != typeof (undefined)) {
      this.pictureEdit = data.reservationPicture;
      if (data.reservationPicture) {
        this.editPicture(this.pictureEdit);
      } else {
        this.newPicture();
      }
    }
  }

  ngOnDestroy(): void {
    //if (this.subscription) {
    //  this.subscription.unsubscribe();
    //}
  }

  //loadUsers() {
  //  this.alertService.startLoadingMessage();
  //  this.loadingIndicator = true;

  //  this.subscription.add(this.accountService.getUsers(null, null, this.accountService.currentUser.institutionId)
  //    .subscribe(results => {
  //      this.alertService.stopLoadingMessage();
  //      this.loadingIndicator = false;

  //      this.allUsers = results;
  //    },
  //      error => {
  //        this.alertService.stopLoadingMessage();
  //        this.loadingIndicator = false;

  //        this.alertService.showStickyMessage("Load Error", `Unable to retrieve users from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
  //          MessageSeverity.error);
  //      }));
  //}

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {

    if (this.isUserInfo) {
      this.dialogRef.close(this.pictureEdit);
    } else {
      this.dialogRef.close(this.pictureEdit);
    }
  }

  private saveSuccessHelper(cardid: UserCardId) {
    this.alertService.stopLoadingMessage();
    this.alertService.showMessage("Success", `Picture has been saved!`, MessageSeverity.success);
    this.dialogRef.close(this.pictureEdit);
  }

  private saveFailedHelper(error: any) {
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  }


  private cancel() {
    this.pictureEdit = new ReservationPicture();

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
  }


  newPicture() {
    this.isNewUserCardId = true;
    this.showValidationErrors = true;
    this.selectedValues = {};
    this.pictureEdit = new ReservationPicture();
    return this.pictureEdit;
  }

  editPicture(picture: ReservationPicture) {
    if (ReservationPicture) {
      this.isNewUserCardId = false;
      this.showValidationErrors = true;

      this.selectedValues = {};
      this.pictureEdit = new ReservationPicture();
      Object.assign(this.pictureEdit, picture);
      return this.pictureEdit;
    }
    else {
      return this.newPicture();
    }
  }

  public uploadFinished = (event) => {
    console.log("finish uploading... ", event)
    this.fileUploadResponse = event;
    this.pictureEdit.pictureUrl = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }
}
