"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.SingleBookingConfiguration = void 0;
var core_1 = require("@angular/core");
var SingleBookingConfiguration = /** @class */ (function () {
    function SingleBookingConfiguration() {
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
    SingleBookingConfiguration.prototype.ngOnInit = function () {
        this.configurations = this.configurations || {};
        this.configurations.contactNo = this.configurations.contactNo || {};
        this.configurations.description = this.configurations.description || {};
        this.configurations.location = this.configurations.location || {};
        this.configurations.time = this.configurations.time || {};
        this.configurations.meetingPurpose = this.configurations.meetingPurpose || {};
    };
    SingleBookingConfiguration.prototype.getConfigurations = function () {
        return this.configurations;
    };
    SingleBookingConfiguration.prototype.show = function (obj) {
        return JSON.stringify(obj);
    };
    __decorate([
        core_1.Input()
    ], SingleBookingConfiguration.prototype, "configurations", void 0);
    SingleBookingConfiguration = __decorate([
        core_1.Component({
            selector: 'single-booking-configuration',
            templateUrl: './single-booking-configuration.component.html'
        })
    ], SingleBookingConfiguration);
    return SingleBookingConfiguration;
}());
exports.SingleBookingConfiguration = SingleBookingConfiguration;
//# sourceMappingURL=single-booking-configuration.component.js.map