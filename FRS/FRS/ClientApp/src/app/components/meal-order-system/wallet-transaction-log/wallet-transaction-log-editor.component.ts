import { Component, Inject, ViewChild } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
import { StudentWalletTransaction } from "src/app/models/meal-order/student.model";
import { FileService } from "src/app/services/file.service";

@Component({
  selector: "wallet-transaction-log-editor",
  templateUrl: "./wallet-transaction-log-editor.component.html",
  styleUrls: ["./wallet-transaction-log-editor.component.css"],
})
export class WalletTransactionLogEditorComponent {
  private studentWalletTransaction: StudentWalletTransaction = new StudentWalletTransaction();
   public formResetToggle = true;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild("f")
  private form;

  public fileUploadResponse: { dbPath: ""; fileId: null; fileName: "" };

  constructor(
    public dialogRef: MatDialogRef<WalletTransactionLogEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fileService: FileService
  ) {
    if (typeof data.StudentWalletTransaction != typeof undefined) {
      this.loadData(data.StudentWalletTransaction);
    }
  }

  loadData(studentWalletTransaction: StudentWalletTransaction) {
    if (studentWalletTransaction) {
      Object.assign(this.studentWalletTransaction, studentWalletTransaction);
      return this.studentWalletTransaction;
    }
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.studentWalletTransaction.filePath = this.fileUploadResponse
      ? this.fileUploadResponse.dbPath
      : null;
  };

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  removePhoto() {
    this.studentWalletTransaction.filePath = null;
    this.studentWalletTransaction.fileId = null;
    this.studentWalletTransaction.fileName = null;
  }

  private cancel(){
    if (this.changesCancelledCallback)
      this.changesCancelledCallback();
    
    this.dialogRef.close();
  }
}
