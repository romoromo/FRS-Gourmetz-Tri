import { Component, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'free-text-configuration',
  templateUrl: './free-text-configuration.component.html'

})
export class FreeTextConfiguration {
  @Input() configurations: any;

  ngOnInit() {
    this.configurations = this.configurations || {};
    this.configurations.name = this.configurations.name || {};
    this.configurations.group = this.configurations.group || {};
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
