import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy, ChangeDetectorRef, ViewEncapsulation, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { MatDatepickerInputEvent, MatDialog } from '@angular/material';
import { AuditService } from 'src/app/services/audit.service';
import { DateTimeOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';
import { saveAs } from 'file-saver';
import * as moment from 'moment';
import { TokenOrder, NewOrder } from 'src/app/models/meal-order/token-order.model';
import { OrderService } from 'src/app/services/meal-order/order.service';
import { OrderCancellationFilter, PagedResult, StudentOrderFilter } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { SearchBoxComponent } from 'src/app/components/controls/search-box.component';
import { Utilities } from 'src/app/services/utilities';
import { DatatableComponent } from '@swimlane/ngx-datatable';
import { StudentOrderDetailComponent } from './student-order-details.component';
import { StudentOrderEditorComponent } from './student-order-editor.component';


@Component({
  selector: 'student-orders-management',
  templateUrl: './student-orders-management.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: ['./student-orders-management.component.css']
})
export class StudentOrderManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  statuses = [
    { name: 'All', val: '' },
    { name: 'Pending', val: 'pending' },
    { name: 'Paid', val: 'paid' },
    { name: 'Cancelled', val: 'cancelled' },
    //{ name: 'Deleted', val: 'deleted' }
  ];

  editing = {};
  beforeEditingRow = {};
  selectAll = false;
  columns: any[] = [];
  rows: TokenOrder[] = [];
  rowsCache: TokenOrder[] = [];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: StudentOrderFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  status: string = 'paid';
  isFAS: boolean = false;
  start?: Date = null;
  end?: Date = null;
  name: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;
  @Input() studentId: string;
  @Input() studentName: string;
  @Input() studentEmail: string;

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

  @ViewChild('mealDescriptionTemplate')
  mealDescriptionTemplate: TemplateRef<any>;

  @ViewChild('orderTable') table: any;

  @ViewChild(DatatableComponent) dataTable: DatatableComponent

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private orderService: OrderService, private cdr: ChangeDetectorRef, public dialog: MatDialog) {
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new StudentOrderFilter(1, 10);
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
      //{ prop: 'className', name: 'Class' },
      //{ prop: 'profileName', name: 'Profile' },
      //{ prop: 'isFASDisplay', name: 'FAS', sortable: false },
      { prop: 'deliveryDate', name: 'Delivery Date', pipe: new DateTimeOnlyPipe('en-SG')},
      { prop: 'transactionTime', name: 'Date', pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'mealSessionDetailName', name: 'Session' },
      { name: 'Meal Description', cellTemplate: this.mealDescriptionTemplate, sortable: false, draggable: false },
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
    this.name = this.studentName;
    //this.loadData();
  }

  ngAfterViewInit() {
    //In this case I need the component instance for edit the with style inside a ngmodal issue 
    //this.dataTable.element.querySelector('.datatable-scroll').setAttribute('style', 'width:100%');
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
    this.filter.filters = '(invoiceNumber|paymentNumber)@=' + this.keyword;

    if (this.start && this.end) {
      this.filter.filters = this.filter.filters + ',(AuditOrderLogDateRange)==' + this.start.toDateString() + '|' + this.end.toDateString();
    }

    if (this.isFAS) {
      this.filter.filters += ',(isFas)==true';
    }

    if (this.status) {
      this.filter.filters += `,(status)==${this.status}`;
    }

    this.filter.outletId = this.outletId;
    this.filter.studentId = this.studentId;
    this.orderService.getStudentOrdersByFilter(this.filter)
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
        this.rows.forEach(row => {
          this.editing[row.id] = false;
          this.beforeEditingRow[row.id] = {};
          Object.assign(this.beforeEditingRow[row.id], row);
        });

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
      const exists = this.selected.filter(item => item.status == 'cancelled');
      if (exists && exists.length > 0) {
        this.alertService.showMessage(`At least one order has status 'Cancelled'`, '', MessageSeverity.info);
        return;
      }
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

  toggleExpandRow(row) {
    console.log('Toggled Expand Row!', row);
    this.table.rowDetail.toggleExpandRow(row);
    this.cdr.detectChanges();
  }

  onDetailToggle(event) {
    console.log('Detail Toggled', event);
  }

  startEditing(row) {
    //Object.assign(this.beforeEditingRow[row.id], row);
    this.editing[row.id] = true;
  }

  cancelEditing(row) {
    this.editing[row.id] = false;
    Object.assign(row, this.beforeEditingRow[row.id]);
  }

  saveRow(row) {
    this.editing[row.id] = false;
    //delete this.beforeEditingRow[row.id];

    if (row) {
      this.alertService.showDialog('Are you sure you want to amend this order?', DialogType.confirm, () => this.amendOrderHelper(row));
    } 
  }


  amendOrderHelper(row) {

    this.alertService.showDialog('Enter reason for updating', DialogType.prompt, (val) => {
      if (!val) return;
      this.alertService.startLoadingMessage("Amending order...");
      this.loadingIndicator = true;
      let model = {
        id: row.id,
        updatedById: this.accountService.currentUser.id,
        reason: val,
        status: row.status,
        invoiceNumber: row.invoiceNumber,
        fomoId: row.fomoId
      }
      this.subscription.add(this.orderService.amendOrder(model)
        .subscribe(results => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showMessage('Success', 'Orders successfully updated.', MessageSeverity.success);
          this.onSearch();
        },
          error => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;

            this.alertService.showStickyMessage("Amend Error", `An error occured while updating the orders\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
              MessageSeverity.error);
          }));
    }, () => {
    });


  }



  selectAllRows() {
    // Toggle the selection state of all rows
    this.rows.forEach(row => (row.selected = this.selectAll));
  }

  selectRow(row) {
    // Update the "Select All" checkbox state based on individual row selection
    this.selectAll = this.rows.every(row => row.selected);
  }

  onStatusChange(row, newStatus) {
    // Handle the status change for the specified row
    row.status = newStatus;
  }

  addOrder(): void {
    let order: NewOrder = new NewOrder();
    order.transactionTime = new Date();
    order.profileId = this.studentId;
    order.studentName = this.studentName;
    order.studentEmail = this.studentEmail;
    order.deliveryDate = new Date();
    order.quantity = 1;
    order.createdBy = this.accountService.currentUser.id;
    order.processedBy = this.accountService.currentUser.userName;
    order.outletId = this.outletId;
    const dialogRef = this.dialog.open(StudentOrderEditorComponent, {
      data: { order: order },
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      //this.loadData(null);
    });
  }

  viewOrder(order: TokenOrder): void {
    const dialogRef = this.dialog.open(StudentOrderDetailComponent, {
      data: { order: order },
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      //this.loadData(null);
    });
  }

  get canManageAuthLogs() {
    return true;// this.accountService.userHasPermission(Permission.viewAuthLogsPermission)
  }

}

@Component({
  selector: 'app-status-dropdown',
  template: `
    <select class="form-control" [(ngModel)]="selectedStatus" (change)="onStatusChange()">
      <option value="pending">Pending</option>
      <option value="paid">Paid</option>
      <option value="cancelled">Cancelled</option>
      <option value="deleted">Deleted</option>
    </select>
  `,
})
export class StatusDropdownComponent {
  @Input() selectedStatus: string;
  @Output() statusChanged = new EventEmitter<string>();

  onStatusChange() {
    this.statusChanged.emit(this.selectedStatus);
  }
}
