import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Floor } from '../models/floor.model';
import { ConfigurationService } from './configuration.service';

export type FloorsChangedOperation = "add" | "delete" | "modify";
export type FloorsChangedEventArg = { floors: Floor[] | string[], operation: FloorsChangedOperation };

@Injectable()
export class FloorService {

  public static readonly floorAddedOperation: FloorsChangedOperation = "add";
  public static readonly floorDeletedOperation: FloorsChangedOperation = "delete";
  public static readonly floorModifiedOperation: FloorsChangedOperation = "modify";

  private _floorsChanged = new Subject<FloorsChangedEventArg>();

  private readonly _floorUrl: string = "/api/floor";
  get floorUrl() { return this.configurations.baseUrl + this._floorUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onFloorsChanged(floors: Floor[] | string[], op: FloorsChangedOperation) {
    this._floorsChanged.next({ floors: floors, operation: op });
  }


  onFloorsCountChanged(floors: Floor[] | string[]) {
    return this.onFloorsChanged(floors, FloorService.floorModifiedOperation);
  }


  getFloorsChangedEvent(): Observable<FloorsChangedEventArg> {
    return this._floorsChanged.asObservable();
  }

  getFloors(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<Floor[]>(this.floorUrl + '/list', page, pageSize));
  }

  getFloorByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<Floor[]>(this.floorUrl + '/list', institutionId));
  }

  getFloorByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<Floor>(this.floorUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateFloor(floor: Floor) {
    return this.commonEndpoint.getUpdateEndpoint(this.floorUrl, floor, floor.id).pipe(
      tap(data => this.onFloorsChanged([floor], FloorService.floorModifiedOperation)));
  }

  newFloor(floor: Floor) {
    return this.commonEndpoint.getNewEndpoint<Floor>(this.floorUrl, floor).pipe<Floor>(
      tap(data => this.onFloorsChanged([floor], FloorService.floorAddedOperation)));
  }


  deleteFloor(id): Observable<Floor> {
    return this.commonEndpoint.getDeleteEndpoint<Floor>(this.floorUrl, <string>id).pipe<Floor>(
      tap(data => this.onFloorsChanged([data], FloorService.floorDeletedOperation)));
  }
}
