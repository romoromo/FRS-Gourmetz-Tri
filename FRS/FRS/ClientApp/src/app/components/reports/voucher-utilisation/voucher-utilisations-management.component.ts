import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { PagedResult, VoucherUtilisationReportFilter } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { DateOnlyPipe, DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { SearchBoxComponent } from '../../controls/search-box.component';
import { saveAs } from 'file-saver';
import * as moment from 'moment';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { VoucherUsageStatus } from '../../../models/enums';
import { FormControl } from '@angular/forms';
import { VoucherUtilisation } from '../../../models/meal-order/voucher.model';
import { ReportService } from '../../../services/report.service';


@Component({
  selector: 'voucher-utilisation-management',
  templateUrl: './voucher-utilisations-management.component.html',
  styleUrls: ['./voucher-utilisations-management.component.css']
})
export class VoucherUtilisationsReportManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  statuses = ['All', VoucherUsageStatus.Expired, VoucherUsageStatus.NotUsed, VoucherUsageStatus.Used];
  columns: any[] = [];
  rows: VoucherUtilisation[] = [];
  rowsCache: VoucherUtilisation[] = [];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: VoucherUtilisationReportFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  status: string = '';
  ordertype: string = 'All';
  isFAS: boolean = false;
  start = new Date();
  end = new Date();
  groups: any[] = [];

  tstart = new Date();
  tend = new Date();
  studentGroupIds = new FormControl();

  public currentPageLimit: number = 10;
  public pageLimitOptions = [
    { value: 5 },
    { value: 10 },
    { value: 25 },
    { value: 50 },
    { value: 100 },
  ];

  @ViewChild('searchbox') searchbox: SearchBoxComponent;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('#voucherUtilisationTable') table: any;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private reportService: ReportService, private studentService: StudentService) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new VoucherUtilisationReportFilter(1, 10);
    this.filter.sorts = '-invoiceNumber';
    this.filter.filters = '';
    this.filter.page = 1;
    this.status = '';
    this.ordertype = '';
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

      { prop: 'voucherName', name: 'Voucher Name' },
      { prop: 'voucherCode', name: 'Voucher Code' },
      { prop: 'voucherAmount', name: 'Value' },
      { prop: 'discountType', name: 'Discount Type' },
      { prop: 'validityStartDate', name: 'Validity Start Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'validityEndDate', name: 'Validity End Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'studentName', name: 'Name' },
      { prop: 'className', name: 'Class' },
      { prop: 'utilisedDate', name: 'Utilized Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'invoiceNumber', name: 'InvoiceNumber' },
      { prop: 'discount', name: 'Amount' },
      { prop: 'voucherStatus', name: 'Voucher Status' }
    ];
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  public onLimitChange(limit: any): void {
    this.changePageLimit(limit);
    this.table.limit = this.currentPageLimit;
    this.table.recalculate();
    setTimeout(() => {
      if (this.table.bodyComponent.temp.length <= 0) {
        this.table.offset = Math.floor((this.table.rowCount - 1) / this.table.limit);
      }
    });
  }

  private changePageLimit(limit: any): void {
    this.currentPageLimit = parseInt(limit, 10);
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
    this.filter.pageSize = this.currentPageLimit;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(studentName|className|invoiceNumber|voucherCode)@=' + this.keyword + ',(IsActive)==true,(status)==' + (this.status == 'All' ? '' : this.status);

    this.filter.reportDateFrom = this.start.toDateString();
    this.filter.reportDateTo = this.end.toDateString();
    this.filter.status = (this.status == 'All' ? '' : this.status);
    this.filter.keyword = this.keyword;

    this.reportService.getVoucherUtilisationsByFilter(this.filter)
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

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve vouchers from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
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

    if (type == 'tstart') {
      this.tstart = new Date(event.value);
    }

    if (type == 'tend') {
      this.tend = new Date(event.value);
    }

  }

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

  downloadResults() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_VoucherUtilisationReport.xlsx';
    this.filter.page = null;
    this.filter.pageSize = null;
    this.filter.reportDateFrom = this.start.toDateString();
    this.filter.reportDateTo = this.end.toDateString();
    this.filter.status = (this.status == 'All' ? '' : this.status);
    this.filter.keyword = this.keyword;
    this.reportService.downloadVoucherUtilisationssReport(this.filter).subscribe(
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
    return this.accountService.userHasPermission(Permission.viewMOSOutletMgtReportsVoucherUtilisationPermission)
  }

}
