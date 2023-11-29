import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { MealPeriod } from 'src/app/models/meal-order/meal-period.model';
import { MealPeriodEditorComponent } from './meal-period-editor.component';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';


@Component({
  selector: 'meal-periods-management',
  templateUrl: './meal-periods-management.component.html',
  styleUrls: ['./meal-periods-management.component.css']
})
export class MealPeriodsManagementComponent implements OnInit {
  columns: any[] = [];
  rows: MealPeriod[] = [];
  rowsCache: MealPeriod[] = [];
  allPermissions: Permission[] = [];
  editedMealPeriod: MealPeriod;
  sourceMealPeriod: MealPeriod;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  @Input() isHideHeader: boolean;
  @Input() catererId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('timeTemplate')
  timeTemplate: TemplateRef<any>;

  @ViewChild('flagTemplate')
  flagTemplate: TemplateRef<any>;

  @ViewChild('mealPeriodEditor')
  mealPeriodEditor: MealPeriodEditorComponent;

  @ViewChild('mealPeriodsTable') table: any;

  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private mealService: MealService, public dialog: MatDialog) {
  }

  openDialog(mealPeriod: MealPeriod): void {
    const dialogRef = this.dialog.open(MealPeriodEditorComponent, {
      data: { header: this.header, mealPeriod: mealPeriod, catererId: this.catererId },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData(null);
    });
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'sequence';
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
      { prop: 'name', name: 'Name' },
      { prop: 'sequence', name: 'Sequence' },
      { prop: 'startDate', name: 'Start Time', cellTemplate: this.timeTemplate },
      { prop: 'endDate', name: 'End Time', cellTemplate: this.timeTemplate },
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
    this.filter.filters = f + '(IsActive)==true,(Name)@=' + this.keyword + ',(InstitutionId)==' + this.accountService.currentUser.institutionId;
    
    this.mealService.getMealPeriodsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let mealPeriods = results.pagedData;

        mealPeriods.forEach((mealPeriod, index, mealPeriods) => {
          (<any>mealPeriod).index = index + 1;
        });


        this.rowsCache = [...mealPeriods];
        this.rows = mealPeriods;

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

  newMealPeriod() {
    this.header = 'New Meal Period';
    this.editedMealPeriod = new MealPeriod();
    this.openDialog(this.editedMealPeriod);
  }


  editMealPeriod(row: MealPeriod) {
    this.editedMealPeriod = row;
    this.header = 'Edit Meal Period';
    this.editedMealPeriod.catererId = this.catererId;
    this.openDialog(this.editedMealPeriod);
  }

  deleteMealPeriod(row: MealPeriod) {
    this.alertService.showDialog('Are you sure you want to delete \"' + row.name + '\"?', DialogType.confirm, () => this.deleteMealPeriodHelper(row));
  }


  deleteMealPeriodHelper(row: MealPeriod) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.mealService.deleteMealPeriod(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the meal period\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageMealPeriods() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCatererMealPeriodsMenu)
  }

}
