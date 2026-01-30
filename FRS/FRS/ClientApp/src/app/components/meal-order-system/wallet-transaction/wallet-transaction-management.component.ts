import { Component, OnInit } from "@angular/core";
import { AccountService } from "../../../services/account.service";
import { Permission } from "src/app/models/permission.model";
import { MatDatepickerInputEvent } from "@angular/material";
import {
  Filter,
  WalletTransactionFilter,
} from "src/app/models/sieve-filter.model";
import { ClassService } from "src/app/services/meal-order/class.service";
import { AlertService, MessageSeverity } from "src/app/services/alert.service";
import { Subscription } from "rxjs";
import { DeliveryService } from "src/app/services/meal-order/delivery.service";
import * as moment from "moment";
import { StudentService } from "src/app/services/meal-order/student.service";
import { saveAs } from "file-saver";

@Component({
  selector: "wallet-transaction-management",
  templateUrl: "./wallet-transaction-management.component.html",
  styleUrls: ["./wallet-transaction-management.component.css"],
})
export class WalletTransactionManagementComponent implements OnInit {
  private subscription: Subscription = new Subscription();
  filter = new WalletTransactionFilter();
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

  constructor(
    private alertService: AlertService,
    private accountService: AccountService,
    private classService: ClassService,
    private deliveryService: DeliveryService,
    private studentService: StudentService,
  ) {}

  ngOnInit() {
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

  clearFilters() {
    this.start = new Date();
    this.end = new Date();
    this.classLevels = [];
    this.classLevelsGrouped = [];
    this.filter.outletId = [];
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

    this.isLoading = true;
    this.alertService.startLoadingMessage("Downloading...");
    const fileName =
      moment().format("DDMMYYYY_hhmmss") + "_Student-Wallet-Transactions.xlsx";

    this.studentService.generateWalletTransactions(this.filter).subscribe(
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
      Permission.viewWalletTransactionPermission,
    );
  }
}
