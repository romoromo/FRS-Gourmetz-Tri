import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { PIBTemplate, PIBTemplateLocation, PIBDevice } from '../models/department.model';
import { ConfigurationService } from './configuration.service';

export type PIBChangedOperation = "add" | "delete" | "modify";
export type PIBChangedEventArg = { pibs: PIBTemplate[] | string[], operation: PIBChangedOperation };
export type PIBDeviceChangedEventArg = { pibs: PIBDevice[] | string[], operation: PIBChangedOperation };

@Injectable()
export class PIBService {

  public static readonly pibAddedOperation: PIBChangedOperation = "add";
  public static readonly pibDeletedOperation: PIBChangedOperation = "delete";
  public static readonly pibModifiedOperation: PIBChangedOperation = "modify";

  private _pibsChanged = new Subject<PIBChangedEventArg>();
  private _pibsDeviceChanged = new Subject<PIBDeviceChangedEventArg>();

  private readonly _pibUrl: string = "/api/pib";
  get pibUrl() { return this.configurations.baseUrl + this._pibUrl; }

  private readonly _templateUrl: string = "/api/pib";
  get templateUrl() { return this.configurations.baseUrl + this._templateUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onPIBChanged(pibs: PIBTemplate[] | string[], op: PIBChangedOperation) {
    this._pibsChanged.next({ pibs: pibs, operation: op });
  }
  getPIBChangedEvent(): Observable<PIBChangedEventArg> {
    return this._pibsChanged.asObservable();
  }

  private onPIBDeviceChanged(pibs: PIBDevice[] | string[], op: PIBChangedOperation) {
    this._pibsDeviceChanged.next({ pibs: pibs, operation: op });
  }

  getPIBDeviceChangedEvent(): Observable<PIBDeviceChangedEventArg> {
    return this._pibsDeviceChanged.asObservable();
  }
  

  //mapTemplate(data: PIBTemplate) {
  //  var queryParams = '';
  //  queryParams += (data ? '?template=' + data : '');
  //  return this.commonEndpoint.get<any>(this.pibUrl + '/map/template', true, data.template_body, true);
  //}

  previewFromImage(data: PIBTemplate) {
    return this.commonEndpoint.getNewEndpoint<any>(this.pibUrl + '/preview/image', data);
  }

  mapTemplate(data: PIBTemplate) {
    return this.commonEndpoint.getNewEndpoint<any>(this.pibUrl + '/map/template', data);
  }

  postTemplateToDevice(url: string, template: PIBTemplate) {
    return this.commonEndpoint.getNewEndpoint<any>(this.pibUrl + '/map/posttodevice', template);
  }

  postTemplateToDevice2(url: string, template: PIBTemplate) {
    return this.commonEndpoint.postTemplateToDevice<any>(url, template);
  }

  getTemplateById(templateId: string) {
    
    return this.commonEndpoint.get<any>(this.templateUrl + '/get/template?templateId=' + templateId);
  }

  getTemplateByMacAddress(macAddress: string) {
    return this.commonEndpoint.get<any>(this.templateUrl + '/get/template?mac=' + macAddress);
  }

  getTemplates(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<PIBTemplate[]>(this.templateUrl + '/templates/list', page, pageSize));
  }

  getRestrictionTypes(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<PIBTemplate[]>(this.templateUrl + '/get/restrictiontypes', page, pageSize));
  }

  syncLocations() {
    return this.commonEndpoint.getPagedList<any>(this.templateUrl + '/get/piblocations/sync');
  }

  getLocations(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<PIBTemplateLocation[]>(this.templateUrl + '/get/piblocations', page, pageSize));
  }

  updateTemplate(template: PIBTemplate) {
    if (template.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.templateUrl, template, template.id).pipe(
        tap(data => this.onPIBChanged([template], PIBService.pibModifiedOperation)));
    }
  }


  newTemplate(template: PIBTemplate) {
    return this.commonEndpoint.getNewEndpoint<PIBTemplate>(this.templateUrl, template).pipe<PIBTemplate>(
      tap(data => this.onPIBChanged([template], PIBService.pibAddedOperation)));
  }


  deleteTemplate(templateOrId: string | PIBTemplate): Observable<PIBTemplate> {

    if (typeof templateOrId === 'number' || templateOrId instanceof Number ||
      typeof templateOrId === 'string' || templateOrId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<PIBTemplate>(this.templateUrl, <string>templateOrId).pipe<PIBTemplate>(
        tap(data => this.onPIBChanged([data], PIBService.pibDeletedOperation)));
    }
    else {

      if (templateOrId.id) {
        return this.deleteTemplate(templateOrId.id);
      }
    }
  }

  //pib devices
  heartbeat(macAddress?: string) {

    return this.commonEndpoint.get<any>(this.templateUrl + '/heartbeat?mac_address=' + macAddress);
  }

  getDeviceById(deviceId?: string, macAddress?: string) {

    return this.commonEndpoint.get<any>(this.templateUrl + '/get/device?deviceId=' + deviceId + '&mac_address=' + macAddress);
  }

  getDevices(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<PIBDevice[]>(this.templateUrl + '/devices/list', page, pageSize),
      //this.commonEndpoint.getPagedList<PIBTemplateLocation[]>(this.templateUrl + '/get/piblocations', page, pageSize)
    );
  }

  updatePIBDevice(device: PIBDevice) {
    if (device.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.templateUrl + '/device', device, device.id).pipe(
        tap(data => this.onPIBDeviceChanged([device], PIBService.pibModifiedOperation)));
    }
  }


  newPIBDevice(device: PIBDevice) {
    return this.commonEndpoint.getNewEndpoint<PIBDevice>(this.templateUrl + '/device', device).pipe<PIBDevice>(
      tap(data => this.onPIBDeviceChanged([device], PIBService.pibAddedOperation)));
  }


  deletePIBDevice(deviceOrId: string | PIBDevice): Observable<PIBDevice> {

    if (typeof deviceOrId === 'number' || deviceOrId instanceof Number ||
      typeof deviceOrId === 'string' || deviceOrId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<PIBDevice>(this.templateUrl + '/device', <string>deviceOrId).pipe<PIBDevice>(
        tap(data => this.onPIBDeviceChanged([data], PIBService.pibDeletedOperation)));
    }
    else {

      if (deviceOrId.id) {
        return this.deletePIBDevice(deviceOrId.id);
      }
    }
  }
}
