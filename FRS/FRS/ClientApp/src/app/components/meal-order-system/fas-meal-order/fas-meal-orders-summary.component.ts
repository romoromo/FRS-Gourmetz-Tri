import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { ClassBatch } from 'src/app/models/meal-order/class-batch.model';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { Subscription, Subject } from 'rxjs';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { forEach } from '@angular/router/src/utils/collection';
import { takeUntil, switchMap } from 'rxjs/operators';


@Component({
  selector: 'fas-meal-order-summary',
  templateUrl: './fas-meal-orders-summary.component.html',
  styleUrls: ['./fas-meal-orders-summary.component.css']
})
export class FasMealOrderSummaryComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private cancelPreviousRequests = new Subject<void>();
  private cancelPreviousDishTypeRequests = new Subject<void>();

  columns: any[] = [];
  rows: any[] = [];
  rowsCache: any[] = [];
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
  isLoadingDishTypes: boolean;

  storeId: string;
  delvdate: Date = new Date();
  delvdateTo: Date = new Date();
  dishTypeId: string;
  mealSessionDetailId: string;
  mealSessionId: string;
  isClear: boolean;
  daysToFreezeOrdering: number = 2;

  @Input() isHideHeader: boolean;
  @Input() outletId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog, private mealService: MealService, private dishService: DishService, private menuService: MenuService) {
  }

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

  getDishtTypes(d: Date, dTo: Date) {
    this.subscription.add(this.dishService.getDishTypesByActiveDishCycles(this.outletId, (d).toDateString(), (dTo).toDateString())
      .subscribe(results => {
        this.allDishTypes = results;

        if (!this.allDishTypes)
          this.allDishTypes = [];
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
            MessageSeverity.error);
        }));

    //this.cancelPreviousDishTypeRequests.next();
    //this.isLoadingDishTypes = true;
    //this.dishService.getDishTypesByActiveDishCycles(this.outletId, (d).toDateString(), (dTo).toDateString())
    //  .pipe(
    //    takeUntil(this.cancelPreviousDishTypeRequests),
    //    switchMap(results => {
    //      this.allDishTypes = results;

    //      this.isLoadingDishTypes = false;
    //      this.alertService.stopLoadingMessage();
    //      return [];
    //    })
    //  )
    //  .subscribe(
    //    () => { this.isLoadingDishTypes = false; },
    //    error => {
    //      this.isLoadingDishTypes = false;
    //      this.alertService.stopLoadingMessage();
    //      this.alertService.showStickyMessage("Get Error", `An error occurred while retrieving records.\r\n"`, MessageSeverity.error);
    //    }
    //  );
  }


  getDishtTypesOld() {
    let filter = new Filter();
    let f = this.outletId ? '(DishTypeInOutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';
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
    this.getDishtTypes(this.delvdate, this.delvdateTo);
    //this.getFasTokenOrderSummary(this.delvdate, this.delvdateTo);
  }

  onChangeStore() {
    //this.getFasTokenOrderSummary(this.delvdate, this.delvdateTo);
    this.getMealSessions(this.delvdate, this.delvdateTo);
    this.getDishtTypes(this.delvdate, this.delvdateTo);
  }

  onShowSummary() {
    this.getFasTokenOrderSummary(this.delvdate, this.delvdateTo);
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
    //          mealSessions.push({ mealSessionId: d.mealSessionId, mealSessionName: d.mealSessionName });
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

  getFasTokenOrderSummary(d: Date, dTo: Date) {
    if (this.outletId && this.storeId) {
      this.isShowSummary = true;
      this.menuService.getFasTokenOrderSummary(this.outletId, this.storeId, d.toDateString(), dTo.toDateString())
        .subscribe(results => {
          this.isShowSummary = false;
          console.log(results);
          this.rows = results;
          //this.columns = [];

          //if (results && results.cols) {
          //  results.cols.forEach((col, i, cols) => {
          //    this.columns.push({ name: col, sortable: false, prop: col });
          //  });

          //  let rows = [];
          //  results.rows.forEach((row, i, r) => {
          //    let rowObj = {};
          //    row.cells.forEach((cell, c, cells) => {
          //      rowObj[this.columns[c].name] = cell;
          //    });
          //    rows.push(rowObj);
          //  });

          //  //if (results.total.cells && results.total.cells.length > 1) {
          //    let totalRowObj = {};
          //    results.total.cells.forEach((cell, c, cells) => {
          //      totalRowObj[this.columns[c].name] = cell;
          //    });
          //    rows.push(totalRowObj);
          //  //}

          //  this.rowsCache = [...rows];
          //  this.rows = rows;
          //}
        },
          error => {
            this.isShowSummary = false;
            this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
              MessageSeverity.error);
          })
    }
  }

  //initializeFilter() {
  //  this.filter = new Filter(1, 10);
  //  this.filter.sorts = 'name';
  //  this.filter.filters = '';
  //  this.filter.page = 1;
  //}

  //initializePagedResult() {
  //  this.pagedResult = new PagedResult();
  //  this.pagedResult.totalCount = 0;
  //  this.pagedResult.pagedData = [];
  //  this.pagedResult.filter = this.filter;
  //}

  //initializeTableDefinition() {
  //  //let gT = (key: string) => this.translationService.getTranslation(key);

  //  //this.columns = [
  //  //  { prop: 'name', name: gT('common.Name'), width: 200 },
  //  //  { prop: 'year', name: 'Year', width: 200 },
  //  //  { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
  //  //];

  //  //if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
  //  //  this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
  //  //}
  //}

  ngOnInit() {
    this.getDeliveryLocations();
    //this.getDishtTypes();
    this.onChangeDate();

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
    this.cancelPreviousDishTypeRequests.next();
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

  //loadData(ev?: any) {
  //  this.alertService.startLoadingMessage();
  //  this.loadingIndicator = true;
  //  this.filter.pageSize = 10;

  //  if (ev) {
  //    this.filter.page = ev.offset + 1;
  //    if (ev.sorts) {
  //      this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
  //    }
  //  }

  //  if (!this.keyword) this.keyword = '';
  //  this.filter.filters = '(IsActive)==true,(Name)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
  //  //this.classService.getClassBatchesByFilter(this.filter)
  //  //  .subscribe(results => {
  //  //    this.pagedResult = results;

  //  //    this.alertService.stopLoadingMessage();
  //  //    this.loadingIndicator = false;

  //  //    let classBatches = results.pagedData;

  //  //    classBatches.forEach((classBatch, index, classBatches) => {
  //  //      (<any>classBatch).index = index + 1;
  //  //    });


  //  //    this.rowsCache = [...classBatches];
  //  //    this.rows = classBatches;

  //  //  },
  //  //    error => {
  //  //      this.alertService.stopLoadingMessage();
  //  //      this.loadingIndicator = false;

  //  //      this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
  //  //        MessageSeverity.error);
  //  //    });
  //}


  //onSearchChanged(value: string) {
  //  this.keyword = value;
  //  this.loadData(null);
  //}


  private save(clear?: boolean) {
    if (!this.outletId || !this.delvdate || !this.delvdateTo || !this.mealSessionId || !this.dishTypeId) {
      alert('Please select options from the dropdown.');
      return false;
    }

    if (this.delvdate.getTime() > this.delvdateTo.getTime()) {
      alert('Cannot create orders on this date. Cut-off limit exceeded.');

      return;
    }

    if (this.delvdate.getTime() < this.getCutoffDate().getTime()) {
      alert('Cannot create orders on this date. Cut-off limit exceeded.');

      return;
    }

    if (!confirm(`Are you sure you want to process FAS orders?`)) return;
    this.isSaving = true;
    this.isClear = clear;
    //this.alertService.startLoadingMessage("Processing orders...");
    this.menuService.bulkFasTokenOrder(this.outletId, this.storeId, this.delvdate.toDateString(), this.delvdateTo.toDateString(), this.dishTypeId, this.mealSessionId, this.accountService.currentUser.id, clear)
      .subscribe(response => {
        if (response.isSuccess) {
          this.alertService.showMessage("Success", `Dishes are assigned to FAS students`, MessageSeverity.success);
          this.getFasTokenOrderSummary(this.delvdate, this.delvdateTo);
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

  get canManageFasMealOrders() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtFasPermission)
  }

}
