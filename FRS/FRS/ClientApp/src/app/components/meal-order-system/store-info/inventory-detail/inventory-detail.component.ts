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

@Component({
  selector: 'inventory-detail',
  templateUrl: './inventory-detail.component.html',
  styleUrls: ['./inventory-detail.component.css']
})
export class InventoryDetailComponent implements OnInit {
  private isSaving: boolean;
  editInventory: StoreInventoryDetail;
  doDishes = [];
  doCartons = [];
  countDish = {};
  countCarton = {};


  constructor(private http: HttpClient, private alertService: AlertService, private deliveryService: DeliveryService, public dialog: MatDialog,
    public dialogRef: MatDialogRef<InventoryDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.do) != typeof (undefined)) {
      this.editInventory = data.do;
    }
  }

  ngOnInit() {
    this.loadData();
  }

  loadData() {

  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  canManageReceive() {
    return true;
  }
}
