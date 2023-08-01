import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Output, EventEmitter, ChangeDetectorRef, NgZone, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from "../../services/app-translation.service";
import { AccountService } from '../../services/account.service';
import { EmsScheduleService } from '../../services/ems-schedule.service';
import { Utilities } from '../../services/utilities';
import { EmsSchedule } from '../../models/ems-schedule.model';
import { Permission } from '../../models/permission.model';
import { EmsScheduleEditorComponent } from "./ems-schedule-editor.component";

import { fadeInOut } from '../../services/animations';
import { DateOnlyPipe } from '../../pipes/datetime.pipe';
import { CalendarView, CalendarEvent, CalendarEventAction, CalendarMonthViewBeforeRenderEvent, CalendarWeekViewBeforeRenderEvent, CalendarDayViewBeforeRenderEvent } from 'angular-calendar';
import { ViewPeriod } from 'calendar-utils';
import RRule from 'rrule';
import * as moment from 'moment';

@Component({
  selector: 'ems-schedules',
  templateUrl: './ems-schedules.component.html',
  styleUrls: ['./ems-schedules.component.css'],
  animations: [fadeInOut],
  encapsulation: ViewEncapsulation.None,
})
export class EmsSchedulesComponent implements OnInit, AfterViewInit {
  schedules: EmsSchedule[] = [];
  rowsCache: EmsSchedule[] = [];
  editedEmsSchedule: EmsSchedule;
  sourceEmsSchedule: EmsSchedule;
  editingEmsScheduleName: { shortDescription: string };
  loadingIndicator: boolean;
  @Input()

  view: CalendarView = CalendarView.Week;
  viewDate: Date = new Date();
  @Output() viewChange: EventEmitter<string> = new EventEmitter();

  @Output() viewDateChange: EventEmitter<Date> = new EventEmitter();

  actions: CalendarEventAction[] = [
    {
      label: '<i class="fa fa-fw fa-pencil" style="color:white"></i>',
      //a11yLabel: 'Edit',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.editEmsSchedule(event.meta);
      },
    },
    {
      label: '<i class="fa fa-fw fa-trash" style="color:white"></i>',
      //a11yLabel: 'Delete',
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.deleteEmsSchedule(event.meta);
      },
    },
  ];

  calendarEvents: CalendarEvent<EmsSchedule>[];

  viewPeriod: ViewPeriod;

  //refresh: Subject<any> = new Subject();

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('emsScheduleEditor')
  emsScheduleEditor: EmsScheduleEditorComponent;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private emsScheduleService: EmsScheduleService, private cdr: ChangeDetectorRef, public zone: NgZone) {
  }

  handleEvent(action: string, event: CalendarEvent): void {

  }

  eventUpdateEvent() {
    this.calendarEvents.length = 0;
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
        if (date >= event.effectiveDateObj && date <= event.ineffectiveDateObj) {
          let start = new Date(date.getFullYear(), date.getMonth(), date.getDate(), event.startTimeObj.getHours(), event.startTimeObj.getMinutes(), event.startTimeObj.getSeconds());
          let end = new Date(date.getFullYear(), date.getMonth(), date.getDate(), event.endTimeObj.getHours(), event.endTimeObj.getMinutes(), event.endTimeObj.getSeconds());

          let time = "(" + event.startTime.substring(0, 5) + " - " + event.endTime.substring(0, 5) + ")"

          events.push({
            title: time + " " + label,
            color,
            start: start,
            end: end,
            actions: this.actions,
            meta: event
          });
        }
      });

      this.calendarEvents = this.calendarEvents.concat(Array.from(events));
    });
    //this.refresh.next();
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

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.loadData();
  }





  ngAfterViewInit() {

    this.emsScheduleEditor.changesSavedCallback = () => {
      this.loadData();
      this.editorModal.hide();
    };

    this.emsScheduleEditor.changesCancelledCallback = () => {
      this.editedEmsSchedule = null;
      this.sourceEmsSchedule = null;
      this.editorModal.hide();
    };
  }







  loadData() {
    console.log("SCHEDULE FILE");

    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.emsScheduleService.getEmsSchedules(null, null, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let emsSchedules = results[0];
        let locations = results[2];

        emsSchedules.forEach((emsSchedule, index, emsSchedules) => {
          try {
            (<any>emsSchedule).index = index + 1;

            let weekday = [];
            if (emsSchedule.monday) weekday.push(RRule.MO);
            if (emsSchedule.tuesday) weekday.push(RRule.TU);
            if (emsSchedule.wednesday) weekday.push(RRule.WE);
            if (emsSchedule.thursday) weekday.push(RRule.TH);
            if (emsSchedule.friday) weekday.push(RRule.FR);
            if (emsSchedule.saturday) weekday.push(RRule.SA);
            if (emsSchedule.sunday) weekday.push(RRule.SU);

            if (emsSchedule.effectiveDate) {
              emsSchedule.effectiveDateObj = new Date(emsSchedule.effectiveDate);
              emsSchedule.effectiveDate = emsSchedule.effectiveDate.split("T")[0];
            }

            if (emsSchedule.ineffectiveDate) {
              emsSchedule.ineffectiveDateObj = new Date(emsSchedule.ineffectiveDate);
              emsSchedule.ineffectiveDate = emsSchedule.ineffectiveDate.split("T")[0];
            }

            if (emsSchedule.startTime) {
              emsSchedule.startTimeObj = new Date(emsSchedule.startTime);
              emsSchedule.startTime = emsSchedule.startTime.split("T")[1];
            }

            if (emsSchedule.endTime) {
              emsSchedule.endTimeObj = new Date(emsSchedule.endTime);
              emsSchedule.endTime = emsSchedule.endTime.split("T")[1];
            }

            emsSchedule.rrule = {
              freq: RRule.WEEKLY,
              byweekday: weekday,
            }
          } catch (ex) {}
        });


        this.rowsCache = [...emsSchedules];
        this.schedules = emsSchedules;

        this.eventUpdateEvent();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve ems schedules from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.label));
  }


  onEditorModalHidden() {
    this.editingEmsScheduleName = null;
    this.emsScheduleEditor.resetForm(true);
  }


  newEmsSchedule() {
    this.editingEmsScheduleName = null;
    this.sourceEmsSchedule = null;
    this.editedEmsSchedule = this.emsScheduleEditor.newEmsSchedule();
    this.editorModal.show();
  }


  editEmsSchedule(row: EmsSchedule) {
    this.editingEmsScheduleName = { shortDescription: row.label };
    this.sourceEmsSchedule = row;
    this.editedEmsSchedule = this.emsScheduleEditor.editEmsSchedule(row);
    this.editorModal.show();
  }

  deleteEmsSchedule(row: EmsSchedule) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" ems schedule?', DialogType.confirm, () => this.deleteEmsScheduleHelper(row));
  }


  deleteEmsScheduleHelper(row: EmsSchedule) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.emsScheduleService.deleteEmsSchedule(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row);
        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the ems schedule.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageEmsSchedules() {
    return true;// || this.accountService.userHasPermission(Permission.manageEmsSchedulesPermission)
  }

}
