import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { MenuCycle } from 'src/app/models/meal-order/menu-cycle.model';
import { MenuCycleEditorComponent } from './menu-cycle-editor.component';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';


@Component({
  selector: 'menu-cycles-management',
  templateUrl: './menu-cycles-management.component.html',
  styleUrls: ['./menu-cycles-management.component.css']
})
export class MenuCyclesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: MenuCycle[] = [];
  rowsCache: MenuCycle[] = [];
  allPermissions: Permission[] = [];
  editedMenuCycle: MenuCycle;
  sourceMenuCycle: MenuCycle;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  @Input() isHideHeader: boolean;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('menuCycleEditor')
  menuCycleEditor: MenuCycleEditorComponent;

  @ViewChild('menuCyclesTable') table: any;

  header: string;
  @Input() catererId: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private menuService: MenuService, public dialog: MatDialog) {
  }

  openDialog(menuCycle: MenuCycle): void {
    const dialogRef = this.dialog.open(MenuCycleEditorComponent, {
      data: { header: this.header, menuCycle: menuCycle, catererId: this.catererId },
      width: '900px',
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
      { prop: 'outletProfileName', name: 'Outlet Profile' },
      { prop: 'label', name: 'Label', width: 200 },
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
    this.filter.filters = f + '(IsActive)==true,(Label)@=' + this.keyword;
    
    this.menuService.getMenuCyclesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let menuCycles = results.pagedData;

        menuCycles.forEach((menuCycle, index, menuCycles) => {
          (<any>menuCycle).index = index + 1;
        });


        this.rowsCache = [...menuCycles];
        this.rows = menuCycles;

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

  newMenuCycle() {
    this.header = 'New Menu Cycle';
    this.editedMenuCycle = new MenuCycle();
    this.openDialog(this.editedMenuCycle);
  }


  editMenuCycle(row: MenuCycle) {
    this.editedMenuCycle = row;
    this.header = 'Edit Menu Cycle';
    this.editedMenuCycle.catererId = this.catererId;
    this.openDialog(this.editedMenuCycle);
  }

  deleteMenuCycle(row: MenuCycle) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" cycle?', DialogType.confirm, () => this.deleteMenuCycleHelper(row));
  }


  deleteMenuCycleHelper(row: MenuCycle) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.menuService.deleteMenuCycle(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the menu cycle.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageMenuCycles() {
    return true; //this.accountService.userHasPermission(Permission.manageMenuCyclesPermission)
  }

}
