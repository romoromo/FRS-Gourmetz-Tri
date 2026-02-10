import { DecimalPipe } from "@angular/common";
import { Component, Inject, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { FormControl } from "@angular/forms";
import {
  MAT_DIALOG_DATA,
  MatDatepickerInputEvent,
  MatDialogRef,
} from "@angular/material";
import * as moment from "moment";
import { StudentWalletTransaction } from "src/app/models/meal-order/student.model";
import {
  Filter,
  PagedResult,
  TransactionFilter,
} from "src/app/models/sieve-filter.model";
import { DateTimeOnlyPipe } from "src/app/pipes/datetime.pipe";
import { AccountService } from "src/app/services/account.service";
import {
  AlertService,
  DialogType,
  MessageSeverity,
} from "src/app/services/alert.service";
import { AppTranslationService } from "src/app/services/app-translation.service";
import { StudentService } from "src/app/services/meal-order/student.service";
import { Utilities } from "src/app/services/utilities";
import { saveAs } from "file-saver";
@Component({
  selector: "app-student-wallet-transfer",
  templateUrl: "./detail-student-wallet-transactions.component.html",
  styleUrls: ["./detail-student-wallet-transactions.component.css"],
  providers: [DecimalPipe],
})
export class DetailStudentWalletTransactionComponent
  implements OnInit, OnDestroy
{
  filter: TransactionFilter;
  pagedResult: PagedResult;
  keyword: string = "";
  columns: any[] = [];
  tstart = new Date();
  tend = new Date();
  start?: Date = null;
  end?: Date = null;
  rows: StudentWalletTransaction[] = [];
  rowsCache: StudentWalletTransaction[] = [];
  loadingIndicator: boolean;
  items: StudentWalletTransaction;
  studentId: number;
  @ViewChild("transactionTable") table: any;
  isClearResults = true;

  constructor(
    private translationService: AppTranslationService,
    public dialogRef: MatDialogRef<DetailStudentWalletTransactionComponent>,
    private studentService: StudentService,
    private decimalPipe: DecimalPipe,
    private alertService: AlertService,
    @Inject(MAT_DIALOG_DATA) public dataStudent: any,
  ) {
    if (dataStudent) {
      if (dataStudent.studentId) {
        this.studentId = dataStudent.studentId;
      }
      this.loadingIndicator = false;
    }
  }

  initializeFilter() {
    this.filter = new TransactionFilter(1, 10);
    this.filter.sorts = "createdDate";
    this.filter.filters = "";
    this.filter.page = 1;
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
      {
        prop: "transactionDateTime",
        name: "Date",
        pipe: new DateTimeOnlyPipe("en-SG"),
        sortable: false,
      },
      {
        prop: "amount",
        name: "Amount",
        pipe: {
          transform: (val: number) => this.decimalPipe.transform(val, "1.2-2"),
        },
        sortable: false,
      },
      { prop: "transactionType", name: "Type", sortable: false },
      { prop: "description", name: "Description", sortable: false },
      { prop: "stripeId", name: "REF_ID", sortable: false },
      { prop: "remarks", name: "Remarks", sortable: false },
      { prop: "userName", name: "Processed By", sortable: false },
    ];
  }

  clearFilterAndPagedResult() {
    this.initializeFilter();
    this.initializePagedResult();
    this.table.offset = 0;
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.loadData();
  }

  ngOnDestroy(): void {}

  loadData(ev?: any) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    this.filter.pageSize = 10;

    if (ev) {
      this.filter.page = ev.offset + 1;
      if (Number.isNaN(this.filter.page)) this.filter.page = 1;

      if (ev.sorts) {
        this.filter.sorts =
          ev.sorts[0].dir == "desc" ? "-" + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    } else {
      if (!this.filter.page) this.filter.page = 1;
    }
    if (Number.isNaN(this.filter.page)) this.filter.page = 1;

    console.log("filter: ", this.filter);

    if (!this.studentId) this.studentId = 0;
    this.filter.filters = "(studentId)==" + this.studentId;

    if (this.filter.transactionType) {
      this.filter.filters =
        this.filter.filters +
        ",(TransactionType)==" +
        this.filter.transactionType;
    }

    if (this.start && this.end) {
      this.filter.filters =
        this.filter.filters +
        ",(WalletTransactionDateRange)==" +
        this.start.toDateString() +
        "|" +
        this.end.toDateString();
    }

    this.studentService
      .getStudentWalletTransactionsSimpleByFilter(this.filter)
      .subscribe(
        (results) => {
          this.pagedResult = results;
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          let walletTrans = results.pagedData;

          walletTrans.forEach((walletTran, index, walletTrans) => {
            (<any>walletTran).index = index + 1;
          });

          this.rowsCache = [...walletTrans];
          this.rows = walletTrans;
        },
        (error) => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage(
            "Load Error",
            `Unable to retrieve order cancellations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(
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
      if (!this.end) this.end = new Date(event.value);
    }

    if (type == "end") {
      this.end = new Date(event.value);
      if (!this.start) this.start = new Date(event.value);
    }

    if (type == "tstart") {
      this.tstart = new Date(event.value);
    }

    if (type == "tend") {
      this.tend = new Date(event.value);
    }
  }

  onSearchChanged(value: string) {
    this.keyword = value;
    console.log("keyword on change: ", this.keyword, value);
  }

  onSearch() {
    this.isClearResults = true;
    this.isClearResults = true;
    this.filter.page = 1;
    if (this.table) {
      this.table.offset = 0;
    }

    this.loadData(null);
  }

  clearFilters() {
    this.start = null;
    this.end = null;
    this.keyword = "";

    this.isClearResults = false;
    this.clearFilterAndPagedResult();
  }

  save() {
    this.loadingIndicator = true;
    const fileName =
      moment().format("DDMMYYYY_hhmmss") + "_WalletTransactionsByStudent.xlsx";
    console.log(this.filter);
    this.studentService.downloadWalletTransactionsByStudent(this.filter).subscribe(
      (data) => {
        console.log(data);
        saveAs(data, fileName);
        this.loadingIndicator = false;
      },
      (err) => {
        alert("Problem while downloading the file.");
        console.error(err);
        this.loadingIndicator = false;
      },
    );
  }

  close() {
    this.dialogRef.close();
  }
}
