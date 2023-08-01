import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Media } from '../models/media.model';
import { ConfigurationService } from './configuration.service';

export type MediasChangedOperation = "add" | "delete" | "modify";
export type MediasChangedEventArg = { medias: Media[] | string[], operation: MediasChangedOperation };

@Injectable()
export class MediaService {

  public static readonly mediaAddedOperation: MediasChangedOperation = "add";
  public static readonly mediaDeletedOperation: MediasChangedOperation = "delete";
  public static readonly mediaModifiedOperation: MediasChangedOperation = "modify";

  private _mediasChanged = new Subject<MediasChangedEventArg>();

  private readonly _mediaUrl: string = "/api/media";
  get mediaUrl() { return this.configurations.baseUrl + this._mediaUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onMediasChanged(medias: Media[] | string[], op: MediasChangedOperation) {
    this._mediasChanged.next({ medias: medias, operation: op });
  }


  onMediasCountChanged(medias: Media[] | string[]) {
    return this.onMediasChanged(medias, MediaService.mediaModifiedOperation);
  }


  getMediasChangedEvent(): Observable<MediasChangedEventArg> {
    return this._mediasChanged.asObservable();
  }

  getMediaById(mediaId: string) {

    return this.commonEndpoint.getById<any>(this.mediaUrl + '/get', mediaId);
  }

  getMedias(page?: number, pageSize?: number, institutionId?: string) {

    return this.commonEndpoint.get<Media[]>(this.mediaUrl + '/medias/list?institutionId=' + institutionId);
  }

  updateMedia(media: Media) {
    if (media.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.mediaUrl, media, media.id).pipe(
        tap(data => this.onMediasChanged([media], MediaService.mediaModifiedOperation)));
    }
  }


  newMedia(media: Media) {
    return this.commonEndpoint.getNewEndpoint<Media>(this.mediaUrl, media).pipe<Media>(
      tap(data => this.onMediasChanged([media], MediaService.mediaAddedOperation)));
  }


  deleteMedia(media: string | Media): Observable<Media> {

    if (typeof media === 'number' || media instanceof Number ||
      typeof media === 'string' || media instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Media>(this.mediaUrl, <string>media).pipe<Media>(
        tap(data => this.onMediasChanged([data], MediaService.mediaDeletedOperation)));
    }
    else {

      if (media.id) {
        return this.deleteMedia(media.id);
      }
    }
  }
}
