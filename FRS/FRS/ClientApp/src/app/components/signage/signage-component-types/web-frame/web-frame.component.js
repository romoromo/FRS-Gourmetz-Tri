"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.WebFrame = void 0;
var core_1 = require("@angular/core");
var WebFrame = /** @class */ (function () {
    function WebFrame(sanitizer) {
        this.sanitizer = sanitizer;
    }
    WebFrame.prototype.ngOnInit = function () {
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
    };
    WebFrame.prototype.ngOnChanges = function (changes) {
        console.log("HTML Changes");
    };
    __decorate([
        core_1.Input()
    ], WebFrame.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], WebFrame.prototype, "preview", void 0);
    WebFrame = __decorate([
        core_1.Component({
            selector: 'signage-web-frame',
            template: "<iframe frameborder=\"0\" scrolling=\"no\" [src]=\"sanitizer.bypassSecurityTrustResourceUrl(configurations.configurationsObj.url)\" height=\"100%\" width=\"100%\" >\n<p>iframes are not supported by your browser.</p>\n</iframe>"
        })
    ], WebFrame);
    return WebFrame;
}());
exports.WebFrame = WebFrame;
//# sourceMappingURL=web-frame.component.js.map