import { Component, OnInit } from "@angular/core";
import { AccountService } from "../../../services/account.service";
import { Permission } from "src/app/models/permission.model";
import { MatDatepickerInputEvent } from "@angular/material";
import {
  FASMonthlyBillingFilter,
  Filter,
  PagedResult,
} from "src/app/models/sieve-filter.model";
import { ClassService } from "src/app/services/meal-order/class.service";
import { AlertService, MessageSeverity } from "src/app/services/alert.service";
import { Subscription } from "rxjs";
import { DeliveryService } from "src/app/services/meal-order/delivery.service";
import { StudentService } from "src/app/services/meal-order/student.service";
import { AppTranslationService } from "src/app/services/app-translation.service";
import { DatePipe, DecimalPipe } from "@angular/common";
import { FASMonthlyBillingReportModel } from "src/app/models/fas-monthly-billing-report.model";
import { AuditService } from "src/app/services/audit.service";
import { Utilities } from "src/app/services/utilities";
import { saveAs } from "file-saver";
import * as moment from "moment";

@Component({
  selector: "fas-mothly-billing-report-management",
  templateUrl: "./fas-mothly-billing-report-management.component.html",
  styleUrls: ["./fas-mothly-billing-report-management.component.css"],
  providers: [DecimalPipe, DatePipe],
})
export class FASMothlyBillingReportManagementComponent implements OnInit {
  private subscription: Subscription = new Subscription();
  filter = new FASMonthlyBillingFilter();
  start = new Date();
  end = new Date();
  tstart = new Date();
  tend = new Date();
  classLevels: any[] = [];
  classLevelIds: string = "";

  classLevelsGrouped: Array<{
    outletId: number;
    outletName: string;
    items: any[];
  }> = [];
  outlets: any[] = [];
  isLoading: boolean;
  loadingIndicator: boolean;
  pagedResult: PagedResult;
  columns: any[] = [];
  rows: FASMonthlyBillingReportModel[] = [];
  rowsCache: FASMonthlyBillingReportModel[] = [];

  constructor(
    private alertService: AlertService,
    private accountService: AccountService,
    private classService: ClassService,
    private deliveryService: DeliveryService,
    private studentService: StudentService,
    private translationService: AppTranslationService,
    private decimalPipe: DecimalPipe,
    private auditService: AuditService,
    private datePipe: DatePipe,
  ) {}

  ngOnInit() {
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.getOutlet();
  }

  getOutlet() {
    let filter = new Filter();
    filter.filters = `(IsActive)==true`;
    this.subscription.add(
      this.deliveryService.getOutletsSimpleByFilter(filter).subscribe(
        (results) => {
          this.outlets = results.pagedData;
        },
        (error) => {
          this.alertService.showStickyMessage(
            "Get Error",
            `An error occured while retrieving student outlets.\r\n"`,
            MessageSeverity.error,
          );
        },
      ),
    );
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
      { prop: "studentID", name: "Student ID", sortable: false },
      { prop: "studentName", name: "Name", sortable: false },
      { prop: "classLevel", name: "Class Level", sortable: false },
      { prop: "class", name: "Class", sortable: false },
      {
        prop: "fasStudent",
        name: "FAS Student",
        sortable: false,
        pipe: {
          transform: (val: boolean) => (val ? "Y" : "N"),
        },
      },
      {
        prop: "deliveryDate",
        name: "Delivery Date",
        sortable: false,
        pipe: {
          transform: (val: string) => this.datePipe.transform(val, 'dd/MM/yyyy HH:mm:ss')
        }
      },
      {
        prop: "mealType",
        name: "Meal Type",
        sortable: false,
      },
      {
        prop: "mealName",
        name: "Meal Name",
        sortable: false,
      },
      {
        prop: "qty",
        name: "QTY",
        sortable: false,
      },
      {
        prop: "price",
        name: "Price",
        pipe: {
          transform: (val: number) => {
            if (val === 0 || val === null || val === undefined) {
              return "-";
            }
            return this.decimalPipe.transform(val, "1.2-2");
          },
        },
        sortable: false,
      },
      {
        prop: "invoiceNumber",
        name: "Invoice Number",
        sortable: false,
      },
      {
        prop: "posInvoiceNumber",
        name: "POS Invoice Number",
        sortable: false,
      },
      {
        prop: "amount",
        name: "Amount",
        pipe: {
          transform: (val: number) => {
            if (val === 0 || val === null || val === undefined) {
              return "-";
            }
            return this.decimalPipe.transform(val, "1.2-2");
          },
        },
        sortable: false,
      }      
    ];
  }

  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = 10;
    this.filter.page = 1;

    this.filter.startDate = this.start.toDateString();
    this.filter.endDate = this.end.toDateString();

    if (ev) {
     this.filter.page = ev.offset + 1;
     if (ev.sorts) {
       this.filter.sorts =
         ev.sorts[0].dir == "desc" ? "-" + ev.sorts[0].prop : ev.sorts[0].prop;
     }
    }

    this.auditService.getFasMothlyBillingReport(this.filter).subscribe(
     (results) => {
       this.pagedResult = results;

       this.alertService.stopLoadingMessage();
       this.loadingIndicator = false;

       let mealTypes = results.pagedData;

       mealTypes.forEach((mealType, index, mealTypes) => {
         (<any>mealType).index = index + 1;
       });

       this.rowsCache = [...mealTypes];
       this.rows = mealTypes;
     },
     (error) => {
       this.alertService.stopLoadingMessage();
       this.loadingIndicator = false;

       this.alertService.showStickyMessage(
         "Load Error",
         `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
         MessageSeverity.error,
       );
     },
    );
  }

  clearFilters() {
    this.start = new Date();
    this.end = new Date();
    this.classLevels = [];
    this.classLevelsGrouped = [];
    this.filter.outletId = [];
    this.filter.classLevelIds = [];

    this.loadData();
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == "start") {
      this.start = new Date(event.value);
    }

    if (type == "end") {
      this.end = new Date(event.value);
    }

    if (type == "tstart") {
      this.tstart = new Date(event.value);
    }

    if (type == "tend") {
      this.tend = new Date(event.value);
    }
  }

  getGetSelectedOutlet(): void {
    const outletSelected = this.filter.outletId || [];
    if (outletSelected.length === 0) {
      this.classLevels = [];
      this.classLevelsGrouped = [];
      return;
    }

    const outletFilter = this.buildOutletFilterFromSelection();
    this.getClassLevel(outletFilter);
  }

  private buildOutletFilterFromSelection(): string {
    const selected = this.filter.outletId || [];

    if (selected.length === 0) {
      return "";
    }

    const outletIds = selected.filter((x) => typeof x === "number") as number[];
    if (outletIds.length === 0) {
      return "";
    }

    return `OutletId==${outletIds.join("|")},`;
  }

  toggleAllSelection(isSelected: boolean) {
    let outletFilter = "OutletId==0,";
    if (isSelected) {
      // Set the model to all IDs plus the 'all' value to keep it visually checked
      const allIds = this.outlets.map((o) => o.id);
      this.filter.outletId = [...allIds, "all"];

      outletFilter = this.buildOutletFilterFromSelection();
    } else {
      // Clear the model
      this.filter.outletId = [];
    }

    this.getClassLevel(outletFilter);
  }

  private groupClassLevelsByOutlet(list: any[]) {
    const map = new Map<
      number,
      { outletId: number; outletName: string; items: any[] }
    >();

    (list || []).forEach((x) => {
      const outletId = x.outletId;
      if (!map.has(outletId)) {
        map.set(outletId, {
          outletId,
          outletName: x.outletName || `Outlet ${outletId}`,
          items: [],
        });
      }
      map.get(outletId)!.items.push(x);
    });

    // optional: sort outletName dan item.name
    this.classLevelsGrouped = Array.from(map.values())
      .sort((a, b) => a.outletName.localeCompare(b.outletName))
      .map((g) => ({
        ...g,
        items: g.items.sort((a, b) =>
          (a.name || "").localeCompare(b.name || ""),
        ),
      }));
  }

  getClassLevel(outletFilter) {
    this.isLoading = true;

    const filter = new Filter();
    filter.filters =
      outletFilter +
      `(IsActive)==true,(InstitutionId)==${this.accountService.currentUser.institutionId}`;

    this.classService.getClassLevelsByFilter(filter).subscribe(
      (results) => {
        const data = results.pagedData || [];

        this.classLevels = results.pagedData;
        this.groupClassLevelsByOutlet(data);

        this.isLoading = false;
      },
      (error) => {
        this.alertService.showStickyMessage(
          "Get Error",
          `An error occured while retrieving class levels.\r\n`,
          MessageSeverity.error,
        );

        this.isLoading = false;
      },
    );
  }

  downloadWalletTransaction() {
    this.filter.startDate = this.start.toDateString();
    this.filter.endDate = this.end.toDateString();
    this.filter.page = 1;
    this.isLoading = true;
    this.alertService.startLoadingMessage("Downloading...");
    const fileName =
     moment().format("DDMMYYYY_hhmmss") + "_FAS-Monthly-Billing-Report.xlsx";

    this.auditService.generateFasMothlyBillingReport(this.filter).subscribe(
     (data) => {
       console.log(data);
       saveAs(data, fileName);

       this.alertService.stopLoadingMessage();
       this.isLoading = false;
     },
     (err) => {
       this.alertService.stopLoadingMessage();
       alert("Problem while downloading the file.");
       console.error(err);
       this.isLoading = false;
     },
    );
  }

  get canManageEmailTemplates() {
    return this.accountService.userHasPermission(
      Permission.viewFASMonthlyBillingReport,
    );
  }
}
