"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
exports.ServiceContractAsset = exports.LocationAsset = exports.Asset = exports.BaseAsset = void 0;
var BaseAsset = /** @class */ (function () {
    function BaseAsset() {
    }
    return BaseAsset;
}());
exports.BaseAsset = BaseAsset;
var Asset = /** @class */ (function (_super) {
    __extends(Asset, _super);
    function Asset() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return Asset;
}(BaseAsset));
exports.Asset = Asset;
var LocationAsset = /** @class */ (function (_super) {
    __extends(LocationAsset, _super);
    function LocationAsset() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return LocationAsset;
}(BaseAsset));
exports.LocationAsset = LocationAsset;
var ServiceContractAsset = /** @class */ (function (_super) {
    __extends(ServiceContractAsset, _super);
    function ServiceContractAsset() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ServiceContractAsset;
}(BaseAsset));
exports.ServiceContractAsset = ServiceContractAsset;
//# sourceMappingURL=asset.model.js.map