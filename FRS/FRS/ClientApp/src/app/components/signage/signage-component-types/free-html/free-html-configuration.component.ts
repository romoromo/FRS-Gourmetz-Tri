import { Component, Input } from '@angular/core';

@Component({
  selector: 'free-html-configuration',
  templateUrl: './free-html-configuration.component.html'

})
export class FreeHTMLConfiguration {
  @Input() configurations: any = {};

  getConfigurations() {
    return this.configurations;
  }
}
