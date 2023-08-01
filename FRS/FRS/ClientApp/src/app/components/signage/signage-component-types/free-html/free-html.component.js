"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var FreeHTML = /** @class */ (function () {
    function FreeHTML(sanitizer) {
        this.sanitizer = sanitizer;
    }
    FreeHTML.prototype.ngOnInit = function () {
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
        if (!this.configurations.configurationsObj.html)
            this.configurations.configurationsObj.html = "";
        this.safeHtml = this.sanitizer.bypassSecurityTrustHtml(this.configurations.configurationsObj.html);
    };
    FreeHTML.prototype.getSafeHTML = function (html) {
        return this.sanitizer.bypassSecurityTrustHtml(html);
    };
    FreeHTML.prototype.ngOnChanges = function (changes) {
        console.log("HTML Changes");
    };
    __decorate([
        core_1.Input()
    ], FreeHTML.prototype, "configurations", void 0);
    FreeHTML = __decorate([
        core_1.Component({
            selector: 'free-html',
            template: "<div [innerHTML]=\"getSafeHTML(configurations.configurationsObj.html)\"></div>"
        })
    ], FreeHTML);
    return FreeHTML;
}());
exports.FreeHTML = FreeHTML;
//# sourceMappingURL=free-html.component.js.map