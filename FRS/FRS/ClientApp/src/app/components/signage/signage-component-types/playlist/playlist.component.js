"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var SignagePlaylist = /** @class */ (function () {
    function SignagePlaylist(sanitizer, playlistService, route, authService, configurationService, deviceService) {
        this.sanitizer = sanitizer;
        this.playlistService = playlistService;
        this.route = route;
        this.authService = authService;
        this.configurationService = configurationService;
        this.deviceService = deviceService;
        this.defaultVolume = 0;
        this.current = 0;
        this.even = false;
    }
    SignagePlaylist.prototype.ngOnInit = function () {
        var _this = this;
        console.log("playlist on INIT");
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
        this.startPlaylist();
    };
    SignagePlaylist.prototype.getData = function () {
        var _this = this;
        this.url = null;
        this.playlistService.getPlaylistById(this.configurations.configurationsObj.playlistId)
            .subscribe(function (result) {
            _this.playlist = result;
            _this.medias = result.images;
            if (_this.medias)
                _this.medias = _this.medias.sort(function (a, b) { return a.imageIndex - b.imageIndex; });
            try {
                localStorage.setItem("playlist-" + _this.configurations.configurationsObj.playlistId, JSON.stringify(_this.medias));
            }
            catch (ex) { }
        }, function (error) { });
    };
    SignagePlaylist.prototype.startPlaylist = function () {
        var _this = this;
        console.log("start playlist", this.medias);
        try {
            if (!this.medias) {
                this.medias = JSON.parse(localStorage.getItem("playlist-" + this.configurations.configurationsObj.playlistId));
            }
        }
        catch (ex) { }
        var duration = 5;
        if (!this.playlist || this.playlist.id != this.configurations.configurationsObj.playlistId)
            this.getData();
        if (this.medias && this.medias.length > 0) {
            if (this.current >= this.medias.length)
                this.current = 0;
            var m = this.medias[this.current];
            this.url = m.imageLocation;
            this.isVideo = m.isVideo;
            if (m.animation)
                this.animation = "w3-animate-" + m.animation;
            if (m.duration)
                duration = m.duration;
            this.even = !this.even;
            this.current++;
        }
        setTimeout(function () {
            _this.startPlaylist();
        }, duration * 1000);
    };
    SignagePlaylist.prototype.getDeviceInfo = function () {
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
    ], SignagePlaylist.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], SignagePlaylist.prototype, "preview", void 0);
    SignagePlaylist = __decorate([
        core_1.Component({
            selector: 'signage-playlist',
            templateUrl: './playlist.component.html',
            styleUrls: ['./playlist.component.css']
        })
    ], SignagePlaylist);
    return SignagePlaylist;
}());
exports.SignagePlaylist = SignagePlaylist;
//# sourceMappingURL=playlist.component.js.map