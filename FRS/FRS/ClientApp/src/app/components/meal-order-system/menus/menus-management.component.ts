import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { Menu } from 'src/app/models/meal-order/menu.model';
import { MenuEditorComponent } from './menu-editor.component';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';


@Component({
  selector: 'menus-management',
  templateUrl: './menus-management.component.html',
  styleUrls: ['./menus-management.component.css']
})
export class MenusManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Menu[] = [];
  rowsCache: Menu[] = [];
  allPermissions: Permission[] = [];
  editedMenu: Menu;
  sourceMenu: Menu;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  @Input() isHideHeader: boolean;
  @Input() catererId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('menuEditor')
  menuEditor: MenuEditorComponent;

  @ViewChild('menusTable') table: any;

  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private menuService: MenuService, public dialog: MatDialog) {
  }

  openDialog(menu: Menu): void {
    const dialogRef = this.dialog.open(MenuEditorComponent, {
      data: { header: this.header, menu: menu, catererId: this.catererId },
      width: '900px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'code';
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
      { prop: 'label', name: 'Label' },
      { prop: 'menuDishNames', name: 'Dishes', sortable: false },
      { prop: 'cuisineName', name: 'Cuisine' },
      { prop: 'startDate', name: 'Start Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'endDate', name: 'End Date', pipe: new DateOnlyPipe('en-SG') },
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
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(Code|Label)@=' + this.keyword;
    
    this.menuService.getMenusByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let menus = results.pagedData;

        menus.forEach((menu, index, menus) => {
          (<any>menu).index = index + 1;
        });


        this.rowsCache = [...menus];
        this.rows = menus;

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

  newMenu() {
    this.header = 'New Menu';
    this.editedMenu = new Menu();
    this.openDialog(this.editedMenu);
  }


  editMenu(row: Menu) {
    this.editedMenu = row;
    this.header = 'Edit Menu';
    this.editedMenu.catererId = this.catererId;
    this.openDialog(this.editedMenu);
  }

  deleteMenu(row: Menu) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" menu?', DialogType.confirm, () => this.deleteMenuHelper(row));
  }


  deleteMenuHelper(row: Menu) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.menuService.deleteMenu(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the menu.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageMenus() {
    return true; //this.accountService.userHasPermission(Permission.manageMenusPermission)
  }

}
