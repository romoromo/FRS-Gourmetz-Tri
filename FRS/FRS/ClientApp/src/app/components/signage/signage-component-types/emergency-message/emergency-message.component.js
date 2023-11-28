"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var animations_1 = require("@angular/animations");
var EmergencyMessage = /** @class */ (function () {
    function EmergencyMessage(changeDetectorRef, builder, route, deviceService, authService, configurationService) {
        this.changeDetectorRef = changeDetectorRef;
        this.builder = builder;
        this.route = route;
        this.deviceService = deviceService;
        this.authService = authService;
        this.configurationService = configurationService;
        this.directionMap = {
            left: 'right',
            right: 'left',
            top: 'bottom',
            bottom: 'top'
        };
        this.regularstyle = {};
        this.regularanimStyle = {};
        this.emergencystyle = {};
        this.emergencyanimStyle = {};
        this.defaultDuration = 30;
    }
    EmergencyMessage.prototype.ngOnInit = function () {
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
    };
    EmergencyMessage.prototype.ngAfterViewInit = function () {
        var _this = this;
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
        if (this.configurations.configurationsObj.textStyle && this.configurations.configurationsObj.textStyle.enableScrolling)
            this.startAnimation();
        if (this.configurations.configurationsObj.emergencyStyle && this.configurations.configurationsObj.emergencyStyle.enableScrolling)
            this.startEmergencyAnimation();
        this.route.params.subscribe(function (queryParams) {
            _this.mac_address = queryParams["mac_address"];
        });
        if (!this.mac_address) {
            this.route.queryParams.subscribe(function (queryParams) {
                _this.mac_address = queryParams["mac_address"];
            });
        }
        console.log("MACS", this.mac_address);
        this.startMessage();
        this.deviceService.getDeviceById(null, this.mac_address, true)
            .subscribe(function (results) {
            _this.device = results.data;
            if (_this.device) {
                _this.locationColorTheme = _this.device.locationColorTheme;
                _this.locationName = _this.device.locationName;
                _this.message1 = _this.device.emergencyMessage1;
                _this.message2 = _this.device.emergencyMessage2;
                _this.message3 = _this.device.emergencyMessage3;
                _this.message4 = _this.device.emergencyMessage4;
                if (_this.to)
                    clearTimeout(_this.to);
                _this.startMessage();
                _this.signalRCoreconnection = _this.authService.signalRConnection(_this.configurationService.baseUrl + "/hub/frsdevice?device_id=" + _this.device.id + "&source=server", true, _this.signalRCoreconnection);
                if (_this.signalRCoreconnection != null) {
                    _this.signalRCoreconnection.on("RefreshDeviceData", function (deviceVM) {
                        console.log('Refreshing Device Display');
                        _this.message1 = deviceVM.emergencyMessage1;
                        _this.message2 = deviceVM.emergencyMessage2;
                        _this.message3 = deviceVM.emergencyMessage3;
                        _this.message4 = deviceVM.emergencyMessage4;
                        if (_this.to)
                            clearTimeout(_this.to);
                        _this.startMessage();
                    });
                }
            }
        }, function (error) {
        });
    };
    EmergencyMessage.prototype.startMessage = function (queue) {
        queue = queue || 1;
        if (queue > 3)
            queue = 1;
        if (this.message4)
            this.message = this.message4;
        else if (this.message1 || this.message2 || this.message3) {
            this.configurations.configurationsObj.message1Duration = this.configurations.configurationsObj.message1Duration || this.defaultDuration;
            this.configurations.configurationsObj.message2Duration = this.configurations.configurationsObj.message2Duration || this.defaultDuration;
            this.configurations.configurationsObj.message3Duration = this.configurations.configurationsObj.message3Duration || this.defaultDuration;
            if (queue == 1) {
                if (this.message1) {
                    this.message = this.message1;
                }
                this.setDelay(queue, this.configurations.configurationsObj.message1Duration);
                return;
            }
            if (queue == 2) {
                if (this.message2) {
                    this.message = this.message2;
                }
                this.setDelay(queue, this.configurations.configurationsObj.message2Duration);
                return;
            }
            if (queue == 3) {
                if (this.message3) {
                    this.message = this.message3;
                }
                this.setDelay(queue, this.configurations.configurationsObj.message3Duration);
                return;
            }
        }
        this.setDelay(queue, this.defaultDuration);
    };
    EmergencyMessage.prototype.setDelay = function (queue, duration) {
        var _this = this;
        this.to = setTimeout(function () { return _this.startMessage(++queue); }, duration * 1000);
    };
    EmergencyMessage.prototype.startAnimation = function () {
        var _this = this;
        this.regularleft = this.el.nativeElement.offsetWidth;
        this.regularstyle[this.directionMap[this.configurations.configurationsObj.direction || 'left']] = "-" + this.regularleft + "px";
        this.regularanimStyle[this.directionMap[this.configurations.configurationsObj.direction || 'left']] = '100%';
        var s = this.configurations.configurationsObj.speed || 10;
        var ss = s + (this.regularleft / (this.configurations.configurationsObj.width / s));
        this.regularfactory = this.builder.build([
            animations_1.style(this.regularstyle),
            animations_1.animate((ss || s) * 1000, animations_1.style(this.regularanimStyle))
        ]);
        this.regularplayer = this.regularfactory.create(this.el.nativeElement, {});
        this.regularplayer.reset();
        this.regularplayer.onDone(function () {
            _this.startAnimation();
        });
        this.regularplayer.play();
    };
    EmergencyMessage.prototype.startEmergencyAnimation = function () {
        var _this = this;
        this.emergencyleft = this.em.nativeElement.offsetWidth;
        this.emergencystyle[this.directionMap[this.configurations.configurationsObj.direction || 'left']] = "-" + this.emergencyleft + "px";
        this.emergencyanimStyle[this.directionMap[this.configurations.configurationsObj.direction || 'left']] = '100%';
        var s = this.configurations.configurationsObj.speed || 10;
        var ss = s + (this.emergencyleft / (this.configurations.configurationsObj.width / s));
        this.emergencyfactory = this.builder.build([
            animations_1.style(this.emergencystyle),
            animations_1.animate((ss || s) * 1000, animations_1.style(this.emergencyanimStyle))
        ]);
        this.emergencyplayer = this.emergencyfactory.create(this.em.nativeElement, {});
        this.emergencyplayer.reset();
        this.emergencyplayer.onDone(function () {
            _this.startEmergencyAnimation();
        });
        this.emergencyplayer.play();
    };
    __decorate([
        core_1.Input()
    ], EmergencyMessage.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], EmergencyMessage.prototype, "preview", void 0);
    __decorate([
        core_1.ViewChild('el')
    ], EmergencyMessage.prototype, "el", void 0);
    __decorate([
        core_1.ViewChild('em')
    ], EmergencyMessage.prototype, "em", void 0);
    EmergencyMessage = __decorate([
        core_1.Component({
            selector: 'emergency-message',
            template: "<div\n[style.visibility]=\"!configurations.configurationsObj.textStyle?.hide && !message4 ? 'visible' : 'hidden'\"\n[style.position]=\"'absolute'\"\n[style.overflow]=\"'hidden'\"\n[style.display]=\"'flex'\"\n[style.align-items]=\"configurations.configurationsObj.textStyle?.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.textStyle?.vAlign == 'top' ? 'flex-start' : 'flex-end')\"\n[style.font-family]=\"configurations.configurationsObj.textStyle?.fontFamily\"\n[style.text-align]=\"configurations.configurationsObj.textStyle?.textAlign\"\n[style.font-size.px]=\"configurations.configurationsObj.textStyle?.textSize\"\n[style.color]=\"configurations.configurationsObj.textStyle?.textColor\"\n[style.left.px]=\"configurations.configurationsObj.textStyle?.x\"\n[style.top.px]=\"configurations.configurationsObj.textStyle?.y\"\n[style.width.px]=\"configurations.configurationsObj.textStyle?.width\"\n[style.height.px]=\"configurations.configurationsObj.textStyle?.height\"\n[style.background-color]=\"configurations.configurationsObj.textStyle?.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.textStyle?.fieldColor\"\n[style.font-weight]=\"configurations.configurationsObj.textStyle?.bold ? 'bold' : 'normal'\"\n[style.text-decoration]=\"configurations.configurationsObj.textStyle?.underline ? 'underline' : 'none'\"\n[style.font-style]=\"configurations.configurationsObj.textStyle?.italic ? 'italic' : 'normal'\">\n  <div\n     \n    [style.position]=\"'absolute'\"\n    id=\"el\" #el\n    [style.white-space]=\"'nowrap'\">\n      <div *ngIf=\"preview\">Sample Message!</div><div *ngIf=\"!preview\">{{message}}</div>\n  </div>\n</div>\n<div\n[style.visibility]=\"!configurations.configurationsObj.emergencyStyle?.hide && (preview || message4) ? 'visible' : 'hidden'\"\n[style.position]=\"'absolute'\"\n[style.overflow]=\"'hidden'\"\n[style.display]=\"'flex'\"\n[style.align-items]=\"configurations.configurationsObj.emergencyStyle?.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.emergencyStyle?.vAlign == 'top' ? 'flex-start' : 'flex-end')\"\n[style.font-family]=\"configurations.configurationsObj.emergencyStyle?.fontFamily\"\n[style.text-align]=\"configurations.configurationsObj.emergencyStyle?.textAlign\"\n[style.font-size.px]=\"configurations.configurationsObj.emergencyStyle?.textSize\"\n[style.color]=\"configurations.configurationsObj.emergencyStyle?.textColor\"\n[style.left.px]=\"configurations.configurationsObj.emergencyStyle?.x\"\n[style.top.px]=\"configurations.configurationsObj.emergencyStyle?.y\"\n[style.width.px]=\"configurations.configurationsObj.emergencyStyle?.width\"\n[style.height.px]=\"configurations.configurationsObj.emergencyStyle?.height\"\n[style.background-color]=\"configurations.configurationsObj.emergencyStyle?.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.emergencyStyle?.fieldColor\"\n[style.font-weight]=\"configurations.configurationsObj.emergencyStyle?.bold ? 'bold' : 'normal'\"\n[style.text-decoration]=\"configurations.configurationsObj.emergencyStyle?.underline ? 'underline' : 'none'\"\n[style.font-style]=\"configurations.configurationsObj.emergencyStyle?.italic ? 'italic' : 'normal'\">\n  <div\n     [style.width.%]=\"100\" [style.text-align]=\"configurations.configurationsObj.emergencyStyle?.textAlign\"\n    [style.position]=\"'absolute'\"\n    id=\"em\" #em>\n      <div\n        [style.animation]=\"configurations.configurationsObj.emergencyStyle?.disableBlinking ? 'none' : 'blinker ' + (configurations.configurationsObj.blinkSpeed || 0.5) + 's linear infinite'\"\n        *ngIf=\"preview\">\n        Emergency Message!\n      </div>\n      <div\n         [style.animation]=\"configurations.configurationsObj.emergencyStyle?.disableBlinking ? 'none' : 'blinker ' + (configurations.configurationsObj.blinkSpeed || 0.5) + 's linear infinite'\"\n         *ngIf=\"!preview\">\n        {{message}}\n      </div>\n  </div>\n</div>\n<style>\n.blink_me {\n  animation: blinker 0.5s linear infinite;\n}\n\n@keyframes blinker {  \n  50% { opacity: 0.2; }\n}\n</style>\n"
        })
    ], EmergencyMessage);
    return EmergencyMessage;
}());
exports.EmergencyMessage = EmergencyMessage;
//# sourceMappingURL=emergency-message.component.js.map