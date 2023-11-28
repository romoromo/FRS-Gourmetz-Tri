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
var PIBService = /** @class */ (function () {
    function PIBService(router, http, authService, accountEndpoint, commonEndpoint, configurations) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.accountEndpoint = accountEndpoint;
        this.commonEndpoint = commonEndpoint;
        this.configurations = configurations;
        this._pibsChanged = new rxjs_1.Subject();
        this._pibsDeviceChanged = new rxjs_1.Subject();
        this._pibUrl = "/api/pib";
        this._templateUrl = "/api/pib";
    }
    PIBService_1 = PIBService;
    Object.defineProperty(PIBService.prototype, "pibUrl", {
        get: function () { return this.configurations.baseUrl + this._pibUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(PIBService.prototype, "templateUrl", {
        get: function () { return this.configurations.baseUrl + this._templateUrl; },
        enumerable: true,
        configurable: true
    });
    PIBService.prototype.onPIBChanged = function (pibs, op) {
        this._pibsChanged.next({ pibs: pibs, operation: op });
    };
    PIBService.prototype.getPIBChangedEvent = function () {
        return this._pibsChanged.asObservable();
    };
    PIBService.prototype.onPIBDeviceChanged = function (pibs, op) {
        this._pibsDeviceChanged.next({ pibs: pibs, operation: op });
    };
    PIBService.prototype.getPIBDeviceChangedEvent = function () {
        return this._pibsDeviceChanged.asObservable();
    };
    //mapTemplate(data: PIBTemplate) {
    //  var queryParams = '';
    //  queryParams += (data ? '?template=' + data : '');
    //  return this.commonEndpoint.get<any>(this.pibUrl + '/map/template', true, data.template_body, true);
    //}
    PIBService.prototype.previewFromImage = function (data) {
        return this.commonEndpoint.getNewEndpoint(this.pibUrl + '/preview/image', data);
    };
    PIBService.prototype.mapTemplate = function (data) {
        return this.commonEndpoint.getNewEndpoint(this.pibUrl + '/map/template', data);
    };
    PIBService.prototype.postTemplateToDevice = function (url, template) {
        return this.commonEndpoint.getNewEndpoint(this.pibUrl + '/map/posttodevice', template);
    };
    PIBService.prototype.postTemplateToDevice2 = function (url, template) {
        return this.commonEndpoint.postTemplateToDevice(url, template);
    };
    PIBService.prototype.getTemplateById = function (templateId) {
        return this.commonEndpoint.get(this.templateUrl + '/get/template?templateId=' + templateId);
    };
    PIBService.prototype.getTemplateByMacAddress = function (macAddress) {
        return this.commonEndpoint.get(this.templateUrl + '/get/template?mac=' + macAddress);
    };
    PIBService.prototype.getTemplates = function (page, pageSize) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.templateUrl + '/templates/list', page, pageSize));
    };
    PIBService.prototype.getRestrictionTypes = function (page, pageSize) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.templateUrl + '/get/restrictiontypes', page, pageSize));
    };
    PIBService.prototype.syncLocations = function () {
        return this.commonEndpoint.getPagedList(this.templateUrl + '/get/piblocations/sync');
    };
    PIBService.prototype.getLocations = function (page, pageSize) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.templateUrl + '/get/piblocations', page, pageSize));
    };
    PIBService.prototype.updateTemplate = function (template) {
        var _this = this;
        if (template.id) {
            return this.commonEndpoint.getUpdateEndpoint(this.templateUrl, template, template.id).pipe(operators_1.tap(function (data) { return _this.onPIBChanged([template], PIBService_1.pibModifiedOperation); }));
        }
    };
    PIBService.prototype.newTemplate = function (template) {
        var _this = this;
        return this.commonEndpoint.getNewEndpoint(this.templateUrl, template).pipe(operators_1.tap(function (data) { return _this.onPIBChanged([template], PIBService_1.pibAddedOperation); }));
    };
    PIBService.prototype.deleteTemplate = function (templateOrId) {
        var _this = this;
        if (typeof templateOrId === 'number' || templateOrId instanceof Number ||
            typeof templateOrId === 'string' || templateOrId instanceof String) {
            return this.commonEndpoint.getDeleteEndpoint(this.templateUrl, templateOrId).pipe(operators_1.tap(function (data) { return _this.onPIBChanged([data], PIBService_1.pibDeletedOperation); }));
        }
        else {
            if (templateOrId.id) {
                return this.deleteTemplate(templateOrId.id);
            }
        }
    };
    //pib devices
    PIBService.prototype.heartbeat = function (macAddress) {
        return this.commonEndpoint.get(this.templateUrl + '/heartbeat?mac_address=' + macAddress);
    };
    PIBService.prototype.getDeviceById = function (deviceId, macAddress) {
        return this.commonEndpoint.get(this.templateUrl + '/get/device?deviceId=' + deviceId + '&mac_address=' + macAddress);
    };
    PIBService.prototype.getDevices = function (page, pageSize) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.templateUrl + '/devices/list', page, pageSize));
    };
    PIBService.prototype.updatePIBDevice = function (device) {
        var _this = this;
        if (device.id) {
            return this.commonEndpoint.getUpdateEndpoint(this.templateUrl + '/device', device, device.id).pipe(operators_1.tap(function (data) { return _this.onPIBDeviceChanged([device], PIBService_1.pibModifiedOperation); }));
        }
    };
    PIBService.prototype.newPIBDevice = function (device) {
        var _this = this;
        return this.commonEndpoint.getNewEndpoint(this.templateUrl + '/device', device).pipe(operators_1.tap(function (data) { return _this.onPIBDeviceChanged([device], PIBService_1.pibAddedOperation); }));
    };
    PIBService.prototype.deletePIBDevice = function (deviceOrId) {
        var _this = this;
        if (typeof deviceOrId === 'number' || deviceOrId instanceof Number ||
            typeof deviceOrId === 'string' || deviceOrId instanceof String) {
            return this.commonEndpoint.getDeleteEndpoint(this.templateUrl + '/device', deviceOrId).pipe(operators_1.tap(function (data) { return _this.onPIBDeviceChanged([data], PIBService_1.pibDeletedOperation); }));
        }
        else {
            if (deviceOrId.id) {
                return this.deletePIBDevice(deviceOrId.id);
            }
        }
    };
    var PIBService_1;
    PIBService.pibAddedOperation = "add";
    PIBService.pibDeletedOperation = "delete";
    PIBService.pibModifiedOperation = "modify";
    PIBService = PIBService_1 = __decorate([
        core_1.Injectable()
    ], PIBService);
    return PIBService;
}());
exports.PIBService = PIBService;
//# sourceMappingURL=pib.service.js.map