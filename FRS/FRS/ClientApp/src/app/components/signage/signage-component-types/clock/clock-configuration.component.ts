import { Component, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'clock-configuration',
  templateUrl: './clock-configuration.component.html'

})
export class ClockConfiguration {
  @Input() configurations: any = {};

  ngOnInit() {
    this.configurations = this.configurations || {};
    this.configurations.text1 = this.configurations.text1 || {};
    this.configurations.text2 = this.configurations.text2 || {};
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

}
