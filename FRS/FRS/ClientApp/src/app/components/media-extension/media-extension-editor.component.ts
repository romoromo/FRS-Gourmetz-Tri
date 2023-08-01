import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { MediaExtension } from 'src/app/models/media-extension.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MediaExtensionService } from 'src/app/services/media-extension.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'media-extension-editor',
  templateUrl: './media-extension-editor.component.html',
  styleUrls: ['./media-extension-editor.component.css'],
  animations: [fadeInOut]
})
export class MediaExtensionEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewMediaExtension = false;
  private originalMediaExtension: MediaExtension = new MediaExtension();
  private mediaExtensionEdit: MediaExtension = new MediaExtension();
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

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private mediaExtensionService: MediaExtensionService,
    public dialogRef: MatDialogRef<MediaExtensionEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.mediaExtension) != typeof (undefined)) {
      if (data.mediaExtension.id) {
        this.editMediaExtension(data.mediaExtension);
      } else {
        this.newMediaExtension();
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


      if (!this.mediaExtensionEdit.id) {
        this.mediaExtensionService.newMediaExtension(this.mediaExtensionEdit).subscribe(mediaExtension => this.saveSuccessHelper(mediaExtension), error => this.saveFailedHelper(error));
      }
      else {
        this.mediaExtensionService.updateMediaExtension(this.mediaExtensionEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
      }
  }

  private saveSuccessHelper(mediaExtension?: MediaExtension) {
    if (mediaExtension)
      Object.assign(this.mediaExtensionEdit, mediaExtension);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewMediaExtension) {
      this.alertService.showMessage("Success", `Data \"${this.mediaExtensionEdit.extension}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.mediaExtensionEdit.extension}\" was saved successfully`, MessageSeverity.success);
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
    this.mediaExtensionEdit = new MediaExtension();

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

  newMediaExtension() {
    this.showValidationErrors = true;
    this.isNewMediaExtension = true;
    this.mediaExtensionEdit = new MediaExtension();
    return this.mediaExtensionEdit;
  }

  editMediaExtension(mediaExtension: MediaExtension) {
    if (mediaExtension) {
      this.isNewMediaExtension = false;
      this.showValidationErrors = true;
      this.originalMediaExtension = mediaExtension;
      this.mediaExtensionEdit = new MediaExtension();
      Object.assign(this.mediaExtensionEdit, mediaExtension);

      return this.mediaExtensionEdit;
    }
    else {
      return this.newMediaExtension();
    }
  }

  get canManageMediaExtensions() {
    return true;//this.accountService.userHasPermission(Permission.manageMediaExtensionPermission);
  }

}
