import { Injectable, Injector } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap, catchError } from 'rxjs/operators';


import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { ConfigurationService } from './configuration.service';
import { EndpointFactory } from './endpoint-factory.service';

@Injectable()
export class FileService extends EndpointFactory {

  private readonly _fileApiUrl: string = "/api/file";
  get fileApiUrl() { return this.configurations.baseUrl + this._fileApiUrl; }

  constructor(http: HttpClient, configurations: ConfigurationService, injector: Injector) {

    super(http, configurations, injector);
  }

  uploadFile<T>(data: any, subDir: string = ''): Observable<T> {

    return this.http.post<T>(this.fileApiUrl + '/upload?subDir=' + subDir, data, { reportProgress: true, observe: 'events' }).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.uploadFile(data, subDir));
      }));
  }

  getFile(filePath: string) {
    return this.configurations.baseUrl + '\\' + filePath;
  }

}
