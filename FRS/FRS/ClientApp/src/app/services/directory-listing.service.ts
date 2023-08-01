import { Injectable, Injector } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap, catchError } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { DirectoryListing } from '../models/directory-listing.model';

import { EndpointFactory } from './endpoint-factory.service';
import { ConfigurationService } from './configuration.service';

export type DirectoryListingsChangedOperation = "add" | "delete" | "modify";
export type DirectoryListingsChangedEventArg = { directoryListings: DirectoryListing[] | string[], operation: DirectoryListingsChangedOperation };

@Injectable()
export class DirectoryListingService extends EndpointFactory {

  public static readonly directoryListingAddedOperation: DirectoryListingsChangedOperation = "add";
  public static readonly directoryListingDeletedOperation: DirectoryListingsChangedOperation = "delete";
  public static readonly directoryListingModifiedOperation: DirectoryListingsChangedOperation = "modify";

  private _directoryListingsChanged = new Subject<DirectoryListingsChangedEventArg>();

  private readonly _directoryListingUrl: string = "/api/directorylisting";
  get directoryListingUrl() { return this.configurations.baseUrl + this._directoryListingUrl; }

  constructor(private router: Router, http: HttpClient,
    private commonEndpoint: CommonEndpoint, public configurations: ConfigurationService, injector: Injector) {
    super(http, configurations, injector);
  }


  private onDirectoryListingsChanged(directoryListings: DirectoryListing[] | string[], op: DirectoryListingsChangedOperation) {
    this._directoryListingsChanged.next({ directoryListings: directoryListings, operation: op });
  }


  onDirectoryListingsCountChanged(directoryListings: DirectoryListing[] | string[]) {
    return this.onDirectoryListingsChanged(directoryListings, DirectoryListingService.directoryListingModifiedOperation);
  }


  getDirectoryListingsChangedEvent(): Observable<DirectoryListingsChangedEventArg> {
    return this._directoryListingsChanged.asObservable();
  }

  getDirectoryListings(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<DirectoryListing[]>(this.directoryListingUrl + '/list', page, pageSize));
  }

  getDirectoryListingsExcInt() {

    return forkJoin(
      this.commonEndpoint.getPagedList<DirectoryListing[]>(this.directoryListingUrl + '/listexcludeinternal'));
  }

  getDirectoryListingByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<DirectoryListing[]>(this.directoryListingUrl + '/list', institutionId));
  }

  getDirectoryListingByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<DirectoryListing>(this.directoryListingUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateDirectoryListing(directoryListing: DirectoryListing) {
    return this.commonEndpoint.getUpdateEndpoint(this.directoryListingUrl, directoryListing, directoryListing.id).pipe(
      tap(data => this.onDirectoryListingsChanged([directoryListing], DirectoryListingService.directoryListingModifiedOperation)));
  }

  newDirectoryListing(directoryListing: DirectoryListing) {
    return this.commonEndpoint.getNewEndpoint<DirectoryListing>(this.directoryListingUrl, directoryListing).pipe<DirectoryListing>(
      tap(data => this.onDirectoryListingsChanged([directoryListing], DirectoryListingService.directoryListingAddedOperation)));
  }


  deleteDirectoryListing(id): Observable<DirectoryListing> {
    return this.commonEndpoint.getDeleteEndpoint<DirectoryListing>(this.directoryListingUrl, <string>id).pipe<DirectoryListing>(
      tap(data => this.onDirectoryListingsChanged([data], DirectoryListingService.directoryListingDeletedOperation)));
  }

  importFile<T>(data: any): Observable<T> {

    return this.http.post<T>(this.directoryListingUrl + '/import', data, { reportProgress: true, observe: 'events' }).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.importFile(data));
      }));
  }
}
