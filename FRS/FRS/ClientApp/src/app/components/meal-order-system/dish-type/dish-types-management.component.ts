import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { DishType } from 'src/app/models/meal-order/dish-type.model';
import { DishTypeEditorComponent } from './dish-type-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';


@Component({
  selector: 'dish-types-management',
  templateUrl: './dish-types-management.component.html',
  styleUrls: ['./dish-types-management.component.css']
})
export class DishTypesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: DishType[] = [];
  rowsCache: DishType[] = [];
  allPermissions: Permission[] = [];
  editedDishType: DishType;
  sourceDishType: DishType;
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

  @ViewChild('dishTypeEditor')
  dishTypeEditor: DishTypeEditorComponent;

  @ViewChild('dishTypesTable') table: any;

  header: string;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private dishService: DishService, public dialog: MatDialog) {
  }

  openDialog(dishType: DishType): void {
    const dialogRef = this.dialog.open(DishTypeEditorComponent, {
      data: { header: this.header, dishType: dishType, catererId: this.catererId },
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
      //{ prop: 'dishTypePeriodNames', name: 'Periods', sortable: false },
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
    
    this.dishService.getDishTypesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let dishTypes = results.pagedData;

        dishTypes.forEach((dishType, index, dishTypes) => {
          (<any>dishType).index = index + 1;
        });


        this.rowsCache = [...dishTypes];
        this.rows = dishTypes;

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

  newDishType() {
    this.header = 'New Dish Type';
    this.editedDishType = new DishType();
    this.openDialog(this.editedDishType);
  }


  editDishType(row: DishType) {
    this.editedDishType = row;
    this.header = 'Edit Dish Type';
    this.editedDishType.catererId = this.catererId;
    this.openDialog(this.editedDishType);
  }

  deleteDishType(row: DishType) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" dish type?', DialogType.confirm, () => this.deleteDishTypeHelper(row));
  }


  deleteDishTypeHelper(row: DishType) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.dishService.deleteDishType(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the dish type.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  get canManageDishTypes() {
    return true; //this.accountService.userHasPermission(Permission.manageDishTypesPermission)
  }

}
