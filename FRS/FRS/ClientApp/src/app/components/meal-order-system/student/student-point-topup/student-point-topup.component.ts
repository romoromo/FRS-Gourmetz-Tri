import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { PointTypeList } from 'src/app/helpers/enums';
import { AccountService } from 'src/app/services/account.service';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { StudentService } from 'src/app/services/meal-order/student.service';

@Component({
  selector: 'app-student-point-topup',
  templateUrl: './student-point-topup.component.html',
  styleUrls: ['./student-point-topup.component.css'],
})
export class StudentPointTopupComponent {

  data = {
    type: '',
    amount: null
  };
  isSaving: boolean;
  pointTypes = PointTypeList
  private studentId: string;
  private isStudentGroup: boolean;

  constructor(
    public dialogRef: MatDialogRef<StudentPointTopupComponent>,
    private accountService: AccountService,
    private alertService: AlertService,
    private studentService: StudentService,
    @Inject(MAT_DIALOG_DATA) public dataParent: any) {
    if (dataParent) {
      if (dataParent.studentId) {
        this.studentId = dataParent.studentId;
      }

      if (dataParent.isStudentGroup) {
        this.isStudentGroup = dataParent.isStudentGroup;
      }
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
    if (this.isStudentGroup) {
      this.studentService.pointTopupForStudentGroup(studentId, amount, this.accountService.currentUser.id, type)
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
              "Point Top-up Error",
              "Unable to add amount.",
              MessageSeverity.error
            );
          }
        });
    } else {
      this.studentService.pointTopupForStudent(studentId, amount, this.accountService.currentUser.id, type)
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
              "Point Top-up Error",
              "Unable to add amount.",
              MessageSeverity.error
            );
          }
        });
    }
  }

  close() {
    this.dialogRef.close();
  }

}
