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
  selector: 'single-booking',
  template: `
<div style="height: 100%;width: 100%;background-size: contain;background-repeat: no-repeat;background-position: center center;" [style.background-image]="'url(' + (configurations.configurationsObj.pictureMode && medias ? medias[current] : '') + ')'">
<div *ngIf="!configurations.configurationsObj.pictureMode" style="height: 100%;width: 100%;">
<text-display [preview]="preview" [textStyle]="configurations.configurationsObj.contactNo" [previewText]="'Contact'" [actualText]="reservation?.meta?.reservation?.createdByName"></text-display>

<text-display [preview]="preview" [textStyle]="configurations.configurationsObj.description" [previewText]="'Descriptions'" [actualText]="reservation?.meta?.reservation?.shortDescription"></text-display>

<text-display [preview]="preview" [textStyle]="configurations.configurationsObj.location" [previewText]="'Location'" [actualText]="reservation?.meta?.location?.name"></text-display>

<text-display [preview]="preview" [textStyle]="configurations.configurationsObj.time" [previewText]="'Time'" [actualText]="time"></text-display>

<text-display [preview]="preview" [textStyle]="configurations.configurationsObj.meetingPurpose" [previewText]="'Meeting Purpose'" [actualText]="reservation?.meta?.reservation?.longDescription"></text-display>
</div>
</div>`

})
export class SingleBooking {
  @Input() configurations: any;
  @Input() preview: boolean;

  mac_address: any;

  private signalRCoreconnection: signalR.HubConnection;
  device: any;
  locationColorTheme: any;
  locationName: any;

  backgroundImage;

  medias;

  current = 0;

  filter: CalendarFilter;
  calendarEvents: CalendarEvent[];
    institutionId: any;
    reservation: any;
    time: any;

  constructor(private changeDetectorRef: ChangeDetectorRef, private authService: AuthService, private route: ActivatedRoute, private deviceService: DeviceService, private reservationService: ReservationService, private configurationService: ConfigurationService) {

  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    console.log('employee')

    if (!this.preview) {
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

      this.startPlaylist();
    }
  }

  start() {
    this.getSingleBooking();
    
    setTimeout(() => {
      this.start();
    }, 5 * 60 * 1000);
  }

  getSingleBooking() {
    if (!this.device || !this.device.locationId) return;

    this.filter = new CalendarFilter();
    this.filter.institutionId = this.institutionId;
    this.filter.locationIds = [this.device.locationId];
    this.filter.isBooking = true;
    this.filter.start = new Date();
    this.filter.start = new Date(this.filter.start.setHours(0, 0, 0, 0));
    this.filter.startTime = new Date(this.filter.start.setHours(0, 0, 0, 0));
    this.filter.endTime = new Date(this.filter.start.setHours(23, 59, 59, 0));

    this.reservationService.getCalendarEvents(this.filter).subscribe(events => {
      this.calendarEvents = [];
      var currentTime = Utilities.getCurrentDate(true);
      events.forEach((event, index, events) => {
        let startObj = new Date(event.start);
        let endObj = new Date(event.end);

        if (event.meta != null && event.meta.reservation != null && startObj <= currentTime
          && endObj >= currentTime) {
          this.calendarEvents.push(event);
        }
      });

      this.reservation = null;
      this.time = null;
      if (this.calendarEvents[0]) {
        this.reservation = this.calendarEvents[0];

        const locale = 'en-SG';
        this.time = this.reservation.meta && this.reservation.meta.reservation && this.reservation.meta.reservation.displayTime ? this.reservation.meta.reservation.displayTime : formatDate(this.reservation.start, 'hh:mm aa', locale) + " - " + formatDate(this.reservation.end, 'hh:mm aa', locale);

        if (this.reservation.meta && this.reservation.meta.reservation && this.reservation.meta.reservation.reservationPictures && this.reservation.meta.reservation.reservationPictures[0]) {
          this.medias = this.reservation.meta.reservation.reservationPictures.map(u => this.removeBackSlash(u));

          console.log("BACK", this.backgroundImage);
        }

        if (this.configurations.configurationsObj.screenon) this.on();
      } else {
        if (this.configurations.configurationsObj.screenoff) this.off();
      }
    });
  }

  removeBackSlash(url) {
    return url.replaceAll('\\', '/')
  }

  startPlaylist() {
    

    let duration = this.configurations.configurationsObj.pictureModeInterval || 5;


    if (this.medias && this.medias.length > 0) {
      this.current++;

      if (this.current >= this.medias.length) this.current = 0;
    }

    setTimeout(() => {
      this.startPlaylist();
    }, duration * 1000);
  }

  off() {
    if (!this.device) return;

    this.deviceService.off(this.device.id).subscribe(results => { });;
  }

  on() {
    if (!this.device) return;

    this.deviceService.on(this.device.id).subscribe(results => { });;
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

            this.getSingleBooking();
          }
        });
    }
  }
}
