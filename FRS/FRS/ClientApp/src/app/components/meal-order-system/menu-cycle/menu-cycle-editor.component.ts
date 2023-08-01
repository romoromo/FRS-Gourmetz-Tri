import { Component, ViewChild, Inject, ViewEncapsulation } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { DateAdapter, MatDatepickerInputEvent, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MenuCycle, MenuCycleSchedule, MenuCycleScheduleMenu, MenuCycleSchedulePeriod } from 'src/app/models/meal-order/menu-cycle.model';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { Dish } from 'src/app/models/meal-order/dish.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import * as moment from 'moment';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Menu } from 'src/app/models/menu.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';

@Component({
  selector: 'menu-cycle-editor',
  templateUrl: './menu-cycle-editor.component.html',
  styleUrls: ['./menu-cycle-editor.component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class MenuCycleEditorComponent {

  private isNewMenuCycle = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingMenuCycleCode: string;
  private menuCycleEdit: MenuCycle = new MenuCycle();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];
  private outletProfiles = [];
  private menus: Menu[] = [];
  start = new Date();
  end = new Date();

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public catererId: string;

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private menuService: MenuService, private accountService: AccountService,
    public dialogRef: MatDialogRef<MenuCycleEditorComponent>, private mealService: MealService, private deliveryService: DeliveryService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dateAdapter: DateAdapter<Date>  ) {
    this.dateAdapter.setLocale('en-SG');
    if (typeof (data.menuCycle) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.menuCycle.id) {
        this.editMenuCycle(data.menuCycle);
      } else {
        this.newMenuCycle();
      }
    }

    this.getMenus();
    //this.getPeriods();
    this.getOutletProfiles();
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
      this.menuCycleEdit.startDate = new Date(event.value);
    }
    if (type == 'end') {
      this.end = new Date(event.value);
      this.menuCycleEdit.endDate = new Date(event.value);
    }
  }

  addSchedule() {
    if (!this.menuCycleEdit.schedules) { this.menuCycleEdit.schedules = []; }
    let d = this.menuCycleEdit.schedules.length + 1;
    let dayPeriods: MenuCycleSchedulePeriod[] = [];
    this.periods.forEach((p, index, ps) => {
      let pd = new MenuCycleSchedulePeriod();
      pd.mealPeriodId = p.id;
      pd.menus = [];
      dayPeriods.push(pd);
    });

    let schedule = new MenuCycleSchedule();
    schedule.day = d;
    schedule.menuCycleId = this.menuCycleEdit.id;
    schedule.periods = dayPeriods;
    this.menuCycleEdit.schedules.push(schedule);
  }

  deleteSchedule(day) {
    let indx = this.menuCycleEdit.schedules.findIndex(e => e.day == day);
    if (indx > -1) {
      this.menuCycleEdit.schedules.splice(indx, 1);
      //rearrange schedules
      this.menuCycleEdit.schedules.forEach((schedule, index, ps) => {
        schedule.day = index + 1;
      });
    }
  }

  addMenu(period: MenuCycleSchedulePeriod, m: Menu) {
    let menu = new MenuCycleScheduleMenu();
    menu.mealPeriodId = period.mealPeriodId;
    menu.menuId = m.id;
    menu.label = m.label;
    if (!period.menus) period.menus = [];
    period.menus.push(menu);
  }

  removeMenu(period: MenuCycleSchedulePeriod, id: string) {
    let indx = period.menus.findIndex(f => f.menuId == id);
    if (indx > -1) {
      period.menus.splice(indx, 1);
    }
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    //let selectedDishes = this.dishes.filter(f => f.checked);
    //this.menuCycleEdit.schedules = [];
    //this.dishes.forEach((p, index, ps) => {
    //  if (p.checked) {
    //    let dtDish = new MenuCycleSchedule();
    //    dtDish.menuCycleId = this.menuCycleEdit.id;
    //    dtDish.dishId = p.id;
    //    this.menuCycleEdit.schedules.push(dtDish);
    //  }
      
    //});

    console.log(this.menuCycleEdit.schedules);
    if (this.isNewMenuCycle) {
      this.menuService.newMenuCycle(this.menuCycleEdit).subscribe(menuCycle => this.saveSuccessHelper(this.menuCycleEdit), error => this.saveFailedHelper(error));
    }
    else {
      this.menuService.updateMenuCycle(this.menuCycleEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(menuCycle?: MenuCycle) {
    if (menuCycle)
      Object.assign(this.menuCycleEdit, menuCycle);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewMenuCycle)
      this.alertService.showMessage("Success", `MenuCycle \"${this.menuCycleEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to menuCycle \"${this.menuCycleEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.menuCycleEdit = new MenuCycle();
    this.resetForm();


    //if (!this.isNewMenuCycle && this.accountService.currentUser.facilities.some(r => r == this.editingMenuCycleCode))
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
    this.menuCycleEdit = new MenuCycle();

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


  newMenuCycle() {
    this.isNewMenuCycle = true;
    this.showValidationErrors = true;

    this.editingMenuCycleCode = null;
    this.selectedValues = {};
    this.menuCycleEdit = new MenuCycle();
    this.start = new Date();
    this.end = new Date();
    this.menuCycleEdit.startDate = this.start;
    this.menuCycleEdit.endDate = this.end;
    this.menuCycleEdit.catererId = this.catererId;
    return this.menuCycleEdit;
  }

  editMenuCycle(menuCycle: MenuCycle) {
    if (menuCycle) {
      this.isNewMenuCycle = false;
      this.showValidationErrors = true;

      this.editingMenuCycleCode = menuCycle.label;
      this.selectedValues = {};
      this.menuCycleEdit = new MenuCycle();
      Object.assign(this.menuCycleEdit, menuCycle);
      this.start = this.menuCycleEdit.startDate;
      this.end = this.menuCycleEdit.endDate;
      return this.menuCycleEdit;
    }
    else {
      return this.newMenuCycle();
    }
  }

  getMenus() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.menuService.getMenusByFilter(filter)
      .subscribe(results => {
        this.menus = results.pagedData;
        //this.dishes.forEach((p, index, ps) => {
        //  (<any>p).checked = this.menuCycleEdit.schedules != null && this.menuCycleEdit.schedules.findIndex(f=> f.dishId == p.id) > -1;
        //});
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving menus.\r\n"`,
            MessageSeverity.error);
        })
  }

  getPeriods(isClearValue: boolean, outletProfileId?: string) {
    if (outletProfileId) {
      //if (isClearValue) this.menuCycleEdit.outletProfileId = '';
      let filter = new Filter();
      filter.sorts = 'sequence';
      let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
      filter.filters = f + '(IsActive)==true,(OutletProfileId)==' + outletProfileId;

      this.mealService.getMealPeriodsByFilter(filter)
        .subscribe(results => {
          this.periods = results.pagedData;
          //this.dishes.forEach((p, index, ps) => {
          //  (<any>p).checked = this.menuCycleEdit.schedules != null && this.menuCycleEdit.schedules.findIndex(f=> f.dishId == p.id) > -1;
          //});
        },
          error => {
            //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            this.alertService.showStickyMessage("Get Error", `An error occured while retrieving periods.\r\n"`,
              MessageSeverity.error);
          })
    }
  }

  getOutletProfiles() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.deliveryService.getOutletProfilesByFilter(filter)
      .subscribe(results => {
        this.outletProfiles = results.pagedData;
        this.getPeriods(false, this.menuCycleEdit.outletProfileId);
        //this.dishes.forEach((p, index, ps) => {
        //  (<any>p).checked = this.menuCycleEdit.schedules != null && this.menuCycleEdit.schedules.findIndex(f=> f.dishId == p.id) > -1;
        //});
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving outlets.\r\n"`,
            MessageSeverity.error);
        })
  }

  onOutletChange(outletProfileId?: string) {
    this.alertService.showDialog('Changing the profile will remove all the schedules. Do you wish to proceed?', DialogType.confirm, () => this.changeOutletProfile(outletProfileId));
  }

  changeOutletProfile(outletProfileId) {
    this.menuCycleEdit.schedules = [];
    this.getPeriods(true, outletProfileId);
  }

  get canManageMenuCycles() {
    return true; //this.accountService.userHasPermission(Permission.manageMenuCyclesPermission)
  }
}
