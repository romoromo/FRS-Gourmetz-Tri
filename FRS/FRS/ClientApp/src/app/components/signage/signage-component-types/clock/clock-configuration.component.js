"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.ClockConfiguration = void 0;
var core_1 = require("@angular/core");
var ClockConfiguration = /** @class */ (function () {
    function ClockConfiguration() {
        this.configurations = {};
        this.fonts = [
            'American Typewriter',
            'Andalé Mono',
            'Arial Black',
            'Arial',
            'Bradley Hand',
            'Brush Script MT',
            'Comic Sans MS',
            'Courier',
            'Didot',
            'Georgia',
            'Impact',
            'Lucida Console',
            'Luminari',
            'Monaco',
            'Tahoma',
            'Times New Roman',
            'Trebuchet MS',
            'Verdana',
        ];
        this.aligns = [
            'left',
            'center',
            'right',
            'justify'
        ];
    }
    ClockConfiguration.prototype.ngOnInit = function () {
        this.configurations = this.configurations || {};
        this.configurations.text1 = this.configurations.text1 || {};
        this.configurations.text2 = this.configurations.text2 || {};
    };
    ClockConfiguration.prototype.getConfigurations = function () {
        return this.configurations;
    };
    __decorate([
        core_1.Input()
    ], ClockConfiguration.prototype, "configurations", void 0);
    ClockConfiguration = __decorate([
        core_1.Component({
            selector: 'clock-configuration',
            templateUrl: './clock-configuration.component.html'
        })
    ], ClockConfiguration);
    return ClockConfiguration;
}());
exports.ClockConfiguration = ClockConfiguration;
//# sourceMappingURL=clock-configuration.component.js.map