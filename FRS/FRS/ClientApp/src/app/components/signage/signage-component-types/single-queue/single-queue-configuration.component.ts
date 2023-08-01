import { Component, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'single-queue-configuration',
  templateUrl: './single-queue-configuration.component.html'

})
export class SingleQueueConfiguration {
  @Input() configurations: any;

  ngOnInit() {
    this.configurations = this.configurations || {};
    this.configurations.texts = this.configurations.texts || {};
    this.configurations.texts.queueNo = this.configurations.texts.queueNo || { label: 'Queue No' };
    this.configurations.texts.roomInfo = this.configurations.texts.roomInfo || { label: 'Room Info' };
    this.configurations.texts.servingInfo = this.configurations.texts.servingInfo || { label: 'Serving Info' };
    this.configurations.texts.doctorInfo = this.configurations.texts.doctorInfo || { label: 'Doctor Info' };
    this.configurations.texts.assistantInfo = this.configurations.texts.assistantInfo || { label: 'Assistant Info' };
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
