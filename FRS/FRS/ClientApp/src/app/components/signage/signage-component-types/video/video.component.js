"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var Video = /** @class */ (function () {
    function Video(sanitizer, route, authService, configurationService, deviceService) {
        this.sanitizer = sanitizer;
        this.route = route;
        this.authService = authService;
        this.configurationService = configurationService;
        this.deviceService = deviceService;
        this.defaultVolume = 0;
    }
    Video.prototype.ngOnInit = function () {
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
            this.getDeviceInfo();
        }
    };
    Video.prototype.ngOnChanges = function (changes) {
        console.log("HTML Changes");
    };
    Video.prototype.getDeviceInfo = function () {
        var _this = this;
        if (this.mac_address) {
            this.deviceService.getDeviceById(null, this.mac_address, true)
                .subscribe(function (results) {
                _this.device = results.data;
                if (_this.device) {
                    _this.locationColorTheme = _this.device.locationColorTheme;
                    _this.locationName = _this.device.locationName;
                    _this.defaultVolume = _this.device.defaultVolume || 0;
                    _this.signalRCoreconnection = _this.authService.signalRConnection(_this.configurationService.baseUrl + "/hub/frsdevice?device_id=" + _this.device.id + "&source=server", true, _this.signalRCoreconnection);
                    if (_this.signalRCoreconnection != null) {
                        _this.signalRCoreconnection.on("RefreshDeviceData", function (deviceVM) {
                            console.log('Refreshing Device Display');
                            _this.defaultVolume = deviceVM.defaultVolume || 0;
                        });
                    }
                }
            });
        }
    };
    __decorate([
        core_1.Input()
    ], Video.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], Video.prototype, "preview", void 0);
    Video = __decorate([
        core_1.Component({
            selector: 'signage-video',
            template: "<app-vjs-player [options]=\"{ autoplay: 'any', loop: true, controls: false, preload: 'auto', muted: false, volume: defaultVolume, fluid: true, sources: [{ src: configurations.configurationsObj.url, type: configurations.configurationsObj.type }]}\"></app-vjs-player>"
        })
    ], Video);
    return Video;
}());
exports.Video = Video;
//# sourceMappingURL=video.component.js.map