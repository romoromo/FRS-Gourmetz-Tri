import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Building } from '../models/building.model';
import { ConfigurationService } from './configuration.service';

export type BuildingsChangedOperation = "add" | "delete" | "modify";
export type BuildingsChangedEventArg = { buildings: Building[] | string[], operation: BuildingsChangedOperation };

@Injectable()
export class BuildingService {

  public static readonly buildingAddedOperation: BuildingsChangedOperation = "add";
  public static readonly buildingDeletedOperation: BuildingsChangedOperation = "delete";
  public static readonly buildingModifiedOperation: BuildingsChangedOperation = "modify";

  private _buildingsChanged = new Subject<BuildingsChangedEventArg>();

  private readonly _buildingUrl: string = "/api/building";
  get buildingUrl() { return this.configurations.baseUrl + this._buildingUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }


  private onBuildingsChanged(buildings: Building[] | string[], op: BuildingsChangedOperation) {
    this._buildingsChanged.next({ buildings: buildings, operation: op });
  }


  onBuildingsCountChanged(buildings: Building[] | string[]) {
    return this.onBuildingsChanged(buildings, BuildingService.buildingModifiedOperation);
  }


  getBuildingsChangedEvent(): Observable<BuildingsChangedEventArg> {
    return this._buildingsChanged.asObservable();
  }

  getBuildings(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<Building[]>(this.buildingUrl + '/list', page, pageSize));
  }

  getBuildingByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<Building[]>(this.buildingUrl + '/list', institutionId));
  }

  getBuildingByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<Building>(this.buildingUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateBuilding(building: Building) {
    return this.commonEndpoint.getUpdateEndpoint(this.buildingUrl, building, building.id).pipe(
      tap(data => this.onBuildingsChanged([building], BuildingService.buildingModifiedOperation)));
  }

  newBuilding(building: Building) {
    return this.commonEndpoint.getNewEndpoint<Building>(this.buildingUrl, building).pipe<Building>(
      tap(data => this.onBuildingsChanged([building], BuildingService.buildingAddedOperation)));
  }


  deleteBuilding(id): Observable<Building> {
    return this.commonEndpoint.getDeleteEndpoint<Building>(this.buildingUrl, <string>id).pipe<Building>(
      tap(data => this.onBuildingsChanged([data], BuildingService.buildingDeletedOperation)));
  }
}
