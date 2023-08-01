"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.MissedQueueConfiguration = void 0;
var core_1 = require("@angular/core");
var MissedQueueConfiguration = /** @class */ (function () {
    function MissedQueueConfiguration() {
        this.configurations = {};
    }
    MissedQueueConfiguration.prototype.ngOnInit = function () {
        this.configurations = this.configurations || {};
        this.configurations.labelHeaderStyle = this.configurations.labelHeaderStyle || {};
        this.configurations.secondLabelHeaderStyle = this.configurations.secondLabelHeaderStyle || {};
        this.configurations.stationids = this.configurations.stationids || {};
    };
    MissedQueueConfiguration.prototype.checkDuplicate = function () {
        this.duplicate = null;
        var map = {};
        for (var sid in this.configurations.stationids) {
            var pid = this.configurations.stationids[sid];
            if (!pid)
                continue;
            if (map[pid]) {
                this.duplicate = "Duplicate pair id between row-column " + map[pid] + " and row-column " + sid + " value \"" + pid + "\"";
                break;
            }
            map[pid] = sid;
        }
    };
    MissedQueueConfiguration.prototype.getConfigurations = function () {
        return this.configurations;
    };
    MissedQueueConfiguration.prototype.getArrays = function (num) {
        if (num && num > 0)
            return new Array(num);
    };
    __decorate([
        core_1.Input()
    ], MissedQueueConfiguration.prototype, "configurations", void 0);
    MissedQueueConfiguration = __decorate([
        core_1.Component({
            selector: 'missed-queue-configuration',
            templateUrl: './missed-queue-configuration.component.html'
        })
    ], MissedQueueConfiguration);
    return MissedQueueConfiguration;
}());
exports.MissedQueueConfiguration = MissedQueueConfiguration;
//# sourceMappingURL=missed-queue-configuration.component.js.map