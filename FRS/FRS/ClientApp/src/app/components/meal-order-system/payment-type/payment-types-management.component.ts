import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { PaymentTypeEditorComponent } from './payment-type-editor.component';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { PaymentType } from 'src/app/models/meal-order/payment-type.model';
import { PaymentService } from 'src/app/services/meal-order/payment.service';


@Component({
  selector: 'payment-types-management',
  templateUrl: './payment-types-management.component.html',
  styleUrls: ['./payment-types-management.component.css']
})
export class PaymentTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: PaymentType[] = [];
  rowsCache: PaymentType[] = [];
  allPermissions: Permission[] = [];
  editedPaymentType: PaymentType;
  sourcePaymentType: PaymentType;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: PaymentTypeEditorComponent;

  @ViewChild('paymentTypesTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private paymentService: PaymentService, public dialog: MatDialog) {
  }

  openDialog(paymentType: PaymentType): void {
    const dialogRef = this.dialog.open(PaymentTypeEditorComponent, {
      data: { header: this.header, paymentType: paymentType },
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
    
    this.paymentService.getPaymentTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let paymentTypes = results.pagedData;

        paymentTypes.forEach((paymentType, index, paymentTypes) => {
          (<any>paymentType).index = index + 1;
        });


        this.rowsCache = [...paymentTypes];
        this.rows = paymentTypes;

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

  newPaymentType() {
    this.header = 'New Payment Type';
    this.editedPaymentType = new PaymentType();
    this.openDialog(this.editedPaymentType);
  }


  editPaymentType(row: PaymentType) {
    this.editedPaymentType = row;
    this.header = 'Edit Payment Type';
    this.openDialog(this.editedPaymentType);
  }

  deletePaymentType(row: PaymentType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" payment type?', DialogType.confirm, () => this.deletePaymentTypeHelper(row));
  }


  deletePaymentTypeHelper(row: PaymentType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.paymentService.deletePaymentType(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the payment type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManagePaymentTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtPaymentTypesPermission)
  }

}
