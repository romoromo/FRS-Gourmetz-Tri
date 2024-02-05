import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { MatDatepickerInputEvent, MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ClassBatch } from 'src/app/models/meal-order/class-batch.model';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { Subscription, Subject } from 'rxjs';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { forEach } from '@angular/router/src/utils/collection';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { StudentGroup, StudentGroupSession } from 'src/app/models/meal-order/student-group.model';
import { takeUntil, switchMap } from 'rxjs/operators';

@Component({
  selector: 'meal-plan-summary',
  templateUrl: './meal-plans-summary.component.html',
  styleUrls: ['./meal-plans-summary.component.css']
})
export class MealPlanSummaryComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private cancelPreviousRequests = new Subject<void>();
  columns: any[] = [];
  rows: ClassBatch[] = [];
  rowsCache: ClassBatch[] = [];
  allPermissions: Permission[] = [];
  stores: StoreInfo[] = [];
  allDishTypes: any[] = [];
  mealSessionDetails: any[] = [];
  mealSessions: any[] = [];
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  isSaving: boolean;
  isShowSummary: boolean;
  isLoadingMealSessions: boolean;
  daysToFreezeOrdering: number = 2;

  storeId: string;
  delvdate: Date = new Date();
  delvdateTo: Date = new Date();
  minDate: Date = new Date();
  maxDate: Date = new Date();

  dishTypeId: string;
  mealSessionDetailId: string;
  sessions: StudentGroupSession[];
  isClear: boolean;
  title: string;
  group: StudentGroup;
  mealSessionId: string;

  @Input() isHideHeader: boolean;
  @Input() outletId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog, private mealService: MealService, private dishService: DishService, private menuService: MenuService,
    @Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<MealPlanSummaryComponent>) {
    this.title = 'Meal Plan ';
    if (data.group) {
      this.group = data.group;
      this.minDate = new Date(this.group.deliveryStartDate);
      this.maxDate = new Date(this.group.deliveryEndDate);
      this.delvdate = new Date(this.group.deliveryStartDate);
      this.delvdateTo = new Date(this.group.deliveryEndDate);
      this.sessions = this.group.sessions;
    }

    if (typeof (data.group.name) != typeof (undefined)) {
      this.title += `for ${data.group.name}`;
    }

    if (data.group.mealSessionName) {
      this.title += ` (${data.group.mealSessionName})`;
    }
    this.outletId = data.outletId;

    this.getMealSessions(this.delvdate, this.delvdateTo);
  }

  public dateFilter = (d: Date | null): boolean => {
    return (d >= this.minDate) && (d <= this.maxDate);
  };

  getDeliveryLocations() {
    let filter = new Filter();
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',(IsActive)==true,(StoreType)@=Inventory' : '(IsActive)==true,(StoreType)@=Inventory';
    filter.filters = f;
    this.subscription.add(this.deliveryService.getStoreInfosByFilter(filter)
      .subscribe(results => {
        this.stores = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getDishtTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.dishService.getDishTypesByFilter(filter)
      .subscribe(results => {
        this.allDishTypes = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
            MessageSeverity.error);
        })
  }

  onChangeDate(event?: MatDatepickerInputEvent<Date>, type?: string) {
    if (type == 'from') {
      this.delvdate = event ? new Date(event.value) : new Date();
    } else {
      this.delvdateTo = event ? new Date(event.value) : new Date();
    }
    
    this.getMealSessions(this.delvdate, this.delvdateTo);
    //this.getMealPlanSummary(this.delvdate, this.delvdateTo);
  }

  onChangeStore() {
    //this.getMealPlanSummary(this.delvdate, this.delvdateTo);
    this.getMealSessions(this.delvdate, this.delvdateTo);
  }

  onShowSummary() {
    this.getMealPlanSummary(this.delvdate, this.delvdateTo);
  }

  getMealSessions(d: Date, dTo: Date) {
    this.cancelPreviousRequests.next();
    this.isLoadingMealSessions = true;
    this.alertService.startLoadingMessage("Loading Meal Sessions...");
    this.menuService.getOutletSessionsByFilter(this.outletId, (d).toDateString(), (dTo).toDateString())
      .pipe(
        takeUntil(this.cancelPreviousRequests),
        switchMap(results => {
          this.mealSessionDetails = results;
          let mealSessions = [];
          if (this.mealSessionDetails) {
            this.mealSessionDetails.forEach((d, i, details) => {
              let indx = mealSessions && mealSessions.length > 0 ? mealSessions.findIndex(e => e.mealSessionId == d.mealSessionId) : -1;
              if (indx < 0) {
                let ms = this.sessions && this.sessions.length > 0 ? this.sessions.findIndex(e => e.mealSessionId == d.mealSessionId) : -1;
                if (ms > -1)
                  mealSessions.push({ mealSessionId: d.mealSessionId, mealSessionName: d.mealSessionName });
              }
            });
          }

          this.mealSessions = mealSessions;
          this.isLoadingMealSessions = false;
          this.alertService.stopLoadingMessage();
          return [];
        })
      )
      .subscribe(
        () => { this.isLoadingMealSessions = false; },
        error => {
          this.isLoadingMealSessions = false;
          this.alertService.stopLoadingMessage();
          this.alertService.showStickyMessage("Get Error", `An error occurred while retrieving records.\r\n"`, MessageSeverity.error);
        }
    );

    //this.menuService.getOutletSessionsByFilter(this.outletId, (d).toDateString(), (dTo).toDateString())
    //  .subscribe(results => {
    //    this.mealSessionDetails = results;
    //    let mealSessions = [];
    //    if (this.mealSessionDetails) {
    //      this.mealSessionDetails.forEach((d, i, details) => {
    //        let indx = mealSessions && mealSessions.length > 0 ? mealSessions.findIndex(e => e.mealSessionId == d.mealSessionId) : -1;
    //        if (indx < 0) {
    //          let ms = this.sessions && this.sessions.length > 0 ? this.sessions.findIndex(e => e.mealSessionId == d.mealSessionId) : -1;
    //          if (ms > -1)
    //            mealSessions.push({ mealSessionId: d.mealSessionId, mealSessionName: d.mealSessionName });
    //        }
    //      })
    //    }

    //    this.mealSessions = mealSessions;
    //  },
    //    error => {
    //      this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
    //        MessageSeverity.error);
    //    })
  }

  getMealPlanSummary(d: Date, dTo: Date) {
    if (this.outletId && this.storeId) {
      this.isShowSummary = true;
      this.menuService.getMealPlanSummary(this.group.id, this.outletId, this.storeId, d.toDateString(), dTo.toDateString(), this.mealSessionId)
        .subscribe(results => {
          this.isShowSummary = false;
          this.columns = [];

          if (results && results.cols) {
            results.cols.forEach((col, i, cols) => {
              this.columns.push({ name: col, sortable: false, prop: col, frozenLeft: i == 0, width: 250 });
            });

            let rows = [];
            results.rows.forEach((row, i, r) => {
              let rowObj = {};
              row.cells.forEach((cell, c, cells) => {
                rowObj[this.columns[c].name] = cell;
              });
              rows.push(rowObj);
            });

            //if (results.total.cells && results.total.cells.length > 1) {
              let totalRowObj = {};
              results.total.cells.forEach((cell, c, cells) => {
                totalRowObj[this.columns[c].name] = cell;
              });
              rows.push(totalRowObj);
            //}

            this.rowsCache = [...rows];
            this.rows = rows;
          }
        },
          error => {
            this.isShowSummary = false;
            this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
              MessageSeverity.error);
          })
    }
  }

  ngOnInit() {
    this.getDeliveryLocations();
    this.getDishtTypes();
    this.getOutlet()
      .subscribe(outlet => {
        console.log(outlet);
        if (outlet) {
          this.daysToFreezeOrdering = outlet.daysToFreezeOrdering;
        } else {
          // set default 2
          this.daysToFreezeOrdering = 2;
        }

        let now = this.getCutoffDate();

        this.delvdate = this.getCutoffDate();
        this.delvdateTo = this.getCutoffDate();
      },
        error => {
          console.error(error);
        });
  }

  ngOnDestroy() {
    this.cancelPreviousRequests.next();
  }

  getOutlet() {
    return this.deliveryService.getOutletByIdSimple(this.outletId);
  }


  getCutoffDate() {
    let now = new Date();
    now.setHours(0, 0, 0, 0);
    now.setDate(now.getDate() + this.daysToFreezeOrdering);
    return now;
  }

  private save(clear?: boolean) {
    if (!this.outletId || !this.delvdate || !this.delvdateTo || !this.mealSessionId || !this.dishTypeId) return false;

    if (this.delvdate.getTime() > this.delvdateTo.getTime()) {
      alert('Cannot create orders on this date. Cut-off limit exceeded.');

      return;
    }

    if (this.delvdate.getTime() < this.getCutoffDate().getTime()) {
      alert('Cannot create orders on this date. Cut-off limit exceeded.');

      return;
    }

    if (!confirm(`Are you sure you want to assign this dish to the meal plan?`)) return;
    this.isSaving = true;
    this.isClear = clear;
    //this.alertService.startLoadingMessage("Processing orders...");
    this.menuService.bulkMealPlanOrder(this.group.id, this.outletId, this.storeId, this.delvdate.toDateString(), this.delvdateTo.toDateString(), this.dishTypeId, this.mealSessionId, this.accountService.currentUser.id, clear)
      .subscribe(response => {
        if (response.isSuccess) {
          this.alertService.showMessage("Success", `Dishes are assigned to the meal plan.`, MessageSeverity.success);
          this.delvdate = new Date(this.minDate);
          this.delvdateTo = new Date(this.maxDate);
          this.onShowSummary();
        } else {
          this.alertService.showMessage("Error", `Something went wrong with the assignment. ${response.message}`, MessageSeverity.error);
        }
        this.isSaving = false;
      },
      error => this.saveFailedHelper(error));
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  }

  private cancel() {
    this.dialogRef.close();
  }

  get canManageMealPlans() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtStudentGroupsPermission)
  }


}
