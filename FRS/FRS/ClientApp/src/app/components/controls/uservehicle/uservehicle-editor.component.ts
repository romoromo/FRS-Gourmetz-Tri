import { Component, ViewChild, Inject, ViewEncapsulation, OnDestroy } from '@angular/core';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Permission } from 'src/app/models/permission.model';
import { UserEdit } from 'src/app/models/user-edit.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AccountService } from 'src/app/services/account.service';
import { UserVehicle } from 'src/app/models/uservehicle.model';
import { User } from 'src/app/models/user.model';
import { Utilities } from 'src/app/services/utilities';
import { Subscription } from 'rxjs';


@Component({
  selector: 'user-vehicle-editor',
  templateUrl: './uservehicle-editor.component.html',
  styleUrls: ['./uservehicle-editor.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class UserVehicleEditorComponent implements OnDestroy{
  private subscription: Subscription = new Subscription();
  private isNewUserVehicle = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingUserVehicleName: string;
  private userVehicleEdit: UserVehicle = new UserVehicle();
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
    public dialogRef: MatDialogRef<UserVehicleEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.userVehicle) != typeof (undefined)) {
      this.isUserInfo = data.isUserInfo;
      this.userVehicleEdit = data.userVehicle;
      if (!this.isUserInfo) {
        this.loadUsers();
      }
      if (data.userVehicle.id) {
        this.editUserVehicle(this.userVehicleEdit);
      } else {
        this.newUserVehicle();
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
      this.dialogRef.close(this.userVehicleEdit);
    } else {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");

      if (!this.userVehicleEdit.id) {
        this.subscription.add(this.accountService.newUserVehicle(this.userVehicleEdit).subscribe(vehicle => this.saveSuccessHelper(vehicle), error => this.saveFailedHelper(error)));
      }
      else {
        this.subscription.add(this.accountService.updateUserVehicle(this.userVehicleEdit).subscribe(response => this.saveSuccessHelper(this.userVehicleEdit), error => this.saveFailedHelper(error)));
      }
    }
  }

  private saveSuccessHelper(vehicle: UserVehicle) {
    this.alertService.stopLoadingMessage();
    this.alertService.showMessage("Success", `"Vehicle with plate no. '${vehicle.plateNumber}\"' has been saved!`, MessageSeverity.success);
    this.dialogRef.close(this.userVehicleEdit);
  }

  private saveFailedHelper(error: any) {
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  }


  private cancel() {
    this.userVehicleEdit = new UserVehicle();

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


  newUserVehicle() {
    this.isNewUserVehicle = true;
    this.showValidationErrors = true;

    this.editingUserVehicleName = null;
    this.selectedValues = {};
    this.userVehicleEdit = new UserVehicle();
    this.userVehicleEdit.vehicleStatus = "PENDING";
    return this.userVehicleEdit;
  }

  editUserVehicle(userVehicle: UserVehicle) {
    if (userVehicle) {
      this.isNewUserVehicle = false;
      this.showValidationErrors = true;

      this.editingUserVehicleName = userVehicle.plateNumber;
      this.selectedValues = {};
      this.userVehicleEdit = new UserVehicle();
      Object.assign(this.userVehicleEdit, userVehicle);
      return this.userVehicleEdit;
    }
    else {
      return this.newUserVehicle();
    }
  }
}
