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
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { Utilities } from 'src/app/services/utilities';
import { Filter } from 'src/app/models/sieve-filter.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import {
  isSameMonth,
  isSameDay
} from 'date-fns';
import { MatDialog } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { ViewMenuCycleMenuComponent } from '../../menu-cycle/view-menu/view-menu.component';
import { Student } from 'src/app/models/meal-order/student.model';
import { CatererInfo } from 'src/app/models/meal-order/caterer-info.model';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { OutletClassRoster, OutletClassRosterSchedule, OutletClassRosterScheduleByDay } from 'src/app/models/meal-order/outlet-class-roster.model';

@Component({
  selector: 'student-calendar',
  templateUrl: './student-calendar.component.html',
  styleUrls: ['./student-calendar.component.css'],
  animations: [fadeInOut],
  encapsulation: ViewEncapsulation.None,
})
export class StudentMenuCycleCalendarComponent implements OnInit, AfterViewInit {
  schedules: OutletClassRosterSchedule[] = [];
  rosters: OutletClassRoster[] = [];
  editedMenuCycle: OutletClassRosterSchedule;
  sourceMenuCycle: OutletClassRosterSchedule;
  editingMenuCycleName: { shortDescription: string };
  loadingIndicator: boolean;
  activeDayIsOpen: boolean = false;
  studentId: any;
  @Input()

  view: CalendarView = CalendarView.Month;
  viewDate: Date = new Date();
  @Output() viewChange: EventEmitter<string> = new EventEmitter();

  @Output() viewDateChange: EventEmitter<Date> = new EventEmitter();

  actions: CalendarEventAction[] = [
    //{
    //  label: '<span class="action-menu"><i class="fa fa-fw fa-pencil" style="color:white"></i> Edit Menu Cycle </span>',
    //  //a11yLabel: 'Edit',
    //  onClick: ({ event }: { event: CalendarEvent }): void => {
    //    //this.editMenuCycle(event.meta);
    //  },
    //},
    {
      label: '<span class="action-menu"><i class="fa fa-list-alt" style="color:white"></i> Enable/Disable Menu </span>',
      //a11yLabel: 'Delete',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        //let startD = moment(new Date(event.meta.startDate).setHours(0));
        //let endD = moment(event.start);
        //let totalDays = endD.diff(startD, "days");
        //let totalSchedules = event.meta.schedules ? event.meta.schedules.length : 0;

        //let day = (totalDays % totalSchedules) + 1; //event.start.getDay();
        //this.openViewMenuDialog(event.meta, day);
        //console.log(event);
      },
    },
  ];

  private studentProfiles = [];
  private student: Student;
  private catererId: string;
  private outletId: string;
  private blockedDates: any[];
  private outletBlockedDates: any[];
  calendarEvents: CalendarEvent<OutletClassRosterSchedule>[];

  viewPeriod: ViewPeriod;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private menuService: MenuService, private cdr: ChangeDetectorRef, private deliveryService: DeliveryService, public zone: NgZone, public dialog: MatDialog,
    private route: ActivatedRoute, private studentService: StudentService) {
    this.route.params.subscribe(queryParams => {
      this.studentId = queryParams["id"];
      this.catererId = queryParams["catererId"];
      this.outletId = queryParams["outletId"];
      this.loadData(this.studentId);
    });

  }

  handleEvent(action: string, event: CalendarEvent): void {

  }

  eventUpdateEvent() {
    this.activeDayIsOpen = false;
    this.calendarEvents = [];
    this.calendarEvents.length = 0;
    

    
    this.rosters.forEach((roster) => {
      //if (!schedule.rrule || !schedule.rrule.byweekday || schedule.rrule.byweekday.length == 0) return;
      const rule: RRule = new RRule({
        ...roster.rrule,
        dtstart: moment(this.viewPeriod.start).startOf('day').toDate(),
        until: moment(this.viewPeriod.end).endOf('day').toDate(),
      });

      let dayCount = roster.schedules.length;
      let day: number = 1;
      let events = [];
      rule.all().forEach((date) => {
        //const { color } = event;
        //if(this.view == 'month') date.setDate(date.getDate() - 1);
        let rStartDate = new Date(roster.startDate);
        rStartDate.setHours(0);
        rStartDate.setMinutes(0);
        rStartDate.setSeconds(0);
        rStartDate.setMilliseconds(0);

        let rEndDate = new Date(roster.endDate);
        rEndDate.setHours(0);
        rEndDate.setMinutes(0);
        rEndDate.setSeconds(0);
        rEndDate.setMilliseconds(0);
        if (date >= new Date(rStartDate) && date <= new Date(rEndDate)) {
          
          var diff = Math.abs(date.getTime() - rStartDate.getTime()) + 1;
          day = Math.ceil(diff / (1000 * 3600 * 24)) % roster.schedules.length;
          day = day == 0 ? roster.schedules.length : day;
          let schedules = roster.schedules.filter(e => e.day == day);
          if (schedules && schedules.length > 0) {
            let schedule = schedules[0];

            var start = new Date(date);
            var end = new Date(date);
            start.setHours(0);
            start.setMinutes(0);
            start.setSeconds(0);
            start.setMilliseconds(0);

            end.setHours(0);
            end.setMinutes(0);
            end.setSeconds(0);
            end.setMilliseconds(0);

            schedule.periods.forEach((p) => {
              if (p.classes && p.classes.length > 0) {
                let s = new Date(p.mealSessionDetailStartDate);
                var pStart = start;
                pStart.setHours(s.getHours());
                pStart.setMinutes(s.getMinutes());
                var pEnd = new Date(pStart);
                pEnd.setMinutes(pStart.getMinutes() + 30);

                var blockedDates = -1;
                this.blockedDates.forEach((bd, index, bds) => {
                  if (new Date(bd.effectiveDate).getTime() === end.getTime()) {
                    blockedDates = 0;
                  }
                });

                var outletBlockedDates = -1;
                this.outletBlockedDates.forEach((bd, index, bds) => {
                  if (new Date(bd.effectiveDate).getTime() === end.getTime()) {
                    outletBlockedDates = 0;
                  }
                });

                //for (var i in this.blockedDates) {
                //  if (new Date(this.blockedDates[i].effectiveDate).getTime() === pStart.getTime()) {
                //    blockedDates = 0;
                //    break;
                //  }
                //}
                //blockedDates = this.blockedDates ? this.blockedDates.findIndex(e => new Date(e.effectiveDate).getTime() === pStart.getTime()) : -1;
                //var outletBlockedDates = this.outletBlockedDates ? this.outletBlockedDates.findIndex(e => new Date(e.effectiveDate).getTime() === pStart.getTime()) : -1;

                let label = p.mealSessionDetailName;
                events.push({
                  title: label,
                  start: pStart,
                  end: pEnd,
                  meta: {
                    isDefault: true, label, obj: { label }
                  },
                  isBlocked: blockedDates > -1,
                  isOutletBlocked: outletBlockedDates > -1
                });
              }
            });

            if (day++ >= dayCount) day = 1;
          }
        }
      });
      this.calendarEvents = this.calendarEvents.concat(Array.from(events));
    });
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
    //this.openViewMenuDialog(event.meta, day);
    console.log(event);
  }

  eventClicked(event: CalendarEvent<{ menuCycle: OutletClassRosterSchedule }>): void {
    //window.open(
    //  `https://www.themoviedb.org/movie/${event.meta.film.id}`,
    //  '_blank'
    //);
  }

  isBlocked(day, events) {
    if (events && events.length > 0) {
      if (events[0].isBlocked) {
        return 'disabled-date';
      } else if (events[0].isOutletBlocked) {
        return 'outlet-disabled-date';
      }
    }
    return '';
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
  }

  loadData(id?: string) {
    this.studentService.getStudentById(id)
      .subscribe(student => {
        this.student = student;
        console.log(this.student);
        let approvedStudent = false;
        let catererStudents = [];
        //if (this.student.catererStudents && this.student.catererStudents) {
        //  catererStudents = this.student.catererStudents.filter(e => e.status == 'APPROVED');
        //  approvedStudent = catererStudents && catererStudents.length > 0;
        //  this.catererId = catererStudents[0];
        //}

        //if (approvedStudent) {

        this.getMenuGroupActiveDishCycles(id)
          .subscribe(menuGroupResults => {
            let menuGroups = menuGroupResults;
            this.blockedDates = [];
            this.outletBlockedDates = [];
            for (const i in menuGroups) {
              let menuGroup = menuGroups[i];
              if (menuGroup && menuGroup.menuGroupDishCycles && menuGroup.menuGroupDishCycles.length > 0) {
                let dishCycles = menuGroup.menuGroupDishCycles.map(function (v) { return v.dishCycle; });
                this.blockedDates = dishCycles.map(function (v) { return v.blockedDates; }).flat();
                this.outletBlockedDates = dishCycles.map(function (v) { return v.outletDishBlockedDates; }).flat();
              }
            }

            this.blockedDates = [...new Set(this.blockedDates)];
            this.outletBlockedDates = [...new Set(this.outletBlockedDates)];

            this.menuService.getStudentMenuCyclesByFilter(id, this.outletId, this.catererId)
              .subscribe(results => {
                let rosters = results;
                rosters.forEach((roster, index, menuCycles) => {
                  roster.schedules.forEach((schedule, index, menuCycles) => {
                    try {
                      (<any>schedule).index = index + 1;

                      let weekday = [];

                      //if (menuCycle.schedules) {
                      //  let dayRules = [RRule.MO, RRule.TU, RRule.WE, RRule.TH, RRule.FR, RRule.SA, RRule.SU];
                      //  menuCycle.schedules.forEach((schedule, i, s) => {
                      //    weekday.push(dayRules[(schedule.day % 7) - 1]);
                      //  });
                      //}

                      //schedule.rrule = {
                      //  freq: RRule.WEEKLY,
                      //  byweekday: [RRule.MO, RRule.TU, RRule.WE, RRule.TH, RRule.FR, RRule.SA, RRule.SU],
                      //}
                    } catch (ex) { }
                  });

                  roster.rrule = {
                    freq: RRule.WEEKLY,
                    byweekday: [RRule.MO, RRule.TU, RRule.WE, RRule.TH, RRule.FR, RRule.SA, RRule.SU],
                  }

                });

                this.rosters = rosters;
                this.eventUpdateEvent();
              },
                error => {
                  this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
                    MessageSeverity.error);
                });
          },
            error => {
              this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
                MessageSeverity.error);
            });
          
        //}
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
            MessageSeverity.error);
        });
  }

  getMenuGroupActiveDishCycles(studentId) {
    return this.menuService.getMenuGroupActiveDishCycles(studentId);
  }

  onStudentProfileChange(studentId: string) {
    this.loadData(studentId);
  }
  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.label));
  }

  get canManageMenuCycles() {
    return true;// || this.accountService.userHasPermission(Permission.manageMenuCyclesPermission)
  }

}
