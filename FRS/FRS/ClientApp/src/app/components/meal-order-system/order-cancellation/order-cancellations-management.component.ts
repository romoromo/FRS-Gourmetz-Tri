import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, OrderCancellationFilter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDatepickerInputEvent } from '@angular/material';
import { AuditService } from 'src/app/services/audit.service';
import { DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { saveAs } from 'file-saver';
import * as moment from 'moment';
import { TokenOrder } from 'src/app/models/meal-order/token-order.model';
import { OrderService } from 'src/app/services/meal-order/order.service';


@Component({
  selector: 'order-cancellations-management',
  templateUrl: './order-cancellations-management.component.html',
  styleUrls: ['./order-cancellations-management.component.css']
})
export class OrderCancellationsManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  statuses = ['All', 'pending', 'paid', 'cancelled', 'deleted'];
  columns: any[] = [];
  rows: TokenOrder[] = [];
  rowsCache: TokenOrder[] = [];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: OrderCancellationFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  status: string = 'paid';
  isFAS: boolean = false;
  start?: Date = null;
  end?: Date = null;

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  private selected: any[] = [];
  private allRowsSelected = false;
  tstart = new Date();
  tend = new Date();

  isClearResults = true;

  @ViewChild('searchbox') searchbox: SearchBoxComponent;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('hdrTpl') hdrTpl: TemplateRef<any>;


  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('mealDescription')
  mealDescriptionTemplate: TemplateRef<any>;

  @ViewChild('orderTable') table: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private orderService: OrderService) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new OrderCancellationFilter(1, 10);
    this.filter.sorts = '-deliveryDate';
    this.filter.filters = '';
    this.filter.page = 1;
  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { width: 50, cellTemplate: this.indexTemplate, canAutoResize: false, headerCheckboxable: true, headerTemplate: this.hdrTpl, },
      { prop: 'className', name: 'Class' },
      { prop: 'profileName', name: 'Profile' },
      { prop: 'isFASDisplay', name: 'FAS', sortable: false },
      { prop: 'deliveryDate', name: 'Delivery Date', pipe: new DateTimeOnlyPipe('en-SG')},
      { prop: 'transactionTime', name: 'Date', pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'mealSessionName', name: 'Session' },
      { prop: 'mealDescription', name: 'Meal Description', cellTemplate: this.mealDescriptionTemplate, sortable: false, draggable: false },
      { prop: 'totalAmount', name: 'Total Amount' },
      { prop: 'status', name: 'Status' },
      { prop: 'paymentTypeName', name: 'Payment Type' },
      { prop: 'paymentNumber', name: 'Order No.' },
      { prop: 'invoiceNumber', name: 'Invoice No.' },
      { prop: 'voucherCode', name: 'Voucher' },
      { prop: 'processedBy', name: 'Processed By' }
    ];
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    //this.loadData();
  }


  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(invoiceNumber|paymentNumber)@=' + this.keyword + ',(status)==paid';

    if (this.start && this.end) {
      this.filter.filters = this.filter.filters + ',(AuditOrderLogDateRange)==' + this.start.toDateString() + '|' + this.end.toDateString();
    }

    if (this.isFAS) {
      this.filter.filters += ',(isFas)==true';
    }

    this.filter.outletId = this.outletId;
    this.orderService.getOrderCancellationsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let orders = results.pagedData;

        orders.forEach((order, index, orders) => {
          (<any>order).index = index + 1;
        });


        this.rowsCache = [...orders];
        this.rows = orders;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve order cancellations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
      if (!this.end) this.end = new Date(event.value)
    }

    if (type == 'end') {
      this.end = new Date(event.value);
      if (!this.start) this.start = new Date(event.value)
    }

    if (type == 'tstart') {
      this.tstart = new Date(event.value);
    }

    if (type == 'tend') {
      this.tend = new Date(event.value);
    }

    //this.loadData();

    //this.loadData();
  }

  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    //this.loadData(null);
  }

  onSearch() {
    this.isClearResults = true;
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  clearFilters() {
    this.start = null;
    this.end = null;
    this.keyword = '';

    this.selected = [];
    this.isClearResults = false;
    this.allRowsSelected = false;
    this.clearFilterAndPagedResult();
    //this.searchbox.clear();
  }

  onCheckboxChangeFn(ev) {
    console.log(ev);
  }
  onSelect({ selected }) {
    console.log(selected);
    this.selected = selected;
  }

  cancelOrder() {
    if (this.selected && this.selected.length > 0) {
      this.alertService.showDialog('Are you sure you want to cancel selected order/s?', DialogType.confirm, () => this.cancelOrderHelper());
    } else {
      this.alertService.showMessage('Select at least one order to cancel', '', MessageSeverity.info);
    }
    
  }


  cancelOrderHelper() {

    this.alertService.showDialog('Enter cancellation reason', DialogType.prompt, (val) => {
      if (!val) return;
      this.alertService.startLoadingMessage("Cancelling orders...");
      this.loadingIndicator = true;
      let orderIds = this.selected.map((e) => { return e.id; });
      let model = { orderIds: orderIds, cancelledBy: this.accountService.currentUser.id, reason: val }
      this.subscription.add(this.orderService.cancelOrders(model)
        .subscribe(results => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showMessage('Success', 'Orders successfully cancelled.', MessageSeverity.success);
          this.onSearch();
        },
          error => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;

            this.alertService.showStickyMessage("Cancel Error", `An error occured while cancelling the orderss\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
              MessageSeverity.error);
          }));
    }, () => {
    });

    
  }

  get canManageAuthLogs() {
    return true;// this.accountService.userHasPermission(Permission.viewAuthLogsPermission)
  }

}
