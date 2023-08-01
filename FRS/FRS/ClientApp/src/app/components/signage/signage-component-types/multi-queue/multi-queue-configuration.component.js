"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.MultiQueueConfiguration = void 0;
var core_1 = require("@angular/core");
var MultiQueueConfiguration = /** @class */ (function () {
    function MultiQueueConfiguration() {
        this.configurations = {};
    }
    MultiQueueConfiguration.prototype.ngOnInit = function () {
        this.configurations = this.configurations || {};
        this.configurations.labelHeaderStyle = this.configurations.labelHeaderStyle || {};
        this.configurations.queueNoHeaderStyle = this.configurations.queueNoHeaderStyle || {};
        this.configurations.stationids = this.configurations.stationids || {};
        this.configurations.stationlabels = this.configurations.stationlabels || {};
        this.configurations.headerlabels = this.configurations.headerlabels || {};
        this.configurations.queuenolabels = this.configurations.queuenolabels || {};
    };
    MultiQueueConfiguration.prototype.checkDuplicate = function () {
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
    MultiQueueConfiguration.prototype.rowColumnChanged = function () {
        if (!this.configurations.noOfRows || !this.configurations.noOfColumns)
            return;
        for (var i in this.configurations.stationids) {
            if (i) {
                var r = i.split("-")[0];
                var c = i.split("-")[1];
                if (r > (this.configurations.noOfRows - 1) + '' || c > (this.configurations.noOfColumns - 1) + '')
                    this.configurations.stationids[i] = null;
            }
        }
        for (var i in this.configurations.stationlabels) {
            if (i) {
                var r = i.split("-")[0];
                var c = i.split("-")[1];
                if (r > (this.configurations.noOfRows - 1) + '' || c > (this.configurations.noOfColumns - 1) + '')
                    this.configurations.stationlabels[i] = null;
            }
        }
        for (var i in this.configurations.headerlabels) {
            if (i > (this.configurations.noOfColumns - 1) + '')
                this.configurations.headerlabels[i] = null;
        }
        for (var i in this.configurations.queuenolabels) {
            if (i > (this.configurations.noOfColumns - 1) + '')
                this.configurations.queuenolabels[i] = null;
        }
    };
    MultiQueueConfiguration.prototype.getConfigurations = function () {
        return this.configurations;
    };
    MultiQueueConfiguration.prototype.getArrays = function (num) {
        if (num && num > 0)
            return new Array(num);
    };
    __decorate([
        core_1.Input()
    ], MultiQueueConfiguration.prototype, "configurations", void 0);
    MultiQueueConfiguration = __decorate([
        core_1.Component({
            selector: 'multi-queue-configuration',
            templateUrl: './multi-queue-configuration.component.html'
        })
    ], MultiQueueConfiguration);
    return MultiQueueConfiguration;
}());
exports.MultiQueueConfiguration = MultiQueueConfiguration;
//# sourceMappingURL=multi-queue-configuration.component.js.map