"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.MultiQueue = void 0;
var core_1 = require("@angular/core");
var MultiQueue = /** @class */ (function () {
    function MultiQueue(changeDetectorRef, route, authService, deviceService) {
        this.changeDetectorRef = changeDetectorRef;
        this.route = route;
        this.authService = authService;
        this.deviceService = deviceService;
        this.data = {};
    }
    MultiQueue.prototype.ngOnInit = function () {
        var _this = this;
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
        if (!this.preview) {
            this.route.params.subscribe(function (queryParams) {
                _this.mac_address = queryParams["mac_address"];
            });
            if (!this.mac_address) {
                this.route.queryParams.subscribe(function (queryParams) {
                    _this.mac_address = queryParams["mac_address"];
                });
            }
            this.signalRCoreconnection = this.authService.signalRConnection("/hub/queue?device_id=" + this.mac_address, true);
            if (this.signalRCoreconnection != null) {
                this.signalRCoreconnection.on("UpdateQueue", function (param) {
                    _this.updateStationId(param);
                });
            }
            this.getDeviceInfo();
        }
    };
    MultiQueue.prototype.updateStationId = function (param) {
        var idx = this.findStationId(param);
        if (!idx)
            return;
        if (!this.data[idx])
            this.data[idx] = {};
      this.data[idx].queueNo = param.callAction == "call" || param.callAction == "silent call" ? param.queueNo : '';
        this.data[idx].roomInfo = param.roomInfo;
        this.data[idx].servingInfo = param.servingInfo;
        this.data[idx].doctorInfo = param.doctorInfo;
        this.data[idx].assistantInfo = param.assistantInfo;
        param.currentDisplay = this.data[idx].queueNo;
        param.deviceId = this.mac_address;
        this.signalRCoreconnection.send("queueReturn", param);
    };
    MultiQueue.prototype.findStationId = function (param) {
        var stationids = this.configurations.configurationsObj.stationids;
        for (var _i = 0, _a = Object.keys(stationids); _i < _a.length; _i++) {
            var key = _a[_i];
            if (stationids[key] == param.stationId)
                return key;
        }
        return null;
    };
    MultiQueue.prototype.getDeviceInfo = function () {
        var _this = this;
        if (this.mac_address) {
            this.deviceService.getDeviceById(null, this.mac_address, true)
                .subscribe(function (results) {
                _this.device = results.data;
                if (_this.device) {
                    _this.locationColorTheme = _this.device.locationColorTheme;
                    _this.locationName = _this.device.locationName;
                }
            });
        }
    };
    MultiQueue.prototype.getArrays = function (num) {
        if (num && num > 0)
            return new Array(num);
    };
    __decorate([
        core_1.Input()
    ], MultiQueue.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], MultiQueue.prototype, "preview", void 0);
    MultiQueue = __decorate([
        core_1.Component({
            selector: 'multi-queue',
            templateUrl: './multi-queue.component.html'
        })
    ], MultiQueue);
    return MultiQueue;
}());
exports.MultiQueue = MultiQueue;
//# sourceMappingURL=multi-queue.component.js.map
