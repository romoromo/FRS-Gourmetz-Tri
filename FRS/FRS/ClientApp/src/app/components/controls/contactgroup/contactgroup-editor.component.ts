import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity, DialogType } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { ContactGroupService } from "../../../services/contactgroup.service";
import { ContactGroup } from '../../../models/contactgroup.model';
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material';
import { ContactGroupMember } from 'src/app/models/contactgroupmember.model';
import { CGNewContactEmailEditorComponent } from './emailcontact/emailcontact-editor.component';
import { Department } from 'src/app/models/department.model';
import { DepartmentService } from 'src/app/services/department.service';
import { FormControl, Validators } from '@angular/forms';
import { UserSelectorComponent } from '../../common/user-selector/user-selector.component';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'contactgroup-editor',
  templateUrl: './contactgroup-editor.component.html',
  styleUrls: ['./contactgroup-editor.component.css']
})
export class ContactGroupEditorComponent {

  private isNewContactGroup = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingContactGroupName: string;
  private contactGroupEdit: ContactGroup = new ContactGroup();
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



  constructor(private alertService: AlertService, private departmentService: DepartmentService, private contactGroupService: ContactGroupService, private accountService: AccountService,
    private fileService: FileService, public dialogRef: MatDialogRef<ContactGroupEditorComponent>, public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.contactGroup) != typeof (undefined)) {
      if (data.contactGroup.id) {
        this.editContactGroup(data.contactGroup);
      } else {
        this.newContactGroup();
      }
    }
    this.loadDepartments();
  }

  openContactDialog(header: string, cg: ContactGroupMember): void {
    const dialogRef = this.dialog.open(CGNewContactEmailEditorComponent, {
      data: { header: header, contactGroupMember: cg },
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.cgm && result.cgm.email && result.cgm.name) {
        if (typeof (this.contactGroupEdit) == typeof (undefined)) {
          this.contactGroupEdit = new ContactGroup();
        }

        if (typeof (this.contactGroupEdit.members) == typeof (undefined)) {
          this.contactGroupEdit.members = [];
        }

        let index = this.contactGroupEdit.members.indexOf(result.cgm);
        if (result.cgm.id > 0 || (result.orig && (result.orig.email || result.orig.name))) {
          this.contactGroupEdit.members.forEach((cgm, index, cgs) => {
            if ((result.cgm.id > 0 && cgm.id == result.cgm.id) || (cgm.name == result.orig.name && cgm.email == result.orig.email)) {
              cgm.company = result.cgm.company;
              cgm.department = result.cgm.department;
              cgm.designation = result.cgm.designation;
              cgm.email = result.cgm.email;
              cgm.filePath = result.cgm.filePath;
              cgm.homeNo = result.cgm.homeNo;
              cgm.mobileNo = result.cgm.mobileNo;
              cgm.name = result.cgm.name;
              cgm.phoneNumber = result.cgm.phoneNumber;
            }
          });
        }
        else {
          if (index < 0) {
            this.contactGroupEdit.members.push(result.cgm);
          }
        }
      }
    })
  }

  openNewEmailContact(header: string, cg?: ContactGroupMember): void {
    if (!cg) {
      cg = new ContactGroupMember();
    }

    var dialog = this.dialog.open(CGNewContactEmailEditorComponent, {
      data: { header: header, contactGroupMember: cg },
      width: '400px'
    });

    dialog.afterClosed().subscribe(result => {
      if (result.email && result.name) {
        if (typeof (this.contactGroupEdit) == typeof (undefined)) {
          this.contactGroupEdit = new ContactGroup();
        }

        if (typeof (this.contactGroupEdit.members) == typeof (undefined)) {
          this.contactGroupEdit.members = [];
        }

        this.contactGroupEdit.members.push(result);
      }
    });
  }



  openFromUserList() {
    const dialogRef = this.dialog.open(UserSelectorComponent, {
      data: { header: "Select users", users: [] },
      width: '950px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result.isCancel && result.selectedUsers) {
        if (typeof (this.contactGroupEdit) == typeof (undefined)) {
          this.contactGroupEdit = new ContactGroup();
        }

        if (typeof (this.contactGroupEdit.members) == typeof (undefined)) {
          this.contactGroupEdit.members = [];
        }

        result.selectedUsers.forEach((user, index, locations) => {
          var exist = this.contactGroupEdit.members.filter((cg) => cg.email == user.email);

          if (exist && exist.length == 0) {
            var member = new ContactGroupMember();
            member.email = user.email;
            member.name = user.fullName;
            member.department = user.departmentName;
            member.company = user.company;
            member.designation = user.jobTitle;
            member.phoneNumber = user.phoneNumber;
            member.homeNo = user.homeNo;
            member.mobileNo = user.mobileNo;
            member.userId = user.id;
            this.contactGroupEdit.members.push(member);
          }
        });
      }
    });
  }

  openFromAddressBook() {
    //const dialogRef = this.dialog.open(UserSelectorComponent, {
    //  data: { header: "Select users", users: [] },
    //  width: '750px'
    //});

    //dialogRef.afterClosed().subscribe(result => {
    //  if (!result.isCancel && result.selectedUsers) {
    //    if (typeof (this.contactGroupEdit) == typeof (undefined)) {
    //      this.contactGroupEdit = new ContactGroup();
    //    }

    //    if (typeof (this.contactGroupEdit.members) == typeof (undefined)) {
    //      this.contactGroupEdit.members = [];
    //    }

    //    result.selectedUsers.forEach((user, index, locations) => {
    //      var exist = this.contactGroupEdit.members.filter((cg) => cg.email == user.email);

    //      if (exist && exist.length == 0) {
    //        var member = new ContactGroupMember();
    //        member.email = user.email;
    //        member.name = user.fullName;
    //        member.department = user.departmentName;
    //        member.company = user.company;
    //        member.designation = user.jobTitle;
    //        member.phoneNumber = user.phoneNumber;
    //        this.contactGroupEdit.members.push(member);
    //      }
    //    });
    //  }
    //});
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    if (this.isNewContactGroup) {
      this.contactGroupService.newContactGroup(this.contactGroupEdit).subscribe(contactGroup => this.saveSuccessHelper(contactGroup), error => this.saveFailedHelper(error));
    }
    else {
      this.contactGroupService.updateContactGroup(this.contactGroupEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(contactGroup?: ContactGroup) {
    if (contactGroup)
      Object.assign(this.contactGroupEdit, contactGroup);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewContactGroup)
      this.alertService.showMessage("Success", `Contact Group \"${this.contactGroupEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to contact group \"${this.contactGroupEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.contactGroupEdit = new ContactGroup();
    this.resetForm();


    //if (!this.isNewContactGroup && this.accountService.currentUser.facilities.some(r => r == this.editingContactGroupName))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close({ isSuccess: true });
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
    this.contactGroupEdit = new ContactGroup();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  loadDepartments() {
    this.departmentService.getDepartments(-1, -1, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.departmentList = results;
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("getDepartments failed", "An error occured while trying to get equipments from the server", MessageSeverity.error);
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


  newContactGroup() {
    this.isNewContactGroup = true;
    this.showValidationErrors = true;

    this.editingContactGroupName = null;
    this.selectedValues = {};
    this.contactGroupEdit = new ContactGroup();
    this.contactGroupEdit.institutionId = this.accountService.currentUser.institutionId;
    return this.contactGroupEdit;
  }

  editContactGroup(contactGroup: ContactGroup) {
    if (contactGroup) {
      this.isNewContactGroup = false;
      this.showValidationErrors = true;

      this.editingContactGroupName = contactGroup.name;
      this.selectedValues = {};
      this.contactGroupEdit = new ContactGroup();
      Object.assign(this.contactGroupEdit, contactGroup);
      this.contactGroupEdit.institutionId = this.accountService.currentUser.institutionId;
      return this.contactGroupEdit;
    }
    else {
      return this.newContactGroup();
    }
  }

  deleteMember(row: ContactGroupMember) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" ?', DialogType.confirm, () => this.deleteMemberHelper(row));
  }

  deleteMemberHelper(row: ContactGroupMember) {

    this.contactGroupEdit.members = this.contactGroupEdit.members.filter(item => item !== row);
  }

  editMember(row: ContactGroupMember) {
    this.openContactDialog('Edit Contact', row);
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageContactGroups() {
    return this.accountService.userHasPermission(Permission.manageContactGroupsPermission)
  }
}
