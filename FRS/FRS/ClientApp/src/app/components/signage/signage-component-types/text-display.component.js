"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.EmployeeName = void 0;
var core_1 = require("@angular/core");
var EmployeeName = /** @class */ (function () {
    function EmployeeName(changeDetectorRef, authService, route, deviceService, employeeDataService, configurationService) {
        this.changeDetectorRef = changeDetectorRef;
        this.authService = authService;
        this.route = route;
        this.deviceService = deviceService;
        this.employeeDataService = employeeDataService;
        this.configurationService = configurationService;
    }
    EmployeeName.prototype.ngOnInit = function () {
        var _this = this;
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
        console.log('employee');
        if (!this.preview) {
            this.route.params.subscribe(function (queryParams) {
                _this.mac_address = queryParams["mac_address"];
            });
            if (!this.mac_address) {
                this.route.queryParams.subscribe(function (queryParams) {
                    _this.mac_address = queryParams["mac_address"];
                });
            }
            this.getDeviceInfo();
            this.start();
        }
    };
    EmployeeName.prototype.start = function () {
        var _this = this;
        this.getEmployeeName();
        setTimeout(function () {
            _this.start();
        }, 5 * 60 * 1000);
    };
    EmployeeName.prototype.getEmployeeName = function () {
        var _this = this;
        if (!this.device || !this.device.locationId)
            return;
        this.employeeDataService.getCurrentEmployeeDataByLocation(this.device.locationId)
            .subscribe(function (results) {
            _this.employees = results;
        }, function (error) {
        });
    };
    EmployeeName.prototype.getDeviceInfo = function () {
        var _this = this;
        if (this.mac_address) {
            this.deviceService.getDeviceById(null, this.mac_address, true)
                .subscribe(function (results) {
                _this.device = results.data;
                if (_this.device) {
                    _this.locationColorTheme = _this.device.locationColorTheme;
                    _this.locationName = _this.device.locationName;
                    if (_this.configurations.configurationsObj.roomInfoLocationName)
                        _this.configurations.configurationsObj.texts.roomInfo.value = _this.locationName;
                    _this.getEmployeeName();
                    _this.signalRCoreconnection = _this.authService.signalRConnection(_this.configurationService.baseUrl + "/hub/employeeschedule?location_id=" + _this.device.locationId, true);
                    if (_this.signalRCoreconnection != null) {
                        _this.signalRCoreconnection.on("UpdateSchedule", function (param) {
                            _this.getEmployeeName();
                        });
                    }
                }
            });
        }
    };
    __decorate([
        core_1.Input()
    ], EmployeeName.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], EmployeeName.prototype, "preview", void 0);
    EmployeeName = __decorate([
        core_1.Component({
            selector: 'employee-name',
            template: "<div\n[style.position]=\"'absolute'\"\n\n[style.display]=\"'flex'\"\n[style.align-items]=\"configurations.configurationsObj.name.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.name.vAlign == 'top' ? 'flex-start' : 'flex-end')\"\n\n[style.font-family]=\"configurations.configurationsObj.name.fontFamily\"\n[style.font-size.px]=\"configurations.configurationsObj.name.textSize\"\n[style.color]=\"configurations.configurationsObj.name.textColor\"\n[style.left.px]=\"configurations.configurationsObj.name.x\"\n[style.top.px]=\"configurations.configurationsObj.name.y\"\n[style.width.px]=\"configurations.configurationsObj.name.width\"\n[style.height.px]=\"configurations.configurationsObj.name.height\"\n[style.background-color]=\"configurations.configurationsObj.name.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.name.fieldColor\"\n[style.font-weight]=\"configurations.configurationsObj.name.bold ? 'bold' : 'normal'\"\n[style.text-decoration]=\"configurations.configurationsObj.name.underline ? 'underline' : 'none'\"\n[style.font-style]=\"configurations.configurationsObj.name.italic ? 'italic' : 'normal'\"\n>\n<div [style.width.%]=\"100\" [style.text-align]=\"configurations.configurationsObj.name.textAlign\">\n<span *ngIf=\"preview\">Employee Name</span>\n<span *ngIf=\"!preview\">\n  <span *ngFor=\"let e of employees; let i = index\"><span *ngIf=\"i > 0\"> | </span>{{e.displayName || e.name}}</span>\n</span>\n</div>\n</div>"
        })
    ], EmployeeName);
    return EmployeeName;
}());
exports.EmployeeName = EmployeeName;
//# sourceMappingURL=employee-name.component.js.map