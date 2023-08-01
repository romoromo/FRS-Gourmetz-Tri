import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { NotificationEvent } from 'src/app/models/meal-order/notification-event.model';
import { NotificationService } from 'src/app/services/notification.service';


@Component({
  selector: 'notification-event-editor',
  templateUrl: './notification-event-editor.component.html',
  styleUrls: ['./notification-event-editor.component.css']
})
export class NotificationEventEditorComponent {

  private isNewNotificationEvent = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingNotificationEventCode: string;
  private notificationEventEdit: NotificationEvent = new NotificationEvent();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  private eventTypes = ['Order', 'User', 'Admin'];

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private notificationService: NotificationService, private accountService: AccountService,
    public dialogRef: MatDialogRef<NotificationEventEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.notificationEvent) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.notificationEvent.id) {
        this.editNotificationEvent(data.notificationEvent);
      } else {
        this.newNotificationEvent();
      }
    }
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    

    if (this.isNewNotificationEvent) {
      this.notificationService.newNotificationEvent(this.notificationEventEdit).subscribe(notificationEvent => this.saveSuccessHelper(notificationEvent), error => this.saveFailedHelper(error));
    }
    else {
      this.notificationService.updateNotificationEvent(this.notificationEventEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(notificationEvent?: NotificationEvent) {
    if (notificationEvent)
      Object.assign(this.notificationEventEdit, notificationEvent);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewNotificationEvent)
      this.alertService.showMessage("Success", `\"${this.notificationEventEdit.title}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.notificationEventEdit.title}\" was saved successfully`, MessageSeverity.success);


    this.notificationEventEdit = new NotificationEvent();
    this.resetForm();


    //if (!this.isNewNotificationEvent && this.accountService.currentUser.facilities.some(r => r == this.editingNotificationEventCode))
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
    this.notificationEventEdit = new NotificationEvent();

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


  newNotificationEvent() {
    this.isNewNotificationEvent = true;
    this.showValidationErrors = true;

    this.editingNotificationEventCode = null;
    this.selectedValues = {};
    this.notificationEventEdit = new NotificationEvent();
    return this.notificationEventEdit;
  }

  editNotificationEvent(notificationEvent: NotificationEvent) {
    if (notificationEvent) {
      this.isNewNotificationEvent = false;
      this.showValidationErrors = true;

      this.editingNotificationEventCode = notificationEvent.title;
      this.selectedValues = {};
      this.notificationEventEdit = new NotificationEvent();
      Object.assign(this.notificationEventEdit, notificationEvent);

      return this.notificationEventEdit;
    }
    else {
      return this.newNotificationEvent();
    }
  }



  get canManageNotificationEvents() {
    return true;//this.accountService.userHasPermission(Permission.manageMOSOrderMgtNotificationEventsPermission)
  }
}
