"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var reservationtime_model_1 = require("./reservationtime.model");
var Reservation = /** @class */ (function () {
    function Reservation(shortDescription, longDescription, startDate, startTime, endDate, endTime, repeatEndDateTime, facilityName, facilityId, locationName, locationId, isAllDay, status, repeatType, repeatEndDate, facilities, timeIntervals, invitees, isAttendee) {
        this.shortDescription = shortDescription;
        this.longDescription = longDescription;
        this.startDate = startDate;
        this.startTime = startTime;
        this.endDate = endDate;
        this.endTime = endTime;
        this.repeatEndDateTime = repeatEndDateTime;
        this.facilityName = facilityName;
        this.facilityId = facilityId;
        this.locationName = locationName;
        this.locationId = locationId;
        this.isAllDay = isAllDay;
        this.status = status;
        this.repeatType = repeatType;
        this.repeatEndDate = repeatEndDate;
        this.facilities = facilities;
        this.timeIntervals = timeIntervals;
        this.invitees = invitees;
        this.isAttendee = isAttendee;
        if (this.startTime == undefined) {
            this.startTime = new reservationtime_model_1.ReservationTime();
        }
        if (this.endTime == undefined) {
            this.endTime = new reservationtime_model_1.ReservationTime();
        }
    }
    return Reservation;
}());
exports.Reservation = Reservation;
var ReservationInvitee = /** @class */ (function () {
    function ReservationInvitee(id, email, userId, name) {
        this.id = id;
        this.email = email;
        this.userId = userId;
        this.name = name;
    }
    return ReservationInvitee;
}());
exports.ReservationInvitee = ReservationInvitee;
var CalendarFilter = /** @class */ (function () {
    function CalendarFilter(locationIds, facilityIds, viewDate, startDate, endDate, startTime, endTime, capacity, currentUserId, isForAttendance, institutionId) {
    }
    return CalendarFilter;
}());
exports.CalendarFilter = CalendarFilter;
//# sourceMappingURL=reservation.model.js.map