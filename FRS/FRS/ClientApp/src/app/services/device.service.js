"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.DeviceService = void 0;
var core_1 = require("@angular/core");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var DeviceService = /** @class */ (function () {
    function DeviceService(router, http, authService, accountEndpoint, commonEndpoint, locationService, configurations) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.accountEndpoint = accountEndpoint;
        this.commonEndpoint = commonEndpoint;
        this.locationService = locationService;
        this.configurations = configurations;
        //public static readonly deviceApprovedOperation: DevicesChangedOperation = "approved";
        this._devicesChanged = new rxjs_1.Subject();
        this._deviceUrl = "/api/devicemanager";
        this._institutionUrl = "/api/institution";
        this._deviceTypeUrl = "/api/devicetype/devicetypes";
    }
    DeviceService_1 = DeviceService;
    Object.defineProperty(DeviceService.prototype, "deviceUrl", {
        get: function () { return this.configurations.baseUrl + this._deviceUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(DeviceService.prototype, "institutionUrl", {
        get: function () { return this.configurations.baseUrl + this._institutionUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(DeviceService.prototype, "deviceTypeUrl", {
        get: function () { return this.configurations.baseUrl + this._deviceTypeUrl; },
        enumerable: false,
        configurable: true
    });
    DeviceService.prototype.onDevicesChanged = function (devices, op) {
        this._devicesChanged.next({ devices: devices, operation: op });
    };
    DeviceService.prototype.onDevicesUserCountChanged = function (devices) {
        return this.onDevicesChanged(devices, DeviceService_1.deviceModifiedOperation);
    };
    DeviceService.prototype.getDeviceByIdentifier = function (macAddress) {
        return this.commonEndpoint.get(this.deviceUrl + '/get/' + macAddress);
    };
    //getDeviceById(deviceId?: string, macAddress?: string) {
    //  return this.commonEndpoint.get<any>(this.deviceUrl + '/get/device?deviceId=' + deviceId + '&mac_address=' + macAddress);
    //}
    DeviceService.prototype.getDevicesChangedEvent = function () {
        return this._devicesChanged.asObservable();
    };
    DeviceService.prototype.getDevicesByFilter = function (filter) {
        return this.commonEndpoint.getSieve(this.deviceUrl + '/devices/sieve/list', filter);
    };
    DeviceService.prototype.getDevicesLocationsInstitutionsByFilter = function (filter) {
        return rxjs_1.forkJoin(this.commonEndpoint.getSieve(this.deviceUrl + '/devices/sieve/list', filter), this.locationService.getLocations(null, null, filter.institutionId), this.commonEndpoint.getPagedList(this.institutionUrl + '/institutions/list'));
    };
    DeviceService.prototype.getDevicesLocationsInstitutions = function (page, pageSize, institutionId) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.deviceUrl + '/devices/list?institutionId=' + institutionId, page, pageSize), this.locationService.getLocations(null, null, institutionId), this.commonEndpoint.getPagedList(this.institutionUrl + '/institutions/list'));
    };
    DeviceService.prototype.getDeviceById = function (deviceId, macAddress, useIP, url) {
        if (url) {
            console.log(url + '?deviceId=' + deviceId + '&mac_address=' + macAddress + '&useIP=' + useIP);
            return this.commonEndpoint.get(url + '?deviceId=' + deviceId + '&mac_address=' + macAddress + '&useIP=' + useIP, false, null, false, true);
        }
        return this.commonEndpoint.get(this.deviceUrl + '/get/device?deviceId=' + deviceId + '&mac_address=' + macAddress + '&useIP=' + useIP);
    };
    DeviceService.prototype.updateDevice = function (device) {
        var _this = this;
        return this.commonEndpoint.getUpdateEndpoint(this.deviceUrl, device, device.id).pipe(operators_1.tap(function (data) { return _this.onDevicesChanged([device], DeviceService_1.deviceModifiedOperation); }));
    };
    DeviceService.prototype.newDevice = function (device) {
        var _this = this;
        return this.commonEndpoint.getNewEndpoint(this.deviceUrl, device).pipe(operators_1.tap(function (data) { return _this.onDevicesChanged([device], DeviceService_1.deviceAddedOperation); }));
    };
    DeviceService.prototype.deleteDevice = function (deviceOrDeviceId) {
        var _this = this;
        if (typeof deviceOrDeviceId === 'number' || deviceOrDeviceId instanceof Number ||
            typeof deviceOrDeviceId === 'string' || deviceOrDeviceId instanceof String) {
            return this.commonEndpoint.getDeleteEndpoint(this.deviceUrl, deviceOrDeviceId).pipe(operators_1.tap(function (data) { return _this.onDevicesChanged([data], DeviceService_1.deviceDeletedOperation); }));
        }
        else {
            if (deviceOrDeviceId.id) {
                return this.deleteDevice(deviceOrDeviceId.id);
            }
        }
    };
    DeviceService.prototype.pushMessages = function (data) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/pushmessages', true, data);
    };
    DeviceService.prototype.bulkReboot = function (filter) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/bulkreboot', true, filter);
    };
    DeviceService.prototype.bulkRefresh = function (filter) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/bulkRefresh', true, filter);
    };
    //approveDevice(device: Device) {
    //  return this.commonEndpoint.getNewEndpoint<Device>(this.deviceUrl + '/approve', device).pipe<Device>(
    //    tap(data => this.onDevicesChanged([device], DeviceService.deviceApprovedOperation)));
    //}
    DeviceService.prototype.getDataByUrl = function (url, isPost, obj) {
        return this.commonEndpoint.get(url, isPost, obj, false, true);
    };
    DeviceService.prototype.getServerTime = function () {
        return this.commonEndpoint.get(this.deviceUrl + '/get/servertime');
    };
    DeviceService.prototype.reboot = function (id) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/reboot/' + id);
    };
    DeviceService.prototype.channelup = function (id) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/channelup/' + id);
    };
    DeviceService.prototype.changechannel = function (id, channelId) {
        return this.commonEndpoint.get(this.deviceUrl + ("/device/changechannel/" + id + "/" + channelId));
    };
    DeviceService.prototype.channeldown = function (id) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/channeldown/' + id);
    };
    DeviceService.prototype.screenon = function (id) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/screenon/' + id);
    };
    DeviceService.prototype.screenoff = function (id) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/screenoff/' + id);
    };
    DeviceService.prototype.off = function (id) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/off/' + id);
    };
    DeviceService.prototype.on = function (id) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/on/' + id);
    };
    DeviceService.prototype.refresh = function (id) {
        return this.commonEndpoint.get(this.deviceUrl + '/device/refresh/' + id);
    };
    //device type
    DeviceService.prototype.getDeviceTypeById = function (deviceTypeId) {
        return this.commonEndpoint.getById(this.deviceTypeUrl + '/get', deviceTypeId);
    };
    DeviceService.prototype.getDeviceTypesByFilter = function (filter) {
        return this.commonEndpoint.getSieve(this.deviceTypeUrl + '/sieve/list', filter);
    };
    DeviceService.prototype.updateDeviceType = function (deviceType) {
        if (deviceType.id) {
            return this.commonEndpoint.getUpdateEndpoint(this.deviceTypeUrl, deviceType, deviceType.id);
        }
    };
    DeviceService.prototype.newDeviceType = function (deviceType) {
        return this.commonEndpoint.getNewEndpoint(this.deviceTypeUrl, deviceType);
    };
    DeviceService.prototype.deleteDeviceType = function (deviceTypeOrDeviceTypeId) {
        return this.commonEndpoint.getDeleteEndpoint(this.deviceTypeUrl, deviceTypeOrDeviceTypeId);
    };
    var DeviceService_1;
    DeviceService.deviceAddedOperation = "add";
    DeviceService.deviceDeletedOperation = "delete";
    DeviceService.deviceModifiedOperation = "modify";
    DeviceService = DeviceService_1 = __decorate([
        core_1.Injectable()
    ], DeviceService);
    return DeviceService;
}());
exports.DeviceService = DeviceService;
//# sourceMappingURL=device.service.js.map