import { Component, ViewChild, Input, ChangeDetectorRef, ViewEncapsulation, Inject, OnInit, AfterViewInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatChipInputEvent } from '@angular/material';
import { AlertService, MessageSeverity, DialogType } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { BookingService } from '../../../services/booking.service';
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
import { Utilities } from 'src/app/services/utilities';
import { ReservationService } from 'src/app/services/reservation.service';
import { ContactGroup } from 'src/app/models/contactGroup.model';
import { ContactGroupSelectorComponent } from '../../common/contact-group-selector/contact-group-selector.component';
import { UserPhonebookSelectorComponent } from '../../common/userphonebook-selector/userphonebook-selector.component';
import { UserPhonebook } from 'src/app/models/userphonebook.model';
import { UserVehicle } from 'src/app/models/uservehicle.model';


@Component({
  selector: 'calendar-booking-grid-editor',
  templateUrl: './calendar-booking-grid-editor.component.html',
  styleUrls: ['./calendar-booking-grid-editor.component.css']
})
export class CalendarBookingGridEditorComponent implements OnInit, AfterViewInit {

  private isEditMode = false;
  private isNewReservation = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingReservationName: string;
  private facilityNames: string;
  private reservationOrig: Reservation = new Reservation();
  private reservationEdit: Reservation = new Reservation();
  repeatTypes: string[] = ['None', 'Daily', 'Weekly', 'Monthly', 'Yearly'];
  private locationList: Location[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  fcStartTimes = new FormControl();
  fcEndTimes = new FormControl();
  fcReservationInvitees = new FormControl();
  fcSelectedStartTime = new FormControl();
  fcSelectedEndTime = new FormControl();

  private allStartTimes: ReservationTime[] = [];
  private allEndTimes: ReservationTime[] = [];
  private allTimes: ReservationTime[] = [];
  startDate: string;
  selectedStartTime: string;
  selectedEndTime: string;
  selectStartTimeId: number;
  selectEndTimeId: number;
  locationCapacity: number;
  disabledTime = false;
  fcLocations = new FormControl('', [Validators.required]);
  filteredReservationInvitees: Observable<User[]>;
  users: User[] = [];
  allUsers: User[] = [];
  inviteeList: User[] = [];
  contactGroups: ContactGroup[] = [];
  contactGroupList: ContactGroup[] = [];
  userPhonebooks: UserPhonebook[] = [];

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
    @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog) {
    if (typeof (data.reservation) != typeof (undefined)) {
      //if (data.reservation.id) {
      //  this.editReservation(data.reservation, data.location);
      //} else {
      //  this.newReservation(data.location);
      //}
      if (data.reservation.invitees) {
        var reservationInvitees: ReservationInvitee[] = data.reservation.invitees;
        reservationInvitees.forEach((invitee, index, invitees) => {
          var u = new User();
          console.log(invitee);
          u.fullName = invitee.name;
          u.id = invitee.userId;
          u.email = invitee.email;
          u.cardType = invitee.cardType;
          u.plateNumber = invitee.plateNumber;
          if (!invitee.contactGroupId) {
            this.inviteeList.push(u);
          }
        });

      }

      Object.assign(this.reservationOrig, data.reservation);
      this.editReservation(data.reservation, data.location, data.timeIntervals);
      this.loadLocations();
      this.allStartTimes = data.allTimeIntervals;
      this.allTimes = data.allTimeIntervals;
      this.fcStartTimes.valueChanges.subscribe(x => {
        this.changeEndTime(x);
      });
    }

    if (!this.reservationOrig.smartRoomScheduleId) {

      this.alertService.startLoadingMessage("Loading contact list...");
      this.accountService.getUsers(null, null, this.accountService.currentUser.institutionId).subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.allUsers = results;
        var invList = this.inviteeList;
        this.users = results;
        this.users = this.users.filter(function (item) {
          return invList.filter(function (i) {
            return i.id == item.id;
          }).length == 0;
        });

        for (var i in this.inviteeList) {
          this.inviteeList[i].userVehicles = this.getUserVehicles(this.inviteeList[i].email);
        }

        this.filteredReservationInvitees = this.fcReservationInvitees.valueChanges
          .pipe(
            startWith<string | User>(''),
            map(value => typeof value === 'string' ? value : value.fullName),
            map(fullName => fullName ? this._filter(fullName) : this.users.slice())
          );
      });
    }


  }

  inviteeSearchListLength: number = 0;
  ngOnInit() {
    
  }

  ngAfterViewInit() {
    this.form.valueChanges.subscribe((change) => {
      //console.log(change)
    })
  }

  displayFn(user?: User): string | undefined {
    return user ? user.fullName : undefined;
  }

  setUserVehicle(row: User, vehicle: UserVehicle) {
    if (vehicle) {
      row.cardType = vehicle.cardType;
      row.plateNumber = vehicle.plateNumber;
    }
  }

  getUserVehicles(email: string) {
    //invitee.userVehicles
    var users: User[] = this.allUsers.filter(item => item.email == email);
    if (users && users.length > 0) {
      return users[0].userVehicles;
    }

    return [];
    
  }

  selectInvitee($ev) {
    let invitee = $ev.option.value;
    if (!invitee.userVehicles) {
      invitee.userVehicles = this.getUserVehicles(invitee.email);
    }

    this.inviteeList.push(invitee);
    this.inputInvitee = new User();
    this.users = this.users.filter(item => item.id !== $ev.option.value.id);
  }

  addInvitee() {
    if (this.fcReservationInvitees.value) {
      var exist = this.inviteeList.filter(item => item.email == this.fcReservationInvitees.value);
      if (exist.length == 0) {
        var invitee = new User();
        invitee.email = this.fcReservationInvitees.value;
        this.inviteeList.push(invitee);
      }
    }
  }

  removeInvitee(invitee) {
    if (invitee.id) {
      this.inviteeList = this.inviteeList.filter(item => item.id !== invitee.id);
    } else {
      this.inviteeList = this.inviteeList.filter(item => item.email !== invitee.email);
    }
    this.users.push(invitee);
  }

  private _filter(fullName: string): User[] {
    const filterValue = fullName.toLowerCase();

    return this.users.filter(option => option && option.fullName && option.fullName.toLowerCase().indexOf(filterValue) === 0);
  }

  openDialog(): void {

  }

  addContactGroup() {
    //if (this.fcReservationInvitees.value) {
    //  var exist = this.inviteeList.filter(item => item.email == this.fcReservationInvitees.value);
    //  if (exist.length == 0) {
    //    var cg = new ContactGroup();
    //    cg.email = this.fcReservationInvitees.value;
    //    this.contactGroupList.push(invitee);
    //  }
    //}
    const dialogRef = this.dialog.open(ContactGroupSelectorComponent, {
      data: { header: "Select Contact Groups", contactGroups: this.reservationEdit.contactGroups },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result.isCancel) {
        this.contactGroups = result.selectedContactGroups;
        this.reservationEdit.contactGroups = this.contactGroups;
      }
    });
  }

  removeContactGroup(contactGroup) {
    if (contactGroup.id) {
      this.reservationEdit.contactGroups = this.reservationEdit.contactGroups.filter(item => item.id !== contactGroup.id);
    }
  }

  addFromPhonebook() {
    const dialogRef = this.dialog.open(UserPhonebookSelectorComponent, {
      data: { header: "My Contacts", userPhonebooks: this.userPhonebooks },
      width: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result.isCancel) {
        this.userPhonebooks = result.selectedUserPhonebooks;
      }
    });
  }

  removeUserPhonebook(phonebook) {
    if (phonebook.id) {
      this.userPhonebooks = this.userPhonebooks.filter(item => item.id !== phonebook.id);
    }
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.reservationEdit.invitees = [];
    var isAttendeeExist = false;
    this.inviteeList.forEach((invitee, index, invitees) => {
      var resInvitee = new ReservationInvitee();
      resInvitee.userId = invitee.id;
      resInvitee.email = invitee.email;
      resInvitee.cardType = invitee.cardType;
      resInvitee.plateNumber = invitee.plateNumber;
      resInvitee.name = invitee.fullName;
      this.reservationEdit.invitees.push(resInvitee);
      if (!isAttendeeExist && (!this.reservationEdit.id || this.reservationEdit.id == "0")) {
        isAttendeeExist = this.accountService.currentUser.id == invitee.id;
      }
    });

    if (this.reservationEdit.isAttendee && !isAttendeeExist && (!this.reservationEdit.id || this.reservationEdit.id == "0")) {
      var resInvitee = new ReservationInvitee();
      resInvitee.userId = this.accountService.currentUser.id;
      resInvitee.email = this.accountService.currentUser.email;
      this.reservationEdit.invitees.push(resInvitee);
    }

    this.userPhonebooks.forEach((invitee, index, invitees) => {
      var resInvitee = new ReservationInvitee();
      resInvitee.email = invitee.email;
      resInvitee.name = invitee.name;
      resInvitee.phoneNumber = invitee.phoneNumber;
      this.reservationEdit.invitees.push(resInvitee);
    });

    //this.reservationEdit.startTime = this.getReservationTimeBy(this.selectStartTimeId);
    //this.reservationEdit.endTime = this.getReservationTimeBy(this.selectEndTimeId);
    var startDateTime = new Date(this.reservationEdit.startDate ? this.reservationEdit.startDate : this.reservationEdit.startDateTime);

    //startDateTime.setHours(this.reservationEdit.startTime.hour);
    //startDateTime.setMinutes(this.reservationEdit.startTime.minute);
    var startTime = this.selectedStartTime.split(':');
    var endTime = this.selectedEndTime.split(':');
    var v = this.fcSelectedStartTime.value;
    this.reservationEdit.startDateTime = new Date(startDateTime);
    this.reservationEdit.startDateTime.setHours(parseInt(startTime[0]), parseInt(startTime[1]));

    this.reservationEdit.endDateTime = new Date(startDateTime);

    this.reservationEdit.endDateTime.setHours(parseInt(endTime[0]), parseInt(endTime[1]));

    //var endDateTime = new Date(this.reservationEdit.endDate);

    //endDateTime.setHours(this.reservationEdit.endTime.hour);
    //endDateTime.setMinutes(this.reservationEdit.endTime.minute);
    //this.reservationEdit.endDateTime = endDateTime;
    //this.reservationEdit.repeatEndDateTime = new Date(this.reservationEdit.repeatEndDate);
    if (this.reservationEdit.id) {
      if ((this.reservationEdit.isRecurring) && (this.reservationOrig.longDescription != this.reservationEdit.longDescription ||
        this.reservationOrig.shortDescription != this.reservationEdit.shortDescription ||
        this.reservationOrig.notes != this.reservationEdit.notes ||
        this.reservationOrig.startDateTime != this.reservationEdit.startDateTime ||
        this.reservationOrig.endDateTime != this.reservationEdit.endDateTime)) {
        this.showDialog("Edit recurring event", (val) => {
          this.reservationEdit.recurApplyChangesType = "OTHER_EVENTS";
          this.reservationService.updateReservation(this.reservationEdit).subscribe(response => this.saveSuccessHelper(response.data), error => this.saveFailedHelper(error));

        },
          () => {
            this.reservationEdit.recurApplyChangesType = "ALL_EVENTS";
            this.reservationService.updateReservation(this.reservationEdit).subscribe(response => this.saveSuccessHelper(response.data), error => this.saveFailedHelper(error));

          });
      } else {
        this.reservationService.updateReservation(this.reservationEdit).subscribe(response => this.saveSuccessHelper(response.data), error => this.saveFailedHelper(error));

      }

    } else {
      this.reservationService.newReservation(this.reservationEdit).subscribe(reservation => this.saveSuccessHelper(reservation), error => this.saveFailedHelper(error));
    }

  }

  private saveSuccessHelper(reservation?: Reservation) {
    if (reservation)
      Object.assign(this.reservationEdit, reservation);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    this.alertService.showMessage("Success", `Reservation \"${this.reservationEdit.shortDescription}\" was saved successfully`, MessageSeverity.success);

    this.reservationEdit = new Reservation();
    this.resetForm();


    //if (!this.isNewReservation && this.accountService.currentUser.reservations.some(r => r == this.editingReservationName))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close({ isCancel: false, reservation: reservation });
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
    var isCancel = this.reservationEdit.id;
    this.reservationEdit = new Reservation();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close({ isCancel: !isCancel });
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

  onCheck() {
    this.reservationEdit.isAttendee = !this.reservationEdit.isAttendee;
  }

  newReservation(location: Location) {
    this.isEditMode = true;
    this.isNewReservation = true;
    this.showValidationErrors = true;

    this.editingReservationName = null;
    this.selectedValues = {};
    this.reservationEdit = new Reservation();
    this.reservationEdit.isAttendee = true;
    this.reservationEdit.locationName = location.name;
    this.reservationEdit.locationId = location.id;
    this.reservationEdit.facilities = location.facilities;
    console.log(this.reservationEdit);
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

      if (!this.reservationEdit.repeatType || typeof this.reservationEdit.repeatType == 'undefined') {
        this.reservationEdit.repeatType = "None";
      }

      this.startDate = (new DateOnlyPipe('en-SG')).transform(this.reservationEdit.startDate ? this.reservationEdit.startDate : this.reservationEdit.startDateTime);
      this.selectedStartTime = Utilities.printTimeOnly(this.reservationEdit.startDate ? this.reservationEdit.startDate : this.reservationEdit.startDateTime, true);
      this.selectedEndTime = Utilities.printTimeOnly(this.reservationEdit.endDate ? this.reservationEdit.endDate : this.reservationEdit.endDateTime, true);

      this.reservationEdit.locationName = location.name;
      this.reservationEdit.locationId = location.id;
      this.reservationEdit.facilities = location.facilities;
      this.reservationEdit.timeIntervals = timeIntervals;
      this.reservationEdit.isAttendee = (this.reservationEdit.id == null || typeof this.reservationEdit.id == 'undefined') ? true : this.reservationEdit.isAttendee;

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

  selectRepeatType() {
    if (this.reservationEdit.isRecurring && this.reservationEdit.repeatType != 'None') {
      this.showDialog("Edit recurring event");
    } else {
      this.reservationEdit.repeatEndDateTime = null;
      this.reservationEdit.recurApplyChangesType = "ALL_EVENTS";
    }
  }

  showDialog(msg: string, okCallback?: (val?: any) => any, cancelCallback?: (val?: any) => any): void {
    if (!okCallback || !cancelCallback) {
      this.alertService.showDialog(msg, DialogType.confirm, (val) => this.configure(true, val), () => this.configure(false), "This and following events", "All events");
    } else {
      this.alertService.showDialog(msg, DialogType.confirm, okCallback, cancelCallback, "This and following events", "All events");
    }
  }

  configure(response: boolean, value?: string) {
    if (response) {
      this.reservationEdit.recurApplyChangesType = "OTHER_EVENTS";
    }
    else {
      this.reservationEdit.recurApplyChangesType = "ALL_EVENTS";
    }
  }

  get canManageReservations() {
    return this.accountService.userHasPermission(Permission.manageReservationsPermission)
  }
}
