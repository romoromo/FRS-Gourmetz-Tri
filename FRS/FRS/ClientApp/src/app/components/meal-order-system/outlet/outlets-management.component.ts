import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ChangeDetectorRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { Outlet } from 'src/app/models/meal-order/outlet.model';
import { OutletEditorComponent } from './outlet-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { MenuService } from '../../../services/meal-order/menu.service';
import { CatererSelectorComponent } from './caterer-selector/caterer-selector.component';
import { MealSessionEditorComponent } from './meal-session/meal-session-editor.component';
import { StoreSelectorComponent } from './store-selector/store-selector.component';
import { OrderReportComponent } from './order-report/order-report.component';
import { OrderDeliveryReportComponent } from './order-delivery-report/order-delivery-report.component';
import { PrintLabelComponent } from './print-label/print-label.component';
import { OrderMealSummaryComponent } from './order-meal-summary/order-meal-summary.component';
import { getBaseUrl } from 'src/app/app.module';

@Component({
  selector: 'outlets-management',
  templateUrl: './outlets-management.component.html',
  styleUrls: ['./outlets-management.component.css']
})
export class OutletsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Outlet[] = [];
  rowsCache: Outlet[] = [];
  allPermissions: Permission[] = [];
  editedOutlet: Outlet;
  sourceOutlet: Outlet;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('outletEditor')
  outletEditor: OutletEditorComponent;
  header: string;

  @ViewChild('infotable') table: any;


  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog, private cdr: ChangeDetectorRef, private menuService: MenuService) {
  }

  openDialog(outlet: Outlet): void {
    const dialogRef = this.dialog.open(OutletEditorComponent, {
      data: { header: this.header, outlet: outlet },
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
      { prop: 'address', name: 'Address' },
      //{ prop: 'outletProfileName', name: 'Outlet Profile Name' },
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
    
    this.deliveryService.getOutletsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let outlets = results.pagedData;

        outlets.forEach((outlet, index, outlets) => {
          (<any>outlet).index = index + 1;
        });


        this.rowsCache = [...outlets];
        this.rows = outlets;

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

  newOutlet() {
    this.header = 'New Outlet';
    this.editedOutlet = new Outlet();
    this.openDialog(this.editedOutlet);
  }


  editOutlet(row: Outlet) {
    this.editedOutlet = row;
    this.header = 'Edit Outlet';
    this.openDialog(this.editedOutlet);
  }

  deleteOutlet(row: Outlet) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" outlet?', DialogType.confirm, () => this.deleteOutletHelper(row));
  }


  deleteOutletHelper(row: Outlet) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteOutlet(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the outlet.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  requestCaterer(row: Outlet) {
    //filter caterer outlets by profile id
    if (!row.catererOutlets) row.catererOutlets = [];
    //let outlets = row.catererOutlets.filter(e => e.outletProfileId == row.outletProfileId);
    //row.catererOutlets = outlets;
    //let catererId = outlets && outlets.length > 0? outlets[0].catererInfoId : '';

    const dialogRef = this.dialog.open(CatererSelectorComponent, {
      data: { header: "Caterers", outlet: row},
      width: '700px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  openMealSession(row) {
    const dialogRef = this.dialog.open(MealSessionEditorComponent, {
      data: { header: "Meal Sessions", outlet: row },
      width: '1200px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  openOutletCalendar(outlet) {
    window.open(getBaseUrl() + '/outletcalendars/' + outlet.id, '_blank');
  }

  openOutletStore(row) {
    const dialogRef = this.dialog.open(StoreSelectorComponent, {
      data: { header: "Add Stores", outlet: row },
      width: '1000px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }


  openOrderReport(row) {
    const dialogRef = this.dialog.open(OrderReportComponent, {
      data: { header: "Order Report", outlet: row },
      width: '800px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  openDeliveryOrderReport(row) {
    const dialogRef = this.dialog.open(OrderDeliveryReportComponent, {
      data: { header: "Production Delivery Report", outlet: row },
      width: '800px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  openMealSummaryReport(row) {
    const dialogRef = this.dialog.open(OrderMealSummaryComponent, {
      data: { header: "Meal Summary Report", outlet: row },
      width: '800px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }


  openPrintLabel(row) {
    const dialogRef = this.dialog.open(PrintLabelComponent, {
      data: { header: "Print Order Label", outlet: row },
      width: '1000px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  openClassBatches() {
    window.open('/classbatches');
  }

  openClassLevels() {
    window.open('/classlevels');
  }

  openClasses() {
    window.open('/classes');
  }

  openStudents() {
    window.open('/students');
  }

  openStaffTypes() {
    window.open('/stafftypes');
  }

  openStaffs() {
    window.open('/staffs');
  }

  openInterestGroups() {
    window.open('/interestgroups');
  }

  toggleExpandRow(row) {
    console.log('Toggled Expand Row!', row);
    this.table.rowDetail.toggleExpandRow(row);
    this.cdr.detectChanges();
  }

  onDetailToggle(event) {
    console.log('Detail Toggled', event);
  }

  get canManageOutlets() {
    return true; //this.accountService.userHasPermission(Permission.manageOutletsPermission)
  }


}
