import { Component, Inject } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
import { AccountService } from "src/app/services/account.service";
import { AlertService, DialogType, MessageSeverity } from "src/app/services/alert.service";
import { StudentService } from "src/app/services/meal-order/student.service";

@Component({
  selector: "app-student-wallet-transfer",
  templateUrl: "./student-wallet-transfer.component.html",
  styleUrls: ["./student-wallet-transfer.component.css"],
})

export class StudentWalletTransferComponent {
  data = { studentIdFrom: "", studentIdTo: "", amount: 0, userId: "" };
  isSaving: boolean;
  private studentId: string;
  private outletId: string;
  private students: any[];
  private studentSearchCtrl = new FormControl('');

  constructor(
    public dialogRef: MatDialogRef<StudentWalletTransferComponent>,
    private accountService: AccountService,
    private alertService: AlertService,
    private studentService: StudentService,
    @Inject(MAT_DIALOG_DATA) public dataStudent: any
  ) {
    if (dataStudent) {
      if (dataStudent.studentId) {
        this.studentId = dataStudent.studentId;
      }
      if(dataStudent.outletId){
        this.outletId = dataStudent.outletId;
      }

      this.getStudents();
    }
  }

  getStudents(){
    this.studentService.getStudentLiteWithClass(this.outletId).subscribe(
      (results) => {
        this.students = results;
      },
      (error) => {
        this.alertService.showStickyMessage("Get Error",`An error occured while retrieving Students.\r\n"`, MessageSeverity.error);
      }
    );
  }

  save() {
    const studentIdFrom = this.studentId;
    const val = this.data.amount as any;
    const studentIdTo = this.data.studentIdTo;

    const normalized = val;
    const amount = parseFloat(normalized);
    console.log("Parsed amount:", this.data);
    if (isNaN(amount) || amount <= 0) {
        this.alertService.showStickyMessage("Invalid Input", "Please enter a valid positive amount (numbers only, decimals allowed).", MessageSeverity.error);
        return;
    }

    this.alertService.showDialog('Are you sure you want to transfer the amount \"' + amount + '\"?', DialogType.confirm, () => {
        
        this.isSaving = true;
        this.alertService.startLoadingMessage("Processing Wallet Transfer...");
        this.studentService.walletTransfer(studentIdFrom, studentIdTo, amount, this.accountService.currentUser.id)
          .subscribe({
            next: (response) => {
              this.alertService.stopLoadingMessage();
              this.isSaving = false;
              this.alertService.showMessage(response.message);

              if (response.data && response.data.length > 0) {
                const messageData = response.data.join("<br/><br/>");
                this.alertService.showStickyMessage("Wallet Transfer Info", messageData, MessageSeverity.info);
              }
              this.dialogRef.close(this.data);
            },
            error: () => {
              this.alertService.stopLoadingMessage();
              this.isSaving = false;
              this.alertService.showStickyMessage("Wallet Transfer Error", "Unable to transfer amount.", MessageSeverity.error );
            },
          });
    });
  }

  close() {
    this.dialogRef.close();
  }
}
