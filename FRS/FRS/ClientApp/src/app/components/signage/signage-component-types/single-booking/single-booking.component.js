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
var reservation_model_1 = require("../../../../models/reservation.model");
var utilities_1 = require("../../../../services/utilities");
var common_1 = require("@angular/common");
var SingleBooking = /** @class */ (function () {
    function SingleBooking(changeDetectorRef, authService, route, deviceService, reservationService, configurationService) {
        this.changeDetectorRef = changeDetectorRef;
        this.authService = authService;
        this.route = route;
        this.deviceService = deviceService;
        this.reservationService = reservationService;
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
        var _this = this;
        if (!this.device || !this.device.locationId)
            return;
        this.filter = new reservation_model_1.CalendarFilter();
        this.filter.institutionId = this.institutionId;
        this.filter.locationIds = [this.device.locationId];
        this.filter.isBooking = true;
        this.filter.start = new Date();
        this.filter.start = new Date(this.filter.start.setHours(0, 0, 0, 0));
        this.filter.startTime = new Date(this.filter.start.setHours(0, 0, 0, 0));
        this.filter.endTime = new Date(this.filter.start.setHours(23, 59, 59, 0));
        this.reservationService.getCalendarEvents(this.filter).subscribe(function (events) {
            _this.calendarEvents = [];
            var currentTime = utilities_1.Utilities.getCurrentDate(true);
            events.forEach(function (event, index, events) {
                //event.start = new Date(event.start);
                //event.end = new Date(event.end);
                if (event.meta != null && event.meta.reservation != null && event.start <= currentTime
                    && event.end >= currentTime) {
                    _this.calendarEvents.push(event);
                }
            });
            _this.reservation = null;
            _this.time = null;
            if (_this.calendarEvents[0]) {
                _this.reservation = _this.calendarEvents[0];
                var locale = 'en-SG';
                _this.time = common_1.formatDate(_this.reservation.start, 'hh:mm aa', locale) + " - " + common_1.formatDate(_this.reservation.end, 'hh:mm aa', locale);
            }
        });
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
                    _this.institutionId = _this.device.institutionId;
                    _this.getSingleBooking();
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
            template: "\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.contactNo\" [previewText]=\"'Contact'\" [actualText]=\"reservation?.meta?.reservation?.createdByName\"></text-display>\n\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.description\" [previewText]=\"'Description'\" [actualText]=\"reservation?.meta?.reservation?.shortDescription\"></text-display>\n\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.location\" [previewText]=\"'Location'\" [actualText]=\"reservation?.meta?.location?.name\"></text-display>\n\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.time\" [previewText]=\"'Time'\" [actualText]=\"time\"></text-display>\n\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.meetingPurpose\" [previewText]=\"'Meeting Purpose'\" [actualText]=\"reservation?.meta?.reservation?.longDescription\"></text-display>\n"
        })
    ], SingleBooking);
    return SingleBooking;
}());
exports.SingleBooking = SingleBooking;
//# sourceMappingURL=single-booking.component.js.map