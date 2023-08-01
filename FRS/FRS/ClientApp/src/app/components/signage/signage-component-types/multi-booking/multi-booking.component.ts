import { Component, Input, ChangeDetectorRef } from '@angular/core'
import { ActivatedRoute } from '@angular/router';
import { CalendarEvent } from 'angular-calendar';
import { CalendarFilter } from '../../../../models/reservation.model';
import { SignageComponent } from '../../../../models/signage-component.model';
import { AccountService } from '../../../../services/account.service';
import { AuthService } from '../../../../services/auth.service';
import { ConfigurationService } from '../../../../services/configuration.service';
import { DeviceService } from '../../../../services/device.service';
import { ReservationService } from '../../../../services/reservation.service';
import { Utilities } from '../../../../services/utilities';
import { formatDate } from '@angular/common';

@Component({
  selector: 'multi-booking',
  templateUrl: './multi-booking.component.html'

})
export class MultiBooking {
  @Input() configurations: any;
  @Input() preview: boolean;

  mac_address: any;

  private signalRCoreconnection: signalR.HubConnection;
  device: any;
  locationColorTheme: any;
  locationName: any;

  filter: CalendarFilter;
  calendarEvents: CalendarEvent[] = [];
  institutionId: any;
  time: any;

  page: number = 0;

  constructor(private changeDetectorRef: ChangeDetectorRef, private authService: AuthService, private route: ActivatedRoute, private deviceService: DeviceService, private reservationService: ReservationService, private configurationService: ConfigurationService) {

  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    console.log('employee')

    if (!this.preview) {
      this.configurations.configurationsObj.noOfRows = this.configurations.configurationsObj.noOfRows || 0;
      this.configurations.configurationsObj.noOfColumns = this.configurations.configurationsObj.noOfColumns || 0;

      this.route.params.subscribe(queryParams => {
        this.mac_address = queryParams["mac_address"];
      });

      if (!this.mac_address) {
        this.route.queryParams.subscribe(queryParams => {
          this.mac_address = queryParams["mac_address"];
        });
      }

      this.getDeviceInfo();

      this.start();

      this.switchPage(true);
    }
  }

  switchPage(first?) {
    if (!first) {
      this.page++;

      if (this.page * this.configurations.configurationsObj.noOfRows * this.configurations.configurationsObj.noOfColumns >= this.calendarEvents.length) this.page = 0;
    }

    setTimeout(() => {
      this.switchPage();
    }, (this.configurations.configurationsObj.interval || 5) * 1000);
  }


  start() {
    this.getMultiBooking();

    setTimeout(() => {
      this.start();
    }, 5 * 60 * 1000);
  }

  getMultiBooking() {
    if (!this.device || !this.device.locationId) return;

    this.filter = new CalendarFilter();
    this.filter.institutionId = this.institutionId;
    this.filter.locationIds = this.configurations.configurationsObj.locationIds && this.configurations.configurationsObj.locationIds[0] ? this.configurations.configurationsObj.locationIds : null;
    this.filter.isBooking = true;
    this.filter.start = new Date();
    this.filter.start = new Date(this.filter.start.setHours(0, 0, 0, 0));
    this.filter.startTime = new Date(this.filter.start.setHours(0, 0, 0, 0));
    this.filter.endTime = new Date(this.filter.start.setHours(23, 59, 59, 0));

    const locale = 'en-SG';

    this.reservationService.getCalendarEvents(this.filter).subscribe(events => {
      this.calendarEvents = [];
      var currentTime = Utilities.getCurrentDate(true);
      events.forEach((event, index, events) => {
        event.start = new Date(event.start);
        event.end = new Date(event.end);
        if (event.meta != null && event.meta.reservation != null && event.end >= currentTime) {

          let tempEvent: any = event;
          tempEvent.time = event.meta.reservation.displayTime || formatDate(event.start, 'hh:mm aa', locale) + " - " + formatDate(event.end, 'hh:mm aa', locale);
          this.calendarEvents.push(tempEvent);
        }
      });

      if (this.calendarEvents[0]) {
        if (this.configurations.configurationsObj.screenon) this.on();
      } else {
        if (this.configurations.configurationsObj.screenoff) this.off();
      }
    });
  }

  off() {
    if (!this.device) return;

    this.deviceService.off(this.device.id);
  }

  on() {
    if (!this.device) return;

    this.deviceService.on(this.device.id);
  }

  getDeviceInfo() {
    if (this.mac_address) {
      this.deviceService.getDeviceById(null, this.mac_address, true)
        .subscribe(results => {

          this.device = results.data;
          if (this.device) {
            this.locationColorTheme = this.device.locationColorTheme;
            this.locationName = this.device.locationName;
            this.institutionId = this.device.institutionId;

            this.getMultiBooking();
          }
        });
    }
  }

  getArrays(num) {
    if (num && num > 0)
      return new Array(num);
  }
}
