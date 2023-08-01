import { Component, EventEmitter, Injectable, Output, ChangeDetectorRef, Inject, LOCALE_ID, Input, ChangeDetectionStrategy } from '@angular/core';
import { CalendarDayViewComponent, CalendarUtils, DateAdapter, CalendarEventTitleFormatter, CalendarDayViewBeforeRenderEvent } from 'angular-calendar';
import { DayView, DayViewEvent, GetDayViewArgs, CalendarEvent, DayViewHourSegment } from 'calendar-utils';
import { takeUntil, finalize } from 'rxjs/operators';
import { fromEvent } from 'rxjs';
import { endOfDay, addMinutes } from 'date-fns';
import { Utilities } from 'src/app/services/utilities';

const EVENT_TOP = 30;
const EVENT_WIDTH = 140;
const EVENT_HEIGHT = 30;
// extend the interface to add the array of locations
interface DayViewScheduler extends DayView {
  locations: any[];
}

@Injectable()
export class DayViewSchedulerCalendarUtils extends CalendarUtils {
  getDayView(args: GetDayViewArgs): DayViewScheduler {
    const view: DayViewScheduler = {
      ...super.getDayView(args),
      locations: [],
      
    };

    const objs = []

    view.events.forEach(({ event }) => {
      if (!event.meta.isDefault) {
        // assumes location objects are the same references,
        // if 2 locations have the same structure but different object references this will fail
        if (event.meta.location && view.locations.filter(f => f.id == event.meta.location.id).length == 0) {
          view.locations.push(event.meta.location);
        }
      } else {
        if (!objs.includes(event.meta.obj)) {
          objs.push(event.meta.obj);
        }
      }
      
    });

    // sort the objs by their names
    objs.sort((o1, o2) => o1.label.localeCompare(o2.label));

    // sort the locations by their names
    view.locations.sort((location1, location2) => location1.name.localeCompare(location2.name));
    view.events = view.events.map(dayViewEvent => {
      if (!dayViewEvent.event.meta.isDefault) {
        const index = view.locations.findIndex(item => item.id == dayViewEvent.event.meta.location.id);
        dayViewEvent.top = (index + 1) * EVENT_TOP; // change the column of the event
        dayViewEvent.height = EVENT_HEIGHT;
        dayViewEvent.left = EVENT_WIDTH * (dayViewEvent.event.start.getHours() - view.period.start.getHours()) + (EVENT_WIDTH * dayViewEvent.event.start.getMinutes() / 60) + (dayViewEvent.event.start.getHours() - view.period.start.getHours());
        let width = EVENT_WIDTH - dayViewEvent.left;
        if (dayViewEvent.event.end) {
          width = (EVENT_WIDTH * (dayViewEvent.event.end.getHours() - view.period.start.getHours()) + (EVENT_WIDTH * dayViewEvent.event.end.getMinutes() / 60) + (dayViewEvent.event.end.getHours() - view.period.start.getHours())) - dayViewEvent.left;
        }

        dayViewEvent.width = width;
      } else {
        const index = objs.indexOf(dayViewEvent.event.meta.obj);
        dayViewEvent.left = index * EVENT_WIDTH; // change the column of the event
        return dayViewEvent;
      }
      return dayViewEvent;
    });
    view.width = EVENT_WIDTH; //view.locations.length * EVENT_WIDTH;
    return view;
  }
}





export class CustomEventTitleFormatter extends CalendarEventTitleFormatter {
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
      return super.dayTooltip(event, tooltip);
    }
  }
}

@Component({
  // tslint:disable-line max-classes-per-file
  selector: 'mwl-day-view-scheduler',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styles: [
    `
.cal-day-view{
display: flex;
}
      .day-view-column-headers {
        
        
      }
      .day-view-column-header {
        width: 250px;
        border: thin solid #e1e1e1;
        text-align: center;
        height: 30px;
      }
.cal-hour-header .cal-hour-cols{
    display: flex;
    width: 100%;
    border: solid 1px #e1e1e1;
    position: relative;
    height: 30px;
}
.cal-hour{

    display: grid;
    grid-template-columns: 70px 70px;
}

.cal-hour-cols .cal-hour{
    border-right: 1px solid #e1e1e1;
}
.cal-hour-cols .cal-time{
border-right: thin dashed #e1e1e1 !important;
}

.cal-day-view .cal-time, .cal-day-view .cal-hour-segment.cal-after-hour-start .cal-time{
display:block !important;
}

.cal-day-view-hour-container{
display: flex;
    width: 90%;
    overflow-x: auto;
    overflow-y: hidden;
}

.cal-day-view .cal-hour:not(:last-child) .cal-hour-segment, .cal-day-view .cal-hour:last-child :not(:last-child) .cal-hour-segment
{
border-bottom: none !important;
border-right: thin dashed #e1e1e1;
}

.cal-hour-container .cal-hour-cols .cal-hour{
    background-color: #fff;
}

.cal-past-time {
background: repeating-linear-gradient( -55deg, #f7f7f7, #f1f1f1 10px, #f7f6f6 10px, #f1f1f1 20px );
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
  @Input() viewDate: Date;
  @Input() events: CalendarEvent[];
  @Input() locations: Location[];
  @Input() isDefaultView: boolean;

  view: DayViewScheduler;

  @Output() locationChanged = new EventEmitter();
  @Output() locationAdded = new EventEmitter();
  @Output() eventsUpdate = new EventEmitter();
  
  eventDragged(dayEvent: DayViewEvent, xPixels: number, yPixels: number): void {
    if (yPixels !== 0) {
      super.dragEnded(dayEvent, { y: yPixels, x: 0 } as any); // original behaviour
    }
    if (xPixels !== 0) {
      const columnsMoved = xPixels / EVENT_WIDTH;
      const currentColumnIndex = this.view.locations.findIndex(
        location => location === dayEvent.event.meta.location
      );
      const newIndex = currentColumnIndex + columnsMoved;
      const newUser = this.view.locations[newIndex];
      if (newUser) {
        this.locationChanged.emit({ event: dayEvent.event, newUser });
      }
    }
  }

  getEvents(location: any, events: DayViewEvent[]) {
    var ev: DayViewEvent[] = [];
    events.forEach(function (e) {
      if (e.event.meta.location.id == location.id) {
        ev.push(e);
      }
    });

    return ev;
  }

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

  createDefaultEvent(
    segment: DayViewHourSegment,
    mouseDownEvent: MouseEvent,
    segmentElement: HTMLElement,
    location: any) {
    console.log('createDefaultEvent');
    console.log(segment);
    console.log(mouseDownEvent);
    console.log(segmentElement);
    console.log(location);
    if (segment.date.getTime() >= Utilities.getCurrentDate(true).getTime()) {
      const endDate = addMinutes(segment.date, 30);
      const event: CalendarEvent = {
        id: this.events ? this.events.length : 1,
        title: 'New reservation',
        start: segment.date,
        end: endDate,
        meta: {
          tmpEvent: true,
          location: location
        }
      };
      //this.events = [...this.events, event];
      this.locationAdded.emit({ event: event, location });
      //this.refreshEv();
    }
  }

  startDragToCreate(
    segment: DayViewHourSegment,
    mouseDownEvent: MouseEvent,
    segmentElement: HTMLElement,
    location: any
  ) {


    if (segment.date.getTime() >= Utilities.getCurrentDate(true).getTime()) {
      
      const dragToSelectEvent: CalendarEvent = {
        id: this.events ? this.events.length : 1,
        title: 'New reservation',
        start: segment.date,
        meta: {
          tmpEvent: true,
          location: location
        }
      };
      this.events = [...this.events, dragToSelectEvent];
      const segmentPosition = segmentElement.getBoundingClientRect();
      this.dragToCreateActive = true;
      
      var endOfView = this.view.period.end ? new Date(this.view.period.end.setMinutes(this.view.period.end.getMinutes() + 1)) : endOfDay(this.viewDate);

      fromEvent(document, 'mousemove')
        .pipe(
          finalize(() => {
            delete dragToSelectEvent.meta.tmpEvent;
          this.dragToCreateActive = false;

          if (dragToSelectEvent.end.getTime() > endOfView.getTime()) {
            dragToSelectEvent.end = endOfView;
          }
            //this.refresh;
            this.locationAdded.emit({ event: dragToSelectEvent, location });
            this.refreshEv();
          }),
          takeUntil(fromEvent(document, 'mouseup'))
        )
        .subscribe((mouseMoveEvent: MouseEvent) => {
          const minutesDiff = this.ceilToNearest(
            mouseMoveEvent.clientX - segmentPosition.left,
            60
          );

          const daysDiff =
            this.floorToNearest(
              mouseMoveEvent.clientX - segmentPosition.left,
              segmentPosition.width
            ) / segmentPosition.width;
          
          const newEnd = addMinutes(segment.date, minutesDiff / 2);//addDays(addMinutes(segment.date, minutesDiff), daysDif;
          if (newEnd.getTime() > segment.date.getTime() && newEnd.getTime() <= endOfView.getTime()) {
            dragToSelectEvent.end = newEnd;

          }
          this.refreshEv();
          //this.refresh;
        });
    }
  }

  private refreshEv() {
    this.events = [...this.events];
    //this.view.events = this.events;
    //console.log(this.events);
    this.eventsUpdate.emit({ events: this.events });
    this.cd.detectChanges();
  }

  setEvents(events: CalendarEvent[]) {
    this.events = events;
  }

  cssPastTime(segment: DayViewHourSegment) {
    return segment.date.getTime() < Utilities.getCurrentDate(true).getTime() ? 'cal-past-time' : ''
  }
}
