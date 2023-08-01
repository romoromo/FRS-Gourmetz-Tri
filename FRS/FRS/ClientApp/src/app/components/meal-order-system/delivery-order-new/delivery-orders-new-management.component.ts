import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { DeliveryOrderNew } from 'src/app/models/meal-order/delivery-order-new.model';
import { DeliveryOrderNewEditorComponent } from './delivery-order-new-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { saveAs } from 'file-saver';


@Component({
  selector: 'delivery-orders-new-management',
  templateUrl: './delivery-orders-new-management.component.html',
  styleUrls: ['./delivery-orders-new-management.component.css']
})
export class DeliveryOrdersNewManagementComponent implements OnInit {
  columns: any[] = [];
  rows: DeliveryOrderNew[] = [];
  rowsCache: DeliveryOrderNew[] = [];
  allPermissions: Permission[] = [];
  editedDeliveryOrder: DeliveryOrderNew;
  sourceDeliveryOrder: DeliveryOrderNew;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('fasTemplate')
  fasTemplate: TemplateRef<any>;

  @ViewChild('deliveryOrderEditor')
  deliveryOrderEditor: DeliveryOrderNewEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(deliveryOrder: DeliveryOrderNew): void {
    const dialogRef = this.dialog.open(DeliveryOrderNewEditorComponent, {
      data: { header: this.header, deliveryOrder: deliveryOrder },
      width: '90vw',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'doNumber';
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
      { prop: 'doNumber', name: 'DO Number' },
      { prop: 'fromStoreName', name: 'From' },
      { prop: 'toStoreName', name: 'To' },
      { name: 'Status', cellTemplate: this.fasTemplate },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
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
    this.filter.filters = '(IsActive)==true,(DONumber)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.deliveryService.getDeliveryOrderNewsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let deliveryOrders = results.pagedData;

        deliveryOrders.forEach((deliveryOrder, index, deliveryOrders) => {
          (<any>deliveryOrder).index = index + 1;
        });


        this.rowsCache = [...deliveryOrders];
        this.rows = deliveryOrders;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
  }

  newDeliveryOrder() {
    this.header = 'New Delivery order';
    this.editedDeliveryOrder = new DeliveryOrderNew();
    this.openDialog(this.editedDeliveryOrder);
  }


  editDeliveryOrder(row: DeliveryOrderNew) {
    this.editedDeliveryOrder = row;
    this.header = 'Edit Delivery order';
    this.openDialog(this.editedDeliveryOrder);
  }

  printDeliveryOrder(row: DeliveryOrderNew) {
    const fileName = row.doNumber + '_Print.pdf';

    this.deliveryService.printDo(row.id).subscribe(
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

  printCCLabel(row: DeliveryOrderNew) {
    const fileName = row.doNumber + '_CartonContentLabel.pdf';

    this.deliveryService.printCCLabel(row.id).subscribe(
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

  deleteDeliveryOrder(row: DeliveryOrderNew) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.doNumber + '\" Delivery order?', DialogType.confirm, () => this.deleteDeliveryOrderHelper(row));
  }


  deleteDeliveryOrderHelper(row: DeliveryOrderNew) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteDeliveryOrderNew(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Delivery order.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageDeliveryOrders() {
    return true; //this.accountService.userHasPermission(Permission.manageDeliveryOrdersPermission)
  }

}
