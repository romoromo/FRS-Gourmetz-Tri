import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Filter } from 'src/app/models/sieve-filter.model';
import { CatererInfo, CatererOutlet } from 'src/app/models/meal-order/caterer-info.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Outlet } from 'src/app/models/meal-order/outlet.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { MealSessionEditorComponent } from '../meal-session/meal-session-editor.component';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { getBaseUrl } from 'src/app/app.module';

@Component({
  selector: 'caterer-selector',
  templateUrl: './caterer-selector.component.html',
  styleUrls: ['./caterer-selector.component.css']
})
export class CatererSelectorComponent implements OnInit {
  caterers: CatererInfo[];
  caterersCache: CatererInfo[];
  editOutlet: Outlet;
  selectedCaterers: CatererOutlet[];
  catererId: string;
  constructor(private http: HttpClient, private alertService: AlertService, private deliveryService: DeliveryService,
    public dialogRef: MatDialogRef<CatererSelectorComponent>, public dialog: MatDialog, private configurationService: ConfigurationService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outlet) != typeof (undefined)) {

      this.alertService.startLoadingMessage("Loading outlet details...");
      this.deliveryService.getOutletById(data.outlet.id).subscribe(results => {
        this.editOutlet = results;
        this.alertService.stopLoadingMessage();
        this.alertService.startLoadingMessage("Loading caterer information");
        this.selectedCaterers = results.catererOutlets;

        let filter = new Filter();
        filter.sorts = 'name';
        //let f = this.catererId ? '(Id)==' + this.catererId + ',' : '';
        filter.filters = '(IsActive)==true';

        this.deliveryService.getCatererInfosByFilter(filter).subscribe(results => {
          this.caterers = results.pagedData;

          this.caterers.map(cg => {
            if (this.editOutlet.catererOutlets && typeof (this.editOutlet.catererOutlets) != typeof (undefined)) {
              let catererOutlet = this.editOutlet.catererOutlets.find(item => item.catererInfoId == cg.id && item.outletId == this.editOutlet.id);
              if (catererOutlet) {
                cg.status = catererOutlet.status;
              }
            }
          });


          this.caterersCache = this.caterers;
          this.alertService.stopLoadingMessage();

        }, error => { this.alertService.stopLoadingMessage(); });

      }, error => { this.alertService.stopLoadingMessage(); });

      //this.editOutlet = data.outlet;
      ////this.catererId = data.catererId;
      //if (data.catererOutlets) {
      //  this.editOutlet.catererOutlets = [];
      //}
      //this.selectedCaterers = data.outlet.catererOutlets;
    }
  }

  ngOnInit() {
    
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  requestCaterer(row: CatererInfo) {
    this.deliveryService.requestCaterer(row.id, this.editOutlet.id, 'REQUESTED', false, '').subscribe(results => {
      if (results.isSuccess) {
        row.status = 'REQUESTED';
      }
    }, error => {
        this.alertService.stopLoadingMessage();
        this.alertService.showStickyMessage("Request Error", `An error occured while requesting the caterer.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          MessageSeverity.error);
    });

  }

  onSearchChanged(value: string) {
    this.caterers = this.caterersCache.filter(r => Utilities.searchArray(value, false, r.name));
  }

  openOutletCalendar(caterer) {
    window.open(getBaseUrl() + '/outletcalendars/' + this.editOutlet.id + '/' + caterer.id, '_blank');
  }

  openOutletDishCycleCalendar(caterer) {
    window.open(getBaseUrl() + '/outletdishcalendars/' + this.editOutlet.id + '/' + caterer.id, '_blank');
  }

  openMealSession(row) {
    let outletProfileId = '';
    let catererId = '';
    let catererOutlet: CatererOutlet;
    if (row.catererOutlets && typeof (row.catererOutlets) != typeof (undefined)) {
      catererOutlet = row.catererOutlets.find(item => item.catererInfoId == row.id && item.outletId == this.editOutlet.id);
      if (catererOutlet) {
        outletProfileId = catererOutlet.outletProfileId;
        catererId = catererOutlet.catererInfoId;
      }
    }

    catererOutlet.outlet.mealSessions = catererOutlet.outlet.mealSessions.filter(e => e.catererId == catererId);
    const dialogRef = this.dialog.open(MealSessionEditorComponent, {
      data: { header: "Meal Sessions", outlet: catererOutlet.outlet, outletProfileId: outletProfileId, catererId: catererId, outletId: this.editOutlet.id },
      width: '85vw',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      
    });
  }
}
