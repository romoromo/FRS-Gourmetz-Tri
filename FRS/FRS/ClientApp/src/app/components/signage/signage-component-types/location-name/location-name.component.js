"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var LocationName = /** @class */ (function () {
    function LocationName(changeDetectorRef, authService, route, deviceService) {
        this.changeDetectorRef = changeDetectorRef;
        this.authService = authService;
        this.route = route;
        this.deviceService = deviceService;
    }
    LocationName.prototype.ngOnInit = function () {
        var _this = this;
        if (!this.configurations.configurationsObj)
            this.configurations.configurationsObj = JSON.parse(this.configurations.configurations);
        console.log('location');
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
        }
    };
    LocationName.prototype.getDeviceInfo = function () {
        var _this = this;
        if (this.mac_address) {
            this.deviceService.getDeviceById(null, this.mac_address, true)
                .subscribe(function (results) {
                _this.device = results.data;
                if (_this.device) {
                    _this.locationColorTheme = _this.device.locationColorTheme;
                    _this.locationName = _this.device.locationName;
                    _this.locationGroup = _this.device.locationGroup;
                    if (_this.configurations.configurationsObj.roomInfoLocationName)
                        _this.configurations.configurationsObj.texts.roomInfo.value = _this.locationName;
                }
            });
        }
    };
    __decorate([
        core_1.Input()
    ], LocationName.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], LocationName.prototype, "preview", void 0);
    LocationName = __decorate([
        core_1.Component({
            selector: 'location-name',
            template: "<div [style.position]=\"'absolute'\" [style.display]=\"'flex'\"\n    [style.align-items]=\"configurations.configurationsObj.name.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.name.vAlign == 'top' ? 'flex-start' : 'flex-end')\"\n    [style.font-family]=\"configurations.configurationsObj.name.fontFamily\"\n    [style.font-size.px]=\"configurations.configurationsObj.name.textSize\"\n    [style.color]=\"configurations.configurationsObj.name.textColor\"\n    [style.left.px]=\"configurations.configurationsObj.name.x\" [style.top.px]=\"configurations.configurationsObj.name.y\"\n    [style.width.px]=\"configurations.configurationsObj.name.width\"\n    [style.height.px]=\"configurations.configurationsObj.name.height\"\n    [style.background-color]=\"configurations.configurationsObj.name.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.name.fieldColor\"\n    [style.font-weight]=\"configurations.configurationsObj.name.bold ? 'bold' : 'normal'\"\n    [style.text-decoration]=\"configurations.configurationsObj.name.underline ? 'underline' : 'none'\"\n    [style.font-style]=\"configurations.configurationsObj.name.italic ? 'italic' : 'normal'\">\n    <div [style.width.%]=\"100\" [style.text-align]=\"configurations.configurationsObj.name.textAlign\">\n        <span *ngIf=\"preview\">Location Name</span>\n        <span *ngIf=\"!preview\">{{locationName}}\n        </span>\n    </div>\n</div>\n\n<div [style.position]=\"'absolute'\" [style.display]=\"'flex'\"\n    [style.align-items]=\"configurations.configurationsObj.group.vAlign == 'middle' ? 'center' : (configurations.configurationsObj.group.vAlign == 'top' ? 'flex-start' : 'flex-end')\"\n    [style.font-family]=\"configurations.configurationsObj.group.fontFamily\"\n    [style.font-size.px]=\"configurations.configurationsObj.group.textSize\"\n    [style.color]=\"configurations.configurationsObj.group.textColor\"\n    [style.left.px]=\"configurations.configurationsObj.group.x\" [style.top.px]=\"configurations.configurationsObj.group.y\"\n    [style.width.px]=\"configurations.configurationsObj.group.width\"\n    [style.height.px]=\"configurations.configurationsObj.group.height\"\n    [style.background-color]=\"configurations.configurationsObj.group.fieldColorLocation && locationColorTheme ? locationColorTheme : configurations.configurationsObj.group.fieldColor\"\n    [style.font-weight]=\"configurations.configurationsObj.group.bold ? 'bold' : 'normal'\"\n    [style.text-decoration]=\"configurations.configurationsObj.group.underline ? 'underline' : 'none'\"\n    [style.font-style]=\"configurations.configurationsObj.group.italic ? 'italic' : 'normal'\">\n    <div [style.width.%]=\"100\" [style.text-align]=\"configurations.configurationsObj.group.textAlign\">\n        <span *ngIf=\"preview\">Location Group</span>\n        <span *ngIf=\"!preview\">{{locationGroup}}\n        </span>\n    </div>\n</div>\n"
        })
    ], LocationName);
    return LocationName;
}());
exports.LocationName = LocationName;
//# sourceMappingURL=location-name.component.js.map