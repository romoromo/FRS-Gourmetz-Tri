import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { KioskSettingsService } from 'src/app/services/kiosk-settings.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { KioskSettings } from 'src/app/models/kiosk-settings.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { Subscription } from 'rxjs';
import { KioskSettingsEditComponent } from './kiosk-settings-edit.component';
import { Playlist } from '../../models/playlist.model';
import { PlaylistService } from 'src/app/services/playlist.service';
//import { PlaylistCreatorComponent } from '../playlistCreator/playlist-creator.component';


@Component({
  selector: 'kiosk-settings-list',
  templateUrl: './kiosk-settings-list.component.html',
  styleUrls: ['./kiosk-settings-list.component.css']
})
export class KioskSettingsListComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: KioskSettings[] = [];
  rowsCache: KioskSettings[] = [];
  allPermissions: Permission[] = [];
  editedSettings: KioskSettings;
  sourceSettings: KioskSettings;
  allPlaylists: Playlist[] = [];
  editingSettingsLabel: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('kioskSettingsEdit')
  kioskSettingsEdit: KioskSettingsEditComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private kioskSettingsService: KioskSettingsService, public dialog: MatDialog, private playlistService: PlaylistService) {
  }

  openDialog(settings: KioskSettings, allPlaylists: Playlist[]): void {
    const dialogRef = this.dialog.open(KioskSettingsEditComponent, {
      data: { header: this.header, settings: settings, allPlaylists: allPlaylists },
      width: '1000px',
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
      { prop: 'label', name: 'Label' },
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

    this.kioskSettingsEdit.changesSavedCallback = () => {
      this.addNewKioskSettingsToList();
      //this.editorModal.hide();
    };

    this.kioskSettingsEdit.changesCancelledCallback = () => {
      this.editedSettings = null;
      this.sourceSettings = null;
      //this.editorModal.hide();
    };
  }


  addNewKioskSettingsToList() {
    if (this.sourceSettings) {
      Object.assign(this.sourceSettings, this.editedSettings);

      let sourceIndex = this.rowsCache.indexOf(this.sourceSettings, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourceSettings, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedSettings = null;
      this.sourceSettings = null;
    }
    else {
      let kioskSettings = new KioskSettings();
      Object.assign(kioskSettings, this.editedSettings);
      this.editedSettings = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>kioskSettings).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, kioskSettings);
      this.rows.splice(0, 0, kioskSettings);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.kioskSettingsService.getKioskSettings(-1, -1, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let kioskSettings = results;
        let permissions = results[1];

        kioskSettings.forEach((setting, index, kioskSettings) => {
          (<any>setting).index = index + 1;
        });


        this.rowsCache = [...kioskSettings];
        this.rows = kioskSettings;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve kiosk settings from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));

    this.subscription.add(this.playlistService.getPlaylists(-1, -1, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let playlists = results;
        this.allPlaylists = playlists;

      },
        error => {
          //this.alertService.stopLoadingMessage();
          //this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve kiosk playlists from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.label));
  }


  onEditorModalHidden() {
    this.editingSettingsLabel = null;
    //this.playlistEditor.resetForm(true);
  }


  newKioskSettings() {
    this.editingSettingsLabel = null;
    this.sourceSettings = null;
    this.editedSettings = this.kioskSettingsEdit.newKioskSettings(this.allPlaylists);
    this.header = 'New Kiosk Settings';
    this.openDialog(this.editedSettings, this.allPlaylists);
    //this.editorModal.show();
  }


  editKioskSettings(row: KioskSettings) {
    this.editingSettingsLabel = { key: row.label };
    this.sourceSettings = row;
    this.editedSettings = this.kioskSettingsEdit.editKioskSettings(row, this.allPlaylists);
    this.header = 'Edit Kiosk Settings';
    //this.editorModal.show();
    this.openDialog(this.editedSettings, this.allPlaylists);
  }

  deleteKioskSettings(row: KioskSettings) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.label + '\" kiosk settings?', DialogType.confirm, () => this.deleteKioskSettingsHelper(row));
  }


  deleteKioskSettingsHelper(row: KioskSettings) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.kioskSettingsService.deletekioskSettings(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the kiosk settings.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManageKioskSettings() {
    return true;//this.accountService.userHasPermission(Permission.managePlaylistPermission);
  }

}
