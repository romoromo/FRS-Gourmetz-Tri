import { Component, Input } from '@angular/core';

@Component({
  selector: 'video-configuration',
  templateUrl: './video-configuration.component.html'

})
export class VideoConfiguration {
  @Input() configurations: any = {};

  getConfigurations() {
    return this.configurations;
  }
}
