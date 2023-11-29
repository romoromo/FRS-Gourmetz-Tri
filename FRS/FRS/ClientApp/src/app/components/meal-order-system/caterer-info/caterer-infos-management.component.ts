import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ChangeDetectorRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { CatererInfo } from 'src/app/models/meal-order/caterer-info.model';
import { CatererInfoEditorComponent } from './caterer-info-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StaffService } from '../../../services/meal-order/staff.service';
import { DeliveryService } from '../../../services/meal-order/delivery.service';
import { CatererApprovalComponent } from './caterer-selector/caterer-selector.component';
import { OutletProfilesManagementComponent } from '../outlet-profile/outlet-profiles-management.component';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { getBaseUrl } from 'src/app/app.module';


@Component({
  selector: 'caterer-infos-management',
  templateUrl: './caterer-infos-management.component.html',
  styleUrls: ['./caterer-infos-management.component.css']
})
export class CatererInfosManagementComponent implements OnInit {
  columns: any[] = [];
  rows: CatererInfo[] = [];
  rowsCache: CatererInfo[] = [];
  allPermissions: Permission[] = [];
  editedCatererInfo: CatererInfo;
  sourceCatererInfo: CatererInfo;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('catererInfoEditor')
  catererInfoEditor: CatererInfoEditorComponent;
  header: string;

  @ViewChild('infotable') table: any;
  @ViewChild('outletProfileTab') private outletProfileComponent: OutletProfilesManagementComponent;
  expanded: any = {};

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private deliveryService: DeliveryService, public dialog: MatDialog, private cdr: ChangeDetectorRef, private configurationService: ConfigurationService) {
  }

  openDialog(catererInfo: CatererInfo): void {
    const dialogRef = this.dialog.open(CatererInfoEditorComponent, {
      data: { header: this.header, catererInfo: catererInfo },
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
      { prop: 'code', name: 'Code' },
      { name: '', width: 350, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
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
    this.filter.filters = `(IsActive)==true,(Name)@=${this.keyword},(InstitutionId)==${this.accountService.currentUser.institutionId},(CatererByUserId)==${this.accountService.currentUser.id}`;
    
    this.deliveryService.getCatererInfosByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let catererInfos = results.pagedData;

        catererInfos.forEach((catererInfo, index, catererInfos) => {
          (<any>catererInfo).index = index + 1;
        });


        this.rowsCache = [...catererInfos];
        this.rows = catererInfos;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  approveOutlet(row) {
    const dialogRef = this.dialog.open(CatererApprovalComponent, {
      data: { header: "Approve Outlet", caterer: row },
      width: '700px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
  }

  newCatererInfo() {
    this.header = 'New Caterer';
    this.editedCatererInfo = new CatererInfo();
    this.openDialog(this.editedCatererInfo);
  }


  editCatererInfo(row: CatererInfo) {
    this.editedCatererInfo = row;
    this.header = 'Edit Caterer';
    this.openDialog(this.editedCatererInfo);
  }

  deleteCatererInfo(row: CatererInfo) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" caterer?', DialogType.confirm, () => this.deleteCatererInfoHelper(row));
  }


  deleteCatererInfoHelper(row: CatererInfo) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.deliveryService.deleteCatererInfo(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the caterer.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
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

  openCalendar(row) {
    window.open(getBaseUrl() + '/menucyclecalendars/' + row.id, '_blank');
  }

  openDishCalendar(row) {
    window.open(getBaseUrl() + '/dishcyclecalendars/' + row.id, '_blank');
  }

  get canManageCatererInfos() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCaterersPermission)
  }

  get canViewCatererInfos() {
    return this.accountService.userHasPermission(Permission.viewMOSOrderMgtCaterersPermission)
  }

  get canViewDishCalendar() {
    return this.accountService.userHasPermission(Permission.viewMOSOrderMgtCatererDishCalendarMenu)
  }

  get canManageCatererOutlets() {
    return this.accountService.userHasPermission(Permission.viewMOSOrderMgtCatererOutletsMenu)
  }

  get canViewOutletProfiles() {
    return this.accountService.userHasPermission(Permission.viewMOSOrderMgtOutletProfilesPermission)
  }

  get canViewMealTypes() {
    return this.accountService.userHasPermission(Permission.viewMOSOrderMgtCatererMealTypesMenu)
  }

  get canViewMealPeriods() {
    return this.accountService.userHasPermission(Permission.viewMOSOrderMgtCatererMealPeriodsMenu)
  }

  get canViewDishTypes() {
    return this.accountService.userHasPermission(Permission.viewMOSOrderMgtCatererDishTypesMenu)
  }

  get canViewDishes() {
    return this.accountService.userHasPermission(Permission.viewMOSOrderMgtCatererDishesMenu)
  }

  get canViewDishCycles() {
    return this.accountService.userHasPermission(Permission.viewMOSOrderMgtCatererDishCyclesMenu)
  }
}
