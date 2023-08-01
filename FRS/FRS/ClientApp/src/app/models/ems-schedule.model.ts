import { Permission } from './permission.model';
import { Facility } from './facility.model';
import { ContactGroup } from './contactgroup.model';
import { timeInterval } from 'rxjs/operators';

export class EmsSchedule {
  effectiveDateObj: Date;
    ineffectiveDateObj: Date;
    startTimeObj: Date;
    endTimeObj: Date;

  constructor(
    label?: string,
    locationId?: string,
    locationCode?: string,
    locationLabel?: string,
    emsProfileId?: string,
    emsProfileCode?: string,
    emsProfileLabel?: string,
    effectiveDate?: string,
    ineffectiveDate?: string,
    startTime?: string,
    endTime?: string,
    monday?: boolean,
    tuesday?: boolean,
    wednesday?: boolean,
    thursday?: boolean,
    friday?: boolean,
    saturday?: boolean,
    sunday?: boolean,
    exceptHoliday?: boolean,
    emsGroupId?: string,
    emsGroupName?: string,
  ) {


    this.label = label;
    this.locationId = locationId;
    this.locationCode = locationCode;
    this.locationLabel = locationLabel;
    this.emsProfileId = emsProfileId;
    this.emsProfileCode = emsProfileCode;
    this.emsProfileLabel = emsProfileLabel;
    this.effectiveDate = effectiveDate;
    this.ineffectiveDate = ineffectiveDate;
    this.startTime = startTime;
    this.endTime = endTime;
    this.monday = monday;
    this.tuesday = tuesday;
    this.wednesday = wednesday;
    this.thursday = thursday;
    this.friday = friday;
    this.saturday = saturday;
    this.sunday = sunday;
    this.exceptHoliday = exceptHoliday;
    this.emsGroupId = emsGroupId;
    this.emsGroupName = emsGroupName;
  }

  id?: string;
  label?: string;
  locationId?: string;
  locationCode?: string;
  locationLabel?: string;
  emsProfileId?: string;
  emsProfileCode?: string;
  emsProfileLabel?: string;
  effectiveDate?: string;
  ineffectiveDate?: string;
  startTime?: string;
  endTime?: string;
  monday?: boolean;
  tuesday?: boolean;
  wednesday?: boolean;
  thursday?: boolean;
  friday?: boolean;
  saturday?: boolean;
  sunday?: boolean;
  exceptHoliday?: boolean;
  color: any;
  rrule?: {
    freq: any;
    byweekday?: any;
  };
  emsGroupId?: string;
  emsGroupName?: string;
}
