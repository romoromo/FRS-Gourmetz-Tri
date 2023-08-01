import { Component, ViewChild, Inject } from '@angular/core';

import { DateAdapter, MatDatepickerInputEvent, MatDialog, MatDialogRef, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MAT_DIALOG_DATA } from '@angular/material';
import { MealSession, MealSessionDetail, MealSessionMealPeriod } from 'src/app/models/meal-order/meal-session.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { Outlet } from 'src/app/models/meal-order/outlet.model';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { Utilities } from 'src/app/services/utilities';
import * as moment from 'moment';
import { OutletClassRosterEditorComponent } from '../class-roster/class-roster-editor.component';
import { OutletClassRostersManagementComponent } from '../class-roster/class-rosters-management.component';

@Component({
  selector: 'meal-session-editor',
  templateUrl: './meal-session-editor.component.html',
  styleUrls: ['./meal-session-editor.component.css'],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class MealSessionEditorComponent {

  private isNewMealSession = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingMealSessionCode: string;
  private mealSessionEdit: MealSession = new MealSession();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  start = new Date();
  route = new Date();
  overheads = new Date();
  calsource = new Date();
  outletProfiles = [];
  routes = [];
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  private outlet: Outlet;
  private outletProfileId: string;
  private catererId: string;
  private outletId: string;
  private mealPeriodSessions: MealSessionMealPeriod[];
  private origMealPeriodSessions: MealSessionMealPeriod[];
  private hasPending = false;
  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private mealService: MealService, private accountService: AccountService,
    public dialogRef: MatDialogRef<MealSessionEditorComponent>, private deliveryService: DeliveryService, private menuService: MenuService,
    @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog) {
    this.hasPending = false;
    if (typeof (data.outlet) != typeof (undefined)) {
      this.outlet = data.outlet;
      this.outletProfileId = data.outletProfileId;
      this.catererId = data.catererId;
      this.outletId = data.outletId;
      
      //if (!this.outlet.mealSessions) {
        //this.editMealSessions(data.outlet);
        this.getMealPeriodSessions();
      //} 
    }

    this.getRoutes();
  }

  getMealSessions() {
    let filter = new Filter();
    filter.sorts = 'startDate';
    filter.filters = '(IsActive)==true,(OutletProfileId)==' + this.outletProfileId;
    this.mealService.getMealPeriodsByFilter(filter)
      .subscribe(results => {
        let mealPeriods = results.pagedData;
        if (!this.outlet.mealSessions) {
          this.outlet.mealSessions = [];
        }

        let newMealSessions: MealSession[] = [];
        mealPeriods.forEach((mp: MealPeriod, index, periods) => {
          let mealSession = new MealSession();
          let mealSessions = this.outlet.mealSessions.filter(e => e.mealPeriodId == mp.id);
          if (mealSessions && mealSessions.length > 0) {
            mealSession = mealSessions[0];
          } else {
            mealSession.mealPeriodId = mp.id;
            mealSession.mealPeriodName = mp.name;
            mealSession.outletId = this.outletId;
            mealSession.sequence = mp.sequence;
            mealSession.name = mp.name;
            mealSession.startDate = mp.startDate;
            mealSession.endDate = mp.endDate;
            mealSession.catererId = this.catererId;
          }
          if (mealSession.details) {
            mealSession.details.forEach((det: MealSessionDetail, i, dets) => {
              det.id = '0';
            });
          } else {
            mealSession.details = [];
          }
          
          //mealSession.id = '';
          mealSession.name = mp.name;
          mealSession.startDate = mp.startDate;
          mealSession.endDate = mp.endDate;

          newMealSessions.push(mealSession);
        });

        this.outlet.mealSessions = newMealSessions;

      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving sessions.\r\n"`,
            MessageSeverity.error);
        })
  }

  getMealPeriodSessions() {
    this.mealService.getAllMealPeriodSessions(this.outletId, this.catererId, this.outletProfileId)
      .subscribe(results => {
        this.mealPeriodSessions = results;
        this.origMealPeriodSessions = results;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving sessions.\r\n"`,
            MessageSeverity.error);
        })
  }

  getRoutes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getRoutesByFilter(filter)
      .subscribe(results => {
        this.routes = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving routes.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  errors: Set<string> = new Set<string>();
  private save() {
    if (this.form.valid) {
      console.log(this.form);
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");
      console.log(this.outlet);

      let sessions: MealSession[] = [];
      this.mealPeriodSessions.forEach((e) => {
        sessions.push(...e.mealSessions);
      });
      
      this.mealService.bulkMealSession(sessions).subscribe(mealSession => this.saveSuccessHelper(mealSession), error => this.saveFailedHelper(error));
    } else {
      this.alertService.showMessage("Validation Error", "Please enter correct values.", MessageSeverity.error);
    }
    
  }


  private saveSuccessHelper(mealSession?: MealSession) {
    if (mealSession)
      Object.assign(this.mealSessionEdit, mealSession);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    this.alertService.showMessage("Success", `Sessions were updated successfully`, MessageSeverity.success);
    this.hasPending = false;

    this.mealSessionEdit = new MealSession();
    this.resetForm();


    //if (!this.isNewMealSession && this.accountService.currentUser.facilities.some(r => r == this.editingMealSessionCode))
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
    this.mealSessionEdit = new MealSession();

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

  onChangeDate(type: string, detail: MealSessionDetail, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
      detail.startDate = new Date(event.value);
    }
    if (type == 'route') {
      this.route = new Date(event.value);
      detail.routeTime = new Date(event.value);
    }
    if (type == 'overheads') {
      this.overheads = new Date(event.value);
      detail.overheadTime = new Date(event.value);
    }
    if (type == 'calsource') {
      this.calsource = new Date(event.value);
      detail.calSourceTime = new Date(event.value);
    }
  }

  addMealSession(mealSession) {
    let newMealSessionObj = new MealSessionDetail();
    if (!mealSession.details) {
      mealSession.details = [];
    }
    newMealSessionObj.mealSessionId = mealSession.mealPeriodId;
    newMealSessionObj.startDate = mealSession.startDate;
    newMealSessionObj.routeInterval = 0;
    newMealSessionObj.overheadInterval = 0;
    newMealSessionObj.isActive = true;
    mealSession.details.push(newMealSessionObj)
    
  }

  deleteMealSession(mealSession, index) {
    if (mealSession.details[index].id > 0) {
      this.hasPending = true;
    }

    mealSession.details.splice(index, 1);
  }

  validateStart(mealSession, time, a, id, i, detail) {
    if (mealSession && time) {
      console.log(mealSession);
      console.log(time);
      let ctrl = 'detail-start-' + a + id + i;
      let currentDate = new Date();
      currentDate.setHours(time.split(":")[0]);
      currentDate.setMinutes(time.split(":")[1]);
      currentDate.setSeconds(0);

      let start = new Date(mealSession.startDate);
      let end = new Date(mealSession.endDate);


      let startTime = new Date();
      startTime.setHours(start.getHours());
      startTime.setMinutes(start.getMinutes());
      startTime.setSeconds(0);
      let endTime = new Date();
      endTime.setHours(end.getHours());
      endTime.setMinutes(end.getMinutes());
      endTime.setSeconds(0);

      let sTime = startTime.getTime();
      let eTime = endTime.getTime();
      let cTime = currentDate.getTime();

      let valid = sTime <= cTime && eTime >= cTime;
      if (valid) {
        this.form.controls[ctrl].setErrors(null);
        this.errors[ctrl] = false;
        detail.startDate = currentDate.toLocaleString();// moment(currentDate).format('DD/MM/YYYY HH:mm');;
      } else {
        this.form.controls[ctrl].setErrors({ 'incorrect': true });
        this.errors[ctrl] = true;
      }
    }
    
  }

  isValid(id, a, i) {
    return this.errors["detail-start-" + a + id + i] == true;
  }

  validateCalSource(mealSession, time, a, id, i, detail) {
    if (mealSession && time) {
      let ctrl = 'detail-calsource-' + a + id + i;
      let currentDate = new Date();
      currentDate.setHours(time.split(":")[0]);
      currentDate.setMinutes(time.split(":")[1]);
      currentDate.setSeconds(0);

      let start = new Date(mealSession.startDate);
      let startTime = new Date();
      startTime.setHours(start.getHours());
      startTime.setMinutes(start.getMinutes());
      startTime.setSeconds(0);

      let sTime = startTime.getTime();
      let cTime = currentDate.getTime();

      let valid = sTime > cTime;
      if (valid) {
        this.form.controls[ctrl].setErrors(null);
        this.errors[ctrl] = false;
        detail.calSourceTime = currentDate.toLocaleString();//moment(currentDate).format('DD/MM/YYYY HH:mm');;
      } else {
        this.form.controls[ctrl].setErrors({ 'incorrect': true });
        this.errors[ctrl] = true;
      }
    }
  }

  calculateCalSource(det: MealSessionDetail, id, i) {
    let d = new Date(det.startDate);
    d.setMinutes(d.getMinutes() - det.routeInterval - det.overheadInterval);
    return Utilities.printTimeOnly(d, false); 
  }
  //isValidCalSource(a, id, i) {
  //  return this.errors["detail-calsource-" + a + id + i] == true;
  //}

  addClassRoster(mealSession: MealSession): void {
    const dialogRef = this.dialog.open(OutletClassRosterEditorComponent, {
      data: { header: mealSession.name + ' - Class Detailing', mealSession: mealSession, catererId: this.catererId, outletId: this.outletId, outletProfileId: this.outletProfileId },
      width: '900px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      
    });
  }

  openClassRoster(mealSession: MealSession): void {

    var unsavedMs = (mealSession.details && mealSession.details.findIndex(e => !e.id) > -1) ||
                      (this.hasPending);

    if (unsavedMs) {
      this.alertService.showDialog('There are unsaved meal sessions. Please save it first.');
      return;
    }

    let mealSessionCopy = {};
    Object.assign(mealSessionCopy, mealSession);
    const dialogRef = this.dialog.open(OutletClassRostersManagementComponent, {
      data: { header: mealSession.name + ' - Class Detailing', mealSession: mealSessionCopy, catererId: this.catererId, outletId: this.outletId, outletProfileId: this.outletProfileId },
      width: '900px'
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }

  get canManageMealSessions() {
    return true; //this.accountService.userHasPermission(Permission.manageMealSessionsPermission)
  }
}
