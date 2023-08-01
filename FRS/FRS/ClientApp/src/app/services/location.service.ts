import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { catchError, mergeMap, tap } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { Location, LocationTreeFilter, LocationType } from '../models/location.model';
import { Institution } from '../models/institution.model';
import { Facility } from '../models/facility.model';
import { CalendarFilter } from '../models/reservation.model';
import { LocationTree } from '../models/location-tree.model';
import { ConfigurationService } from './configuration.service';
import { EndpointFactory } from './endpoint-factory.service';

export type LocationsChangedOperation = "add" | "delete" | "modify";
export type LocationsChangedEventArg = { locations: Location[] | string[], operation: LocationsChangedOperation };

@Injectable()
export class LocationService {

  public static readonly locationAddedOperation: LocationsChangedOperation = "add";
  public static readonly locationDeletedOperation: LocationsChangedOperation = "delete";
  public static readonly locationModifiedOperation: LocationsChangedOperation = "modify";

  private _locationsChanged = new Subject<LocationsChangedEventArg>();


  private readonly _locationUrl: string = "/api/location";
  get locationUrl() { return this.configurationService.baseUrl + this._locationUrl; }

  private readonly _institutionUrl: string = "/api/institution";
  get institutionUrl() { return this.configurationService.baseUrl + this._institutionUrl; }

  private readonly _simpleresultUrl: string = "/api/simpleresult";
  get simpleresultUrl() { return this.configurationService.baseUrl + this._simpleresultUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, private configurationService: ConfigurationService) {
  }


  private onLocationsChanged(locations: Location[] | string[], op: LocationsChangedOperation) {
    this._locationsChanged.next({ locations: locations, operation: op });
  }


  onLocationsUserCountChanged(locations: Location[] | string[]) {
    return this.onLocationsChanged(locations, LocationService.locationModifiedOperation);
  }


  getLocationsChangedEvent(): Observable<LocationsChangedEventArg> {
    return this._locationsChanged.asObservable();
  }

  getLocations(page?: number, pageSize?: number, institutionId?: string, isBooking?: boolean) {

    return this.commonEndpoint.getPagedList<Location[]>(this.locationUrl + '/locations/list?institutionId=' + institutionId + '&isBooking=' + isBooking, page, pageSize);
  }

  getLocationsByUser(userId?: string, isBooking?: boolean) {

    return this.commonEndpoint.getPagedList<any>(this.locationUrl + '/signage/' + userId + '/' + isBooking, null, null);
  }

  getLocationById(id: string) {

    return this.commonEndpoint.getById<Location>(this.locationUrl, id);
  }

  getLocationTree(filter?: LocationTreeFilter) {
    return this.commonEndpoint.getLocationTree<LocationTree>(this.locationUrl + '/locationtree', filter);
  }

  getLocationsAndInstitutions(page?: number, pageSize?: number, institutionId?: string) {

    return forkJoin(
      this.commonEndpoint.getPagedList<Location[]>(this.locationUrl + '/locations/list?institutionId=' + institutionId, page, pageSize),
      this.commonEndpoint.getPagedList<Institution[]>(this.institutionUrl + '/institutions/list'));
  }

  getLocationsAndFacilities(institutionId?: string) {

    return forkJoin(
      this.commonEndpoint.getPagedList<Location[]>(this.locationUrl + '/locations/list?institutionId=' + institutionId),
      this.commonEndpoint.getFacilitiesEndpoint<Facility[]>(null, null, institutionId),);
  }

  getLocationsByInstitutionId(id: number | string) {
    return this.commonEndpoint.getPagedList<Location[]>(this.locationUrl + '/locations/list?institutionId=' + id.toString());
      
  }

  getAvailableLocations(filter: CalendarFilter) {
    return this.commonEndpoint.getReservations<Location[]>(this.locationUrl + '/GetAvailableLocations', filter);
  }

  getCalendarLocations(filter: CalendarFilter) {
    return this.commonEndpoint.getReservations<any[]>(this.locationUrl + '/getcalendarlocations', filter);
  }

  getLocationTypes() {

    return this.commonEndpoint.getPagedList<LocationType[]>(this.locationUrl + '/locationtypes');
  }

  updateLocation(location: Location) {
    if (location.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.locationUrl, location, location.id).pipe(
        tap(data => this.onLocationsChanged([location], LocationService.locationModifiedOperation)));
    }
  }


  newLocation(location: Location) {
    return this.commonEndpoint.getNewEndpoint<Location>(this.locationUrl, location).pipe<Location>(
      tap(data => this.onLocationsChanged([location], LocationService.locationAddedOperation)));
  }


  deleteLocation(locationOrLocationId: string | Location): Observable<Location> {

    if (typeof locationOrLocationId === 'number' || locationOrLocationId instanceof Number ||
      typeof locationOrLocationId === 'string' || locationOrLocationId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Location>(this.locationUrl, <string>locationOrLocationId).pipe<Location>(
        tap(data => this.onLocationsChanged([data], LocationService.locationDeletedOperation)));
    }
    else {

      if (locationOrLocationId.id) {
        return this.deleteLocation(locationOrLocationId.id);
      }
    }
  }

  importFile<T>(data: any): Observable<T> {

    return this.commonEndpoint.importFile(this.locationUrl + '/importtree?institutionId=' + this.authService.currentUser.institutionId, data);
  }

  syncSmartRoomResources(institutionId?: string) {

  return this.commonEndpoint.get<any>(this.locationUrl + `/syncSmartRoomResources?institutionId=${institutionId}`);
  }

  syncSmartRoomSchedules(institutionId?: string) {
    return this.commonEndpoint.get<any>(this.locationUrl + `/syncSmartRoomSchedules?institutionId=${institutionId}`);
  }
}
