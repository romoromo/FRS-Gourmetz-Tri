import { Component, OnInit, Output, EventEmitter, Input, Inject, TemplateRef, ViewChild } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Dish, DishDetail } from 'src/app/models/meal-order/dish.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { Outlet } from 'src/app/models/meal-order/outlet.model';
import { CatererInfo, CatererOutlet } from 'src/app/models/meal-order/caterer-info.model';

@Component({
  selector: 'store-selector-details',
  templateUrl: './store-selector-details.component.html',
  styleUrls: ['./store-selector-details.component.css']
})
export class StoreSelectorDetailsComponent implements OnInit {
  public storeIds: number[];
  stores: StoreInfo[];
  storesCache: StoreInfo[];
  selectedStores: StoreInfo[];
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  columns: any[] = [];
  rows: StoreInfo[] = [];
  rowsCache: StoreInfo[] = [];
  outletId: string;
  editOutlet: Outlet;
  selectedCaterers: CatererOutlet[];

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  constructor(private http: HttpClient, private dishService: DishService, private deliveryService: DeliveryService,
    private alertService: AlertService, public dialogRef: MatDialogRef<StoreSelectorDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.editOutlet = data.outlet;
    if (typeof (data.stores) != typeof (undefined)) {
      this.selectedStores = data.stores;
    }

    if (typeof (data.outlet) != typeof (undefined) && data.outlet.catererOutlets) {
      this.selectedCaterers = data.outlet.catererOutlets;
    }
    console.log("selected caterers: ", this.selectedCaterers)
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'code';
    this.filter.filters = '';
    this.filter.page = 0;


  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {

    this.columns = [
      { prop: 'code', name: 'Store Code' },
      { prop: 'name', name: 'Store Name' },
      { prop: 'storeType', name: 'Store Type' },
      { prop: 'catererInfoName', name: 'Caterer' },
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
    let filterCaterer = "";
    if (this.selectedCaterers) {
      this.selectedCaterers.forEach((caterer, i) => {
        if (caterer.status == "APPROVED") {
          if (filterCaterer != "") {
            filterCaterer += '|'
          }
          filterCaterer += caterer.catererInfoId
        }
      });
    }
    console.log("caterer filter = ", filterCaterer)

    this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    let f = filterCaterer ? '(CatererInfoId)==' + filterCaterer + ',' : '';
    this.filter.filters = f + '(IsActive)==true,(Code|Name)@=' + this.keyword;

    this.deliveryService.getStoreInfosByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        console.log("result page: ", this.pagedResult)

        let stores = results.pagedData;

        stores.forEach((store,index) => {
          if (store.outletId == null) {
            store.status = "Available"
          } else if (store.outletId == this.editOutlet.id) {
            store.status = "Added"
          } else {
            store.status = "Taken"
            stores.splice(index, 1);
          }
        })

        stores.forEach((store, index, stores) => {
          (<any>store).index = index + 1;
        });

        if (typeof (this.selectedStores) != typeof (undefined)) {
          stores.map(cg => {
            const index = this.selectedStores.findIndex(item => item.id == cg.id)
            if (index >= 0) {
              cg.checked = true;
            }
          });
        }

        this.rowsCache = [...stores];
        this.rows = stores;

      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.keyword = value;
    this.loadData(null);
  }

  public save = () => {
    let stores = this.rows.filter((cg) => cg.checked); 
    this.dialogRef.close({ isCancel: false, selectedStores: stores });
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }
}
