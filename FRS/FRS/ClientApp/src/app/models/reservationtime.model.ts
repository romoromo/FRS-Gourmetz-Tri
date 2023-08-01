import { Reservation } from "./reservation.model";

export class ReservationTime {

  constructor(id?: number, description?: string, value?: string, hour?: number, minutes?: number) {

    this.id = id;
    this.description = description;
    this.hour = hour;
    this.minutes = minutes;

    let d = new Date();
    d.setHours(hour, minutes);
    this.value = d;
  }

  public id: number;
  public description: string;
  public value: Date;
  public hour: number;
  public minutes: number;


  public toTimeIntervalId: number;
  public toTimeIntervalDescription: string;
  public toTimeIntervalValue: Date;
  public toTimeIntervalHour: number;
  public toTimeIntervalMinutes: number;
  //public toTimeInterval: ReservationTime;
}

export class TimeIntervalFilter {
  constructor(locationId?: number, startDate?: Date, endDate?: Date, startTimeInterval?: ReservationTime, endTimeInterval?: ReservationTime) {
    this.locationId = locationId;
    this.startDate = startDate;
    this.endDate = endDate;
    this.startTimeInterval = startTimeInterval;
    this.endTimeInterval = endTimeInterval;
  }

  public locationId: number;
  public startDate: Date;
  public endDate: Date;
  public startTimeInterval: ReservationTime;
  public endTimeInterval: ReservationTime;
}
