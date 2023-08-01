import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { CommonEndpoint } from './common-endpoint.service';
import { AuthService } from './auth.service';
import { Filter, PagedResult, ServiceContractFilter } from '../models/sieve-filter.model';
import { ConfigurationService } from './configuration.service';
import { ServiceContract } from '../models/service-contract.model';

export type ServiceContractsChangedOperation = "add" | "delete" | "modify";
export type ServiceContractsChangedEventArg = { serviceContracts: ServiceContract[] | string[], operation: ServiceContractsChangedOperation };

@Injectable()
export class ServiceContractService {

  public static readonly serviceContractAddedOperation: ServiceContractsChangedOperation = "add";
  public static readonly serviceContractDeletedOperation: ServiceContractsChangedOperation = "delete";
  public static readonly serviceContractModifiedOperation: ServiceContractsChangedOperation = "modify";

  private _serviceContractsChanged = new Subject<ServiceContractsChangedEventArg>();

  private readonly _serviceContractUrl: string = "/api/servicecontract";
  get serviceContractUrl() { return this.configurations.baseUrl + this._serviceContractUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private commonEndpoint: CommonEndpoint, protected configurations: ConfigurationService) {

  }
  
  getServiceContractById(serviceContractId: string) {

    return this.commonEndpoint.getById<any>(this.serviceContractUrl + '/get', serviceContractId);
  }

  getServiceContractsByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.serviceContractUrl + '/servicecontracts/sieve/list', filter);
  }

  updateServiceContract(serviceContract: ServiceContract) {
    if (serviceContract.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.serviceContractUrl, serviceContract, serviceContract.id);
    }
  }


  newServiceContract(serviceContract: ServiceContract) {
    return this.commonEndpoint.getNewEndpoint<ServiceContract>(this.serviceContractUrl, serviceContract);
  }


  deleteServiceContract(serviceContractOrServiceContractId: string | ServiceContract): Observable<ServiceContract> {
    return this.commonEndpoint.getDeleteEndpoint<ServiceContract>(this.serviceContractUrl, <string>serviceContractOrServiceContractId);
  }

  downloadServiceContractReport(serviceContractFilter: ServiceContractFilter) {
    return this.commonEndpoint.getFile<any>(this.serviceContractUrl + '/exportreport', serviceContractFilter);
  }

  
}
