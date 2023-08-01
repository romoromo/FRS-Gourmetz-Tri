import { Component, ViewChild, Input } from '@angular/core';
import { fadeInOut } from '../../../services/animations';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { ReservationService } from '../../../services/reservation.service';
import { Reservation } from '../../../models/reservation.model';
import { Permission } from '../../../models/permission.model';
import { Facility } from '../../../models/facility.model';
import { Location } from '../../../models/location.model';
import { ReservationTime } from '../../../models/reservationtime.model';

@Component({
  selector: 'reservation-list-editor',
  templateUrl: './reservation-editor.component.html',
  styleUrls: ['./reservation-editor.component.css'],
  animations: [fadeInOut]
})
export class ReservationListEditorComponent {

  private isEditMode = false;
  private isNewReservation = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingReservationName: string;
  private reservationEdit: Reservation = new Reservation();
  private allFacilities: Facility[] = [];
  private allLocations: Location[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  private allStartTimes: ReservationTime[] = [];
  private allEndTimes: ReservationTime[] = [];

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public disabledTime = false;

  @ViewChild('f')
  private form;

  @ViewChild('facilities')
  private facilities;

  @ViewChild('locationsSelector')
  private locationsSelector;

  @ViewChild('allStartTimesSelector')
  private allStartTimesSelector;

  @ViewChild('allEndTimesSelector')
  private allEndTimesSelector;

  @ViewChild('startTime')
  private startTime;

  @ViewChild('endTime')
  private endTime;

  @Input()
  isViewOnly: boolean;

  constructor(private alertService: AlertService, private accountService: AccountService, private reservationService: ReservationService) {
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {

    if (!this.reservationEdit.isAllDay) {
      if (this.reservationEdit.startTime == null) {
        this.startTime.valid = false;
      }

      if (this.reservationEdit.endTime == null) {
        this.endTime.valid = false;
      }

      if (!this.startTime.valid || !this.endTime.valid) return;
    }
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    this.reservationEdit.startTime = this.getReservationTimeBy(this.reservationEdit.startTime.id);
    this.reservationEdit.endTime = this.getReservationTimeBy(this.reservationEdit.endTime.id);
    var startDateTime = new Date(this.reservationEdit.startDate);

    startDateTime.setHours(this.reservationEdit.startTime.hour);
    startDateTime.setMinutes(this.reservationEdit.startTime.minutes);
    this.reservationEdit.startDateTime = startDateTime;

    var endDateTime = new Date(this.reservationEdit.endDate);

    endDateTime.setHours(this.reservationEdit.endTime.hour);
    endDateTime.setMinutes(this.reservationEdit.endTime.minutes);
    this.reservationEdit.endDateTime = endDateTime;
    this.reservationEdit.repeatEndDateTime = new Date(this.reservationEdit.repeatEndDate);
    if (this.isNewReservation) {
      this.reservationService.newReservation(this.reservationEdit).subscribe(reservation => this.saveSuccessHelper(reservation), error => this.saveFailedHelper(error));
    }
    else {
      this.reservationService.updateReservation(this.reservationEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(reservation?: Reservation) {
    if (reservation) {
      Object.assign(this.reservationEdit, reservation);
    }

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewReservation)
      this.alertService.showMessage("Success", `Reservation \"${this.reservationEdit.shortDescription}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to reservation \"${this.reservationEdit.shortDescription}\" was saved successfully`, MessageSeverity.success);


    this.reservationEdit = new Reservation();
    this.resetForm();


    //if (!this.isNewReservation && this.accountService.currentUser.reservations.some(r => r == this.editingReservationName))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();
  }


  private refreshLoggedInUser() {
    this.accountService.refreshLoggedInUser()
      .subscribe(user => { },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("Refresh failed", "An error occured while refreshing logged in user information from the server", MessageSeverity.error);
        });
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
    this.reservationEdit = new Reservation();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();
  }

  private getAllTimes(): ReservationTime[] {
    let times: ReservationTime[] =
      [new ReservationTime(1, "00:00", "00:00", 0, 0),
      new ReservationTime(2, "00:30", "00:29", 0, 29),
      new ReservationTime(3, "01:00", "01:00", 1, 59),
      new ReservationTime(4, "01:30", "02:29", 1, 29),
      new ReservationTime(5, "02:00", "02:00", 2, 59),
      new ReservationTime(6, "02:30", "03:29", 2, 29),
      new ReservationTime(7, "03:00", "03:00", 3, 59),
      new ReservationTime(8, "03:30", "04:29", 3, 29),
      new ReservationTime(9, "04:00", "04:00", 4, 59),
      new ReservationTime(10, "04:30", "05:29", 4, 29),
      new ReservationTime(11, "05:00", "05:00", 5, 59),
      new ReservationTime(12, "05:30", "06:29", 5, 29),
      new ReservationTime(13, "06:00", "06:00", 6, 59),
      new ReservationTime(14, "06:30", "07:29", 6, 29),
      new ReservationTime(15, "07:00", "07:00", 7, 59),
      new ReservationTime(16, "07:30", "08:29", 7, 29),
      new ReservationTime(17, "08:00", "08:00", 8, 59),
      new ReservationTime(18, "08:30", "09:29", 8, 29),
      new ReservationTime(19, "09:00", "09:00", 9, 59),
      new ReservationTime(20, "09:30", "10:29", 9, 29),
      new ReservationTime(21, "10:00", "10:00", 10, 59),
      new ReservationTime(22, "10:30", "11:29", 11, 29),
      new ReservationTime(23, "11:00", "11:00", 10, 59),
      new ReservationTime(24, "11:30", "12:29", 11, 29),
      new ReservationTime(25, "12:00", "12:00", 12, 59),
      new ReservationTime(26, "12:30", "13:29", 12, 29),
      new ReservationTime(27, "13:00", "13:00", 13, 59),
      new ReservationTime(28, "13:30", "14:29", 13, 29),
      new ReservationTime(29, "14:00", "14:00", 14, 59),
      new ReservationTime(29, "14:30", "15:29", 14, 29),
      new ReservationTime(30, "15:00", "15:00", 15, 59),
      new ReservationTime(32, "15:30", "16:29", 15, 29),
      new ReservationTime(33, "16:00", "16:00", 16, 59),
      new ReservationTime(34, "16:30", "17:29", 16, 29),
      new ReservationTime(35, "17:00", "17:00", 17, 59),
      new ReservationTime(36, "17:30", "18:29", 17, 29),
      new ReservationTime(37, "18:00", "18:00", 18, 59),
      new ReservationTime(38, "18:30", "19:29", 18, 29),
      new ReservationTime(39, "19:00", "19:00", 19, 59),
      new ReservationTime(40, "19:30", "20:29", 19, 29),
      new ReservationTime(41, "20:00", "20:00", 20, 59),
      new ReservationTime(42, "20:30", "21:29", 20, 29),
      new ReservationTime(43, "21:00", "21:00", 21, 59),
      new ReservationTime(44, "21:30", "22:29", 21, 29),
      new ReservationTime(45, "22:00", "22:00", 22, 59),
      new ReservationTime(46, "22:30", "23:29", 22, 29),
      new ReservationTime(47, "23:00", "23:00", 22, 59),
      new ReservationTime(48, "23:30", "23:29", 23, 29),
      new ReservationTime(49, "24:00", "23:59", 23, 59)];
    return times;
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

  public toggleTime() {
    this.disabledTime = !this.reservationEdit.isAllDay;
    setTimeout(() => this.allStartTimesSelector.refresh());
    setTimeout(() => this.allEndTimesSelector.refresh());
  }
  public changeEndTime(id) {
    if (this.reservationEdit.startTime != null && this.reservationEdit.startTime.id != null && this.reservationEdit.startTime.id.toString() != "") {
      var allTimes = this.getAllTimes();
      var startTime = this.reservationEdit.startTime;
      this.reservationEdit.startTime = this.getReservationTimeBy(this.reservationEdit.startTime.id);
      var $this = this;
      this.allEndTimes = allTimes.filter(function (p, index, array) {
        if (new Date(new Date(p.value).setSeconds(0)) > new Date(startTime.value)) {
          p.description = p.description + " (" + $this.getHourDifferenceDescription(p) + " hours)";
          return p;
        }
      });

      setTimeout(() => this.allEndTimesSelector.refresh());
    }
  }

  public getHourDifferenceDescription(endTime: ReservationTime): string {
    var diff = new Date(new Date(endTime.value).setSeconds(0)).valueOf() - new Date(new Date(this.reservationEdit.startTime.value).setSeconds(0)).valueOf();
    var diffHours = diff / (1000.0 * 3600.0);

    return diffHours.toFixed(1).toString();
  }

  private getReservationTimeBy(id?: any, hour?: number, minute?: number): ReservationTime {
    var allTimes = this.getAllTimes();
    var time: ReservationTime;
    if (id != null) {
      time = allTimes.filter(f => f.id == id)[0];
    } else {
      var t = allTimes.filter(f => f.hour == hour && f.minutes == minute);
      time = t != null && t.length > 0 ? t[0] : allTimes[0];
    }
    return time;
  }

  newReservation(allLocations: Location[]) {
    this.isEditMode = true;
    this.isNewReservation = true;
    this.showValidationErrors = true;
    this.disabledTime = false;
    this.editingReservationName = null;
    this.allLocations = allLocations;
    this.selectedValues = {};
    this.reservationEdit = new Reservation();
    this.reservationEdit.repeatType = "None";
    this.reservationEdit.startDate = new Date();
    this.reservationEdit.endDate = new Date();

    this.allStartTimes = this.getAllTimes();
    this.reservationEdit.startTime = this.allStartTimes[0];
    this.reservationEdit.endTime = this.allStartTimes[47];
    //this.allEndTimes = this.getAllTimes();
    setTimeout(() => this.allStartTimesSelector.refresh());
    //setTimeout(() => this.allEndTimesSelector.refresh());
    return this.reservationEdit;
  }

  editReservation(reservation: Reservation, allLocations: Location[]) {
    this.isEditMode = true;
    if (reservation) {
      this.isNewReservation = false;
      this.showValidationErrors = true;
      var allT = this.getAllTimes();
      this.allStartTimes = allT;
      this.allEndTimes = allT;
      setTimeout(() => this.allStartTimesSelector.refresh());
      this.editingReservationName = reservation.shortDescription;
      this.selectedValues = {};
      this.setReservationTypes(reservation, allLocations);
      this.reservationEdit = new Reservation();
      Object.assign(this.reservationEdit, reservation);
      this.disabledTime = this.reservationEdit.isAllDay;
      this.reservationEdit.repeatEndDate = new Date(reservation.repeatEndDateTime);
      var startTime = new Date(reservation.startDateTime);
      var endTime = new Date(reservation.endDateTime);

      this.reservationEdit.startDate = new Date(startTime);
      this.reservationEdit.endDate = new Date(endTime);

      this.reservationEdit.startTime = this.getReservationTimeBy(null, startTime.getHours(), startTime.getMinutes());
      this.reservationEdit.endTime = this.getReservationTimeBy(null, endTime.getHours(), endTime.getMinutes());

      return this.reservationEdit;
    }
    else {
      return this.newReservation(allLocations);
    }
  }

  private setReservationTypes(reservation: Reservation, allLocations?: Location[]) {

    this.allLocations = allLocations ? [...allLocations] : [];

    if (allLocations == null || this.allLocations.length != allLocations.length)
      setTimeout(() => this.locationsSelector.refresh());
  }

  get canManageReservations() {
    return this.accountService.userHasPermission(Permission.manageReservationsPermission)
  }
}
