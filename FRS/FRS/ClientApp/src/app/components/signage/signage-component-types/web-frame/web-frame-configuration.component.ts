import { Component, Input } from '@angular/core';

@Component({
  selector: 'web-frame-configuration',
  templateUrl: './web-frame-configuration.component.html'

})
export class WebFrameConfiguration {
  @Input() configurations: any = {};

  getConfigurations() {
    return this.configurations;
  }
}
