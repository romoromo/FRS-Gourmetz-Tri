import { Component, ViewChild, Inject, OnDestroy } from '@angular/core';

import { AlertService, MessageSeverity, DialogType } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { UserPhonebook } from '../../../models/userphonebook.model';
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material';
import { Department } from 'src/app/models/department.model';
import { DepartmentService } from 'src/app/services/department.service';
import { FormControl, Validators } from '@angular/forms';
import { UserSelectorComponent } from '../../common/user-selector/user-selector.component';
import { FileService } from 'src/app/services/file.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'userphonebook-editor',
  templateUrl: './userphonebook-editor.component.html',
  styleUrls: ['./userphonebook-editor.component.css']
})
export class UserPhonebookEditorComponent implements OnDestroy {
  private subscription: Subscription = new Subscription();
  private isNewUserPhonebook = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingUserPhonebookName: string;
  private userPhonebookEdit: UserPhonebook = new UserPhonebook();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  departments = new FormControl('', [Validators.required]);
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  private departmentList: Department[] = [];

  @ViewChild('f')
  private form;

  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };

  constructor(private alertService: AlertService, private departmentService: DepartmentService, private accountService: AccountService,
    private fileService: FileService, public dialogRef: MatDialogRef<UserPhonebookEditorComponent>, public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.userPhonebook) != typeof (undefined)) {
      if (data.userPhonebook.id) {
        this.editUserPhonebook(data.userPhonebook);
      } else {
        this.newUserPhonebook();
      }
    }
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }


  openNewEmailContact(): void {
    //var dialog = this.dialog.open(CGNewContactEmailEditorComponent, {
    //  data: { header: "New contact", userPhonebookMember: new UserPhonebook() },
    //  width: '400px'
    //});

    //dialog.afterClosed().subscribe(result => {
    //  if (result.email && result.name) {
    //    if (typeof (this.userPhonebookEdit) == typeof (undefined)) {
    //      this.userPhonebookEdit = new UserPhonebook();
    //    }

    //    //if (typeof (this.userPhonebookEdit.members) == typeof (undefined)) {
    //    //  this.userPhonebookEdit.members = [];
    //    //}

    //    //this.userPhonebookEdit.members.push(result);
    //  }
    //});
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.userPhonebookEdit.userId = this.accountService.currentUser.id;
    if (this.isNewUserPhonebook) {
      this.subscription.add(this.accountService.newUserPhonebook(this.userPhonebookEdit).subscribe(userPhonebook => this.saveSuccessHelper(userPhonebook), error => this.saveFailedHelper(error)));
    }
    else {
      this.subscription.add(this.accountService.updateUserPhonebook(this.userPhonebookEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error)));
    }
  }




  private saveSuccessHelper(userPhonebook?: UserPhonebook) {
    if (userPhonebook)
      Object.assign(this.userPhonebookEdit, userPhonebook);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewUserPhonebook)
      this.alertService.showMessage("Success", `Contact Group \"${this.userPhonebookEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to contact group \"${this.userPhonebookEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.userPhonebookEdit = new UserPhonebook();
    this.resetForm();


    //if (!this.isNewUserPhonebook && this.accountService.currentUser.facilities.some(r => r == this.editingUserPhonebookName))
    //    this.refreshLoggedInUser();

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


  private cancel() {
    this.userPhonebookEdit = new UserPhonebook();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  loadDepartments() {
    this.subscription.add(this.departmentService.getDepartments(-1, -1, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.departmentList = results;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getDepartments failed", "An error occured while trying to get equipments from the server", MessageSeverity.error);
        }));
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


  newUserPhonebook() {
    this.isNewUserPhonebook = true;
    this.showValidationErrors = true;

    this.editingUserPhonebookName = null;
    this.selectedValues = {};
    this.userPhonebookEdit = new UserPhonebook();

    return this.userPhonebookEdit;
  }

  editUserPhonebook(userPhonebook: UserPhonebook) {
    if (userPhonebook) {
      this.isNewUserPhonebook = false;
      this.showValidationErrors = true;

      this.editingUserPhonebookName = userPhonebook.name;
      this.selectedValues = {};
      this.userPhonebookEdit = new UserPhonebook();
      Object.assign(this.userPhonebookEdit, userPhonebook);

      return this.userPhonebookEdit;
    }
    else {
      return this.newUserPhonebook();
    }
  }

  //deleteMember(row: UserPhonebook) {
  //  this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" ?', DialogType.confirm, () => this.deleteMemberHelper(row));
  //}

  //deleteMemberHelper(row: UserPhonebookMember) {

  //  this.userPhonebookEdit.members = this.userPhonebookEdit.members.filter(item => item !== row);
  //}
  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.userPhonebookEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageUserPhonebooks() {
    return this.accountService.userHasPermission(Permission.manageUserPhonebooksPermission)
  }
}
