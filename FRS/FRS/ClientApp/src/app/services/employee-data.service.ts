import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { EmployeeData } from '../models/employee-data.model';
import { ConfigurationService } from './configuration.service';

export type EmployeeDatasChangedOperation = "add" | "delete" | "modify";
export type EmployeeDatasChangedEventArg = { employeeDatas: EmployeeData[] | string[], operation: EmployeeDatasChangedOperation };

@Injectable()
export class EmployeeDataService {

  public static readonly employeeDataAddedOperation: EmployeeDatasChangedOperation = "add";
  public static readonly employeeDataDeletedOperation: EmployeeDatasChangedOperation = "delete";
  public static readonly employeeDataModifiedOperation: EmployeeDatasChangedOperation = "modify";

  private _employeeDatasChanged = new Subject<EmployeeDatasChangedEventArg>();

  private readonly _employeeDataUrl: string = "/api/employeedata";
  get employeeDataUrl() { return this.configurations.baseUrl + this._employeeDataUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onEmployeeDatasChanged(employeeDatas: EmployeeData[] | string[], op: EmployeeDatasChangedOperation) {
    this._employeeDatasChanged.next({ employeeDatas: employeeDatas, operation: op });
  }


  onEmployeeDatasCountChanged(employeeDatas: EmployeeData[] | string[]) {
    return this.onEmployeeDatasChanged(employeeDatas, EmployeeDataService.employeeDataModifiedOperation);
  }


  getEmployeeDatasChangedEvent(): Observable<EmployeeDatasChangedEventArg> {
    return this._employeeDatasChanged.asObservable();
  }

  getEmployeeDatas(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<EmployeeData[]>(this.employeeDataUrl + '/list', page, pageSize));
  }

  getEmployeeDataByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<EmployeeData[]>(this.employeeDataUrl + '/list', institutionId));
  }

  getEmployeeDataByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<EmployeeData>(this.employeeDataUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  getCurrentEmployeeDataByLocation(locationId: any) {

    return this.commonEndpoint.get<EmployeeData[]>(this.employeeDataUrl + '/getcurrentemployeebylocation?locationId=' + locationId);
  }

  updateEmployeeData(employeeData: EmployeeData) {
    return this.commonEndpoint.getUpdateEndpoint(this.employeeDataUrl, employeeData, employeeData.id).pipe(
      tap(data => this.onEmployeeDatasChanged([employeeData], EmployeeDataService.employeeDataModifiedOperation)));
  }

  newEmployeeData(employeeData: EmployeeData) {
    return this.commonEndpoint.getNewEndpoint<EmployeeData>(this.employeeDataUrl, employeeData).pipe<EmployeeData>(
      tap(data => this.onEmployeeDatasChanged([employeeData], EmployeeDataService.employeeDataAddedOperation)));
  }


  deleteEmployeeData(id): Observable<EmployeeData> {
    return this.commonEndpoint.getDeleteEndpoint<EmployeeData>(this.employeeDataUrl, <string>id).pipe<EmployeeData>(
      tap(data => this.onEmployeeDatasChanged([data], EmployeeDataService.employeeDataDeletedOperation)));
  }
}
