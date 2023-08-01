"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.LocationNameConfiguration = void 0;
var core_1 = require("@angular/core");
var LocationNameConfiguration = /** @class */ (function () {
    function LocationNameConfiguration() {
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
    LocationNameConfiguration.prototype.ngOnInit = function () {
        this.configurations = this.configurations || {};
        this.configurations.name = this.configurations.name || {};
        this.configurations.group = this.configurations.group || {};
    };
    LocationNameConfiguration.prototype.getConfigurations = function () {
        return this.configurations;
    };
    LocationNameConfiguration.prototype.show = function (obj) {
        return JSON.stringify(obj);
    };
    __decorate([
        core_1.Input()
    ], LocationNameConfiguration.prototype, "configurations", void 0);
    LocationNameConfiguration = __decorate([
        core_1.Component({
            selector: 'location-name-configuration',
            templateUrl: './location-name-configuration.component.html'
        })
    ], LocationNameConfiguration);
    return LocationNameConfiguration;
}());
exports.LocationNameConfiguration = LocationNameConfiguration;
//# sourceMappingURL=location-name-configuration.component.js.map