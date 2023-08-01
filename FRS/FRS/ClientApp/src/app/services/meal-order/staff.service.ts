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
import { Staff } from 'src/app/models/meal-order/staff.model';
import { CatererInfo } from '../../models/meal-order/caterer-info.model';

@Injectable()
export class StaffService {

  private readonly _catererInfoUrl: string = "/api/staff/catererinfos";
  get catererInfoUrl() { return this.configurations.baseUrl + this._catererInfoUrl; }

  private readonly _staffUrl: string = "/api/staff/staffs";
  get staffUrl() { return this.configurations.baseUrl + this._staffUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }

  //staff level
  getStaffById(staffId: string) {

    return this.commonEndpoint.getById<any>(this.staffUrl + '/get', staffId);
  }

  getStaffsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.staffUrl + '/sieve/list', filter);
  }

  updateStaff(staff: Staff) {
    if (staff.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.staffUrl, staff, staff.id);
    }
  }


  newStaff(staff: Staff) {
    return this.commonEndpoint.getNewEndpoint<Staff>(this.staffUrl, staff);
  }


  deleteStaff(staffOrStaffId: string | Staff): Observable<Staff> {
    return this.commonEndpoint.getDeleteEndpoint<Staff>(this.staffUrl, <string>staffOrStaffId);
  }
}
