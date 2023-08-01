import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { Device, DevicePushMessage } from '../models/device.model';
import { Location } from '../models/location.model';
import { Institution } from '../models/institution.model';
import { LocationService } from './location.service';
import { ConfigurationService } from './configuration.service';
import { CommonFilter, Filter, PagedResult } from '../models/sieve-filter.model';
import { SignageDashboardFilter } from '../models/dashboard.model';
import { DeviceType } from '../models/device-type.model';

export type DevicesChangedOperation = "add" | "delete" | "modify";
export type DevicesChangedEventArg = { devices: Device[] | string[], operation: DevicesChangedOperation };

export type LocationsChangedOperation = "add" | "delete" | "modify";
export type LocationsChangedEventArg = { locations: Location[] | string[], operation: LocationsChangedOperation };

@Injectable()
export class DeviceService {

  public static readonly deviceAddedOperation: DevicesChangedOperation = "add";
  public static readonly deviceDeletedOperation: DevicesChangedOperation = "delete";
  public static readonly deviceModifiedOperation: DevicesChangedOperation = "modify";
  //public static readonly deviceApprovedOperation: DevicesChangedOperation = "approved";

  private _devicesChanged = new Subject<DevicesChangedEventArg>();


  private readonly _deviceUrl: string = "/api/devicemanager";
  get deviceUrl() { return this.configurations.baseUrl + this._deviceUrl; }

  private readonly _institutionUrl: string = "/api/institution";
  get institutionUrl() { return this.configurations.baseUrl + this._institutionUrl; }

  private readonly _deviceTypeUrl: string = "/api/devicetype/devicetypes";
  get deviceTypeUrl() { return this.configurations.baseUrl + this._deviceTypeUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, private locationService: LocationService, protected configurations: ConfigurationService) {

  }


  private onDevicesChanged(devices: Device[] | string[], op: DevicesChangedOperation) {
    this._devicesChanged.next({ devices: devices, operation: op });
  }


  onDevicesUserCountChanged(devices: Device[] | string[]) {
    return this.onDevicesChanged(devices, DeviceService.deviceModifiedOperation);
  }

  getDeviceByIdentifier(macAddress?: string) {

    return this.commonEndpoint.get<any>(this.deviceUrl + '/get/' + macAddress);
  }

  //getDeviceById(deviceId?: string, macAddress?: string) {

  //  return this.commonEndpoint.get<any>(this.deviceUrl + '/get/device?deviceId=' + deviceId + '&mac_address=' + macAddress);
  //}

  getDevicesChangedEvent(): Observable<DevicesChangedEventArg> {
    return this._devicesChanged.asObservable();
  }

  getDevicesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.deviceUrl + '/devices/sieve/list', filter);
  }

  getDevicesLocationsInstitutionsByFilter(filter: CommonFilter) {

    return forkJoin(
      this.commonEndpoint.getSieve<PagedResult>(this.deviceUrl + '/devices/sieve/list', filter),
      this.locationService.getLocations(null, null, filter.institutionId),
      this.commonEndpoint.getPagedList<Institution[]>(this.institutionUrl + '/institutions/list'));
  }

  getDevicesLocationsInstitutions(page?: number, pageSize?: number, institutionId?: string) {

    return forkJoin(
      this.commonEndpoint.getPagedList<Device[]>(this.deviceUrl + '/devices/list?institutionId=' + institutionId, page, pageSize),
      this.locationService.getLocations(null, null, institutionId),
      this.commonEndpoint.getPagedList<Institution[]>(this.institutionUrl + '/institutions/list'));
  }

  getDeviceById(deviceId?: string, macAddress?: string, useIP?: boolean, url?: string) {

    if (url) {
      console.log(url + '?deviceId=' + deviceId + '&mac_address=' + macAddress + '&useIP=' + useIP);
      return this.commonEndpoint.get<any>(url + '?deviceId=' + deviceId + '&mac_address=' + macAddress + '&useIP=' + useIP, false, null, false, true);
    }

    return this.commonEndpoint.get<any>(this.deviceUrl + '/get/device?deviceId=' + deviceId + '&mac_address=' + macAddress + '&useIP=' + useIP);
  }

  updateDevice(device: Device) {
    return this.commonEndpoint.getUpdateEndpoint(this.deviceUrl, device, device.id).pipe(
      tap(data => this.onDevicesChanged([device], DeviceService.deviceModifiedOperation)));
  }


  newDevice(device: Device) {
    return this.commonEndpoint.getNewEndpoint<Device>(this.deviceUrl, device).pipe<Device>(
      tap(data => this.onDevicesChanged([device], DeviceService.deviceAddedOperation)));
  }


  deleteDevice(deviceOrDeviceId: string | Device): Observable<Device> {

    if (typeof deviceOrDeviceId === 'number' || deviceOrDeviceId instanceof Number ||
      typeof deviceOrDeviceId === 'string' || deviceOrDeviceId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Device>(this.deviceUrl, <string>deviceOrDeviceId).pipe<Device>(
        tap(data => this.onDevicesChanged([data], DeviceService.deviceDeletedOperation)));
    }
    else {

      if (deviceOrDeviceId.id) {
        return this.deleteDevice(deviceOrDeviceId.id);
      }
    }
  }

  pushMessages(data: DevicePushMessage) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/pushmessages', true, data);
  }

  bulkReboot(filter: SignageDashboardFilter) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/bulkreboot', true, filter);
  }

  bulkRefresh(filter: SignageDashboardFilter) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/bulkRefresh', true, filter);
  }

  //approveDevice(device: Device) {
  //  return this.commonEndpoint.getNewEndpoint<Device>(this.deviceUrl + '/approve', device).pipe<Device>(
  //    tap(data => this.onDevicesChanged([device], DeviceService.deviceApprovedOperation)));
  //}

  getDataByUrl(url: string, isPost?: boolean, obj?: any) {
    return this.commonEndpoint.get<any>(url, isPost, obj, false, true);
  }

  getServerTime() {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/get/servertime');
  }

  reboot(id?: string) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/reboot/' + id);
  }

  channelup(id?: string) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/channelup/' + id);
  }

  changechannel(id?: string, channelId?: string) {
    return this.commonEndpoint.get<any>(this.deviceUrl + `/device/changechannel/${id}/${channelId}`);
  }

  channeldown(id?: string) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/channeldown/' + id);
  }

  screenon(id?: string) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/screenon/' + id);
  }

  screenoff(id?: string) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/screenoff/' + id);
  }

  off(id?: string) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/off/' + id);
  }

  on(id?: string) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/on/' + id);
  }

  refresh(id?: string) {
    return this.commonEndpoint.get<any>(this.deviceUrl + '/device/refresh/' + id);
  }

  //device type

  getDeviceTypeById(deviceTypeId: string) {

    return this.commonEndpoint.getById<any>(this.deviceTypeUrl + '/get', deviceTypeId);
  }

  getDeviceTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.deviceTypeUrl + '/sieve/list', filter);
  }

  updateDeviceType(deviceType: DeviceType) {
    if (deviceType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.deviceTypeUrl, deviceType, deviceType.id);
    }
  }


  newDeviceType(deviceType: DeviceType) {
    return this.commonEndpoint.getNewEndpoint<DeviceType>(this.deviceTypeUrl, deviceType);
  }


  deleteDeviceType(deviceTypeOrDeviceTypeId: string | DeviceType): Observable<DeviceType> {
    return this.commonEndpoint.getDeleteEndpoint<DeviceType>(this.deviceTypeUrl, <string>deviceTypeOrDeviceTypeId);
  }
}
