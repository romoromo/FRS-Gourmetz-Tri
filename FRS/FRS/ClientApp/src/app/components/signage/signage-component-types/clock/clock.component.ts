import { formatDate } from '@angular/common';
import { Component, Input, ChangeDetectorRef } from '@angular/core'
import { SignageComponent } from '../../../../models/signage-component.model';

@Component({
  selector: 'clock',
  template: `<div [style.visibility]="configurations.configurationsObj.text1?.hide ? 'hidden': 'visible'"
[style.position]="'absolute'"

[style.display]="'flex'"
[style.align-items]="configurations.configurationsObj.text1.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.text1.vAlign == 'top' ? 'flex-start' : 'flex-end')"

[style.font-family]="configurations.configurationsObj.text1.fontFamily"
[style.font-size.px]="configurations.configurationsObj.text1.textSize"
[style.color]="configurations.configurationsObj.text1.textColor"
[style.left.px]="configurations.configurationsObj.text1.x"
[style.top.px]="configurations.configurationsObj.text1.y"
[style.width.px]="configurations.configurationsObj.text1.width"
[style.height.px]="configurations.configurationsObj.text1.height"
[style.background-color]="configurations.configurationsObj.text1.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.text1.fieldColor"
[style.font-weight]="configurations.configurationsObj.text1.bold ? 'bold' : 'normal'"
[style.text-decoration]="configurations.configurationsObj.text1.underline ? 'underline' : 'none'"
[style.font-style]="configurations.configurationsObj.text1.italic ? 'italic' : 'normal'"
>
<div [style.width.%]="100" [style.text-align]="configurations.configurationsObj.text1.textAlign">
<span >{{text1}}</span>
</div>
</div>
<div [style.visibility]="configurations.configurationsObj.text2?.hide ? 'hidden': 'visible'"
[style.position]="'absolute'"

[style.display]="'flex'"
[style.align-items]="configurations.configurationsObj.text2.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.text2.vAlign == 'top' ? 'flex-start' : 'flex-end')"

[style.font-family]="configurations.configurationsObj.text2.fontFamily"
[style.font-size.px]="configurations.configurationsObj.text2.textSize"
[style.color]="configurations.configurationsObj.text2.textColor"
[style.left.px]="configurations.configurationsObj.text2.x"
[style.top.px]="configurations.configurationsObj.text2.y"
[style.width.px]="configurations.configurationsObj.text2.width"
[style.height.px]="configurations.configurationsObj.text2.height"
[style.background-color]="configurations.configurationsObj.text2.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.text2.fieldColor"
[style.font-weight]="configurations.configurationsObj.text2.bold ? 'bold' : 'normal'"
[style.text-decoration]="configurations.configurationsObj.text2.underline ? 'underline' : 'none'"
[style.font-style]="configurations.configurationsObj.text2.italic ? 'italic' : 'normal'"
>
<div [style.width.%]="100" [style.text-align]="configurations.configurationsObj.text2.textAlign">
<span >{{text2}}</span>
</div>
</div>`

})
export class Clock {
  @Input() configurations: any;
    text1: string;
    text2: string;

  constructor(private changeDetectorRef: ChangeDetectorRef) {

  }

  start() {
    let now = new Date();

    const locale = 'en-SG';

    this.text1 = formatDate(now, this.configurations.configurationsObj.text1.format || 'dd-MM-yyyy', locale);
    this.text2 = formatDate(now, this.configurations.configurationsObj.text2.format || 'hh:mm aa', locale);

    setTimeout(() => {
      this.start();
    }, (this.configurations.configurationsObj.interval || 30) * 1000);
  }

  ngOnInit() {
    if (!this.configurations.configurationsObj) this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);

    this.start();
  }
}
