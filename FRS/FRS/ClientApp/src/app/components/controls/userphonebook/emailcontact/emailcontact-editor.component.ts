//import { Component, ViewChild, Inject } from '@angular/core';

//import { AlertService, MessageSeverity, DialogType } from '../../../../services/alert.service';
//import { AccountService } from "../../../../services/account.service";
//import { Permission } from '../../../../models/permission.model';
//import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
//import { ContactGroupMember } from 'src/app/models/contactgroupmember.model';
//import { ContactGroupService } from 'src/app/services/contactgroup.service';


//@Component({
//  selector: 'cg-newemail-editor',
//  templateUrl: './emailcontact-editor.component.html',
//  styleUrls: ['./emailcontact-editor.component.css']
//})
//export class CGNewContactEmailEditorComponent {

//  private isNewContactGroupMember = false;
//  private isSaving: boolean;
//  private showValidationErrors: boolean = true;
//  private editingContactGroupMemberName: string;
//  private contactGroupMemberEdit: ContactGroupMember = new ContactGroupMember();
//  private allPermissions: Permission[] = [];
//  private selectedValues: { [key: string]: boolean; } = {};

//  public formResetToggle = true;

//  public changesSavedCallback: () => void;
//  public changesFailedCallback: () => void;
//  public changesCancelledCallback: () => void;


//  @ViewChild('f')
//  private form;



//  constructor(private alertService: AlertService, private contactGroupService: ContactGroupService, private accountService: AccountService,
//    public dialogRef: MatDialogRef<CGNewContactEmailEditorComponent>,
//    @Inject(MAT_DIALOG_DATA) public data: any) {
//    if (typeof (data.contactGroupMember) != typeof (undefined)) {
//      if (data.contactGroupMember.id) {
//        this.editContactGroupMember(data.contactGroupMember);
//      } else {
//        this.newContactGroupMember();
//      }
//    }
//  }



//  private showErrorAlert(caption: string, message: string) {
//    this.alertService.showMessage(caption, message, MessageSeverity.error);
//  }


//  private save() {
//    this.dialogRef.close(this.contactGroupMemberEdit);
//  }

  

//  private saveFailedHelper(error: any) {
//    this.isSaving = false;
//    this.alertService.stopLoadingMessage();
//    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
//    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

//    if (this.changesFailedCallback)
//      this.changesFailedCallback();
//  }


//  private cancel() {
//    this.contactGroupMemberEdit = new ContactGroupMember();

//    this.showValidationErrors = false;
//    this.resetForm();

//    this.alertService.resetStickyMessage();

//    if (this.changesCancelledCallback)
//      this.changesCancelledCallback();

//    this.dialogRef.close();
//  }
//  resetForm(replace = false) {

//    if (!replace) {
//      this.form.reset();
//    }
//    else {
//      this.formResetToggle = false;

//      setTimeout(() => {
//        this.formResetToggle = true;
//      });
//    }
//  }


//  newContactGroupMember() {
//    this.isNewContactGroupMember = true;
//    this.showValidationErrors = true;

//    this.editingContactGroupMemberName = null;
//    this.selectedValues = {};
//    this.contactGroupMemberEdit = new ContactGroupMember();

//    return this.contactGroupMemberEdit;
//  }

//  editContactGroupMember(contactGroupMember: ContactGroupMember) {
//    if (contactGroupMember) {
//      this.isNewContactGroupMember = false;
//      this.showValidationErrors = true;

//      this.editingContactGroupMemberName = contactGroupMember.name;
//      this.selectedValues = {};
//      this.contactGroupMemberEdit = new ContactGroupMember();
//      Object.assign(this.contactGroupMemberEdit, contactGroupMember);

//      return this.contactGroupMemberEdit;
//    }
//    else {
//      return this.newContactGroupMember();
//    }
//  }
//}
