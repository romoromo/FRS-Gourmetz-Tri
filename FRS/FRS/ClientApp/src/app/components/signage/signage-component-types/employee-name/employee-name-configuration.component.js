"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.EmployeeNameConfiguration = void 0;
var core_1 = require("@angular/core");
var EmployeeNameConfiguration = /** @class */ (function () {
    function EmployeeNameConfiguration() {
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
    EmployeeNameConfiguration.prototype.ngOnInit = function () {
        this.configurations = this.configurations || {};
        this.configurations.name = this.configurations.name || {};
    };
    EmployeeNameConfiguration.prototype.getConfigurations = function () {
        return this.configurations;
    };
    EmployeeNameConfiguration.prototype.show = function (obj) {
        return JSON.stringify(obj);
    };
    __decorate([
        core_1.Input()
    ], EmployeeNameConfiguration.prototype, "configurations", void 0);
    EmployeeNameConfiguration = __decorate([
        core_1.Component({
            selector: 'employee-name-configuration',
            templateUrl: './employee-name-configuration.component.html'
        })
    ], EmployeeNameConfiguration);
    return EmployeeNameConfiguration;
}());
exports.EmployeeNameConfiguration = EmployeeNameConfiguration;
//# sourceMappingURL=employee-name-configuration.component.js.map