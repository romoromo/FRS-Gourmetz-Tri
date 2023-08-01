import { Component, ViewChild, Inject, ViewEncapsulation, OnDestroy } from '@angular/core';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Permission } from 'src/app/models/permission.model';
import { UserEdit } from 'src/app/models/user-edit.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AccountService } from 'src/app/services/account.service';
import { UserCardId } from 'src/app/models/usercardid.model';
import { User } from 'src/app/models/user.model';
import { Utilities } from 'src/app/services/utilities';
import { Subscription } from 'rxjs';


@Component({
  selector: 'user-cardid-editor',
  templateUrl: './usercardid-editor.component.html',
  styleUrls: ['./usercardid-editor.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class UserCardIdEditorComponent implements OnDestroy{
  private subscription: Subscription = new Subscription();
  private isNewUserCardId = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingUserCardIdName: string;
  private userCardIdEdit: UserCardId = new UserCardId();
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



  constructor(private alertService: AlertService, private accountService: AccountService,
    public dialogRef: MatDialogRef<UserCardIdEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.userCardId) != typeof (undefined)) {
      this.isUserInfo = data.isUserInfo;
      this.userCardIdEdit = data.userCardId;
      if (!this.isUserInfo) {
        this.loadUsers();
      }
      if (data.userCardId) {
        this.editUserCardId(this.userCardIdEdit);
      } else {
        this.newUserCardId();
      }
    }
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  loadUsers() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.accountService.getUsers(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.allUsers = results;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve users from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {

    if (this.isUserInfo) {
      this.dialogRef.close(this.userCardIdEdit);
    } else {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");

      if (!this.userCardIdEdit.id) {
        this.subscription.add(this.accountService.newUserCardId(this.userCardIdEdit).subscribe(cardid => this.saveSuccessHelper(cardid), error => this.saveFailedHelper(error)));
      }
      else {
        this.subscription.add(this.accountService.updateUserCardId(this.userCardIdEdit).subscribe(response => this.saveSuccessHelper(this.userCardIdEdit), error => this.saveFailedHelper(error)));
      }
    }
  }

  private saveSuccessHelper(cardid: UserCardId) {
    this.alertService.stopLoadingMessage();
    this.alertService.showMessage("Success", `"Card with id. '${cardid.cardId}\"' has been saved!`, MessageSeverity.success);
    this.dialogRef.close(this.userCardIdEdit);
  }

  private saveFailedHelper(error: any) {
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  }


  private cancel() {
    this.userCardIdEdit = new UserCardId();

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


  newUserCardId() {
    this.isNewUserCardId = true;
    this.showValidationErrors = true;

    this.editingUserCardIdName = null;
    this.selectedValues = {};
    this.userCardIdEdit = new UserCardId();
    this.userCardIdEdit.status = "INACTIVE";
    return this.userCardIdEdit;
  }

  editUserCardId(userCardId: UserCardId) {
    if (UserCardId) {
      this.isNewUserCardId = false;
      this.showValidationErrors = true;

      this.editingUserCardIdName = userCardId.cardId;
      this.selectedValues = {};
      this.userCardIdEdit = new UserCardId();
      Object.assign(this.userCardIdEdit, userCardId);
      return this.userCardIdEdit;
    }
    else {
      return this.newUserCardId();
    }
  }
}
