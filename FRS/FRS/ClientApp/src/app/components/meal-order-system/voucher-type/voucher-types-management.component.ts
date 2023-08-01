import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { VoucherTypeEditorComponent } from './voucher-type-editor.component';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { VoucherType } from 'src/app/models/meal-order/voucher-type.model';
import { PaymentService } from 'src/app/services/meal-order/payment.service';


@Component({
  selector: 'voucher-types-management',
  templateUrl: './voucher-types-management.component.html',
  styleUrls: ['./voucher-types-management.component.css']
})
export class VoucherTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: VoucherType[] = [];
  rowsCache: VoucherType[] = [];
  allPermissions: Permission[] = [];
  editedVoucherType: VoucherType;
  sourceVoucherType: VoucherType;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: VoucherTypeEditorComponent;

  @ViewChild('voucherTypesTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private paymentService: PaymentService, public dialog: MatDialog) {
  }

  openDialog(voucherType: VoucherType): void {
    const dialogRef = this.dialog.open(VoucherTypeEditorComponent, {
      data: { header: this.header, voucherType: voucherType },
      width: '400px',
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
      { prop: 'name', name: 'Name' },
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
    this.filter.filters = '(IsActive)==true,(Name)@=' + this.keyword;
    
    this.paymentService.getVoucherTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let voucherTypes = results.pagedData;

        voucherTypes.forEach((voucherType, index, voucherTypes) => {
          (<any>voucherType).index = index + 1;
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

  newVoucherType() {
    this.header = 'New Voucher Type';
    this.editedVoucherType = new VoucherType();
    this.openDialog(this.editedVoucherType);
  }


  editVoucherType(row: VoucherType) {
    this.editedVoucherType = row;
    this.header = 'Edit Voucher Type';
    this.openDialog(this.editedVoucherType);
  }

  deleteVoucherType(row: VoucherType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" voucher type?', DialogType.confirm, () => this.deleteVoucherTypeHelper(row));
  }


  deleteVoucherTypeHelper(row: VoucherType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.paymentService.deleteVoucherType(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the voucher type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageVoucherTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtVoucherTypesPermission)
  }

}
