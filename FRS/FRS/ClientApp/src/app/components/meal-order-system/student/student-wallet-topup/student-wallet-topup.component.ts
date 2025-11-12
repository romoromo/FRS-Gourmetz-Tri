import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { WalletTypeList } from 'src/app/helpers/enums';
import { AccountService } from 'src/app/services/account.service';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { StudentService } from 'src/app/services/meal-order/student.service';

@Component({
  selector: 'app-student-wallet-topup',
  templateUrl: './student-wallet-topup.component.html',
  styleUrls: ['./student-wallet-topup.component.css'],
})
export class StudentWalletTopupComponent {

  data = {
    type: '',
    amount: null
  };
  isSaving: boolean;
  walletTypes = WalletTypeList
  private studentId: string;

  constructor(
    public dialogRef: MatDialogRef<StudentWalletTopupComponent>,
    private accountService: AccountService,
    private alertService: AlertService,
    private studentService: StudentService,
    @Inject(MAT_DIALOG_DATA) public dataParent: any) {
    if (dataParent && dataParent.studentId) {
      this.studentId = dataParent.studentId;
    }
  }

  save() {
    const studentId = this.studentId;
    const val = this.data.amount as any;
    const type = this.data.type;

    const normalized = val;
    const amount = parseFloat(normalized);
    console.log('Parsed amount:', this.data);
    if (isNaN(amount) || amount <= 0) {
      this.alertService.showStickyMessage(
        "Invalid Input",
        "Please enter a valid positive amount (numbers only, decimals allowed).",
        MessageSeverity.error
      );
      return;
    }
    this.isSaving = true;
    this.alertService.startLoadingMessage("Processing top-up...");
    this.studentService.walletTopupForStudent(studentId, amount, this.accountService.currentUser.id, type)
      .subscribe({
        next: (response) => {
          this.alertService.stopLoadingMessage();
          this.isSaving = false;
          this.alertService.showMessage(response.message);

          if (response.data && response.data.length > 0) {
            const messageData = response.data.join("<br/><br/>");
            this.alertService.showStickyMessage("Top-up Info", messageData, MessageSeverity.info);
          }
          this.dialogRef.close(this.data);
        },
        error: () => {
          this.alertService.stopLoadingMessage();
          this.isSaving = false;
          this.alertService.showStickyMessage(
            "Wallet Top-up Error",
            "Unable to add amount.",
            MessageSeverity.error
          );
        }
      });
  }

  close() {
    this.dialogRef.close();
  }

}
