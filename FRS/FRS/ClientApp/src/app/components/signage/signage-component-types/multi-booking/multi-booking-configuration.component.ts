import { Component, Input, EventEmitter, Output } from '@angular/core';
import { Utilities } from 'src/app/services/utilities';
import { Location } from 'src/app/models/location.model';
import { LocType } from 'src/app/models/enums';
import { CalendarFilter } from '../../../../models/reservation.model';
import { AccountService } from '../../../../services/account.service';
import { ReservationService } from '../../../../services/reservation.service';

@Component({
  selector: 'multi-booking-configuration',
  templateUrl: './multi-booking-configuration.component.html'

})
export class MultiBookingConfiguration {
  @Input() configurations: any;

  buildingList: Location[];
  locationList: Location[];
  locationListCopy: Location[];

  filter: CalendarFilter = new CalendarFilter();

  constructor(private accountService: AccountService, private reservationService: ReservationService) {

  }

  ngOnInit() {
    this.configurations = this.configurations || {};
    this.configurations.contactNo = this.configurations.contactNo || {};
    this.configurations.description = this.configurations.description || {};
    this.configurations.location = this.configurations.location || {};
    this.configurations.time = this.configurations.time || {};
    this.configurations.meetingPurpose = this.configurations.meetingPurpose || {};

    this.getFilters();
    this.initFilter();
  }

  fonts = [
    'American Typewriter',
    'Andalé Mono',
    'Arial Black',
    'Arial',
    'Bradley Hand',
    'Brush Script MT',
    'Comic Sans MS',
    'Courier',
    'Didot',
    'Georgia',
    'Impact',
    'Lucida Console',
    'Luminari',
    'Monaco',
    'Tahoma',
    'Times New Roman',
    'Trebuchet MS',
    'Verdana',
  ];

  aligns = [
    'left',
    'center',
    'right',
    'justify'
  ]

  getConfigurations() {
    return this.configurations;
  }

  show(obj) {
    return JSON.stringify(obj);
  }

  getRoomsByBuildingId(id, value) {
    let unsortedLocations = this.locationList.filter(r => r.parentLocationId == id && Utilities.searchArray(value, false, r.name, r.description))
    unsortedLocations.sort((a, b) => (a.name < b.name ? -1 : 1));

    return unsortedLocations;
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
        this.getLocationsByBuild();
      },
        error => {
        });
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
