import { Component, OnInit, TemplateRef, ViewChild, Input } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { PaymentService } from 'src/app/services/meal-order/payment.service';


@Component({
  selector: 'vouchers-unassign-management',
  templateUrl: './vouchers-unassign-management.component.html',
  styleUrls: ['./vouchers-unassign-management.component.css']
})
export class VouchersUnassignManagementComponent implements OnInit {
  columns: any[] = [];
  rows: any[] = [];
  rowsCache: any[] = [];
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('vouchersTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private paymentService: PaymentService, public dialog: MatDialog) {
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = '-voucherId';
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
      { prop: 'voucherTypeName', name: 'Type' },
      { prop: 'name', name: 'Name' },
      { prop: 'code', name: 'Code' },
      { prop: 'studentName', name: 'Student' },
      { prop: 'status', name: 'Status' },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];
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
    this.filter.filters = '(Code|studentName)@=' + this.keyword;
    
    this.paymentService.getVoucherStudent(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let dataResponse = results.pagedData;

        dataResponse.forEach((voucher, index, voucherTypes) => {
          (<any>voucher).index = index + 1;
        });


        this.rowsCache = [...dataResponse];
        this.rows = dataResponse;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  onSearchChanged(value: string) {
    this.keyword = value;
  }

  onSearchTriggered(){
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  unAssignVoucher(row: any) {
    this.alertService.showDialog('Are you sure you want to unassign this voucher from the student? the process cannot be undone', DialogType.confirm, () => this.unAssignVoucherHelper(row));
  }


  unAssignVoucherHelper(row: any) {

    this.alertService.startLoadingMessage("Unassigning...");
    this.loadingIndicator = true;

    this.paymentService.deleteVoucherStudent(row.studentVoucherId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;
        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while unassign the voucher.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageVouchers() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtVoucherUnAssignPermission)
  }

}
