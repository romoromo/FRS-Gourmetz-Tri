import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Inject, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { PlaylistService } from 'src/app/services/playlist.service';
import { MatDialog } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Playlist } from 'src/app/models/playlist.model';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AccountService } from 'src/app/services/account.service';
import { Subscription } from 'rxjs';
import { PlaylistCreatorComponent } from '../playlistCreator/playlist-creator.component';


@Component({
  selector: 'playlists-management',
  templateUrl: './playlists-management.component.html',
  styleUrls: ['./playlists-management.component.css']
})
export class PlaylistsManagementComponent implements OnInit, OnDestroy, AfterViewInit {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: Playlist[] = [];
  rowsCache: Playlist[] = [];
  allPermissions: Permission[] = [];
  editedPlaylist: Playlist;
  sourcePlaylist: Playlist;
  editingPlaylistName: { key: string };
  loadingIndicator: boolean;



  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal')
  editorModal: ModalDirective;

  @ViewChild('playlistEditor')
  playlistEditor: PlaylistCreatorComponent;
  header: string;
  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private playlistService: PlaylistService, public dialog: MatDialog) {
  }

  openDialog(playlist: Playlist): void {
    const dialogRef = this.dialog.open(PlaylistCreatorComponent, {
      data: { header: this.header, playlist: playlist },
      width: '80vw',
      height: '90vh',
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
      { prop: 'name', name: gT('playlists.management.Name') },
      { prop: 'description', name: gT('playlists.management.Description') },
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

    this.playlistEditor.changesSavedCallback = () => {
      this.addNewPlaylistToList();
      //this.editorModal.hide();
    };

    this.playlistEditor.changesCancelledCallback = () => {
      this.editedPlaylist = null;
      this.sourcePlaylist = null;
      //this.editorModal.hide();
    };
  }


  addNewPlaylistToList() {
    if (this.sourcePlaylist) {
      Object.assign(this.sourcePlaylist, this.editedPlaylist);

      let sourceIndex = this.rowsCache.indexOf(this.sourcePlaylist, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);

      sourceIndex = this.rows.indexOf(this.sourcePlaylist, 0);
      if (sourceIndex > -1)
        Utilities.moveArrayItem(this.rows, sourceIndex, 0);

      this.editedPlaylist = null;
      this.sourcePlaylist = null;
    }
    else {
      let playlist = new Playlist();
      Object.assign(playlist, this.editedPlaylist);
      this.editedPlaylist = null;

      let maxIndex = 0;
      for (let r of this.rowsCache) {
        if ((<any>r).index > maxIndex)
          maxIndex = (<any>r).index;
      }

      (<any>playlist).index = maxIndex + 1;

      this.rowsCache.splice(0, 0, playlist);
      this.rows.splice(0, 0, playlist);
      this.rows = [...this.rows];
    }
  }




  loadData() {
    //this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.subscription.add(this.playlistService.getPlaylists(-1, -1, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let playlists = results;
        let permissions = results[1];

        playlists.forEach((playlist, index, playlists) => {
          (<any>playlist).index = index + 1;
        });


        this.rowsCache = [...playlists];
        this.rows = playlists;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve playlist from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }


  onEditorModalHidden() {
    this.editingPlaylistName = null;
    //this.playlistEditor.resetForm(true);
  }


  newPlaylist() {
    this.editingPlaylistName = null;
    this.sourcePlaylist = null;
    this.editedPlaylist = this.playlistEditor.newPlaylist();
    this.header = 'New Playlist';
    this.openDialog(this.editedPlaylist);
    //this.editorModal.show();
  }


  editPlaylist(row: Playlist) {
    this.editingPlaylistName = { key: row.name };
    this.sourcePlaylist = row;
    this.editedPlaylist = this.playlistEditor.editPlaylist(row);
    this.header = 'Edit Playlist';
    //this.editorModal.show();
    this.openDialog(this.editedPlaylist);
  }

  deletePlaylist(row: Playlist) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" playlist?', DialogType.confirm, () => this.deletePlaylistHelper(row));
  }


  deletePlaylistHelper(row: Playlist) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.subscription.add(this.playlistService.deletePlaylist(row.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the playlist.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }


  get canManagePlaylists() {
    return true;//this.accountService.userHasPermission(Permission.managePlaylistPermission);
  }

}
