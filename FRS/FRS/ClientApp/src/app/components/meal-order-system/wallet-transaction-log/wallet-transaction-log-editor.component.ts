import { Component, Inject, ViewChild } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
import { StudentWalletTransaction } from "src/app/models/meal-order/student.model";
import { AlertService, MessageSeverity } from "src/app/services/alert.service";
import { FileService } from "src/app/services/file.service";
import { StudentService } from "src/app/services/meal-order/student.service";

@Component({
  selector: "wallet-transaction-log-editor",
  templateUrl: "./wallet-transaction-log-editor.component.html",
  styleUrls: ["./wallet-transaction-log-editor.component.css"],
})
export class WalletTransactionLogEditorComponent {
  private studentWalletTransaction: StudentWalletTransaction =
    new StudentWalletTransaction();
  public formResetToggle = true;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  private isSaving: boolean;

  @ViewChild("f")
  private form;

  public fileUploadResponse: { dbPath: ""; fileId: null; fileName: "" };

  constructor(
    public dialogRef: MatDialogRef<WalletTransactionLogEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fileService: FileService,
    private alertService: AlertService,
    private studentService: StudentService,
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

  isImageFile(path: string | null | undefined): boolean {
    if (!path) return false;
    const p = path.toLowerCase();
    return p.endsWith(".jpg") || p.endsWith(".jpeg") || p.endsWith(".png");
  }

  get uploadFileName(): string {
    if (
      this.studentWalletTransaction &&
      this.studentWalletTransaction.id !== null &&
      this.studentWalletTransaction.id !== undefined
    ) {
      this.studentWalletTransaction.fileId = this.studentWalletTransaction.id.toString();
      return this.studentWalletTransaction.id.toString();
    }

    return "";
  }

  private cancel() {
    if (this.changesCancelledCallback) this.changesCancelledCallback();

    this.dialogRef.close();
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    this.studentService.updateWalletTransactionLog(this.studentWalletTransaction).subscribe(success => this.saveSuccessHelper(), error => this.saveFailedHelper(error))
  }

  private saveSuccessHelper() {
      this.isSaving = false;
      this.alertService.stopLoadingMessage();
  
        this.alertService.showMessage("Success", `Wallet Transaction Log was saved successfully`, MessageSeverity.success);
  
      if (this.changesSavedCallback)
        this.changesSavedCallback();
  
      this.dialogRef.close();
    }
  
  
    private saveFailedHelper(error: any) {
      this.isSaving = false;
      this.alertService.stopLoadingMessage();
      this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
      this.alertService.showStickyMessage(error, null, MessageSeverity.error);
  
      if (this.changesFailedCallback)
        this.changesFailedCallback();
    }
}
