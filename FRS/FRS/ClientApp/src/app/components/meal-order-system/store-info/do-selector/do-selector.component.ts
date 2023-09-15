import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Filter } from 'src/app/models/sieve-filter.model';
import { CatererInfo, CatererOutlet } from 'src/app/models/meal-order/caterer-info.model';
import { DeliveryOrder } from 'src/app/models/meal-order/delivery-order.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { MatDialog } from '@angular/material';
import { ReceiveDOComponent } from './../receive-do/receive-do.component'

@Component({
  selector: 'do-selector',
  templateUrl: './do-selector.component.html',
  styleUrls: ['./do-selector.component.css']
})
export class DOSelectorComponent implements OnInit {
  dos: DeliveryOrder[];
  dosCache: DeliveryOrder[];
  editStore: StoreInfo;
  selectedCaterers: CatererOutlet[];

  constructor(private http: HttpClient, private alertService: AlertService, private deliveryService: DeliveryService, public dialog: MatDialog,
    public dialogRef: MatDialogRef<DOSelectorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.store) != typeof (undefined)) {
      this.editStore = data.store;

      //if (data.catererOutlets) {
      //  //this.editStore.catererOutlets = [];
      //}
      //this.selectedCaterers = data.outlet.catererOutlets;
    }
  }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true,(ToStoreId)==' + this.editStore.id;

    this.deliveryService.getDeliveryOrderNewsByFilter(filter).subscribe(results => {
      this.dos = results.pagedData;

      //this.dos.map(cg => {
      //  if (this.editOutlet.catererOutlets && typeof (this.editOutlet.catererOutlets) != typeof (undefined)) {
      //    let catererOutlet = this.editOutlet.catererOutlets.find(item => item.catererInfoId == cg.id);
      //    if (catererOutlet) {
      //      cg.status = catererOutlet.status;
      //    }
      //  }
      //});

      console.log("dos: ", this.dos)


      this.dosCache = this.dos;

    }, error => { });

  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  //requestCaterer(row: CatererInfo) {
  //  this.deliveryService.requestCaterer(row.id, this.editOutlet.id, 'REQUESTED').subscribe(results => {
  //    if (results.isSuccess) {
  //      row.status = 'REQUESTED';
  //    }
  //  }, error => {
  //      this.alertService.stopLoadingMessage();
  //      this.alertService.showStickyMessage("Request Error", `An error occured while requesting the caterer.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
  //        MessageSeverity.error);
  //  });
  //}

  onSearchChanged(value: string) {
    this.dos = this.dosCache.filter(r => Utilities.searchArray(value, false, r.doNumber));
  }


  receiveDO(row) {
    const dialogRef = this.dialog.open(ReceiveDOComponent, {
      data: { header: "Receive Delivery Order", do: row },
      width: '1200px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }
}
