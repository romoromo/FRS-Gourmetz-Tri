import { Component, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'employee-name-configuration',
  templateUrl: './employee-name-configuration.component.html'

})
export class EmployeeNameConfiguration {
  @Input() configurations: any;

  ngOnInit() {
    this.configurations = this.configurations || {};
    this.configurations.name = this.configurations.name || {};
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
