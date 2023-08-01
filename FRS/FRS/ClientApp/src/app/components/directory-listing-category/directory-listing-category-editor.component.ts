import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { DirectoryListingCategory } from 'src/app/models/directory-listing-category.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DirectoryListingCategoryService } from 'src/app/services/directory-listing-category.service';
import { Subscription } from 'rxjs';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'directory-listing-category-editor',
  templateUrl: './directory-listing-category-editor.component.html',
  styleUrls: ['./directory-listing-category-editor.component.css'],
  animations: [fadeInOut]
})
export class DirectoryListingCategoryEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewDirectoryListingCategory = false;
  private originalDirectoryListingCategory: DirectoryListingCategory = new DirectoryListingCategory();
  private directoryListingCategoryEdit: DirectoryListingCategory = new DirectoryListingCategory();
  private allInstitutions: Institution[] = [];
  private validation = { code: false, label: false};
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };

  @ViewChild('f')
  private form;

  @ViewChild('code')
  private code;

  @ViewChild('label')
  private label;

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private directoryListingCategoryService: DirectoryListingCategoryService, private fileService: FileService,
    public dialogRef: MatDialogRef<DirectoryListingCategoryEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.directoryListingCategory) != typeof (undefined)) {
      if (data.directoryListingCategory.id) {
        this.editDirectoryListingCategory(data.directoryListingCategory);
      } else {
        this.newDirectoryListingCategory();
      }
    }
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  private save() {
      this.isSaving = true;
      this.alertService.startLoadingMessage("Saving changes...");


      if (!this.directoryListingCategoryEdit.id) {
        this.directoryListingCategoryService.newDirectoryListingCategory(this.directoryListingCategoryEdit).subscribe(directoryListingCategory => this.saveSuccessHelper(directoryListingCategory), error => this.saveFailedHelper(error));
      }
      else {
        this.directoryListingCategoryService.updateDirectoryListingCategory(this.directoryListingCategoryEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
  }

  private saveSuccessHelper(directoryListingCategory?: DirectoryListingCategory) {
    if (directoryListingCategory)
      Object.assign(this.directoryListingCategoryEdit, directoryListingCategory);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDirectoryListingCategory) {
      this.alertService.showMessage("Success", `Data \"${this.directoryListingCategoryEdit.code}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.directoryListingCategoryEdit.code}\" was saved successfully`, MessageSeverity.success);
    }

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    //if (this.changesFailedCallback)
    //  this.changesFailedCallback();

    //this.dialogRef.close();
  }


  private cancel() {
    this.directoryListingCategoryEdit = new DirectoryListingCategory();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  loadInstitutions() {
    this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.allInstitutions = results[0];

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve institutions from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  newDirectoryListingCategory() {
    this.showValidationErrors = true;
    this.isNewDirectoryListingCategory = true;
    this.directoryListingCategoryEdit = new DirectoryListingCategory();
    return this.directoryListingCategoryEdit;
  }

  editDirectoryListingCategory(directoryListingCategory: DirectoryListingCategory) {
    if (directoryListingCategory) {
      this.isNewDirectoryListingCategory = false;
      this.showValidationErrors = true;
      this.originalDirectoryListingCategory = directoryListingCategory;
      this.directoryListingCategoryEdit = new DirectoryListingCategory();
      Object.assign(this.directoryListingCategoryEdit, directoryListingCategory);

      return this.directoryListingCategoryEdit;
    }
    else {
      return this.newDirectoryListingCategory();
    }
  }

  get canManageDirectoryListingCategorys() {
    return true;//this.accountService.userHasPermission(Permission.manageDirectoryListingCategoryPermission);
  }

  public uploadFinished = (event) => {
    console.log("finish uploading... ", event)
    this.fileUploadResponse = event;
    this.directoryListingCategoryEdit.icon = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

}
