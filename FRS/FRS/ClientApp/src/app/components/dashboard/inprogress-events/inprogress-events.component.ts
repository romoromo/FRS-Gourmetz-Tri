import { Component, OnInit, OnDestroy, TemplateRef, ViewChild, Input } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { DashboardService } from "../../../services/dashboard.service";
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { Utilities } from "../../../services/utilities";
import { CalendarFilter, Reservation } from 'src/app/models/reservation.model';


@Component({
  selector: 'inprogress-events',
  templateUrl: './inprogress-events.component.html',
  styleUrls: ['./inprogress-events.component.css']
})
export class InProgressEventsComponent implements OnInit, OnDestroy {
  events: Reservation[] = [];
  loadingIndicator: boolean;
  show_no_events = true;

  dataLoadingConsecutiveFailurs = 0;
  dataLoadingSubscription: any;


  @Input()
  isViewOnly: boolean;

  @Input()
  verticalScrollbar: boolean = false;


  @ViewChild('statusHeaderTemplate')
  statusHeaderTemplate: TemplateRef<any>;

  @ViewChild('statusTemplate')
  statusTemplate: TemplateRef<any>;

  @ViewChild('dateTemplate')
  dateTemplate: TemplateRef<any>;

  @ViewChild('contentHeaderTemplate')
  contentHeaderTemplate: TemplateRef<any>;

  @ViewChild('contenBodytTemplate')
  contenBodytTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService, private dashboardService: DashboardService) {
  }


  ngOnInit() {
    this.initDataLoading();
  }


  ngOnDestroy() {
    if (this.dataLoadingSubscription)
      this.dataLoadingSubscription.unsubscribe();
  }



  initDataLoading() {

    var filter = new CalendarFilter();
    filter.start = Utilities.getCurrentDate(true);
    filter.isForAttendance = true;
    filter.currentUserId = parseInt(this.accountService.currentUser.id);
    filter.institutionId = this.accountService.currentUser.institutionId;
    this.dataLoadingSubscription = this.dashboardService.getInProgressEvents(5, filter)
      .subscribe(events => {
        this.loadingIndicator = false;
        this.dataLoadingConsecutiveFailurs = 0;

        this.events = events;
        if (events && events.length > 0) {
          this.show_no_events = false;
        }
      },
        error => {
          this.loadingIndicator = false;

          this.alertService.showMessage("Load Error", "Loading upcoming events from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);

          if (this.dataLoadingConsecutiveFailurs++ < 5)
            setTimeout(() => this.initDataLoading(), 5000);
          else
            this.alertService.showStickyMessage("Load Error", "Loading upcoming events from the server failed!", MessageSeverity.error);

        });
  }
  


  get canManageNotifications() {
    return this.accountService.userHasPermission(Permission.manageRolesPermission); //Todo: Consider creating separate permission for notifications
  }

}
