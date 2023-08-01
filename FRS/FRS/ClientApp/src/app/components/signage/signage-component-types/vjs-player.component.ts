// vjs-player.component.ts
import { Component, ElementRef, Input, OnDestroy, OnInit, SimpleChanges, ViewChild, ViewEncapsulation } from '@angular/core';
import videojs from 'video.js';

@Component({
  selector: 'app-vjs-player',
  template: `<video style="object-fit: contain;width: 100%;height: 100%;" #target class="video-js" controls playsinline preload="none">
    <source [src]="getUrl()" [type]="getType()">
</video>
  `,
  styleUrls: [
    './vjs-player.component.css'
  ],
  encapsulation: ViewEncapsulation.None,
})
export class VjsPlayerComponent implements OnInit, OnDestroy {
  @ViewChild('target') target: ElementRef;
  // see options: https://github.com/videojs/video.js/blob/maintutorial-options.html
  @Input() options: {
    fluid: boolean,
    aspectRatio: string,
    autoplay: boolean,
    muted: boolean,
    errorDisplay: boolean,
    volume: number,
    sources: {
      src: string,
      type: string,
    }[],
  };

  previousVolume: number;

  player: videojs.Player;

  previousUrl: string;
  previousType: string;

  constructor(
    private elementRef: ElementRef,
  ) { }

  getUrl() {
    if (this.previousUrl != this.options.sources[0].src) {
      this.player.src(this.options.sources[0].src);
      this.previousUrl = this.options.sources[0].src;
    }

    return this.options.sources[0].src;
  }

  getType() {
    if (this.previousType != this.options.sources[0].type) {
      this.player.src(this.options.sources[0].type);
      this.previousType = this.options.sources[0].type;
    }

    return this.options.sources[0].type;
  }

  ngOnInit() {
    let volume = this.options.volume || 0;
    // instantiate Video.js
    console.log('check option', this.options);
    this.player = videojs(this.target.nativeElement, this.options, function onPlayerReady() {
      console.log('onPlayerReady', this);
      //console.log("volume", getVolume(this));
      this.muted(false);
      this.volume(volume / 100);
    });

    this.previousVolume = this.player.volume();
  }

  ngOnDestroy() {
    // destroy player
    if (this.player) {
      this.player.dispose();
    }
  }

  ngOnChanges(changes: SimpleChanges) {

    if (this.options.volume && this.previousVolume != this.options.volume) {
      this.player.volume(this.options.volume / 100);
      this.previousVolume = this.player.volume();
    }
  }
}
