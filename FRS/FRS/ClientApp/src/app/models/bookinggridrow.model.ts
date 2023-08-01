import { ReservationTime } from "./reservationtime.model";
import { Location } from "./location.model";
import { Reservation } from "./reservation.model";
export class BookingGridRow {
  constructor(location?: Location, locationTimeIntervals?: LocationTimeInterval[], count?: number) {
    this.location = location;
    this.locationTimeIntervals = locationTimeIntervals;
    this.count = count;
  }
  public location: Location;
  public locationName: string;
  public locationTimeIntervals: LocationTimeInterval[];
  public count: number;
}

export class BookingGrid {
  constructor(location?: Location, rows?: BookingGridRow[]) {
    this.location = location;
    this.rows = rows;
  }
  public id: number;
  public location: Location;
  public rows: BookingGridRow[];
}

export class LocationTimeInterval {
  public selected: boolean;
  public isNewSelected: boolean;
  public isBlocked: boolean;
  public colSpan: number;
  public timeInterval: ReservationTime;
  public reservation: Reservation;
  public skip: boolean;
}
