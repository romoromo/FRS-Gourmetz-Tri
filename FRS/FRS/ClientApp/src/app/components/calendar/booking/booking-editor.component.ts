import { Component, ViewChild, Input, ChangeDetectorRef, ViewEncapsulation, Inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatChipInputEvent } from '@angular/material';
import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { ReservationService } from '../../../services/reservation.service';
import { Reservation, ReservationInvitee } from '../../../models/reservation.model';
import { Permission } from '../../../models/permission.model';
import { LocationService } from 'src/app/services/location.service';
import { Location } from '../../../models/location.model';
import { FormControl, Validators } from '@angular/forms';
import { ReservationEditorComponent } from '../../reservations/reservation-editor.component';
import { ReservationTime, TimeIntervalFilter } from 'src/app/models/reservationtime.model';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { User } from 'src/app/models/user.model';


@Component({
  selector: 'booking-editor',
  templateUrl: './booking-editor.component.html',
  styleUrls: ['./booking-editor.component.css']
})
export class BookingEditorComponent implements OnInit {

  private isEditMode = false;
  private isNewReservation = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingReservationName: string;
  private facilityNames: string;
  private reservationEdit: Reservation = new Reservation();
  private locationList: Location[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  fcStartTimes = new FormControl();
  fcEndTimes = new FormControl();
  fcReservationInvitees = new FormControl();
  private allStartTimes: ReservationTime[] = [];
  private allEndTimes: ReservationTime[] = [];
  private allTimes: ReservationTime[] = [];
  startDate: string;
  selectStartTimeId: number;
  selectEndTimeId: number;
  locationCapacity: number;
  disabledTime = false;
  fcLocations = new FormControl('', [Validators.required]);
  filteredReservationInvitees: Observable<User[]>;
  users: User[] = [];
  inviteeList: User[] = [];
  inputInvitee: User;
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @Input()
  isViewOnly: boolean;

  constructor(private cdRef: ChangeDetectorRef, private alertService: AlertService, private accountService: AccountService, private reservationService: ReservationService, private locationService: LocationService,
    public dialogRef: MatDialogRef<ReservationEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    if (typeof (data.reservation) != typeof (undefined)) {
      //if (data.reservation.id) {
      //  this.editReservation(data.reservation, data.location);
      //} else {
      //  this.newReservation(data.location);
      //}
      this.editReservation(data.reservation, data.location, data.timeIntervals);
      this.loadLocations();
      this.allStartTimes = data.allTimeIntervals;
      this.allTimes = data.allTimeIntervals;
      this.fcStartTimes.valueChanges.subscribe(x => {
        this.changeEndTime(x);
      });
    }


  }

  ngOnInit() {
    this.accountService.getUsers(null, null, this.accountService.currentUser.institutionId).subscribe(results => {
      this.users = results;

      this.filteredReservationInvitees = this.fcReservationInvitees.valueChanges
        .pipe(
          startWith<string | User>(''),
          map(value => typeof value === 'string' ? value : value.fullName),
          map(fullName => fullName ? this._filter(fullName) : this.users.slice())
        );
    });
  }

  displayFn(user?: User): string | undefined {
    return user ? user.fullName : undefined;
  }

  selectInvitee($ev) {
    this.inviteeList.push($ev.option.value);
    this.inputInvitee = new User();
    this.users = this.users.filter(item => item.id !== $ev.option.value.id);
  }

  removeInvitee(invitee) {
    this.inviteeList = this.inviteeList.filter(item => item.id !== invitee.id);
    this.users.push(invitee);
  }

  private _filter(fullName: string): User[] {
    const filterValue = fullName.toLowerCase();

    return this.users.filter(option => option.fullName.toLowerCase().indexOf(filterValue) === 0);
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.reservationEdit.invitees = [];
    this.inviteeList.forEach((invitee, index, invitees) => {
      var resInvitee = new ReservationInvitee();
      resInvitee.userId = invitee.id;
      this.reservationEdit.invitees.push(resInvitee);
    });

    //this.reservationEdit.startTime = this.getReservationTimeBy(this.selectStartTimeId);
    //this.reservationEdit.endTime = this.getReservationTimeBy(this.selectEndTimeId);
    var startDateTime = new Date(this.reservationEdit.startDate);

    //startDateTime.setHours(this.reservationEdit.startTime.hour);
    //startDateTime.setMinutes(this.reservationEdit.startTime.minute);
    this.reservationEdit.startDateTime = startDateTime;

    //var endDateTime = new Date(this.reservationEdit.endDate);

    //endDateTime.setHours(this.reservationEdit.endTime.hour);
    //endDateTime.setMinutes(this.reservationEdit.endTime.minute);
    //this.reservationEdit.endDateTime = endDateTime;
    //this.reservationEdit.repeatEndDateTime = new Date(this.reservationEdit.repeatEndDate);
    this.reservationService.newReservation(this.reservationEdit).subscribe(reservation => this.saveSuccessHelper(reservation), error => this.saveFailedHelper(error));

  }

  private saveSuccessHelper(reservation?: Reservation) {
    if (reservation)
      Object.assign(this.reservationEdit, reservation);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    this.alertService.showMessage("Success", `Reservation \"${this.reservationEdit.shortDescription}\" was created successfully`, MessageSeverity.success);

    this.reservationEdit = new Reservation();
    this.resetForm();


    //if (!this.isNewReservation && this.accountService.currentUser.reservations.some(r => r == this.editingReservationName))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
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


  newReservation(location: Location) {
    this.isEditMode = true;
    this.isNewReservation = true;
    this.showValidationErrors = true;

    this.editingReservationName = null;
    this.selectedValues = {};
    this.reservationEdit = new Reservation();
    this.reservationEdit.locationName = location.name;
    this.reservationEdit.locationId = location.id;
    this.reservationEdit.facilities = location.facilities;
    return this.reservationEdit;
  }

  editReservation(reservation: Reservation, location: Location, timeIntervals: ReservationTime[]) {
    this.isEditMode = true;
    if (reservation) {
      this.isNewReservation = false;
      this.showValidationErrors = true;

      this.editingReservationName = reservation.shortDescription;
      this.selectedValues = {};
      this.reservationEdit = new Reservation();
      Object.assign(this.reservationEdit, reservation);

      this.reservationEdit.repeatType = "None";
      this.startDate = (new DateOnlyPipe('en-SG')).transform(this.reservationEdit.startDate);
      this.reservationEdit.locationName = location.name;
      this.reservationEdit.locationId = location.id;
      this.reservationEdit.facilities = location.facilities;
      this.reservationEdit.timeIntervals = timeIntervals;

      this.locationCapacity = location.capacity;
      //this.selectStartTimeId = this.reservationEdit.startTime.id;
      //this.selectEndTimeId = this.reservationEdit.endTime.id;
      return this.reservationEdit;
    }
    else {
      return this.newReservation(location);
    }
  }

  loadLocations() {
    //this.locationService.getLocationsAndLocationTypes()
    //  .subscribe(results => {
    //    let locations = results[0];
    //    this.locationsList = locations;
    //  },
    //    error => {
    //      this.alertService.resetStickyMessage();
    //      this.alertService.showStickyMessage("getLocationsByLocationId failed", "An error occured while trying to get locations from the server", MessageSeverity.error);
    //    });
  }


  changeLocations(id) {
    //this.locationService.getLocationsByLocationId(id)
    //  .subscribe(results => {
    //    let locations = results[0];
    //    this.locationsList = locations;
    //  },
    //    error => {
    //      this.alertService.resetStickyMessage();
    //      this.alertService.showStickyMessage("getLocationsByLocationId failed", "An error occured while trying to get locations from the server", MessageSeverity.error);
    //    });
  }

  setReservationTypeId(id) {
    //this.reservationEdit.reservationTypeId = id;
  }


  public changeEndTime(id) {
    if (id != null && id != null && id.toString() != "") {
      var allTimes = this.allTimes;
      var resStartTime = this.getReservationTimeBy(id);
      var $this = this;
      this.allEndTimes = allTimes.filter(function (p, index, array) {
        if (new Date(new Date(p.value).setSeconds(0)) > new Date(new Date(resStartTime.value))) {
          p.description = p.description;
          return p;
        }
      });
    }
  }

  public getHourDifferenceDescription(endTime: ReservationTime): string {
    var diff = new Date(new Date(endTime.value).setSeconds(0)).valueOf() - new Date(new Date(this.getReservationTimeBy(this.selectStartTimeId).value).setSeconds(0)).valueOf();
    var diffHours = diff / (1000.0 * 3600.0);

    return diffHours.toFixed(1).toString();
  }

  private getReservationTimeBy(id?: any, hour?: number, minute?: number): ReservationTime {
    var allTimes = this.allTimes;
    var time: ReservationTime;
    if (id != null) {
      time = allTimes.filter(f => f.id == id)[0];
    } else {
      var t = allTimes.filter(f => f.hour == hour && f.minutes == minute);
      time = t != null && t.length > 0 ? t[0] : allTimes[0];
    }
    return time;
  }

  get canManageReservations() {
    return this.accountService.userHasPermission(Permission.manageReservationsPermission)
  }
}
