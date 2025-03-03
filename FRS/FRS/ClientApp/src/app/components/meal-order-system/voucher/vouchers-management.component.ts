import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { VoucherEditorComponent } from './voucher-editor.component';
import { Voucher } from 'src/app/models/meal-order/voucher.model';
import { PaymentService } from 'src/app/services/meal-order/payment.service';
import { DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';


@Component({
  selector: 'vouchers-management',
  templateUrl: './vouchers-management.component.html',
  styleUrls: ['./vouchers-management.component.css']
})
export class VouchersManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Voucher[] = [];
  rowsCache: Voucher[] = [];
  allPermissions: Permission[] = [];
  editedVoucher: Voucher;
  sourceVoucher: Voucher;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: VoucherEditorComponent;

  @ViewChild('vouchersTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private paymentService: PaymentService, public dialog: MatDialog) {
  }

  openDialog(voucher: Voucher): void {
    const dialogRef = this.dialog.open(VoucherEditorComponent, {
      data: { header: this.header, voucher: voucher },
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'name';
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
      { prop: 'startDateTime', name: 'Start', pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'endDateTime', name: 'End', pipe: new DateTimeOnlyPipe('en-SG') },
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
    this.filter.filters = '(IsActive)==true,(Name|Code)@=' + this.keyword;
    
    this.paymentService.getVouchersByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let voucherTypes = results.pagedData;

        voucherTypes.forEach((voucher, index, voucherTypes) => {
          (<any>voucher).index = index + 1;
        });


        this.rowsCache = [...voucherTypes];
        this.rows = voucherTypes;

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
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  newVoucher() {
    this.header = 'New Voucher';
    this.editedVoucher = new Voucher();
    this.openDialog(this.editedVoucher);
  }


  editVoucher(row: Voucher) {
    this.editedVoucher = row;
    this.header = 'Edit Voucher';
    this.openDialog(this.editedVoucher);
  }

  deleteVoucher(row: Voucher) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" voucher?', DialogType.confirm, () => this.deleteVoucherHelper(row));
  }


  deleteVoucherHelper(row: Voucher) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.paymentService.deleteVoucher(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the voucher.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageVouchers() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtVouchersPermission)
  }

}
