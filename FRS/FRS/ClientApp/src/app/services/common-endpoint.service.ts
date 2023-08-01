import { Injectable, Injector } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { EndpointFactory } from './endpoint-factory.service';
import { ConfigurationService } from './configuration.service';
import { CalendarFilter } from '../models/reservation.model';
import { PIBTemplate } from '../models/department.model';
import { HttpParamsOptions } from '@angular/common/http/src/params';
import { LocationTreeFilter } from '../models/location.model';


@Injectable()
export class CommonEndpoint extends EndpointFactory {

  private readonly _facilitiesUrl: string = "/api/facility";
  private readonly _facilityByFacilityNameUrl: string = "/api/facility/name";
  private readonly _facilityTypesUrl: string = "/api/facilitytype";
  private readonly _facilityTypeByFacilityTypeNameUrl: string = "/api/facilitytype/name";
  get facilitiesUrl() { return this.configurations.baseUrl + this._facilitiesUrl; }
  get facilityByFacilityNameUrl() { return this.configurations.baseUrl + this._facilityByFacilityNameUrl; }
  get facilityTypesUrl() { return this.configurations.baseUrl + this._facilityTypesUrl; }
  get facilityTypeByFacilityTypeNameUrl() { return this.configurations.baseUrl + this._facilityTypeByFacilityTypeNameUrl; }

  constructor(http: HttpClient, configurations: ConfigurationService, injector: Injector) {

    super(http, configurations, injector);
  }

  //Common methods
  getByInstitutionId<T>(url: string, id?: string): Observable<T> {
    //let endpointUrl = this.configurations.baseUrl + url + "?institutionId=" + id;

    let endpointUrl = url + "?institutionId=" + id;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getByInstitutionId(url, id));
      }));
  }

  getById<T>(url: string, id?: string): Observable<T> {
    //let endpointUrl = this.configurations.baseUrl + url + "/id/" + id;
    let endpointUrl = url + "/id/" + id;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getById(url, id));
      }));
  }

  getSieve<T>(url: string, filter: any): Observable<T> {
    //let endpointUrl = isTrueUrl ? url : this.configurations.baseUrl + url;
    const headers = this.getRequestHeaders();

    const httpParams: HttpParamsOptions = { fromObject: filter } as HttpParamsOptions;

    const options = { params: new HttpParams(httpParams), headers: headers.headers };

    return this.http.get<T>(url, options).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getSieve(url, filter));
      }));
  }

  get<T>(url: string, isPost?: boolean, obj?: any, isParams?: boolean, isTrueUrl?: boolean): Observable<T> {
    //let endpointUrl = isTrueUrl ? url : this.configurations.baseUrl + url;
    let endpointUrl = url;

    if (isParams) {
      let params = new HttpParams()
        .append('templateBody', obj);

      let requestBody = params.toString();

      return this.http.post<T>(endpointUrl, requestBody, this.getRequestHeaders()).pipe<T>(
        catchError(error => {
          return this.handleError(error, () => this.getRefreshLoginEndpoint());
        }));
    } else {
      if (isPost) {
        return this.http.post<T>(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
          catchError(error => {
            return this.handleError(error, () => this.get(url, isPost, obj));
          }));
      }
      else {
        return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
          catchError(error => {
            return this.handleError(error, () => this.getPagedList(url));
          }));
      }
    }
  }

  getFile<T>(url: string, obj?: any): Observable<T> {
    let endpointUrl = url;

    const headers = this.getRequestHeaders();

    const httpParams: HttpParamsOptions = { fromObject: obj } as HttpParamsOptions;

    const options = { params: new HttpParams(httpParams), headers: headers.headers, responseType: 'blob' as 'json' };

    return this.http.post<T>(endpointUrl, JSON.stringify(obj), options).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getFile(url, obj));
      }));
  }

  getFileget<T>(url: string): Observable<T> {
    let endpointUrl = url;

    const headers = this.getRequestHeaders();

    const options = { headers: headers.headers, responseType: 'blob' as 'json' };

    return this.http.get<T>(endpointUrl, options).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getFileget(url));
      }));
  }

  getPagedList<T>(url: string, page?: number, pageSize?: number, isPost?: boolean, obj?: any): Observable<T> {
    //url = this.configurations.baseUrl + url;
    let endpointUrl = page && pageSize ? `${url}/${page}/${pageSize}` : url;

    if (isPost) {
      return this.http.post<T>(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
        catchError(error => {
          return this.handleError(error, () => this.getPagedList(url, page, pageSize, isPost, obj));
        }));
    }
    else {
      return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
        catchError(error => {
          return this.handleError(error, () => this.getPagedList(url, page, pageSize));
        }));
    }
  }

  getNewEndpoint<T>(url: string, obj: any): Observable<T> {
    //url = this.configurations.baseUrl + url;
    return this.http.post<T>(url, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getNewEndpoint(url, obj));
      }));
  }

  getUpdateEndpoint<T>(url: string, obj: any, id: string): Observable<T> {
    //url = this.configurations.baseUrl + url;
    let endpointUrl = `${url}/update/${id}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateEndpoint(url, obj, id));
      }));
  }

  getUpdateStoreEndpoint<T>(url: string, obj: any, id: string): Observable<T> {
    //url = this.configurations.baseUrl + url;
    let endpointUrl = `${url}/updatestores/${id}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateStoreEndpoint(url, obj, id));
      }));
  }

  getDeleteEndpoint<T>(url: string, id: string, query?: string): Observable<T> {
    //url = this.configurations.baseUrl + url;
    let endpointUrl = query ? `${url}/delete/${id}?${query}` : `${url}/delete/${id}`;

    return this.http.delete<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getDeleteEndpoint(url, id, query));
      }));
  }

  //Facilities methods
  getFacilitiesEndpoint<T>(page?: number, pageSize?: number, institutionId?: string): Observable<T> {
    let endpointUrl = page && pageSize ? `${this.facilitiesUrl}/facilities/list/${page}/${pageSize}` : this.facilitiesUrl + "/facilities/list?institutionId=" + institutionId;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getFacilitiesEndpoint(page, pageSize));
      }));
  }

  getNewFacilityEndpoint<T>(facilityObject: any): Observable<T> {

    return this.http.post<T>(this.facilitiesUrl, JSON.stringify(facilityObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getNewFacilityEndpoint(facilityObject));
      }));
  }

  getUpdateFacilityEndpoint<T>(facilityObject: any, facilityId: string): Observable<T> {
    let endpointUrl = `${this.facilitiesUrl}/update/${facilityId}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(facilityObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateFacilityEndpoint(facilityObject, facilityId));
      }));
  }

  getDeleteFacilityEndpoint<T>(facilityId: string): Observable<T> {
    let endpointUrl = `${this.facilitiesUrl}/delete/${facilityId}`;

    return this.http.delete<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getDeleteFacilityEndpoint(facilityId));
      }));
  }

  getFacilityByFacilityNameEndpoint<T>(facilityName: string): Observable<T> {
    let endpointUrl = `${this.facilityByFacilityNameUrl}/${facilityName}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getFacilityByFacilityNameEndpoint(facilityName));
      }));
  }

  //Facility Type
  getFacilityTypesEndpoint<T>(page?: number, pageSize?: number, institutionId?: string): Observable<T> {
    let endpointUrl = page && pageSize ? `${this.facilityTypesUrl}/facilitytypes/list/${page}/${pageSize}` : this.facilityTypesUrl + '/facilitytypes/list?institutionId=' + institutionId;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getFacilityTypesEndpoint(page, pageSize, institutionId));
      }));
  }

  getNewFacilityTypeEndpoint<T>(facilityTypeObject: any): Observable<T> {

    return this.http.post<T>(this.facilityTypesUrl, JSON.stringify(facilityTypeObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getNewFacilityTypeEndpoint(facilityTypeObject));
      }));
  }

  getUpdateFacilityTypeEndpoint<T>(facilityTypeObject: any, facilityTypeId: string): Observable<T> {
    let endpointUrl = `${this.facilityTypesUrl}/update/${facilityTypeId}`;

    return this.http.put<T>(endpointUrl, JSON.stringify(facilityTypeObject), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getUpdateFacilityTypeEndpoint(facilityTypeObject, facilityTypeId));
      }));
  }

  getDeleteFacilityTypeEndpoint<T>(facilityTypeId: string): Observable<T> {
    let endpointUrl = `${this.facilityTypesUrl}/delete/${facilityTypeId}`;

    return this.http.delete<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getDeleteFacilityTypeEndpoint(facilityTypeId));
      }));
  }

  getFacilityTypeByFacilityTypeNameEndpoint<T>(facilityTypeName: string): Observable<T> {
    let endpointUrl = `${this.facilityTypeByFacilityTypeNameUrl}/${facilityTypeName}`;

    return this.http.get<T>(endpointUrl, this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getFacilityTypeByFacilityTypeNameEndpoint(facilityTypeName));
      }));
  }

  //device
  getDeviceApprovalEndpoint<T>(url: string, obj: any): Observable<T> {
    //url = this.configurations.baseUrl + url;
    return this.http.post<T>(url, JSON.stringify(obj), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getDeviceApprovalEndpoint(url, JSON.stringify(obj)));
      }));
  }

  //Reservation methods
  getReservations<T>(url, filter?: CalendarFilter): Observable<T> {
    return this.http.post<T>(url, JSON.stringify(filter), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getReservations(url, filter));
      }));
  }

  //Location methods
  getLocationTree<T>(url, filter?: LocationTreeFilter): Observable<T> {
    return this.http.post<T>(url, JSON.stringify(filter), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getLocationTree(url, filter));
      }));
  }

  postTemplateToDevice<T>(url: string, template?: PIBTemplate): Observable<T> {
    let header = new HttpHeaders({ 'Content-Type': 'multipart/form-data', });
    //header.append('Access-Control-Allow-Origin', 'http://localhost:56767');
    //header.append('Access-Control-Allow-Credentials', 'true');

    let params = new HttpParams()
    params.append('name', template.name);
    params.append('eImage', template.imgUrl);
    //const formData = new FormData();
    //formData.append(imgName, uri);
    return this.http.post<T>(url, params, this.getRequestHeaders('multipart/form-data')).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.postTemplateToDevice(url, template));
      }));
  }

  sendQueueSimulator<T>(url, data: any): Observable<T> {
    return this.http.post<T>(url, JSON.stringify(data), this.getRequestHeaders()).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.getLocationTree(url, data));
      }));
  }

  importFile<T>(url, data: any): Observable<T> {
    return this.http.post<T>(url, data, { reportProgress: true, observe: 'events' }).pipe<T>(
      catchError(error => {
        return this.handleError(error, () => this.importFile(url, data));
      }));
  }
}
