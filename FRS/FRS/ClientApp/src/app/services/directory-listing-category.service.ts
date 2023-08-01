import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { DirectoryListingCategory } from '../models/directory-listing-category.model';
import { ConfigurationService } from './configuration.service';

export type DirectoryListingCategorysChangedOperation = "add" | "delete" | "modify";
export type DirectoryListingCategorysChangedEventArg = { directoryListingCategorys: DirectoryListingCategory[] | string[], operation: DirectoryListingCategorysChangedOperation };

@Injectable()
export class DirectoryListingCategoryService {

  public static readonly directoryListingCategoryAddedOperation: DirectoryListingCategorysChangedOperation = "add";
  public static readonly directoryListingCategoryDeletedOperation: DirectoryListingCategorysChangedOperation = "delete";
  public static readonly directoryListingCategoryModifiedOperation: DirectoryListingCategorysChangedOperation = "modify";

  private _directoryListingCategorysChanged = new Subject<DirectoryListingCategorysChangedEventArg>();

  private readonly _directoryListingCategoryUrl: string = "/api/directorylistingcategory";
  get directoryListingCategoryUrl() { return this.configurations.baseUrl + this._directoryListingCategoryUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onDirectoryListingCategorysChanged(directoryListingCategorys: DirectoryListingCategory[] | string[], op: DirectoryListingCategorysChangedOperation) {
    this._directoryListingCategorysChanged.next({ directoryListingCategorys: directoryListingCategorys, operation: op });
  }


  onDirectoryListingCategorysCountChanged(directoryListingCategorys: DirectoryListingCategory[] | string[]) {
    return this.onDirectoryListingCategorysChanged(directoryListingCategorys, DirectoryListingCategoryService.directoryListingCategoryModifiedOperation);
  }


  getDirectoryListingCategorysChangedEvent(): Observable<DirectoryListingCategorysChangedEventArg> {
    return this._directoryListingCategorysChanged.asObservable();
  }

  getDirectoryListingCategorys(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<DirectoryListingCategory[]>(this.directoryListingCategoryUrl + '/list', page, pageSize));
  }

  getDirectoryListingCategoryByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<DirectoryListingCategory[]>(this.directoryListingCategoryUrl + '/list', institutionId));
  }

  getDirectoryListingCategoryByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<DirectoryListingCategory>(this.directoryListingCategoryUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateDirectoryListingCategory(directoryListingCategory: DirectoryListingCategory) {
    return this.commonEndpoint.getUpdateEndpoint(this.directoryListingCategoryUrl, directoryListingCategory, directoryListingCategory.id).pipe(
      tap(data => this.onDirectoryListingCategorysChanged([directoryListingCategory], DirectoryListingCategoryService.directoryListingCategoryModifiedOperation)));
  }

  newDirectoryListingCategory(directoryListingCategory: DirectoryListingCategory) {
    return this.commonEndpoint.getNewEndpoint<DirectoryListingCategory>(this.directoryListingCategoryUrl, directoryListingCategory).pipe<DirectoryListingCategory>(
      tap(data => this.onDirectoryListingCategorysChanged([directoryListingCategory], DirectoryListingCategoryService.directoryListingCategoryAddedOperation)));
  }


  deleteDirectoryListingCategory(id): Observable<DirectoryListingCategory> {
    return this.commonEndpoint.getDeleteEndpoint<DirectoryListingCategory>(this.directoryListingCategoryUrl, <string>id).pipe<DirectoryListingCategory>(
      tap(data => this.onDirectoryListingCategorysChanged([data], DirectoryListingCategoryService.directoryListingCategoryDeletedOperation)));
  }
}
