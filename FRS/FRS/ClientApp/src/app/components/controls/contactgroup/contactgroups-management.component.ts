import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { ContactGroup } from '../../../models/contactgroup.model';
import { Permission } from '../../../models/permission.model';
import { ContactGroupEditorComponent } from "./contactgroup-editor.component";
import { ContactGroupService } from 'src/app/services/contactgroup.service';
import { MatDialog } from '@angular/material';
import { UserSelectorComponent } from '../../common/user-selector/user-selector.component';


@Component({
  selector: 'contactgroups-management',
  templateUrl: './contactgroups-management.component.html',
  styleUrls: ['./contactgroups-management.component.css']
})
export class ContactGroupsManagementComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: ContactGroup[] = [];
  rowsCache: ContactGroup[] = [];
  allPermissions: Permission[] = [];
  editedContactGroup: ContactGroup;
  sourceContactGroup: ContactGroup;
  editingContactGroupName: { name: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('contactGroupEditor')
  contactGroupEditor: ContactGroupEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private contactGroupService: ContactGroupService, public dialog: MatDialog) {
  }

  openDialog(contactGroup: ContactGroup): void {
    const dialogRef = this.dialog.open(ContactGroupEditorComponent, {
      data: { header: this.header, contactGroup: contactGroup },
      width: '950px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.isSuccess) {
        this.loadData();
      }
    });
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('contactGroups.management.Name') },
      { prop: 'description', name: gT('contactGroups.management.Description') },
      { prop: 'departmentNames', name: gT('contactGroups.management.Department') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(4, 0, { prop: 'institutionName', name: gT('contactGroups.management.Institution'), width: 200 });
    }

    this.loadData();
  }





  ngAfterViewInit() {

    this.contactGroupEditor.changesSavedCallback = () => {
      this.addNewContactGroupToList();
      this.editorModal.hide();
    };

    this.contactGroupEditor.changesCancelledCallback = () => {
      this.editedContactGroup = null;
      this.sourceContactGroup = null;
      this.editorModal.hide();
    };
  }


  addNewContactGroupToList() {
    if (this.sourceContactGroup) {
      Object.assign(this.sourceContactGroup, this.editedContactGroup);

      let sourceIndex = this.rowsCache.indexOf(this.sourceContactGroup, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceContactGroup, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedContactGroup = null;
      this.sourceContactGroup = null;
    }
    else {
      let contactGroup = new ContactGroup();
      Object.assign(contactGroup, this.editedContactGroup);
      this.editedContactGroup = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>contactGroup).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, contactGroup);
      this.rows.splice(0, 0, contactGroup);
      this.rows = [...this.rows];
    }
  }

  getContactGroup(id: string) {
    return this.contactGroupService.getContactGroupById(id);
  }

  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.contactGroupService.getContactGroups(null, null, this.accountService.currentUser.institutionId, true)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let contactGroups = results;

        contactGroups.forEach((contactGroup, index, contactGroups) => {
          (<any>contactGroup).index = index + 1;
        });


        this.rowsCache = [...contactGroups];
        this.rows = contactGroups;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve contact groups from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingContactGroupName = null;
    this.contactGroupEditor.resetForm(true);
  }


  newContactGroup() {
    this.editingContactGroupName = null;
    this.sourceContactGroup = null;
    this.editedContactGroup = this.contactGroupEditor.newContactGroup();
    //this.editorModal.show();
    this.header = 'New Contact Group';
    this.openDialog(this.editedContactGroup);
  }


  editContactGroup(row: ContactGroup) {
    this.editingContactGroupName = { name: row.name };
    this.sourceContactGroup = row;
    this.editedContactGroup = this.contactGroupEditor.editContactGroup(row);
    //this.editorModal.show();

    this.header = 'Edit Contact Group';
    this.loadingIndicator = true;
    this.getContactGroup(row.id)
      .subscribe(result => {
        this.loadingIndicator = false;
        this.openDialog(result);

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve contact group from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  deleteContactGroup(row: ContactGroup) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" contact group?', DialogType.confirm, () => this.deleteContactGroupHelper(row));
  }


  deleteContactGroupHelper(row: ContactGroup) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.contactGroupService.deleteContactGroup(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the contact group.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  syncLdap() {
    this.alertService.showDialog('You are about to synchronise the active directory records to FRS. Do you wish to continue?',
      DialogType.confirm, () => {

      this.loadingIndicator = true;
      this.alertService.startLoadingMessage();
      this.alertService.startLoadingMessage("Synchronising LDAP data...");
      this.contactGroupService.syncLdap()
        .subscribe(results => {
          this.loadingIndicator = false;
          this.alertService.stopLoadingMessage();
          if (results && results.isSuccess) {
            this.alertService.showStickyMessage("Sync Success", "Successfully synced!", MessageSeverity.success);
          } else {
            this.alertService.showStickyMessage("Sync Error", `An error occured while synchronising the contact group.\r\nError: "${Utilities.getHttpResponseMessage(results.message)}"`,
              MessageSeverity.error, results.message);
          }
        },
          error => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;

            this.alertService.showStickyMessage("Sync Error", `An error occured while synchronising the contact group.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
              MessageSeverity.error);
          });
    });
  }

  get canManageContactGroups() {
    return this.accountService.userHasPermission(Permission.manageContactGroupsPermission)
  }

}
