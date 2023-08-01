import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { MediaExtension } from '../models/media-extension.model';
import { ConfigurationService } from './configuration.service';

export type MediaExtensionsChangedOperation = "add" | "delete" | "modify";
export type MediaExtensionsChangedEventArg = { mediaExtensions: MediaExtension[] | string[], operation: MediaExtensionsChangedOperation };

@Injectable()
export class MediaExtensionService {

  public static readonly mediaExtensionAddedOperation: MediaExtensionsChangedOperation = "add";
  public static readonly mediaExtensionDeletedOperation: MediaExtensionsChangedOperation = "delete";
  public static readonly mediaExtensionModifiedOperation: MediaExtensionsChangedOperation = "modify";

  private _mediaExtensionsChanged = new Subject<MediaExtensionsChangedEventArg>();

  private readonly _mediaExtensionUrl: string = "/api/mediaextension";
  get mediaExtensionUrl() { return this.configurations.baseUrl + this._mediaExtensionUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onMediaExtensionsChanged(mediaExtensions: MediaExtension[] | string[], op: MediaExtensionsChangedOperation) {
    this._mediaExtensionsChanged.next({ mediaExtensions: mediaExtensions, operation: op });
  }


  onMediaExtensionsCountChanged(mediaExtensions: MediaExtension[] | string[]) {
    return this.onMediaExtensionsChanged(mediaExtensions, MediaExtensionService.mediaExtensionModifiedOperation);
  }


  getMediaExtensionsChangedEvent(): Observable<MediaExtensionsChangedEventArg> {
    return this._mediaExtensionsChanged.asObservable();
  }

  getMediaExtensions(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<MediaExtension[]>(this.mediaExtensionUrl + '/list', page, pageSize));
  }

  getMediaExtensionByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<MediaExtension[]>(this.mediaExtensionUrl + '/list', institutionId));
  }

  getMediaExtensionByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<MediaExtension>(this.mediaExtensionUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateMediaExtension(mediaExtension: MediaExtension) {
    return this.commonEndpoint.getUpdateEndpoint(this.mediaExtensionUrl, mediaExtension, mediaExtension.id).pipe(
      tap(data => this.onMediaExtensionsChanged([mediaExtension], MediaExtensionService.mediaExtensionModifiedOperation)));
  }

  newMediaExtension(mediaExtension: MediaExtension) {
    return this.commonEndpoint.getNewEndpoint<MediaExtension>(this.mediaExtensionUrl, mediaExtension).pipe<MediaExtension>(
      tap(data => this.onMediaExtensionsChanged([mediaExtension], MediaExtensionService.mediaExtensionAddedOperation)));
  }


  deleteMediaExtension(id): Observable<MediaExtension> {
    return this.commonEndpoint.getDeleteEndpoint<MediaExtension>(this.mediaExtensionUrl, <string>id).pipe<MediaExtension>(
      tap(data => this.onMediaExtensionsChanged([data], MediaExtensionService.mediaExtensionDeletedOperation)));
  }
}
