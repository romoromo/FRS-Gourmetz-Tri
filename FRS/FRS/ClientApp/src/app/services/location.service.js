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
var LocationService = /** @class */ (function () {
    function LocationService(router, http, authService, accountEndpoint, commonEndpoint, configurationService) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.accountEndpoint = accountEndpoint;
        this.commonEndpoint = commonEndpoint;
        this.configurationService = configurationService;
        this._locationsChanged = new rxjs_1.Subject();
        this._locationUrl = "/api/location";
        this._institutionUrl = "/api/institution";
        this._simpleresultUrl = "/api/simpleresult";
    }
    LocationService_1 = LocationService;
    Object.defineProperty(LocationService.prototype, "locationUrl", {
        get: function () { return this.configurationService.baseUrl + this._locationUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(LocationService.prototype, "institutionUrl", {
        get: function () { return this.configurationService.baseUrl + this._institutionUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(LocationService.prototype, "simpleresultUrl", {
        get: function () { return this.configurationService.baseUrl + this._simpleresultUrl; },
        enumerable: true,
        configurable: true
    });
    LocationService.prototype.onLocationsChanged = function (locations, op) {
        this._locationsChanged.next({ locations: locations, operation: op });
    };
    LocationService.prototype.onLocationsUserCountChanged = function (locations) {
        return this.onLocationsChanged(locations, LocationService_1.locationModifiedOperation);
    };
    LocationService.prototype.getLocationsChangedEvent = function () {
        return this._locationsChanged.asObservable();
    };
    LocationService.prototype.getLocations = function (page, pageSize, institutionId, isBooking) {
        return this.commonEndpoint.getPagedList(this.locationUrl + '/locations/list?institutionId=' + institutionId + '&isBooking=' + isBooking, page, pageSize);
    };
    LocationService.prototype.getLocationsByUser = function (userId, isBooking) {
        return this.commonEndpoint.getPagedList(this.locationUrl + '/signage/' + userId + '/' + isBooking, null, null);
    };
    LocationService.prototype.getLocationById = function (id) {
        return this.commonEndpoint.getById(this.locationUrl, id);
    };
    LocationService.prototype.getLocationTree = function (filter) {
        return this.commonEndpoint.getLocationTree(this.locationUrl + '/locationtree', filter);
    };
    LocationService.prototype.getLocationsAndInstitutions = function (page, pageSize, institutionId) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.locationUrl + '/locations/list?institutionId=' + institutionId, page, pageSize), this.commonEndpoint.getPagedList(this.institutionUrl + '/institutions/list'));
    };
    LocationService.prototype.getLocationsAndFacilities = function (institutionId) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.locationUrl + '/locations/list?institutionId=' + institutionId), this.commonEndpoint.getFacilitiesEndpoint(null, null, institutionId));
    };
    LocationService.prototype.getLocationsByInstitutionId = function (id) {
        return this.commonEndpoint.getPagedList(this.locationUrl + '/locations/list?institutionId=' + id.toString());
    };
    LocationService.prototype.getAvailableLocations = function (filter) {
        return this.commonEndpoint.getReservations(this.locationUrl + '/GetAvailableLocations', filter);
    };
    LocationService.prototype.getCalendarLocations = function (filter) {
        return this.commonEndpoint.getReservations(this.locationUrl + '/getcalendarlocations', filter);
    };
    LocationService.prototype.getLocationTypes = function () {
        return this.commonEndpoint.getPagedList(this.locationUrl + '/locationtypes');
    };
    LocationService.prototype.updateLocation = function (location) {
        var _this = this;
        if (location.id) {
            return this.commonEndpoint.getUpdateEndpoint(this.locationUrl, location, location.id).pipe(operators_1.tap(function (data) { return _this.onLocationsChanged([location], LocationService_1.locationModifiedOperation); }));
        }
    };
    LocationService.prototype.newLocation = function (location) {
        var _this = this;
        return this.commonEndpoint.getNewEndpoint(this.locationUrl, location).pipe(operators_1.tap(function (data) { return _this.onLocationsChanged([location], LocationService_1.locationAddedOperation); }));
    };
    LocationService.prototype.deleteLocation = function (locationOrLocationId) {
        var _this = this;
        if (typeof locationOrLocationId === 'number' || locationOrLocationId instanceof Number ||
            typeof locationOrLocationId === 'string' || locationOrLocationId instanceof String) {
            return this.commonEndpoint.getDeleteEndpoint(this.locationUrl, locationOrLocationId).pipe(operators_1.tap(function (data) { return _this.onLocationsChanged([data], LocationService_1.locationDeletedOperation); }));
        }
        else {
            if (locationOrLocationId.id) {
                return this.deleteLocation(locationOrLocationId.id);
            }
        }
    };
    LocationService.prototype.importFile = function (data) {
        return this.commonEndpoint.importFile(this.locationUrl + '/importtree?institutionId=' + this.authService.currentUser.institutionId, data);
    };
    LocationService.prototype.syncSmartRoomResources = function (institutionId) {
        return this.commonEndpoint.get(this.locationUrl + ("/syncSmartRoomResources?institutionId=" + institutionId));
    };
    LocationService.prototype.syncSmartRoomSchedules = function (institutionId) {
        return this.commonEndpoint.get(this.locationUrl + ("/syncSmartRoomSchedules?institutionId=" + institutionId));
    };
    var LocationService_1;
    LocationService.locationAddedOperation = "add";
    LocationService.locationDeletedOperation = "delete";
    LocationService.locationModifiedOperation = "modify";
    LocationService = LocationService_1 = __decorate([
        core_1.Injectable()
    ], LocationService);
    return LocationService;
}());
exports.LocationService = LocationService;
//# sourceMappingURL=location.service.js.map