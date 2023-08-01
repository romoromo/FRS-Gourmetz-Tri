import { Component, ChangeDetectionStrategy, Input, OnInit, AfterViewInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import {
  CalendarEvent,
  CalendarEventTimesChangedEvent,
  CalendarDayViewBeforeRenderEvent,
  CalendarEventAction
} from 'angular-calendar';
import { colors } from '../colors';
import { addHours, startOfDay, addMinutes } from 'date-fns';
import { CalendarFilter, Reservation } from 'src/app/models/reservation.model';
import { MatDialog } from '@angular/material';
import { CalendarBookingGridEditorComponent } from './calendar-booking-grid-editor.component';
import { ReservationService } from 'src/app/services/reservation.service';
import { LocationService } from 'src/app/services/location.service';
import { DayViewSchedulerComponent } from './day-view-scheduler.component';
import { AuthService } from 'src/app/services/auth.service';
import { DialogType, AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { Utilities } from 'src/app/services/utilities';
import { Permission } from 'src/app/models/permission.model';
import { AccountService } from 'src/app/services/account.service';

const locations = [
  {
    id: 0,
    name: 'Room 1',
    color: colors.yellow
  },
  {
    id: 1,
    name: 'Room 2',
    color: colors.blue
  }
];

@Component({
  selector: 'calendar-booking-grid',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'calendar-booking-grid.component.html'
})
export class CalendarBookingGridComponent implements OnInit, AfterViewInit{
  @Input() viewDate: Date;
  @Input() events: CalendarEvent[];
  @Input() locations: Location[];
  @Input() startHour: number;
  @Input() endHour: number;

  @Input('calendarFilter') filter: CalendarFilter;
  @ViewChild('dayViewer')
  dayViewer: DayViewSchedulerComponent

  currentEvent: CalendarEvent;
  //events: CalendarEvent[] = [];
  //  [
  //  {
  //    title: 'An event',
  //    color: users[0].color,
  //    start: addHours(startOfDay(new Date()), 5),
  //    end: addMinutes(addHours(startOfDay(new Date()), 6), 15),
  //    meta: {
  //      user: users[0]
  //    },
  //    resizable: {
  //      beforeStart: true,
  //      afterEnd: true
  //    },
  //    draggable: true
  //  },
  //  {
  //    title: 'Another event',
  //    color: users[1].color,
  //    start: addMinutes(addHours(startOfDay(new Date()), 2), 10),
  //    end: addMinutes(addHours(startOfDay(new Date()), 2), 45),
  //    meta: {
  //      user: users[1]
  //    },
  //    resizable: {
  //      beforeStart: true,
  //      afterEnd: true
  //    },
  //    draggable: true
  //  },
  //  {
  //    title: 'An 3rd event',
  //    color: users[0].color,
  //    start: addHours(startOfDay(new Date()), 7),
  //    end: addMinutes(addHours(startOfDay(new Date()), 8), 30),
  //    meta: {
  //      user: users[0]
  //    },
  //    resizable: {
  //      beforeStart: true,
  //      afterEnd: true
  //    },
  //    draggable: true
  //  }
  //];

  actions: CalendarEventAction[] = [
    //{
    //  label: '<i class="fa fa-fw fa-eye"></i>',
    //  onClick: ({ event }: { event: CalendarEvent }): void => {
    //    this.handleEvent('view', event);
    //  }
    //},
    {
      label: '<i class="fa fa-fw fa-pencil"></i>',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.handleEvent('update', event);
      }
    },
    {
      label: '<i class="fa fa-fw fa-times"></i>',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.handleEvent('delete', event);
      }
    }
  ];

  constructor(private accountService: AccountService, private alertService: AlertService,private reservationService: ReservationService, private locationService: LocationService, public dialog: MatDialog, private cd: ChangeDetectorRef,
    private authService: AuthService ) {
  }

  ngOnInit(): void {
    if (!this.events) this.events = [];
  }

  ngAfterViewInit() {
    if (!this.events) this.events = [];
    this.dayViewer.setEvents(this.events);
    this.cd.detectChanges();

  }

  loadCalendarEvents(filter?: CalendarFilter) {
    this.reservationService.getCalendarEvents(filter).subscribe(events => {
      this.events = [];
      var currentTime = Utilities.getCurrentDate(true);
      events.forEach((event, index, events) => {
        event.start = new Date(event.start);
        event.end = new Date(event.end);
        if (event.meta != null && event.meta.reservation != null &&
          (event.meta.reservation.createdById == this.authService.currentUser.id || this.isAdmin)
         && event.end.getTime() >= currentTime.getTime()) {
          event.actions = this.actions;
        }
        this.events.push(event);
      });
      this.events = [...this.events];
      this.cd.detectChanges();
    });
  }

  eventTimesChanged({
    event,
    newStart,
    newEnd
  }: CalendarEventTimesChangedEvent): void {
    event.start = newStart;
    event.end = newEnd;
    this.events = [...this.events];
  }

  locationChanged({ event, newLocation }) {
    event.color = newLocation.color;
    event.meta.location = newLocation;
    this.events = [...this.events];
  }

  locationAdded({ event }) {
    event.actions = this.actions;
    this.events = [...this.events, event];
    this.currentEvent = event;
    //open a dialog to update here
    var reservation = new Reservation();
    reservation.startDate = event.start;
    reservation.endDate = event.end;
    
    reservation.institutionId = this.authService.currentUser.institutionId;
    this.openDialog(reservation, event.meta.location);

  }

  eventsUpdate({ events }) {
    this.events = events;
  }

  handleEvent(action: string, event: CalendarEvent): void {
    this.reservationService.getReservation(event.id.toString()).subscribe(result => {

      if (action === "update") {
        this.openDialog(result.data, event.meta.location);
      } else if (action === "delete") {
        this.deleteReservation(result.data);
      } else if (action === "view") {
        this.openDialog(result.data, event.meta.location);
      }
      
    });

  }

  apply(filter?: CalendarFilter) {
    //this.loadCalendarEvents(filter);
  }

  showDeleteDialog(row: Reservation, msg: string, okCallback?: (val?: any) => any, cancelCallback?: (val?: any) => any): void {
    if (!okCallback || !cancelCallback) {
      this.alertService.showDialog(msg, DialogType.confirm, (val) => this.configure(row, true, val), () => this.configure(row, false), "This and following events", "All events");
    } else {
      this.alertService.showDialog(msg, DialogType.confirm, okCallback, cancelCallback, "This and following events", "All events");
    }
  }

  configure(row: Reservation, response: boolean, value?: string) {
    if (response) {
      row.recurApplyChangesType = "OTHER_EVENTS";
    }
    else {
      row.recurApplyChangesType = "ALL_EVENTS";
    }
  }

  deleteReservation(row: Reservation) {
    if (row.isRecurring) {
      this.alertService.showDialog('Are you sure you want to delete the \"' + row.shortDescription + '\" reservation?',
        DialogType.confirm, () => {
          this.showDeleteDialog(row, "Delete recurring event", (val) => {
            row.recurApplyChangesType = "OTHER_EVENTS";
            this.deleteReservationHelper(row);
          },
            () => {
              row.recurApplyChangesType = "ALL_EVENTS";
              this.deleteReservationHelper(row);
            });
        });
      
    } else {
      row.recurApplyChangesType = "";
      this.alertService.showDialog('Are you sure you want to delete the \"' + row.shortDescription + '\" reservation?', DialogType.confirm, () => this.deleteReservationHelper(row));
    }
  }

  deleteReservationHelper(row: Reservation) {

    this.alertService.startLoadingMessage("Deleting...");

    this.reservationService.deleteReservation(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        var index = this.events.findIndex(e => e.id == row.id);
        this.events.splice(index, 1);
        this.events = [...this.events];
        this.cd.detectChanges();
      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the reservation.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  openDialog(reservation: Reservation, location: any): void {
    const dialogRef = this.dialog.open(CalendarBookingGridEditorComponent, {
      data: { header: "New Reservation for " + location.name, reservation: reservation, location: location },
      width: '800px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      var index = this.events.findIndex(e => result && result.reservation && e.id == result.reservation.id && e.meta.location.id == result.reservation.locationId);


      if (index < 0) {
        index = this.events.findIndex(e => this.currentEvent != null && e.id == this.currentEvent.id && e.meta.location.id == this.currentEvent.meta.location.id);
      }

      if (result) {
        if (!result.isCancel) {
          if (result.reservation && index > -1) {
            this.events[index].start = new Date(result.reservation.startDateTime);
            this.events[index].end = new Date(result.reservation.endDateTime);
            this.events[index].allDay = result.reservation.isAllDay;
            this.events[index].title = result.reservation.shortDescription; //`<div><b>Subject: </b> <br>${result.reservation.shortDescription}</div><br><div><b>Description: </b> <br>${result.reservation.longDescription}</div>`;
            this.events[index].id = result.reservation.id;
            this.events = [...this.events];
            this.cd.detectChanges();
          }
          //this.loadCalendarEvents(null);
        } else {
          this.events.splice(index, 1);
          this.events = [...this.events];
          this.cd.detectChanges();
        }
      } 
    });
  }

  get isAdmin(): boolean {
    return this.accountService.userHasPermission(Permission.manageRolesPermission);
  }
}
