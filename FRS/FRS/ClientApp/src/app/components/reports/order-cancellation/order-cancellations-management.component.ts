import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult, SalesOrderReportFilter } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { AuditService } from 'src/app/services/audit.service';
import { DateOnlyPipe, DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { saveAs } from 'file-saver';
import * as moment from 'moment';
import { TokenOrder } from 'src/app/models/meal-order/token-order.model';
import { ReportService } from 'src/app/services/report.service';
import { SalesOrderReportType } from 'src/app/models/enums';


@Component({
  selector: 'order-cancellation-report-management',
  templateUrl: './order-cancellations-management.component.html',
  styleUrls: ['./order-cancellations-management.component.css']
})
export class OrderCancellationReportManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: TokenOrder[] = [];
  rowsCache: TokenOrder[] = [];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: SalesOrderReportFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  status: string = 'paid';
  isFAS: boolean = false;
  start = new Date();
  end = new Date();

  tstart = new Date();
  tend = new Date();

  @ViewChild('searchbox') searchbox: SearchBoxComponent;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('mealDescription')
  mealDescriptionTemplate: TemplateRef<any>;

  @ViewChild('orderCancellationTable') table: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private service: ReportService) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new SalesOrderReportFilter(1, 10);
    this.filter.sorts = '-invoiceNumber';
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
      //{ prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'paymentNumber', name: 'Order Number' },
      { prop: 'profileName', name: 'Profile' },
      { prop: 'className', name: 'Class' },
      { prop: 'cancelledOn', name: 'Cancelled Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'transactionTime', name: 'Order Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'deliveryDate', name: 'Delivery Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'mealSessionName', name: 'Meal Session' },
      { prop: 'mealDescription', name: 'Meal Description', sortable: false, draggable: false },
      { prop: 'voucherCode', name: 'Voucher' },
      //{ prop: 'quantity', name: 'Qty', sortable: false, draggable: false },
      { prop: 'subtotal', name: 'Subtotal', sortable: false, draggable: false },
      { prop: 'discount', name: 'Discount' },
      //{ prop: 'subDiscTotal', name: 'Subtotal after discount', sortable: false, draggable: false },
      //{ prop: 'paymentGst', name: 'Tax', sortable: false },
      { prop: 'paymentTransactionFee', name: 'T.Fee', sortable: false },
      { prop: 'paymentFixedTransactionFee', name: 'Fixed T.Fee', sortable: false },
      { prop: 'totalAmount', name: 'Total Amount' },
      //{ prop: 'paymentMethod', name: 'Payment Method' },
      //{ prop: 'paymentStatus', name: 'Payment Status' },
      //{ prop: 'status', name: 'Order Status' },
      { prop: 'cancellationReason', name: 'Cancellation Reason' }
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
    this.loadData();
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
    this.filter.filters = '(profileName|processedBy|className|invoiceNumber|voucherCode)@=' + this.keyword + ',(status)==cancelled,(IsActive)==true,(OrderCancelledDateRange)==' + this.start.toDateString() + '|' + this.end.toDateString();

    if (this.isFAS) {
      this.filter.filters += ',(isFas)==true';
    }

    this.filter.reportDateFrom = this.start.toDateString();
    this.filter.reportDateTo = this.end.toDateString();
    this.filter.status = 'cancelled';
    this.filter.keyword = this.keyword;
    this.filter.reportType = SalesOrderReportType.CancellationReport;
    if (this.isFAS) this.filter.isFas = true;

    this.service.getCancelledOrdersByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;
        console.log(this.pagedResult);
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let orderLogs = results.pagedData;

        orderLogs.forEach((orderLog, index, orderLogs) => {
          (<any>orderLog).index = index + 1;
        });


        this.rowsCache = [...orderLogs];
        this.rows = orderLogs;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve orders from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
    }

    if (type == 'end') {
      this.end = new Date(event.value);
    }

  }

  onSearchChanged(value: string) {
    this.keyword = value;
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  clearFilters() {
    this.start = new Date();
    this.end = new Date();
    this.keyword = '';
    this.clearFilterAndPagedResult();
    this.searchbox.clear();
  }

  downloadResults() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_CancelledOrders.xlsx';
    this.filter.page = null;
    this.filter.pageSize = null;
    this.filter.reportDateFrom = this.start.toDateString();
    this.filter.reportDateTo = this.end.toDateString();
    this.filter.status = 'cancelled';
    this.filter.keyword = this.keyword;
    if (this.isFAS) this.filter.isFas = true;
    this.service.downloadCancelledOrdersReport(this.filter).subscribe(
      data => {
        console.log(data);
        saveAs(data, fileName);
      },
      err => {
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }

  downloadFlattentResults() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_OrdersDetails.xlsx';
    this.filter.page = null;
    this.filter.pageSize = null;
    this.filter.reportDateFrom = this.start.toDateString();
    this.filter.reportDateTo = this.end.toDateString();
    this.filter.status = 'cancelled';
    this.filter.keyword = this.keyword;
    if (this.isFAS) this.filter.isFas = true;
    this.service.downloadFlattenCancelledOrdersReport(this.filter).subscribe(
      data => {
        console.log(data);
        saveAs(data, fileName);
      },
      err => {
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }

  get canManageAuthLogs() {
    return true;// this.accountService.userHasPermission(Permission.viewAuthLogsPermission)
  }

}
