import { Component, Input } from '@angular/core'
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'free-html',
  template: `<div [innerHTML]="getSafeHTML(configurations.configurationsObj.html)"></div>`

})
export class FreeHTML {
  @Input() configurations: any;
    safeHtml: any;

  constructor(private sanitizer: DomSanitizer) {

  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    if (!this.configurations.configurationsObj.html) this.configurations.configurationsObj.html = "";

    this.safeHtml = this.sanitizer.bypassSecurityTrustHtml(this.configurations.configurationsObj.html);
  }

  getSafeHTML(html) {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }

  ngOnChanges(changes: any) {
    console.log("HTML Changes");
  }
}
