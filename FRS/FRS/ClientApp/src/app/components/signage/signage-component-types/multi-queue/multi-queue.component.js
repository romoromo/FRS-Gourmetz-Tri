"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.NgInit = exports.MultiQueue = void 0;
var core_1 = require("@angular/core");
var animations_1 = require("@angular/animations");
var MultiQueue = /** @class */ (function () {
    function MultiQueue(changeDetectorRef, route, authService, deviceService, configurationService) {
        this.changeDetectorRef = changeDetectorRef;
        this.route = route;
        this.authService = authService;
        this.deviceService = deviceService;
        this.configurationService = configurationService;
        this.data = {};
        this.dataFifo = [];
        this.swap = true;
    }
    MultiQueue.prototype.checkEven = function (j) {
        return (this.configurations.configurationsObj.swapLabel && j % 2 != 0) || (!this.configurations.configurationsObj.swapLabel && j % 2 == 0);
    };
    MultiQueue.prototype.checkOdd = function (j) {
        return (this.configurations.configurationsObj.swapLabel && j % 2 == 0) || (!this.configurations.configurationsObj.swapLabel && j % 2 != 0);
    };
    MultiQueue.prototype.floor = function (j) {
        return Math.floor(j);
    };
    MultiQueue.prototype.parseInteger = function (str) {
        try {
            return parseInt(str);
        }
        catch (ex) {
            return str;
        }
    };
    MultiQueue.prototype.ngOnInit = function () {
        var _this = this;
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
        this.configurations.configurationsObj.stationlabels = this.configurations.configurationsObj.stationlabels || {};
        this.configurations.configurationsObj.headerlabels = this.configurations.configurationsObj.headerlabels || {};
        this.configurations.configurationsObj.queuenolabels = this.configurations.configurationsObj.queuenolabels || {};
        this.obj = this.configurations.configurationsObj;
        if (!this.preview) {
            this.route.params.subscribe(function (queryParams) {
                _this.mac_address = queryParams["mac_address"];
            });
            if (!this.mac_address) {
                this.route.queryParams.subscribe(function (queryParams) {
                    _this.mac_address = queryParams["mac_address"];
                });
            }
            this.signalRCoreconnection = this.authService.signalRConnection(this.configurationService.baseUrl + "/hub/queue?device_id=" + this.mac_address, true, this.signalRCoreconnection);
            if (this.signalRCoreconnection != null) {
                this.signalRCoreconnection.on("UpdateQueue", function (param) {
                    _this.updateStationId(param);
                });
            }
            this.getDeviceInfo();
        }
    };
    MultiQueue.prototype.updateStationId = function (param) {
        var dataidx = null;
        var idx = this.findStationId(param);
        if (!idx)
            return;
        if (!this.configurations.configurationsObj.fifoMode) {
            if (!this.data[idx])
                this.data[idx] = {};
            dataidx = this.data[idx];
        }
        else {
            var exist = this.dataFifo.find(function (d) { return d ? d.roomName === param.roomName : false; });
            if (!exist) {
                this.dataFifo.unshift({});
                dataidx = this.dataFifo[0];
                this.dataFifo.length = this.configurations.configurationsObj.noOfRows * this.configurations.configurationsObj.noOfColumns;
            }
            else {
                dataidx = exist;
            }
        }
        if (!dataidx)
            return;
        dataidx.stationId = param.stationId;
        dataidx.roomName = param.callAction == "call" || param.callAction == "silent call" ? param.roomName : '';
        dataidx.queueNo = param.callAction == "call" || param.callAction == "silent call" ? param.queueNo : '';
        dataidx.roomInfo = param.roomInfo;
        dataidx.servingInfo = param.servingInfo;
        dataidx.doctorInfo = param.doctorInfo;
        dataidx.assistantInfo = param.assistantInfo;
        param.currentDisplay = dataidx.queueNo;
        param.deviceId = this.mac_address;
        if (dataidx.queueNo && !this.configurations.configurationsObj.noSound && param.callAction == "call") {
            var audio = new Audio(this.configurations.configurationsObj.queueSound || 'Resources/queue-bell.mp3'); //this.configurations.configurationsObj.queueSound);
            audio.play();
        }
        if (dataidx.queueNo) {
            dataidx.isFlashing = true;
            dataidx.isOn = true;
            this.toggle(dataidx);
            setTimeout(function () { return dataidx.isFlashing = false; }, (this.configurations.configurationsObj.flashDuration || 7) * 1000);
        }
        this.signalRCoreconnection.send("queueReturn", param);
        if (this.configurations.configurationsObj.fifoMode)
            this.dataFifo = this.dataFifo.filter(function (d) { return d.queueNo && d.roomName; });
    };
    MultiQueue.prototype.toggle = function (dataidx) {
        var _this = this;
        setTimeout(function () {
            dataidx.isOn = !dataidx.isOn;
            if (dataidx.isFlashing)
                _this.toggle(dataidx);
        }, 500);
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
            templateUrl: './multi-queue.component.html',
            animations: [
                animations_1.trigger("changeBodyColor", [
                    animations_1.state('off', animations_1.style({
                        color: '{{default}}'
                    }), { params: { default: 'black' } }),
                    animations_1.state('on', animations_1.style({
                        color: '{{color}}'
                    }), { params: { color: 'red' } }),
                    animations_1.transition('off => on', animations_1.animate('0.5s')),
                    animations_1.transition('on => off', animations_1.animate('0.5s'))
                ])
            ]
        })
    ], MultiQueue);
    return MultiQueue;
}());
exports.MultiQueue = MultiQueue;
var NgInit = /** @class */ (function () {
    function NgInit() {
        this.values = {};
    }
    NgInit.prototype.ngOnInit = function () {
        if (this.ngInit) {
            this.ngInit();
        }
    };
    __decorate([
        core_1.Input()
    ], NgInit.prototype, "values", void 0);
    __decorate([
        core_1.Input()
    ], NgInit.prototype, "ngInit", void 0);
    NgInit = __decorate([
        core_1.Directive({
            selector: 'ngInit',
            exportAs: 'ngInit'
        })
    ], NgInit);
    return NgInit;
}());
exports.NgInit = NgInit;
//# sourceMappingURL=multi-queue.component.js.map