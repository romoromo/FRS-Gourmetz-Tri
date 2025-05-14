import { Component, ViewChild, Inject, ViewEncapsulation, LOCALE_ID, OnInit, OnDestroy } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { DateAdapter, MatDatepickerInputEvent, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Filter } from 'src/app/models/sieve-filter.model';
import * as moment from 'moment';
import { Subscription, Subject } from 'rxjs';
import { takeUntil, switchMap } from 'rxjs/operators';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { DishCycle, DishCyclePeriod, DishCycleSchedule, DishCycleScheduleDetail, DishCycleScheduleDetailMenu, DishCycleScheduleSet } from 'src/app/models/meal-order/dish-cycle';
import { Dish } from 'src/app/models/meal-order/dish.model';
import { DishType } from 'src/app/models/meal-order/dish-type.model';
import { MealType } from 'src/app/models/meal-order/meal-type.model';
import { FormControl } from '@angular/forms';

export const CUSTOM_DATE_FORMAT = {
  parse: {
    dateInput: 'DD/MM/YYYY',
  },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MMMM YYYY',
    dateA11yLabel: 'DD/MM/YYYY',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};


@Component({
  selector: 'dish-cycle-editor',
  templateUrl: './dish-cycle-editor.component.html',
  styleUrls: ['./dish-cycle-editor.component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    
    { provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMAT },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class DishCycleEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private cancelPreviousRequests = new Subject<void>();
  private isNewDishCycle = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingDishCycleCode: string;
  private dishCycleEdit: DishCycle = new DishCycle();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private periods: MealPeriod[] = [];
  private outletProfiles = [];
  private dishes: Dish[] = [];
  private dishTypes: DishType[] = [];
  private cycleTypes = ['Ala Carte', 'Main Menu', 'Snacks', 'Light Meal', 'Full Meal', 'Premium Full Meal'];
  private mealCycleTypes = ['Snacks', 'Light Meal', 'Full Meal', 'Premium Full Meal'];
  private dishCycles: DishCycle[] = [];
  private mealTypes: MealType[] = [];
  public searchForm: FormControl = new FormControl();
  start = new Date();
  end = new Date();
  isLoadingDishCycles = false;
  private sets = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public catererId: string;

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private dishService: DishService, private accountService: AccountService,
    public dialogRef: MatDialogRef<DishCycleEditorComponent>, private mealService: MealService, private deliveryService: DeliveryService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dateAdapter: DateAdapter<Date>  ) {
    this.dateAdapter.setLocale('en-SG');
    if (typeof (data.dishCycle) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.dishCycle.id) {
        this.getDishCycleById(data.dishCycle.id);
        //this.editDishCycle(data.dishCycle);
        //this.generateScheduleDetails();
      } else {
        this.newDishCycle();
      }
    }
    this.getDishTypes();
    //this.getPeriods();
    console.log(this.dishCycleEdit);
    //if (this.dishCycleEdit.outletProfileId) {
    //  this.getDishCyles();
    //  this.getMealTypes();
    //} else {
    //  this.initiliaseSet();
    //}
    //this.getDishCyles();
    //this.getMealTypes();
    this.getOutletProfiles();
    
    //this.getMealPeriods();
  }

  ngOnInit() {
  }

  ngOnDestroy(): void {
    this.cancelPreviousRequests.next();
    this.subscription.unsubscribe();
  }

  getDishCycleById(id) {
    this.dishService.getDishCycleById(id)
      .subscribe(results => {
        this.editDishCycle(results);
        this.generateScheduleDetails();
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.stopLoadingMessage();

          console.log(error);
          //this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
          //  MessageSeverity.error);
        });
  }
  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
      this.dishCycleEdit.startDate = new Date(event.value);
    }
    if (type == 'end') {
      this.end = new Date(event.value);
      this.dishCycleEdit.endDate = new Date(event.value);
    }
  }

  addSchedule() {
    //if (!this.dishCycleEdit.schedules) { this.dishCycleEdit.schedules = []; }
    //let d = this.dishCycleEdit.schedules.length + 1;
    //let de: DishCycleScheduleDetail[] = [];
    //this.periods.forEach((p, index, ps) => {
    //  let pd = new DishCycleSchedulePeriod();
    //  pd.mealPeriodId = p.id;
    //  pd.dishes = [];
    //  dayPeriods.push(pd);
    //});

    //let schedule = new DishCycleSchedule();
    //schedule.day = d;
    //schedule.dishCycleId = this.dishCycleEdit.id;
    //schedule.periods = dayPeriods;
    //this.dishCycleEdit.schedules.push(schedule);
  }

  deleteSchedule(day) {
    let indx = this.dishCycleEdit.schedules.findIndex(e => e.day == day);
    if (indx > -1) {
      this.dishCycleEdit.schedules.splice(indx, 1);
      //rearrange schedules
      this.dishCycleEdit.schedules.forEach((schedule, index, ps) => {
        schedule.day = index + 1;
      });
    }
  }

  addDish(detail: DishCycleScheduleDetail, m: any, schedule: DishCycleSchedule, cycle: DishCycle, isMeal: boolean) {
    let dish = new DishCycleScheduleDetailMenu();
    if (isMeal) {
      dish.dishCycleScheduleDetailId = detail.id;
      dish.dishId = m.dishId;
      dish.dishLabel = m.dishLabel;
      dish.dishCode = m.dishCode;
      if (cycle) {
        dish.dishCycleId = cycle.id;
        dish.dishCycleLabel = cycle.label;
      }
      
    } else {
      dish.dishCycleScheduleDetailId = detail.id;
      dish.dishId = m.id;
      dish.dishLabel = m.label;
      dish.dishCode = m.code;
    }

    if (!detail.menus) detail.menus = [];
    detail.menus.push(dish);

    detail.selectedDish = null;
    detail.selectedCycle = null;
  }

  removeDish(detail: DishCycleScheduleDetail, id: string, isMeal: boolean) {
    let indx = isMeal ? detail.menus.findIndex(f => f.dishCycleId == id) : detail.menus.findIndex(f => f.dishId == id);
    if (indx > -1) {
      detail.menus.splice(indx, 1);
    }
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.renameSetLabels();
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    let selectedMealPeriods = this.periods.filter(f => f.checked);
    this.dishCycleEdit.dishCyclePeriods = [];
    this.periods.forEach((p, index, ps) => {
      if (p.checked) {
        let period = new DishCyclePeriod();
        period.dishCycleId = this.dishCycleEdit.id;
        period.mealPeriodId = p.id;
        this.dishCycleEdit.dishCyclePeriods.push(period);
      }

    });


    console.log(this.dishCycleEdit.schedules);
    if (this.isNewDishCycle) {
      this.subscription.add(this.dishService.newDishCycle(this.dishCycleEdit).subscribe(dishCycle => this.saveSuccessHelper(this.dishCycleEdit), error => this.saveFailedHelper(error)));
    }
    else {
      this.subscription.add(this.dishService.updateDishCycle(this.dishCycleEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error)));
    }
  }


  private saveSuccessHelper(dishCycle?: DishCycle) {
    if (dishCycle)
      Object.assign(this.dishCycleEdit, dishCycle);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDishCycle)
      this.alertService.showMessage("Success", `DishCycle \"${this.dishCycleEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to dishCycle \"${this.dishCycleEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.dishCycleEdit = new DishCycle();
    this.resetForm();


    //if (!this.isNewDishCycle && this.accountService.currentUser.facilities.some(r => r == this.editingDishCycleCode))
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
    this.dishCycleEdit = new DishCycle();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close(true);
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


  newDishCycle() {
    this.isNewDishCycle = true;
    this.showValidationErrors = true;

    this.editingDishCycleCode = null;
    this.selectedValues = {};
    this.dishCycleEdit = new DishCycle();
    this.start = new Date();
    this.end = new Date();
    this.dishCycleEdit.startDate = this.start;
    this.dishCycleEdit.endDate = this.end;
    this.dishCycleEdit.catererId = this.catererId;
    return this.dishCycleEdit;
  }

  editDishCycle(dishCycle: DishCycle) {
    if (dishCycle) {
      this.isNewDishCycle = false;
      this.showValidationErrors = true;

      this.editingDishCycleCode = dishCycle.label;
      this.selectedValues = {};
      this.dishCycleEdit = new DishCycle();
      Object.assign(this.dishCycleEdit, dishCycle);
      this.start = this.dishCycleEdit.startDate;
      this.end = this.dishCycleEdit.endDate;

      if (this.dishCycleEdit.outletProfileId) {
        this.getDishCyles();
        this.getMealTypes();
        this.getPeriods(true);
        this.getDishes();
      } else {
        this.initiliaseSet();
      }

      //this.getOutletProfiles();
      return this.dishCycleEdit;
    }
    else {
      return this.newDishCycle();
    }

    
  }

  getDishTypes() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.subscription.add(this.dishService.getDishTypesByFilter(filter)
      .subscribe(results => {
        this.dishTypes = results.pagedData;
        this.getDishes();
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
          //  MessageSeverity.error);
        }));
  }

  getDishes() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    f = f + (this.dishCycleEdit.dishTypeId ? '(DishTypeId)==' + this.dishCycleEdit.dishTypeId + ',' : '');
    filter.filters = f + '(IsActive)==true';
    this.subscription.add(this.dishService.getDishesByFilter(filter)
      .subscribe(results => {
        this.dishes = results.pagedData;
        //this.dishes.forEach((p, index, ps) => {
        //  (<any>p).checked = this.dishCycleEdit.schedules != null && this.dishCycleEdit.schedules.findIndex(f=> f.dishId == p.id) > -1;
        //});
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving dishes.\r\n"`,
            MessageSeverity.error);
        }))
  }

  getPeriods(isClearValue: boolean, outletProfileId?: string) {
    if (outletProfileId || this.dishCycleEdit.outletProfileId) {
      //if (isClearValue) this.dishCycleEdit.outletProfileId = '';
      let filter = new Filter();
      filter.sorts = 'sequence';
      let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
      filter.filters = f + '(IsActive)==true,(OutletProfileId)==' + this.dishCycleEdit.outletProfileId;

      this.subscription.add(this.mealService.getMealPeriodsByFilter(filter)
        .subscribe(results => {
          this.periods = results.pagedData;
          this.periods.forEach((p, index, ps) => {
            (<any>p).checked = this.dishCycleEdit.dishCyclePeriods != null && this.dishCycleEdit.dishCyclePeriods.findIndex(f => f.mealPeriodId == p.id) > -1;
          });
        },
          error => {
            //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving periods.\r\n"`,
            //  MessageSeverity.error);
          }))
    } else {

    }
  }

  getOutletProfiles() {
    let filter = new Filter();
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.subscription.add(this.deliveryService.getOutletProfilesByFilter(filter)
      .subscribe(results => {
        this.outletProfiles = results.pagedData;
        //this.getPeriods(false, this.dishCycleEdit.outletProfileId);
        //this.dishes.forEach((p, index, ps) => {
        //  (<any>p).checked = this.dishCycleEdit.schedules != null && this.dishCycleEdit.schedules.findIndex(f=> f.dishId == p.id) > -1;
        //});
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving outlets.\r\n"`,
            MessageSeverity.error);
        }))
  }

  //getMealPeriods() {
  //  let filter = new Filter();
  //  filter.filters = '(IsActive)==true';
  //  this.mealService.getMealPeriodsByFilter(filter)
  //    .subscribe(results => {
  //      this.periods = results.pagedData;
  //      this.periods.forEach((p, index, ps) => {
  //        (<any>p).checked = this.dishCycleEdit.dishCyclePeriods != null && this.dishCycleEdit.dishCyclePeriods.findIndex(f => f.mealPeriodId == p.id) > -1;
  //      });
  //    },
  //      error => {
  //        //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
  //        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving cperiods.\r\n"`,
  //          MessageSeverity.error);
  //      })
  //}

  getDishCyles() {
    this.cancelPreviousRequests.next();
    this.isLoadingDishCycles = true;
    let filter = new Filter();
    filter.sorts = 'label';
    filter.filters = '(IsActive)==true,(OutletProfileId)==' + this.dishCycleEdit.outletProfileId;

    this.dishService.getDishCyclesByFilter(filter)
      .pipe(
        takeUntil(this.cancelPreviousRequests),
        switchMap(results => {

          this.dishCycles = results.pagedData;
          this.initiliaseSet();
          return [];
        })
      )
      .subscribe(
        () => { this.isLoadingDishCycles = false; },
        error => {
          this.isLoadingDishCycles = false;
          this.alertService.stopLoadingMessage();
          this.alertService.showStickyMessage("Get Error", `An error occurred while retrieving records.\r\n"`, MessageSeverity.error);
        }
    );
  }

  getMealTypes() {
    console.log('getMealTypes');
    let filter = new Filter();
    filter.sorts = 'name';
    //let f = this.menuCycle.catererId ? '(CatererId)==' + this.menuCycle.catererId + ',' : '';
    filter.filters = '(MealTypeByOutletProfileId)==' + this.dishCycleEdit.outletProfileId + ',(IsActive)==true';
    //filter.filters = f + '(IsActive)==true';
    this.subscription.add(this.mealService.getMealTypesByFilter(filter).subscribe(results => {
      this.mealTypes = results.pagedData;
    },
      error => {
        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
          MessageSeverity.error);
      }))
  }

  getMealTypesByType(type) {
    return this.mealTypes ? this.mealTypes : [];
  }

  dishTypeChange(dishTypeId) {
    this.dishes = [];
    if (dishTypeId) {
      this.getDishes();
      this.generateSchedule(true);
    }
  }

  renameSetLabels() {
    if (this.dishCycleEdit.sets && this.dishCycleEdit.sets.length > 0 && !this.isMeal()) {
      this.dishCycleEdit.sets.forEach((set: DishCycleScheduleSet, i, arr) => {
        set.label = this.dishCycleEdit.label;
      });
    }
  }

  initiliaseSet() {
    if (!this.dishCycleEdit.sets || (this.dishCycleEdit.sets.length != this.dishCycleEdit.numOfSets)) {
      let sets = [];
      if (!this.dishCycleEdit.sets) this.dishCycleEdit.sets = [];
      for (var i = 1; i <= this.dishCycleEdit.numOfSets; i++) {
        var s = this.dishCycleEdit.sets ? this.dishCycleEdit.sets.filter(e => e.sequence == i) : [];
        if (s && s.length > 0) {
          sets.push(s[0]);
        } else {
          sets.push(new DishCycleScheduleSet(this.dishCycleEdit.id, i, this.dishCycleEdit.label));
        }
      }

      this.dishCycleEdit.sets = sets;
    } else {
      for (var x = 0; x <= this.dishCycleEdit.sets.length; x++) {
        if (this.dishCycleEdit.sets[x]) {
          this.getAlaCarteCols(this.dishCycleEdit.sets[x].cycleTypeId, this.dishCycleEdit.sets[x]);
          //if (this.dishCycleEdit.cycleType == 'Main Menu') {
            this.getDishesByCycle(this.dishCycleEdit.sets[x].cycleTypeId, this.dishCycleEdit.sets[x].cycleTypeSequence, this.dishCycleEdit.sets[x].sequence, false);
          //} else {
          //  this.getAlaCarteDishes(this.dishCycleEdit.sets[x].cycleTypeId, this.dishCycleEdit.sets[x].cycleTypeSequence, this.dishCycleEdit.sets[x].sequence, false);
          //}
          
        }
      }
    }
  }

  generateSchedule(clearMenu? : boolean) {
    if (this.dishCycleEdit.id) {
      this.alertService.showDialog('Changing this field may affect existing data. Do you wish to proceed?', DialogType.confirm, () => this.generateScheduleDetails(clearMenu));
    } else {
      this.generateScheduleDetails(clearMenu);
    }
  }

  generateScheduleDetails(clearMenu?: boolean) {
    this.getDishCyles();
    if (this.dishCycleEdit.numOfDays > 0 && this.dishCycleEdit.numOfSets > 0) {
      if (!this.dishCycleEdit.sets || this.dishCycleEdit.sets.length == 0 || this.dishCycleEdit.sets.length != this.dishCycleEdit.numOfSets) {
        let sets = [];
        for (var i = 1; i <= this.dishCycleEdit.numOfSets; i++) {
          var s = this.dishCycleEdit.sets ? this.dishCycleEdit.sets.filter(e => e.sequence == i) : [];
          if (s && s.length > 0) {
            sets.push(s[0]);
          } else {
            sets.push(new DishCycleScheduleSet(this.dishCycleEdit.id, i, this.dishCycleEdit.label));
          }
        }

        this.dishCycleEdit.sets = sets;
      }
      

      let schedules = [];
      for (var i = 1; i <= this.dishCycleEdit.numOfDays; i++) {
        let schedule = new DishCycleSchedule();
        
        let sched = this.dishCycleEdit.schedules.filter(e => e.day == i);
        if (sched && sched.length > 0) {
          Object.assign(schedule, sched[0]);
        } else {
          schedule.day = i;
          schedule.dishCycleId = this.dishCycleEdit.id;
        }


        schedule.details = [];

        for (var x = 1; x <= this.dishCycleEdit.sets.length; x++) {
          let detail = new DishCycleScheduleDetail();
          detail.sequence = x;
          detail.dishCycleScheduleId = schedule.id;
          if (sched && sched.length > 0) {
            let dets = sched[0].details.filter(e => e.sequence == x);
            if (dets && dets.length > 0) {
              detail = dets[0];
            }
          }

          if (clearMenu) {
            detail.menus = [];
          }

          schedule.details.push(detail);
        }

        schedules.push(schedule);
      }

      this.dishCycleEdit.schedules = schedules;

    }
  }

  onOutletChange(outletProfileId?: string) {
    if (this.dishCycleEdit.id) {
      this.alertService.showDialog('Changing the profile will remove all the schedules. Do you wish to proceed?', DialogType.confirm, () => this.changeOutletProfile(outletProfileId));
    } else {
      this.changeOutletProfile(outletProfileId);
    }
    
  }

  changeOutletProfile(outletProfileId?: string) {
    this.dishCycleEdit.schedules = [];
    this.getMealTypes();
    this.generateSchedule();
    this.getPeriods(false, this.dishCycleEdit.outletProfileId);
    //this.getPeriods(true, outletProfileId);
  }

  onCycleTypeChange() {
    if (this.dishCycleEdit.id) {
      this.alertService.showDialog('Changing the type will remove all the schedules. Do you wish to proceed?', DialogType.confirm, () => this.changeOutletProfile());
    } else {
      this.changeOutletProfile();
    }
  }

  isMeal() {
    return this.dishCycleEdit.cycleType == 'Ala Carte' || this.dishCycleEdit.cycleType == 'Main Menu';
  }

  onSetTypeChange(val, i) {
    if (this.dishCycleEdit.sets && this.dishCycleEdit.sets[i]) {
      this.dishCycleEdit.sets[i].cycleTypeId = val;
      this.dishCycleEdit.sets[i].dishCycleId = this.dishCycleEdit.id;
      this.dishCycleEdit.sets[i].options = this.dishCycles.filter(e => e.id == val);
    }
  }

  getSetOptions(i, day) {
    let menus = [];
    if (this.dishCycleEdit.sets && this.dishCycleEdit.sets.length > 0) {
      const set = this.dishCycleEdit.sets[i];
      let cycles = this.dishCycles.filter(e => e.id == set.cycleTypeId);
      if (cycles && cycles.length > 0) {
        let schedules = cycles[0].schedules;
        if (schedules && schedules.length > 0 && i < cycles[0].numOfSets) {
          let mod = day % cycles[0].numOfDays;
          let d = mod == 0 ? cycles[0].numOfDays : mod;
          let schedDays = cycles[0].schedules.filter(e => e.day == d);
          if (schedDays && schedDays.length > 0) {
            let dets = schedDays[0].details.filter(e => e.sequence == set.cycleTypeSequence);
            if (dets && dets.length > 0) {
              menus = dets[0].menus;
            }
          }
        }
      }

    }
    return menus;
  }

  onMealCreditColChange(cycleTypeId, cycleSeq, seq) {
    if (this.dishCycleEdit.id) {
      this.alertService.showDialog('Changing the type will remove all the menus. Do you wish to proceed?', DialogType.confirm, () => this.getDishesByCycle(cycleTypeId, cycleSeq, seq));
    } else {
      this.getDishesByCycle(cycleTypeId, cycleSeq, seq);
    }
  }

  getDishesByCycle(cycleTypeId, cycleSeq, seq, clearMenu: boolean = true) {
    if (!this.dishCycleEdit.schedules) this.dishCycleEdit.schedules = [];
    if (clearMenu) {
      this.dishCycleEdit.schedules.forEach((sched) => {
        if (sched.details && sched.details.length > 0 && seq < sched.details.length) {
          let dets = sched.details.filter(e => e.sequence == seq);
          if (dets && dets[0] && dets[0].menus) {
            dets[0].menus = [];
          }
        }
      });
    }

    let cycles = this.dishCycles.filter(e => e.id == cycleTypeId);
    if (cycles && cycles.length > 0) {
      let schedules = cycles[0].schedules;
      if (schedules && schedules.length > 0 && cycleSeq <= cycles[0].numOfSets) {
        if (!this.dishCycleEdit.schedules) this.dishCycleEdit.schedules = [];
        this.dishCycleEdit.schedules.forEach((dcSched) => {
          let mod = dcSched.day % cycles[0].numOfDays;
          let day = mod == 0 ? cycles[0].numOfDays : mod;
          let sScheds = schedules.filter(e => e.day == day);
          if (sScheds && sScheds.length > 0) {
            //set menus for that day/s
            if (!dcSched.details) dcSched.details = [];
            let sDets = sScheds[0].details.filter(e => e.sequence == cycleSeq);
            let dets = dcSched.details.filter(e => e.sequence == seq);
            if (dets && dets.length > 0) {
              if (sDets && sDets.length > 0 && seq <= dcSched.details.length) {
                dets[0].displayMenus = sDets[0].menus;
              } else {
                dets[0].displayMenus = [];
              }
              
            }
          }
        });
      }
    }
  }

  getAlaCarteCols(val, set) {
    let col = [];
    let cycles = this.dishCycles.filter(e => e.id == val);
    if (cycles && cycles.length > 0) {
      let numOfSets = cycles[0].numOfSets;
      let arr = [];
      for (var i = 1; i <= numOfSets; i++) {
        arr.push({ label: cycles[0].sets[i-1].label, sequence: i });
      }
      set.cycleSequences = arr;
    }
  }

  onAlaCarteColChange(cycleTypeId, i, seq) {
    if (this.dishCycleEdit.id) {
      this.alertService.showDialog('Changing the type will remove all the selected menus. Do you wish to proceed?', DialogType.confirm, () => this.getAlaCarteDishes(cycleTypeId, i, seq));
    } else {
      this.getAlaCarteDishes(cycleTypeId, i, seq);
    }
  }

  getAlaCarteDishes(cycleTypeId, cycleSeq, seq, clearMenu: boolean = true) {
    if (!this.dishCycleEdit.schedules) this.dishCycleEdit.schedules = [];
    if (clearMenu) {
      this.dishCycleEdit.schedules.forEach((sched) => {
        if (sched.details && sched.details.length > 0 && seq <= sched.details.length) {
          let dets = sched.details.filter(e => e.sequence == seq);
          if (dets && dets[0] && dets[0].menus) {
            dets[0].menus = [];
          }
        }
      });
    }
    

    let menus = [];
    //let cycles = this.dishCycles.filter(e => e.id == cycleTypeId);
    //if (cycles && cycles.length > 0) {
    //  let schedules = cycles[0].schedules;
    //  if (schedules && schedules.length > 0 && cycleSeq <= cycles[0].numOfSets) {
    //    schedules.forEach((sched) => {
    //      if (sched.details && sched.details.length > 0 && cycleSeq <= sched.details.length) {
    //        let dets = sched.details.filter(e => e.sequence == cycleSeq);
    //        if (dets && dets[0] && dets[0].menus && dets[0].menus.length > 0) {
    //          menus = [...menus, ...dets[0].menus];
    //        }
    //      }
    //    });
    //  }
    //}

    //if (this.dishCycleEdit.sets && this.dishCycleEdit.sets[seq - 1]) {
    //  this.dishCycleEdit.sets[seq - 1].options = [];
    //  this.dishCycleEdit.sets[seq - 1].options = menus;
    //}

    let cycles = this.dishCycles.filter(e => e.id == cycleTypeId);
    if (cycles && cycles.length > 0) {
      let schedules = cycles[0].schedules;
      if (schedules && schedules.length > 0 && cycleSeq <= cycles[0].numOfSets) {
        if (!this.dishCycleEdit.schedules) this.dishCycleEdit.schedules = [];
        this.dishCycleEdit.schedules.forEach((dcSched) => {
          let mod = dcSched.day % cycles[0].numOfDays;
          let day = mod == 0 ? cycles[0].numOfDays : mod;
          let sScheds = schedules.filter(e => e.day == day);
          if (sScheds && sScheds.length > 0) {
            //set menus for that day/s
            if (!dcSched.details) dcSched.details = [];
            let sDets = sScheds[0].details.filter(e => e.sequence == cycleSeq);
            let dets = dcSched.details.filter(e => e.sequence == seq);
            if (dets && dets.length > 0) {
              if (sDets && sDets.length > 0 && seq <= dcSched.details.length) {
                //menus = [...menus, ...dets[0].menus];
                this.dishCycleEdit.sets[seq - 1].options = sDets[0].menus;
              } 
            }
          }
        });
      }
    }

    //if (this.dishCycleEdit.sets && this.dishCycleEdit.sets[seq - 1]) {
    //  this.dishCycleEdit.sets[seq - 1].options = [];
    //  this.dishCycleEdit.sets[seq - 1].options = menus;
    //}
  }

  //setMinEndDate() {
  //  console.log('start date', this.voucherEdit.startDateTime.toString());
  //  this.minEndDate = this.voucherEdit.startDateTime.toString();
  //}

  get canManageDishCycles() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCatererDishCyclesMenu)
  }
}
