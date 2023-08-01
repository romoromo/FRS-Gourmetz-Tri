import { Component, OnInit, Output, EventEmitter, Input, Inject, AfterViewInit, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { Asset } from 'src/app/models/asset.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AccountService } from 'src/app/services/account.service';
import { Utilities } from 'src/app/services/utilities';
import { UserVehicle } from 'src/app/models/uservehicle.model';
import { AssetService } from 'src/app/services/asset.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { DateOnlyPipe } from 'src/app/pipes/datetime.pipe';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';

@Component({
  selector: 'asset-selector',
  templateUrl: './asset-selector.component.html',
  styleUrls: ['./asset-selector.component.css']
})
export class AssetSelectorComponent implements OnInit, AfterViewChecked {
  public assetIds: number[];
  assets: Asset[];
  assetsCache: Asset[];
  selectedAssets: any[];
  loadingIndicator: boolean;
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  columns: any[] = [];
  rows: Asset[] = [];
  rowsCache: Asset[] = [];
  allRowsSelected = false;

  totalRows = 0;
  //@Input() showTxt: boolean;
  //@Output() public onSelectedFinished = new EventEmitter();

  constructor(private alertService: AlertService, private http: HttpClient, private assetService: AssetService, private translationService: AppTranslationService,
    public dialogRef: MatDialogRef<AssetSelectorComponent>, private cdref: ChangeDetectorRef,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.assets) != typeof (undefined)) {
      this.selectedAssets = [];
      Object.assign(this.selectedAssets, data.assets);
      this.allRowsSelected = false;
    }
  }

  ngAfterViewChecked(): void {
    window.dispatchEvent(new Event('resize'));
    this.cdref.detectChanges();
  }

  initializeFilter() {
    this.filter = new Filter(1, 1);
    this.filter.sorts = 'serialNumber';
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
      { prop: 'serialNumber', name: 'Serial No.' },
      { prop: 'assetModelName', name: 'Asset Model' },
      { prop: 'assetTypeName', name: 'Asset Type' },
      { prop: 'purchaseDate', name: 'Purchase Date', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'warrantyStart', name: 'Warranty Start', pipe: new DateOnlyPipe('en-SG') },
      { prop: 'warrantyEnd', name: 'Warranty End', pipe: new DateOnlyPipe('en-SG') },
    ];
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
    //this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    this.filter.filters = '(IsActive)==true,(SerialNumber|assetModelName|assetTypeName)@=' + this.keyword;

    this.assetService.getAssetsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let assets = results.pagedData;

        assets.forEach((asset, index, assets) => {
          (<any>asset).index = index + 1;
          (<any>asset).assetId = asset.id;
        });


        this.rowsCache = [...assets];
        this.rows = assets;
        this.rows = [...this.rows]
        this.totalRows = results.totalCount;
        this.allRowsSelected = false;
        //if (this.totalRows == this.selectedAssets.length) {
        //  this.allRowsSelected = true;
        //}
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    this.loadData(null);
  }

  onSelect({ selected }) {
    console.log('Select Event', selected, this.selectedAssets);

    //this.selectedAssets.splice(0, this.selectedAssets.length);
    if (selected && selected.length > 0) {
      this.selectedAssets.push(...selected);
    } else {
      //remove only unselected
      if (this.selectedAssets) {
        this.selectedAssets = Utilities.getUniqueValues(this.selectedAssets, 'assetId');
        this.selectedAssets.forEach((asset, index, assets) => {
          let indx = this.rows.findIndex(e => e.assetId == asset.assetId);
          if (indx > -1) {
            this.selectedAssets[index].deleted = true;
            console.log(index);
          }
        });

        this.selectedAssets = this.selectedAssets.filter(e => !e.deleted);
      }
    }

    this.selectedAssets = Utilities.getUniqueValues(this.selectedAssets, 'assetId');
    //this.setSelectAll();
  }

  setSelectAll() {
    let isAllSelected = true;
    this.rows.forEach((asset, index, assets) => {
      let indx = this.selectedAssets.findIndex(e => e.assetId == asset.assetId);
      if (isAllSelected) {
        isAllSelected = indx > -1;
      }
    });

    this.allRowsSelected = isAllSelected;
  }

  onCheckboxSelect(event, row) {
    if (event.currentTarget.checked) {
      this.selectedAssets.push(row);
    } else {
      let indx = this.selectedAssets.findIndex(e => e.assetId == row.assetId);
      if(indx > -1)this.selectedAssets.splice(indx, 1);
    }
    this.selectedAssets = Utilities.getUniqueValues(this.selectedAssets, 'assetId');
    this.setSelectAll();
  }

  onActivate(event) {
    //console.log(event);
    //console.log(event.row);
  }

  getId(row) {
    return row.assetId;
  }
  //ngOnInit() {
  //  this.assetService.getAssets(this.assetService.currentUser.id).subscribe(results => {
  //    this.assets = results;
  //    if (typeof (this.selectedAssets) != typeof (undefined)) {
  //      this.assets.map(cg => {
  //        const index = this.selectedAssets.findIndex(item => item.email == cg.email)
  //        if (index >= 0) {
  //          cg.checked = true;
  //        }
  //      });
  //    }

  //    this.assetsCache = this.assets;

  //  }, error => { });
  //}

  public save = () => {
    this.selectedAssets = Utilities.getUniqueValues(this.selectedAssets, 'assetId');
    this.dialogRef.close({ isCancel: false, selectedAssets: this.selectedAssets });
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }
}
