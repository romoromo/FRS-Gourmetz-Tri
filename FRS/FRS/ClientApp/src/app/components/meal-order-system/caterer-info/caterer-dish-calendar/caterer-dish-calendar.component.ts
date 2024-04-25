import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Output, EventEmitter, ChangeDetectorRef, NgZone, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { CalendarView, CalendarEvent, CalendarEventAction, CalendarMonthViewBeforeRenderEvent, CalendarWeekViewBeforeRenderEvent, CalendarDayViewBeforeRenderEvent } from 'angular-calendar';
import { ViewPeriod } from 'calendar-utils';
import RRule from 'rrule';
import * as moment from 'moment';
import { fadeInOut } from 'src/app/services/animations';
import { AlertService, DialogType, MessageSeverity } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { Utilities } from 'src/app/services/utilities';
import { Filter } from 'src/app/models/sieve-filter.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import {
  isSameMonth,
  isSameDay
} from 'date-fns';
import { MatDialog } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { DishBlockUnblockDateModel, DishCycle, DishCycleBlockedDate } from 'src/app/models/meal-order/dish-cycle';
import { ViewDishCycleMenuComponent } from '../../dish-cycle/view-menu/view-menu.component';

@Component({
  selector: 'dish-cycle-calendar',
  templateUrl: './caterer-dish-calendar.component.html',
  styleUrls: ['./caterer-dish-calendar.component.css'],
  animations: [fadeInOut],
  encapsulation: ViewEncapsulation.None,
})
export class DishCycleCalendarComponent implements OnInit, AfterViewInit {
  schedules: DishCycle[] = [];
  editedDishCycle: DishCycle;
  sourceDishCycle: DishCycle;
  editingDishCycleName: { shortDescription: string };
  loadingIndicator: boolean;
  activeDayIsOpen: boolean = false;
  public catererId: string;
  public catererName: string;
  outletProfileId: any;
  @Input()

  view: CalendarView = CalendarView.Month;
  viewDate: Date = new Date();
  @Output() viewChange: EventEmitter<string> = new EventEmitter();

  @Output() viewDateChange: EventEmitter<Date> = new EventEmitter();

  actions: CalendarEventAction[] = [
    //{
    //  label: '<span class="action-dish"><i class="fa fa-fw fa-pencil" style="color:white"></i> Edit Dish Cycle </span>',
    //  //a11yLabel: 'Edit',
    //  onClick: ({ event }: { event: CalendarEvent }): void => {
    //    //this.editDishCycle(event.meta);
    //  },
    //},
    {
      label: '<span class="action-dish"><i class="fa fa-list-alt" style="color:white"></i> Add/Remove Dish </span>',
      //a11yLabel: 'Delete',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        let startD = moment(new Date(event.meta.startDate).setHours(0));
        let endD = moment(event.start);
        let totalDays = endD.diff(startD, "days");
        let totalSchedules = event.meta.schedules ? event.meta.schedules.length : 0;

        let day = (totalDays % totalSchedules) + 1; //event.start.getDay();
        this.openViewDishDialog(event.meta, day);
        console.log(event);
      },
    },
  ];

  private outletProfiles = [];

  calendarEvents: CalendarEvent<DishCycle>[];

  viewPeriod: ViewPeriod;

  //refresh: Subject<any> = new Subject();

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  //@ViewChild('dishCycleEditor')
  //dishCycleEditor: DishCycleEditorComponent;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private dishService: DishService, private cdr: ChangeDetectorRef, private deliveryService: DeliveryService, public zone: NgZone, public dialog: MatDialog,
    private route: ActivatedRoute) {
    this.route.params.subscribe(queryParams => {
      this.catererId = queryParams["catererId"];
      this.catererName = queryParams["name"]; 
    });

    this.getOutletProfiles();
  }

  handleEvent(action: string, event: CalendarEvent): void {

  }

  eventUpdateEvent() {
    this.activeDayIsOpen = false;
    this.calendarEvents.length = 0;
    this.calendarEvents = [];
    this.schedules.forEach((event) => {
      if (!event.rrule || !event.rrule.byweekday || event.rrule.byweekday.length == 0) return;

      const rule: RRule = new RRule({
        ...event.rrule,
        dtstart: moment(this.viewPeriod.start).startOf('day').toDate(),
        until: moment(this.viewPeriod.end).endOf('day').toDate(),
      });
      const { label, color } = event;

      let events = [];

      rule.all().forEach((date) => {
        date.setDate(date.getDate() - 1);
        if (date >= new Date(new Date(event.startDate).setHours(0)) && date <= new Date(new Date(event.endDate).setHours(0))) {
          let start = new Date(date);
          let end = new Date(date);
          start.setHours(0);
          start.setMinutes(0);
          start.setMilliseconds(0);

          end.setHours(0);
          end.setMinutes(0);
          end.setMilliseconds(0);

          let blockedDates = event.blockedDates ? event.blockedDates.filter(e => date.getTime() == new Date(e.effectiveDate).getTime()) : [];
          events.push({
            title: label,
            color,
            start: start,
            end: end,
            actions: this.actions,
            meta: event,
            isBlocked: blockedDates && blockedDates.length > 0
          });
        }
      });

      this.calendarEvents = this.calendarEvents.concat(Array.from(events));
    });
    //this.refresh.next();

    this.cdr.detectChanges();
  }

  updateCalendarEvents(
    viewRender:
      | CalendarMonthViewBeforeRenderEvent
      | CalendarWeekViewBeforeRenderEvent
      | CalendarDayViewBeforeRenderEvent
  ): void {
    if (!this.viewPeriod ||
      !moment(this.viewPeriod.start).isSame(viewRender.period.start) ||
      !moment(this.viewPeriod.end).isSame(viewRender.period.end)
    ) {
      this.viewPeriod = viewRender.period;
      this.calendarEvents = [];

      this.eventUpdateEvent();
    }
  }

  dayClicked(
    date,
    event
  ): void {
    if (isSameMonth(date, this.viewDate)) {
      this.viewDate = date;
    }

    let startD = moment(new Date(event.meta.startDate).setHours(0));
    let endD = moment(event.start);
    let totalDays = endD.diff(startD, "days");
    let totalSchedules = event.meta.schedules ? event.meta.schedules.length : 0;

    let day = (totalDays % totalSchedules) + 1; //event.start.getDay();
    this.openViewDishDialog(event.meta, day);
    console.log(event);
  }

  eventClicked(event: CalendarEvent<{ dishCycle: DishCycle }>): void {
    //window.open(
    //  `https://www.themoviedb.org/movie/${event.meta.film.id}`,
    //  '_blank'
    //);
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    //this.loadData();
  }





  ngAfterViewInit() {

    //this.dishCycleEditor.changesSavedCallback = () => {
    //  this.loadData();
    //  this.editorModal.hide();
    //};

    //this.dishCycleEditor.changesCancelledCallback = () => {
    //  this.editedDishCycle = null;
    //  this.sourceDishCycle = null;
    //  this.editorModal.hide();
    //};

    this.cdr.detectChanges();
  }

  loadData(id?: string) {
    //this.alertService.startLoadingMessage();
    //this.loadingIndicator = true;
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    f += '(IsActive) == true';
    if (id) {
      f += ',(OutletProfileId)==' + id;
    }
    
    let filter = new Filter();
    filter.filters = f;
    this.dishService.getDishCyclesByFilter(filter)
      .subscribe(results => {
        let dishCycles = results.pagedData;

        dishCycles.forEach((dishCycle, index, dishCycles) => {
          try {
            (<any>dishCycle).index = index + 1;

            let weekday = [];

            //if (dishCycle.schedules) {
            //  let dayRules = [RRule.MO, RRule.TU, RRule.WE, RRule.TH, RRule.FR, RRule.SA, RRule.SU];
            //  dishCycle.schedules.forEach((schedule, i, s) => {
            //    weekday.push(dayRules[(schedule.day % 7) - 1]);
            //  });
            //}

            dishCycle.rrule = {
              freq: RRule.WEEKLY,
              byweekday: [RRule.MO, RRule.TU, RRule.WE, RRule.TH, RRule.FR, RRule.SA, RRule.SU],
            }
          } catch (ex) { }
        });


        this.schedules = dishCycles;

        this.eventUpdateEvent();
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
            MessageSeverity.error);
        });
  }

  getOutletProfiles() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';

    this.deliveryService.getOutletProfilesByFilter(filter)
      .subscribe(results => {
        this.outletProfiles = results.pagedData;
        this.loadData();
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving outlet profiles.\r\n"`,
            MessageSeverity.error);
        })
  }

  onOutletProfileChange(outletProfileId: string) {
    this.loadData(outletProfileId);
  }
  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.label));
  }


  onEditorModalHidden() {
    this.editingDishCycleName = null;
    //this.dishCycleEditor.resetForm(true);
  }

  blockDate(ev, day) {
    this.activeDayIsOpen = false;
    
    if (day && day.events && day.events.length > 0) {
      let isBlocked = this.isBlocked(day, day.events);
      this.alertService.showDialog(`Are you sure you want to "${(isBlocked ? "unblock": "block")}" this date?`, DialogType.confirm, () => this.blockDishCycleHelper(day, isBlocked));
    }
  }


  blockDishCycleHelper(day: any, isUnblock) {
    let m = new Date(day.date);
    let date = new Date(m.getFullYear(), m.getMonth(), m.getDate());
    let blockedDates = day.events.map(function (e) {
      let blockedDate = new DishCycleBlockedDate();
      blockedDate.effectiveDate = date.toLocaleDateString();
      blockedDate.label = e.meta.label;
      blockedDate.dishCycleId = e.meta.id;
      return blockedDate;
    });
    this.alertService.startLoadingMessage("Processing...");
    this.loadingIndicator = true;

    let model = new DishBlockUnblockDateModel();
    model.isUnblock = isUnblock;
    model.blockedDates = blockedDates;
    this.dishService.blockunblockdate(model)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.alertService.showMessage("Success", `Date is now \"${isUnblock ? "unblocked" : "blocked"}\"`, MessageSeverity.success);
        this.loadData(this.outletProfileId);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Error", `An error occured while processing the record.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  isBlocked(day, events) {
    return events && events.length > 0 && events[0].isBlocked;
  }

  openViewDishDialog(dishCycle: DishCycle, day): void {
    const dialogRef = this.dialog.open(ViewDishCycleMenuComponent, {
      data: { header: 'Dish List', dishCycle: dishCycle, day, catererId: this.catererId },
      width: '600px'
    });
  }


  get canManageDishCycles() {
    return true;// || this.accountService.userHasPermission(Permission.manageDishCyclesPermission)
  }

}
