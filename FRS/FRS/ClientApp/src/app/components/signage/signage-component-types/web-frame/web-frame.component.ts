import { Component, Input } from '@angular/core'
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'signage-web-frame',
  template: `<iframe frameborder="0" scrolling="no" [src]="sanitizer.bypassSecurityTrustResourceUrl(configurations.configurationsObj.url)" height="100%" width="100%" >
<p>iframes are not supported by your browser.</p>
</iframe>`

})
export class WebFrame {
  @Input() configurations: any;
  @Input() preview: boolean;

  constructor(private sanitizer: DomSanitizer) {

  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
  }

  ngOnChanges(changes: any) {
    console.log("HTML Changes");
  }
}
