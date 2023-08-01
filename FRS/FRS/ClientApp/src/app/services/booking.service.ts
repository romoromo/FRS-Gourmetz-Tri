import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap, catchError } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { Reservation, CalendarFilter } from '../models/reservation.model';
import { Facility } from '../models/facility.model';
import { Location } from '../models/location.model';
import { LocationService } from './location.service';
import { ConfigurationService } from './configuration.service';
import { ReservationTime, TimeIntervalFilter } from '../models/reservationtime.model';
import { BookingGridRow, BookingGrid } from '../models/bookinggridrow.model';

export type ReservationsChangedOperation = "add" | "delete" | "modify";
export type ReservationsChangedEventArg = { reservations: Reservation[] | string[], operation: ReservationsChangedOperation };

export type FacilitiesChangedOperation = "add" | "delete" | "modify";
export type FacilitiesChangedEventArg = { facilities: Facility[] | string[], operation: FacilitiesChangedOperation };

@Injectable()
export class BookingService {

  public static readonly reservationAddedOperation: ReservationsChangedOperation = "add";
  public static readonly reservationDeletedOperation: ReservationsChangedOperation = "delete";
  public static readonly reservationModifiedOperation: ReservationsChangedOperation = "modify";
  private readonly _reservationUrl: string = "/api/booking";

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
    return this.onReservationsChanged(reservations, BookingService.reservationModifiedOperation);
  }


  getReservationsChangedEvent(): Observable<ReservationsChangedEventArg> {
    return this._reservationsChanged.asObservable();
  }

  getTimeIntervals(filter: TimeIntervalFilter) {
    return this.commonEndpoint.getPagedList<ReservationTime[]>(this.reservationUrl + '/GetAllTimeIntervals', null, null, true, filter);
  }

  getBookingGridRows(filter: CalendarFilter) {
    return this.commonEndpoint.getPagedList<BookingGrid[]>(this.reservationUrl + '/GetBookingGridRows', null, null, true, filter);
  }

  getReservationsLocationsFacilities(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<Reservation[]>(this.reservationUrl + '/GetAllReservations', page, pageSize, true),
      this.commonEndpoint.getFacilitiesEndpoint<Facility[]>(),
      this.locationService.getLocations());
  }

  getReservationsWithFilter(filter: CalendarFilter) {
    return this.commonEndpoint.getReservations<Reservation[]>(this.reservationUrl + '/GetAllReservations', filter);
  }

  getReservations(page?: number, pageSize?: number) {
    return this.commonEndpoint.getPagedList<Reservation[]>(this.reservationUrl + '/GetAllReservations', page, pageSize, true);
  }

  updateReservation(reservation: Reservation) {
    if (reservation.id) {
      return this.commonEndpoint.getUpdateEndpoint<any>(this.reservationUrl, reservation, reservation.id).pipe(
        tap(data => this.onReservationsChanged([reservation], BookingService.reservationModifiedOperation)));
    }
  }


  newReservation(reservation: Reservation) {
    return this.commonEndpoint.getNewEndpoint<Reservation>(this.reservationUrl, reservation).pipe<Reservation>(
      tap(data => this.onReservationsChanged([reservation], BookingService.reservationAddedOperation)));
  }


  deleteReservation(reservationOrReservationId: string | Reservation): Observable<Reservation> {

    if (typeof reservationOrReservationId === 'number' || reservationOrReservationId instanceof Number ||
      typeof reservationOrReservationId === 'string' || reservationOrReservationId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Reservation>(this.reservationUrl, <string>reservationOrReservationId).pipe<Reservation>(
        tap(data => this.onReservationsChanged([data], BookingService.reservationDeletedOperation)));
    }
    else {

      if (reservationOrReservationId.id) {
        return this.deleteReservation(reservationOrReservationId.id);
      }
    }
  }
}
