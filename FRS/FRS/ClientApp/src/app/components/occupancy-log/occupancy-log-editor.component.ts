import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { OccupancyLog } from 'src/app/models/occupancy-log.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { OccupancyLogService } from 'src/app/services/occupancy-log.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'occupancy-log-editor',
  templateUrl: './occupancy-log-editor.component.html',
  styleUrls: ['./occupancy-log-editor.component.css'],
  animations: [fadeInOut]
})
export class OccupancyLogEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewOccupancyLog = false;
  private originalOccupancyLog: OccupancyLog = new OccupancyLog();
  private occupancyLogEdit: OccupancyLog = new OccupancyLog();
  private allInstitutions: Institution[] = [];
  private validation = { code: false, label: false};
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @ViewChild('code')
  private code;

  @ViewChild('label')
  private label;

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private occupancyLogService: OccupancyLogService,
    public dialogRef: MatDialogRef<OccupancyLogEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.occupancyLog) != typeof (undefined)) {
      if (data.occupancyLog.id) {
        this.editOccupancyLog(data.occupancyLog);
      } else {
        this.newOccupancyLog();
      }
    }
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  private save() {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");


      if (!this.occupancyLogEdit.id) {
        this.occupancyLogService.newOccupancyLog(this.occupancyLogEdit).subscribe(occupancyLog => this.saveSuccessHelper(occupancyLog), error => this.saveFailedHelper(error));
      }
      else {
        this.occupancyLogService.updateOccupancyLog(this.occupancyLogEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
  }

  private saveSuccessHelper(occupancyLog?: OccupancyLog) {
    if (occupancyLog)
      Object.assign(this.occupancyLogEdit, occupancyLog);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewOccupancyLog) {
      this.alertService.showMessage("Success", `Data \"${this.occupancyLogEdit.datetime}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.occupancyLogEdit.datetime}\" was saved successfully`, MessageSeverity.success);
    }

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    //if (this.changesFailedCallback)
    //  this.changesFailedCallback();

    //this.dialogRef.close();
  }


  private cancel() {
    this.occupancyLogEdit = new OccupancyLog();

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

  loadInstitutions() {
    this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.allInstitutions = results[0];

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve institutions from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  newOccupancyLog() {
    this.showValidationErrors = true;
    this.isNewOccupancyLog = true;
    this.occupancyLogEdit = new OccupancyLog();
    return this.occupancyLogEdit;
  }

  editOccupancyLog(occupancyLog: OccupancyLog) {
    if (occupancyLog) {
      this.isNewOccupancyLog = false;
      this.showValidationErrors = true;
      this.originalOccupancyLog = occupancyLog;
      this.occupancyLogEdit = new OccupancyLog();
      Object.assign(this.occupancyLogEdit, occupancyLog);

      return this.occupancyLogEdit;
    }
    else {
      return this.newOccupancyLog();
    }
  }

  get canManageOccupancyLogs() {
    return true;//this.accountService.userHasPermission(Permission.manageOccupancyLogPermission);
  }

}
