import { Component, Input, EventEmitter, Output, SimpleChanges, OnChanges, OnInit, ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'text-configuration',
  templateUrl: './text-configuration.component.html'

})
export class TextConfiguration {
  @Input() configurations: any = {};
  @Input() hidePosSize: boolean;
  @Output() configurationsChange = new EventEmitter<boolean>();

  ngOnInit() {
    this.setConfiguration(this.configurations);
  }

  setConfiguration(configurations?) {
    this.configurations = configurations || {};
    this.configurationsChange.emit(this.configurations);

    this.configurations.vAlign = this.configurations.vAlign || 'middle';
  }

  fonts = [
    'American Typewriter',
    'Andalé Mono',
    'Arial Black',
    'Arial',
    'Avenir Next',
    'Bradley Hand',
    'Brush Script MT',
    'Comic Sans MS',
    'Courier',
    'Didot',
    'Franklin Gothic',
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

  valigns = [
    'top',
    'middle',
    'bottom',
  ]

  getConfigurations() {
    return this.configurations;
  }
}
