import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { MapModel } from '../models/map.model';
import { PointModel } from '../models/point.model';
import { ConfigurationService } from './configuration.service';

export type MapsChangedOperation = "add" | "delete" | "modify";
export type MapsChangedEventArg = { maps: MapModel[] | string[], operation: MapsChangedOperation };

@Injectable()
export class MapService {

  public static readonly mapAddedOperation: MapsChangedOperation = "add";
  public static readonly mapDeletedOperation: MapsChangedOperation = "delete";
  public static readonly mapModifiedOperation: MapsChangedOperation = "modify";

  private _mapsChanged = new Subject<MapsChangedEventArg>();

  private readonly _mapUrl: string = "/api/map";
  get mapUrl() { return this.configurations.baseUrl + this._mapUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onMapsChanged(maps: MapModel[] | string[], op: MapsChangedOperation) {
    this._mapsChanged.next({ maps: maps, operation: op });
  }


  onMapsCountChanged(maps: MapModel[] | string[]) {
    return this.onMapsChanged(maps, MapService.mapModifiedOperation);
  }


  getMapsChangedEvent(): Observable<MapsChangedEventArg> {
    return this._mapsChanged.asObservable();
  }

  getMaps(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<MapModel[]>(this.mapUrl + '/list', page, pageSize));
  }

  getMapByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<MapModel[]>(this.mapUrl + '/list', institutionId));
  }

  getMapByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<MapModel>(this.mapUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  getFloorConnector(mapId) {

    return this.commonEndpoint.get<PointModel[]>(this.mapUrl + '/floorconnector?mapId=' + mapId);
  }

  updateMap(map: MapModel) {
    return this.commonEndpoint.getUpdateEndpoint(this.mapUrl, map, map.id).pipe(
      tap(data => this.onMapsChanged([map], MapService.mapModifiedOperation)));
  }

  newMap(map: MapModel) {
    return this.commonEndpoint.getNewEndpoint<MapModel>(this.mapUrl, map).pipe<MapModel>(
      tap(data => this.onMapsChanged([map], MapService.mapAddedOperation)));
  }


  deleteMap(id): Observable<MapModel> {
    return this.commonEndpoint.getDeleteEndpoint<MapModel>(this.mapUrl, <string>id).pipe<MapModel>(
      tap(data => this.onMapsChanged([data], MapService.mapDeletedOperation)));
  }

  getRoute(from, to, wheel, sheltered) {
    return this.commonEndpoint.get(this.mapUrl + '/getroute?startDirectoryId=' + from + '&destDirectoryId=' + to + '&wheelchair=' + wheel + '&sheltered=' + sheltered)
  }

  getPointById(id) {
    return this.commonEndpoint.get(this.mapUrl + '/getpointbyid?id=' + id )
  }

  getMapById(id) {
    return this.commonEndpoint.get(this.mapUrl + '/getmapbyid?id=' + id)
  }
}
