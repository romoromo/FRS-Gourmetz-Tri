import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { StoreInfoEditorComponent } from './store-info-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { DOSelectorComponent } from './do-selector/do-selector.component';
import { InventorySelectorComponent } from './inventory-selector/inventory-selector.component';


@Component({
  selector: 'store-infos-management',
  templateUrl: './store-infos-management.component.html',
  styleUrls: ['./store-infos-management.component.css']
})
export class StoreInfosManagementComponent implements OnInit {
  columns: any[] = [];
  rows: StoreInfo[] = [];
  rowsCache: StoreInfo[] = [];
  allPermissions: Permission[] = [];
  editedStoreInfo: StoreInfo;
  sourceStoreInfo: StoreInfo;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('storeInfoEditor')
  storeInfoEditor: StoreInfoEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog) {
  }

  openDialog(storeInfo: StoreInfo): void {
    const dialogRef = this.dialog.open(StoreInfoEditorComponent, {
      data: { header: this.header, storeInfo: storeInfo },
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
      { prop: 'code', name: 'Code' },
      { prop: 'name', name: 'Name' },
      { prop: 'address', name: 'Address' },
      { prop: 'storeType', name: 'Store Type' },
      { prop: 'catererInfoName', name: 'Caterer' },
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
    this.filter.filters = '(IsActive)==true,(Name)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.deliveryService.getStoreInfosByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let storeInfos = results.pagedData;

        storeInfos.forEach((storeInfo, index, storeInfos) => {
          (<any>storeInfo).index = index + 1;
        });


        this.rowsCache = [...storeInfos];
        this.rows = storeInfos;

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

  newStoreInfo() {
    this.header = 'New Store Info';
    this.editedStoreInfo = new StoreInfo();
    this.openDialog(this.editedStoreInfo);
  }


  editStoreInfo(row: StoreInfo) {
    this.editedStoreInfo = row;
    this.header = 'Edit Store Info';
    this.openDialog(this.editedStoreInfo);
  }

  deleteStoreInfo(row: StoreInfo) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" Store Info?', DialogType.confirm, () => this.deleteStoreInfoHelper(row));
  }


  deleteStoreInfoHelper(row: StoreInfo) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteStoreInfo(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the Store Info.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageStoreInfos() {
    return true; //this.accountService.userHasPermission(Permission.manageStoreInfosPermission)
  }

  receiveDO(row) {
    const dialogRef = this.dialog.open(DOSelectorComponent, {
      data: { header: "Delivery Order List", store: row },
      width: '500px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  stock(row) {
    const dialogRef = this.dialog.open(InventorySelectorComponent, {
      data: { header: "Stock List", store: row },
      width: '1000px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

}
