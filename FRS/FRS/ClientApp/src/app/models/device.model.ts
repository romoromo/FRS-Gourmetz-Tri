import { SignageDashboardFilter } from './dashboard.model';
import { Permission } from './permission.model';
import { Filter } from './sieve-filter.model';

export class Device {

  constructor(id?: string, code?: string, ipAddress?: string, macAddress?: string, serialNumber?: string, isApproved?: boolean,
    startDate?: Date, endDate?: Date, facilityId?: string, facilityName?: string, status?: string, locationId?: string, locationName?: string) {

    this.code = code;
    this.ipAddress = ipAddress;
    this.macAddress = macAddress;
    this.serialNumber = serialNumber;
    this.isApproved = isApproved;
    this.startDate = startDate;
    this.endDate = endDate;
    this.facilityId = facilityId;
    this.facilityName = facilityName;
    this.status = status
    this.locationId = locationId;
    this.locationName = locationName;
  }

  public id: string;
  public code: string;
  public ipAddress: string;
  public macAddress: string;
  public serialNumber: string;
  public isApproved: boolean;
  public startDate: Date;
  public endDate: Date;
  public facilityId: string;
  public facilityName: string;
  public locationId: string;
  public locationName: string;
  public status: string;
  public institutionId: string;
  public publicationId: string;
  public module_path: string;
  public module_path_value: string;
  public emergencyMessage1: string;
  public emergencyMessage2: string;
  public emergencyMessage3: string;
  public emergencyMessage4: string;
  public moduleId: string;
  public device_label: string;
  public module_uri: string;
  public communicatorMenuId: string;
  public pibTemplateId: string;
  public isScreenOn: boolean;
  public ems_id: string;
  public media_id: string;
  public external_location_id: string;
  public external_ems_id: string;
  public external_media_id: string;
  public device_status: string;

  public institutionName: string;
  public status_display: string;
  public scheme: string;
  public port: string;
  public connection_status_display: string;
  public deviceTypeId: string;
}

export class DeviceFilter {
  public isActive?: boolean;
  public isApproved: boolean;
  public institutionId?: number;
}

export class DevicePushMessage {
  public deviceIds?: string[];
  public filter: SignageDashboardFilter;
  public emergencyMessage1: string;
  public emergencyMessage2: string;
  public emergencyMessage3: string;
  public emergencyMessage4: string;
}
