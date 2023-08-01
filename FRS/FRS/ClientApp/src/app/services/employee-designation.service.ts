import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { EmployeeDesignation } from '../models/employee-designation.model';
import { ConfigurationService } from './configuration.service';

export type EmployeeDesignationsChangedOperation = "add" | "delete" | "modify";
export type EmployeeDesignationsChangedEventArg = { employeeDesignations: EmployeeDesignation[] | string[], operation: EmployeeDesignationsChangedOperation };

@Injectable()
export class EmployeeDesignationService {

  public static readonly employeeDesignationAddedOperation: EmployeeDesignationsChangedOperation = "add";
  public static readonly employeeDesignationDeletedOperation: EmployeeDesignationsChangedOperation = "delete";
  public static readonly employeeDesignationModifiedOperation: EmployeeDesignationsChangedOperation = "modify";

  private _employeeDesignationsChanged = new Subject<EmployeeDesignationsChangedEventArg>();

  private readonly _employeeDesignationUrl: string = "/api/employeedesignation";
  get employeeDesignationUrl() { return this.configurations.baseUrl + this._employeeDesignationUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, private configurations: ConfigurationService) {

  }


  private onEmployeeDesignationsChanged(employeeDesignations: EmployeeDesignation[] | string[], op: EmployeeDesignationsChangedOperation) {
    this._employeeDesignationsChanged.next({ employeeDesignations: employeeDesignations, operation: op });
  }


  onEmployeeDesignationsCountChanged(employeeDesignations: EmployeeDesignation[] | string[]) {
    return this.onEmployeeDesignationsChanged(employeeDesignations, EmployeeDesignationService.employeeDesignationModifiedOperation);
  }


  getEmployeeDesignationsChangedEvent(): Observable<EmployeeDesignationsChangedEventArg> {
    return this._employeeDesignationsChanged.asObservable();
  }

  getEmployeeDesignations(page?: number, pageSize?: number) {

    return forkJoin(
      this.commonEndpoint.getPagedList<EmployeeDesignation[]>(this.employeeDesignationUrl + '/list', page, pageSize));
  }

  getEmployeeDesignationByInstitutionId(institutionId: string) {

    return forkJoin(
      this.commonEndpoint.getByInstitutionId<EmployeeDesignation[]>(this.employeeDesignationUrl + '/list', institutionId));
  }

  getEmployeeDesignationByKey(institutionId: string, key: string) {

    return this.commonEndpoint.get<EmployeeDesignation>(this.employeeDesignationUrl + '/get?institutionId=' + institutionId + '&key=' + key);
  }

  updateEmployeeDesignation(employeeDesignation: EmployeeDesignation) {
    return this.commonEndpoint.getUpdateEndpoint(this.employeeDesignationUrl, employeeDesignation, employeeDesignation.id).pipe(
      tap(data => this.onEmployeeDesignationsChanged([employeeDesignation], EmployeeDesignationService.employeeDesignationModifiedOperation)));
  }

  newEmployeeDesignation(employeeDesignation: EmployeeDesignation) {
    return this.commonEndpoint.getNewEndpoint<EmployeeDesignation>(this.employeeDesignationUrl, employeeDesignation).pipe<EmployeeDesignation>(
      tap(data => this.onEmployeeDesignationsChanged([employeeDesignation], EmployeeDesignationService.employeeDesignationAddedOperation)));
  }


  deleteEmployeeDesignation(id): Observable<EmployeeDesignation> {
    return this.commonEndpoint.getDeleteEndpoint<EmployeeDesignation>(this.employeeDesignationUrl, <string>id).pipe<EmployeeDesignation>(
      tap(data => this.onEmployeeDesignationsChanged([data], EmployeeDesignationService.employeeDesignationDeletedOperation)));
  }
}
