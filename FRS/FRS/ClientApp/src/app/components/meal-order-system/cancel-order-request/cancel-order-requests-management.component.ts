import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { CancelOrderRequestFilter, Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { CancelOrderRequestEditorComponent } from './cancel-order-request-editor.component';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { CancelOrderRequest } from 'src/app/models/meal-order/cancel-order-request.model';
import { PaymentService } from 'src/app/services/meal-order/payment.service';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { Subscription } from 'rxjs';


@Component({
  selector: 'cancel-order-requests-management',
  templateUrl: './cancel-order-requests-management.component.html',
  styleUrls: ['./cancel-order-requests-management.component.css']
})
export class CancelOrderRequestsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: CancelOrderRequest[] = [];
  rowsCache: CancelOrderRequest[] = [];
  allPermissions: Permission[] = [];
  editedCancelOrderRequest: CancelOrderRequest;
  sourceCancelOrderRequest: CancelOrderRequest;
  loadingIndicator: boolean;
  filter: CancelOrderRequestFilter;
  pagedResult: PagedResult;
  keyword: string = '';
  fStatusVal: string = '';
  limitPerPagArr = [{
    value: 10,
    label: '10'
  },
  {
    value: 20,
    label: '20'
  },
  {
    value: 50,
    label: '50'
  },
  {
    value: 100,
    label: '100'
  }];
  limitPerPage: number = 10;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: CancelOrderRequestEditorComponent;

  @ViewChild('cancelOrderRequestsTable') table: any;

  header: string;
  @Input() isHideHeader: boolean;

  private subscription: Subscription = new Subscription();
  outletId: string = "";
  outlets: any[] = [];

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private paymentService: PaymentService, public dialog: MatDialog,
    private deliveryService: DeliveryService) {
  }

  openDialog(cancelOrderRequest: CancelOrderRequest, isDisabled: boolean): void {
    const dialogRef = this.dialog.open(CancelOrderRequestEditorComponent, {
      data: { header: this.header, cancelOrderRequest: cancelOrderRequest, isDisabled: isDisabled },
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new CancelOrderRequestFilter(1, 10);
    this.filter.sorts = 'id';
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
      { prop: 'userName', name: 'Name' },
      { prop: 'studentName', name: 'Student' },
      { prop: 'cancellationDate', name: 'Order Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'mealSessionName', name: 'Session' },
      { prop: 'submissionDate', name: 'Submission Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'reason', name: 'Reason' },
      //  { prop: 'note', name: 'Note' },
      { prop: 'status', name: 'Status' },
      { prop: 'response', name: 'Response' },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];
  }

  ngOnInit() {
    this.getOutlet();
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.loadData();
  }

  getOutlet() {    
    let filter = new Filter();
    filter.filters = `(IsActive)==true`;
    this.subscription.add(
      this.deliveryService.getOutletsSimpleByFilter(filter).subscribe(
        (results) => {
          this.outlets = results.pagedData;
        },
        (error) => {
          this.alertService.showStickyMessage(
            "Get Error",
            `An error occured while retrieving student outlets.\r\n"`,
            MessageSeverity.error
          );
        }
      )
    );
  }

  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = this.limitPerPage;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    //if (!this.fStatusVal) this.fStatusVal = '';
    this.filter.filters = '(Status)==' + this.fStatusVal + ',(IsActive)==true,(userName|studentName)@=' + this.keyword;

    if(this.outletId)
      this.filter.catererInfoId = this.outletId;

    this.paymentService.getCancelOrderRequestsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let cancelOrderRequests = results.pagedData;

        cancelOrderRequests.forEach((cancelOrderRequest, index, cancelOrderRequests) => {
          (<any>cancelOrderRequest).index = index + 1;
        });


        this.rowsCache = [...cancelOrderRequests];
        this.rows = cancelOrderRequests;

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

  onEnterSearchPress() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }


  approvalCancelOrderRequest(row: CancelOrderRequest) {
    this.editedCancelOrderRequest = row;
    this.header = 'Cancel Order Request Approval';
    this.openDialog(this.editedCancelOrderRequest, row.status != 'Submitted');
  }

  get canManageCancelOrderRequests() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCancelRequestsPermission)
  }

}
