import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { EpaperTemplate, EpaperDevice } from '../models/epaper.model';
import { ConfigurationService } from './configuration.service';

export type EpaperChangedOperation = "add" | "delete" | "modify";
export type EpaperChangedEventArg = { epapers: EpaperTemplate[] | string[], operation: EpaperChangedOperation };
export type EpaperDeviceChangedEventArg = { epapers: EpaperDevice[] | string[], operation: EpaperChangedOperation };

@Injectable()
export class EpaperService {

  public static readonly epaperAddedOperation: EpaperChangedOperation = "add";
  public static readonly epaperDeletedOperation: EpaperChangedOperation = "delete";
  public static readonly epaperModifiedOperation: EpaperChangedOperation = "modify";

  private _epapersChanged = new Subject<EpaperChangedEventArg>();
  private _epapersDeviceChanged = new Subject<EpaperDeviceChangedEventArg>();

  private readonly _epaperUrl: string = "/api/epaper";
  get epaperUrl() { return this.configurations.baseUrl + this._epaperUrl; }

  private readonly _templateUrl: string = "/api/epaper";
  get templateUrl() { return this.configurations.baseUrl + this._templateUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onEpaperChanged(epapers: EpaperTemplate[] | string[], op: EpaperChangedOperation) {
    this._epapersChanged.next({ epapers: epapers, operation: op });
  }
  getEpaperChangedEvent(): Observable<EpaperChangedEventArg> {
    return this._epapersChanged.asObservable();
  }

  private onEpaperDeviceChanged(epapers: EpaperDevice[] | string[], op: EpaperChangedOperation) {
    this._epapersDeviceChanged.next({ epapers: epapers, operation: op });
  }

  getEpaperDeviceChangedEvent(): Observable<EpaperDeviceChangedEventArg> {
    return this._epapersDeviceChanged.asObservable();
  }
  

  //mapTemplate(data: EpaperTemplate) {
  //  var queryParams = '';
  //  queryParams += (data ? '?template=' + data : '');
  //  return this.commonEndpoint.get<any>(this.epaperUrl + '/map/template', true, data.template_body, true);
  //}

  previewFromImage(data: EpaperTemplate) {
    return this.commonEndpoint.getNewEndpoint<any>(this.epaperUrl + '/preview/image', data);
  }

  mapTemplate(data: EpaperTemplate) {
    return this.commonEndpoint.getNewEndpoint<any>(this.epaperUrl + '/map/template', data);
  }

  postTemplateToDevice(url: string, template: EpaperTemplate) {
    return this.commonEndpoint.getNewEndpoint<any>(this.epaperUrl + '/map/posttodevice', template);
  }

  postTemplateToDevice2(url: string, template: EpaperTemplate) {
    return null; //this.commonEndpoint.postTemplateToDevice<any>(url, template);
  }

  getTemplateById(templateId: string) {
    
    return this.commonEndpoint.get<any>(this.templateUrl + '/get/template?templateId=' + templateId);
  }

  getTemplateByMacAddress(macAddress: string) {
    return this.commonEndpoint.get<any>(this.templateUrl + '/get/template?mac=' + macAddress);
  }

  getTemplates(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<EpaperTemplate[]>(this.templateUrl + '/templates/list', page, pageSize));
  }

  getRestrictionTypes(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<EpaperTemplate[]>(this.templateUrl + '/get/restrictiontypes', page, pageSize));
  }

  syncLocations() {
    return this.commonEndpoint.getPagedList<any>(this.templateUrl + '/get/epaperlocations/sync');
  }

  //getLocations(page?: number, pageSize?: number) {

  //  return forkJoin(
  //    this.commonEndpoint.getPagedList<Location[]>(this.templateUrl + '/get/epaperlocations', page, pageSize));
  //}

  updateTemplate(template: EpaperTemplate) {
    if (template.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.templateUrl, template, template.id).pipe(
        tap(data => this.onEpaperChanged([template], EpaperService.epaperModifiedOperation)));
    }
  }


  newTemplate(template: EpaperTemplate) {
    return this.commonEndpoint.getNewEndpoint<EpaperTemplate>(this.templateUrl, template).pipe<EpaperTemplate>(
      tap(data => this.onEpaperChanged([template], EpaperService.epaperAddedOperation)));
  }


  deleteTemplate(templateOrId: string | EpaperTemplate): Observable<EpaperTemplate> {

    if (typeof templateOrId === 'number' || templateOrId instanceof Number ||
      typeof templateOrId === 'string' || templateOrId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<EpaperTemplate>(this.templateUrl, <string>templateOrId).pipe<EpaperTemplate>(
        tap(data => this.onEpaperChanged([data], EpaperService.epaperDeletedOperation)));
    }
    else {

      if (templateOrId.id) {
        return this.deleteTemplate(templateOrId.id);
      }
    }
  }

  //epaper devices
  getDeviceById(deviceId: string) {

    return this.commonEndpoint.getById<any>(this.templateUrl + '/get/device', deviceId);
  }

  getDevices(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<EpaperDevice[]>(this.templateUrl + '/devices/list', page, pageSize),
      //this.commonEndpoint.getPagedList<EpaperTemplateLocation[]>(this.templateUrl + '/get/epaperlocations', page, pageSize)
    );
  }

  updateEpaperDevice(device: EpaperDevice) {
    if (device.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.templateUrl + '/device', device, device.id).pipe(
        tap(data => this.onEpaperDeviceChanged([device], EpaperService.epaperModifiedOperation)));
    }
  }


  newEpaperDevice(device: EpaperDevice) {
    return this.commonEndpoint.getNewEndpoint<EpaperDevice>(this.templateUrl + '/device', device).pipe<EpaperDevice>(
      tap(data => this.onEpaperDeviceChanged([device], EpaperService.epaperAddedOperation)));
  }


  deleteEpaperDevice(deviceOrId: string | EpaperDevice): Observable<EpaperDevice> {

    if (typeof deviceOrId === 'number' || deviceOrId instanceof Number ||
      typeof deviceOrId === 'string' || deviceOrId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<EpaperDevice>(this.templateUrl + '/device', <string>deviceOrId).pipe<EpaperDevice>(
        tap(data => this.onEpaperDeviceChanged([data], EpaperService.epaperDeletedOperation)));
    }
    else {

      if (deviceOrId.id) {
        return this.deleteEpaperDevice(deviceOrId.id);
      }
    }
  }
}
