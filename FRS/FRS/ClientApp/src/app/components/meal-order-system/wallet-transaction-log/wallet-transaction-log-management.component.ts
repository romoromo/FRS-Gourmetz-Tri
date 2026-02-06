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
  TransactionFilter,
  PagedResult,
} from "../../../models/sieve-filter.model";
import { Permission } from "../../../models/permission.model";
import { MatDatepickerInputEvent, MatDialog } from "@angular/material";
import { AuditService } from "src/app/services/audit.service";
import { DateTimeOnlyPipe } from "src/app/pipes/datetime.pipe";
import { Subscription } from "rxjs";
import { SearchBoxComponent } from "../../controls/search-box.component";
import { saveAs } from "file-saver";
import * as moment from "moment";
import {
  Student,
  StudentWalletTransaction,
} from "src/app/models/meal-order/student.model";
//import { StudentEditorComponent } from './student-editor.component';
import { StudentService } from "src/app/services/meal-order/student.service";
import { WalletTransactionLogEditorComponent } from "./wallet-transaction-log-editor.component";
import { DecimalPipe } from '@angular/common';

@Component({
  selector: "wallet-transaction-log-management",
  providers: [DecimalPipe],
  templateUrl: "./wallet-transaction-log-management.component.html",
  styleUrls: ["./wallet-transaction-log-management.component.css"],
})
export class WalletTransactionLogManagementComponent
  implements OnInit, OnDestroy
{
  private subscription: Subscription = new Subscription();
  transactionType = ["Credit", "Debit"];
  columns: any[] = [];
  rows: StudentWalletTransaction[] = [];
  rowsCache: StudentWalletTransaction[] = [];
  editingAuthLogName: { name: string };
  loadingIndicator: boolean;
  filter: TransactionFilter;
  pagedResult: PagedResult;
  keyword: string = "";
  status: string = "paid";
  start?: Date = null;
  end?: Date = null;
  selectedRow: StudentWalletTransaction;

  @Input() isHideHeader: boolean;

  @Input() outletId: string;

  private selected: any[] = [];
  private allRowsSelected = false;
  tstart = new Date();
  tend = new Date();

  isClearResults = true;

  @ViewChild("searchbox") searchbox: SearchBoxComponent;

  @ViewChild("indexTemplate")
  indexTemplate: TemplateRef<any>;

  @ViewChild("hdrTpl") hdrTpl: TemplateRef<any>;

  @ViewChild("actionTemplate")
  actionsTemplate: TemplateRef<any>;

  @ViewChild("editorModal")
  editorModal: ModalDirective;

  @ViewChild("mealDescription")
  mealDescriptionTemplate: TemplateRef<any>;

  @ViewChild("orderTable") table: any;

  constructor(
    private alertService: AlertService,
    private translationService: AppTranslationService,
    private accountService: AccountService,
    private studentService: StudentService,
    public dialog: MatDialog,
    private decimalPipe: DecimalPipe
  ) {}

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  initializeFilter() {
    this.filter = new TransactionFilter(1, 10);
    this.filter.sorts = "-id";
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
        width: 50,
        cellTemplate: this.indexTemplate,
        canAutoResize: false,
        headerCheckboxable: true,
        headerTemplate: this.hdrTpl,
      },
      {
        prop: "transactionDateTime",
        name: "Date",
        pipe: new DateTimeOnlyPipe("en-SG"),
      },
      {
        prop: "amount", name: "Amount",
        pipe: {
          transform: (val: number) => this.decimalPipe.transform(val, '1.2-2')
        }
      },
      //{ prop: 'mealDescription', name: 'Meal Description', cellTemplate: this.mealDescriptionTemplate, sortable: false, draggable: false },
      { prop: "transactionType", name: "Type" },
      { prop: "description", name: "Description" },
      { prop: "stripeId", name: "Stripe Id" },
      { prop: "studentId", name: "Student Id" },
      { prop: "studentName", name: "Student Name" },
      { prop: "remarks", name: "Remarks" },
      { prop: "userName", name: "Processed By" },
      {
        name: "",
        width: 150,
        cellTemplate: this.actionsTemplate,
        resizeable: false,
        canAutoResize: false,
        sortable: false,
        draggable: false,
      },
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
    //this.loadData();
  }

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
    }

    console.log("keyword: ", this.filter.keyword);

    console.log("filter: ", this.filter);

    if (!this.filter.keyword) this.filter.keyword = "";
    this.filter.filters = "(studentName)@=" + this.filter.keyword;

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
      .getStudentWalletTransactionsByFilter(this.filter)
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
              error
            )}"`,
            MessageSeverity.error
          );
        }
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

    //this.loadData();

    //this.loadData();
  }

  onSearchChanged(value: string) {
    //this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    this.keyword = value;
    console.log("keyword on change: ", this.keyword, value);
    //this.loadData(null);
  }

  onSearch() {
    this.isClearResults = true;
    this.clearFilterAndPagedResult();
    this.loadData(null);
  }

  clearFilters() {
    this.start = null;
    this.end = null;
    this.keyword = "";

    this.selected = [];
    this.isClearResults = false;
    this.allRowsSelected = false;
    this.clearFilterAndPagedResult();
    //this.searchbox.clear();
  }

  onCheckboxChangeFn(ev) {
    console.log(ev);
  }
  onSelect({ selected }) {
    console.log(selected);
    this.selected = selected;
  }

  downloadResults() {
    const fileName =
      moment().format("DDMMYYYY_hhmmss") + "_WalletTransactions.xlsx";
    console.log(this.filter);
    this.studentService.downloadWalletTransactions(this.filter).subscribe(
      (data) => {
        console.log(data);
        saveAs(data, fileName);
      },
      (err) => {
        alert("Problem while downloading the file.");
        console.error(err);
      }
    );
  }

  get canManageWalletTransaction() {
    return this.accountService.userHasPermission(
      Permission.manageMOSOrderMgtWalletTransactionsPermission
    );
  }

  openDialog(StudentWalletTransaction: StudentWalletTransaction): void {
    const dialogRef = this.dialog.open(WalletTransactionLogEditorComponent, {
      data: { StudentWalletTransaction: StudentWalletTransaction },
      width: "400px"
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.loadData(null);
    });
  }

  uploadTransaction(row: StudentWalletTransaction) {
    this.selectedRow = row;
    this.openDialog(this.selectedRow);
  }
}
