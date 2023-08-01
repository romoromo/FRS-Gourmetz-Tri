"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.VjsPlayerComponent = void 0;
// vjs-player.component.ts
var core_1 = require("@angular/core");
var video_js_1 = require("video.js");
var VjsPlayerComponent = /** @class */ (function () {
    function VjsPlayerComponent(elementRef) {
        this.elementRef = elementRef;
    }
    VjsPlayerComponent.prototype.getUrl = function () {
        if (this.previousUrl != this.options.sources[0].src) {
            this.player.src(this.options.sources[0].src);
            this.previousUrl = this.options.sources[0].src;
        }
        return this.options.sources[0].src;
    };
    VjsPlayerComponent.prototype.getType = function () {
        if (this.previousType != this.options.sources[0].type) {
            this.player.src(this.options.sources[0].type);
            this.previousType = this.options.sources[0].type;
        }
        return this.options.sources[0].type;
    };
    VjsPlayerComponent.prototype.ngOnInit = function () {
        var volume = this.options.volume || 0;
        // instantiate Video.js
        console.log('check option', this.options);
        this.player = video_js_1.default(this.target.nativeElement, this.options, function onPlayerReady() {
            console.log('onPlayerReady', this);
            //console.log("volume", getVolume(this));
            this.muted(false);
            this.volume(volume / 100);
        });
        this.previousVolume = this.player.volume();
    };
    VjsPlayerComponent.prototype.ngOnDestroy = function () {
        // destroy player
        if (this.player) {
            this.player.dispose();
        }
    };
    VjsPlayerComponent.prototype.ngOnChanges = function (changes) {
        if (this.options.volume && this.previousVolume != this.options.volume) {
            this.player.volume(this.options.volume / 100);
            this.previousVolume = this.player.volume();
        }
    };
    __decorate([
        core_1.ViewChild('target')
    ], VjsPlayerComponent.prototype, "target", void 0);
    __decorate([
        core_1.Input()
    ], VjsPlayerComponent.prototype, "options", void 0);
    VjsPlayerComponent = __decorate([
        core_1.Component({
            selector: 'app-vjs-player',
            template: "<video style=\"object-fit: contain;width: 100%;height: 100%;\" #target class=\"video-js\" controls playsinline preload=\"none\">\n    <source [src]=\"getUrl()\" [type]=\"getType()\">\n</video>\n  ",
            styleUrls: [
                './vjs-player.component.css'
            ],
            encapsulation: core_1.ViewEncapsulation.None,
        })
    ], VjsPlayerComponent);
    return VjsPlayerComponent;
}());
exports.VjsPlayerComponent = VjsPlayerComponent;
//# sourceMappingURL=vjs-player.component.js.map