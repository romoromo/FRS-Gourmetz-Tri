"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.LocationTimeInterval = exports.BookingGrid = exports.BookingGridRow = void 0;
var BookingGridRow = /** @class */ (function () {
    function BookingGridRow(location, locationTimeIntervals, count) {
        this.location = location;
        this.locationTimeIntervals = locationTimeIntervals;
        this.count = count;
    }
    return BookingGridRow;
}());
exports.BookingGridRow = BookingGridRow;
var BookingGrid = /** @class */ (function () {
    function BookingGrid(location, rows) {
        this.location = location;
        this.rows = rows;
    }
    return BookingGrid;
}());
exports.BookingGrid = BookingGrid;
var LocationTimeInterval = /** @class */ (function () {
    function LocationTimeInterval() {
    }
    return LocationTimeInterval;
}());
exports.LocationTimeInterval = LocationTimeInterval;
//# sourceMappingURL=bookinggridrow.model.js.map