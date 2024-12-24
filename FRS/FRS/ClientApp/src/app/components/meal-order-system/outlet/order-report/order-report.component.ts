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
import * as moment from 'moment';
import { saveAs } from 'file-saver';
import { MenuService } from '../../../../services/meal-order/menu.service';

@Component({
  selector: 'order-report',
  templateUrl: './order-report.component.html',
  styleUrls: ['./order-report.component.css']
})
export class OrderReportComponent implements OnInit {
  stores: StoreInfo[];
  storesCache: StoreInfo[];
  selectedStores: StoreInfo[] = [];
  editOutlet: Outlet;
  selectedCaterers: CatererOutlet[];
  startDate = moment();
  endDate = moment();

  constructor(private http: HttpClient, private alertService: AlertService, private deliveryService: DeliveryService, public dialog: MatDialog,
    public dialogRef: MatDialogRef<OrderReportComponent>,private menuService: MenuService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outlet) != typeof (undefined)) {
      this.editOutlet = data.outlet;
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

    //this.getStore(filterCaterer);
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    console.log("event value: ", moment(event.value))
    if (type == "start") {
      this.startDate = moment(event.value);
    } else if (type == "end") {
      this.endDate = moment(event.value);
    }
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  onSearchChanged(value: string) {
    this.stores = this.storesCache.filter(r => Utilities.searchArray(value, false, r.name));
  }


  downloadReport() {
    const fileName = moment().format('DDMMYYYY_hhmmss') + '_OrderReport.xlsx';

    var insideFilter = new Filter();

    var strStartDate = this.startDate.format().split('T');
    var strEndDate = this.endDate.format().split('T');
    insideFilter.filters = '(IsActive)==true,(outletId)==' + this.editOutlet.id + '(TokenOrderDateRange)==' + strStartDate[0] + '|' + strEndDate[0];
    insideFilter.filters += ',(Status)==paid';
    console.log("filters: ", insideFilter.filters)
    insideFilter.sorts = "DeliveryDate";

    this.menuService.downloadOrderReport(insideFilter).subscribe(
      data => {
        console.log(data);
        saveAs(data, fileName);
      },
      err => {
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }
}
