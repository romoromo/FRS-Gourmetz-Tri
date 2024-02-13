import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult, SalesOrderReportFilter } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { DateOnlyPipe, DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { saveAs } from 'file-saver';
import * as moment from 'moment';
import { TokenOrder } from 'src/app/models/meal-order/token-order.model';
import { StudentService } from 'src/app/services/meal-order/student.service';


@Component({
  selector: 'bento-usage-management',
  templateUrl: './bento-usage-management.component.html',
  styleUrls: ['./bento-usage-management.component.css']
})
export class BentoUsageManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  statuses = ['All', 'pending', 'paid', 'cancelled', 'deleted'];
  ordertypes = ['All', 'Adhoc', 'Meal Plan'];
  columns: any[] = [];
  rows: TokenOrder[] = [];
  rowsCache: TokenOrder[] = [];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  status: string = 'paid';
  ordertype: string = 'All';
  isFAS: boolean = false;
  start = new Date();
  end = new Date();
  groups: any[] = [];

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

  @ViewChild('orderLogTable') table: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, private studentService: StudentService) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = '-usageCount';
    this.filter.filters = '';
    this.filter.page = 1;
  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.filter.page = 1;
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      //{ prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'bentoId', name: 'Bento Id', sortable: false },
      { prop: 'bentoCode', name: 'Bento Code', sortable: false },
      { prop: 'usageCount', name: 'Usage Count' },
    ];
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  ngOnInit() {
    //this.getStudentGroups();
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

    //if (!this.keyword) this.keyword = '';
    //this.filter.filters = '(profileName|processedBy|className|invoiceNumber|voucherCode)@=' + this.keyword + ',(status)==' + (this.status == 'All' ? '' : this.status) + ',(orderType)==' + (this.ordertype == 'All' ? '' : this.ordertype) + ',(AuditOrderLogDateRange)==' + this.start.toDateString() + '|' + this.end.toDateString();

    //if (this.isFAS) {
    //  this.filter.filters += ',(isFas)==true';
    //}

    //this.filter.reportDateFrom = this.start.toDateString();
    //this.filter.reportDateTo = this.end.toDateString();
    //this.filter.status = (this.status == 'All' ? '' : this.status);
    //this.filter.orderType = (this.ordertype == 'All' ? '' : this.ordertype);
    //this.filter.keyword = this.keyword;
    //if (this.isFAS) this.filter.isFas = true;

    this.deliveryService.getBentoUsageByFilter(this.filter)
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

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve order logs from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  //onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
  //  if (type == 'start') {
  //    this.start = new Date(event.value);
  //  }

  //  if (type == 'end') {
  //    this.end = new Date(event.value);
  //  }

  //  if (type == 'tstart') {
  //    this.tstart = new Date(event.value);
  //  }

  //  if (type == 'tend') {
  //    this.tend = new Date(event.value);
  //  }

  //  //this.loadData();

  //  //this.loadData();
  //}

  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    //this.loadData(null);
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

  //downloadResults() {
  //  const fileName = moment().format('DDMMYYYY_hhmmss') + '_Orders.xlsx';
  //  this.filter.page = null;
  //  this.filter.pageSize = null;
  //  this.filter.reportDateFrom = this.start.toDateString();
  //  this.filter.reportDateTo = this.end.toDateString();
  //  this.filter.status = (this.status == 'All' ? '' : this.status);
  //  this.filter.orderType = (this.ordertype == 'All' ? '' : this.ordertype);
  //  this.filter.keyword = this.keyword;
  //  if (this.isFAS) this.filter.isFas = true;
  //  this.orderLogService.downloadOrderLogsReport(this.filter).subscribe(
  //    data => {
  //      console.log(data);
  //      saveAs(data, fileName);
  //    },
  //    err => {
  //      alert("Problem while downloading the file.");
  //      console.error(err);
  //    }
  //  );
  //}

  //downloadFlattentResults() {
  //  const fileName = moment().format('DDMMYYYY_hhmmss') + '_OrdersDetails.xlsx';
  //  this.filter.page = null;
  //  this.filter.pageSize = null;
  //  this.filter.reportDateFrom = this.start.toDateString();
  //  this.filter.reportDateTo = this.end.toDateString();
  //  this.filter.status = (this.status == 'All' ? '' : this.status);
  //  this.filter.orderType = (this.ordertype == 'All' ? '' : this.ordertype);
  //  this.filter.keyword = this.keyword;
  //  if (this.isFAS) this.filter.isFas = true;
  //  this.orderLogService.downloadFlattenOrderLogsReport(this.filter).subscribe(
  //    data => {
  //      console.log(data);
  //      saveAs(data, fileName);
  //    },
  //    err => {
  //      alert("Problem while downloading the file.");
  //      console.error(err);
  //    }
  //  );
  //}

  get canManageAuthLogs() {
    return true;// this.accountService.userHasPermission(Permission.viewAuthLogsPermission)
  }

}
