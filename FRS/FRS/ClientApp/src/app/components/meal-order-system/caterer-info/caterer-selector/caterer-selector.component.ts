import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Filter } from 'src/app/models/sieve-filter.model';
import { CatererInfo, CatererOutlet } from 'src/app/models/meal-order/caterer-info.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Outlet, OutletProfile } from 'src/app/models/meal-order/outlet.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';

@Component({
  selector: 'caterer-selector',
  templateUrl: './caterer-selector.component.html',
  styleUrls: ['./caterer-selector.component.css']
})
export class CatererApprovalComponent implements OnInit {
  outlets: Outlet[];
  //out: CatererInfo[];
  editCaterer: Outlet;
  selectedOutlets: CatererOutlet[];
  private outletProfiles: OutletProfile[] = [];
  id: string;
  constructor(private http: HttpClient, private alertService: AlertService, private deliveryService: DeliveryService,
    public dialogRef: MatDialogRef<CatererApprovalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.getOutletProfiles();
    this.id = data.caterer.id;
    //if (typeof (data.caterer) != typeof (undefined)) {
    //  this.editCaterer = data.caterer;
    //  if (!data.caterer.catererOutlets) {
    //    this.editCaterer.catererOutlets = [];
    //  }
    //  this.selectedOutlets = this.editCaterer.catererOutlets;
    //}
    this.getCatererById();
  }

  ngOnInit() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';

    //this.deliveryService.getCatererInfosByFilter(filter).subscribe(results => {
    //  this.outlets = results.pagedData;
      
    //  this.outlets.map(cg => {
    //    if (this.editCaterer.catererOutlets && typeof (this.editCaterer.catererOutlets) != typeof (undefined)) {
    //      let catererOutlet = this.editCaterer.catererOutlets.find(item => item.outletId == cg.id);
    //      if (catererOutlet) {
    //        cg.status = catererOutlet.status;
    //      }
    //    }
    //  });
      

    //  this.caterersCache = this.caterers;

    //}, error => { });
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  getCatererById() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';

    this.deliveryService.getCatererInfoById(this.id)
      .subscribe(results => {
        if (results) {
          this.editCaterer = results;
          this.selectedOutlets = this.editCaterer.catererOutlets;
        }
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving caterers.\r\n"`,
            MessageSeverity.error);
        })
  }

  getOutletProfiles() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';

    this.deliveryService.getOutletProfilesByFilter(filter)
      .subscribe(results => {
        this.outletProfiles = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving outlet profiles.\r\n"`,
            MessageSeverity.error);
        })
  }

  approve(row) {
    if (row && row.outletProfileId) {
      this.deliveryService.requestCaterer(row.catererInfoId, row.outletId, 'APPROVED', row.isRsp, row.outletProfileId).subscribe(results => {
        if (results.isSuccess) {
          row.status = 'APPROVED';
        }
      }, error => {
        this.alertService.stopLoadingMessage();
        this.alertService.showStickyMessage("Request Error", `An error occured while requesting the caterer.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          MessageSeverity.error);
      });
    } else {
      row.showValidationErrors = true;
    }
  }

  reject(row) {
    row.showValidationErrors = false;
    this.deliveryService.requestCaterer(row.catererInfoId, row.outletId, 'REJECTED', row.isRsp, null).subscribe(results => {
      if (results.isSuccess) {
        row.status = 'REJECTED';
      }
    }, error => {
      this.alertService.stopLoadingMessage();
      this.alertService.showStickyMessage("Request Error", `An error occured while requesting the caterer.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
        MessageSeverity.error);
    });

  }

  outletProfileByCaterer(row) {
    return this.outletProfiles ? this.outletProfiles.filter(e => e.catererId == row.catererInfoId) : [];
  }

  //onSearchChanged(value: string) {
  //  this.outlets = this.caterersCache.filter(r => Utilities.searchArray(value, false, r.name));
  //}
}
