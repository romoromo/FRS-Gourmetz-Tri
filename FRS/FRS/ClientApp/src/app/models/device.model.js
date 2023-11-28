"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Device = /** @class */ (function () {
    function Device(id, code, ipAddress, macAddress, serialNumber, isApproved, startDate, endDate, facilityId, facilityName, status, locationId, locationName) {
        this.code = code;
        this.ipAddress = ipAddress;
        this.macAddress = macAddress;
        this.serialNumber = serialNumber;
        this.isApproved = isApproved;
        this.startDate = startDate;
        this.endDate = endDate;
        this.facilityId = facilityId;
        this.facilityName = facilityName;
        this.status = status;
        this.locationId = locationId;
        this.locationName = locationName;
    }
    return Device;
}());
exports.Device = Device;
var DeviceFilter = /** @class */ (function () {
    function DeviceFilter() {
    }
    return DeviceFilter;
}());
exports.DeviceFilter = DeviceFilter;
var DevicePushMessage = /** @class */ (function () {
    function DevicePushMessage() {
    }
    return DevicePushMessage;
}());
exports.DevicePushMessage = DevicePushMessage;
//# sourceMappingURL=device.model.js.map