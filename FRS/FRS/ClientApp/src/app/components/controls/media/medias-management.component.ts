import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { Media } from '../../../models/media.model';
import { Permission } from '../../../models/permission.model';
import { MediaEditorComponent } from "./media-editor.component";
import { MediaService } from 'src/app/services/media.service';
import { MatDialog } from '@angular/material';


@Component({
  selector: 'medias-management',
  templateUrl: './medias-management.component.html',
  styleUrls: ['./medias-management.component.css']
})
export class MediasManagementComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: Media[] = [];
  rowsCache: Media[] = [];
  allPermissions: Permission[] = [];
  editedMedia: Media;
  sourceMedia: Media;
  editingMediaName: { name: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('mediaEditor')
  mediaEditor: MediaEditorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private mediaService: MediaService, public dialog: MatDialog) {
  }

  openDialog(media: Media): void {
    const dialogRef = this.dialog.open(MediaEditorComponent, {
      data: { header: this.header, media: media },
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.loadData();
    });
  }

  ngOnInit() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('medias.management.Name') },
      { prop: 'description', name: gT('medias.management.Description') },
      { prop: 'directorySize', name: gT('medias.management.Size') },
      { prop: 'numberOfFiles', name: gT('medias.management.NumberOfFiles') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    this.loadData();
  }





  ngAfterViewInit() {

    this.mediaEditor.changesSavedCallback = () => {
      this.addNewMediaToList();
      this.editorModal.hide();
    };

    this.mediaEditor.changesCancelledCallback = () => {
      this.editedMedia = null;
      this.sourceMedia = null;
      this.editorModal.hide();
    };
  }


  addNewMediaToList() {
    if (this.sourceMedia) {
      Object.assign(this.sourceMedia, this.editedMedia);

      let sourceIndex = this.rowsCache.indexOf(this.sourceMedia, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceMedia, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedMedia = null;
      this.sourceMedia = null;
    }
    else {
      let facility = new Media();
      Object.assign(facility, this.editedMedia);
      this.editedMedia = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>facility).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, facility);
      this.rows.splice(0, 0, facility);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.mediaService.getMedias()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let medias = results;

        medias.forEach((facility, index, medias) => {
          (<any>facility).index = index + 1;
        });


        this.rowsCache = [...medias];
        this.rows = medias;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve medias from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingMediaName = null;
    this.mediaEditor.resetForm(true);
  }


  newMedia() {
    this.editingMediaName = null;
    this.sourceMedia = null;
    this.editedMedia = this.mediaEditor.newMedia();
    //this.editorModal.show();
    this.header = 'New Media';
    this.openDialog(this.editedMedia);
  }


  editMedia(row: Media) {
    this.editingMediaName = { name: row.name };
    this.sourceMedia = row;
    this.editedMedia = this.mediaEditor.editMedia(row);
    //this.editorModal.show();

    this.header = 'Edit Media';
    this.openDialog(this.editedMedia);
  }

  deleteMedia(row: Media) {
    this.alertService.showDialog('Deleting this media will also delete its files. Are you sure you want to delete \"' + row.name + '\" media?', DialogType.confirm, () => this.deleteMediaHelper(row));
  }


  deleteMediaHelper(row: Media) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.mediaService.deleteMedia(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the media.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }


  get canManageMedias() {
    return true; //this.accountService.userHasPermission(Permission.manageMediasPermission)
  }

}
