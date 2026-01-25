import {
  Component,
  OnInit,
  AfterViewInit,
  TemplateRef,
  ViewChild,
  Input,
  OnDestroy,
} from "@angular/core";
import { ModalDirective } from "ngx-bootstrap/modal";

import {
  AlertService,
  DialogType,
  MessageSeverity,
} from "../../../services/alert.service";
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from "../../../services/account.service";
import { Utilities } from "../../../services/utilities";
import {
  Filter,
  PagedResult,
  SalesOrderReportFilter,
} from "../../../models/sieve-filter.model";
import { Permission } from "../../../models/permission.model";
import { MatDatepickerInputEvent, MatDialog } from "@angular/material";
import { AuditService } from "src/app/services/audit.service";
import { DateOnlyPipe, DateTimeOnlyPipe } from "src/app/pipes/datetime.pipe";
import { Subscription } from "rxjs";
import { SearchBoxComponent } from "../../controls/search-box.component";
import { saveAs } from "file-saver";
import * as moment from "moment";
import { TokenOrder } from "src/app/models/meal-order/token-order.model";
import { StudentService } from "src/app/services/meal-order/student.service";
import { PaymentTypes } from "../../../models/enums";
import { FormControl } from "@angular/forms";
import { OrderService } from "src/app/services/meal-order/order.service";
import { DeliveryService } from "src/app/services/meal-order/delivery.service";
import { MatOption } from "@angular/material/core";
import { ClassService } from "src/app/services/meal-order/class.service";

@Component({
  selector: "order-logs-management",
  templateUrl: "./order-logs-management.component.html",
  styleUrls: ["./order-logs-management.component.css"],
})
export class OrderLogsManagementComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  statuses = ["All", "pending", "paid", "cancelled"];
  ordertypes = [
    "All",
    PaymentTypes.Adhoc,
    PaymentTypes.Fas,
    PaymentTypes.MealPlan,
  ];
  columns: any[] = [];
  rows: TokenOrder[] = [];
  rowsCache: TokenOrder[] = [];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: SalesOrderReportFilter;
  pagedResult: PagedResult;
  keyword: string = "";
  status: string = "paid";
  ordertype: string = "All";
  isFAS: boolean = false;
  start = new Date();
  end = new Date();
  groups: any[] = [];
  outlets: any[] = [];
  tstart = new Date();
  tend = new Date();
  studentGroupIds = new FormControl();
  outletId: string = "";

  classLevels: any[] = [];
  classLevelIds: string = "";

  public currentPageLimit: number = 10;
  public pageLimitOptions = [
    { value: 5 },
    { value: 10 },
    { value: 25 },
    { value: 50 },
    { value: 100 },
  ];

  @ViewChild("searchbox") searchbox: SearchBoxComponent;

  @ViewChild("indexTemplate")
  indexTemplate: TemplateRef<any>;

  @ViewChild("actionsTemplate")
  actionsTemplate: TemplateRef<any>;

  @ViewChild("editorModal")
  editorModal: ModalDirective;

  @ViewChild("mealDescription")
  mealDescriptionTemplate: TemplateRef<any>;

  @ViewChild("orderLogTable") table: any;

  private selected: any[] = [];
  @ViewChild("hdrTpl") hdrTpl: TemplateRef<any>;

  @ViewChild("allSelected") private allSelected!: MatOption;

  classLevelsGrouped: Array<{
    outletId: number;
    outletName: string;
    items: any[];
  }> = [];

  constructor(
    private alertService: AlertService,
    private translationService: AppTranslationService,
    private accountService: AccountService,
    private orderLogService: AuditService,
    private studentService: StudentService,
    private orderService: OrderService,
    private deliveryService: DeliveryService,
    private classService: ClassService,
  ) {}

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new SalesOrderReportFilter(1, 10);
    this.filter.sorts = "-invoiceNumber";
    this.filter.filters = "";
    this.filter.page = 1;
    this.filter.studentGroupIds = [];
    this.filter.collectionStatuses = [];
    this.isFAS = false;
    this.status = "paid";
    this.ordertype = "";
    this.outletId = "";
  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.filter.page = 1;
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      //{ prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      {
        width: 50,
        cellTemplate: this.indexTemplate,
        canAutoResize: false,
        headerCheckboxable: true,
        headerTemplate: this.hdrTpl,
      },
      { prop: "className", name: "Class" },
      { prop: "profileName", name: "Profile" },
      { prop: "isFASDisplay", name: "FAS", sortable: false },
      {
        prop: "deliveryDate",
        name: "Delivery Date",
        pipe: new DateOnlyPipe("en-SG"),
      },
      {
        prop: "transactionTime",
        name: "Order Date",
        pipe: new DateOnlyPipe("en-SG"),
      },
      { prop: "mealSessionName", name: "Session" },
      //{ prop: 'mealDescription', name: 'Meal Description', cellTemplate: this.mealDescriptionTemplate, sortable: false, draggable: false },
      {
        prop: "mealDescription",
        name: "Meal Description",
        sortable: false,
        draggable: false,
      },
      { prop: "discount", name: "Discount" },
      { prop: "totalAmount", name: "Total Amount" },
      { prop: "status", name: "Status" },
      //{ prop: 'remarks', name: 'Remarks' },
      { prop: "paymentMethod", name: "Payment Type" },
      { prop: "paymentNumber", name: "Order No." },
      { prop: "fomoId", name: "Fomo ID" },
      { prop: "invoiceNumber", name: "Invoice No." },
      { prop: "voucherCode", name: "Voucher" },
      { prop: "cancellationReason", name: "Cancellation Reason" },
      //{ prop: 'processedBy', name: 'Processed By' }
    ];
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  public onLimitChange(limit: any): void {
    this.changePageLimit(limit);
    this.table.limit = this.currentPageLimit;
    this.table.recalculate();
    setTimeout(() => {
      if (this.table.bodyComponent.temp.length <= 0) {
        this.table.offset = Math.floor(
          (this.table.rowCount - 1) / this.table.limit,
        );
      }

      this.loadData();
    });
  }

  private changePageLimit(limit: any): void {
    this.currentPageLimit = parseInt(limit, 10);
  }

  ngOnInit() {
    this.getStudentGroups();
    this.getOutlet();
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.loadData();
  }

  loadData(ev?: any) {
    this.selected = [];
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = this.currentPageLimit;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (ev.sorts) {
        this.filter.sorts =
          ev.sorts[0].dir == "desc" ? "-" + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = "";
    this.filter.filters =
      "(profileName|processedBy|className|invoiceNumber|voucherCode)@=" +
      this.keyword +
      ",(IsActive)==true,(status)==" +
      (this.status == "All" ? "" : this.status) +
      ",(orderType)==" +
      (this.ordertype == "All" ? "" : this.ordertype) +
      ",(AuditOrderLogDateRange)==" +
      this.start.toDateString() +
      "|" +
      this.end.toDateString();

    if (this.isFAS) {
      this.filter.filters += ",(isFas)==true";
    }

    this.filter.reportDateFrom = this.start.toDateString();
    this.filter.reportDateTo = this.end.toDateString();
    this.filter.status = this.status == "All" ? "" : this.status;
    this.filter.orderType = this.ordertype == "All" ? "" : this.ordertype;
    this.filter.keyword = this.keyword;
    if (this.isFAS) this.filter.isFas = true;
    //this.filter.outletId = this.outletId;

    this.orderLogService.getOrderLogsByFilter(this.filter).subscribe(
      (results) => {
        this.pagedResult = results;
        console.log(this.pagedResult);
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let orderLogs = results.pagedData;

        orderLogs.forEach((orderLog, index, orderLogs) => {
          (<any>orderLog).index = index + 1;
        });

        this.rowsCache = [...orderLogs];
        this.rows = orderLogs;
      },
      (error) => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.alertService.showStickyMessage(
          "Load Error",
          `Unable to retrieve order logs from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(
            error,
          )}"`,
          MessageSeverity.error,
        );
      },
    );
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

    //this.loadData();

    //this.loadData();
  }

  isSelected(id: any): boolean {
    return this.filter.studentGroupIds.includes(id);
  }

  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    //this.loadData(null);
  }

  onSearch() {
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  clearFilters() {
    this.start = new Date();
    this.end = new Date();
    this.keyword = "";
    this.clearFilterAndPagedResult();
    this.searchbox.clear();
  }

  getStudentGroups() {
    let filter = new Filter();
    filter.sorts = "name";
    filter.filters = "(IsActive)==true";
    this.subscription.add(
      this.studentService.getStudentGroupsSimpleByFilter(filter).subscribe(
        (results) => {
          this.groups = results.pagedData;
          console.log("groups: ", this.groups);
        },
        (error) => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage(
            "Get Error",
            `An error occured while retrieving student groups.\r\n"`,
            MessageSeverity.error,
          );
        },
      ),
    );
  }

  getOutlet() {
    let filter = new Filter();
    filter.filters = `(IsActive)==true,(Name)@=${this.keyword},(OutletByUserId)==${this.accountService.currentUser.id}`;
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

  downloadResults() {
    const fileName = moment().format("DDMMYYYY_hhmmss") + "_Orders.xlsx";
    this.filter.page = null;
    this.filter.pageSize = null;
    this.filter.reportDateFrom = this.start.toDateString();
    this.filter.reportDateTo = this.end.toDateString();
    this.filter.status = this.status == "All" ? "" : this.status;
    this.filter.orderType = this.ordertype == "All" ? "" : this.ordertype;
    this.filter.keyword = this.keyword;
    //this.filter.outletId = this.outletId;
    if (this.isFAS) this.filter.isFas = true;
    this.orderLogService.downloadOrderLogsReport(this.filter).subscribe(
      (data) => {
        console.log(data);
        saveAs(data, fileName);
      },
      (err) => {
        alert("Problem while downloading the file.");
        console.error(err);
      },
    );
  }

  downloadFlattentResults() {
    const fileName = moment().format("DDMMYYYY_hhmmss") + "_OrdersDetails.xlsx";
    this.filter.page = null;
    this.filter.pageSize = null;
    this.filter.reportDateFrom = this.start.toDateString();
    this.filter.reportDateTo = this.end.toDateString();
    this.filter.status = this.status == "All" ? "" : this.status;
    this.filter.orderType = this.ordertype == "All" ? "" : this.ordertype;
    this.filter.keyword = this.keyword;
    //this.filter.outletId = this.outletId;
    if (this.isFAS) this.filter.isFas = true;
    this.orderLogService.downloadFlattenOrderLogsReport(this.filter).subscribe(
      (data) => {
        console.log(data);
        saveAs(data, fileName);
      },
      (err) => {
        alert("Problem while downloading the file.");
        console.error(err);
      },
    );
  }

  onSelect({ selected }) {
    console.log(selected);
    this.selected = selected;
  }

  onCheckboxChangeFn(ev) {
    console.log(ev);
  }

  cancelSelectedOrders() {
    if (this.selected.length < 1) {
      this.alertService.showMessage(
        "Cancel Order",
        "Please select at least 1 order to cancel.",
        MessageSeverity.warn,
      );
      return;
    }

    const notPaid = this.selected.filter(
      (x) => x.status.toLowerCase() !== "paid",
    );
    if (notPaid.length) {
      this.alertService.showMessage(
        "Cancel Order",
        `Only orders with status "paid" can be cancelled. Please unselect the non-paid orders.`,
        MessageSeverity.warn,
      );
      return;
    }

    const ok = window.confirm(
      `Are you sure you want to cancel selected order/s? (Total: ${this.selected.length})`,
    );
    if (!ok) return;

    this.alertService.showDialog(
      "Enter cancellation reason",
      DialogType.prompt,
      (reason) => {
        if (!reason) return;

        this.alertService.startLoadingMessage("Cancelling selected order/s.");
        this.loadingIndicator = true;

        let orderIds = this.selected.map((e) => {
          return e.id;
        });

        let model = {
          orderIds: orderIds,
          cancelledBy: this.accountService.currentUser.id,
          reason: reason,
        };
        this.subscription.add(
          this.orderService.cancelOrders(model).subscribe(
            (results) => {
              this.alertService.stopLoadingMessage();
              this.loadingIndicator = false;

              this.alertService.showMessage(
                "Success",
                "Selected order/s successfully cancelled.",
                MessageSeverity.success,
              );

              this.loadData();
            },
            (error) => {
              this.alertService.stopLoadingMessage();
              this.loadingIndicator = false;
              this.alertService.showStickyMessage(
                "Cancel Order",
                `An error occured while cancelling the orderss\r\nError: "${Utilities.getHttpResponseMessage(
                  error,
                )}"`,
                MessageSeverity.error,
              );
            },
          ),
        );
      },
    );
  }

  toggleAllSelection(isSelected: boolean) {
    let outletFilter = 'OutletId==0,';
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

  private buildOutletFilterFromSelection(): string {
    const selected = this.filter.outletId || [];

    if (selected.length === 0) {
      return "";
    }

    const outletIds = selected.filter((x) => typeof x === "number") as number[];
    if (outletIds.length === 0) {
      return "";
    }

    return `OutletId==${outletIds.join('|')},`;
  }

  getGetSelectedOutlet(): void {
    const outletSelected = this.filter.outletId || [];
    if (outletSelected.length === 0) {
      this.classLevels = [];
      this.classLevelsGrouped = [];
      return;
    }

    const hasAll = outletSelected.some((x) => x.toString() === "all");
    if (hasAll) {
      this.classLevels = [];
      this.classLevelsGrouped = [];
      return;
    }

    const outletFilter = this.buildOutletFilterFromSelection();
    this.getClassLevel(outletFilter);
  }

  private groupClassLevelsByOutlet(list: any[]) {
    const map = new Map<number, { outletId: number; outletName: string; items: any[] }>();

    (list || []).forEach(x => {
      const outletId = x.outletId;
      if (!map.has(outletId)) {
        map.set(outletId, {
          outletId,
          outletName: x.outletName || `Outlet ${outletId}`,
          items: []
        });
      }
      map.get(outletId)!.items.push(x);
    });

    // optional: sort outletName dan item.name
    this.classLevelsGrouped = Array.from(map.values())
      .sort((a, b) => a.outletName.localeCompare(b.outletName))
      .map(g => ({
        ...g,
        items: g.items.sort((a, b) => (a.name || '').localeCompare(b.name || ''))
      }));
  }

  getClassLevel(outletFilter) {
    const filter = new Filter();
    filter.filters =
      outletFilter +
      `(IsActive)==true,(InstitutionId)==${this.accountService.currentUser.institutionId}`;

    this.classService.getClassLevelsByFilter(filter).subscribe(
      (results) => {

        const data = results.pagedData || [];

        this.classLevels = results.pagedData;
        this.groupClassLevelsByOutlet(data);
      },
      (error) => {
        this.alertService.showStickyMessage(
          "Get Error",
          `An error occured while retrieving class levels.\r\n`,
          MessageSeverity.error,
        );
      },
    );
  }

  get canManageAuthLogs() {
    return true; // this.accountService.userHasPermission(Permission.viewAuthLogsPermission)
  }
}
