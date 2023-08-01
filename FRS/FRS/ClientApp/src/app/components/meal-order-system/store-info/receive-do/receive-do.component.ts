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
import { StoreInventory, StoreInventoryDetail } from '../../../../models/meal-order/store-inventory.model';
import * as moment from 'moment';

@Component({
  selector: 'receive-do',
  templateUrl: './receive-do.component.html',
  styleUrls: ['./receive-do.component.css']
})
export class ReceiveDOComponent implements OnInit {
  private isSaving: boolean;
  editDo: DeliveryOrder;
  inventories: StoreInventory[];
  editInventory: StoreInventory;
  doDishes = [];
  doCartons = [];
  countDish = {};
  countCarton = {};

  constructor(private http: HttpClient, private alertService: AlertService, private deliveryService: DeliveryService, public dialog: MatDialog,
    public dialogRef: MatDialogRef<ReceiveDOComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.do) != typeof (undefined)) {
      this.editDo = data.do;
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
    filter.filters = '(IsActive)==true,(DeliveryOrderID)==' + this.editDo.id;

    this.deliveryService.getStoreInventoriesByFilter(filter).subscribe(results => {
      this.inventories = results.pagedData;

      //this.dos.map(cg => {
      //  if (this.editOutlet.catererOutlets && typeof (this.editOutlet.catererOutlets) != typeof (undefined)) {
      //    let catererOutlet = this.editOutlet.catererOutlets.find(item => item.catererInfoId == cg.id);
      //    if (catererOutlet) {
      //      cg.status = catererOutlet.status;
      //    }
      //  }
      //});

      console.log(this.inventories)

      if (this.inventories.length == 0) {
        this.getDoDishes();
        this.editInventory = new StoreInventory();
      } else {
        this.editInventory = this.inventories[0];
        this.getSIDishes();
      }

    }, error => { });

  }

  getDoDishes() {
    this.editDo.deliveryDetails.forEach((dod, index, periods) => {
      dod.deliveryBentos.forEach((db, index2, periods2) => {
        if (!this.countDish[db.dishId]) {
          db.qty = 1;
          db.cartonId = dod.cartonAssetId;
          this.countDish[db.dishId] = db;
        } else {
          this.countDish[db.dishId].qty = this.countDish[db.dishId].qty + 1;
        }
      });
      if (!this.countCarton[dod.cartonAssetId]) {
        dod.qty = 1;
        this.countCarton[dod.cartonAssetId] = dod;
      } else {
        this.countCarton[dod.cartonAssetId].qty = this.countCarton[dod.cartonAssetId].qty + 1;
      }
    });

    console.log("count Dish: ", this.countDish)

    console.log("count Carton: ", this.countCarton)

    for (let [key, value] of Object.entries(this.countCarton)) {
      console.log(key, value)
      let carton = {
        cartonId: key,
        cartonCode: (value as any).cartonAssetCode,
        qtyExpected: (value as any).qty,
        qtyReceived: 0,
        qtyToReceive: 0
      }
      this.doCartons.push(carton);
    }

    for (let [key, value] of Object.entries(this.countDish)) {
      console.log(key, value)
      let dish = {
        dishId: key,
        dishCode: (value as any).dishCode,
        cartonId: (value as any).cartonId,
        qtyExpected: (value as any).qty,
        qtyReceived: 0,
        qtyToReceive: 0
      }
      this.doDishes.push(dish);
    }

    console.log("do Cartons: ", this.doCartons)

    console.log("do Dishes: ",this.doDishes )
  }

  getSIDishes() {
    this.editInventory.storeInventoryDetails.forEach((detail) => {
      if ((detail as any).dish) {
        (detail as any).qtyToReceive = 0;
        (detail as any).dishCode = (detail as any).dish.code;
        this.doDishes.push(detail);
      } else if ((detail as any).carton) {
        (detail as any).qtyToReceive = 0;
        (detail as any).cartonCode = (detail as any).carton.code;
        this.doCartons.push(detail);
      }
    });
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }


  private save() {
    var complete = true;
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.editInventory.timeReceived = moment().utc().local().toDate();
    let allDetail = [...this.doDishes, ...this.doCartons];
    allDetail.forEach((sid) => {
      sid.qtyReceived = sid.qtyReceived + sid.qtyToReceive;
      sid.timeReceived = this.editInventory.timeReceived;
      sid.storeInfoId = this.editInventory.storeInfoId;
      if (sid.qtyExpected > sid.qtyReceived) {
        complete = false;
      }
    })
    this.editInventory.storeInventoryDetails = allDetail;
    this.editInventory.deliveryOrderId = this.editDo.id;
    this.editInventory.storeInfoId = this.editDo.toStoreId;
    console.log(this.editInventory);


    if (this.editInventory.id) {
      this.deliveryService.updateStoreInventory(this.editInventory).subscribe(storeInventory => this.saveSuccessHelper(this.editInventory), error => this.saveFailedHelper(error));
    } else {
      this.deliveryService.newStoreInventory(this.editInventory).subscribe(storeInventory => this.saveSuccessHelper(this.editInventory), error => this.saveFailedHelper(error));
    }

    if (complete) {
      this.editDo.completed = true;
      this.deliveryService.updateDeliveryOrder(this.editDo).subscribe();
    }
  }


  private saveSuccessHelper(storeInventory?: StoreInventory) {
    if (storeInventory)
      Object.assign(this.editInventory, storeInventory);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();

    this.alertService.showMessage("Success", `Sessions were updated successfully`, MessageSeverity.success);


    this.editInventory = new StoreInventory();


    this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  }

  canManageReceive() {
    return true;
  }
}
