"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.FreeText = void 0;
var core_1 = require("@angular/core");
var FreeText = /** @class */ (function () {
    function FreeText(changeDetectorRef, authService, route, deviceService) {
        this.changeDetectorRef = changeDetectorRef;
        this.authService = authService;
        this.route = route;
        this.deviceService = deviceService;
    }
    FreeText.prototype.ngOnInit = function () {
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
    FreeText.prototype.getDeviceInfo = function () {
        var _this = this;
        if (this.mac_address) {
            this.deviceService.getDeviceById(null, this.mac_address, true)
                .subscribe(function (results) {
                _this.device = results.data;
                if (_this.device) {
                    _this.locationColorTheme = _this.device.locationColorTheme;
                }
            });
        }
    };
    __decorate([
        core_1.Input()
    ], FreeText.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], FreeText.prototype, "preview", void 0);
    FreeText = __decorate([
        core_1.Component({
            selector: 'free-text',
            template: "\n<text-display [preview]=\"preview\" [textStyle]=\"configurations.configurationsObj.name\" [previewText]=\"configurations.configurationsObj.freetext || 'Free Text'\" [actualText]=\"configurations.configurationsObj.freetext\"></text-display>\n"
        })
    ], FreeText);
    return FreeText;
}());
exports.FreeText = FreeText;
//# sourceMappingURL=free-text.component.js.map