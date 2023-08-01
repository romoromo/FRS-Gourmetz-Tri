import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { TransactionFeeEditorComponent } from './transaction-fee-editor.component';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { PaymentService } from 'src/app/services/meal-order/payment.service';
import { TransactionFee } from 'src/app/models/meal-order/transaction-fee.model';


@Component({
  selector: 'transaction-fees-management',
  templateUrl: './transaction-fees-management.component.html',
  styleUrls: ['./transaction-fees-management.component.css']
})
export class TransactionFeesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: TransactionFee[] = [];
  rowsCache: TransactionFee[] = [];
  allPermissions: Permission[] = [];
  editedTransactionFee: TransactionFee;
  sourceTransactionFee: TransactionFee;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('chargeByTemplate')
  chargeByTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: TransactionFeeEditorComponent;

  @ViewChild('transactionFeesTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private paymentService: PaymentService, public dialog: MatDialog) {
  }

  openDialog(transactionFee: TransactionFee): void {
    const dialogRef = this.dialog.open(TransactionFeeEditorComponent, {
      data: { header: this.header, transactionFee: transactionFee },
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
      { prop: 'paymentTypeName', name: 'Payment Type' },
      { prop: 'name', name: 'Name' },
      { name: 'Charge by', cellTemplate: this.chargeByTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false },
      { prop: 'amount', name: 'Amount/Percentage' },
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
    
    this.paymentService.getTransactionFeesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let transactionFees = results.pagedData;

        transactionFees.forEach((transactionFee, index, transactionFees) => {
          (<any>transactionFee).index = index + 1;
        });


        this.rowsCache = [...transactionFees];
        this.rows = transactionFees;

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

  newTransactionFee() {
    this.header = 'New Transaction Fee';
    this.editedTransactionFee = new TransactionFee();
    this.openDialog(this.editedTransactionFee);
  }


  editTransactionFee(row: TransactionFee) {
    this.editedTransactionFee = row;
    this.header = 'Edit Transaction Fee';
    this.openDialog(this.editedTransactionFee);
  }

  deleteTransactionFee(row: TransactionFee) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" ?', DialogType.confirm, () => this.deleteTransactionFeeHelper(row));
  }


  deleteTransactionFeeHelper(row: TransactionFee) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.paymentService.deleteTransactionFee(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageTransactionFees() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtTransactionFeesPermission)
  }

}
