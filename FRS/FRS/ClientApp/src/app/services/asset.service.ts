import { Injectable, Injector } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient, HttpEvent } from '@angular/common/http';
import { Observable, Subject, forkJoin, throwError } from 'rxjs';
import { catchError, mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';
import { AssetType } from '../models/asset-type.model';
import { AssetModel } from '../models/asset-model.model';
import { Asset } from '../models/asset.model';
import { EndpointFactory } from './endpoint-factory.service';

export type AssetTypesChangedOperation = "add" | "delete" | "modify";
export type AssetTypesChangedEventArg = { assetTypes: AssetType[] | string[], operation: AssetTypesChangedOperation };

@Injectable()
export class AssetService extends EndpointFactory {

  public static readonly assetTypeAddedOperation: AssetTypesChangedOperation = "add";
  public static readonly assetTypeDeletedOperation: AssetTypesChangedOperation = "delete";
  public static readonly assetTypeModifiedOperation: AssetTypesChangedOperation = "modify";

  private _assetTypesChanged = new Subject<AssetTypesChangedEventArg>();

  private readonly _assetTypeUrl: string = "/api/asset/assettypes";
  get assetTypeUrl() { return this.configurations.baseUrl + this._assetTypeUrl; }

  private readonly _assetModelUrl: string = "/api/asset/assetmodels";
  get assetModelUrl() { return this.configurations.baseUrl + this._assetModelUrl; }

  private readonly _assetUrl: string = "/api/asset/assets";
  get assetUrl() { return this.configurations.baseUrl + this._assetUrl; }

  constructor(private router: Router, http: HttpClient,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService, injector: Injector) {
    super(http, configurations, injector);
  }
  
  getAssetTypeById(assetTypeId: string) {

    return this.commonEndpoint.getById<any>(this.assetTypeUrl + '/get', assetTypeId);
  }

  getAssetTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.assetTypeUrl + '/sieve/list', filter);
  }

  updateAssetType(assetType: AssetType) {
    if (assetType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.assetTypeUrl, assetType, assetType.id);
    }
  }


  newAssetType(assetType: AssetType) {
    return this.commonEndpoint.getNewEndpoint<AssetType>(this.assetTypeUrl, assetType);
  }


  deleteAssetType(assetTypeOrAssetTypeId: string | AssetType): Observable<AssetType> {
    return this.commonEndpoint.getDeleteEndpoint<AssetType>(this.assetTypeUrl, <string>assetTypeOrAssetTypeId);
  }

  getAssetModelById(assetModelId: string) {

    return this.commonEndpoint.getById<any>(this.assetModelUrl + '/get', assetModelId);
  }

  getAssetModelsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.assetModelUrl + '/sieve/list', filter);
  }

  updateAssetModel(assetModel: AssetModel) {
    if (assetModel.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.assetModelUrl, assetModel, assetModel.id);
    }
  }


  newAssetModel(assetModel: AssetModel) {
    return this.commonEndpoint.getNewEndpoint<AssetModel>(this.assetModelUrl, assetModel);
  }


  deleteAssetModel(assetModelOrAssetModelId: string | AssetModel): Observable<AssetModel> {
    return this.commonEndpoint.getDeleteEndpoint<AssetModel>(this.assetModelUrl, <string>assetModelOrAssetModelId);
  }

  getAssetById(assetId: string) {

    return this.commonEndpoint.getById<any>(this.assetUrl + '/get', assetId);
  }

  getAssetsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.assetUrl + '/sieve/list', filter);
  }

  updateAsset(asset: Asset) {
    if (asset.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.assetUrl, asset, asset.id);
    }
  }


  newAsset(asset: Asset) {
    return this.commonEndpoint.getNewEndpoint<Asset>(this.assetUrl, asset);
  }


  deleteAsset(assetOrAssetId: string | Asset): Observable<Asset> {
    return this.commonEndpoint.getDeleteEndpoint<Asset>(this.assetUrl, <string>assetOrAssetId);
  }

  importFile<T>(data: any) {
    return this.http.post<T>(this.assetUrl + '/import', data, { reportProgress: true, observe: 'events' }).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.importFile(data));
      }));
  }


}
