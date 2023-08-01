import { Component, ViewChild, Inject, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { SignagePublicationService } from "../../../services/signage-publication.service";
import { SignagePublication } from '../../../models/signage-publication.model';
import { Permission } from '../../../models/permission.model';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { SignageComponentService } from '../../../services/signage-component.service';
import { SignageComponent } from '../../../models/signage-component.model';
import { SignageSchedule } from '../../../models/signage-schedule.model';
import { SignageCompilationService } from '../../../services/signage-compilation.service';
import { SignageCompilation } from '../../../models/signage-compilation.model';
import { FormControl } from '@angular/forms';


@Component({
  selector: 'signage-publication-editor',
  templateUrl: './signage-publication-editor.component.html',
  styleUrls: ['./signage-publication-editor.component.css', './signage-publication-editor.component.scss']
})
export class SignagePublicationEditorComponent {

  private isNewSignagePublication = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingSignagePublicationName: string;
  private signagePublicationEdit: SignagePublication = new SignagePublication();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  public searchForm: FormControl = new FormControl();

  @ViewChild('f')
  private form;

  @ViewChild('publicationContainer') elementView;

  scale: number;

    signageCompilations: SignageCompilation[];

  constructor(private alertService: AlertService, private signagePublicationService: SignagePublicationService, private accountService: AccountService, private cdRef: ChangeDetectorRef, signageCompilationService: SignageCompilationService,
    //public dialogRef: MatDialogRef<SignagePublicationEditorComponent>,
    ) {

    signageCompilationService.getSignageCompilations()
      .subscribe(results => {
        this.signageCompilations = results[0];
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve signage components from the server.\r\nErrors:`,
            MessageSeverity.error);
        });
  }

  approvedClicked() {
    if (this.signagePublicationEdit.approved) this.signagePublicationEdit.rejected = false;

  }

  rejectedClicked() {
    if (this.signagePublicationEdit.rejected) this.signagePublicationEdit.approved = false;
  }

  add() {
    if (!this.signagePublicationEdit.schedules) this.signagePublicationEdit.schedules = [];

    let s = new SignageSchedule();
    s.isActive = 1;

    this.signagePublicationEdit.schedules.push(s);
  }

  addCompilation(compilation, schedule: SignageSchedule) {
    if (!compilation) return;
    if (!schedule.compilations) schedule.compilations = [];

    let c = new SignageCompilation();
    Object.assign(c, compilation);
    c.isActive = 1;
    c.compilationId = c.id;
    c.id = "0";
    schedule.compilations.push(c);
  }

  ngAfterViewInit() {

    this.cdRef.detectChanges();
  }

  getStr(obj: any) {
    return JSON.stringify(obj);
  }

  xChanged(event, c, w) {
    if (c.x > w) c.x = w - c.width;
    if (c.x + c.width > w) c.width = w - c.x;
  }

  yChanged(event, c, h) {
    if (c.y > h) c.y = h - c.height;
    if (c.y + c.height > h) c.height = h - c.y;
  }

  wChanged(event, c, w) {
    if (c.width > w) c.width = w - c.x;
    if (c.x + c.width > w) c.x = w - c.width;
  }

  hChanged(event, c, h) {
    if (c.height > h) c.height = h - c.y;
    if (c.y + c.height > h) c.y = h - c.height;
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    if (this.signagePublicationEdit.rejected && !this.signagePublicationEdit.remark) {
      console.log("REMark", "aa");
      this.alertService.showStickyMessage("Save Error", "If rejected, please provide remark.", MessageSeverity.error);
      return;
    }

    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewSignagePublication) {
      this.signagePublicationService.newSignagePublication(this.signagePublicationEdit).subscribe(signagePublication => this.saveSuccessHelper(signagePublication), error => this.saveFailedHelper(error));
    }
    else {
      this.signagePublicationService.updateSignagePublication(this.signagePublicationEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(signagePublication?: SignagePublication) {
    if (signagePublication)
      Object.assign(this.signagePublicationEdit, signagePublication);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewSignagePublication)
      this.alertService.showMessage("Success", `Signage Publication \"${this.signagePublicationEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to signage publication \"${this.signagePublicationEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.signagePublicationEdit = new SignagePublication();
    this.resetForm();


    if (this.changesSavedCallback)
      this.changesSavedCallback();

    //this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();

    //this.dialogRef.close();
  }


  private cancel() {
    this.signagePublicationEdit = new SignagePublication();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    //this.dialogRef.close();
  }

  private toggleGroup(groupName: string) {
    let firstMemberValue: boolean;

    this.allPermissions.forEach(p => {
      if (p.groupName != groupName)
        return;

      if (firstMemberValue == null)
        firstMemberValue = this.selectedValues[p.value] == true;

      this.selectedValues[p.value] = !firstMemberValue;
    });
  }


  private getSelectedPermissions() {
    return this.allPermissions.filter(p => this.selectedValues[p.value] == true);
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


  newSignagePublication() {
    this.isNewSignagePublication = true;
    this.showValidationErrors = true;

    this.editingSignagePublicationName = null;
    this.selectedValues = {};
    this.signagePublicationEdit = new SignagePublication();
    return this.signagePublicationEdit;
  }

  editSignagePublication(signagePublication: SignagePublication) {
    if (signagePublication) {
      this.isNewSignagePublication = false;
      this.showValidationErrors = true;

      this.editingSignagePublicationName = signagePublication.name;
      this.selectedValues = {};
      this.signagePublicationEdit = new SignagePublication();
      Object.assign(this.signagePublicationEdit, signagePublication);

      return this.signagePublicationEdit;
    }
    else {
      return this.newSignagePublication();
    }
  }



  get canManageSignagePublications() {
    return true;//this.accountService.userHasPermission(Permission.manageSignagePublicationsPermission)
  }

  get canApproveSignagePublications() {
    return this.accountService.userHasPermission(Permission.approvePublicationsPermission);
  }
}
