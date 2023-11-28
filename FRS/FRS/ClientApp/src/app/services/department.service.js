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
var DepartmentService = /** @class */ (function () {
    function DepartmentService(router, http, authService, commonEndpoint, configurations) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.commonEndpoint = commonEndpoint;
        this.configurations = configurations;
        this._departmentsChanged = new rxjs_1.Subject();
        this._departmentUrl = "/api/department";
    }
    DepartmentService_1 = DepartmentService;
    Object.defineProperty(DepartmentService.prototype, "departmentUrl", {
        get: function () { return this.configurations.baseUrl + this._departmentUrl; },
        enumerable: true,
        configurable: true
    });
    DepartmentService.prototype.onDepartmentsChanged = function (departments, op) {
        this._departmentsChanged.next({ departments: departments, operation: op });
    };
    DepartmentService.prototype.onDepartmentsCountChanged = function (departments) {
        return this.onDepartmentsChanged(departments, DepartmentService_1.departmentModifiedOperation);
    };
    DepartmentService.prototype.getDepartmentsChangedEvent = function () {
        return this._departmentsChanged.asObservable();
    };
    DepartmentService.prototype.getDepartmentById = function (departmentId) {
        return this.commonEndpoint.getById(this.departmentUrl + '/get', departmentId);
    };
    DepartmentService.prototype.getDepartments = function (page, pageSize, institutionId, institutionCode) {
        var queryParams = '';
        queryParams += (institutionId ? '?institutionId=' + institutionId : '');
        queryParams += (institutionCode ? '?institutionCode=' + institutionCode : '');
        if (queryParams == '' || page || pageSize) {
            return this.commonEndpoint.getPagedList(this.departmentUrl + '/departments/list', page, pageSize);
        }
        else {
            return this.commonEndpoint.get(this.departmentUrl + '/departments/list' + queryParams);
        }
    };
    DepartmentService.prototype.getDepartmentsByFilter = function (filter) {
        return this.commonEndpoint.getSieve(this.departmentUrl + '/departments/sieve/list', filter);
    };
    DepartmentService.prototype.updateDepartment = function (department) {
        var _this = this;
        if (department.id) {
            return this.commonEndpoint.getUpdateEndpoint(this.departmentUrl, department, department.id).pipe(operators_1.tap(function (data) { return _this.onDepartmentsChanged([department], DepartmentService_1.departmentModifiedOperation); }));
        }
    };
    DepartmentService.prototype.newDepartment = function (department) {
        var _this = this;
        return this.commonEndpoint.getNewEndpoint(this.departmentUrl, department).pipe(operators_1.tap(function (data) { return _this.onDepartmentsChanged([department], DepartmentService_1.departmentAddedOperation); }));
    };
    DepartmentService.prototype.deleteDepartment = function (departmentOrDepartmentId) {
        var _this = this;
        if (typeof departmentOrDepartmentId === 'number' || departmentOrDepartmentId instanceof Number ||
            typeof departmentOrDepartmentId === 'string' || departmentOrDepartmentId instanceof String) {
            return this.commonEndpoint.getDeleteEndpoint(this.departmentUrl, departmentOrDepartmentId).pipe(operators_1.tap(function (data) { return _this.onDepartmentsChanged([data], DepartmentService_1.departmentDeletedOperation); }));
        }
        else {
            if (departmentOrDepartmentId.id) {
                return this.deleteDepartment(departmentOrDepartmentId.id);
            }
        }
    };
    var DepartmentService_1;
    DepartmentService.departmentAddedOperation = "add";
    DepartmentService.departmentDeletedOperation = "delete";
    DepartmentService.departmentModifiedOperation = "modify";
    DepartmentService = DepartmentService_1 = __decorate([
        core_1.Injectable()
    ], DepartmentService);
    return DepartmentService;
}());
exports.DepartmentService = DepartmentService;
//# sourceMappingURL=department.service.js.map