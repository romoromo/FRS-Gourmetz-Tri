import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';
import { ImageReferenceColor } from '../models/image-reference-color.model';

export type ImageReferenceColorsChangedOperation = "add" | "delete" | "modify";
export type ImageReferenceColorsChangedEventArg = { imageReferenceColors: ImageReferenceColor[] | string[], operation: ImageReferenceColorsChangedOperation };

@Injectable()
export class ImageReferenceColorService {

  public static readonly imageReferenceColorAddedOperation: ImageReferenceColorsChangedOperation = "add";
  public static readonly imageReferenceColorDeletedOperation: ImageReferenceColorsChangedOperation = "delete";
  public static readonly imageReferenceColorModifiedOperation: ImageReferenceColorsChangedOperation = "modify";

  private _imageReferenceColorsChanged = new Subject<ImageReferenceColorsChangedEventArg>();

  private readonly _imageReferenceColorUrl: string = "/api/imagereferencecolor";
  get imageReferenceColorUrl() { return this.configurations.baseUrl + this._imageReferenceColorUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }
  
  getImageReferenceColorById(imageReferenceColorId: string) {

    return this.commonEndpoint.getById<any>(this.imageReferenceColorUrl + '/get', imageReferenceColorId);
  }

  getImageReferenceColorsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.imageReferenceColorUrl + '/imagereferencecolors/sieve/list', filter);
  }

  updateImageReferenceColor(imageReferenceColor: ImageReferenceColor) {
    if (imageReferenceColor.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.imageReferenceColorUrl, imageReferenceColor, imageReferenceColor.id);
    }
  }


  newImageReferenceColor(imageReferenceColor: ImageReferenceColor) {
    return this.commonEndpoint.getNewEndpoint<ImageReferenceColor>(this.imageReferenceColorUrl, imageReferenceColor);
  }


  deleteImageReferenceColor(imageReferenceColorOrImageReferenceColorId: string | ImageReferenceColor): Observable<ImageReferenceColor> {
    return this.commonEndpoint.getDeleteEndpoint<ImageReferenceColor>(this.imageReferenceColorUrl, <string>imageReferenceColorOrImageReferenceColorId);
  }
}
