"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var ReservationService = /** @class */ (function () {
    function ReservationService(router, http, authService, accountEndpoint, commonEndpoint, locationService, configurations) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.accountEndpoint = accountEndpoint;
        this.commonEndpoint = commonEndpoint;
        this.locationService = locationService;
        this.configurations = configurations;
        this._reservationUrl = "/api/reservation";
        this._reservationsChanged = new rxjs_1.Subject();
    }
    ReservationService_1 = ReservationService;
    Object.defineProperty(ReservationService.prototype, "reservationUrl", {
        get: function () { return this.configurations.baseUrl + this._reservationUrl; },
        enumerable: true,
        configurable: true
    });
    ReservationService.prototype.onReservationsChanged = function (reservations, op) {
        this._reservationsChanged.next({ reservations: reservations, operation: op });
    };
    ReservationService.prototype.onReservationsUserCountChanged = function (reservations) {
        return this.onReservationsChanged(reservations, ReservationService_1.reservationModifiedOperation);
    };
    ReservationService.prototype.getReservationsChangedEvent = function () {
        return this._reservationsChanged.asObservable();
    };
    ReservationService.prototype.getTimeIntervals = function (filter) {
        return this.commonEndpoint.getPagedList(this.reservationUrl + '/GetAllTimeIntervals', null, null, true, filter);
    };
    ReservationService.prototype.getBookingGridRows = function (filter) {
        return this.commonEndpoint.getPagedList(this.reservationUrl + '/GetBookingGridRows', null, null, true, filter);
    };
    ReservationService.prototype.getReservationsLocationsFacilities = function (page, pageSize, filter) {
        var institutionId = filter != null ? filter.institutionId : null;
        var isBooking = filter != null ? filter.isBooking : null;
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.reservationUrl + '/GetAllReservations', page, pageSize, true, filter), this.commonEndpoint.getFacilitiesEndpoint(null, null, institutionId), this.locationService.getLocations(null, null, institutionId, isBooking));
    };
    ReservationService.prototype.getCalendarFilters = function (filter) {
        var institutionId = filter != null ? filter.institutionId : null;
        var isBooking = filter != null ? filter.isBooking : null;
        return rxjs_1.forkJoin(this.commonEndpoint.getFacilitiesEndpoint(null, null, institutionId), this.locationService.getLocations(null, null, institutionId, isBooking));
    };
    ReservationService.prototype.getReservationsWithFilter = function (filter) {
        return this.commonEndpoint.getReservations(this.reservationUrl + '/GetAllReservations', filter);
    };
    ReservationService.prototype.getReservation = function (id) {
        return this.commonEndpoint.get(this.reservationUrl + '/get' + "?id=" + id);
    };
    ReservationService.prototype.getKioskReservation = function () {
        return this.commonEndpoint.get(this.reservationUrl + '/GetKiosk');
    };
    ReservationService.prototype.getReservations = function (page, pageSize) {
        return this.commonEndpoint.getPagedList(this.reservationUrl + '/GetAllReservations', page, pageSize, true);
    };
    ReservationService.prototype.getCalendarEvents = function (filter) {
        return this.commonEndpoint.getReservations(this.reservationUrl + '/calendarevents', filter);
    };
    ReservationService.prototype.updateReservation = function (reservation) {
        var _this = this;
        if (reservation.id) {
            return this.commonEndpoint.getUpdateEndpoint(this.reservationUrl, reservation, reservation.id).pipe(operators_1.tap(function (data) { return _this.onReservationsChanged([reservation], ReservationService_1.reservationModifiedOperation); }));
        }
    };
    ReservationService.prototype.newReservation = function (reservation) {
        var _this = this;
        return this.commonEndpoint.getNewEndpoint(this.reservationUrl, reservation).pipe(operators_1.tap(function (data) { return _this.onReservationsChanged([reservation], ReservationService_1.reservationAddedOperation); }));
    };
    ReservationService.prototype.deleteReservation = function (reservationOrReservationId) {
        var _this = this;
        if (typeof reservationOrReservationId === 'number' || reservationOrReservationId instanceof Number ||
            typeof reservationOrReservationId === 'string' || reservationOrReservationId instanceof String
            || typeof reservationOrReservationId == 'object') {
            var queryStrings = "";
            var id = "";
            if (typeof reservationOrReservationId == 'object') {
                queryStrings += "recurApplyChangesType=" + reservationOrReservationId.recurApplyChangesType;
                id = reservationOrReservationId.id;
            }
            else {
                id = reservationOrReservationId;
            }
            return this.commonEndpoint.getDeleteEndpoint(this.reservationUrl, id, queryStrings).pipe(operators_1.tap(function (data) { return _this.onReservationsChanged([data], ReservationService_1.reservationDeletedOperation); }));
        }
        //else {
        //  if (reservationOrReservationId.id) {
        //    return this.deleteReservation(reservationOrReservationId.id);
        //  }
        //}
    };
    ReservationService.prototype.validateAttendance = function (reservationId, userName, name, company, designation) {
        return this.commonEndpoint.get(this.reservationUrl + '/checkin' + "?reservationId=" + reservationId + '&userName=' + userName + '&name=' + name + '&company=' + company + '&designation=' + designation);
    };
    ReservationService.prototype.validateFeedback = function (feedback) {
        return this.commonEndpoint.get(this.reservationUrl + '/feedback' + "?reservationId=" + feedback.reservationId + '&userId=' + feedback.userId + '&feedback=' + feedback.feedback.name + '&comment=' + feedback.comment);
    };
    ReservationService.prototype.getCurrentLocationDetail = function (locationId, start, end) {
        return this.commonEndpoint.get(this.reservationUrl + '/locationcurrentdetail?locationId=' + locationId + '&start=' + start + '&end=' + end);
    };
    ReservationService.prototype.getVehicleLogs = function (filter) {
        return this.commonEndpoint.get(this.reservationUrl + '/GetVehicleLogs', true, filter);
    };
    var ReservationService_1;
    ReservationService.reservationAddedOperation = "add";
    ReservationService.reservationDeletedOperation = "delete";
    ReservationService.reservationModifiedOperation = "modify";
    ReservationService = ReservationService_1 = __decorate([
        core_1.Injectable()
    ], ReservationService);
    return ReservationService;
}());
exports.ReservationService = ReservationService;
//# sourceMappingURL=reservation.service.js.map