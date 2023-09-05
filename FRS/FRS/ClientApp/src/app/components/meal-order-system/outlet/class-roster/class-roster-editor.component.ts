import { Component, ViewChild, Inject, ViewEncapsulation } from '@angular/core';
import { DateAdapter, MatDatepickerInputEvent, MatDialogRef, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { AccountService } from 'src/app/services/account.service';
import { OutletClassRoster, OutletClassRosterSchedule, OutletClassRosterScheduleClass, OutletClassRosterSchedulePeriod } from 'src/app/models/meal-order/outlet-class-roster.model';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { MealSession } from 'src/app/models/meal-order/meal-session.model';
import { Class } from 'src/app/models/meal-order/class.model';


@Component({
  selector: 'class-roster-editor',
  templateUrl: './class-roster-editor.component.html',
  styleUrls: ['./class-roster-editor.component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class OutletClassRosterEditorComponent {

  private isNewOutletClassRoster = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingOutletClassRosterCode: string;
  private outletClassRosterEdit: OutletClassRoster = new OutletClassRoster();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  start = new Date();
  end = new Date();

  outletId: any;
  catererId: any;
  outletProfileId: any;
  private classes: Class[] = [];
  private origClasses: Class[] = [];
  private periods: MealSession[] = [];
  private mealSession: MealSession;
  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private classService: ClassService, private accountService: AccountService,
    public dialogRef: MatDialogRef<OutletClassRosterEditorComponent>, private mealService: MealService, private menuService: MenuService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dateAdapter: DateAdapter<Date>    ) {
    this.dateAdapter.setLocale('en-SG');

    if (typeof (data.mealSession) != typeof (undefined)) {
      this.outletId = data.mealSession.outletId;
      this.catererId = data.mealSession.catererId;
      this.outletProfileId = data.outletProfileId;
      this.mealSession = data.mealSession;
       
      //if (this.mealSession.details && this.mealSession.details.length > 0) {
      //  this.mealSession.details.forEach((s) => {
      //    var d = new Date(s.startDate);
      //    var curr = new Date();
      //    curr.setHours(d.getHours());
      //    curr.setMinutes(d.getMinutes());
      //    curr.setSeconds(0);
      //    curr.setMilliseconds(0);
      //    s.startDate = curr;
      //  });


      //}
      let activePs = this.mealSession.details.filter(e => e.isActive == true);
      let ps = activePs.sort((x, y) => {
        var xD = new Date(x.startDate);
        var xT = new Date();
        xT.setHours(xD.getHours());
        xT.setMinutes(xD.getHours());
        xT.setMilliseconds(0);

        var yD = new Date(y.startDate);
        var yT = new Date();
        yT.setHours(yD.getHours());
        yT.setMinutes(yD.getHours());
        yT.setMilliseconds(0);

        return xT.getTime() > (yT).getTime() ? 1 : (xT).getTime() < (yT).getTime() ? -1 : 0;
      });
      this.mealSession.details = ps;
    }

    if (data.outletClassRoster.id) {
      this.editOutletClassRoster(data.outletClassRoster);
    } else {
      this.newOutletClassRoster();
    }

    this.getClasses()
      .subscribe(results => {
        this.classes = results.pagedData;
        Object.assign(this.origClasses, results.pagedData);
        if (data.outletClassRoster.id) {
          this.editOutletClassRoster(data.outletClassRoster);
        } else {
          this.newOutletClassRoster();
        }
      },
        error => {
          if (data.outletClassRoster.id) {
            this.editOutletClassRoster(data.outletClassRoster);
          } else {
            this.newOutletClassRoster();
          }
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving classes.\r\n"`,
          //  MessageSeverity.error);
          console.log(error);
        });

    this.getPeriods()
      .subscribe(results => {
        this.periods = results.pagedData;
        //this.getOutletClassRoster(this.outletClassRosterEdit.id);
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving periods.\r\n"`,
            MessageSeverity.error);
        });
  }

  getOutletClassRoster(id) {
    //let filter = new Filter();
    //let f = this.outletProfileId ? '(OutletProfileId)==' + this.outletProfileId + ',' : '';
    //f = mealSessionId ? '(MealSessionId)==' + mealSessionId + ',' : '';
    //filter.filters = f + '(IsActive)==true';
    this.classService.getClassRosterById(id)
      .subscribe(results => {
        //let rosters = results.pagedData;
        this.outletClassRosterEdit = results;
        if (!this.outletClassRosterEdit) {
          this.outletClassRosterEdit = new OutletClassRoster();
          this.outletClassRosterEdit.mealSessionId = this.mealSession.id;
          this.outletClassRosterEdit.outletProfileId = this.outletProfileId;
          this.isNewOutletClassRoster = true;
          
          this.outletClassRosterEdit.startDate = this.start;
          this.outletClassRosterEdit.endDate = this.end;
          this.outletClassRosterEdit.startDate = new Date();
          this.outletClassRosterEdit.endDate = new Date();
        }

        this.start = this.outletClassRosterEdit.startDate;
        this.end = this.outletClassRosterEdit.endDate;

        let schedules = [];
        this.outletClassRosterEdit.schedules.forEach((s) => {
          let schedule = new OutletClassRosterSchedule();
          let periods = [];
          //this.mealSession.details.sort((a, b) => a.name.localeCompare(b.name));

          this.mealSession.details.forEach((p) => {
            let period = {};

            //let details = p.details.map((e) => { return e.id });
            const filteredPeriods = s.periods.filter(value => p.id == value.mealSessionDetailId);

            if (filteredPeriods && filteredPeriods.length > 0) {
              periods.push(filteredPeriods[0]);
            }
          });
          if (periods && periods.length > 0) {
            periods.sort((a, b) => a.name.localeCompare(b.name));
            schedule.periods = periods;
            schedule.outletClassRosterId = this.outletClassRosterEdit.id;
            schedule.id = s.id;
            schedule.day = s.day;
            schedules.push(schedule);
          }
          
        });

        this.outletClassRosterEdit.schedules = schedules;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving roster.\r\n"`,
            MessageSeverity.error);
        })
  }

  getClasses() {
    let filter = new Filter();
    let f = this.outletId ? '(classOutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    return this.classService.getClassesByFilter(filter);
  }

  getPeriods() {
    let filter = new Filter();
    filter.sorts = 'sequence';
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';

    return this.mealService.getMealSessionsByFilter(filter);
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
      this.outletClassRosterEdit.startDate = new Date(event.value);
    }
    if (type == 'end') {
      this.end = new Date(event.value);
      this.outletClassRosterEdit.endDate = new Date(event.value);
    }
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewOutletClassRoster) {
      this.classService.newClassRoster(this.outletClassRosterEdit).subscribe(outletClassRoster => this.saveSuccessHelper(outletClassRoster), error => this.saveFailedHelper(error));
    }
    else {
      this.classService.updateClassRoster(this.outletClassRosterEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }

    this.alertService.stopLoadingMessage();
  }


  private saveSuccessHelper(outletClassRoster?: OutletClassRoster) {
    if (outletClassRoster)
      Object.assign(this.outletClassRosterEdit, outletClassRoster);


    // Only uncomment this when the change is okay to pu
    //console.log("success saving now update order Session")

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewOutletClassRoster)
      this.alertService.showMessage("Success", `Class Roster was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to roster was saved successfully`, MessageSeverity.success);


    this.outletClassRosterEdit = new OutletClassRoster();
    this.resetForm();


    //if (!this.isNewOutletClassRoster && this.accountService.currentUser.facilities.some(r => r == this.editingOutletClassRosterCode))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }


  private cancel() {
    this.outletClassRosterEdit = new OutletClassRoster();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }


  newOutletClassRoster() {
    this.isNewOutletClassRoster = true;
    this.showValidationErrors = true;

    this.editingOutletClassRosterCode = null;
    this.selectedValues = {};
    this.outletClassRosterEdit = new OutletClassRoster();
    this.start = new Date();
    this.end = new Date();
    this.outletClassRosterEdit.startDate = this.start;
    this.outletClassRosterEdit.endDate = this.end;
    this.outletClassRosterEdit.catererId = this.catererId;
    this.outletClassRosterEdit.outletId = this.outletId;
    this.outletClassRosterEdit.mealSessionId = this.mealSession.id;
    this.outletClassRosterEdit.outletProfileId = this.outletProfileId;
    //this.setSchedules();
    return this.outletClassRosterEdit;
  }

  editOutletClassRoster(outletClassRoster: OutletClassRoster) {
    if (outletClassRoster) {
      this.isNewOutletClassRoster = false;
      this.showValidationErrors = true;

      //this.editingOutletClassRosterCode = outletClassRoster.name;
      this.selectedValues = {};
      this.outletClassRosterEdit = new OutletClassRoster();
      Object.assign(this.outletClassRosterEdit, outletClassRoster);
      this.start = this.outletClassRosterEdit.startDate;
      this.end = this.outletClassRosterEdit.endDate;

      this.outletClassRosterEdit.schedules.forEach((s) => {
        //let ps = s.periods.sort((x, y) => x.mealSessionDetailStartDate > y.mealSessionDetailStartDate ? 1 : x.mealSessionDetailStartDate < y.mealSessionDetailStartDate ? -1 : 0);
        let periods = [];
        this.mealSession.details.forEach((p) => {
          let period = {};

          //let details = p.details.map((e) => { return e.id });
          const filteredPeriods = s.periods.filter(value => p.id == value.mealSessionDetailId);

          if (filteredPeriods && filteredPeriods.length > 0) {
            periods.push(filteredPeriods[0]);
          } else {
            let newPeriod = new OutletClassRosterSchedulePeriod();
            newPeriod.outletClassRosterScheduleId = s.id;
            newPeriod.classes = [];
            newPeriod.mealSessionDetailId = p.id;
            periods.push(newPeriod);
          }
        });

        s.periods = periods;
        s.classSelections = [];
        Object.assign(s.classSelections, this.classes);
      });

      
      //this.setSchedules();
      return this.outletClassRosterEdit;
    }
    else {
      return this.newOutletClassRoster();
    }
  }

  setSchedules() {
    this.start = this.outletClassRosterEdit.startDate;
    this.end = this.outletClassRosterEdit.endDate;

    let schedules = [];
    this.outletClassRosterEdit.schedules.forEach((s) => {
      let schedule = new OutletClassRosterSchedule();
      let periods = [];
      this.mealSession.details.forEach((p) => {
        let period = {};

        //let details = p.details.map((e) => { return e.id });
        const filteredPeriods = s.periods.filter(value => p.id == value.mealSessionDetailId);

        if (filteredPeriods && filteredPeriods.length > 0) {
          periods.push(filteredPeriods[0]);
        }
      });
      if (periods && periods.length > 0) {
        schedule.periods = periods;
        schedule.outletClassRosterId = this.outletClassRosterEdit.id;
        schedule.id = s.id;
        schedule.day = s.day;
        schedules.push(schedule);
      }

    });

    this.outletClassRosterEdit.schedules = schedules;
  }

  addSchedule() {
    if (!this.outletClassRosterEdit.schedules) { this.outletClassRosterEdit.schedules = []; }
    let d = this.outletClassRosterEdit.schedules.length + 1;
    let dayPeriods: OutletClassRosterSchedulePeriod[] = [];
    this.mealSession.details.forEach((p, index, ps) => {
      let pd = new OutletClassRosterSchedulePeriod();
      pd.mealSessionDetailId = p.id;
      pd.classes = [];
      dayPeriods.push(pd);
    });

    let schedule = new OutletClassRosterSchedule();
    schedule.day = d;
    schedule.outletClassRosterId = this.outletClassRosterEdit.id;
    schedule.periods = dayPeriods;
    schedule.classSelections = [];
    Object.assign(schedule.classSelections, this.classes);
    this.outletClassRosterEdit.schedules.push(schedule);
    
  }

  deleteSchedule(day) {
    let indx = this.outletClassRosterEdit.schedules.findIndex(e => e.day == day);
    if (indx > -1) {
      this.outletClassRosterEdit.schedules.splice(indx, 1);
      //rearrange schedules
      this.outletClassRosterEdit.schedules.forEach((schedule, index, ps) => {
        schedule.day = index + 1;
      });
    }
  }

  addClass(schedule: OutletClassRosterSchedule, period: OutletClassRosterSchedulePeriod, m: Class) {
    let c = new OutletClassRosterScheduleClass();
    c.mealSessionDetailId = period.mealSessionDetailId;
    c.classId = m.id;
    c.name = m.name;
    c.outletClassRosterSchedulePeriodId = period.id;
    if (!period.classes) period.classes = [];

    let indx = period.classes.findIndex(e => e.outletClassRosterSchedulePeriodId == period.id && e.classId == m.id);
    if (indx < 0) {
      period.classes.push(c);
    }

    //remove class from classes
    let indxToDel = schedule.classSelections.findIndex(e => e.id == m.id);
    schedule.classSelections.splice(indxToDel, 1);
  }

  removeClass(schedule: OutletClassRosterSchedule, period: OutletClassRosterSchedulePeriod, id: string) {
    let indx = period.classes.findIndex(f => f.classId == id);
    if (indx > -1) {
      period.classes.splice(indx, 1);
    }

    //add class to classes
    let indxToIns = this.origClasses.findIndex(e => e.id == id);
    if (indxToIns > -1) {
      schedule.classSelections.splice(indxToIns, 0, this.origClasses[indxToIns]);
    }
  }

  filterClasses(schedule) {
    let filteredClass = [];
    let addedClasses = [];
    for (const c of schedule.classSelections) {
      let exist = false;
      for (const s of schedule.periods) {
        for (const sc of s.classes) {
          if (c.id == sc.classId) {
            exist = true;
            break;
          }
        }

        if (exist) break;
      }

      if (!exist) {
        let indx = filteredClass.findIndex(e => e.id == c.id);
        if(indx < 0)
          filteredClass.push(c);
      }
    }

    return filteredClass;
  }

  get canManageOutletClassRosters() {
    return true; //this.accountService.userHasPermission(Permission.manageOutletClassRostersPermission)
  }
}
