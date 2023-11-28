"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var common_1 = require("@angular/common");
var core_1 = require("@angular/core");
var Clock = /** @class */ (function () {
    function Clock(changeDetectorRef) {
        this.changeDetectorRef = changeDetectorRef;
    }
    Clock.prototype.start = function () {
        var _this = this;
        var now = new Date();
        var locale = 'en-SG';
        this.text1 = common_1.formatDate(now, this.configurations.configurationsObj.text1.format || 'dd-MM-yyyy', locale);
        this.text2 = common_1.formatDate(now, this.configurations.configurationsObj.text2.format || 'hh:mm aa', locale);
        setTimeout(function () {
            _this.start();
        }, (this.configurations.configurationsObj.interval || 30) * 1000);
    };
    Clock.prototype.ngOnInit = function () {
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
        this.start();
    };
    __decorate([
        core_1.Input()
    ], Clock.prototype, "configurations", void 0);
    Clock = __decorate([
        core_1.Component({
            selector: 'clock',
            template: "<div [style.visibility]=\"configurations.configurationsObj.text1?.hide ? 'hidden': 'visible'\"\n[style.position]=\"'absolute'\"\n\n[style.display]=\"'flex'\"\n[style.align-items]=\"configurations.configurationsObj.text1.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.text1.vAlign == 'top' ? 'flex-start' : 'flex-end')\"\n\n[style.font-family]=\"configurations.configurationsObj.text1.fontFamily\"\n[style.font-size.px]=\"configurations.configurationsObj.text1.textSize\"\n[style.color]=\"configurations.configurationsObj.text1.textColor\"\n[style.left.px]=\"configurations.configurationsObj.text1.x\"\n[style.top.px]=\"configurations.configurationsObj.text1.y\"\n[style.width.px]=\"configurations.configurationsObj.text1.width\"\n[style.height.px]=\"configurations.configurationsObj.text1.height\"\n[style.background-color]=\"configurations.configurationsObj.text1.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.text1.fieldColor\"\n[style.font-weight]=\"configurations.configurationsObj.text1.bold ? 'bold' : 'normal'\"\n[style.text-decoration]=\"configurations.configurationsObj.text1.underline ? 'underline' : 'none'\"\n[style.font-style]=\"configurations.configurationsObj.text1.italic ? 'italic' : 'normal'\"\n>\n<div [style.width.%]=\"100\" [style.text-align]=\"configurations.configurationsObj.text1.textAlign\">\n<span >{{text1}}</span>\n</div>\n</div>\n<div [style.visibility]=\"configurations.configurationsObj.text2?.hide ? 'hidden': 'visible'\"\n[style.position]=\"'absolute'\"\n\n[style.display]=\"'flex'\"\n[style.align-items]=\"configurations.configurationsObj.text2.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.text2.vAlign == 'top' ? 'flex-start' : 'flex-end')\"\n\n[style.font-family]=\"configurations.configurationsObj.text2.fontFamily\"\n[style.font-size.px]=\"configurations.configurationsObj.text2.textSize\"\n[style.color]=\"configurations.configurationsObj.text2.textColor\"\n[style.left.px]=\"configurations.configurationsObj.text2.x\"\n[style.top.px]=\"configurations.configurationsObj.text2.y\"\n[style.width.px]=\"configurations.configurationsObj.text2.width\"\n[style.height.px]=\"configurations.configurationsObj.text2.height\"\n[style.background-color]=\"configurations.configurationsObj.text2.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.text2.fieldColor\"\n[style.font-weight]=\"configurations.configurationsObj.text2.bold ? 'bold' : 'normal'\"\n[style.text-decoration]=\"configurations.configurationsObj.text2.underline ? 'underline' : 'none'\"\n[style.font-style]=\"configurations.configurationsObj.text2.italic ? 'italic' : 'normal'\"\n>\n<div [style.width.%]=\"100\" [style.text-align]=\"configurations.configurationsObj.text2.textAlign\">\n<span >{{text2}}</span>\n</div>\n</div>"
        })
    ], Clock);
    return Clock;
}());
exports.Clock = Clock;
//# sourceMappingURL=clock.component.js.map