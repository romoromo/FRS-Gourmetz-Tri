"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.EmployeeDataService = void 0;
var core_1 = require("@angular/core");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var EmployeeDataService = /** @class */ (function () {
    function EmployeeDataService(router, http, authService, commonEndpoint, configurations) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.commonEndpoint = commonEndpoint;
        this.configurations = configurations;
        this._employeeDatasChanged = new rxjs_1.Subject();
        this._employeeDataUrl = "/api/employeedata";
    }
    EmployeeDataService_1 = EmployeeDataService;
    Object.defineProperty(EmployeeDataService.prototype, "employeeDataUrl", {
        get: function () { return this.configurations.baseUrl + this._employeeDataUrl; },
        enumerable: false,
        configurable: true
    });
    EmployeeDataService.prototype.onEmployeeDatasChanged = function (employeeDatas, op) {
        this._employeeDatasChanged.next({ employeeDatas: employeeDatas, operation: op });
    };
    EmployeeDataService.prototype.onEmployeeDatasCountChanged = function (employeeDatas) {
        return this.onEmployeeDatasChanged(employeeDatas, EmployeeDataService_1.employeeDataModifiedOperation);
    };
    EmployeeDataService.prototype.getEmployeeDatasChangedEvent = function () {
        return this._employeeDatasChanged.asObservable();
    };
    EmployeeDataService.prototype.getEmployeeDatas = function (page, pageSize) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.employeeDataUrl + '/list', page, pageSize));
    };
    EmployeeDataService.prototype.getEmployeeDataByInstitutionId = function (institutionId) {
        return rxjs_1.forkJoin(this.commonEndpoint.getByInstitutionId(this.employeeDataUrl + '/list', institutionId));
    };
    EmployeeDataService.prototype.getEmployeeDataByKey = function (institutionId, key) {
        return this.commonEndpoint.get(this.employeeDataUrl + '/get?institutionId=' + institutionId + '&key=' + key);
    };
    EmployeeDataService.prototype.getCurrentEmployeeDataByLocation = function (locationId) {
        return this.commonEndpoint.get(this.employeeDataUrl + '/getcurrentemployeebylocation?locationId=' + locationId);
    };
    EmployeeDataService.prototype.updateEmployeeData = function (employeeData) {
        var _this = this;
        return this.commonEndpoint.getUpdateEndpoint(this.employeeDataUrl, employeeData, employeeData.id).pipe(operators_1.tap(function (data) { return _this.onEmployeeDatasChanged([employeeData], EmployeeDataService_1.employeeDataModifiedOperation); }));
    };
    EmployeeDataService.prototype.newEmployeeData = function (employeeData) {
        var _this = this;
        return this.commonEndpoint.getNewEndpoint(this.employeeDataUrl, employeeData).pipe(operators_1.tap(function (data) { return _this.onEmployeeDatasChanged([employeeData], EmployeeDataService_1.employeeDataAddedOperation); }));
    };
    EmployeeDataService.prototype.deleteEmployeeData = function (id) {
        var _this = this;
        return this.commonEndpoint.getDeleteEndpoint(this.employeeDataUrl, id).pipe(operators_1.tap(function (data) { return _this.onEmployeeDatasChanged([data], EmployeeDataService_1.employeeDataDeletedOperation); }));
    };
    var EmployeeDataService_1;
    EmployeeDataService.employeeDataAddedOperation = "add";
    EmployeeDataService.employeeDataDeletedOperation = "delete";
    EmployeeDataService.employeeDataModifiedOperation = "modify";
    EmployeeDataService = EmployeeDataService_1 = __decorate([
        core_1.Injectable()
    ], EmployeeDataService);
    return EmployeeDataService;
}());
exports.EmployeeDataService = EmployeeDataService;
//# sourceMappingURL=employee-data.service.js.map