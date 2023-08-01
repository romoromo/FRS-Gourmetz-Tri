import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Playlist } from '../models/playlist.model';
import { ConfigurationService } from './configuration.service';

//export type ContactGroupsChangedOperation = "add" | "delete" | "modify";
//export type ContactGroupsChangedEventArg = { contactgroups: ContactGroup[] | string[], operation: ContactGroupsChangedOperation };

@Injectable()
export class PlaylistService {

  //public static readonly contactgroupAddedOperation: ContactGroupsChangedOperation = "add";
  //public static readonly contactgroupDeletedOperation: ContactGroupsChangedOperation = "delete";
  //public static readonly contactgroupModifiedOperation: ContactGroupsChangedOperation = "modify";

  //private _contactgroupsChanged = new Subject<ContactGroupsChangedEventArg>();

  private readonly _playlistUrl: string = "/api/playlist";
  get playlistUrl() { return this.configurations.baseUrl + this._playlistUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  //private onContactGroupsChanged(contactgroups: ContactGroup[] | string[], op: ContactGroupsChangedOperation) {
  //  this._contactgroupsChanged.next({ contactgroups: contactgroups, operation: op });
  //}


  //onContactGroupsCountChanged(contactgroups: ContactGroup[] | string[]) {
  //  return this.onContactGroupsChanged(contactgroups, ContactGroupService.contactgroupModifiedOperation);
  //}


  //getContactGroupsChangedEvent(): Observable<ContactGroupsChangedEventArg> {
  //  return this._contactgroupsChanged.asObservable();
  //}

  getPlaylistById(playlistId: string) {

    return this.commonEndpoint.getById<any>(this.playlistUrl, playlistId);
  }

  getPlaylists(page?: number, pageSize?: number, institutionId?: string) {
    return this.commonEndpoint.getPagedList<Playlist[]>(this.playlistUrl + '/playlists/list?institutionId=' + institutionId, page, pageSize);
  }

  updatePlaylist(playlist: Playlist) {
    if (playlist.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.playlistUrl, playlist, playlist.id);
    }
  }


  newPlaylist(playlist: Playlist) {
    return this.commonEndpoint.getNewEndpoint<Playlist>(this.playlistUrl, playlist);
  }


  deletePlaylist(playlistOrPlaylistId: string | Playlist): Observable<Playlist> {

    if (typeof playlistOrPlaylistId === 'number' || playlistOrPlaylistId instanceof Number ||
      typeof playlistOrPlaylistId === 'string' || playlistOrPlaylistId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Playlist>(this.playlistUrl, <string>playlistOrPlaylistId);
    }
    else {

      if (playlistOrPlaylistId.id) {
        return this.deletePlaylist(playlistOrPlaylistId.id);
      }
    }
  }
}
