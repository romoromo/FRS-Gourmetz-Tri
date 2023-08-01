import { Component, OnInit, OnDestroy, TemplateRef, ViewChild, Input, AfterViewInit, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { MatCalendar, MatDatepickerInputEvent } from '@angular/material';
import { AccountService } from 'src/app/services/account.service';
import { AlertService, DialogType, MessageSeverity } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { AuthService } from 'src/app/services/auth.service';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { Location } from 'src/app/models/location.model';
import { CalendarFilter } from 'src/app/models/reservation.model';
import { Facility } from 'src/app/models/facility.model';
import { ReservationService } from 'src/app/services/reservation.service';
import { LocType } from 'src/app/models/enums';
import { Utilities } from 'src/app/services/utilities';
import { FormControl, Validators } from '@angular/forms';
import { animate, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'calendar-landing-page',
  templateUrl: './landing-page.component.html',
  animations: [
    trigger('slideInOut', [
      transition(':enter', [
        style({ transform: 'translateX(-100%)' }),
        animate('200ms ease-in', style({ transform: 'translateX(0%)' }))
      ]),
      transition(':leave', [
        animate('200ms ease-in', style({ transform: 'translateX(-100%)' }))
      ])
    ])
  ],
  styleUrls: ['./landing-page.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class CalendarLandingPageComponent implements OnInit, AfterViewInit {
  loadingIndicator: boolean;
  show_no_results = true;
  buildingList: Location[];
  facilityList: Facility[];
  locationList: Location[];
  locationListCopy: Location[];
  fcLocations = new FormControl('', [Validators.required]);
  fcFacilities = new FormControl();
  filter: CalendarFilter = new CalendarFilter();
  showcalendar: boolean = false;
  selectedDate: any;
  locSearchKeyword: string;
  @ViewChild('calendar')
  _datePicker: MatCalendar<Date>

  @Output()
  dateChange: EventEmitter<MatDatepickerInputEvent<any>>;

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService,
    private authService: AuthService, private configurations: ConfigurationService, private reservationService: ReservationService) {

  }

  ngOnInit() {
    this.initFilter();
    this.getFilters();
  }

  ngAfterViewInit() {

    if (this._datePicker) {
      this._datePicker.selectedChange.subscribe(x => {
        if (typeof x != typeof (undefined)) {
          this.filter.viewDate = x;
          this.filter.start = this.filter.end = x;
        }
      });
    }
  }

  selectDate(ev) {
    this.filter.viewDate = ev;
    this.filter.start = this.filter.end = ev;

  }

  initFilter() {
    this.filter = new CalendarFilter();
    this.filter.start = new Date();
  }

  getFilters() {
    if (this.filter == null) {
      this.filter = new CalendarFilter();
    }

    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.filter.isBooking = true;
    this.reservationService.getCalendarFilters(this.filter)
      .subscribe(results => {

        let facilities = results[0];
        let locations = results[1];

        //let parentLocations = new Map();

        //locations.forEach((location, index, locations) => {
        //  (<any>location).index = index + 1;
        //  if (location.parentLocation && location.parentLocation.locationTypeName == LocType.Building) {
        //    if (!parentLocations.has(location.parentLocationId)) {
        //      parentLocations.set(location.parentLocationId, location.parentLocation);
        //    }
        //  }

        //});


        this.locationList = locations;
        this.locationListCopy = locations;
        this.facilityList = facilities;
        this.getLocationsByBuild();
        //this.buildingList = Array.from(parentLocations.values()); //this.locationList.filter(f => f.locationTypeName == LocType.Building);
        this.facilityList = facilities;
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve locations/facilities from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  getRoomsByBuildingId(id, value) {
    let unsortedLocations = this.locationList.filter(r => r.parentLocationId == id && Utilities.searchArray(value, false, r.name, r.description))
    unsortedLocations.sort((a, b) => (a.name < b.name ? -1 : 1));

    return unsortedLocations;
  }

  apply() {
    if (this.filter.locationIds && this.filter.locationIds.length > 0) {
      this.showcalendar = !this.showcalendar;
    } else {

    }
    
  }

  clear() {
    this.filter = new CalendarFilter();
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.locSearchKeyword = '';
    this.locationSearchOnKeyUp(this.locSearchKeyword);
    this.apply();
  }

  bookingComplete(ev) {
    //this.clear();
    this.filter = new CalendarFilter();
    this.filter.institutionId = this.accountService.currentUser.institutionId;
    this.filter.start = ev;
    this.selectedDate = ev;
    this.showcalendar = false;
    this.locSearchKeyword = '';
    //this.buildingList = [...this.buildingList];
    this.locationSearchOnKeyUp(this.locSearchKeyword);
    this.apply();
    
  }

  getLocationsByBuild() {
    let parentLocations = new Map();

    this.locationListCopy.forEach((location, index, locations) => {
      (<any>location).index = index + 1;
      if (location.parentLocation && location.parentLocation.locationTypeName == LocType.Building) {
        if (!parentLocations.has(location.parentLocationId)) {
          parentLocations.set(location.parentLocationId, location.parentLocation);
        }
      }

    });

    this.buildingList = Array.from(parentLocations.values()); 
  }

  locationSearchOnKeyUp(value) {
    this.locationListCopy = this.search(value);
    this.getLocationsByBuild();
  }

  search(value: string) {
    return this.locationList.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }
}
