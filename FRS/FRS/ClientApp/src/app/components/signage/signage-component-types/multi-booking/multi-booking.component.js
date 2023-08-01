"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.SingleBooking = void 0;
var core_1 = require("@angular/core");
var SingleBooking = /** @class */ (function () {
    function SingleBooking(changeDetectorRef, authService, route, deviceService, configurationService) {
        this.changeDetectorRef = changeDetectorRef;
        this.authService = authService;
        this.route = route;
        this.deviceService = deviceService;
        this.configurationService = configurationService;
    }
    SingleBooking.prototype.ngOnInit = function () {
        var _this = this;
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
        console.log('employee');
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
            this.start();
        }
    };
    SingleBooking.prototype.start = function () {
        var _this = this;
        this.getSingleBooking();
        setTimeout(function () {
            _this.start();
        }, 5 * 60 * 1000);
    };
    SingleBooking.prototype.getSingleBooking = function () {
        if (!this.device || !this.device.locationId)
            return;
        //this.employeeDataService.getCurrentEmployeeDataByLocation(this.device.locationId)
        //.subscribe(results => {
        //  this.employees = results;
        //},
        //  error => {
        //  });
    };
    SingleBooking.prototype.getDeviceInfo = function () {
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
                    _this.getSingleBooking();
                    _this.signalRCoreconnection = _this.authService.signalRConnection(_this.configurationService.baseUrl + "/hub/employeeschedule?location_id=" + _this.device.locationId, true);
                    if (_this.signalRCoreconnection != null) {
                        _this.signalRCoreconnection.on("UpdateSchedule", function (param) {
                            _this.getSingleBooking();
                        });
                    }
                }
            });
        }
    };
    __decorate([
        core_1.Input()
    ], SingleBooking.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], SingleBooking.prototype, "preview", void 0);
    SingleBooking = __decorate([
        core_1.Component({
            selector: 'single-booking',
            template: "\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.contactNo\" [previewText]=\"'Contact'\" [actualText]=\"reservation?.createdByName\"></text-display>\n\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.description\" [previewText]=\"'Description'\" [actualText]=\"reservation?.shortDescription\"></text-display>\n\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.location\" [previewText]=\"'Location'\" [actualText]=\"reservation?.locationName\"></text-display>\n\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.time\" [previewText]=\"'Time'\" [actualText]=\"time\"></text-display>\n\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.meetingPurpose\" [previewText]=\"'Meeting Purpose'\" [actualText]=\"reservation?.longDescription\"></text-display>\n"
        })
    ], SingleBooking);
    return SingleBooking;
}());
exports.SingleBooking = SingleBooking;
//# sourceMappingURL=single-booking.component.js.map