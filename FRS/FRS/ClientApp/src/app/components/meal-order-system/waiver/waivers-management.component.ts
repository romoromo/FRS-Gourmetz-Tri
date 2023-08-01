import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { WaiverEditorComponent } from './waiver-editor.component';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { Waiver } from 'src/app/models/meal-order/waiver.model';
import { PaymentService } from 'src/app/services/meal-order/payment.service';


@Component({
  selector: 'waivers-management',
  templateUrl: './waivers-management.component.html',
  styleUrls: ['./waivers-management.component.css']
})
export class WaiversManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Waiver[] = [];
  rowsCache: Waiver[] = [];
  allPermissions: Permission[] = [];
  editedWaiver: Waiver;
  sourceWaiver: Waiver;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: WaiverEditorComponent;

  @ViewChild('waiversTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private paymentService: PaymentService, public dialog: MatDialog) {
  }

  openDialog(waiver: Waiver): void {
    const dialogRef = this.dialog.open(WaiverEditorComponent, {
      data: { header: this.header, waiver: waiver },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'label';
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
      { prop: 'label', name: 'Label' },
      { prop: 'name', name: 'Description' },
      { prop: 'transactionFeePaymentType', name: 'Payment Type' },
      { prop: 'transactionFeeLabel', name: 'Transaction Fee' },
      { prop: 'amount', name: 'Amount to Reach' },
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
    
    this.paymentService.getWaiversByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let waivers = results.pagedData;

        waivers.forEach((waiver, index, waivers) => {
          (<any>waiver).index = index + 1;
        });


        this.rowsCache = [...waivers];
        this.rows = waivers;

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

  newWaiver() {
    this.header = 'New Waiver';
    this.editedWaiver = new Waiver();
    this.openDialog(this.editedWaiver);
  }


  editWaiver(row: Waiver) {
    this.editedWaiver = row;
    this.header = 'Edit Waiver';
    this.openDialog(this.editedWaiver);
  }

  deleteWaiver(row: Waiver) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" waiver?', DialogType.confirm, () => this.deleteWaiverHelper(row));
  }


  deleteWaiverHelper(row: Waiver) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.paymentService.deleteWaiver(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the waiver.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageWaivers() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtWaiversPermission)
  }

}
