import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Filter, PagedResult } from '../../../models/sieve-filter.model';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { Dish } from 'src/app/models/meal-order/dish.model';
import { DishEditorComponent } from './dish-editor.component';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { getBaseUrl } from 'src/app/app.module';


@Component({
  selector: 'dishes-management',
  templateUrl: './dishes-management.component.html',
  styleUrls: ['./dishes-management.component.css']
})
export class DishesManagementComponent implements OnInit {
  columns: any[] = [];
  rows: Dish[] = [];
  rowsCache: Dish[] = [];
  allPermissions: Permission[] = [];
  editedDish: Dish;
  sourceDish: Dish;
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

  @ViewChild('dishEditor')
  dishEditor: DishEditorComponent;

  @ViewChild('dishesTable') table: any;

  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private dishService: DishService, public dialog: MatDialog) {
  }

  openDialog(dish: Dish): void {
    const dialogRef = this.dialog.open(DishEditorComponent, {
      data: { header: this.header, dish: dish, catererId: this.catererId },
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
      { prop: 'productionDescription', name: 'Production Description' },
      { prop: 'dishTypeName', name: 'Type' },
      { prop: 'cuisineName', name: 'Cuisine' },
      { prop: 'bentoBoxTypeCode', name: 'Bento Box' },
//      { prop: 'dishPeriodNames', name: 'Periods', sortable: false },
      { prop: 'rrPrice', name: 'RRP' },
      { prop: 'cost', name: 'Cost' },
      { prop: 'isEnabled', name: 'Is Enabled', cellTemplate: this.flagTemplate },
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
    
    this.dishService.getDishesByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let dishes = results.pagedData;

        dishes.forEach((dish, index, dishes) => {
          (<any>dish).index = index + 1;
        });


        this.rowsCache = [...dishes];
        this.rows = dishes;

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

  newDish() {
    this.header = 'New Dish';
    this.editedDish = new Dish();
    this.openDialog(this.editedDish);
  }


  editDish(row: Dish) {
    this.editedDish = row;
    this.header = 'Edit Dish';
    this.editedDish.catererId = this.catererId;
    this.openDialog(this.editedDish);
  }

  deleteDish(row: Dish) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" dish?', DialogType.confirm, () => this.deleteDishHelper(row));
  }


  deleteDishHelper(row: Dish) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.dishService.deleteDish(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.loadData();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the dish.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  previewImage(row) {
    window.open(getBaseUrl() + '/preview/dish/' + row.id, '_blank');
  }

  get canManageDishes() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtCatererDishesMenu)
  }

}
