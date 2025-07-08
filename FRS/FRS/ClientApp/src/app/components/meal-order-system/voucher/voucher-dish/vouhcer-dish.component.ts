import { Component, Inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { SearchBoxComponent } from 'src/app/components/controls/search-box.component';
import { Dish } from 'src/app/models/meal-order/dish.model';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { Utilities } from 'src/app/services/utilities';

@Component({
  selector: 'app-voucher-dish',
  templateUrl: './vouhcer-dish.component.html',
  styleUrls: ['./vouhcer-dish.component.css']
})
export class VoucherDishComponent implements OnInit {
  messageErrors: any[] = []
  isSaving: boolean = false;
  keyword: string = '';
  filter: Filter;
  filterDishCycle: Filter;
  pagedResult: PagedResult;
  columns: any[] = [];
  rows: Dish[] = [];
  rowsCache: Dish[] = [];
  loadingIndicator: boolean = false;
  private selected: any[] = [];
  private selectedOriginal: any[] = [];
  private selectedDishes: any[] = [];
  outletProfiles: any[] = []
  selectedOutletProfileId: number | null = null;
  dishCycles: any[] = []
  dishCycleLoading: boolean = false;
  selectedDishCycleId: number | null = null;


  @ViewChild('hdrTpl')
  hdrTpl: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('searchbox') searchbox: SearchBoxComponent;


  constructor(
    public dialogRef: MatDialogRef<VoucherDishComponent>,
    private dishService: DishService,
    private alertService: AlertService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (typeof (data.selectedDishes) != typeof (undefined)) {
      this.selectedDishes = data.selectedDishes;
    }

    if (typeof (data.outletProfiles) != typeof (undefined)) {
      this.outletProfiles = data.outletProfiles;
    }
  }

  ngOnInit(): void {
    this.initializeFilter()
    this.initializePagedResult()
    this.initializeTableDefinition()
    this.loadData();
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'code';
    this.filter.filters = '';
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
      { prop: 'catererName', name: 'Caterer' },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false, headerCheckboxable: true, headerTemplate: this.hdrTpl }
    ];
  }



  loadData(ev?: any) {
    this.filter.pageSize = -1;
    this.loadingIndicator = true;
    if (ev) {
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(IsActive)==true,(Code|Label)@=' + this.keyword;

    this.dishService.getDishesLiteByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        let dishes = results.pagedData;

        dishes.forEach((dish, index, dishes) => {
          (<any>dish).index = index + 1;
        });

        if (typeof (this.selectedDishes) != typeof (undefined)) {
          dishes.map(cg => {
            const index = this.selectedDishes.findIndex(item => item.dishId == cg.id)
            if (index >= 0) {
              cg.checked = true;
            }
          });
        }

        this.rowsCache = [...dishes];
        this.rows = dishes;
        this.selectRowsOnInit();

      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }
        , () => {
          this.loadingIndicator = false;
        });
  }

  onSelect({ selected }) {
    this.selected = selected;

    const visibleIds = this.rows.map(row => row.id);
    if (selected.length === 0) {
      this.selectedOriginal = this.selectedOriginal.filter(item =>
        !visibleIds.includes(item.id)
      );
    }

    for (let item of selected) {
      if (!this.selectedOriginal.some(x => x.id == item.id)) {
        this.selectedOriginal.push(item);
      }
    }

    this.selectedOriginal = this.selectedOriginal.filter(item =>
      !visibleIds.includes(item.id) || selected.some(sel => sel.id === item.id)
    );
    this.rows.forEach(row => (row.checked = this.selectedOriginal.findIndex(e => e.id == row.id) > -1));
  }

  selectRowsOnInit(): void {
    let selected = this.rows.filter(e => e.checked);
    this.selected = selected
    this.selectedOriginal = selected
  }

  async filterDataOnClientSide() {
    const keywordLC = this.keyword.toLowerCase().trim() || '';
    const hasKeyword = keywordLC.length > 0;

    let dishIds: number[] = [];

    const hasDishCycle = this.selectedDishCycleId > 0;
    const hasOutlet = this.selectedOutletProfileId > 0;
    const hasDishFilter = hasDishCycle && hasOutlet;

    this.loadingIndicator = true;

    if (hasDishFilter) {
      dishIds = await this.dishService.getDishesIdsByCycleId(this.selectedDishCycleId).toPromise();
    }

    const dishIdSet = dishIds.length > 0 ? new Set(dishIds.map(id => id.toString())) : null;

    if (!hasKeyword && !dishIdSet) {
      this.rows = [...this.rowsCache];
      this.loadingIndicator = false;
      return;
    }


    this.rows = [];

    for (const item of this.rowsCache) {
      const idStr = item.id.toString();

      const matchesName =
        hasKeyword &&
        (item.label.toLowerCase().includes(keywordLC) || item.code.toLowerCase().includes(keywordLC));

      const matchesDishCycle = dishIdSet && dishIdSet.has(idStr);

      let shouldInclude = false
      if (hasKeyword && hasDishFilter) {
        shouldInclude = matchesName && matchesDishCycle;
      } else if (hasKeyword && !hasDishFilter) {
        shouldInclude = matchesName;
      } else if (!hasKeyword && hasDishFilter) {
        shouldInclude = matchesDishCycle;
      }

      if (shouldInclude) {
        this.rows.push(item);
      }
    }

    this.loadingIndicator = false;
  }

  onSearchChanged(value: string) {
    this.keyword = value;
  }

  async onSearchTriggered() {
    await this.filterDataOnClientSide()
  }

  async onAppyFilterTrigger() {
    await this.filterDataOnClientSide()
  }

  selectFn(ev) {
    console.log(ev);
  }

  onCheckboxChangeFn(ev) {
    console.log(ev);
  }

  save() {
    this.dialogRef.close({ isCancel: false, selectedData: this.selectedOriginal });
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  onOutletChange = (selectedId: number) => {
    this.selectedOutletProfileId = selectedId
    this.loadDishCyle(selectedId)
  }

  loadDishCyle(outletProfileId: number) {
    this.dishCycleLoading = true
    const now: Date = new Date();
    this.filterDishCycle = new Filter(1, 10);
    this.filterDishCycle.sorts = 'label';
    this.filterDishCycle.filters = `(IsActive)==true,(OutletProfileId)==${outletProfileId},(EndDate)>=${now.toDateString()},(CycleType)==Main Menu`;

    this.dishService.getDishCyclesSimpleByFilter(this.filterDishCycle)
      .subscribe(results => {
        this.dishCycles = results.pagedData
        this.dishCycleLoading = false
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
          this.dishCycleLoading = false
        },
        () => {
          this.dishCycleLoading = false
        });
  }

  onDishCycleChange = (selectedId: number) => {
    this.selectedDishCycleId = selectedId
  }

  clearFilters() {
    this.keyword = '';
    this.searchbox.clear();
    this.selectedOutletProfileId = null;
    this.selectedDishCycleId = null
    const originalData = [...this.rowsCache];
    this.rows = originalData
  }
}
