import { Component, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'emergency-message-configuration',
  templateUrl: './emergency-message-configuration.component.html'

})
export class EmergencyMessageConfiguration {
  @Input() configurations: any = {};


  ngOnInit() {
    this.configurations = this.configurations || {};
    this.configurations.textStyle = this.configurations.textStyle || {};
    this.configurations.emergencyStyle = this.configurations.emergencyStyle || {};
  }

  directions = [
    'left',
    'right',
    'top',
    'bottom',
  ];

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

}
