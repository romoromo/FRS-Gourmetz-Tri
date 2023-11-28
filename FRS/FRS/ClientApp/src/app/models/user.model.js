"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var User = /** @class */ (function () {
    // Note: Using only optional constructor properties without backing store disables typescript's type checking for the type
    function User(id, userName, fullName, email, jobTitle, phoneNumber, roles, institutionId, pin, departmentId, employeeId, registeredDepartment, unitNumber) {
        this.id = id;
        this.userName = userName;
        this.fullName = fullName;
        this.email = email;
        this.jobTitle = jobTitle;
        this.phoneNumber = phoneNumber;
        this.roles = roles;
        this.institutionId = institutionId;
        this.pin = pin;
        this.departmentId = departmentId;
        this.employeeId = employeeId;
        this.registeredDepartment = registeredDepartment;
        this.unitNumber = unitNumber;
    }
    Object.defineProperty(User.prototype, "friendlyName", {
        get: function () {
            var name = this.fullName || this.userName;
            if (this.jobTitle)
                name = this.jobTitle + " " + name;
            return name;
        },
        enumerable: true,
        configurable: true
    });
    return User;
}());
exports.User = User;
var UserOutlet = /** @class */ (function () {
    function UserOutlet() {
    }
    return UserOutlet;
}());
exports.UserOutlet = UserOutlet;
var FRSHubConnections = /** @class */ (function () {
    function FRSHubConnections() {
    }
    FRSHubConnections = __decorate([
        core_1.Injectable()
    ], FRSHubConnections);
    return FRSHubConnections;
}());
exports.FRSHubConnections = FRSHubConnections;
var UserReportFilter = /** @class */ (function () {
    function UserReportFilter() {
    }
    return UserReportFilter;
}());
exports.UserReportFilter = UserReportFilter;
var UserRoleReportFilter = /** @class */ (function () {
    function UserRoleReportFilter() {
    }
    return UserRoleReportFilter;
}());
exports.UserRoleReportFilter = UserRoleReportFilter;
//# sourceMappingURL=user.model.js.map