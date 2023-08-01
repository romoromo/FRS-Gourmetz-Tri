import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap, catchError } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { Reservation, CalendarFilter, VehicleLogFilter } from '../models/reservation.model';
import { Facility } from '../models/facility.model';
import { Location } from '../models/location.model';
import { LocationService } from './location.service';
import { ConfigurationService } from './configuration.service';
import { ReservationTime, TimeIntervalFilter } from '../models/reservationtime.model';
import { BookingGridRow } from '../models/bookinggridrow.model';
import { CalendarEvent } from 'calendar-utils';
import { VMSVehicleLog } from '../models/uservehicle.model';

export type ReservationsChangedOperation = "add" | "delete" | "modify";
export type ReservationsChangedEventArg = { reservations: Reservation[] | string[], operation: ReservationsChangedOperation };

export type FacilitiesChangedOperation = "add" | "delete" | "modify";
export type FacilitiesChangedEventArg = { facilities: Facility[] | string[], operation: FacilitiesChangedOperation };

@Injectable()
export class ReservationService {

  public static readonly reservationAddedOperation: ReservationsChangedOperation = "add";
  public static readonly reservationDeletedOperation: ReservationsChangedOperation = "delete";
  public static readonly reservationModifiedOperation: ReservationsChangedOperation = "modify";
  private readonly _reservationUrl: string = "/api/reservation";

  private _reservationsChanged = new Subject<ReservationsChangedEventArg>();

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, private locationService: LocationService,
    protected configurations: ConfigurationService ) {

  }

  get reservationUrl() { return this.configurations.baseUrl + this._reservationUrl; }

  private onReservationsChanged(reservations: Reservation[] | string[], op: ReservationsChangedOperation) {
    this._reservationsChanged.next({ reservations: reservations, operation: op });
  }


  onReservationsUserCountChanged(reservations: Reservation[] | string[]) {
    return this.onReservationsChanged(reservations, ReservationService.reservationModifiedOperation);
  }


  getReservationsChangedEvent(): Observable<ReservationsChangedEventArg> {
    return this._reservationsChanged.asObservable();
  }

  getTimeIntervals(filter: TimeIntervalFilter) {
    return this.commonEndpoint.getPagedList<ReservationTime[]>(this.reservationUrl + '/GetAllTimeIntervals', null, null, true, filter);
  }

  getBookingGridRows(filter: CalendarFilter) {
    return this.commonEndpoint.getPagedList<BookingGridRow[]>(this.reservationUrl + '/GetBookingGridRows', null, null, true, filter);
  }

  getReservationsLocationsFacilities(page?: number, pageSize?: number, filter?: CalendarFilter) {
    let institutionId = filter != null ? filter.institutionId : null;
    let isBooking = filter != null ? filter.isBooking : null;
    return forkJoin(
      this.commonEndpoint.getPagedList<Reservation[]>(this.reservationUrl + '/GetAllReservations', page, pageSize, true, filter),
      this.commonEndpoint.getFacilitiesEndpoint<Facility[]>(null, null, institutionId),
      this.locationService.getLocations(null, null, institutionId, isBooking));
  }

  getCalendarFilters(filter?: CalendarFilter) {
    let institutionId = filter != null ? filter.institutionId : null;
    let isBooking = filter != null ? filter.isBooking : null;
    return forkJoin(
      this.commonEndpoint.getFacilitiesEndpoint<Facility[]>(null, null, institutionId),
      this.locationService.getLocations(null, null, institutionId, isBooking));
  }

  getReservationsWithFilter(filter: CalendarFilter) {
    return this.commonEndpoint.getReservations<Reservation[]>(this.reservationUrl + '/GetAllReservations', filter);
  }

  getReservation(id?: string) {
    return this.commonEndpoint.get<any>(this.reservationUrl + '/get' + "?id=" + id);
  }

  getKioskReservation() {
    return this.commonEndpoint.get<any>(this.reservationUrl + '/GetKiosk')
  }

  getReservations(page?: number, pageSize?: number) {
    return this.commonEndpoint.getPagedList<Reservation[]>(this.reservationUrl + '/GetAllReservations', page, pageSize, true);
  }

  getCalendarEvents(filter?: CalendarFilter) {
    return this.commonEndpoint.getReservations<CalendarEvent[]>(this.reservationUrl + '/calendarevents', filter);
  }

  updateReservation(reservation: Reservation) {
    if (reservation.id) {
      return this.commonEndpoint.getUpdateEndpoint<any>(this.reservationUrl, reservation, reservation.id).pipe(
        tap(data => this.onReservationsChanged([reservation], ReservationService.reservationModifiedOperation)));
    }
  }


  newReservation(reservation: Reservation) {
    return this.commonEndpoint.getNewEndpoint<Reservation>(this.reservationUrl, reservation).pipe<Reservation>(
      tap(data => this.onReservationsChanged([reservation], ReservationService.reservationAddedOperation)));
  }


  deleteReservation(reservationOrReservationId: string | Reservation): Observable<Reservation> {

    if (typeof reservationOrReservationId === 'number' || reservationOrReservationId instanceof Number ||
      typeof reservationOrReservationId === 'string' || reservationOrReservationId instanceof String
      || typeof reservationOrReservationId == 'object') {
      var queryStrings = "";
      var id = "";
      if (typeof reservationOrReservationId == 'object') {
        queryStrings += "recurApplyChangesType=" + reservationOrReservationId.recurApplyChangesType;
        id = reservationOrReservationId.id;
      } else {
        id = reservationOrReservationId;
      }

      return this.commonEndpoint.getDeleteEndpoint<Reservation>(this.reservationUrl, id, queryStrings).pipe<Reservation>(
        tap(data => this.onReservationsChanged([data], ReservationService.reservationDeletedOperation)));
    }
    //else {

    //  if (reservationOrReservationId.id) {
    //    return this.deleteReservation(reservationOrReservationId.id);
    //  }
    //}
  }

  validateAttendance(reservationId, userName, name, company, designation) {
    return this.commonEndpoint.get<any>(this.reservationUrl + '/checkin' + "?reservationId=" + reservationId + '&userName=' + userName + '&name=' + name+ '&company=' + company + '&designation=' + designation);
  }

  validateFeedback(feedback: any) {
    return this.commonEndpoint.get<any>(this.reservationUrl + '/feedback' + "?reservationId=" + feedback.reservationId + '&userId=' + feedback.userId + '&feedback=' + feedback.feedback.name + '&comment=' + feedback.comment);
  }

  getCurrentLocationDetail(locationId?: string, start?: string, end?: string) {
    return this.commonEndpoint.get<any>(this.reservationUrl + '/locationcurrentdetail?locationId=' + locationId + '&start=' + start + '&end=' + end);
  }

  getVehicleLogs(filter: VehicleLogFilter) {
    return this.commonEndpoint.get<VMSVehicleLog[]>(this.reservationUrl + '/GetVehicleLogs', true, filter);
  }
}
