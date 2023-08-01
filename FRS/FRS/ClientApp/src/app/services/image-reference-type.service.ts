import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';
import { ImageReferenceType } from '../models/image-reference-type.model';

export type ImageReferenceTypesChangedOperation = "add" | "delete" | "modify";
export type ImageReferenceTypesChangedEventArg = { imageReferenceTypes: ImageReferenceType[] | string[], operation: ImageReferenceTypesChangedOperation };

@Injectable()
export class ImageReferenceTypeService {

  public static readonly imageReferenceTypeAddedOperation: ImageReferenceTypesChangedOperation = "add";
  public static readonly imageReferenceTypeDeletedOperation: ImageReferenceTypesChangedOperation = "delete";
  public static readonly imageReferenceTypeModifiedOperation: ImageReferenceTypesChangedOperation = "modify";

  private _imageReferenceTypesChanged = new Subject<ImageReferenceTypesChangedEventArg>();

  private readonly _imageReferenceTypeUrl: string = "/api/imagereferencetype";
  get imageReferenceTypeUrl() { return this.configurations.baseUrl + this._imageReferenceTypeUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }
  
  getImageReferenceTypeById(imageReferenceTypeId: string) {

    return this.commonEndpoint.getById<any>(this.imageReferenceTypeUrl + '/get', imageReferenceTypeId);
  }

  getImageReferenceTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.imageReferenceTypeUrl + '/imagereferencetypes/sieve/list', filter);
  }

  updateImageReferenceType(imageReferenceType: ImageReferenceType) {
    if (imageReferenceType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.imageReferenceTypeUrl, imageReferenceType, imageReferenceType.id);
    }
  }


  newImageReferenceType(imageReferenceType: ImageReferenceType) {
    return this.commonEndpoint.getNewEndpoint<ImageReferenceType>(this.imageReferenceTypeUrl, imageReferenceType);
  }


  deleteImageReferenceType(imageReferenceTypeOrImageReferenceTypeId: string | ImageReferenceType): Observable<ImageReferenceType> {
    return this.commonEndpoint.getDeleteEndpoint<ImageReferenceType>(this.imageReferenceTypeUrl, <string>imageReferenceTypeOrImageReferenceTypeId);
  }
}
