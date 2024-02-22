import { ViewChild, Component, Inject, OnInit, OnDestroy } from "@angular/core";
import { AlertService, MessageSeverity, DialogType } from "src/app/services/alert.service";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MatDatepickerInputEvent } from "@angular/material";
import { Subscription } from "rxjs";
import { NewOrder } from "src/app/models/meal-order/token-order.model";
import { DefaultPipe } from "../../../../pipes/default-string-val.pipe";
import { MenuService } from "src/app/services/meal-order/menu.service";
import { MomentUtcDateAdapter } from "src/app/helpers/moment-utc-adapter";
import { MAT_MOMENT_DATE_FORMATS } from "@angular/material-moment-adapter";
import { MealSessionDetail } from "src/app/models/meal-order/meal-session.model";
import * as moment from 'moment';
import { FileService } from "src/app/services/file.service";
import { Utilities } from "src/app/services/utilities";
import { OrderService } from "src/app/services/meal-order/order.service";
import { DeliveryService } from "src/app/services/meal-order/delivery.service";
import { Filter } from "src/app/models/sieve-filter.model";
import { DishService } from "../../../../services/meal-order/dish.service";

@Component({
  selector: 'student-order-editor',
  templateUrl: './student-order-editor.component.html',
  styleUrls: ['./student-order-editor.component.css'],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class StudentOrderEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private order: NewOrder;
  private blockedDates: any[];
  private outletBlockedDates: any[];
  private allDishCycles: any[] = [];
  private combined: any = [];
  private returnDishCycles: any = [];
  private availableDishCycles: any = [];
  
  private selectedDish: any;
  selectedDate: Date;
  today = new Date();

  // Calculate the date 4 days from today
  minDate = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate() + 4);
  private sessions: MealSessionDetail[] = [];

  constructor(private alertService: AlertService,
    private fileService: FileService, public dialogRef: MatDialogRef<StudentOrderEditorComponent>, public dialog: MatDialog,
    private deliveryService: DeliveryService, private orderService: OrderService, public menuService: MenuService, public dishService: DishService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.order = data.order;
    this.selectedDate = this.minDate;
    this.order.deliveryDate = moment(this.minDate).format('YYYY-MM-DD');
    this.getStoreInfos();
    //this.getAllDishCycles();
    this.getAllDishCyclesByDate();
    this.getSessions();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  private cancel() {

    this.alertService.resetStickyMessage();

    this.dialogRef.close();
  }


  onDateSelected(event: MatDatepickerInputEvent<Date>) {
    // Handle the date selection here
    const selectedDate = event.value;

    // Check if the selected date is valid
    if (selectedDate) {
      // Set the model to the selected date
      this.selectedDate = selectedDate;
      this.order.deliveryDate = moment(selectedDate).format('YYYY-MM-DD');
      this.getSessions();

    } else {
      // Clear the model if the date is not valid
      this.selectedDate = null;
    }
  }

  onDateSelected2(event: MatDatepickerInputEvent<Date>) {
    // Handle the date selection here
    const selectedDate = event.value;

    // Check if the selected date is valid
    if (selectedDate) {
      // Set the model to the selected date
      this.selectedDate = selectedDate;
      this.order.deliveryDate = moment(selectedDate).format('YYYY-MM-DD');
      this.getSessions();
      for (let dc of this.allDishCycles) {
        this.updateMenu(new Date(this.selectedDate), dc);
      }

      this.updateCombined();

    } else {
      // Clear the model if the date is not valid
      this.selectedDate = null;
    }
  }

  updateCombined() {
    let allset: any = [];

    for (let dc of this.availableDishCycles) {
      allset = [...allset, ...(dc.sets != null ? dc.sets : [])];
    }

    this.combined = this.sortSets([...new Set(allset)]);
    console.log(this.combined);
  }

  updateCombined2() {
    let allset: any = [];

    for (let dc of this.allDishCycles) {
      allset = [...allset, ...(dc.newSets != null ? dc.newSets : [])];
    }

    this.combined = this.sortSets([...new Set(allset)]);
    console.log(this.combined);
  }

  updateMenu(date, dishCycle) {
    if (!date) return;

    let day =
      (Math.abs(this.dateDiffInDays(date, new Date(dishCycle.startDate))) + 1) % dishCycle.numOfDays;

    let dishCycleSchedule = dishCycle.schedules ? dishCycle.schedules.find((s) => s.day === day) : [];

    for (let set of dishCycle.sets) {
      let detail = dishCycleSchedule && dishCycleSchedule.details ? dishCycleSchedule.details.find(
        (d) =>
          d.cycleTypeId === set.cycleTypeId &&
          d.cycleSeq === set.cycleTypeSequence &&
          d.seq === set.sequence &&
          d.menus.length > 0
      ) : [];

      set.displayMenus = detail && detail.menus ? detail.menus : [];

      set.dishCyclePeriods = dishCycle.dishCyclePeriods;
      set.startDate = dishCycle.startDate;
      set.endDate = dishCycle.endDate;
      set.mgStartDate = dishCycle.mgStartDate;
      set.mgEndDate = dishCycle.mgEndDate;
      //set.star
    }

    dishCycle.newSets = this.sortSets(dishCycle.sets);
  }

  updateMenu2(date, dishCycle) {
    if (!date) return;

    let day =
      (Math.abs(this.dateDiffInDays(date, new Date(dishCycle.startDate))) + 1) % dishCycle.numOfDays;

    let dishCycleSchedule = dishCycle.schedules ? dishCycle.schedules.find((s) => s.day === day) : [];

    for (let set of dishCycle.sets) {
      let detail = dishCycleSchedule && dishCycleSchedule.details ? dishCycleSchedule.details.find(
        (d) =>
          d.cycleTypeId === set.cycleTypeId &&
          d.cycleSeq === set.cycleTypeSequence &&
          d.seq === set.sequence &&
          d.displayMenus.length > 0
      ) : [];

      set.displayMenus = detail && detail.displayMenus ? detail.displayMenus : [];

      set.dishCyclePeriods = dishCycle.dishCyclePeriods;
      set.startDate = dishCycle.startDate;
      set.endDate = dishCycle.endDate;
      set.mgStartDate = dishCycle.mgStartDate;
      set.mgEndDate = dishCycle.mgEndDate;
      //set.star
    }

    dishCycle.newSets = this.sortSets(dishCycle.sets);
  }

  getDishesByCycle(dishCycle, cycleTypeId, cycleSeq, seq, clearMenu: boolean = true) {
    if (!dishCycle.schedules) dishCycle.schedules = [];
    //cycleSeq = (cycleSeq ? cycleSeq : 1);
    let cycles;
    if (cycleTypeId) cycles = this.availableDishCycles.filter((e) => e.id == cycleTypeId);
    else cycles = [dishCycle];

    if (cycles && cycles.length > 0) {
      let schedules = cycles[0].schedules;
      if (schedules && schedules.length > 0 && cycleSeq <= cycles[0].numOfSets) {
        if (!dishCycle.schedules) dishCycle.schedules = [];
        dishCycle.schedules.forEach((dcSched) => {
          let mod = dcSched.day % cycles[0].numOfDays;
          let day = mod == 0 ? cycles[0].numOfDays : mod;
          let sScheds = schedules.filter((e) => e.day == day);

          if (sScheds && sScheds.length > 0) {
            //set menus for that day/s
            if (!dcSched.details) dcSched.details = [];

            let sDetsSequence = cycleSeq;
            if (!cycleTypeId && !cycleSeq) sDetsSequence = seq;
            if (!sDetsSequence) sDetsSequence = 1;

            let sDets = sScheds[0].details.filter((e) => e.sequence == sDetsSequence);
            let dets = dcSched.details.filter((e) => e.sequence == (seq ? seq : 1));

            if (dets && dets.length > 0) {
              if (sDets && sDets.length > 0 && seq <= dcSched.details.length) {
                dets[0].displayMenus = sDets[0].menus;
                dets[0].cycleTypeId = cycleTypeId;
                dets[0].cycleSeq = cycleSeq;
                dets[0].seq = seq;
              } else {
                dets[0].displayMenus = [];
              }
            }
          }
        });
      }
    }
  }

  getDishesByCycle2(dishCycle, cycleTypeId, cycleSeq, seq, clearMenu: boolean = true) {
    if (!dishCycle.schedules) dishCycle.schedules = [];
    //cycleSeq = (cycleSeq ? cycleSeq : 1);
    let cycles;
    if (cycleTypeId) cycles = this.returnDishCycles.filter((e) => e.id == cycleTypeId);
    else cycles = [dishCycle];

    if (cycles && cycles.length > 0) {
      let schedules = cycles[0].schedules;
      if (schedules && schedules.length > 0 && cycleSeq <= cycles[0].numOfSets) {
        if (!dishCycle.schedules) dishCycle.schedules = [];
        dishCycle.schedules.forEach((dcSched) => {
          let mod = dcSched.day % cycles[0].numOfDays;
          let day = mod == 0 ? cycles[0].numOfDays : mod;
          let sScheds = schedules.filter((e) => e.day == day);

          if (sScheds && sScheds.length > 0) {
            //set menus for that day/s
            if (!dcSched.details) dcSched.details = [];

            let sDetsSequence = cycleSeq;
            if (!cycleTypeId && !cycleSeq) sDetsSequence = seq;
            if (!sDetsSequence) sDetsSequence = 1;

            let sDets = sScheds[0].details.filter((e) => e.sequence == sDetsSequence);
            let dets = dcSched.details.filter((e) => e.sequence == (seq ? seq : 1));

            if (dets && dets.length > 0) {
              if (sDets && sDets.length > 0 && seq <= dcSched.details.length) {
                dets[0].displayMenus = sDets[0].menus;
                dets[0].cycleTypeId = cycleTypeId;
                dets[0].cycleSeq = cycleSeq;
                dets[0].seq = seq;
              } else {
                dets[0].displayMenus = [];
              }
            }
          }
        });
      }
    }
  }

  updateDishCycleSet(dishCycle) {
    for (var x = 0; x <= dishCycle.sets.length; x++) {
      if (dishCycle.sets[x]) {
        this.getDishesByCycle(
          dishCycle,
          dishCycle.sets[x].cycleTypeId,
          dishCycle.sets[x].cycleTypeSequence,
          dishCycle.sets[x].sequence,
          false
        );
      }
    }

    for (let set of dishCycle.sets) {
      this.getAlaCarteCols(set.cycleTypeId, set, dishCycle);
    }

    dishCycle.newSets = this.sortSets(dishCycle.sets);
  }

  getAlaCarteCols(val, set, dishCycle) {
    let col = [];
    let cycles = this.availableDishCycles.filter((e) => e.id == val);

    if (!val) cycles = [dishCycle];

    if (cycles && cycles.length > 0) {
      let numOfSets = cycles[0].numOfSets;
      let arr: any[] = [];
      for (var i = 1; i <= numOfSets; i++) {
        let set = cycles[0].sets[i - 1];

        arr.push({ label: set.label, sequence: i });
      }
      set.cycleSequences = arr;

      let sequence = set.cycleSequences.find((c) => c.sequence === (set.cycleTypeSequence ? set.cycleTypeSequence : 1));

      set.sequenceLabel = sequence.label ? sequence.label : "";
    }
  }

  getAlaCarteCols2(val, set, dishCycle) {
    let col = [];
    let cycles = this.returnDishCycles.filter((e) => e.id == val);

    if (!val) cycles = [dishCycle];

    if (cycles && cycles.length > 0) {
      let numOfSets = cycles[0].numOfSets;
      let arr: any[] = [];
      for (var i = 1; i <= numOfSets; i++) {
        let set = cycles[0].sets[i - 1];

        arr.push({ label: set.label, sequence: i });
      }
      set.cycleSequences = arr;

      let sequence = set.cycleSequences.find((c) => c.sequence === (set.cycleTypeSequence ? set.cycleTypeSequence : 1));

      set.sequenceLabel = sequence.label ? sequence.label : "";
    }
  }

  sortSets(sets) {
    const uniqueSets: any = Array.from(new Set(sets)); // Remove duplicates
    return uniqueSets
      .slice() // Create a shallow copy to avoid modifying the original array
      .sort((a, b) => {
        const labelComparison = (a.sequenceLabel || '').localeCompare(b.sequenceLabel || '');

        if (labelComparison !== 0) {
          return labelComparison;
        } else {
          return a.price - b.price;
        }
      });

    //return sets
    //  .slice() // Create a shallow copy to avoid modifying the original array
    //  .sort((a, b) => {
    //    if (a.sequenceLabel !== b.sequenceLabel) {
    //      return a.sequenceLabel.localeCompare(b.sequenceLabel);
    //    } else {
    //      return a.price - b.price;
    //    }
    //  });

    //let map: any = {};
    //let newSets: any[] = [];

    //for (let set of sets) {
    //  if (!map[set.sequenceLabel]) {
    //    let sameSets = sets.filter((s) => s.sequenceLabel === set.sequenceLabel);
    //    sameSets = sameSets.sort((a, b) => a.price - b.price);

    //    newSets = [...newSets, ...sameSets];

    //    map[set.sequenceLabel] = set;
    //  }
    //}

    //return newSets;
  }

  dateDiffInDays(a, b) {


    const _MS_PER_DAY = 1000 * 60 * 60 * 24;
    // Discard the time and time-zone information.
    const utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
    const utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());

    return Math.floor((utc2 - utc1) / _MS_PER_DAY);
  }

  formatDate(date) {
    if (typeof date.getMonth !== 'function') date = new Date(date);

    let month = '' + (date.getMonth() + 1);
    let day = '' + date.getDate();
    let year = date.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
  }

  dec2(num) {
    return (Math.round(num * 100) / 100).toFixed(2);
  }

  selectDish(dish, set) {
    if (this.selectedDish !== dish) {
      dish.mealTypeId = set.mealTypeId;
      dish.tokenDesc = set.sequenceLabel;
      dish.setPrice = set.price;
      dish.setId = set.id;
      this.selectedDish = dish;
      console.log(this.selectedDish);
    } else {
      this.selectedDish = null;
    }
  }

  isValidSet(set) {
    const mp = this.sessions.find((m) => this.order.mealSessionDetailId === m.id);
    const mpId = mp ? mp.mealPeriodId : null;
    return set.dishCyclePeriods.find((d) => d.mealPeriodId === mpId) &&
      set.displayMenus[0]; // &&
      //this.formatDate(set.startDate) <= this.order.deliveryDate &&
      //this.formatDate(set.endDate) >= this.order.deliveryDate &&
      //this.formatDate(set.mgStartDate) <= this.order.deliveryDate &&
      //this.formatDate(set.mgEndDate) >= this.order.deliveryDate;
  }

  isValidSet2(set) {
    const mp = this.sessions.find((m) => this.order.mealSessionDetailId === m.id);
    const mpId = mp ? mp.mealPeriodId : null;
    return set.dishCyclePeriods.find((d) => d.mealPeriodId === mpId) &&
      set.displayMenus[0] &&
      this.formatDate(set.startDate) <= this.order.deliveryDate &&
      this.formatDate(set.endDate) >= this.order.deliveryDate &&
      this.formatDate(set.mgStartDate) <= this.order.deliveryDate &&
      this.formatDate(set.mgEndDate) >= this.order.deliveryDate;
  }

  getStoreInfos() {
    let filter = new Filter();
    filter.filters = `(IsActive)==true,(outletId)==${this.order.outletId}`;
    this.deliveryService.getStoreInfosByFilter(filter)
      .subscribe(results => {
        if (results.pagedData.length > 0) {
          this.order.storeId = results.pagedData[0].id;
        } else {
          this.order.storeId = '';
        }
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving stores.\r\n"`,
          //  MessageSeverity.error);
        })
  }

  getSessions() {
    this.sessions = [];
    this.combined = [];
    this.subscription.add(this.menuService.getStudentSessionsByFilter(this.order.profileId, this.order.deliveryDate)
      .subscribe(results => {
        this.order.mealSessionDetailId = '';
        this.sessions = results;
        console.log("sessions: ", this.sessions)
      },
        error => {
          console.log(error);
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving meal sessions.\r\n"`,
          //  MessageSeverity.error);
        }))
  }

  // new implementations
  onChangeSession() {
    this.getAllDishCyclesByDate();
  }

  getAllDishCyclesByDate() {
    if (!this.order.mealSessionDetailId || !this.order.deliveryDate) return;

    try {
      this.alertService.startLoadingMessage("Loading meals...");
      this.subscription.add(this.dishService.getDishCyclesByStudent(this.order.profileId, this.order.deliveryDate, this.order.mealSessionDetailId)
        .subscribe(results => {
          this.availableDishCycles = results;
          this.blockedDates = [];
          this.outletBlockedDates = [];

          //this.getActiveDishCycles();
          console.log('availableDishCycles', this.availableDishCycles);
          for (let dc of this.availableDishCycles) {
            this.updateDishCycleSet(dc);

            let set = new Set(); 
            for (let date of dc.blockedDates) {
              set.add(date.effectiveDate);
            }
            this.blockedDates = [...new Set([...this.blockedDates, ...set])].sort();
          }

          //this.updateCombined();
          let filteredBlockedDates = this.blockedDates.filter(e => {
            let delDate = new Date(this.order.deliveryDate);
            let d = new Date(e);
            d.setHours(0, 0, 0, 0);
            delDate.setHours(0, 0, 0, 0);
            return d.getTime() === delDate.getTime();
          });

          if (!filteredBlockedDates || filteredBlockedDates.length == 0)
            this.updateDate(this.order.deliveryDate, true);

          this.alertService.stopLoadingMessage();
        },
          error => {
            this.alertService.stopLoadingMessage();
          }))
    } catch (ex) {
      console.log(ex);
      this.alertService.stopLoadingMessage();
    }
  }


  // end new implementations
  getMenuGroupActiveDishCycles(studentId) {
    return this.menuService.getMenuGroupActiveDishCycles(studentId);
  }

  getAllDishCycles() {
    try {
      this.alertService.startLoadingMessage("Loading meals...");
      let filter = new Filter();
      let f = this.order.outletId ? '(InOutletId)==' + this.order.outletId + ',' : '';
      filter.filters = f + '(IsActive)==true';
      

      this.subscription.add(this.dishService.getDishCyclesByFilter(filter)
        .subscribe(results => {
          this.returnDishCycles = results.pagedData;

          //this.getActiveDishCycles();
          console.log('returnDishCycles', this.returnDishCycles);

          this.alertService.stopLoadingMessage();
        },
          error => {
            this.alertService.stopLoadingMessage();
          }))
    } catch (ex) {
      console.log(ex);
      this.alertService.stopLoadingMessage();
    }
  }

  getActiveDishCycles() {
    try {
      //this.alertService.startLoadingMessage("Loading meals...");
      this.subscription.add(this.getMenuGroupActiveDishCycles(this.order.profileId)
        .subscribe(results => {
          let menuGroups = results;
          this.blockedDates = [];
          this.outletBlockedDates = [];

          this.allDishCycles = [];
          for (let mg of menuGroups) {
            for (let mgdc of mg.menuGroupDishCycles) {
              if (mgdc.dishCycle) {
                mgdc.dishCycle.mgStartDate = mg.startDate;
                mgdc.dishCycle.mgEndDate = mg.endDate;

                let ed = new Date(mgdc.dishCycle.endDate);

                ed.setDate(ed.getDate() + 1);

                mgdc.dishCycle.endDate = new Date(ed.toString().split('GMT')[0] + ' UTC').toISOString();

                this.allDishCycles.push(mgdc.dishCycle);
              }
            }
          }

          for (let dc of this.allDishCycles) {
            this.updateDishCycleSet(dc);

            let set = new Set();
            for (let date of dc.blockedDates) {
              set.add(date.effectiveDate);
            }
            //blockedDates = [...set].sort();

            this.blockedDates = [...new Set([...this.blockedDates, ...set])].sort();

            //if staff
            //updateIfStaff();
          }

          //this.updateDate(this.order.deliveryDate, true);
          this.updateCombined();

          //console.log('STUDENT', student);
          console.log('dishStudentCycleswithmenu', this.allDishCycles);

          //for (const i in menuGroups) {
          //  let menuGroup = menuGroups[i];
          //  if (menuGroup && menuGroup.menuGroupDishCycles && menuGroup.menuGroupDishCycles.length > 0) {
          //    let dishCycles = menuGroup.menuGroupDishCycles.map(function (v) { return v.dishCycle; });
          //    this.blockedDates = dishCycles.map(function (v) { return v.blockedDates; }).flat();
          //    this.outletBlockedDates = dishCycles.map(function (v) { return v.outletDishBlockedDates; }).flat();
          //    this.allDishCycles = this.allDishCycles.concat(dishCycles);
          //  }
          //}

          //console.log(this.allDishCycles);

          //this.blockedDates = [...new Set(this.blockedDates)];
          //this.outletBlockedDates = [...new Set(this.outletBlockedDates)];
          //console.log(this.blockedDates);
          //console.log(this.outletBlockedDates);

          this.alertService.stopLoadingMessage();
        },
          error => {
            this.alertService.stopLoadingMessage();
          }))
    } catch (ex) {
      console.log(ex);
      this.alertService.stopLoadingMessage();
    }
  }

  updateDate(date, noRefreshOrder) {
    console.log('update date', date);
    if (typeof date.getMonth !== 'function') date = new Date(date);

    for (let dc of this.availableDishCycles) {
      this.updateMenu(date, dc);
    }

    if (noRefreshOrder) this.selectedDish = null;

    this.updateCombined();
  }

  updateDate2(date, noRefreshOrder) {
    console.log('update date', date);
    if (typeof date.getMonth !== 'function') date = new Date(date);

    //this.dateString = this.formatDate(date);
    //this.getMealSessions(student, date);
    this.getSessions();
    for (let dc of this.allDishCycles) {
      this.updateMenu(date, dc);
    }

    //if (!noRefreshOrder) this.getOrderedMeal(student, date);

    if (noRefreshOrder) this.selectedDish = null;

    this.updateCombined();
  }

  saveOrder() {
    if (!this.selectedDish || !this.selectedDish.dishId) {
      alert('Please select dish.');
      return;
    }
    if (!this.order.deliveryDate || !this.order.mealSessionDetailId) return;

    this.alertService.showDialog('Are you sure you want to add selected order?', DialogType.confirm, () => this.saveOrderHelper());
  }

  saveOrderHelper() {

    this.alertService.startLoadingMessage("Saving order...");
    const order: any = {};

    Object.assign(order, this.order);
    order.tokens = [
      {
        tokenId: this.selectedDish.mealTypeId,
        mealTypeId: this.selectedDish.mealTypeId,
        tokenDesc: this.selectedDish.tokenDesc,
        qty: 1,
        dishId: this.selectedDish.dishId
      }
    ];

    order.totalAmount = this.selectedDish.setPrice;
    order.status = 'paid';
    order.periodId = '';

    this.subscription.add(this.orderService.createPrepaidOrder(order)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        if (results.isSuccess) {
          this.alertService.showMessage("Success", `Order added successfully`, MessageSeverity.success);
        }
        else {
          this.alertService.showMessage("Error", `${results.message}`, MessageSeverity.error);
        }

        this.dialogRef.close();
      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Save Error", `An error occured while saving the orders\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageStudents() {
    return true; //this.accountService.userHasPermission(Permission.manageStudentsPermission)
  }
}
