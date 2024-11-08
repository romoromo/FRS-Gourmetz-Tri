import { Component, OnInit, Output, EventEmitter, Input, Inject, TemplateRef, ViewChild } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Dish, DishDetail } from 'src/app/models/meal-order/dish.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { CartonAsset } from 'src/app/models/meal-order/carton-asset.model';
import { BentoAsset } from 'src/app/models/meal-order/bento-asset.model';
import { Student } from 'src/app/models/meal-order/student.model';
import { Class } from 'src/app/models/meal-order/class.model';
import { InterestGroup } from 'src/app/models/meal-order/interest-group.model';
import { StudentGroup, StudentGroupDetail } from 'src/app/models/meal-order/student-group.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { Outlet } from 'src/app/models/meal-order/outlet.model';
import { CatererInfo, CatererOutlet } from 'src/app/models/meal-order/caterer-info.model';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { ClassService } from 'src/app/services/meal-order/class.service';

@Component({
  selector: 'scan-tag',
  templateUrl: './scan-tag.component.html',
  styleUrls: ['./scan-tag.component.css']
})
export class ScanTagComponent implements OnInit {
  scanCartonCode = "";
  scanBentoCode = "";

  
  selectedBento: BentoAsset;
  filter: Filter;
  allCarton: CartonAsset[] = [];
  allBento: BentoAsset[] = [];
  outletId: string;
  routeId: string;
  packingDate: Date;
  dishId: string;
  isSaving = false;
  isLoading = false;
  bentoSaved: BentoAsset;
  step: number;
  dishCode: string;
  savedBentoCode: string;
  scannedQty = 0;
  totalQty = 0;

  selectedCartonIndex = -1;
  selectedBentoIndex = -1;
  //private allRowsSelected = false;


  constructor(private http: HttpClient,
    private alertService: AlertService, public dialogRef: MatDialogRef<ScanTagComponent>, private studentService: StudentService, private classService: ClassService, private deliveryService: DeliveryService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    console.log("data inside: ", data);
    this.outletId = data.outletId;
    this.routeId = data.routeId;
    this.dishId = data.dishId;
    this.packingDate = data.packingDate;
    this.dishCode = data.dishCode;
    this.scannedQty = data.scannedQty;
    this.totalQty = data.qty;
  }

  ngOnInit() {
    //this.initializeFilter();
    //this.initializePagedResult();
    //this.initializeTableDefinition();
    //this.loadData();
    //this.loadAllData();
    this.step = 1;

    this.getCartons();
    this.getBentos();
  }

  selectFn(ev) {
    console.log(ev);
  }

  onCheckboxChangeFn(ev) {
    console.log(ev);
  }

  //onSelect({ selected }) {
  //  console.log(selected);
  //  //this.selected = selected;
  //  let length = selected.length;
  //  if (selected.length == 0) length = this.rows.length;

  //  this.selected.splice(0, length);
  //  this.selected.push(...selected);
  //  this.rows.forEach(row => (row.checked = this.selected.findIndex(e => e.id == row.id) > -1));
  //}

  //getClasses() {
  //  let filter = new Filter();
  //  let f = this.outletId ? '(classOutletId)==' + this.outletId + ',' : '';
  //  filter.filters = f + '(IsActive)==true';

  //  this.classService.getClassesByFilter(filter)
  //    .subscribe(results => {
  //      this.classes = results.pagedData;
  //    },
  //      error => {
  //        //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
  //        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving classes.\r\n"`,
  //          MessageSeverity.error);
  //  });
  //}

  getBentos() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';

    this.deliveryService.getBentoAssetsByFilter(filter)
      .subscribe(results => {
        this.allBento = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving bento.\r\n"`,
            MessageSeverity.error);
        });
  }

  getCartons() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';

    this.deliveryService.getCartonAssetsByFilter(filter)
      .subscribe(results => {
        this.allCarton = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving carton.\r\n"`,
            MessageSeverity.error);
        });
  }

  public save = () => {
    //let studentIds= this.selected.map((e) => { return e.id; });

    //let students = this.rows.filter((cg) => (<any>cg).checked);

    this.alertService.startLoadingMessage("Saving bento...");

    console.log("allbenbto ", this.allBento);
    console.log("allCarton ", this.allCarton);

    console.log("bento Index", this.selectedBentoIndex);

    

    var savedBento = this.allBento[this.selectedBentoIndex]

    savedBento.routeId = this.routeId;
    savedBento.dishId = this.dishId;
    savedBento.lastPackingTime = this.packingDate;
    savedBento.cartonAssetId = this.allCarton[this.selectedCartonIndex].id;

    this.scannedQty += 1;

    this.deliveryService.updateBentoAsset(savedBento).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
  }

  private saveSuccessHelper(bento?: BentoAsset) {
    if (bento)
      Object.assign(this.bentoSaved, bento);


    this.savedBentoCode = this.scanBentoCode;
    this.scanBentoCode = null;
    this.isSaving = false;
    this.alertService.stopLoadingMessage();

    this.alertService.showMessage("Success", `Bento Assets is tagged`, MessageSeverity.success);

    //this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while tagging Bento Assets:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  }

  onChangeCarton() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Checking Carton...");

    this.selectedCartonIndex = this.allCarton.findIndex(e => e.code == this.scanCartonCode)

    if (this.selectedCartonIndex == -1) {
      this.isSaving = false;
      this.alertService.stopLoadingMessage();
      this.alertService.showStickyMessage("Check Error", "Carton Code " + this.scanCartonCode + " is not exist on Carton data, Please register the Carton first", MessageSeverity.error);
    } else {
      this.isSaving = false;
      this.alertService.stopLoadingMessage();

      this.step = 2;
    }
  }

  onChangeBento() {

    this.isSaving = true;
    this.alertService.startLoadingMessage("Checking Bento...");

    this.selectedBentoIndex = this.allBento.findIndex(e => e.code == this.scanBentoCode)

    if (this.selectedBentoIndex == -1) {
      this.isSaving = false;
      this.alertService.stopLoadingMessage();
      this.alertService.showStickyMessage("Check Error", "Bento Code " + this.scanBentoCode + " is not exist on Bento data, Please register the Bento first", MessageSeverity.error);
    } else {
      this.alertService.stopLoadingMessage();
      this.save();
    }

  }

  private cancel() {
    this.dialogRef.close({ isCancel: false, scannedQty: this.scannedQty });
  }
}
