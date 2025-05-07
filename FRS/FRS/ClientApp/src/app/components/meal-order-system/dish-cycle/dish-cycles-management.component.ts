import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { DishCycleEditorComponent } from './dish-cycle-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { DishCycle } from 'src/app/models/meal-order/dish-cycle';


@Component({
  selector: 'dish-cycles-management',
  templateUrl: './dish-cycles-management.component.html',
  styleUrls: ['./dish-cycles-management.component.css']
})
export class DishCyclesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: DishCycle[] = [];
  rowsCache: DishCycle[] = [];
  allPermissions: Permission[] = [];
  editedDishCycle: DishCycle;
  sourceDishCycle: DishCycle;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  @Input() isHideHeader: boolean;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('dishCycleEditor')
  dishCycleEditor: DishCycleEditorComponent;

  @ViewChild('dishCyclesTable') table: any;

  header: string;
  @Input() catererId: string;

  futureDate = false;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private dishService: DishService, public dialog: MatDialog) {
  }

  openDialog(dishCycle: DishCycle): void {
    const dialogRef = this.dialog.open(DishCycleEditorComponent, {
      data: { header: this.header, dishCycle: dishCycle, catererId: this.catererId },
      width: '100vw',
      height: '100vh',
      panelClass: 'full-screen-dialog',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if(!result) this.loadData(null);
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

    const now: Date = new Date();

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    let f = this.catererId ? '(CatererId)==' + this.catererId + ',' : '';
    f = this.futureDate ? f + '(EndDate)>=' + now.toDateString() + ',' : f
    this.filter.filters = f + '(IsActive)==true,(Label)@=' + this.keyword;
    
    this.dishService.getDishCyclesSimpleByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let dishCycles = results.pagedData;

        dishCycles.forEach((dishCycle, index, dishCycles) => {
          (<any>dishCycle).index = index + 1;
        });


        this.rowsCache = [...dishCycles];
        this.rows = dishCycles;

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
    //this.clearFilterAndPagedResult();
    //this.loadData(null);
  }

  onSearch() {
    this.filter.page = 1;
    this.loadData(null);
  }

  onFuturDate() {
    console.log('change');
    this.filter.page = 1;
    this.loadData(null);
  }

  newDishCycle() {
    this.header = 'New Dish Cycle';
    this.editedDishCycle = new DishCycle();
    this.openDialog(this.editedDishCycle);
  }


  editDishCycle(row: DishCycle) {
    this.header = 'Edit Dish Cycle';
    this.editedDishCycle = row;
    this.editedDishCycle.catererId = this.catererId;
    this.editedDishCycle.id = row.id;
    this.openDialog(this.editedDishCycle);

    //this.alertService.startLoadingMessage("Fetching Dish Cycle Details");

    //this.dishService.getDishCycleById(row.id)
    //  .subscribe(results => {
    //    this.editedDishCycle = results;
    //    this.header = 'Edit Dish Cycle';
    //    this.editedDishCycle.catererId = this.catererId;
    //    this.editedDishCycle.id = row.id;
    //    this.openDialog(this.editedDishCycle);
    //    this.alertService.stopLoadingMessage();
    //  },
    //    error => {
    //      this.alertService.stopLoadingMessage();
    //      this.loadingIndicator = false;

    //      this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
    //        MessageSeverity.error);
    //    });

    
  }

  deleteDishCycle(row: DishCycle) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" cycle?', DialogType.confirm, () => this.deleteDishCycleHelper(row));
  }


  deleteDishCycleHelper(row: DishCycle) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.dishService.deleteDishCycle(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the dish cycle.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageDishCycles() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCatererDishCyclesMenu)
  }

}
