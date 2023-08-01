import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Filter } from 'src/app/models/sieve-filter.model';
import { CatererInfo, CatererOutlet } from 'src/app/models/meal-order/caterer-info.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Outlet } from 'src/app/models/meal-order/outlet.model';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { StoreSelectorDetailsComponent } from '../store-selector-details/store-selector-details.component'

@Component({
  selector: 'store-selector',
  templateUrl: './store-selector.component.html',
  styleUrls: ['./store-selector.component.css']
})
export class StoreSelectorComponent implements OnInit {
  stores: StoreInfo[];
  storesCache: StoreInfo[];
  selectedStores: StoreInfo[] = [];
  editOutlet: Outlet;
  selectedCaterers: CatererOutlet[];

  constructor(private http: HttpClient, private alertService: AlertService, private deliveryService: DeliveryService, public dialog: MatDialog,
    public dialogRef: MatDialogRef<StoreSelectorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outlet) != typeof (undefined)) {
      this.editOutlet = data.outlet;
      if (data.catererOutlets) {
        this.editOutlet.catererOutlets = [];
      }
      this.selectedCaterers = data.outlet.catererOutlets;
      console.log("selected caterers: ", this.selectedCaterers)
    }
  }

  ngOnInit() {

    this.loadData();
  }

  loadData() {
    let filterCaterer = "";
    this.selectedCaterers.forEach((caterer, i) => {
      if (caterer.status == "APPROVED") {
        if (filterCaterer != "") {
          filterCaterer += '|'
        }
        filterCaterer += caterer.catererInfoId
      }
    });
    console.log("caterer filter = ", filterCaterer)

    this.getStore(filterCaterer);
  }

  getStore(filterCaterer) {
    let filter = new Filter();
    filter.filters = '(IsActive)==true,(CatererInfoId)==' + filterCaterer;

    console.log("filter:", filter)

    this.deliveryService.getStoreInfosByFilter(filter).subscribe(results => {
      this.stores = results.pagedData;

      this.stores.forEach((store) => {
        if (store.outletId == null) {
          store.status = "Available"
        } else if (store.outletId == this.editOutlet.id) {
          store.status = "Added"
          this.selectedStores.push(store);
        } else {
          store.status = "Taken"
        }
      })

      //this.caterers.map(cg => {
      //  if (this.editOutlet.catererOutlets && typeof (this.editOutlet.catererOutlets) != typeof (undefined)) {
      //    let catererOutlet = this.editOutlet.catererOutlets.find(item => item.catererInfoId == cg.id);
      //    if (catererOutlet) {
      //      cg.status = catererOutlet.status;
      //    }
      //  }
      //});

      console.log("stores: ", this.stores)


      this.storesCache = this.stores;

    }, error => { });
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  addStore(row: StoreInfo) {
    row.outletId = this.editOutlet.id;
    this.deliveryService.updateStoreInfo(row).subscribe(results => {
        row.status = 'Added';
    }, error => {
        this.alertService.stopLoadingMessage();
        this.alertService.showStickyMessage("Request Error", `An error occured while adding Store.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          MessageSeverity.error);
    });

  }

  save() {
    this.editOutlet.stores = this.selectedStores;
    console.log("outlet: ", this.editOutlet);
    this.deliveryService.updateOutletStore(this.editOutlet).subscribe(results => {
      this.alertService.showMessage("Success", `Stores were updated successfully`, MessageSeverity.success);
      this.dialogRef.close();
    }, error => {
      this.alertService.stopLoadingMessage();
      this.alertService.showStickyMessage("Request Error", `An error occured while adding Store.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
        MessageSeverity.error);
    });
  }

  addStoreSel(outlet,stores) {
    const dialogRef = this.dialog.open(StoreSelectorDetailsComponent, {
      data: { header: "Stores", outlet: outlet, stores: stores },
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result.isCancel) {
        console.log("early selstore:", this.selectedStores);
        console.log("change selstore:", result.selectedStores);
        this.selectedStores = result.selectedStores;
      }
    });
  }

  onSearchChanged(value: string) {
    this.stores = this.storesCache.filter(r => Utilities.searchArray(value, false, r.name));
  }
}
