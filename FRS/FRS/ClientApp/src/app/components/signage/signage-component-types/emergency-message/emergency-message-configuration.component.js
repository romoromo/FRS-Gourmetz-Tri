"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.EmergencyMessageConfiguration = void 0;
var core_1 = require("@angular/core");
var EmergencyMessageConfiguration = /** @class */ (function () {
    function EmergencyMessageConfiguration() {
        this.configurations = {};
        this.directions = [
            'left',
            'right',
            'top',
            'bottom',
        ];
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
    EmergencyMessageConfiguration.prototype.ngOnInit = function () {
        this.configurations = this.configurations || {};
        this.configurations.textStyle = this.configurations.textStyle || {};
        this.configurations.emergencyStyle = this.configurations.emergencyStyle || {};
    };
    EmergencyMessageConfiguration.prototype.getConfigurations = function () {
        return this.configurations;
    };
    __decorate([
        core_1.Input()
    ], EmergencyMessageConfiguration.prototype, "configurations", void 0);
    EmergencyMessageConfiguration = __decorate([
        core_1.Component({
            selector: 'emergency-message-configuration',
            templateUrl: './emergency-message-configuration.component.html'
        })
    ], EmergencyMessageConfiguration);
    return EmergencyMessageConfiguration;
}());
exports.EmergencyMessageConfiguration = EmergencyMessageConfiguration;
//# sourceMappingURL=emergency-message-configuration.component.js.map