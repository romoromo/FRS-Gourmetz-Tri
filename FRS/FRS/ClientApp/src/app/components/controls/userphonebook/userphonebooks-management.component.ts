import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Permission } from '../../../models/permission.model';
import { MatDialog } from '@angular/material';
import { UserSelectorComponent } from '../../common/user-selector/user-selector.component';
import { UserPhonebook } from 'src/app/models/userphonebook.model';
import { UserPhonebookEditorComponent } from './userphonebook-editor.component';
import { FileService } from 'src/app/services/file.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'userphonebooks-management',
  templateUrl: './userphonebooks-management.component.html',
  styleUrls: ['./userphonebooks-management.component.css']
})
export class UserPhonebooksManagementComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: UserPhonebook[] = [];
  rowsCache: UserPhonebook[] = [];
  allPermissions: Permission[] = [];
  editedUserPhonebook: UserPhonebook;
  sourceUserPhonebook: UserPhonebook;
  editingUserPhonebookName: { name: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('photoTemplate')
  photoTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('userPhonebookEditor')
  userPhonebookEditor: UserPhonebookEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private fileService: FileService, public dialog: MatDialog) {
  }

  openDialog(userPhonebook: UserPhonebook): void {
    const dialogRef = this.dialog.open(UserPhonebookEditorComponent, {
      data: { header: this.header, userPhonebook: userPhonebook },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('userPhonebooks.management.Name') },
      { prop: 'email', name: gT('userPhonebooks.management.Email')},
      { prop: 'phoneNumber', name: gT('userPhonebooks.management.PhoneNumber') },
      { prop: 'homeNo', name: gT('userPhonebooks.management.HomeNo') },
      { prop: 'mobileNo', name: gT('userPhonebooks.management.MobileNo') },
      { prop: 'designation', name: gT('userPhonebooks.management.Designation') },
      { name: 'Photo', width: 50, cellTemplate: this.photoTemplate, canAutoResize: false },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    this.loadData();
  }





  ngAfterViewInit() {

    this.userPhonebookEditor.changesSavedCallback = () => {
      this.addNewUserPhonebookToList();
      this.editorModal.hide();
    };

    this.userPhonebookEditor.changesCancelledCallback = () => {
      this.editedUserPhonebook = null;
      this.sourceUserPhonebook = null;
      this.editorModal.hide();
    };
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }


  addNewUserPhonebookToList() {
    if (this.sourceUserPhonebook) {
      Object.assign(this.sourceUserPhonebook, this.editedUserPhonebook);

      let sourceIndex = this.rowsCache.indexOf(this.sourceUserPhonebook, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceUserPhonebook, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedUserPhonebook = null;
      this.sourceUserPhonebook = null;
    }
    else {
      let userPhonebook = new UserPhonebook();
      Object.assign(userPhonebook, this.editedUserPhonebook);
      this.editedUserPhonebook = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>userPhonebook).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, userPhonebook);
      this.rows.splice(0, 0, userPhonebook);
      this.rows = [...this.rows];
    }
  }


  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.accountService.getUserPhonebooks(this.accountService.currentUser.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let userPhonebooks = results;

        userPhonebooks.forEach((userPhonebook, index, userPhonebooks) => {
          (<any>userPhonebook).index = index + 1;
        });


        this.rowsCache = [...userPhonebooks];
        this.rows = userPhonebooks;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve contacts from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.email, r.phoneNumber));
  }


  onEditorModalHidden() {
    this.editingUserPhonebookName = null;
    this.userPhonebookEditor.resetForm(true);
  }


  newUserPhonebook() {
    this.editingUserPhonebookName = null;
    this.sourceUserPhonebook = null;
    this.editedUserPhonebook = this.userPhonebookEditor.newUserPhonebook();
    //this.editorModal.show();
    this.header = 'New Contact';
    this.openDialog(this.editedUserPhonebook);
  }


  editUserPhonebook(row: UserPhonebook) {
    this.editingUserPhonebookName = { name: row.name };
    this.sourceUserPhonebook = row;
    this.editedUserPhonebook = this.userPhonebookEditor.editUserPhonebook(row);
    //this.editorModal.show();

    this.header = 'Edit Contact';
    this.openDialog(this.editedUserPhonebook);
  }

  deleteUserPhonebook(row: UserPhonebook) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" contact?', DialogType.confirm, () => this.deleteUserPhonebookHelper(row));
  }


  deleteUserPhonebookHelper(row: UserPhonebook) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.accountService.deleteUserPhonebook(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the contact.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  get canManageUserPhonebooks() {
    return this.accountService.userHasPermission(Permission.manageUserPhonebooksPermission)
  }

}
