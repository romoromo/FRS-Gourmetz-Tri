import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { ApplicationSetting } from '../models/application-setting.model';
import { ConfigurationService } from './configuration.service';

export type ApplicationSettingsChangedOperation = "add" | "delete" | "modify";
export type ApplicationSettingsChangedEventArg = { applicationSettings: ApplicationSetting[] | string[], operation: ApplicationSettingsChangedOperation };

@Injectable()
export class ApplicationSettingService {

  public static readonly applicationSettingAddedOperation: ApplicationSettingsChangedOperation = "add";
  public static readonly applicationSettingDeletedOperation: ApplicationSettingsChangedOperation = "delete";
  public static readonly applicationSettingModifiedOperation: ApplicationSettingsChangedOperation = "modify";

  private _applicationSettingsChanged = new Subject<ApplicationSettingsChangedEventArg>();

  private readonly _applicationSettingUrl: string = "/api/applicationsetting";
  get applicationSettingUrl() { return this.configurations.baseUrl + this._applicationSettingUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onApplicationSettingsChanged(applicationSettings: ApplicationSetting[] | string[], op: ApplicationSettingsChangedOperation) {
    this._applicationSettingsChanged.next({ applicationSettings: applicationSettings, operation: op });
  }


  onApplicationSettingsCountChanged(applicationSettings: ApplicationSetting[] | string[]) {
    return this.onApplicationSettingsChanged(applicationSettings, ApplicationSettingService.applicationSettingModifiedOperation);
  }


  getApplicationSettingsChangedEvent(): Observable<ApplicationSettingsChangedEventArg> {
    return this._applicationSettingsChanged.asObservable();
  }

  getApplicationSettings(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<ApplicationSetting[]>(this.applicationSettingUrl + '/applicationsettings/list', page, pageSize));
  }

  getApplicationSettingByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<ApplicationSetting[]>(this.applicationSettingUrl + '/applicationsettings/list', institutionId));
  }

  getApplicationSettingByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<ApplicationSetting>(this.applicationSettingUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateApplicationSetting(applicationSetting: ApplicationSetting) {
    return this.commonEndpoint.getUpdateEndpoint(this.applicationSettingUrl, applicationSetting, applicationSetting.id).pipe(
      tap(data => this.onApplicationSettingsChanged([applicationSetting], ApplicationSettingService.applicationSettingModifiedOperation)));
  }

  newApplicationSetting(applicationSetting: ApplicationSetting) {
    return this.commonEndpoint.getNewEndpoint<ApplicationSetting>(this.applicationSettingUrl, applicationSetting).pipe<ApplicationSetting>(
      tap(data => this.onApplicationSettingsChanged([applicationSetting], ApplicationSettingService.applicationSettingAddedOperation)));
  }


  deleteApplicationSetting(applicationsettingId): Observable<ApplicationSetting> {
    return this.commonEndpoint.getDeleteEndpoint<ApplicationSetting>(this.applicationSettingUrl, <string>applicationsettingId).pipe<ApplicationSetting>(
      tap(data => this.onApplicationSettingsChanged([data], ApplicationSettingService.applicationSettingDeletedOperation)));
  }
}
