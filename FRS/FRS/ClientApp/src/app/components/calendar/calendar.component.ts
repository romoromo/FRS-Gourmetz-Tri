import { LOCALE_ID, Inject, Component, ChangeDetectionStrategy, OnInit, ChangeDetectorRef, ViewEncapsulation, ViewChild, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
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
import { Observable, fromEvent } from 'rxjs';
import { colors } from './colors';
import { finalize, takeUntil } from 'rxjs/operators';
import { DayViewHourSegment } from 'calendar-utils';
import MapUtils from 'src/app/helpers/maputils';
import { UserInfoVal, Facility } from 'src/app/models/facility.model';
import { Location } from 'src/app/models/location.model';
import { Reservation, CalendarFilter } from 'src/app/models/reservation.model';
import { Utilities } from 'src/app/services/utilities';
import { MessageSeverity, AlertService } from 'src/app/services/alert.service';
import { ReservationService } from '../../services/reservation.service';
import { AccountService } from 'src/app/services/account.service';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { MatButtonModule, MatCalendar, MatDatepicker } from '@angular/material';
import { FormControl, Validators } from '@angular/forms';
import { forEach } from '@angular/router/src/utils/collection';
import { DatePipe } from '@angular/common';
import { trigger, transition, animate, style } from '@angular/animations';
import { LocType } from 'src/app/models/enums';
import { CalendarBookingGridComponent } from './calendar-booking-grid/calendar-booking-grid.component';
import { ReservationListComponent } from './reservations/reservations.component';
import { LocationService } from 'src/app/services/location.service';
import { Institution } from 'src/app/models/institution.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { AuthService } from 'src/app/services/auth.service';

function getTimezoneOffsetString(date: Date): string {
  const timezoneOffset = date.getTimezoneOffset();
  const hoursOffset = String(
    Math.floor(Math.abs(timezoneOffset / 60))
  ).padStart(2, '0');
  const minutesOffset = String(Math.abs(timezoneOffset % 60)).padEnd(2, '0');
  const direction = timezoneOffset > 0 ? '-' : '+';
  return `T00:00:00${direction}${hoursOffset}${minutesOffset}`;
}

function floorToNearest(amount: number, precision: number) {
  return Math.floor(amount / precision) * precision;
}

function ceilToNearest(amount: number, precision: number) {
  return Math.ceil(amount / precision) * precision;
}

export class CustomEventTitleFormatter extends CalendarEventTitleFormatter {
  constructor(@Inject(LOCALE_ID) private locale: string) {
    super();
  }

  weekTooltip(event: CalendarEvent, title: string) {
    if (!event.meta.tmpEvent) {
      return super.weekTooltip(event, title);
    }
  }

  dayTooltip(event: CalendarEvent, title: string) {
    if (!event.meta.tmpEvent) {
      var tooltip = '';
      if (event.meta.reservation != null) {
        tooltip = `<div><b>Subject: </b> <br>${event.meta.reservation.shortDescription}</div>`;
        tooltip += `<br><div><b>Organised By: </b> <br>${event.meta.reservation.createdByName}</div>`;
      } else {
        tooltip = title;
      }
      return super.dayTooltip(event, "ambot");
    }
  }

  //day(event: CalendarEvent, title: string) {
  //  var s = `<div><b>Subject: </b> <br>${event.title}</div>`;
  //  if (event.meta.reservation != null || event.meta.reservation != undefined) {
  //    s += `<br><div><b>Location: </b> <br>${event.meta.reservation.locationName}</div>`;
  //    var facilityNames = '';
  //    for (let f of event.meta.reservation.facilities) {
  //      facilityNames += f.name + ", ";
  //    }
  //    if (facilityNames) {
  //      var count = facilityNames.length - 2;
  //      facilityNames = facilityNames.slice(0, count);
  //      s += `<div><b>Facilities: </b> <br>${facilityNames}</div>`;
  //    }
  //  }
  //  return s;
  //}

  month(event: CalendarEvent): string {
    var s = `<b>${new DatePipe(this.locale).transform(
      event.start,
      'h:m a',
      this.locale
    )}</b> ${event.title}`;
    if (event.meta.reservation != null || event.meta.reservation != undefined) {
      s += `<br><div class="row"><div class="col-sm-4"><b>Location: </b> <br>${event.meta.reservation.locationName}</div>`;
      var facilityNames = '';
      for (let f of event.meta.reservation.facilities) {
        facilityNames += f.name + ", ";
      }
      if (facilityNames) {
        var count = facilityNames.length - 2;
        facilityNames = facilityNames.slice(0, count);
        s += `<div class="col-sm-4"><b>Facilities: </b> <br>${facilityNames}</div>`;
      }
      s += '</div> <hr/>';
    }
    return s;
  }
}

@Component({
  selector: 'booking-calendar',
  changeDetection: ChangeDetectionStrategy.Default,
  templateUrl: 'calendar.component.html',
  styleUrls: ['calendar.component.css'],
  animations: [
    trigger('slideInOut', [
      transition(':enter', [
        style({ transform: 'translateX(-100%)' }),
        animate('200ms ease-in', style({ transform: 'translateX(0%)' }))
      ]),
      transition(':leave', [
        animate('200ms ease-in', style({ transform: 'translateX(-100%)' }))
      ])
    ])
  ],
  providers: [
    {
      provide: CalendarEventTitleFormatter,
      useClass: CustomEventTitleFormatter
    }
  ],
  //styles: [
  //  `
  //    .disable-hover {
  //      pointer-events: none;
  //    }
  //  `
  //],
  encapsulation: ViewEncapsulation.None
})
export class CalendarComponent
  implements OnInit, OnChanges
{
  view: string = 'day';
  startDate = new Date();
  endDate = new Date();
  viewDate = new Date();
  reservations: Reservation[];
  facilityList: Facility[];
  locationList: Location[];
  locationColList: Location[];
  calendarLocations: any[];
  changeDetector: ChangeDetectorRef;
  events$: Observable<Array<CalendarEvent<{ reservation: Reservation }>>>;
  dragToCreateActive = false;
  activeDayIsOpen: boolean = false;
  fcLocations = new FormControl();
  fcFacilities = new FormControl();
  fcStartTimes = new FormControl();
  fcEndTimes = new FormControl();
  //filter: CalendarFilter = new CalendarFilter();
  events: CalendarEvent[] = [];
  sideBarOpened: boolean = true;
  currentInstitution: Institution;
  dayStartHour: number;
  dayEndHour: number;
  eventTimesChanged({
    event,
    newStart,
    newEnd
  }: CalendarEventTimesChangedEvent): void {
    event.start = newStart;
    event.end = newEnd;
    this.events = [...this.events];
  }

  facilityChanged({ event, newReservation }) {
    event.color = newReservation.color;
    event.meta.reservation = newReservation;
    this.events = [...this.events];
  }

  
  constructor(private http: HttpClient, private cdr: ChangeDetectorRef,private alertService: AlertService, private accountService: AccountService,
    private reservationService: ReservationService, protected configurations: ConfigurationService, private locationService: LocationService,
    private institutionService: InstitutionService, private authService: AuthService) {
    //this.filter.locationIds = [];
    //this.filter.facilityIds = [];

    //this.institutionService.getInstitutionById(this.accountService.currentUser.institutionId).subscribe(result => {
    //  this.currentInstitution = result.data;
    //  this.dayStartHour = this.currentInstitution && this.currentInstitution.startTime > 0 ? this.currentInstitution.startTime : 8;
    //  this.dayEndHour = this.currentInstitution && this.currentInstitution.endTime > 0 ? this.currentInstitution.endTime - 1 : 20;
    //  //this.cdr.detectChanges();
    //});
  }
  ngOnChanges(changes: SimpleChanges): void {
    console.log(this.filter);
    this.viewDate = this.filter.start;
    this.apply();
    }

  @Input() filter: CalendarFilter;
  @Output() public bookingComplete = new EventEmitter();

  @ViewChild('bookingGrid')
  bookingGrid: CalendarBookingGridComponent

  @ViewChild('bookingList')
  bookingList: ReservationListComponent

  @ViewChild('calendar')
  _datePicker: MatCalendar<Date>

  calendarEvents: CalendarEvent[];

  ngOnInit(): void {
    
  }

  ngAfterViewInit() {

    if (this._datePicker) {
      this._datePicker.selectedChange.subscribe(x => {
        if (typeof x != typeof (undefined)) {
          this.viewDate = x;
          this.filter.viewDate = this.viewDate;
          this.filter.start = this.filter.end = this.viewDate;
          this.apply();
          //if (this.view == 'day') {
          //  this.bookingGrid.apply(this.filter);
          //  this.bookingGrid.loadCalendarEvents(this.filter);
          //}

        }
      });
    }

    this.apply();
  }

  buildingList: Location[];

  actions: CalendarEventAction[] = [
    //{
    //  label: '<i class="fa fa-fw fa-eye"></i>',
    //  onClick: ({ event }: { event: CalendarEvent }): void => {
    //    this.bookingGrid.handleEvent('view', event);
    //  }
    //},
    {
      label: '<i class="fa fa-fw fa-pencil"></i>',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.bookingGrid.handleEvent('update', event);
      }
    },
    {
      label: '<i class="fa fa-fw fa-times"></i>',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.bookingGrid.handleEvent('delete', event);
      }
    }
  ];

  loadData() {

    //this.reservationService.getCalendarEvents(this.filter).subscribe(events => {
    //  this.calendarEvents = [];
    //  events.forEach((event, index, events) => {
    //    event.start = new Date(event.start);
    //    event.end = new Date(event.end);
    //    event.actions = this.actions;
    //    this.calendarEvents.push(event);
    //  });

    //});
    if (this.filter == null) {
      this.filter = new CalendarFilter();
    }

    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.filter.isBooking = true;
    this.reservationService.getReservationsLocationsFacilities(null, null, this.filter)
      .subscribe(results => {

        let reservations = results[0];
        let facilities = results[1];
        let locations = results[2];

        reservations.forEach((reservation, index, reservations) => {
          (<any>reservation).index = index + 1;
        });


        this.reservations = reservations;
        let parentLocations = new Map();

        locations.forEach((location, index, locations) => {
          (<any>location).index = index + 1;
          if (location.parentLocation && location.parentLocation.locationTypeName == LocType.Building) {
            if (!parentLocations.has(location.parentLocationId)) {
              parentLocations.set(location.parentLocationId, location.parentLocation);
            }
          }
          
        });


        this.locationList = locations;
        //this.locationColList = locations;
        this.facilityList = facilities;

        this.buildingList = Array.from(parentLocations.values()); //this.locationList.filter(f => f.locationTypeName == LocType.Building);
        console.log(this.buildingList);
        //this.locationList = locations;
        //this.locationColList = locations;
        this.facilityList = facilities;

        //this._datePicker.selectedChange.emit();
        //if (this.view != 'day') {
        //  this.fetchEvents();
        //}
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve reservations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }
  
  getRoomsByBuildingId(id) {
    let unsortedLocations = this.locationList.filter(f => f.parentLocationId == id)
    unsortedLocations.sort((a, b) => (a.name < b.name ? -1 : 1));

    return unsortedLocations;
  }


  fetchEvents(): void {
    const getStart: any = {
      month: startOfMonth,
      week: startOfWeek,
      day: startOfDay
    }[this.view];

    const getEnd: any = {
      month: endOfMonth,
      week: endOfWeek,
      day: endOfDay
    }[this.view];

    if (this.view == 'month') {
      this.filter.start = null;
      this.filter.end = null;
    }

    var locColumnList = this.locationColList;
    this.events$ = this.reservationService.getReservationsWithFilter(this.filter)
      .pipe(map((results: Reservation[]) => {
        return results.map((reservation: Reservation) => {
          return {
            title: reservation.shortDescription,//`<div><b>Subject: </b> <br>${reservation.shortDescription}</div><br><div><b>Description: </b> <br>${reservation.longDescription}</div>`,
            start: new Date(reservation.startDateTime),
            end: new Date(reservation.endDateTime),
            allDay: reservation.isAllDay,
              color: colors.yellow,
              meta: {
                reservation: reservation,
                locations: locColumnList
              }
            };
        });
      }));

    this.events$.subscribe(ev => this.events = ev);
  }

  fetchReservations() {
    if (this.view == 'month') {
      this.filter.start = null;
    }

    this.reservationService.getReservationsWithFilter(this.filter)
        .subscribe(results => {

          let reservations = results;

          reservations.forEach((reservation, index, reservations) => {
            (<any>reservation).index = index + 1;
          });


          this.reservations = reservations;
          var locList: Location[] = [];
          for (var l of reservations) {
            var loc = new Location();
            loc.id = l.locationId;
            loc.name = l.locationName;
            if (locList.map(function (e) { return e.id; }).indexOf(loc.id) < 0) {
              locList.push(loc);
            }
          }
          this.locationColList = [];
          this.locationColList = locList;

          if (this.view == 'list') {
            this.bookingList.loadData(this.reservations);
          }
        },
          error => {
            this.alertService.showStickyMessage("Load Error", `Unable to retrieve reservations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
              MessageSeverity.error);
          });
    
  }

  fetchLocations() {
    this.locationService.getCalendarLocations(this.filter)
      .subscribe(results => {
        this.calendarLocations = results;
      });
  }
  loadCalendarEvents(filter?: CalendarFilter) {
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.reservationService.getCalendarEvents(this.filter).subscribe(events => {
      this.calendarEvents = [];
      var currentTime = Utilities.getCurrentDate(true);
      events.forEach((event, index, events) => {
        event.start = new Date(event.start);
        event.end = new Date(event.end);
        if (event.meta != null && event.meta.reservation != null && event.meta.reservation.createdById == this.authService.currentUser.id
          && event.end.getTime() >= currentTime.getTime()) {
          event.actions = this.actions;
        }
        this.calendarEvents.push(event);
      });

    });
  }
  
  //dayStartHour = 8;
  //dayEndHour = 20;

  apply() {
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    if (this.view == 'month') {
      this.filter.start = null;
      this.filter.startTime = null;
      this.filter.endTime = null;
      this.fetchReservations();
      this.fetchEvents();
    } else {
      this.institutionService.getInstitutionById(this.accountService.currentUser.institutionId).subscribe(result => {
        this.currentInstitution = result.data;
        this.dayStartHour = !this.currentInstitution || !this.currentInstitution.startTime ? 8 : this.currentInstitution.startTime;
        this.dayEndHour = !this.currentInstitution || !this.currentInstitution.endTime  ? 20 : this.currentInstitution.endTime - 1;
        this.filter.start = this.viewDate;
        this.filter.start = new Date(this.filter.start.setHours(this.dayStartHour, 0, 0, 0));
        this.filter.startTime = new Date(this.filter.start.setHours(this.dayStartHour, 0, 0, 0));
        this.filter.endTime = new Date(this.filter.start.setHours(this.dayEndHour, 0, 0, 0));

        if (this.view == 'day') {
          this.fetchLocations();
          if (this.bookingGrid) {
            this.bookingGrid.loadCalendarEvents(this.filter);
          } else {
            this.loadCalendarEvents(this.filter);
          }
        } else {
          this.filter.start = null;
          this.filter.end = null;
          //this.view = 'day';
          this.fetchReservations();
        }

      });
      
    }

    
  }

  clear() {
    this.filter = new CalendarFilter();
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.apply();
  }

  dayClicked({
    date,
    events
  }: {
      date: Date;
      events: Array<CalendarEvent<{ reservation: Reservation }>>;
    }): void {
    if (isSameMonth(date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) ||
        events.length === 0
      ) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
        this.viewDate = date;
      }
    }
  }

  eventClicked(event: CalendarEvent<{ reservation: Reservation }>): void {
    //window.open(
    //  `https://www.themoviedb.org/movie/${event.meta.film.id}`,
    //  '_blank'
    //);
  }

  startDragToCreate(
    segment: DayViewHourSegment,
    mouseDownEvent: MouseEvent,
    segmentElement: HTMLElement
  ) {
    const dragToSelectEvent: CalendarEvent = {
      id: this.events.length,
      title: 'New event',
      start: segment.date,
      meta: {
        tmpEvent: true
      }
    };
    this.events = [...this.events, dragToSelectEvent];
    const segmentPosition = segmentElement.getBoundingClientRect();
    this.dragToCreateActive = true;
    const endOfView = endOfWeek(this.viewDate);

    fromEvent(document, 'mousemove')
      .pipe(
        finalize(() => {
          delete dragToSelectEvent.meta.tmpEvent;
          this.dragToCreateActive = false;
          this.refresh();
        }),
        takeUntil(fromEvent(document, 'mouseup'))
      )
      .subscribe((mouseMoveEvent: MouseEvent) => {
        const minutesDiff = ceilToNearest(
          mouseMoveEvent.clientY - segmentPosition.top,
          30
        );

        const daysDiff =
          floorToNearest(
            mouseMoveEvent.clientX - segmentPosition.left,
            segmentPosition.width
          ) / segmentPosition.width;

        const newEnd = addDays(addMinutes(segment.date, minutesDiff), daysDiff);
        if (newEnd > segment.date && newEnd < endOfView) {
          dragToSelectEvent.end = newEnd;
        }
        this.refresh();
      });
  }

  private refresh() {
    this.events = [...this.events];
    this.cdr.detectChanges();
  }

  public goBack() {
    this.bookingComplete.emit(this.viewDate); 
  }
}
