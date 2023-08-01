import { Component, OnInit, Output, EventEmitter, Input, Inject, TemplateRef, ViewChild } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Dish, DishDetail } from 'src/app/models/meal-order/dish.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';

@Component({
  selector: 'dish-selector',
  templateUrl: './dish-selector.component.html',
  styleUrls: ['./dish-selector.component.css']
})
export class DishSelectorComponent implements OnInit {
  public dishIds: number[];
  dishes: Dish[];
  dishesCache: Dish[];
  selectedDishes: DishDetail[];
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  columns: any[] = [];
  rows: Dish[] = [];
  rowsCache: Dish[] = [];
  catererId: string;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  constructor(private http: HttpClient, private dishService: DishService,
    private alertService: AlertService, public dialogRef: MatDialogRef<DishSelectorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.catererId = data.catererId;
    if (typeof (data.dishes) != typeof (undefined)) {
      this.selectedDishes = data.dishes;
    }
  }

  initializeFilter() {
    this.filter = new Filter(-1, -10);
    this.filter.sorts = 'code';
    this.filter.filters = '';
    this.filter.page = -1;


  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {

    this.columns = [
      { prop: 'code', name: 'Dish Code' },
      { prop: 'label', name: 'Label' },
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
    this.filter.pageSize = -1;

    if (ev) {
      //this.filter.page = ev.offset + 1;
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

        let dishes = results.pagedData;

        dishes.forEach((dish, index, dishes) => {
          (<any>dish).index = index + 1;
        });

        dishes.map(cg => {
          const index = this.selectedDishes.findIndex(item => item.dishId == cg.id)
          if (index >= 0) {
            cg.checked = true;
          }
          
        });
        

        this.rowsCache = [...dishes];
        this.rows = dishes;

      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  //onSearchChanged(value: string) {
  //  this.keyword = value;
  //  this.loadData(null);
  //}

  addRemoveDish(row) {
    //const index = this.selectedDishes.findIndex(item => item.dishId == row.id)
    //if (row.checked) {
    //  if (index < 0) {
    //    let dt = new DishDetail();
    //    dt.dishId = row.id;
    //    dt.code = row.code;
    //    dt.label = row.label;
    //    dt.cookedWeight = row.cookedWeight;

    //    this.selectedDishes.push(dt);
    //  }
    //} else {
    //  if (index >= 0) {
    //    this.selectedDishes.splice(index, 1);
    //  }
    //}
    
  }

  //ngOnInit() {
  //  let filter = new Filter();
  //  filter.filters = '(IsActive)==true';

  //  this.dishService.getDishesByFilter(filter).subscribe(results => {
  //    this.dishes = results.pagedData;
  //    if (typeof (this.selectedDishes) != typeof (undefined)) {
  //      this.dishes.map(cg => {
  //        const index = this.selectedDishes.findIndex(item => item.dishId == cg.id)
  //        if (index >= 0) {
  //          cg.checked = true;
  //        }
  //      });
  //    }

  //    this.dishesCache = this.dishes;

  //  }, error => { });
  //}

  public save = () => {
    let dishes = this.rowsCache.filter((cg) => cg.checked);
    //let selected = [];
    //this.rows.forEach((dish, i, dishes) => {
    //  const index = this.selectedDishes.findIndex(item => item.dishId == dish.id)
    //  if (index >= 0) {
    //    selected.push(dish);
    //  }
    //});

    this.dialogRef.close({ isCancel: false, selectedDishes: dishes });
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.code, r.label));
  }
}
