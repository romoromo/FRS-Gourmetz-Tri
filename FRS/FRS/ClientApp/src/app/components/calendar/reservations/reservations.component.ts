import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { ReservationService } from '../../../services/reservation.service';
import { Utilities } from '../../../services/utilities';
import { Reservation, CalendarFilter } from '../../../models/reservation.model';
import { Permission } from '../../../models/permission.model';
import { ReservationListEditorComponent } from "./reservation-editor.component";
import { Facility } from '../../../models/facility.model';

import { Location } from '../../../models/location.model';
import { fadeInOut } from '../../../services/animations';

import { DateTimeOnlyPipe } from '../../../pipes/datetime.pipe';
import { DateOnlyPipe } from '../../../pipes/datetime.pipe';
import { CalendarEvent } from 'calendar-utils';
import { ReservationTime } from 'src/app/models/reservationtime.model';
import { MatDialog } from '@angular/material';
import { BookingGridEditorComponent } from '../booking-grid/booking-grid-editor.component';
import { CalendarBookingGridEditorComponent } from '../calendar-booking-grid/calendar-booking-grid-editor.component';
import { ConfigurationService } from 'src/app/services/configuration.service';
@Component({
  selector: 'reservation-list',
  templateUrl: './reservations.component.html',
  styleUrls: ['./reservations.component.css'],
  animations: [fadeInOut]
})
export class ReservationListComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: Reservation[] = [];
  rowsCache: Reservation[] = [];
  allFacilities: Facility[] = [];
  allLocations: Location[] = [];
  editedReservation: Reservation;
  sourceReservation: Reservation;
  editingReservationName: { shortDescription: string };
  loadingIndicator: boolean;
  @Input() viewDate: Date;
  @Input() filteredReservations: Reservation[];
  @Input() calendarFilter: CalendarFilter;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('reservationEditor')
  reservationEditor: ReservationListEditorComponent;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private reservationService: ReservationService, private configurationService: ConfigurationService, public dialog: MatDialog) {
  }


  ngOnInit() {
    this.loadingIndicator = false;
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'shortDescription', name: gT('reservations.management.ShortDescription') },
      { prop: 'longDescription', name: gT('reservations.management.LongDescription') },
      { prop: 'startDateTime', name: 'Date', pipe: new DateOnlyPipe('en-SG') },
      //{ prop: 'endDateTime', name: gT('reservations.management.EndDateTime'), pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'locationName', name: gT('reservations.management.LocationName') },
      //{ prop: 'isAllDayYesNo', name: gT('reservations.management.IsAllDay') },
      { prop: 'status', name: gT('reservations.management.Status') },
      { prop: 'bookedBy', name: gT('reservations.management.BookedBy') },
      { prop: 'repeatType', name: gT('reservations.management.RepeatType') },
      { prop: 'repeatEndDateTime', name: gT('reservations.management.RepeatEndDateTime'), pipe: new DateOnlyPipe('en-SG') },
      { name: 'Attendance', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: true, sortable: false, draggable: false }

    ];

    //this.loadData();
  }





  ngAfterViewInit() {


    this.reservationEditor.changesSavedCallback = () => {
      this.addNewReservationToList();
      this.editorModal.hide();
    };

    this.reservationEditor.changesCancelledCallback = () => {
      this.editedReservation = null;
      this.sourceReservation = null;
      this.editorModal.hide();
    };
  }


  addNewReservationToList() {
    if (this.sourceReservation) {
      Object.assign(this.sourceReservation, this.editedReservation);

      let sourceIndex = this.rowsCache.indexOf(this.sourceReservation, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceReservation, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedReservation = null;
      this.sourceReservation = null;
    }
    else {
      let reservation = new Reservation();
      Object.assign(reservation, this.editedReservation);
      this.editedReservation = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>reservation).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, reservation);
      this.rows.splice(0, 0, reservation);
      this.rows = [...this.rows];
    }
  }




  loadData(reservations?: Reservation[]) {
    this.loadingIndicator = true;

    if (reservations) {
      this.filteredReservations = reservations;
    }
    this.filteredReservations.forEach((reservation, index, reservations) => {
      (<any>reservation).index = index + 1;
    });


    this.rowsCache = [...this.filteredReservations];
    this.rows = this.filteredReservations;
    this.loadingIndicator = false;


    //this.reservationService.getReservationsLocationsFacilities()
    //  .subscribe(results => {
    //    this.alertService.stopLoadingMessage();
    //    this.loadingIndicator = false;

    //    //let reservations = results[0];
    //    let locations = results[2];

    //    this.filteredReservations.forEach((reservation, index, reservations) => {
    //      (<any>reservation).index = index + 1;
    //    });


    //    this.rowsCache = [...this.filteredReservations];
    //    this.rows = this.filteredReservations;

    //    this.allLocations = locations;
    //  },
    //    error => {
    //      this.alertService.stopLoadingMessage();
    //      this.loadingIndicator = false;

    //      this.alertService.showStickyMessage("Load Error", `Unable to retrieve reservations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
    //        MessageSeverity.error);
    //    });
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.shortDescription, r.longDescription));
  }


  onEditorModalHidden() {
    this.editingReservationName = null;
    this.reservationEditor.resetForm(true);
  }


  newReservation() {
    this.editingReservationName = null;
    this.sourceReservation = null;
    this.editedReservation = this.reservationEditor.newReservation(this.allLocations);
    this.editorModal.show();
  }


  editReservation(row: Reservation) {
    this.editingReservationName = { shortDescription: row.shortDescription };
    this.sourceReservation = row;
    this.editedReservation = this.reservationEditor.editReservation(row, this.allLocations);
    this.editorModal.show();
  }

  deleteReservation(row: Reservation) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.shortDescription + '\" reservation?', DialogType.confirm, () => this.deleteReservationHelper(row));
  }


  deleteReservationHelper(row: Reservation) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.reservationService.deleteReservation(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the reservation.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  book(reservation: Reservation) {
    //var reservation = new Reservation();
    //reservation.startDate = reservation.endDate = this.viewDate;
    //var startTime = this.getReservationTimeBy(this.selectStartTimeId);
    //var endTime = this.getReservationTimeBy(this.selectEndTimeId);
    //reservation.startTime = startTime;
    //reservation.endTime = endTime;
    //reservation.institutionId = this.authService.currentUser.institutionId;
    //var selectedTimes: ReservationTime[] = [];
    //this.selectedTimings.sort((a, b) => (a.timeInterval.hour * 60 + a.timeInterval.minutes > b.timeInterval.hour * 60 + b.timeInterval.minutes) ? 1 :
    //  ((b.timeInterval.hour * 60 + b.timeInterval.minutes > a.timeInterval.hour * 60 + a.timeInterval.minutes ? -1 : 0)));

    //this.selectedTimings.forEach((s, sIndex, si) => {
    //  selectedTimes.push(s.timeInterval);
    //});

    //if (this.selectRow != null) {

    //}
    var location = new Location();
    location.id = reservation.locationId;
    this.openDialog(reservation, location, reservation.timeIntervals);
  }

  header: string;
  openDialog(reservation: Reservation, location: Location, timeIntervals: ReservationTime[]): void {
    const dialogRef = this.dialog.open(BookingGridEditorComponent, {
      data: { header: this.header, reservation: reservation, location: location, timeIntervals: timeIntervals, allTimeIntervals: null },
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      //this.selectedTimings = [];
      //this.selectRow = null;
      //this.loadAllTimeIntervals();
    });
  }


  downloadAttendanceList(reservation) {

  }

  get canManageReservations() {
    return this.accountService.userHasPermission(Permission.manageReservationsPermission)
  }

}
