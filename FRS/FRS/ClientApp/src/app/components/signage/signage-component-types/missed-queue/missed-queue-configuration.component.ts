import { Component, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'missed-queue-configuration',
  templateUrl: './missed-queue-configuration.component.html'

})
export class MissedQueueConfiguration {
  @Input() configurations: any = {};

  duplicate: string;

  ngOnInit() {
    this.configurations = this.configurations || {};
    this.configurations.labelHeaderStyle = this.configurations.labelHeaderStyle || {};
    this.configurations.secondLabelHeaderStyle = this.configurations.secondLabelHeaderStyle || {};

    this.configurations.stationids = this.configurations.stationids || {};
  }

  checkDuplicate() {
    this.duplicate = null;
    let map = {}
    for (let sid in this.configurations.stationids) {
      let pid = this.configurations.stationids[sid];

      if (!pid) continue;

      if (map[pid]) {
        this.duplicate = `Duplicate pair id between row-column ${map[pid]} and row-column ${sid} value "${pid}"`;
        break;
      }

      map[pid] = sid;
    }
  }

  getConfigurations() {
    return this.configurations;
  }

  getArrays(num) {
    if (num && num > 0)
      return new Array(num);
  }

}
