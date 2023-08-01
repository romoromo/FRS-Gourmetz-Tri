import { Component, OnInit, OnDestroy, TemplateRef, ViewChild, Input, AfterViewInit } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { DashboardService } from "../../../services/dashboard.service";
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { Utilities } from "../../../services/utilities";
import { MatPaginator, MatSort } from '@angular/material';
import { PIBDevice } from 'src/app/models/department.model';
import { DeviceFilter } from 'src/app/models/device.model';


@Component({
  selector: 'pib-device-list',
  templateUrl: './pib-device-list.component.html',
  styleUrls: ['./pib-device-list.component.css']
})
export class PIBDeviceStatusListComponent implements OnInit, AfterViewInit, OnDestroy {
  devices: PIBDevice[] = [];
  loadingIndicator: boolean;
  show_no_results = true;
  displayedColumns: string[];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataLoadingConsecutiveFailurs = 0;
  dataLoadingSubscription: any;


  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private dashboardService: DashboardService) {
  }


  ngOnInit() {

    this.displayedColumns = ['device_code', 'location_code', 'status_display'];
    this.initDataLoading();
  }

  ngAfterViewInit() {
    
  }

  ngOnDestroy() {
    if (this.dataLoadingSubscription)
      this.dataLoadingSubscription.unsubscribe();
  }

  initDataLoading() {
    var filter = new DeviceFilter();
    filter.institutionId = parseInt(this.accountService.currentUser.institutionId);
    filter.isActive = true;
    filter.isApproved = false;
    this.dataLoadingSubscription = this.dashboardService.getPIBDeviceStatusList(1, 5, filter)
      .subscribe(devices => {
        this.loadingIndicator = false;
        this.dataLoadingConsecutiveFailurs = 0;

        this.devices = devices;
        if (devices && devices.length > 0) {
          this.show_no_results = false;
        }
      },
        error => {
          this.loadingIndicator = false;

          this.alertService.showMessage("Load Error", "Loading devices from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);

          if (this.dataLoadingConsecutiveFailurs++ < 5)
            setTimeout(() => this.initDataLoading(), 5000);
          else
            this.alertService.showStickyMessage("Load Error", "Loading devices from the server failed!", MessageSeverity.error);

        });
  }
}
