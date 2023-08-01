import { Injectable, Injector } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap, catchError } from 'rxjs/operators';


import { CommonEndpoint } from './common-endpoint.service';
import { ConfigurationService } from './configuration.service';
import { EndpointFactory } from './endpoint-factory.service';
import { Image } from '../models/image.model';

@Injectable()
export class ImageFileService extends EndpointFactory {

  private readonly _fileApiUrl: string = "/api/imagefile";
  get fileApiUrl() { return this.configurations.baseUrl + this._fileApiUrl; }


  constructor(http: HttpClient, configurations: ConfigurationService, injector: Injector, private router: Router,
    private commonEndpoint: CommonEndpoint) {

    super(http, configurations, injector);
  }

  uploadFile<T>(data: any): Observable<T> {

    return this.http.post<T>(this.fileApiUrl + '/upload', data, { reportProgress: true, observe: 'events' }).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.uploadFile(data));
      }));
  }

  getFile(filePath: string) {
    return this.configurations.baseUrl + '\\' + filePath;
  }

  newImage(image: Image) {
    return this.commonEndpoint.getNewEndpoint<Image>(this.fileApiUrl, image);
  }

  updateImage(image: Image) {
    return this.commonEndpoint.getUpdateEndpoint<Image>(this.fileApiUrl, image, image.id);
  }

  getImages(page?: number, pageSize?: number, institutionId?: string, institutionCode?: string, userId?: string) {
    var queryParams = '';
    if (userId) queryParams += '?userId=' + userId;
    else if (institutionId) queryParams += '?institutionId=' + institutionId;
    else if (institutionCode) queryParams += '?institutionCode=' + institutionCode;
    

    if (queryParams == '') {
      return this.commonEndpoint.getPagedList<Image[]>(this.fileApiUrl + '/images/list', page, pageSize);
    } else {
      return this.commonEndpoint.get<Image[]>(this.fileApiUrl + '/images/list' + queryParams);
    }
  }

  deleteImage(imageOrImageId: string | Image): Observable<Image> {

    if (typeof imageOrImageId === 'number' || imageOrImageId instanceof Number ||
      typeof imageOrImageId === 'string' || imageOrImageId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Image>(this.fileApiUrl, <string>imageOrImageId);
    }
    else {

      if (imageOrImageId.id) {
        return this.deleteImage(imageOrImageId.id);
      }
    }
  }

}
