import { Component, ChangeDetectionStrategy, OnInit, ChangeDetectorRef, ViewEncapsulation, ViewChild, TemplateRef, AfterViewInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CalendarEvent, CalendarEventTitleFormatter, CalendarView, CalendarEventTimesChangedEvent } from 'angular-calendar';
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
import { finalize, takeUntil, timeInterval } from 'rxjs/operators';
import { DayViewHourSegment } from 'calendar-utils';
import MapUtils from 'src/app/helpers/maputils';
import { UserInfoVal, Facility } from 'src/app/models/facility.model';
import { Location } from 'src/app/models/location.model';
import { Reservation, CalendarFilter } from 'src/app/models/reservation.model';
import { Utilities } from 'src/app/services/utilities';
import { MessageSeverity, AlertService } from 'src/app/services/alert.service';
import { ReservationService } from '../../../services/reservation.service';
import { AccountService } from 'src/app/services/account.service';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { MatButtonModule, MatCalendar, MatDatepicker, MatSelect, MatDialog } from '@angular/material';
import { FormControl, Validators } from '@angular/forms';
import { forEach } from '@angular/router/src/utils/collection';
import { fadeInOut } from 'src/app/services/animations';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { LocationService } from 'src/app/services/location.service';
import { ReservationTime, TimeIntervalFilter } from 'src/app/models/reservationtime.model';
import { BookingGridEditorComponent } from './booking-grid-editor.component';

import { MatPaginator, MatSort } from '@angular/material';
import { merge, Observable, of as observableOf, fromEvent } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { BookingGridRow, LocationTimeInterval } from 'src/app/models/bookinggridrow.model';
import { GroupByPipe } from 'src/app/pipes/group-by.pipe';
import { AuthService } from 'src/app/services/auth.service';
import { LocType } from 'src/app/models/enums';

@Component({
  selector: 'booking-grid',
  templateUrl: './booking-grid.component.html',
  styleUrls: ['./booking-grid.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', display: 'none' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
    trigger('slideInOut', [
      transition(':enter', [
        style({ transform: 'translateX(-100%)' }),
        animate('200ms ease-in', style({ transform: 'translateX(0%)' }))
      ]),
      transition(':leave', [
        animate('200ms ease-in', style({ transform: 'translateX(-100%)' }))
      ])
    ])
  ]
})
export class BookingGridComponent
  implements OnInit, AfterViewInit {
  startDate = new Date();
  endDate = new Date();
  viewDate = new Date();
  reservations: Reservation[];
  facilityList: Facility[];
  locationList: Location[];
  buildingList: Location[];
  roomList: Location[];
  locationColList: Location[];
  changeDetector: ChangeDetectorRef;
  events$: Observable<Array<CalendarEvent<{ reservation: Reservation }>>>;
  dragToCreateActive = false;
  activeDayIsOpen: boolean = false;
  fcLocations = new FormControl();
  fcFacilities = new FormControl();
  fcStartTimes = new FormControl();
  fcEndTimes = new FormControl();
  filter: CalendarFilter = new CalendarFilter();
  columns: any[] = [];
  rows: Location[] = [];
  rowsCache: Location[] = [];
  private allTimes: ReservationTime[] = [];
  private allAvailableTimeIntervals: ReservationTime[] = [];
  private allStartTimes: ReservationTime[] = [];
  private allEndTimes: ReservationTime[] = [];
  selectStartTimeId: number;
  selectEndTimeId: number;
  header: string;

  displayedColumns: string[];

  bookingGridRows: BookingGridRow[] = [];
  sideBarOpened: boolean = true;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  constructor(private translationService: AppTranslationService, private http: HttpClient, private cdr: ChangeDetectorRef, private alertService: AlertService, private accountService: AccountService,
    private reservationService: ReservationService, private locationService: LocationService, protected configurations: ConfigurationService, public dialog: MatDialog,
    private authService: AuthService  ) {
  }

  @ViewChild('calendar')
  _datePicker: MatCalendar<Date>


  @ViewChild(MatSelect)
  selectStart: MatSelect

  ngAfterViewInit() {

    if (this._datePicker) {
      this._datePicker.selectedChange.subscribe(x => {
        if (typeof x != typeof (undefined)) {
          this.viewDate = x;
          this.filter.viewDate = this.viewDate;
          this.filter.start = this.filter.end = this.viewDate;

        }
      });
    }
    // If the user changes the sort order, reset back to the first page.
    //this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

  }

  openDialog(reservation: Reservation, location: Location, timeIntervals: ReservationTime[]): void {
    const dialogRef = this.dialog.open(BookingGridEditorComponent, {
      data: { header: "New Booking for " + location.name, reservation: reservation, location: location, timeIntervals: timeIntervals, allTimeIntervals: this.allTimes },
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.selectedTimings = [];
      this.selectRow = null;
      this.loadAllTimeIntervals();
    });
  }

  ngOnInit(): void {
    this.filter.locationIds = [];
    this.filter.facilityIds = [];

    //this.allTimes = this.getAllTimes();
    //this.allStartTimes = this.getAllTimes();
    let gT = (key: string) => this.translationService.getTranslation(key);
    this.displayedColumns = ['name', 'description', 'capacity', 'facilityNames', 'select'];
    //this.columns = [
    //  { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
    //  { prop: 'name', name: gT('locations.management.Name'), width: 200 },
    //  { prop: 'description', name: gT('locations.management.Description'), width: 150 },
    //  { prop: 'capacity', name: gT('locations.management.Capacity'), width: 50 },
    //  { prop: 'facilityNames', name: gT('locations.management.Facilities'), width: 150 },
    //  { name: '', width: 130, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    //];

    this.loadFilterObjects();
    this.loadAllTimeIntervals();


    this.fcStartTimes.valueChanges.subscribe(x => {
      this.changeEndTime(x);
    });
    this.apply();
  }


  buildings: Location[] = [];

  loadAllTimeIntervals() {
    this.reservationService.getTimeIntervals(new TimeIntervalFilter())
      .subscribe(results => {
        this.allStartTimes = results;
        this.allTimes = results;
      });

    this.reservationService.getBookingGridRows(this.filter).subscribe(results => {
      this.bookingGridRows = results;

      var buildings: Location[] = [];
      results.forEach(function (p, index, array) {
        if (p.location.parentLocation != null &&
          p.location.parentLocation.locationTypeName == LocType.Building && 
          buildings.filter(e => e.id === p.location.parentLocation.id).length == 0) {
            buildings.push(p.location.parentLocation);
        }
      });

      this.buildings = buildings;
    });


  }

  filterByParentLocationId(id) {
    var groupedGridRows: any[] = [];
    this.bookingGridRows.forEach((r, index, rooms) => {
      if (r.location.parentLocationId == id) {
        var obj = {
          location: r.location,
          locationTimeIntervals: r.locationTimeIntervals
        }

        groupedGridRows.push(obj);
      }
    });

    return groupedGridRows;
  }

  loadTimeIntervals(location) {
    // var date = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), this.viewDate.getDate(), 0, 0, 0);
    var startTime = this.getReservationTimeBy(this.selectStartTimeId);
    var endTime = this.getReservationTimeBy(this.selectEndTimeId);


    if (startTime) {
      this.filter.start = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), this.viewDate.getDate(), startTime.hour, startTime.minutes, 0);
    } else {
      this.filter.start = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), this.viewDate.getDate(), 0, 0, 0);
    }

    if (endTime) {
      this.filter.end = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), this.viewDate.getDate(), endTime.hour, endTime.minutes, 0);
    } else {
      this.filter.end = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), this.viewDate.getDate(), 23, 59, 59);
    }


    var tFilter = new TimeIntervalFilter();
    tFilter.startDate = this.filter.start;
    tFilter.endDate = this.filter.end;
    tFilter.locationId = location != null ? location.id : null;
    tFilter.startTimeInterval = this.getReservationTimeBy(this.selectStartTimeId);
    tFilter.endTimeInterval = this.getReservationTimeBy(this.selectEndTimeId);

    this.reservationService.getBookingGridRows(this.filter).subscribe(results => {
      this.bookingGridRows = results;
    });

    //this.reservationService.getTimeIntervals(tFilter)
    //  .subscribe(results => {
    //    this.allAvailableTimeIntervals = results;
    //    //this.allStartTimes = results;
    //  });

  }


  loadFilterObjects() {
    this.locationService.getLocationsAndFacilities(this.accountService.currentUser.institutionId)
      .subscribe(results => {

        let locations = results[0];
        let facilities = results[1];

        locations.forEach((location, index, locations) => {
          (<any>location).index = index + 1;
        });


        this.locationList = locations;
        this.locationColList = locations;
        this.facilityList = facilities;

        this.buildingList = this.locationList.filter(f => f.locationTypeName == LocType.Building);
        //var x = (new GroupByPipe).transform(rooms, 'parentLocationId');
        //this._datePicker.selectedChange.emit();
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve locations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  getRoomsByBuildingId(id) {
    return this.locationList.filter(f => f.parentLocationId == id);
  }

  fetchLocations() {
    this.locationService.getAvailableLocations(this.filter)
      .subscribe(results => {

        let locations = results;

        locations.forEach((location, index, locations) => {
          (<any>location).index = index + 1;
        });


        this.locationColList = locations;
        this.rowsCache = [...locations];
        this.rows = locations;
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve locations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });

  }
  apply() {
    var startTime = this.getReservationTimeBy(this.selectStartTimeId);
    var endTime = this.getReservationTimeBy(this.selectEndTimeId);
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.filter.start = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), this.viewDate.getDate(), startTime != null ? startTime.hour : 0, startTime != null ? startTime.minutes : 0, 0);
    this.filter.end = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), this.viewDate.getDate(), endTime != null ? endTime.hour : 23, endTime != null ? endTime.minutes : 59, endTime != null ? 0 : 59);
    this.fetchLocations();
    this.loadAllTimeIntervals();
  }

  clear() {
    this.filter = new CalendarFilter();
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.apply();
  }

  compare(a, b) {
    if (a.last_nom < b.last_nom)
      return -1;
    if (a.last_nom > b.last_nom)
      return 1;
    return 0;
  }

  colReservation: Reservation;
  getColspan(row: BookingGridRow, col: LocationTimeInterval, index: number) {
    var count = 0;
    if (col != undefined && col.selected && !col.skip) {
      while (row.locationTimeIntervals[index] != null && row.locationTimeIntervals[index].reservation != null &&
        row.locationTimeIntervals[index].reservation.id == col.reservation.id && col.selected) {
        count++;
        if (count > 1 && row.locationTimeIntervals[index] != undefined) {
          row.locationTimeIntervals[index].skip = true;
        }
        index++;
      }
    }

    return count;
  }

  selectedTimings: Array<LocationTimeInterval> = [];
  selectRow?: BookingGridRow = null;
  selectTiming(row, col) {

    var isOk = false;
    if (this.selectRow == null || this.selectRow.location.id == row.location.id) {
      var length = this.selectedTimings.length;
      if (length > 0) {
        this.selectedTimings.sort((a, b) => (a.timeInterval.hour * 60 + a.timeInterval.minutes > b.timeInterval.hour * 60 + b.timeInterval.minutes) ? 1 :
          ((b.timeInterval.hour * 60 + b.timeInterval.minutes > a.timeInterval.hour * 60 + a.timeInterval.minutes ? -1 : 0)));

        //this.selectedTimings.sort(function (a, b) {
        //  return a.timeInterval.hour * 60 + a.timeInterval.minutes - b.timeInterval.hour * 60 + b.timeInterval.minutes});

        var existCol = this.selectedTimings[0];
        if (Math.abs((existCol.timeInterval.hour * 60 + existCol.timeInterval.minutes) -
          (col.timeInterval.hour * 60 + col.timeInterval.minutes)) == 30 ||
          Math.abs((existCol.timeInterval.hour * 60 + existCol.timeInterval.minutes) -
            (col.timeInterval.hour * 60 + col.timeInterval.minutes)) == 0) {
          isOk = true;
        } else {
          existCol = this.selectedTimings[length - 1];
          if (Math.abs((existCol.timeInterval.hour * 60 + existCol.timeInterval.minutes) -
            (col.timeInterval.hour * 60 + col.timeInterval.minutes)) == 30 ||
            Math.abs((existCol.timeInterval.hour * 60 + existCol.timeInterval.minutes) -
              (col.timeInterval.hour * 60 + col.timeInterval.minutes)) == 0) {
            isOk = true;
          }
        }
      }
      else {
        isOk = true;
      }

      if (isOk) {
        var index = this.selectedTimings.indexOf(col);
        if (index > -1) {
          if (index == 0 || index == length - 1) {
            col.isNewSelected = !col.isNewSelected;
            this.selectedTimings.splice(index, 1);
          }
        } else {
          col.isNewSelected = !col.isNewSelected;
          this.selectedTimings.push(col);
        }

        this.selectRow = row;
      }
    }

    if (this.selectedTimings != null && this.selectedTimings.length == 0) this.selectRow = null;
  }

  book() {
    var reservation = new Reservation();
    reservation.startDate = reservation.endDate = this.viewDate;
    var startTime = this.getReservationTimeBy(this.selectStartTimeId);
    var endTime = this.getReservationTimeBy(this.selectEndTimeId);
    reservation.startTime = startTime;
    reservation.endTime = endTime;
    reservation.institutionId = this.authService.currentUser.institutionId;
    var selectedTimes: ReservationTime[] = [];
    this.selectedTimings.sort((a, b) => (a.timeInterval.hour * 60 + a.timeInterval.minutes > b.timeInterval.hour * 60 + b.timeInterval.minutes) ? 1 :
      ((b.timeInterval.hour * 60 + b.timeInterval.minutes > a.timeInterval.hour * 60 + a.timeInterval.minutes ? -1 : 0)));

    this.selectedTimings.forEach((s, sIndex, si) => {
      selectedTimes.push(s.timeInterval);
    });

    if (this.selectRow != null) {
      this.openDialog(reservation, this.selectRow.location, selectedTimes);
    }
  }

  public changeEndTime(id) {
    if (id != null && id != null && id.toString() != "") {
      var allTimes = this.allTimes;//this.getAllTimes();
      var resStartTime = this.getReservationTimeBy(id);
      var $this = this;
      this.allEndTimes = allTimes.filter(function (p, index, array) {
        if (p.hour > resStartTime.hour || (p.hour == resStartTime.hour && p.minutes > resStartTime.minutes)) {
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
    var allTimes = this.allTimes;//this.getAllTimes();
    var time: ReservationTime;
    if (id != null) {
      time = allTimes.filter(f => f.id == id)[0];
    } else if (hour != null && minute != null) {
      var t = allTimes.filter(f => f.hour == hour && f.minutes == minute);
      time = t != null && t.length > 0 ? t[0] : allTimes[0];
    } else {
      time = null;
    }
    return time;
  }

  hightlightStatus: Array<boolean> = [];
  selectTime(ev) {


  }

  getTooltip(reservation) {
    if (reservation != null) {
      var s = 'Title: ' + reservation.shortDescription;
      s += ' Booked By: ' + reservation.bookedBy;
      return s;
    }
  }
}
