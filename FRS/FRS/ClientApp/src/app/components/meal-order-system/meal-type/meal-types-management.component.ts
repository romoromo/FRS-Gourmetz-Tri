import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { MealType } from 'src/app/models/meal-order/meal-type.model';
import { MealTypeEditorComponent } from './meal-type-editor.component';
import { MealService } from 'src/app/services/meal-order/meal.service';


@Component({
  selector: 'meal-types-management',
  templateUrl: './meal-types-management.component.html',
  styleUrls: ['./meal-types-management.component.css']
})
export class MealTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: MealType[] = [];
  rowsCache: MealType[] = [];
  allPermissions: Permission[] = [];
  editedMealType: MealType;
  sourceMealType: MealType;
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  @Input() isHideHeader: boolean;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('chargeByTemplate')
  chargeByTemplate: TemplateRef<any>;

  @ViewChild('submenuTemplate')
  submenuTemplate: TemplateRef<any>;

  @ViewChild('mealTypeEditor')
  mealTypeEditor: MealTypeEditorComponent;

  @ViewChild('mealTypesTable') table: any;
  header: string;
  @Input() catererId: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private mealService: MealService, public dialog: MatDialog) {
  }

  openDialog(mealType: MealType): void {
    const dialogRef = this.dialog.open(MealTypeEditorComponent, {
      data: { header: this.header, mealType: mealType, catererId: this.catererId },
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
      { prop: 'price', name: 'Price', width: 100 },
      { name: 'Charge By', cellTemplate: this.chargeByTemplate },
      { name: 'Sub Menus', cellTemplate: this.submenuTemplate },
      //{ prop: 'cuisineName', name: 'Cuisine' },
      //{ prop: 'dishNames', name: 'Dishes', sortable: false },
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
    
    this.mealService.getMealTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let mealTypes = results.pagedData;

        mealTypes.forEach((mealType, index, mealTypes) => {
          (<any>mealType).index = index + 1;
        });


        this.rowsCache = [...mealTypes];
        this.rows = mealTypes;

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

  newMealType() {
    this.header = 'New Meal Type';
    this.editedMealType = new MealType();
    this.openDialog(this.editedMealType);
  }


  editMealType(row: MealType) {
    this.editedMealType = row;
    this.header = 'Edit Meal Type';
    this.editedMealType.catererId = this.catererId;
    this.openDialog(this.editedMealType);
  }

  deleteMealType(row: MealType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" meal type?', DialogType.confirm, () => this.deleteMealTypeHelper(row));
  }


  deleteMealTypeHelper(row: MealType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.mealService.deleteMealType(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the meal type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageMealTypes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCatererMealTypesMenu)
  }

}
