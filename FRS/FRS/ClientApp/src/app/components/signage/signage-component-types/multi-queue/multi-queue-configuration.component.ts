import { Component, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'multi-queue-configuration',
  templateUrl: './multi-queue-configuration.component.html'

})
export class MultiQueueConfiguration {
  @Input() configurations: any = {};

  duplicate: string;

  ngOnInit() {
    this.configurations = this.configurations || {};
    this.configurations.labelHeaderStyle = this.configurations.labelHeaderStyle || { };
    this.configurations.queueNoHeaderStyle = this.configurations.queueNoHeaderStyle || {};

    this.configurations.stationids = this.configurations.stationids || {};
    this.configurations.stationlabels = this.configurations.stationlabels || {};

    this.configurations.headerlabels = this.configurations.headerlabels || {};
    this.configurations.queuenolabels = this.configurations.queuenolabels || {};
  }

  public uploadSoundFinished = (event) => {
    this.configurations.queueSound = event ? event.dbPath : null;

    if (this.configurations.queueSound) this.configurations.queueSound = this.configurations.queueSound.replace(/\\/g, '/');
  }

  play() {
    var audio = new Audio(this.configurations.queueSound || 'Resources/queue-bell.mp3')//this.configurations.queueSound);
    audio.play();
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

  rowColumnChanged() {
    if (!this.configurations.noOfRows || !this.configurations.noOfColumns) return;

    for (let i in this.configurations.stationids) {
      if (i) {
        let r = i.split("-")[0];
        let c = i.split("-")[1];

        if (r > (this.configurations.noOfRows - 1) + '' || c > (this.configurations.noOfColumns - 1) + '') this.configurations.stationids[i] = null;
      }
    }

    for (let i in this.configurations.stationlabels) {
      if (i) {
        let r = i.split("-")[0];
        let c = i.split("-")[1];

        if (r > (this.configurations.noOfRows - 1) + '' || c > (this.configurations.noOfColumns - 1) + '') this.configurations.stationlabels[i] = null;
      }
    }

    for (let i in this.configurations.headerlabels) { 
      if (i > (this.configurations.noOfColumns - 1) + '') this.configurations.headerlabels[i] = null;
    }

    for (let i in this.configurations.queuenolabels) {
      if (i > (this.configurations.noOfColumns - 1) + '') this.configurations.queuenolabels[i] = null;
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
