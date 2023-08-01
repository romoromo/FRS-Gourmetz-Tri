import { Component, OnInit, OnDestroy, TemplateRef, ViewChild, Input, AfterViewInit } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { DashboardService } from "../../../services/dashboard.service";
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { Utilities } from "../../../services/utilities";
import { Location } from 'src/app/models/location.model';
import { MatPaginator, MatSort } from '@angular/material';
import { LocationService } from 'src/app/services/location.service';
import { CalendarFilter } from 'src/app/models/reservation.model';
import { endOfDay } from 'date-fns';


@Component({
  selector: 'dashboard-available-room',
  templateUrl: './available-room.component.html',
  styleUrls: ['./available-room.component.css']
})
export class DashboardAvailableRoomComponent implements OnInit, AfterViewInit, OnDestroy {
  locations: Location[] = [];
  loadingIndicator: boolean;
  show_no_results = true;

  dataLoadingConsecutiveFailurs = 0;
  dataLoadingSubscription: any;


  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private locationService: LocationService) {
  }


  ngOnInit() {

    this.initDataLoading();
  }

  ngAfterViewInit() {
    
  }

  ngOnDestroy() {
    if (this.dataLoadingSubscription)
      this.dataLoadingSubscription.unsubscribe();
  }

  initDataLoading() {
    var filter = new CalendarFilter();
    filter.start = Utilities.getCurrentDate(true);
    filter.end = endOfDay(filter.start);
    filter.hasAvailableTimeSlot = true;
    filter.institutionId = this.accountService.currentUser.institutionId;
    this.dataLoadingSubscription = this.locationService.getAvailableLocations(filter)
      .subscribe(results => {

        this.locations = results;
        if (this.locations && this.locations.length > 0) {
          this.show_no_results = false;
        }
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve locations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }
  


  get canManageNotifications() {
    return this.accountService.userHasPermission(Permission.manageRolesPermission); //Todo: Consider creating separate permission for notifications
  }

}
