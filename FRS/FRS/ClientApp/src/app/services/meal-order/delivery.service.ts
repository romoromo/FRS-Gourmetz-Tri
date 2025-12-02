import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { AccountEndpoint } from '../account-endpoint.service';
import { AuthService } from '../auth.service';
import { CommonEndpoint } from '../common-endpoint.service';
import { ConfigurationService } from '../configuration.service';
import { AssetComponentFilter, CatererAssetFilter, Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { Staff } from 'src/app/models/meal-order/staff.model';
import { CatererInfo, CatererInfoSimple } from '../../models/meal-order/caterer-info.model';
import { BentoBoxType } from '../../models/meal-order/bento-box-type.model';
import { BentoAsset } from '../../models/meal-order/bento-asset.model';
import { CartonType } from '../../models/meal-order/carton-type.model';
import { CartonAsset } from '../../models/meal-order/carton-asset.model';
import { TrackingStatus } from '../../models/meal-order/tracking-status.model';
import { DeliveryOrder } from '../../models/meal-order/delivery-order.model';
import { DeliveryOrderNew } from '../../models/meal-order/delivery-order-new.model';
import { StoreInventory } from '../../models/meal-order/store-inventory.model';
import { Outlet, OutletProfile, OutletTerm, OutletSimple } from 'src/app/models/meal-order/outlet.model';
import { StoreInfo } from '../../models/meal-order/store-info.model';
import { Driver } from 'src/app/models/meal-order/driver.model';
import { Route } from 'src/app/models/meal-order/route.model';
import { CatererAsset } from 'src/app/models/meal-order/caterer-asset.model';
import { AssetComponent } from 'src/app/models/meal-order/asset-component.model';

@Injectable()
export class DeliveryService {

  private readonly _catererInfoUrl: string = "/api/delivery/catererinfos";
  get catererInfoUrl() { return this.configurations.baseUrl + this._catererInfoUrl; }

  private readonly _outletUrl: string = "/api/delivery/outlets";
  get outletUrl() { return this.configurations.baseUrl + this._outletUrl; }

  private readonly _outletProfileUrl: string = "/api/delivery/outletprofiles";
  get outletProfileUrl() { return this.configurations.baseUrl + this._outletProfileUrl; }

  private readonly _outletTermUrl: string = "/api/delivery/outletterms";
  get outletTermUrl() { return this.configurations.baseUrl + this._outletTermUrl; }

  private readonly _bentoBoxTypeUrl: string = "/api/delivery/bentoboxtypes";
  get bentoBoxTypeUrl() { return this.configurations.baseUrl + this._bentoBoxTypeUrl; }

  private readonly _cartonTypeUrl: string = "/api/delivery/cartontypes";
  get cartonTypeUrl() { return this.configurations.baseUrl + this._cartonTypeUrl; }

  private readonly _bentoAssetUrl: string = "/api/delivery/bentoassets";
  get bentoAssetUrl() { return this.configurations.baseUrl + this._bentoAssetUrl; }

  private readonly _cartonAssetUrl: string = "/api/delivery/cartonassets";
  get cartonAssetUrl() { return this.configurations.baseUrl + this._cartonAssetUrl; }

  private readonly _trackingStatusUrl: string = "/api/delivery/trackingstatuss";
  get trackingStatusUrl() { return this.configurations.baseUrl + this._trackingStatusUrl; }

  private readonly _storeInfoUrl: string = "/api/delivery/storeinfos";
  get storeInfoUrl() { return this.configurations.baseUrl + this._storeInfoUrl; }

  private readonly _deliveryOrderUrl: string = "/api/delivery/deliveryorders";
  get deliveryOrderUrl() { return this.configurations.baseUrl + this._deliveryOrderUrl; }

  private readonly _deliveryOrderNewUrl: string = "/api/delivery/deliveryordernews";
  get deliveryOrderNewUrl() { return this.configurations.baseUrl + this._deliveryOrderNewUrl; }

  private readonly _storeInventoryUrl: string = "/api/delivery/storeinventories";
  get storeInventoryUrl() { return this.configurations.baseUrl + this._storeInventoryUrl; }

  private readonly _storeInventoryDetailUrl: string = "/api/delivery/storeinventorydetails";
  get storeInventoryDetailUrl() { return this.configurations.baseUrl + this._storeInventoryDetailUrl; }

  private readonly _driverUrl: string = "/api/delivery/drivers";
  get driverUrl() { return this.configurations.baseUrl + this._driverUrl; }

  private readonly _routeUrl: string = "/api/delivery/routes";
  get routeUrl() { return this.configurations.baseUrl + this._routeUrl; }

  private readonly _deliveryBaseurl: string = "/api/delivery";
  get deliveryBaseurl() { return this.configurations.baseUrl + this._deliveryBaseurl; }

  private readonly _catererAsset: string = "/api/delivery/catererasset";
  get catererAssetBaseurl() { return this.configurations.baseUrl + this._catererAsset; }

  private readonly _assetComponent: string ="api/delivery/assetcomponent";
  get assetComponentBaseurl() {return this.configurations.baseUrl + this._assetComponent}

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }


  //catererInfo
  getCatererInfoById(catererInfoId: string) {

    return this.commonEndpoint.getById<any>(this.catererInfoUrl + '/get', catererInfoId);
  }

  getCatererInfosSimpleByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.catererInfoUrl + '/simple/sieve/list', filter);
  }

  getCatererInfosByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.catererInfoUrl + '/sieve/list', filter);
  }

  updateCatererInfo(catererInfo: CatererInfoSimple) {
    if (catererInfo.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.catererInfoUrl, catererInfo, catererInfo.id);
    }
  }


  newCatererInfo(catererInfo: CatererInfoSimple) {
    return this.commonEndpoint.getNewEndpoint<CatererInfoSimple>(this.catererInfoUrl, catererInfo);
  }


  deleteCatererInfo(catererInfoOrCatererInfoId: string | CatererInfo): Observable<CatererInfo> {
    return this.commonEndpoint.getDeleteEndpoint<CatererInfo>(this.catererInfoUrl, <string>catererInfoOrCatererInfoId);
  }

  requestCaterer(catererId: string, outletId: string, status: string, isRsp: boolean, outletProfileId: string) {
    return this.commonEndpoint.get<any>(`${this.catererInfoUrl}/request/${catererId}/${outletId}/${status}/${isRsp}?outletProfileId=${outletProfileId}`);
  }

  //outletProfile
  getOutletProfileById(outletProfileId: string) {
    return this.commonEndpoint.getById<any>(this.outletProfileUrl + '/get', outletProfileId);
  }

  getOutletProfilesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.outletProfileUrl + '/sieve/list', filter);
  }

  updateOutletProfile(outletProfile: OutletProfile) {
    if (outletProfile.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.outletProfileUrl, outletProfile, outletProfile.id);
    }
  }


  newOutletProfile(outletProfile: OutletProfile) {
    return this.commonEndpoint.getNewEndpoint<OutletProfile>(this.outletProfileUrl, outletProfile);
  }


  deleteOutletProfile(outletProfileOrOutletProfileId: string | OutletProfile): Observable<OutletProfile> {
    return this.commonEndpoint.getDeleteEndpoint<OutletProfile>(this.outletProfileUrl, <string>outletProfileOrOutletProfileId);
  }

  //outlet
  getOutletById(outletId: string) {
    return this.commonEndpoint.getById<any>(this.outletUrl + '/get', outletId);
  }

  getOutletByIdSimple(outletId: string) {
    return this.commonEndpoint.getById<any>(this.outletUrl + '/simple/get', outletId);
  }

  getOutletsSimpleByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.outletUrl + '/simple/sieve/list', filter);
  }

  getOutletsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.outletUrl + '/sieve/list', filter);
  }

  updateOutlet(outlet: OutletSimple) {
    if (outlet.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.outletUrl, outlet, outlet.id);
    }
  }

  updateOutletStore(outlet: Outlet) {
    if (outlet.id) {
      return this.commonEndpoint.getUpdateStoreEndpoint(this.outletUrl, outlet, outlet.id);
    }
  }


  newOutlet(outlet: OutletSimple) {
    return this.commonEndpoint.getNewEndpoint<OutletSimple>(this.outletUrl, outlet);
  }


  deleteOutlet(outletOrOutletId: string | Outlet): Observable<Outlet> {
    return this.commonEndpoint.getDeleteEndpoint<Outlet>(this.outletUrl, <string>outletOrOutletId);
  }

  //bentoBoxType
  getBentoBoxTypeById(bentoBoxTypeId: string) {

    return this.commonEndpoint.getById<any>(this.bentoBoxTypeUrl + '/get', bentoBoxTypeId);
  }

  getBentoBoxTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.bentoBoxTypeUrl + '/sieve/list', filter);
  }

  updateBentoBoxType(bentoBoxType: BentoBoxType) {
    if (bentoBoxType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.bentoBoxTypeUrl, bentoBoxType, bentoBoxType.id);
    }
  }


  newBentoBoxType(bentoBoxType: BentoBoxType) {
    return this.commonEndpoint.getNewEndpoint<BentoBoxType>(this.bentoBoxTypeUrl, bentoBoxType);
  }


  deleteBentoBoxType(bentoBoxTypeOrBentoBoxTypeId: string | BentoBoxType): Observable<BentoBoxType> {
    return this.commonEndpoint.getDeleteEndpoint<BentoBoxType>(this.bentoBoxTypeUrl, <string>bentoBoxTypeOrBentoBoxTypeId);
  }

  //cartonType
  getCartonTypeById(cartonTypeId: string) {

    return this.commonEndpoint.getById<any>(this.cartonTypeUrl + '/get', cartonTypeId);
  }

  getCartonTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.cartonTypeUrl + '/sieve/list', filter);
  }

  updateCartonType(cartonType: CartonType) {
    if (cartonType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.cartonTypeUrl, cartonType, cartonType.id);
    }
  }


  newCartonType(cartonType: CartonType) {
    return this.commonEndpoint.getNewEndpoint<CartonType>(this.cartonTypeUrl, cartonType);
  }


  deleteCartonType(cartonTypeOrCartonTypeId: string | CartonType): Observable<CartonType> {
    return this.commonEndpoint.getDeleteEndpoint<CartonType>(this.cartonTypeUrl, <string>cartonTypeOrCartonTypeId);
  }

  //deliveryOrder
  getDeliveryOrderById(deliveryOrderId: string) {

    return this.commonEndpoint.getById<any>(this.deliveryOrderUrl + '/get', deliveryOrderId);
  }

  getDeliveryOrdersByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.deliveryOrderUrl + '/sieve/list', filter);
  }

  updateDeliveryOrder(deliveryOrder: DeliveryOrder) {
    if (deliveryOrder.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.deliveryOrderUrl, deliveryOrder, deliveryOrder.id);
    }
  }


  newDeliveryOrder(deliveryOrder: DeliveryOrder) {
    return this.commonEndpoint.getNewEndpoint<DeliveryOrder>(this.deliveryOrderUrl, deliveryOrder);
  }


  deleteDeliveryOrder(deliveryOrderOrDeliveryOrderId: string | DeliveryOrder): Observable<DeliveryOrder> {
    return this.commonEndpoint.getDeleteEndpoint<DeliveryOrder>(this.deliveryOrderUrl, <string>deliveryOrderOrDeliveryOrderId);
  }


  //deliveryOrderNew
  getDeliveryOrderNewById(deliveryOrderId: string) {

    return this.commonEndpoint.getById<any>(this.deliveryOrderNewUrl + '/get', deliveryOrderId);
  }

  getDeliveryOrderNewsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.deliveryOrderNewUrl + '/sieve/list', filter);
  }

  getBentoUsageByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.deliveryBaseurl + '/bentousages/sieve/list', filter);
  }

  updateDeliveryOrderNew(deliveryOrder: DeliveryOrderNew) {
    if (deliveryOrder.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.deliveryOrderNewUrl, deliveryOrder, deliveryOrder.id);
    }
  }


  newDeliveryOrderNew(deliveryOrder: DeliveryOrderNew) {
    return this.commonEndpoint.getNewEndpoint<DeliveryOrderNew>(this.deliveryOrderNewUrl, deliveryOrder);
  }


  deleteDeliveryOrderNew(deliveryOrderOrDeliveryOrderId: string | DeliveryOrderNew): Observable<DeliveryOrderNew> {
    return this.commonEndpoint.getDeleteEndpoint<DeliveryOrderNew>(this.deliveryOrderNewUrl, <string>deliveryOrderOrDeliveryOrderId);
  }


  //StoreInventory
  getStoreInventoryById(storeInventoryId: string) {

    return this.commonEndpoint.getById<any>(this.storeInventoryUrl + '/get', storeInventoryId);
  }

  getStoreInventoriesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.storeInventoryUrl + '/sieve/list', filter);
  }

  getStoreInventoryDetailsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.storeInventoryDetailUrl + '/sieve/list', filter);
  }

  updateStoreInventory(storeInventory: StoreInventory) {
    if (storeInventory.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.storeInventoryUrl, storeInventory, storeInventory.id);
    }
  }


  newStoreInventory(storeInventory: StoreInventory) {
    return this.commonEndpoint.getNewEndpoint<StoreInventory>(this.storeInventoryUrl, storeInventory);
  }


  deleteStoreInventory(storeInventoryOrStoreInventoryId: string | StoreInventory): Observable<StoreInventory> {
    return this.commonEndpoint.getDeleteEndpoint<StoreInventory>(this.storeInventoryUrl, <string>storeInventoryOrStoreInventoryId);
  }

  //trackingStatus
  getTrackingStatusById(trackingStatusId: string) {

    return this.commonEndpoint.getById<any>(this.trackingStatusUrl + '/get', trackingStatusId);
  }

  getTrackingStatussByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.trackingStatusUrl + '/sieve/list', filter);
  }

  updateTrackingStatus(trackingStatus: TrackingStatus) {
    if (trackingStatus.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.trackingStatusUrl, trackingStatus, trackingStatus.id);
    }
  }


  newTrackingStatus(trackingStatus: TrackingStatus) {
    return this.commonEndpoint.getNewEndpoint<TrackingStatus>(this.trackingStatusUrl, trackingStatus);
  }


  deleteTrackingStatus(trackingStatusOrTrackingStatusId: string | TrackingStatus): Observable<TrackingStatus> {
    return this.commonEndpoint.getDeleteEndpoint<TrackingStatus>(this.trackingStatusUrl, <string>trackingStatusOrTrackingStatusId);
  }

  //storeInfo
  getStoreInfoById(storeInfoId: string) {

    return this.commonEndpoint.getById<any>(this.storeInfoUrl + '/get', storeInfoId);
  }

  getStoreInfosByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.storeInfoUrl + '/sieve/list', filter);
  }

  updateStoreInfo(storeInfo: StoreInfo) {
    if (storeInfo.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.storeInfoUrl, storeInfo, storeInfo.id);
    }
  }


  newStoreInfo(storeInfo: StoreInfo) {
    return this.commonEndpoint.getNewEndpoint<StoreInfo>(this.storeInfoUrl, storeInfo);
  }


  deleteStoreInfo(storeInfoOrStoreInfoId: string | StoreInfo): Observable<StoreInfo> {
    return this.commonEndpoint.getDeleteEndpoint<StoreInfo>(this.storeInfoUrl, <string>storeInfoOrStoreInfoId);
  }

  //bentoAsset
  getBentoAssetById(bentoAssetId: string) {

    return this.commonEndpoint.getById<any>(this.bentoAssetUrl + '/get', bentoAssetId);
  }

  getBentoAssetsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.bentoAssetUrl + '/sieve/list', filter);
  }

  updateBentoAsset(bentoAsset: BentoAsset) {
    if (bentoAsset.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.bentoAssetUrl, bentoAsset, bentoAsset.id);
    }
  }


  newBentoAsset(bentoAsset: BentoAsset) {
    return this.commonEndpoint.getNewEndpoint<BentoAsset>(this.bentoAssetUrl, bentoAsset);
  }


  deleteBentoAsset(bentoAssetOrBentoAssetId: string | BentoAsset): Observable<BentoAsset> {
    return this.commonEndpoint.getDeleteEndpoint<BentoAsset>(this.bentoAssetUrl, <string>bentoAssetOrBentoAssetId);
  }

  //cartonAsset
  getCartonAssetById(cartonAssetId: string) {

    return this.commonEndpoint.getById<any>(this.cartonAssetUrl + '/get', cartonAssetId);
  }

  getCartonAssetsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.cartonAssetUrl + '/sieve/list', filter);
  }

  updateCartonAsset(cartonAsset: CartonAsset) {
    if (cartonAsset.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.cartonAssetUrl, cartonAsset, cartonAsset.id);
    }
  }


  newCartonAsset(cartonAsset: CartonAsset) {
    return this.commonEndpoint.getNewEndpoint<CartonAsset>(this.cartonAssetUrl, cartonAsset);
  }


  deleteCartonAsset(cartonAssetOrCartonAssetId: string | CartonAsset): Observable<CartonAsset> {
    return this.commonEndpoint.getDeleteEndpoint<CartonAsset>(this.cartonAssetUrl, <string>cartonAssetOrCartonAssetId);
  }

  downloadCartonAssetLabel(id) {
    return this.commonEndpoint.getFile<any>(this.cartonAssetUrl + '/label?id=' + id, {  });
  }

  //driver
  getDriverById(driverId: string) {

    return this.commonEndpoint.getById<any>(this.driverUrl + '/get', driverId);
  }

  getDriversByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.driverUrl + '/sieve/list', filter);
  }

  updateDriver(driver: Driver) {
    if (driver.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.driverUrl, driver, driver.id);
    }
  }


  newDriver(driver: Driver) {
    return this.commonEndpoint.getNewEndpoint<Driver>(this.driverUrl, driver);
  }


  deleteDriver(driverOrDriverId: string | Driver): Observable<Driver> {
    return this.commonEndpoint.getDeleteEndpoint<Driver>(this.driverUrl, <string>driverOrDriverId);
  }

  //route
  getRouteById(routeId: string) {

    return this.commonEndpoint.getById<any>(this.routeUrl + '/get', routeId);
  }

  getRoutesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.routeUrl + '/sieve/list', filter);
  }

  updateRoute(route: Route) {
    if (route.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.routeUrl, route, route.id);
    }
  }


  newRoute(route: Route) {
    return this.commonEndpoint.getNewEndpoint<Route>(this.routeUrl, route);
  }


  deleteRoute(routeOrRouteId: string | Route): Observable<Route> {
    return this.commonEndpoint.getDeleteEndpoint<Route>(this.routeUrl, <string>routeOrRouteId);
  }

  //printDO

  printDo(doId: string) {
    return this.commonEndpoint.getFileget<any>(this.deliveryBaseurl + '/printDo/' + doId);
  }

  printCCLabel(doId: string) {
    return this.commonEndpoint.getFileget<any>(this.deliveryBaseurl + '/printCCLabel/' + doId);
  }

  printCCLabelV2(doId: string, routeId: string) {
    return this.commonEndpoint.getFileget<any>(this.deliveryBaseurl + '/printCCLabelV2/' + doId + '/' + routeId);
  }

  getOutletTermsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.outletTermUrl + '/sieve/list', filter);
  }

  updateOutletTerm(outlet: OutletTerm) {
    if (outlet.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.outletTermUrl, outlet, outlet.id);
    }
  }

  newOutletTerm(outlet: OutletTerm) {
    return this.commonEndpoint.getNewEndpoint<OutletTerm>(this.outletTermUrl, outlet);
  }


  deleteOutletTerm(outletOrOutletId: string | OutletTerm): Observable<OutletTerm> {
    return this.commonEndpoint.getDeleteEndpoint<OutletTerm>(this.outletTermUrl, <string>outletOrOutletId);
  }



  getCatererAssetsByFilter(filter: CatererAssetFilter){
    return this.commonEndpoint.getSieve<PagedResult>(this.catererAssetBaseurl + '/sieve/list', filter);
  }

   newCatererAsset(asset: CatererAsset) {
    return this.commonEndpoint.getNewEndpoint<CatererAsset>(this.catererAssetBaseurl, asset);
  }

   getCatererAssetById(assetId: string) {
    return this.commonEndpoint.getById<any>(this.catererAssetBaseurl + '/get', assetId);
  }

  getCatererAssetByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.catererAssetBaseurl + '/sieve/list', filter);
  }

  updateCatererAsset(catererAsset: CatererAsset) {
    if (catererAsset.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.catererAssetBaseurl, catererAsset, catererAsset.id);
    }
  }

  deleteCatererAsset(id: string | CatererAsset): Observable<CatererAsset> {
    return this.commonEndpoint.getDeleteEndpoint<CatererAsset>(this.catererAssetBaseurl, <string>id);
  }

  getAssetQRCode(catererId: string){
    return this.commonEndpoint.get<any>(this.catererAssetBaseurl + '/getQRCode/' + catererId);
  }

  generateAssetQRCode(id:string, catererId: string){
    return this.commonEndpoint.getFile<any>(this.catererAssetBaseurl + '/generateQRCode?id='+id+'&catererId=' + catererId);
  }





  getAssetComponentsByFilter(filter: AssetComponentFilter){
    return this.commonEndpoint.getSieve<PagedResult>(this.assetComponentBaseurl + '/sieve/list', filter);
  }

   newAssetComponent(asset: AssetComponent) {
    return this.commonEndpoint.getNewEndpoint<AssetComponent>(this.assetComponentBaseurl, asset);
  }

   getAssetComponentById(assetId: string) {
    return this.commonEndpoint.getById<any>(this.assetComponentBaseurl + '/get', assetId);
  }

  getAssetComponentByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.assetComponentBaseurl + '/sieve/list', filter);
  }

  updateAssetComponent(assetComponent: AssetComponent) {
    if (assetComponent.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.assetComponentBaseurl, assetComponent, assetComponent.id);
    }
  }

  deleteAssetComponent(id: string | AssetComponent): Observable<AssetComponent> {
    return this.commonEndpoint.getDeleteEndpoint<AssetComponent>(this.assetComponentBaseurl, <string>id);
  }
}
