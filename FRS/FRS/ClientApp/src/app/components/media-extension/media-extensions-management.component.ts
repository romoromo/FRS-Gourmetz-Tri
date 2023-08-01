import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { MediaExtensionService } from 'src/app/services/media-extension.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { MediaExtension } from 'src/app/models/media-extension.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { MediaExtensionEditorComponent } from './media-extension-editor.component';
import { Subscription } from 'rxjs';


@Component({
  selector: 'media-extensions-management',
  templateUrl: './media-extensions-management.component.html',
  styleUrls: ['./media-extensions-management.component.css']
})
export class MediaExtensionsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: MediaExtension[] = [];
  rowsCache: MediaExtension[] = [];
  allPermissions: Permission[] = [];
  editedMediaExtension: MediaExtension;
  sourceMediaExtension: MediaExtension;
  editingMediaExtensionName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('mediaExtensionEditor')
  mediaExtensionEditor: MediaExtensionEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private mediaExtensionService: MediaExtensionService, public dialog: MatDialog) {
  }

  openDialog(mediaExtension: MediaExtension): void {
    const dialogRef = this.dialog.open(MediaExtensionEditorComponent, {
      data: { header: this.header, mediaExtension: mediaExtension },
      width: '400px',
      disableClose: true
    });

    this.subscription.add(dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    }));
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'extension', name: gT('mediaExtensions.management.Code') },
      { prop: 'label', name: gT('mediaExtensions.management.Label') },
      { prop: 'min', name: gT('mediaExtensions.management.Min') },
      { prop: 'max', name: gT('mediaExtensions.management.Max') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    if (!this.accountService.currentUser.institutionId || this.accountService.currentUser.institutionId == '0') {
      this.columns.splice(1, 0, { prop: 'institutionName', name: gT('roles.management.Institution'), width: 120 });
    }
    this.loadData();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }



  ngAfterViewInit() {

    this.mediaExtensionEditor.changesSavedCallback = () => {
      this.addNewMediaExtensionToList();
      //this.editorModal.hide();
    };

    this.mediaExtensionEditor.changesCancelledCallback = () => {
      this.editedMediaExtension = null;
      this.sourceMediaExtension = null;
      //this.editorModal.hide();
    };
  }


  addNewMediaExtensionToList() {
    if (this.sourceMediaExtension) {
      Object.assign(this.sourceMediaExtension, this.editedMediaExtension);

      let sourceIndex = this.rowsCache.indexOf(this.sourceMediaExtension, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceMediaExtension, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedMediaExtension = null;
      this.sourceMediaExtension = null;
    }
    else {
      let mediaExtension = new MediaExtension();
      Object.assign(mediaExtension, this.editedMediaExtension);
      this.editedMediaExtension = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>mediaExtension).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, mediaExtension);
      this.rows.splice(0, 0, mediaExtension);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.mediaExtensionService.getMediaExtensionByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let mediaExtensions = results[0];
        let permissions = results[1];

        mediaExtensions.forEach((mediaExtension, index, mediaExtensions) => {
          (<any>mediaExtension).index = index + 1;
        });


        this.rowsCache = [...mediaExtensions];
        this.rows = mediaExtensions;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.extension, r.label));
  }


  onEditorModalHidden() {
    this.editingMediaExtensionName = null;
    this.mediaExtensionEditor.resetForm(true);
  }


  newMediaExtension() {
    this.editingMediaExtensionName = null;
    this.sourceMediaExtension = null;
    this.editedMediaExtension = this.mediaExtensionEditor.newMediaExtension();
    this.header = 'New';
    this.openDialog(this.editedMediaExtension);
    //this.editorModal.show();
  }


  editMediaExtension(row: MediaExtension) {
    this.editingMediaExtensionName = { key: row.extension };
    this.sourceMediaExtension = row;
    this.editedMediaExtension = this.mediaExtensionEditor.editMediaExtension(row);
    this.header = 'Edit';
    //this.editorModal.show();
    this.openDialog(this.editedMediaExtension);
  }

  deleteMediaExtension(row: MediaExtension) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.extension + '\"?', DialogType.confirm, () => this.deleteMediaExtensionHelper(row));
  }


  deleteMediaExtensionHelper(row: MediaExtension) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.mediaExtensionService.deleteMediaExtension(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the data.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageMediaExtensions() {
    return true;//this.accountService.userHasPermission(Permission.manageMediaExtensionPermission);
  }

}
