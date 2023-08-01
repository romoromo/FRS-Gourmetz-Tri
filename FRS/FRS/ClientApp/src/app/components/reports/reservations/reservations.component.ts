import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { ReservationService } from '../../../services/reservation.service';
import { Utilities } from '../../../services/utilities';
import { Reservation, CalendarFilter } from '../../../models/reservation.model';
import { Permission } from '../../../models/permission.model';
import { Facility } from '../../../models/facility.model';

import { Location } from '../../../models/location.model';
import { fadeInOut } from '../../../services/animations';

import { DateTimeOnlyPipe } from '../../../pipes/datetime.pipe';
import { DateOnlyPipe } from '../../../pipes/datetime.pipe';
import { CalendarEvent } from 'calendar-utils';
import { ReservationTime } from 'src/app/models/reservationtime.model';
import { MatDialog, MatDatepickerInputEvent } from '@angular/material';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { take } from 'rxjs/operators';
import { Subscription } from 'rxjs';
@Component({
  selector: 'reports-reservation-list',
  templateUrl: './reservations.component.html',
  styleUrls: ['./reservations.component.css'],
  animations: [fadeInOut]
})
export class ReportsReservationListComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: Reservation[] = [];
  rowsCache: Reservation[] = [];
  allFacilities: Facility[] = [];
  allLocations: Location[] = [];
  editedReservation: Reservation;
  sourceReservation: Reservation;
  editingReservationName: { shortDescription: string };
  loadingIndicator: boolean;
  //@Input() viewDate: Date;
  filteredReservations: Reservation[];
  filter: CalendarFilter;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private reservationService: ReservationService, private configurationService: ConfigurationService, public dialog: MatDialog) {
  }


  ngOnInit() {
    this.filter = new CalendarFilter();
    if (!this.filter.start) {
      this.filter.start = new Date();
    }

    if (!this.filter.end) {
      this.filter.end = new Date();
    }

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

  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  loadData(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.filter.start = new Date(event.value);
    }

    if (type == 'end') {
      this.filter.end = new Date(event.value);
    }

    this.loadingIndicator = true;
    this.filter.start = new Date(this.filter.start.setHours(0, 0, 0, 0));
    this.filter.end = new Date(this.filter.end.setHours(23, 59, 59, 0));
    this.filter.institutionId = this.accountService.currentUser.institutionId;

    this.subscription.add(this.reservationService.getReservationsWithFilter(this.filter)
      .subscribe(results => {

        let reservations = results;

        reservations.forEach((reservation, index, reservations) => {
          (<any>reservation).index = index + 1;
        });


        if (reservations) {
          this.filteredReservations = reservations;
        }
        this.filteredReservations.forEach((reservation, index, reservations) => {
          (<any>reservation).index = index + 1;
        });


        this.rowsCache = [...this.filteredReservations];
        this.rows = this.filteredReservations;
        this.loadingIndicator = false;
        
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve reservations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
      }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.shortDescription, r.longDescription, r.repeatType, r.status, r.locationName, r.bookedBy));
  }

  get canManageReservations() {
    return this.accountService.userHasPermission(Permission.viewReportsPermission)
  }

}
