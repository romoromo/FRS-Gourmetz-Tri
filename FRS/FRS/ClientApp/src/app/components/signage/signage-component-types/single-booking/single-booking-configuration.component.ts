import { Component, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'single-booking-configuration',
  templateUrl: './single-booking-configuration.component.html'

})
export class SingleBookingConfiguration {
  @Input() configurations: any;

  ngOnInit() {
    this.configurations = this.configurations || {};
    this.configurations.contactNo = this.configurations.contactNo || {};
    this.configurations.description = this.configurations.description || {};
    this.configurations.location = this.configurations.location || {};
    this.configurations.time = this.configurations.time || {};
    this.configurations.meetingPurpose = this.configurations.meetingPurpose || {};
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

}
