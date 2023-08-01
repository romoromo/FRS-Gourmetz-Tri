import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { AccountEndpoint } from '../account-endpoint.service';
import { AuthService } from '../auth.service';
import { CommonEndpoint } from '../common-endpoint.service';
import { ConfigurationService } from '../configuration.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { StaffType } from 'src/app/models/meal-order/staff-type.model';

@Injectable()
export class UserService {
  private readonly _staffTypeUrl: string = "/api/user/stafftypes";
  get staffTypeUrl() { return this.configurations.baseUrl + this._staffTypeUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  //user type
  getStaffTypeById(staffTypeId: string) {

    return this.commonEndpoint.getById<any>(this.staffTypeUrl + '/get', staffTypeId);
  }

  getStaffTypesByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.staffTypeUrl + '/sieve/list', filter);
  }

  updateStaffType(staffType: StaffType) {
    if (staffType.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.staffTypeUrl, staffType, staffType.id);
    }
  }


  newStaffType(staffType: StaffType) {
    return this.commonEndpoint.getNewEndpoint<StaffType>(this.staffTypeUrl, staffType);
  }


  deleteStaffType(staffTypeOrStaffTypeId: string | StaffType): Observable<StaffType> {
    return this.commonEndpoint.getDeleteEndpoint<StaffType>(this.staffTypeUrl, <string>staffTypeOrStaffTypeId);
  }
}
