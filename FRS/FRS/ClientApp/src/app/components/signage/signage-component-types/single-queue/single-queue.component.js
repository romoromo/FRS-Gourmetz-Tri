"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.SingleQueue = void 0;
var core_1 = require("@angular/core");
var SingleQueue = /** @class */ (function () {
    function SingleQueue(changeDetectorRef, authService, route, deviceService, configurationService) {
        this.changeDetectorRef = changeDetectorRef;
        this.authService = authService;
        this.route = route;
        this.deviceService = deviceService;
        this.configurationService = configurationService;
    }
    SingleQueue.prototype.ngOnInit = function () {
        var _this = this;
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
        console.log("initqueue");
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
                    _this.configurations.configurationsObj.texts.queueNo.value = param.callAction == "call" || param.callAction == "silent call" ? param.queueNo : '';
                    if (!_this.configurations.configurationsObj.roomInfoLocationName)
                        _this.configurations.configurationsObj.texts.roomInfo.value = param.roomInfo;
                    _this.configurations.configurationsObj.texts.servingInfo.value = param.servingInfo;
                    _this.configurations.configurationsObj.texts.doctorInfo.value = param.doctorInfo;
                    _this.configurations.configurationsObj.texts.assistantInfo.value = param.assistantInfo;
                    param.currentDisplay = _this.configurations.configurationsObj.texts.queueNo.value;
                    param.deviceId = _this.mac_address;
                    _this.signalRCoreconnection.send("queueReturn", param);
                });
            }
            this.getDeviceInfo();
        }
    };
    SingleQueue.prototype.getDeviceInfo = function () {
        var _this = this;
        if (this.mac_address) {
            this.deviceService.getDeviceById(null, this.mac_address, true)
                .subscribe(function (results) {
                _this.device = results.data;
                if (_this.device) {
                    _this.locationColorTheme = _this.device.locationColorTheme;
                    _this.locationName = _this.device.locationName;
                    if (_this.configurations.configurationsObj.roomInfoLocationName)
                        _this.configurations.configurationsObj.texts.roomInfo.value = _this.locationName;
                }
            });
        }
    };
    __decorate([
        core_1.Input()
    ], SingleQueue.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], SingleQueue.prototype, "preview", void 0);
    SingleQueue = __decorate([
        core_1.Component({
            selector: 'single-queue',
            template: "<div *ngFor=\"let t of configurations.configurationsObj.texts | keyvalue\" [style.visibility]=\"t.value.hide ? 'hidden': 'visible'\"\n[style.position]=\"'absolute'\"\n\n[style.display]=\"'flex'\"\n[style.align-items]=\"t.value.vAlign == 'middle' ? 'center' : (t.value.vAlign == 'top' ? 'flex-start' : 'flex-end')\"\n\n[style.font-family]=\"t.value.fontFamily\"\n[style.font-size.px]=\"t.value.textSize\"\n[style.color]=\"t.value.textColor\"\n[style.left.px]=\"t.value.x\"\n[style.top.px]=\"t.value.y\"\n[style.width.px]=\"t.value.width\"\n[style.height.px]=\"t.value.height\"\n[style.background-color]=\"t.value.fieldColorLocation && locationColorTheme ? locationColorTheme : t.value.fieldColor\"\n[style.font-weight]=\"t.value.bold ? 'bold' : 'normal'\"\n[style.text-decoration]=\"t.value.underline ? 'underline' : 'none'\"\n[style.font-style]=\"t.value.italic ? 'italic' : 'normal'\"\n>\n<div [style.width.%]=\"100\" [style.text-align]=\"t.value.textAlign\">\n<span *ngIf=\"preview\">{{t.value.label}}</span>\n<span *ngIf=\"!preview\">{{t.value.value}}</span>\n</div>\n</div>"
        })
    ], SingleQueue);
    return SingleQueue;
}());
exports.SingleQueue = SingleQueue;
//# sourceMappingURL=single-queue.component.js.map