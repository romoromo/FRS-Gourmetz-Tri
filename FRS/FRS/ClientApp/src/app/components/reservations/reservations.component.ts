import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from "../../services/app-translation.service";
import { AccountService } from '../../services/account.service';
import { ReservationService } from '../../services/reservation.service';
import { Utilities } from '../../services/utilities';
import { Reservation, CalendarFilter } from '../../models/reservation.model';
import { Permission } from '../../models/permission.model';
import { ReservationEditorComponent } from "./reservation-editor.component";
import { ReservationPictureComponent } from "./reservation-picture.component";
import { Facility } from '../../models/facility.model';

import { Location } from '../../models/location.model';
import { fadeInOut } from '../../services/animations';

import { DateTimeOnlyPipe } from '../../pipes/datetime.pipe';
import { DateOnlyPipe } from '../../pipes/datetime.pipe';
import { TimeOnlyPipe } from '../../pipes/datetime.pipe';

import { CalendarEvent, CalendarEventTitleFormatter, CalendarView, CalendarEventTimesChangedEvent, CalendarEventAction } from 'angular-calendar';
import {
  isSameMonth,
  isSameDay,
  startOfMonth,
  endOfMonth,
  startOfWeek,
  endOfWeek,
  startOfDay,
  endOfDay,
  format,
  addHours, addDays, addMinutes
} from 'date-fns';
@Component({
  selector: 'reservations',
  templateUrl: './reservations.component.html',
  styleUrls: ['./reservations.component.css'],
  animations: [fadeInOut]
})
export class ReservationsComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: Reservation[] = [];
  rowsCache: Reservation[] = [];
  allFacilities: Facility[] = [];
  allLocations: Location[] = [];
  editedReservation: Reservation;
  sourceReservation: Reservation;
  editingReservationName: { shortDescription: string };
  loadingIndicator: boolean;
  filter: CalendarFilter = new CalendarFilter();

  @Input()
  viewDate: Date;


  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('pictureModal')
  pictureModal: ModalDirective;

  @ViewChild('reservationEditor')
  reservationEditor: ReservationEditorComponent;

  @ViewChild('reservationPictureEditor')
  reservationPictureEditor: ReservationPictureComponent;


  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private reservationService: ReservationService) {
  }


  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'shortDescription', name: gT('reservations.management.ShortDescription') },
      { prop: 'longDescription', name: gT('reservations.management.LongDescription')},
      { prop: 'startDateTime', name: 'Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'startDateTime', name: 'Start Time', pipe: new TimeOnlyPipe('en-SG') },
      { prop: 'endDateTime', name: 'End Time', pipe: new TimeOnlyPipe('en-SG') },
      { prop: 'locationName', name: gT('reservations.management.LocationName')},
      //{ prop: 'isAllDayYesNo', name: gT('reservations.management.IsAllDay') },
      { prop: 'status', name: gT('reservations.management.Status') },
      { prop: 'repeatType', name: gT('reservations.management.RepeatType') },
      //{ prop: 'repeatEndDateTime', name: gT('reservations.management.RepeatEndDateTime'), pipe: new DateOnlyPipe('en-SG')},
      { name: '', width: 100, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: true, sortable: false, draggable: false  }
      
    ];

    this.loadData();
  }





  ngAfterViewInit() {

    this.reservationEditor.changesSavedCallback = () => {
      this.addNewReservationToList();
      this.editorModal.hide();
    };

    this.reservationPictureEditor.changesSavedCallback = () => {
      this.addNewReservationToList();
      this.pictureModal.hide();
    };

    this.reservationEditor.changesCancelledCallback = () => {
      this.editedReservation = null;
      this.sourceReservation = null;
      this.editorModal.hide();
    };

    this.reservationPictureEditor.changesCancelledCallback = () => {
      this.editedReservation = null;
      this.sourceReservation = null;
      this.pictureModal.hide();
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




  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    if (this.filter == null) {
      this.filter = new CalendarFilter();
    }

    this.filter.institutionId = this.accountService.currentUser.institutionId;

    this.reservationService.getReservationsLocationsFacilities(null, null, this.filter)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let reservations = results[0];
        let locations = results[2];

        reservations.forEach((reservation, index, reservations) => {
          (<any>reservation).index = index + 1;
        });


        this.rowsCache = [...reservations];
        this.rows = reservations;

        this.allLocations = locations;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve reservations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.shortDescription, r.longDescription));
  }


  onEditorModalHidden() {
    this.editingReservationName = null;
    this.reservationEditor.resetForm(true);
  }

  onPictureEditorModalHidden() {
    this.editingReservationName = null;
    this.reservationPictureEditor.resetForm(true);
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

  reservationPicture(row: Reservation) {
    this.editingReservationName = { shortDescription: row.shortDescription };
    this.sourceReservation = row;
    this.editedReservation = this.reservationPictureEditor.editReservation(row, this.allLocations);
    this.pictureModal.show();
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


  get canManageReservations() {
    return this.accountService.userHasPermission(Permission.manageReservationsPermission)
  }

}
