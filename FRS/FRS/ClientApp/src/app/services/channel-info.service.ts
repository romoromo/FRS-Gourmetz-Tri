import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, forkJoin } from 'rxjs';



import { CommonEndpoint } from './common-endpoint.service';
import { AccountEndpoint } from './account-endpoint.service';
import { AuthService } from './auth.service';
import { LocationService } from './location.service';
import { ConfigurationService } from './configuration.service';
import { CommonFilter, Filter, PagedResult } from '../models/sieve-filter.model';
import { ChannelInfo } from '../models/channel-info.model';

@Injectable()
export class ChannelInfoService {


  private readonly _channelInfoUrl: string = "/api/channelinfo/channelinfos";
  get channelInfoUrl() { return this.configurations.baseUrl + this._channelInfoUrl; }

  constructor(private router: Router, private http: HttpClient, private authService: AuthService,
    private accountEndpoint: AccountEndpoint, private commonEndpoint: CommonEndpoint, private locationService: LocationService, protected configurations: ConfigurationService) {

  }

  getDataByUrl(url: string, isPost?: boolean, obj?: any) {
    return this.commonEndpoint.get<any>(url, isPost, obj, false, true);
  }


  //device type

  getChannelInfoById(channelInfoId: string) {

    return this.commonEndpoint.getById<any>(this.channelInfoUrl + '/get', channelInfoId);
  }

  getChannelInfosByFilter(filter: Filter) {
    return this.commonEndpoint.getSieve<PagedResult>(this.channelInfoUrl + '/sieve/list', filter);
  }

  updateChannelInfo(channelInfo: ChannelInfo) {
    if (channelInfo.id) {
      return this.commonEndpoint.getUpdateEndpoint(this.channelInfoUrl, channelInfo, channelInfo.id);
    }
  }


  newChannelInfo(channelInfo: ChannelInfo) {
    return this.commonEndpoint.getNewEndpoint<ChannelInfo>(this.channelInfoUrl, channelInfo);
  }


  deleteChannelInfo(channelInfoOrChannelInfoId: string | ChannelInfo): Observable<ChannelInfo> {
    return this.commonEndpoint.getDeleteEndpoint<ChannelInfo>(this.channelInfoUrl, <string>channelInfoOrChannelInfoId);
  }
}
