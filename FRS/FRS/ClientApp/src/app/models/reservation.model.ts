import { Permission } from './permission.model';
import { Facility } from './facility.model';
import { ContactGroup } from './contactgroup.model';
import { ReservationTime } from './reservationtime.model';
import { timeInterval } from 'rxjs/operators';
import { UserVehicle } from './uservehicle.model';

export class Reservation {

  constructor(shortDescription?: string, longDescription?: string, startDate?: Date, startTime?: ReservationTime, endDate?: Date, endTime?: ReservationTime, repeatEndDateTime?: Date,
    facilityName?: string, facilityId?: string, locationName?: string, locationId?: string, isAllDay?: boolean, status?: string, repeatType?: string, repeatEndDate?: Date,
    facilities?: Facility[], locationObj?: Location, timeIntervals?: ReservationTime[], invitees?: ReservationInvitee[], isAttendee?: boolean) {

    this.shortDescription = shortDescription;
    this.longDescription = longDescription;
    this.startDate = startDate;
    
    this.startTime = startTime;
    this.endDate = endDate;
    this.endTime = endTime;
    this.repeatEndDateTime = repeatEndDateTime;
    this.facilityName = facilityName;
    this.facilityId = facilityId;
    this.locationName = locationName;
    this.locationId = locationId;
    this.locationObj = locationObj;
    this.isAllDay = isAllDay;
    this.status = status;
    this.repeatType = repeatType;
    this.repeatEndDate = repeatEndDate;
    this.facilities = facilities;
    this.timeIntervals = timeIntervals;
    this.invitees = invitees;
    this.isAttendee = isAttendee;
    if (this.startTime == undefined) {
      this.startTime = new ReservationTime();
    }

    if (this.endTime == undefined) {
      this.endTime = new ReservationTime();
    }
  }

  public id: string;
  public shortDescription: string;
  public longDescription: string;
  public startDateTime: Date;
  public endDateTime: Date;
  public startDate: Date;
  public startTime: ReservationTime;
  public endDate: Date;
  public endTime: ReservationTime;
  public repeatEndDateTime?: Date;
  public repeatEndDate: Date;
  public facilityName: string;
  public facilityId: string;
  public isAllDay: boolean;
  public isRecurring: boolean;
  public status: string;
  public repeatType: string;
  public locationName: string;
  public locationId: string;
  public locationObj: Location;
  public facilities: any[];
  public timeIntervals: ReservationTime[];
  public invitees: ReservationInvitee[];
  public institutionId: string;
  public isAttendee: boolean;
  public parentId: string;
  public recurApplyChangesType: string;

  public selectedStartTime: string;
  public selectedEndTime: string;

  public contactGroups: ContactGroup[];
  public bookedBy: string;
  public lastUpdatedByName: string;
  public createdDate: Date;
  public updatedDate: Date;
  public notes: string;
  public createdBy: string;
  public requestedBy: string;
  public meetingContactNo: string;
  public createdOn: Date;
  public lastUpdatedOn: Date;
  public smartRoomScheduleId: string;
  public alternateMeetingTitle: string;
  public isSignage: boolean;
  public isKiosk: boolean;

  public DisplayTitle: string;
  public MeetingPurpose: string;
  public StatusRemark: string;

  public filePath: string;
  public fileName: string;

  public reservationPictures: ReservationPicture[];
}

export class ReservationPicture {
  constructor(reservationId?: string, pictureUrl?: string) {
    this.reservationId = reservationId;
    this.pictureUrl = pictureUrl;
  }

  public id: string;
  public reservationId: string;
  public pictureUrl: string;
}

export class ImageReference {
  constructor(reservationId?: string, filePath?: string, remarks?: string, fileId?: number, fileName?: string, imageReferenceTypeId?: string, referenceDate?: Date) {
    this.reservationId = reservationId;
    this.filePath = filePath;
    this.remarks = remarks;
    this.fileId = fileId;
    this.fileName = fileName;
    this.referenceDate = referenceDate;
    this.imageReferenceTypeId = imageReferenceTypeId;
  }

  public fileId: number;
  public id: string;
  public reservationId: string;
  public filePath: string;
  public fileName: string;
  public remarks: string;
  public referenceDate: Date;
  public imageReferenceTypeId: string;
  public imageReferenceColorId: string;
  public imageReferenceColorCode: string;
  public hidden: boolean;
}

export class ReservationInvitee {
  constructor(id?: number, email?: string, userId?: string, name?: string) {
    this.id = id;
    this.email = email
    this.userId = userId;
    this.name = name;
  }
  public id: number;
  public email: string;
  public userId: string;
  public name: string;
  public phoneNumber: string;
  public plateNumber: string;
  public vehicle: string;
  public issueDate?: Date;
  public expiryDate?: Date;
  public cardType: string;
  public contactGroupId?: string;
  public userVehicles: UserVehicle[];
}
export class CalendarFilter  {
  constructor(
    locationIds?: number[],
    facilityIds?: number[],
    viewDate?: Date,
    startDate?: Date,
    endDate?: Date,
    startTime?: Date,
    endTime?: Date,
    capacity?: number,
    currentUserId?: number,
    isForAttendance?: boolean,
    institutionId?: string,
    isForKiosk?: boolean
  ) {

  }
  public locationIds: number[];
  public facilityIds: number[];
  public viewDate: Date;
  public start: Date;
  public end: Date;
  public startTime: Date;
  public endTime: Date;
  public startTimeInterval: ReservationTime;
  public endTimeInterval: ReservationTime;
  public capacity: number;
  public bookedById: number;
  public hasAvailableTimeSlot: boolean;
  public currentUserId: number;
  public isForAttendance: boolean;
  public institutionId: string;
  public isBooking: boolean
}

export class VehicleLogFilter {
  constructor(
  ) {

  }
  public start: Date;
  public end: Date;
}
