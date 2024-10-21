"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.ApplicationSettingService = void 0;
var core_1 = require("@angular/core");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var ApplicationSettingService = /** @class */ (function () {
    function ApplicationSettingService(router, http, authService, commonEndpoint, configurations) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.commonEndpoint = commonEndpoint;
        this.configurations = configurations;
        this._applicationSettingsChanged = new rxjs_1.Subject();
        this._applicationSettingUrl = "/api/applicationsetting";
    }
    ApplicationSettingService_1 = ApplicationSettingService;
    Object.defineProperty(ApplicationSettingService.prototype, "applicationSettingUrl", {
        get: function () { return this.configurations.baseUrl + this._applicationSettingUrl; },
        enumerable: false,
        configurable: true
    });
    ApplicationSettingService.prototype.onApplicationSettingsChanged = function (applicationSettings, op) {
        this._applicationSettingsChanged.next({ applicationSettings: applicationSettings, operation: op });
    };
    ApplicationSettingService.prototype.onApplicationSettingsCountChanged = function (applicationSettings) {
        return this.onApplicationSettingsChanged(applicationSettings, ApplicationSettingService_1.applicationSettingModifiedOperation);
    };
    ApplicationSettingService.prototype.getApplicationSettingsChangedEvent = function () {
        return this._applicationSettingsChanged.asObservable();
    };
    ApplicationSettingService.prototype.getApplicationSettings = function (page, pageSize) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.applicationSettingUrl + '/applicationsettings/list', page, pageSize));
    };
    ApplicationSettingService.prototype.getApplicationSettingByInstitutionId = function (institutionId) {
        return rxjs_1.forkJoin(this.commonEndpoint.getByInstitutionId(this.applicationSettingUrl + '/applicationsettings/list', institutionId));
    };
    ApplicationSettingService.prototype.getApplicationSettingByKey = function (institutionId, key) {
        return this.commonEndpoint.get(this.applicationSettingUrl + '/get?institutionId=' + institutionId + '&key=' + key);
    };
    ApplicationSettingService.prototype.updateApplicationSetting = function (applicationSetting) {
        var _this = this;
        return this.commonEndpoint.getUpdateEndpoint(this.applicationSettingUrl, applicationSetting, applicationSetting.id).pipe(operators_1.tap(function (data) { return _this.onApplicationSettingsChanged([applicationSetting], ApplicationSettingService_1.applicationSettingModifiedOperation); }));
    };
    ApplicationSettingService.prototype.newApplicationSetting = function (applicationSetting) {
        var _this = this;
        return this.commonEndpoint.getNewEndpoint(this.applicationSettingUrl, applicationSetting).pipe(operators_1.tap(function (data) { return _this.onApplicationSettingsChanged([applicationSetting], ApplicationSettingService_1.applicationSettingAddedOperation); }));
    };
    ApplicationSettingService.prototype.deleteApplicationSetting = function (applicationsettingId) {
        var _this = this;
        return this.commonEndpoint.getDeleteEndpoint(this.applicationSettingUrl, applicationsettingId).pipe(operators_1.tap(function (data) { return _this.onApplicationSettingsChanged([data], ApplicationSettingService_1.applicationSettingDeletedOperation); }));
    };
    var ApplicationSettingService_1;
    ApplicationSettingService.applicationSettingAddedOperation = "add";
    ApplicationSettingService.applicationSettingDeletedOperation = "delete";
    ApplicationSettingService.applicationSettingModifiedOperation = "modify";
    ApplicationSettingService = ApplicationSettingService_1 = __decorate([
        core_1.Injectable()
    ], ApplicationSettingService);
    return ApplicationSettingService;
}());
exports.ApplicationSettingService = ApplicationSettingService;
//# sourceMappingURL=application-setting.service.js.map