import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { DirectoryListing } from 'src/app/models/directory-listing.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DirectoryListingService } from 'src/app/services/directory-listing.service';
import { Subscription } from 'rxjs';
import { DirectoryListingCategoryService } from '../../services/directory-listing-category.service';
import { DirectoryListingCategory } from '../../models/directory-listing-category.model';
import { FloorService } from '../../services/floor.service';
import { Floor } from '../../models/floor.model';


@Component({
  selector: 'directory-listing-editor',
  templateUrl: './directory-listing-editor.component.html',
  styleUrls: ['./directory-listing-editor.component.css'],
  animations: [fadeInOut]
})
export class DirectoryListingEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewDirectoryListing = false;
  private originalDirectoryListing: DirectoryListing = new DirectoryListing();
  private directoryListingEdit: DirectoryListing = new DirectoryListing();
  private allInstitutions: Institution[] = [];
  private validation = { code: false, label: false};
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @ViewChild('code')
  private code;

  @ViewChild('label')
  private label;
  directoryListingCategorys: DirectoryListingCategory[];
  floors: Floor[];

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private directoryListingService: DirectoryListingService, private directoryListingCategoryService: DirectoryListingCategoryService,
    public dialogRef: MatDialogRef<DirectoryListingEditorComponent>, private floorService: FloorService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.directoryListing) != typeof (undefined)) {
      if (data.directoryListing.id) {
        this.editDirectoryListing(data.directoryListing);
      } else {
        this.newDirectoryListing();
      }
    }

    if (!this.directoryListingCategorys) this.directoryListingCategoryService.getDirectoryListingCategoryByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.directoryListingCategorys = results[0];
      },
        error => {
        });

    if (!this.floors) this.floorService.getFloorByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.floors = results[0];
      },
        error => {
        });
  }

  public uploadFinished = (event) => {
    this.directoryListingEdit.iconUrl = event ? event.dbPath : null;

    if (this.directoryListingEdit.iconUrl) this.directoryListingEdit.iconUrl = this.directoryListingEdit.iconUrl.replace(/\\/g, '/');
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


      if (!this.directoryListingEdit.id) {
        this.directoryListingService.newDirectoryListing(this.directoryListingEdit).subscribe(directoryListing => this.saveSuccessHelper(directoryListing), error => this.saveFailedHelper(error));
      }
      else {
        this.directoryListingService.updateDirectoryListing(this.directoryListingEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
  }

  private saveSuccessHelper(directoryListing?: DirectoryListing) {
    if (directoryListing)
      Object.assign(this.directoryListingEdit, directoryListing);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewDirectoryListing) {
      this.alertService.showMessage("Success", `Data \"${this.directoryListingEdit.code}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.directoryListingEdit.code}\" was saved successfully`, MessageSeverity.success);
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
    this.directoryListingEdit = new DirectoryListing();

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

  newDirectoryListing() {
    this.showValidationErrors = true;
    this.isNewDirectoryListing = true;
    this.directoryListingEdit = new DirectoryListing();
    return this.directoryListingEdit;
  }

  editDirectoryListing(directoryListing: DirectoryListing) {
    if (directoryListing) {
      this.isNewDirectoryListing = false;
      this.showValidationErrors = true;
      this.originalDirectoryListing = directoryListing;
      this.directoryListingEdit = new DirectoryListing();
      Object.assign(this.directoryListingEdit, directoryListing);

      return this.directoryListingEdit;
    }
    else {
      return this.newDirectoryListing();
    }
  }

  get canManageDirectoryListings() {
    return true;//this.accountService.userHasPermission(Permission.manageDirectoryListingPermission);
  }

}
