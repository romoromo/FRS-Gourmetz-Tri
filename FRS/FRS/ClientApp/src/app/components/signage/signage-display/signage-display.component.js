"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.SignageDisplayComponent = void 0;
var core_1 = require("@angular/core");
var rxjs_1 = require("rxjs");
var SignageDisplayComponent = /** @class */ (function () {
    function SignageDisplayComponent(route, deviceService, signagePublicationService, signageComponentTypeService, authService, configurations, applicationSetting) {
        this.route = route;
        this.deviceService = deviceService;
        this.signagePublicationService = signagePublicationService;
        this.signageComponentTypeService = signageComponentTypeService;
        this.authService = authService;
        this.configurations = configurations;
        this.applicationSetting = applicationSetting;
        this.subscription = new rxjs_1.Subscription();
        this.mac_address = '';
        this.publication_id = '';
        this.message = "Unable to load display.";
    }
    SignageDisplayComponent.prototype.getTimeString = function (time) {
        return ("0" + time.getHours()).slice(-2) + ":" +
            ("0" + time.getMinutes()).slice(-2) + ":" +
            ("0" + time.getSeconds()).slice(-2);
    };
    SignageDisplayComponent.prototype.getEndTimeSeconds = function () {
        if (this.schedule.endTime) {
            var endTimeObj = new Date(this.schedule.endTime);
            var now = new Date();
            var end = new Date(now.getFullYear(), now.getMonth(), now.getDate(), endTimeObj.getHours(), endTimeObj.getMinutes(), endTimeObj.getSeconds());
            var dif = end.getTime() - new Date().getTime();
            var sec = Math.floor(dif / 1000);
            if (sec > 0)
                return sec;
        }
    };
    SignageDisplayComponent.prototype.startInterval = function (index) {
        var _this = this;
        if (index > this.compilationList.length - 1)
            index = 0;
        this.compilation = this.compilationList[index];
        var interval = null;
        if (this.compilation.interval)
            interval = this.compilation.interval;
        var sec = this.getEndTimeSeconds();
        if ((sec && !interval) || (sec && interval && interval > sec)) {
            setTimeout(function () {
                _this.loadData();
            }, sec * 1000);
        }
        else if (interval) {
            setTimeout(function () {
                _this.startInterval(index + 1);
            }, interval * 1000);
        }
    };
    SignageDisplayComponent.prototype.start = function (publication) {
        var _this = this;
        if (publication.schedules) {
            for (var _i = 0, _a = publication.schedules; _i < _a.length; _i++) {
                var s = _a[_i];
                var now = new Date();
                if (s.effectiveDate && s.ineffectiveDate) {
                    s.effectiveDateObj = new Date(s.effectiveDate.split("T")[0]);
                    s.ineffectiveDateObj = new Date(s.ineffectiveDate.split("T")[0]);
                    if (s.effectiveDateObj <= now && s.ineffectiveDateObj >= now) {
                        if (s.startTime && s.endTime) {
                            s.startTimeObj = s.startTime.split("T")[1];
                            s.endTimeObj = s.endTime.split("T")[1];
                            var nowTime = this.getTimeString(now);
                            if (s.startTimeObj <= nowTime && s.endTimeObj >= nowTime) {
                                var day = now.getDay();
                                if ((day == 0 && s.sunday) ||
                                    (day == 1 && s.monday) ||
                                    (day == 2 && s.tuesday) ||
                                    (day == 3 && s.wednesday) ||
                                    (day == 4 && s.thursday) ||
                                    (day == 5 && s.friday) ||
                                    (day == 6 && s.saturday)) {
                                    this.schedule = s;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (this.schedule || this.preview) {
            if (this.preview && !this.schedule && publication.schedules && publication.schedules[0])
                this.schedule = publication.schedules[0];
            if (this.schedule.compilations && this.schedule.compilations[0]) {
                this.compilationList = this.schedule.compilations;
                for (var _b = 0, _c = this.compilationList; _b < _c.length; _b++) {
                    var c = _c[_b];
                    if (c.backgroundImage)
                        c.backgroundImage = c.backgroundImage.replace(/\\/g, '/');
                    for (var _d = 0, _e = c.components; _d < _e.length; _d++) {
                        var p = _e[_d];
                        if (p.backgroundImage)
                            p.backgroundImage = p.backgroundImage.replace(/\\/g, '/');
                    }
                }
                this.startInterval(0);
            }
            else {
                this.message = "Current schedule does not have any compilation.";
                setTimeout(function () { return _this.start(_this.publication); }, 5 * 60 * 1000);
            }
        }
        else {
            this.message = "No schedule match for current time.";
            setTimeout(function () { return _this.start(_this.publication); }, 5 * 60 * 1000);
        }
    };
    SignageDisplayComponent.prototype.getPublication = function (publicationId) {
        var _this = this;
        this.publication_id = publicationId;
        this.signagePublicationService.getSignagePublication(publicationId).subscribe(function (p) {
            _this.publication = p;
            if (_this.publication) {
                if (_this.publication.approved || _this.preview)
                    _this.start(_this.publication);
                else
                    _this.message = "This publication '" + _this.publication.name + "' has not been approved.";
            }
            else {
                _this.message = "No publication data found.";
            }
        });
    };
    SignageDisplayComponent.prototype.loadData = function () {
        var _this = this;
        if (this.publication_id)
            this.getPublication(this.publication_id);
        this.deviceService.getDeviceById(null, this.mac_address, true)
            .subscribe(function (results) {
            _this.device = results.data;
            if (!_this.publication_id) {
                if (_this.device) {
                    if (_this.device.publicationId) {
                        _this.getPublication(_this.device.publicationId);
                    }
                    else {
                        _this.message = "This device has not been assigned publication.";
                    }
                }
                else {
                    _this.message = "No device data found.";
                }
            }
            _this.initSignalR();
        }, function (error) {
            _this.message = "Error Fetching Device.";
        });
    };
    SignageDisplayComponent.prototype.initSignalR = function () {
        var _this = this;
        if (this.device) {
            this.signalRCoreconnection = this.authService.signalRConnection(this.configurations.baseUrl + "/hub/frsdevice?device_id=" + this.device.id + "&source=server", true, this.signalRCoreconnection);
            if (this.signalRCoreconnection != null) {
                this.signalRCoreconnection.on("RefreshDeviceData", function (deviceVM) {
                    if (_this.device.moduleId != deviceVM.moduleId)
                        window.location.replace(window.location.origin + '/display/' + _this.mac_address);
                    else if (_this.device.publicationId != deviceVM.publicationId || _this.device.locationId != deviceVM.locationId)
                        window.location.replace(window.location.origin + '/display/' + _this.mac_address);
                });
                this.signalRCoreconnection.on("RefreshPanel", function (deviceVM) {
                    console.log('Refreshing Panel');
                    window.location.reload(true);
                });
            }
        }
        else {
            this.subscription.add(this.applicationSetting.getApplicationSettingByKey(null, 'URL_PIB_SERVER').subscribe(function (res) {
                if (res) {
                    _this.subscription.add(_this.deviceService.getDeviceById(null, _this.mac_address, false, res.value + '/api/pib/get/device').subscribe(function (result) {
                        if (result && result.data) {
                            var device_1 = result.data;
                            if (!device_1.module_path_value) {
                                location.replace(device_1.module_path);
                            }
                            _this.pibSignalRCoreconnection = _this.authService.signalRConnection(res.value + "/hub/pibdevice?device_id=" + device_1.id, true, _this.pibSignalRCoreconnection);
                            if (_this.pibSignalRCoreconnection != null) {
                                _this.pibSignalRCoreconnection.on("RefreshDeviceData", function (deviceVM) {
                                    if (device_1.moduleId != deviceVM.moduleId)
                                        window.location.replace(res.value + '/display/' + _this.mac_address);
                                    else if (device_1.publicationId != deviceVM.publicationId || device_1.locationId != deviceVM.locationId)
                                        window.location.replace(window.location.origin + '/display/' + _this.mac_address);
                                });
                                _this.pibSignalRCoreconnection.on("RefreshPanel", function (deviceVM) {
                                    console.log('Refreshing Panel');
                                    window.location.reload(true);
                                });
                            }
                        }
                    }, function (error) { }));
                }
            }, function (error) { return function (error) {
                this.alertService.stopLoadingMessage();
                console.log(error);
            }; }));
        }
    };
    SignageDisplayComponent.prototype.ngOnInit = function () {
        var _this = this;
        try {
            document.body.style.overflow = 'hidden';
        }
        catch (ex) { }
        this.componentTypeMap = this.signageComponentTypeService.getComponentTypeMap();
        this.route.params.subscribe(function (queryParams) {
            _this.mac_address = queryParams["mac_address"];
        });
        if (!this.mac_address) {
            this.route.queryParams.subscribe(function (queryParams) {
                _this.mac_address = queryParams["mac_address"];
            });
        }
        if (this.mac_address == 'serverpreview')
            this.preview = true;
        this.route.params.subscribe(function (queryParams) {
            _this.publication_id = queryParams["publication_id"];
        });
        if (!this.publication_id) {
            this.route.queryParams.subscribe(function (queryParams) {
                _this.publication_id = queryParams["publication_id"];
            });
        }
        this.loadData();
    };
    SignageDisplayComponent = __decorate([
        core_1.Component({
            selector: 'signage-display',
            templateUrl: "./signage-display.component.html",
            styleUrls: ['./signage-display.component.css']
        })
    ], SignageDisplayComponent);
    return SignageDisplayComponent;
}());
exports.SignageDisplayComponent = SignageDisplayComponent;
//# sourceMappingURL=signage-display.component.js.map