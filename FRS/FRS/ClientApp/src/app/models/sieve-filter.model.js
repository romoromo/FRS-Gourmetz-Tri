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
exports.OrderCancellationReportFilter = exports.RoleReportFilter = exports.SalesOrderReportFilter = exports.OrderCancellationFilter = exports.ClassRosterFilter = exports.ServiceContractFilter = exports.CommonFilter = exports.PagedResult = exports.Filter = void 0;
var Filter = /** @class */ (function () {
    function Filter(page, pageSize) {
    }
    return Filter;
}());
exports.Filter = Filter;
var PagedResult = /** @class */ (function () {
    function PagedResult() {
    }
    return PagedResult;
}());
exports.PagedResult = PagedResult;
var CommonFilter = /** @class */ (function (_super) {
    __extends(CommonFilter, _super);
    function CommonFilter(page, pageSize) {
        return _super.call(this, page, pageSize) || this;
    }
    return CommonFilter;
}(Filter));
exports.CommonFilter = CommonFilter;
var ServiceContractFilter = /** @class */ (function () {
    function ServiceContractFilter(includeAssets, filter) {
        this.includeAssets = includeAssets;
        this.filter = filter;
    }
    return ServiceContractFilter;
}());
exports.ServiceContractFilter = ServiceContractFilter;
var ClassRosterFilter = /** @class */ (function (_super) {
    __extends(ClassRosterFilter, _super);
    function ClassRosterFilter() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ClassRosterFilter;
}(Filter));
exports.ClassRosterFilter = ClassRosterFilter;
var OrderCancellationFilter = /** @class */ (function (_super) {
    __extends(OrderCancellationFilter, _super);
    function OrderCancellationFilter() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return OrderCancellationFilter;
}(Filter));
exports.OrderCancellationFilter = OrderCancellationFilter;
var SalesOrderReportFilter = /** @class */ (function (_super) {
    __extends(SalesOrderReportFilter, _super);
    function SalesOrderReportFilter() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return SalesOrderReportFilter;
}(Filter));
exports.SalesOrderReportFilter = SalesOrderReportFilter;
var RoleReportFilter = /** @class */ (function (_super) {
    __extends(RoleReportFilter, _super);
    function RoleReportFilter() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return RoleReportFilter;
}(Filter));
exports.RoleReportFilter = RoleReportFilter;
var OrderCancellationReportFilter = /** @class */ (function (_super) {
    __extends(OrderCancellationReportFilter, _super);
    function OrderCancellationReportFilter() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return OrderCancellationReportFilter;
}(Filter));
exports.OrderCancellationReportFilter = OrderCancellationReportFilter;
//# sourceMappingURL=sieve-filter.model.js.map