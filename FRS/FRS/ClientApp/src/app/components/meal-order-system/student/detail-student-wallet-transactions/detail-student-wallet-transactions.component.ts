import { DecimalPipe } from "@angular/common";
import { Component, Inject } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
import { StudentWalletTransaction } from "src/app/models/meal-order/student.model";
import { Filter, PagedResult } from "src/app/models/sieve-filter.model";
import { AccountService } from "src/app/services/account.service";
import {
  AlertService,
  DialogType,
  MessageSeverity,
} from "src/app/services/alert.service";
import { AppTranslationService } from "src/app/services/app-translation.service";
import { StudentService } from "src/app/services/meal-order/student.service";

@Component({
  selector: "app-student-wallet-transfer",
  templateUrl: "./detail-student-wallet-transactions.component.html",
  styleUrls: ["./detail-student-wallet-transactions.component.css"],
  providers: [DecimalPipe],
})
export class DetailStudentWalletTransactionComponent {
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = "";
  columns: any[] = [];

  isSaving: boolean;
  items: StudentWalletTransaction;
  studentId: number;

  constructor(
    private translationService: AppTranslationService,
    public dialogRef: MatDialogRef<DetailStudentWalletTransactionComponent>,
    private studentService: StudentService,
    private decimalPipe: DecimalPipe,
    @Inject(MAT_DIALOG_DATA) public dataStudent: any,
  ) {
    if (dataStudent) {
      if (dataStudent.studentId) {
        this.studentId = dataStudent.studentId;
      }
      this.getDetailTransactions();
      this.isSaving = false;
    }
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = "id";
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
      { prop: "transactionDateTime", name: "Date" },
      { prop: "transactionType", name: "Transaction Type" },
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
      },
      { prop: "description", name: "Description" },
    ];
  }

  getDetailTransactions() {
    //this.studentService.getStudentLiteWithClass(this.outletId).subscribe(
    //  (results) => {
    //    this.students = results;
    //  },
    //  (error) => {
    //    this.alertService.showStickyMessage("Get Error",`An error occured while retrieving Students.\r\n"`, MessageSeverity.error);
    //  }
    //);
  }

  save() {
    //const studentIdFrom = this.studentId;
    //const val = this.data.amount as any;
    //const studentIdTo = this.data.studentIdTo;
    //
    //const normalized = val;
    //const amount = parseFloat(normalized);
    //console.log("Parsed amount:", this.data);
    //if (isNaN(amount) || amount <= 0) {
    //    this.alertService.showStickyMessage("Invalid Input", "Please enter a valid positive amount (numbers only, decimals allowed).", MessageSeverity.error);
    //    return;
    //}
    //
    //this.alertService.showDialog('Are you sure you want to transfer the amount \"' + amount + '\"?', DialogType.confirm, () => {
    //
    //    this.isSaving = true;
    //    this.alertService.startLoadingMessage("Processing Wallet Transfer...");
    //    this.studentService.walletTransfer(studentIdFrom, studentIdTo, amount, this.accountService.currentUser.id)
    //      .subscribe({
    //        next: (response) => {
    //          this.alertService.stopLoadingMessage();
    //          this.isSaving = false;
    //          this.alertService.showMessage(response.message);
    //
    //          if (response.data && response.data.length > 0) {
    //            const messageData = response.data.join("<br/><br/>");
    //            this.alertService.showStickyMessage("Wallet Transfer Info", messageData, MessageSeverity.info);
    //          }
    //          this.dialogRef.close(this.data);
    //        },
    //        error: () => {
    //          this.alertService.stopLoadingMessage();
    //          this.isSaving = false;
    //          this.alertService.showStickyMessage("Wallet Transfer Error", "Unable to transfer amount.", MessageSeverity.error );
    //        },
    //      });
    //});
  }

  close() {
    this.dialogRef.close();
  }
}
