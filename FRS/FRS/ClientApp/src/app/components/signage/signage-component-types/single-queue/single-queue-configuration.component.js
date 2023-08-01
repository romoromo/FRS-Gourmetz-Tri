"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.SingleQueueConfiguration = void 0;
var core_1 = require("@angular/core");
var SingleQueueConfiguration = /** @class */ (function () {
    function SingleQueueConfiguration() {
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
    SingleQueueConfiguration.prototype.ngOnInit = function () {
        this.configurations = this.configurations || {};
        this.configurations.texts = this.configurations.texts || {};
        this.configurations.texts.queueNo = this.configurations.texts.queueNo || { label: 'Queue No' };
        this.configurations.texts.roomInfo = this.configurations.texts.roomInfo || { label: 'Room Info' };
        this.configurations.texts.servingInfo = this.configurations.texts.servingInfo || { label: 'Serving Info' };
        this.configurations.texts.doctorInfo = this.configurations.texts.doctorInfo || { label: 'Doctor Info' };
        this.configurations.texts.assistantInfo = this.configurations.texts.assistantInfo || { label: 'Assistant Info' };
    };
    SingleQueueConfiguration.prototype.getConfigurations = function () {
        return this.configurations;
    };
    SingleQueueConfiguration.prototype.show = function (obj) {
        return JSON.stringify(obj);
    };
    __decorate([
        core_1.Input()
    ], SingleQueueConfiguration.prototype, "configurations", void 0);
    SingleQueueConfiguration = __decorate([
        core_1.Component({
            selector: 'single-queue-configuration',
            templateUrl: './single-queue-configuration.component.html'
        })
    ], SingleQueueConfiguration);
    return SingleQueueConfiguration;
}());
exports.SingleQueueConfiguration = SingleQueueConfiguration;
//# sourceMappingURL=single-queue-configuration.component.js.map