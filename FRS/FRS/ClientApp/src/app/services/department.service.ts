import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';




import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Department } from '../models/department.model';
import { Filter, PagedResult } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';

export type DepartmentsChangedOperation = "add" | "delete" | "modify";
export type DepartmentsChangedEventArg = { departments: Department[] | string[], operation: DepartmentsChangedOperation };

@Injectable()
export class DepartmentService {

  public static readonly departmentAddedOperation: DepartmentsChangedOperation = "add";
  public static readonly departmentDeletedOperation: DepartmentsChangedOperation = "delete";
  public static readonly departmentModifiedOperation: DepartmentsChangedOperation = "modify";

  private _departmentsChanged = new Subject<DepartmentsChangedEventArg>();

  private readonly _departmentUrl: string = "/api/department";
  get departmentUrl() { return this.configurations.baseUrl + this._departmentUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }


  private onDepartmentsChanged(departments: Department[] | string[], op: DepartmentsChangedOperation) {
    this._departmentsChanged.next({ departments: departments, operation: op });
  }


  onDepartmentsCountChanged(departments: Department[] | string[]) {
    return this.onDepartmentsChanged(departments, DepartmentService.departmentModifiedOperation);
  }


  getDepartmentsChangedEvent(): Observable<DepartmentsChangedEventArg> {
    return this._departmentsChanged.asObservable();
  }

  getDepartmentById(departmentId: string) {

    return this.commonEndpoint.getById<any>(this.departmentUrl + '/get', departmentId);
  }

  getDepartments(page?: number, pageSize?: number, institutionId?: string, institutionCode?: string) {
    var queryParams = '';
    queryParams += (institutionId ? '?institutionId=' + institutionId : '');
    queryParams += (institutionCode ? '?institutionCode=' + institutionCode : '');

    if (queryParams == '' || page || pageSize) {
      return this.commonEndpoint.getPagedList<Department[]>(this.departmentUrl + '/departments/list', page, pageSize);
    } else {
      return this.commonEndpoint.get<Department[]>(this.departmentUrl + '/departments/list' + queryParams);
    }
  }

  getDepartmentsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.departmentUrl + '/departments/sieve/list', filter);
  }

  updateDepartment(department: Department) {
    if (department.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.departmentUrl, department, department.id).pipe(
        tap(data => this.onDepartmentsChanged([department], DepartmentService.departmentModifiedOperation)));
    }
  }


  newDepartment(department: Department) {
    return this.commonEndpoint.getNewEndpoint<Department>(this.departmentUrl, department).pipe<Department>(
      tap(data => this.onDepartmentsChanged([department], DepartmentService.departmentAddedOperation)));
  }


  deleteDepartment(departmentOrDepartmentId: string | Department): Observable<Department> {

    if (typeof departmentOrDepartmentId === 'number' || departmentOrDepartmentId instanceof Number ||
      typeof departmentOrDepartmentId === 'string' || departmentOrDepartmentId instanceof String) {
      return this.commonEndpoint.getDeleteEndpoint<Department>(this.departmentUrl, <string>departmentOrDepartmentId).pipe<Department>(
        tap(data => this.onDepartmentsChanged([data], DepartmentService.departmentDeletedOperation)));
    }
    else {

      if (departmentOrDepartmentId.id) {
        return this.deleteDepartment(departmentOrDepartmentId.id);
      }
    }
  }
}
