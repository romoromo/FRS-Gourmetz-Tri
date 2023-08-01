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
import { BookingEditorComponent } from './booking-editor.component';

import { MatPaginator, MatSort } from '@angular/material';
import { merge, Observable, of as observableOf, fromEvent } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { animate, state, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', display: 'none' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class BookingComponent
  implements OnInit, AfterViewInit 
{
  startDate = new Date();
  endDate = new Date();
  viewDate = new Date();
  reservations: Reservation[];
  facilityList: Facility[];
  locationList: Location[];
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
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  constructor(private translationService: AppTranslationService, private http: HttpClient, private cdr: ChangeDetectorRef,private alertService: AlertService, private accountService: AccountService,
    private reservationService: ReservationService, private locationService: LocationService, protected configurations: ConfigurationService, public dialog: MatDialog) {
  }

  @ViewChild(MatCalendar)
  _datePicker: MatCalendar<Date>


  @ViewChild(MatSelect)
  selectStart: MatSelect

  ngAfterViewInit() {

    
    // If the user changes the sort order, reset back to the first page.
    //this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    
  }

  openDialog(reservation: Reservation, location: Location, timeIntervals: ReservationTime[]): void {
    const dialogRef = this.dialog.open(BookingEditorComponent, {
      data: { header: "New Booking for " + location.name, reservation: reservation, location: location, timeIntervals: timeIntervals, allTimeIntervals: this.allTimes },
      width: '800px'
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

    this.loadData();
    this.loadAllTimeIntervals() ;
    this._datePicker.selectedChange.subscribe(x => {
      if (typeof x != typeof (undefined)) {
        this.viewDate = x;
        this.filter.viewDate = this.viewDate;
        this.filter.start = this.filter.end = this.viewDate;
      }
    });

    this.fcStartTimes.valueChanges.subscribe(x => {
      this.changeEndTime(x);
    });
  }

  loadAllTimeIntervals() {

    this.reservationService.getTimeIntervals(new TimeIntervalFilter())
      .subscribe(results => {
        this.allStartTimes = results;
        this.allTimes = results;
      });
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

    this.reservationService.getTimeIntervals(tFilter)
      .subscribe(results => {
        this.allAvailableTimeIntervals = results;
        //this.allStartTimes = results;
      });

  }
  loadData() {

    this.locationService.getLocationsAndFacilities()
      .subscribe(results => {

        let locations = results[0];
        let facilities = results[1];

        locations.forEach((location, index, locations) => {
          (<any>location).index = index + 1;
        });


        this.locationList = locations;
        this.locationColList = locations;
        this.facilityList = facilities;

        this._datePicker.selectedChange.emit();
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve locations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
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
  }

  clear() {
    this.filter = new CalendarFilter();
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.apply();
  }

  book(row) {
    var reservation = new Reservation();
    reservation.startDate = reservation.endDate = this.viewDate;
    var startTime = this.getReservationTimeBy(this.selectStartTimeId);
    var endTime = this.getReservationTimeBy(this.selectEndTimeId);
    reservation.startTime = startTime;
    reservation.endTime = endTime;

    //var selectedIndexes = this.hightlightStatus.filter(function (p, index, array) {
    //  if (p) {
    //    return index;
    //  }
    //});

    var selectedIndexes:number[] = [];
    this.hightlightStatus.forEach((s, sIndex, si) => {
      if (s) {
        selectedIndexes.push(sIndex);
      }
    });

    var selectedTimes: ReservationTime[] = [];
    //this.allTimes = this.allTimes.sort(function (a, b) {
    //  if (a.hour === b.hour) {
    //    return b.minutes - a.minutes;
    //  }
    //  return a.hour > b.hour ? 1 : -1;
    //});

    this.allAvailableTimeIntervals.forEach((t, tIndex, ts) => {
      selectedIndexes.forEach((s, sIndex, si) => {
        if (s == tIndex) {
          selectedTimes.push(t);
        }
      });
    });

    this.openDialog(reservation, row, selectedTimes);
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
}
