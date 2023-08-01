"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.UserInfoVal = exports.Facility = void 0;
var Facility = /** @class */ (function () {
    function Facility(name, description, capacity, institutionName, link) {
        this.name = name;
        this.description = description;
        this.institutionName = institutionName;
        this.link = link;
        this.capacity = capacity;
    }
    return Facility;
}());
exports.Facility = Facility;
var UserInfoVal = /** @class */ (function () {
    function UserInfoVal(id, title, color) {
    }
    return UserInfoVal;
}());
exports.UserInfoVal = UserInfoVal;
//# sourceMappingURL=facility.model.js.map