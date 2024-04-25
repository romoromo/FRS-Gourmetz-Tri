import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { DateAdapter, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { Menu } from 'src/app/models/meal-order/menu.model';
import { MenuGroup } from 'src/app/models/meal-order/menu-group.model';
import { MenuGroupEditorComponent } from './menu-group-editor.component';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { TokenOrdersBulkManagementComponent } from './../token-order-bulk/token-orders-bulk-management.component';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';


@Component({
  selector: 'menu-groups-management',
  templateUrl: './menu-groups-management.component.html',
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-SG' },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ],
  styleUrls: ['./menu-groups-management.component.css']
})
export class MenuGroupsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: MenuGroup[] = [];
  rowsCache: MenuGroup[] = [];
  allPermissions: Permission[] = [];
  editedGroup: MenuGroup;
  sourceGroup: MenuGroup;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  @ViewChild('fileImport')
  fileImport: ElementRef;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('publishTemplate')
  publishTemplate: TemplateRef<any>;

  @ViewChild('studentEditorComponent')
  studentEditorComponent: MenuGroupEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private menuService: MenuService, public dialog: MatDialog) {
  }

  openDialog(group: MenuGroup): void {
    const dialogRef = this.dialog.open(MenuGroupEditorComponent, {
      data: { header: this.header, group: group, outletId: this.outletId },
      width: '1000px',
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
      { prop: 'startDate', name: 'Start Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'endDate', name: 'End Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'isPublished', name: 'Is Published', cellTemplate: this.publishTemplate, },
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
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(Name)@=' + this.keyword;
    
    this.menuService.getMenuGroupsSimpleByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let groups = results.pagedData;

        groups.forEach((group, index, students) => {
          (<any>group).index = index + 1;
        });


        this.rowsCache = [...groups];
        this.rows = groups;

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

  newGroup() {
    this.header = 'New Menu Group';
    this.editedGroup = new MenuGroup();
    this.openDialog(this.editedGroup);
  }


  editGroup(row: MenuGroup) {
    this.editedGroup = row;
    this.header = 'Edit Menu Group';
    this.openDialog(this.editedGroup);
  }

  deleteGroup(row: MenuGroup) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" menu group?', DialogType.confirm, () => this.deleteGroupHelper(row));
  }


  deleteGroupHelper(row: MenuGroup) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.menuService.deleteMenuGroup(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the menu group.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  openTokenOrder(row) {
    const dialogRef = this.dialog.open(TokenOrdersBulkManagementComponent, {
      data: { header: "Meal Order", group: row },
      width: '1200px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if(!result) this.loadData();
    });
  }

  get canManageMenuGroups() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtMenusPermission)
  }

}
