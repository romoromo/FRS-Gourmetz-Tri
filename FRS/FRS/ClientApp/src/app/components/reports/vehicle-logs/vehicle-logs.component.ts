import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { AccountService } from '../../../services/account.service';
import { ReservationService } from '../../../services/reservation.service';
import { Utilities } from '../../../services/utilities';
import { Reservation, CalendarFilter, VehicleLogFilter } from '../../../models/reservation.model';
import { Permission } from '../../../models/permission.model';
import { Facility } from '../../../models/facility.model';

import { Location } from '../../../models/location.model';
import { fadeInOut } from '../../../services/animations';

import { DateTimeOnlyPipe } from '../../../pipes/datetime.pipe';
import { DateOnlyPipe } from '../../../pipes/datetime.pipe';
import { CalendarEvent } from 'calendar-utils';
import { ReservationTime } from 'src/app/models/reservationtime.model';
import { MatDialog, MatDatepickerInputEvent } from '@angular/material';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { take } from 'rxjs/operators';
import { VMSVehicleLog } from 'src/app/models/uservehicle.model';
import { Subscription } from 'rxjs';
@Component({
  selector: 'reports-vehicle-log-list',
  templateUrl: './vehicle-logs.component.html',
  styleUrls: ['./vehicle-logs.component.css'],
  animations: [fadeInOut]
})
export class ReportsVehicleLogListComponent implements OnInit, AfterViewInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  columns: any[] = [];
  rows: VMSVehicleLog[] = [];
  rowsCache: VMSVehicleLog[] = [];
  filter: VehicleLogFilter;
  loadingIndicator: boolean;

  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private reservationService: ReservationService, private configurationService: ConfigurationService, public dialog: MatDialog) {
  }


  ngOnInit() {
    this.filter = new CalendarFilter();
    if (!this.filter.start) {
      this.filter.start = new Date();
    }

    if (!this.filter.end) {
      this.filter.end = new Date();
    }

    this.loadingIndicator = false;
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'personName', name: gT('userVehicles.management.Owner') },
      { prop: 'cardType', name: gT('userVehicles.management.CardType') },
      { prop: 'vehicle', name: gT('userVehicles.management.PlateNumber') },
      { prop: 'type', name: 'Type' },
      { prop: 'timestamp', name: 'Timestamp', pipe: new DateTimeOnlyPipe('en-SG') },
      { prop: 'bookingDescription', name: gT('userVehicles.management.BookingDescription') }
      //{ prop: 'entryDate', name: 'Entry Date', pipe: new DateTimeOnlyPipe('en-SG') },
      //{ prop: 'exitDate', name: 'Exit Date', pipe: new DateTimeOnlyPipe('en-SG') }
    ];

    //this.loadData();
  }

  ngAfterViewInit() {

  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  loadData(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.filter.start = new Date(event.value);
    }

    if (type == 'end') {
      this.filter.end = new Date(event.value);
    }

    this.loadingIndicator = true;
    this.filter.start = new Date(this.filter.start.setHours(0, 0, 0, 0));
    this.filter.end = new Date(this.filter.end.setHours(23, 59, 59, 0));

    this.subscription.add(this.reservationService.getVehicleLogs(this.filter)
      .subscribe(results => {

        let logs = results;

        logs.forEach((log, index, logs) => {
          (<any>log).index = index + 1;
        });

        this.rowsCache = [...logs];
        this.rows = logs;
        this.loadingIndicator = false;
        
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve logs from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
      }));
  }


  onSearchChanged(value: string) {
    console.log(this.rowsCache);
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.bookingDescription, r.personName, r.cardType, r.vehicle));
  }

  get canManageReservations() {
    return this.accountService.userHasPermission(Permission.viewReportsPermission)
  }

}
