import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { TrackingStatus } from 'src/app/models/meal-order/tracking-status.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { CatererInfo } from '../../../models/meal-order/caterer-info.model';


@Component({
  selector: 'tracking-status-editor',
  templateUrl: './tracking-status-editor.component.html',
  styleUrls: ['./tracking-status-editor.component.css']
})
export class TrackingStatusEditorComponent {

  private isNewTrackingStatus = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingTrackingStatusCode: string;
  private trackingStatusEdit: TrackingStatus = new TrackingStatus();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];
  private catererInfos: CatererInfo[] = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private deliveryService: DeliveryService, private accountService: AccountService,
    public dialogRef: MatDialogRef<TrackingStatusEditorComponent>, private mealService: MealService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.trackingStatus) != typeof (undefined)) {
      if (data.trackingStatus.id) {
        this.editTrackingStatus(data.trackingStatus);
      } else {
        this.newTrackingStatus();
      }
    }

    this.getCatererInfos();
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.trackingStatusEdit.institutionId = this.accountService.currentUser.institutionId;
    console.log(this.periods);
    let selectedPeriods = this.periods.filter(f => f.checked);
    

    if (this.isNewTrackingStatus) {
      this.deliveryService.newTrackingStatus(this.trackingStatusEdit).subscribe(trackingStatus => this.saveSuccessHelper(trackingStatus), error => this.saveFailedHelper(error));
    }
    else {
      this.deliveryService.updateTrackingStatus(this.trackingStatusEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(trackingStatus?: TrackingStatus) {
    if (trackingStatus)
      Object.assign(this.trackingStatusEdit, trackingStatus);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewTrackingStatus)
      this.alertService.showMessage("Success", `Tracking status \"${this.trackingStatusEdit.status}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Tracking status \"${this.trackingStatusEdit.status}\" was saved successfully`, MessageSeverity.success);


    this.trackingStatusEdit = new TrackingStatus();
    this.resetForm();


    //if (!this.isNewTrackingStatus && this.accountService.currentUser.facilities.some(r => r == this.editingTrackingStatusCode))
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
    this.trackingStatusEdit = new TrackingStatus();

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


  newTrackingStatus() {
    this.isNewTrackingStatus = true;
    this.showValidationErrors = true;

    this.editingTrackingStatusCode = null;
    this.selectedValues = {};
    this.trackingStatusEdit = new TrackingStatus();

    return this.trackingStatusEdit;
  }

  editTrackingStatus(trackingStatus: TrackingStatus) {
    if (trackingStatus) {
      this.isNewTrackingStatus = false;
      this.showValidationErrors = true;

      this.editingTrackingStatusCode = trackingStatus.status;
      this.selectedValues = {};
      this.trackingStatusEdit = new TrackingStatus();
      Object.assign(this.trackingStatusEdit, trackingStatus);

      return this.trackingStatusEdit;
    }
    else {
      return this.newTrackingStatus();
    }
  }

  getCatererInfos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getCatererInfosByFilter(filter)
      .subscribe(results => {
        this.catererInfos = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving caterers.\r\n"`,
            MessageSeverity.error);
        })
  }



  get canManageTrackingStatuss() {
    return true; //this.accountService.userHasPermission(Permission.manageTrackingStatussPermission)
  }
}
