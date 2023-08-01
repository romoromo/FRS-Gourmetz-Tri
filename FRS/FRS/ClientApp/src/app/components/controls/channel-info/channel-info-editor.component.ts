import { Component, ViewChild, Inject } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ChannelInfo } from 'src/app/models/channel-info.model';
import { ChannelInfoService } from 'src/app/services/channel-info.service';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'channel-info-editor',
  templateUrl: './channel-info-editor.component.html',
  styleUrls: ['./channel-info-editor.component.css']
})
export class ChannelInfoEditorComponent {

  private isNewChannelInfo = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingChannelInfoName: string;
  private channelInfoEdit: ChannelInfo = new ChannelInfo();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private channelInfoService: ChannelInfoService, private accountService: AccountService,
    private fileService: FileService,
    public dialogRef: MatDialogRef<ChannelInfoEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.channelInfo) != typeof (undefined)) {
      if (data.channelInfo.id) {
        this.editChannelInfo(data.channelInfo);
      } else {
        this.newChannelInfo();
      }
    }
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    console.log("Save channel info")

    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.channelInfoEdit.institutionId = this.accountService.currentUser.institutionId;
    if (this.isNewChannelInfo) {
      this.channelInfoService.newChannelInfo(this.channelInfoEdit).subscribe(channelInfo => this.saveSuccessHelper(channelInfo), error => this.saveFailedHelper(error));
    }
    else {
      this.channelInfoService.updateChannelInfo(this.channelInfoEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(channelInfo?: ChannelInfo) {
    if (channelInfo)
      Object.assign(this.channelInfoEdit, channelInfo);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewChannelInfo)
      this.alertService.showMessage("Success", `Channel Info \"${this.channelInfoEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to channel info \"${this.channelInfoEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.channelInfoEdit = new ChannelInfo();
    this.resetForm();


    //if (!this.isNewChannelInfo && this.accountService.currentUser.facilities.some(r => r == this.editingChannelInfoName))
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
    this.channelInfoEdit = new ChannelInfo();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
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


  newChannelInfo() {
    this.isNewChannelInfo = true;
    this.showValidationErrors = true;

    this.editingChannelInfoName = null;
    this.selectedValues = {};
    this.channelInfoEdit = new ChannelInfo();

    return this.channelInfoEdit;
  }

  editChannelInfo(channelInfo: ChannelInfo) {
    if (channelInfo) {
      this.isNewChannelInfo = false;
      this.showValidationErrors = true;

      this.editingChannelInfoName = channelInfo.name;
      this.selectedValues = {};
      this.channelInfoEdit = new ChannelInfo();
      Object.assign(this.channelInfoEdit, channelInfo);

      return this.channelInfoEdit;
    }
    else {
      return this.newChannelInfo();
    }
  }

  get canManageChannelInfos() {
    return true;// this.accountService.userHasPermission(Permission.manageChannelInfosPermission)
  }
}
