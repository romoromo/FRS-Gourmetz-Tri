import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
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
import { Subscription } from 'rxjs';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { forEach } from '@angular/router/src/utils/collection';


@Component({
  selector: 'fas-meal-order-summary',
  templateUrl: './fas-meal-orders-summary.component.html',
  styleUrls: ['./fas-meal-orders-summary.component.css']
})
export class FasMealOrderSummaryComponent implements OnInit {
  private subscription: Subscription = new Subscription();
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

  storeId: string;
  delvdate: Date = new Date();
  delvdateTo: Date = new Date();
  dishTypeId: string;
  mealSessionDetailId: string;
  mealSessionId: string;
  isClear: boolean;

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
    this.getFasTokenOrderSummary(this.delvdate, this.delvdateTo);
  }

  onChangeStore() {
    this.getFasTokenOrderSummary(this.delvdate, this.delvdateTo);
  }

  getMealSessions(d: Date, dTo: Date) {
    this.menuService.getOutletSessionsByFilter(this.outletId, (d).toDateString(), (dTo).toDateString())
      .subscribe(results => {
        this.mealSessionDetails = results;
        let mealSessions = [];
        if (this.mealSessionDetails) {
          this.mealSessionDetails.forEach((d, i, details) => {
            let indx = mealSessions && mealSessions.length > 0 ? mealSessions.findIndex(e => e.mealSessionId == d.mealSessionId) : -1;
            if (indx < 0) {
              mealSessions.push({ mealSessionId: d.mealSessionId, mealSessionName: d.mealSessionName });
            }
          })
        }

        this.mealSessions = mealSessions;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
            MessageSeverity.error);
        })
  }

  getFasTokenOrderSummary(d: Date, dTo: Date) {
    if (this.outletId && this.storeId) {
      this.menuService.getFasTokenOrderSummary(this.outletId, this.storeId, d.toDateString(), dTo.toDateString())
        .subscribe(results => {

          this.columns = [];

          if (results && results.cols) {
            results.cols.forEach((col, i, cols) => {
              this.columns.push({ name: col, sortable: false, prop: col });
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
    this.getDishtTypes();
    this.onChangeDate();

    let now = new Date();
    now.setHours(0, 0, 0, 0);
    now.setDate(now.getDate() + 3);

    this.delvdate = this.getCutoffDate();
    this.delvdateTo = this.getCutoffDate();
    //this.initializePagedResult();
    //this.initializeTableDefinition();
    //this.loadData();
  }

  getCutoffDate() {
    let now = new Date();
    now.setHours(0, 0, 0, 0);
    now.setDate(now.getDate() + 3);
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
    if (!this.outletId || !this.delvdate || !this.delvdateTo || !this.mealSessionId || !this.dishTypeId) return false;

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
          this.onChangeStore();
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
    return true; //this.accountService.userHasPermission(Permission.manageClassBatchesPermission)
  }

}
