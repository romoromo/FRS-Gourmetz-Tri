"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ReservationTime = /** @class */ (function () {
    function ReservationTime(id, description, value, hour, minutes) {
        this.id = id;
        this.description = description;
        this.hour = hour;
        this.minutes = minutes;
        var d = new Date();
        d.setHours(hour, minutes);
        this.value = d;
    }
    return ReservationTime;
}());
exports.ReservationTime = ReservationTime;
var TimeIntervalFilter = /** @class */ (function () {
    function TimeIntervalFilter(locationId, startDate, endDate, startTimeInterval, endTimeInterval) {
        this.locationId = locationId;
        this.startDate = startDate;
        this.endDate = endDate;
        this.startTimeInterval = startTimeInterval;
        this.endTimeInterval = endTimeInterval;
    }
    return TimeIntervalFilter;
}());
exports.TimeIntervalFilter = TimeIntervalFilter;
//# sourceMappingURL=reservationtime.model.js.map