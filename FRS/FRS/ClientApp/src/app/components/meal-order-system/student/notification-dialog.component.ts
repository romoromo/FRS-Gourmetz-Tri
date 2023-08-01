import { Component, ViewChild, Inject, OnInit, OnDestroy } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Student, StudentCard, StudentInterestGroup, StudentRestriction } from 'src/app/models/meal-order/student.model';
import { CommonFilter, Filter } from 'src/app/models/sieve-filter.model';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { NotificationService } from 'src/app/services/notification.service';
import { Utilities } from 'src/app/services/utilities';


@Component({
  selector: 'student-notification-dialog',
  template: `
    <h1 mat-dialog-title>{{data.header}}</h1>
    <div>
      <form *ngIf="formResetToggle" class="form-horizontal" name="notificationEventEditorForm" #f="ngForm" novalidate
            (ngSubmit)="f.form.valid ? save() :
              (!evId.valid && showErrorAlert('Notification Event is required', ''));">
        <div mat-dialog-content>
          <div class="row">
            <mat-form-field class="col-md-12">
              <mat-label>Type</mat-label>
              <mat-select id="evId" name="evId" [(ngModel)]="notificationEventId" #evId="ngModel" required>
                <mat-option *ngFor="let notificationEvent of notificationEvents" [value]="notificationEvent.id">
                  {{notificationEvent.title}}
                </mat-option>
              </mat-select>
            </mat-form-field>
          </div>

          <div class="clearfix"></div>
        </div>
        <div mat-dialog-actions align="center">
          <button type="button" mat-button (click)="cancel()" [disabled]="isSaving"><i class='fa fa-times'></i> {{'common.editor.Cancel' | translate}}</button>
          <button type="submit" mat-button [disabled]="isSaving">
            <i *ngIf="!isSaving" class='fa fa-save'></i><i *ngIf="isSaving" class='fa fa-circle-o-notch fa-spin'></i> {{isSaving ? 'Sending' : 'Send'}}
          </button>
        </div>
      </form>
    </div>

  `
})
export class StudentNotificationDialogComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  public formResetToggle = true;

  public student: Student;
  public notificationEventId: any;
  public notificationEvents =  [];
  public isSaving = false;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  loadingIndicator: boolean;

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private notificationService: NotificationService, 
    public dialogRef: MatDialogRef<StudentNotificationDialogComponent>, public dialog: MatDialog, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.student = data.student;
    this.getNotificationEvents();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  getNotificationEvents() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.notificationService.getNotificationEventsByFilter(filter)
      .subscribe(results => {
        this.notificationEvents = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving notification events.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.notificationService.sendStudentNotification(this.notificationEventId, this.student.email, this.student.userId)
      .subscribe(results => {
        this.loadingIndicator = false;
        this.alertService.stopLoadingMessage();
        this.alertService.showMessage("Success", `Notification sent!`, MessageSeverity.success);
        this.alertService.resetStickyMessage();
        this.dialogRef.close();
      },
        error => {
          this.isSaving = false;
          this.loadingIndicator = false;
          this.alertService.stopLoadingMessage();
          this.alertService.showStickyMessage("Sending Error", `An error occured while triggering notification.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  private cancel() {
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

  get canManageStudents() {
    return true; //this.accountService.userHasPermission(Permission.manageStudentsPermission)
  }
}
