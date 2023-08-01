import { Component, EventEmitter, Injectable, Output, Input, ChangeDetectorRef, OnInit, Inject, LOCALE_ID } from '@angular/core';
import { CalendarDayViewComponent, CalendarUtils, DateAdapter } from 'angular-calendar';
import { DayView, DayViewEvent, GetDayViewArgs, DayViewHourSegment, CalendarEvent } from 'calendar-utils';
import { UserInfoComponent } from '../controls/user-info.component';
import { Location } from 'src/app/models/location.model';
import { UserInfoVal } from 'src/app/models/facility.model';
import { Reservation } from 'src/app/models/reservation.model';
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
import { finalize, takeUntil } from 'rxjs/operators';

const EVENT_WIDTH = 150;

// extend the interface to add the array of reservations
interface DayViewScheduler extends DayView {
  reservations: any[];
  locations: any[];
}

@Injectable()
export class DayViewSchedulerCalendarUtils extends CalendarUtils {
  getDayView(args: GetDayViewArgs): DayViewScheduler {
    const view: DayViewScheduler = {
      ...super.getDayView(args),
      reservations: [],
      locations: []
    };

    var i = 0;
    view.events.forEach(({ event }) => {
      // assumes user objects are the same references,
      // if 2 users have the same structure but different object references this will fail
      if (!view.reservations.includes(event.meta.reservation)) {
        view.reservations.push(event.meta.reservation);
      }

      if (i++ == 0) {
        view.locations = event.meta.locations;
      }
    });
    // sort the users by their names
   // view.reservations.sort((location1, location2) => location1.description.localeCompare(location2.description));
    view.events = view.events.map(dayViewEvent => {
      if (view.locations) {
        const index = view.locations.map(function (e) { return e.id; }).indexOf(dayViewEvent.event.meta.reservation.locationId);
        //const index = view.locations.indexOf(dayViewEvent.event.meta.reservation);
        dayViewEvent.left = index * EVENT_WIDTH; // change the column of the event
      }
      return dayViewEvent;
    });
    view.width = view.reservations.length * EVENT_WIDTH;
    return view;
  }
}

@Component({
  selector: 'mwl-day-view-scheduler',
  styles: [
    `
      .day-view-column-headers {
        display: flex;
        margin-left: 70px;
      }
      .day-view-column-header {
        width: 250px;
        border: solid 1px #e1e1e1;
        text-align: center;
        height: 20px;
      }
    `
  ],
  providers: [
    {
      provide: CalendarUtils,
      useClass: DayViewSchedulerCalendarUtils
    }
  ],
  templateUrl: 'day-view-scheduler.component.html'
})

export class DayViewSchedulerComponent extends CalendarDayViewComponent {

  constructor(private cd: ChangeDetectorRef, private ut: CalendarUtils, @Inject(LOCALE_ID) private loc: string, private da: DateAdapter) {
    super(cd, ut, loc, da);
  }
  view: DayViewScheduler;
  @Input() users: UserInfoVal[];
  @Input() cdref: ChangeDetectorRef;
  @Input() locationColList: Location[];
  @Output() reservationChanged = new EventEmitter();

  dragToCreateActive = false;




  getTimezoneOffsetString(date: Date): string {
    const timezoneOffset = date.getTimezoneOffset();
    const hoursOffset = String(
      Math.floor(Math.abs(timezoneOffset / 60))
    ).padStart(2, '0');
    const minutesOffset = String(Math.abs(timezoneOffset % 60)).padEnd(2, '0');
    const direction = timezoneOffset > 0 ? '-' : '+';
    return `T00:00:00${direction}${hoursOffset}${minutesOffset}`;
  }

  floorToNearest(amount: number, precision: number) {
    return Math.floor(amount / precision) * precision;
  }

  ceilToNearest(amount: number, precision: number) {
    return Math.ceil(amount / precision) * precision;
  }
  eventDragged(dayEvent: DayViewEvent, xPixels: number, yPixels: number): void {
    if (yPixels !== 0) {
      super.dragEnded(dayEvent, { y: yPixels, x: 0 } as any); // original behaviour
    }
    if (xPixels !== 0) {
      const columnsMoved = xPixels / EVENT_WIDTH;
      const currentColumnIndex = this.view.reservations.findIndex(
        location => location === dayEvent.event.meta.location
      );
      const newIndex = currentColumnIndex + columnsMoved;
      const newReservation = this.view.reservations[newIndex];
      if (newReservation) {
        this.reservationChanged.emit({ event: dayEvent.event, newReservation });
      }
    }
  }

  startDragToCreate(
    segment: DayViewHourSegment,
    mouseDownEvent: MouseEvent,
    segmentElement: HTMLElement
  ) {
    const dragToSelectEvent: CalendarEvent = {
      id: this.events.length,
      title: 'New reservation',
      start: segment.date,
      meta: {
        tmpEvent: true
      }
    };
    this.events = [...this.events, dragToSelectEvent];
    const segmentPosition = segmentElement.getBoundingClientRect();
    this.dragToCreateActive = true;
    const endOfView = endOfDay(this.viewDate);

    fromEvent(document, 'mousemove')
      .pipe(
        finalize(() => {
          delete dragToSelectEvent.meta.tmpEvent;
          this.dragToCreateActive = false;
          this.refresh;
          this.refreshEv();
        }),
        takeUntil(fromEvent(document, 'mouseup'))
      )
      .subscribe((mouseMoveEvent: MouseEvent) => {
        const minutesDiff = this.ceilToNearest(
          mouseMoveEvent.clientY - segmentPosition.top,
          30
        );

        const daysDiff =
          this.floorToNearest(
            mouseMoveEvent.clientX - segmentPosition.left,
            segmentPosition.width
          ) / segmentPosition.width;

        const newEnd = addMinutes(segment.date, minutesDiff);//addDays(addMinutes(segment.date, minutesDiff), daysDiff);
        if (newEnd > segment.date && newEnd < endOfView) {
          dragToSelectEvent.end = newEnd;
        }
        this.refreshEv();
        this.refresh;
      });
  }

  private refreshEv() {
    this.events = [...this.events];
    //this.view.events = this.events;
    this.cdref.detectChanges();
  }
}
