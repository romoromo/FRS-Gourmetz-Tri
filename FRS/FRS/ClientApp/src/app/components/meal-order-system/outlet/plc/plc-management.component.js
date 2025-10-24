"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.PlcManagementComponent = void 0;
var core_1 = require("@angular/core");
var plc_model_1 = require("../../../../models/meal-order/plc.model");
var sieve_filter_model_1 = require("../../../../models/sieve-filter.model");
var alert_service_1 = require("../../../../services/alert.service");
var utilities_1 = require("../../../../services/utilities");
var PlcManagementComponent = /** @class */ (function () {
    function PlcManagementComponent(alertService, translationService, accountService, classService, dialog) {
        this.alertService = alertService;
        this.translationService = translationService;
        this.accountService = accountService;
        this.classService = classService;
        this.dialog = dialog;
        this.columns = [];
        this.rows = [];
        this.rowsCache = [];
        this.allpermissions = [];
        this.keyword = '';
    }
    PlcManagementComponent.prototype.openDialog = function (dispenserOutlet) {
        //const dialogRef = this.dialog.open(DispenserOutletEditorComponent, {
        //  data: { header: this.header, dispenserOutlet: dispenserOutlet },
        //  width: '400px',
        //  disableClose: true
        //});
        //dialogRef.afterClosed().subscribe(result => {
        //  this.loadData(null);
        //});
    };
    PlcManagementComponent.prototype.initializeFilter = function () {
        this.filter = new sieve_filter_model_1.Filter(1, 10);
        this.filter.sorts = 'IPAddress';
        this.filter.filters = '';
        this.filter.page = 1;
    };
    PlcManagementComponent.prototype.initializePagedResult = function () {
        this.pagedResult = new sieve_filter_model_1.PagedResult();
        this.pagedResult.totalCount = 0;
        this.pagedResult.pagedData = [];
        this.pagedResult.filter = this.filter;
    };
    PlcManagementComponent.prototype.initializeTableDefinition = function () {
        var _this = this;
        var gT = function (key) { return _this.translationService.getTranslation(key); };
        this.columns = [
            { prop: 'ipAddress', name: 'IP Address', width: 200 },
            { prop: 'framework', name: 'Framework', width: 200 },
            { prop: 'totalNumber', name: 'Total Number', width: 200 },
            { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
        ];
    };
    PlcManagementComponent.prototype.ngOnInit = function () {
        this.initializeFilter();
        this.initializePagedResult();
        this.initializeTableDefinition();
        this.loadData();
    };
    PlcManagementComponent.prototype.loadData = function (ev) {
        var _this = this;
        this.alertService.startLoadingMessage();
        this.loadingIndicator = true;
        this.filter.pageSize = 10;
        if (ev) {
            this.filter.page = ev.offset + 1;
            if (ev.sorts) {
                this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
            }
        }
        if (!this.keyword)
            this.keyword = '';
        var f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
        this.filter.filters = f + '(IsActive)==true,(IPAddress)@=' + this.keyword;
        this.classService.getPLCByFilter(this.filter)
            .subscribe(function (results) {
            _this.pagedResult = results;
            _this.alertService.stopLoadingMessage();
            _this.loadingIndicator = false;
            var plcs = results.pagedData;
            plcs.forEach(function (plc, index, plcs) {
                plc.index = index + 1;
            });
            _this.rowsCache = __spreadArrays(plcs);
            _this.rows = plcs;
        }, function (error) {
            _this.alertService.stopLoadingMessage();
            _this.loadingIndicator = false;
            _this.alertService.showStickyMessage("Load Error", "Unable to retrieve records from the server.\r\nErrors: \"" + utilities_1.Utilities.getHttpResponseMessage(error) + "\"", alert_service_1.MessageSeverity.error);
        });
    };
    PlcManagementComponent.prototype.onSearchChanged = function (value) {
        this.keyword = value;
        this.loadData(null);
    };
    PlcManagementComponent.prototype.newPLC = function () {
        this.header = 'New Dispenser';
        this.editedPlc = new plc_model_1.PlcModel();
        this.editedPlc.id = this.outletId;
        this.openDialog(this.editedPlc);
    };
    PlcManagementComponent.prototype.editPLC = function (row) {
        this.editedPlc = row;
        this.header = 'Edit Dispenser';
        this.openDialog(this.editedPlc);
    };
    PlcManagementComponent.prototype.deletePLC = function (row) {
        var _this = this;
        this.alertService.showDialog('Are you sure you want to delete the \"' + row.ipAddress + '\" PLC?', alert_service_1.DialogType.confirm, function () { return _this.deletePLCHelper(row); });
    };
    PlcManagementComponent.prototype.deletePLCHelper = function (row) {
        var _this = this;
        this.alertService.startLoadingMessage("Deleting...");
        this.loadingIndicator = true;
        this.classService.deletePLC(row.id)
            .subscribe(function (results) {
            _this.alertService.stopLoadingMessage();
            _this.loadingIndicator = false;
            _this.loadData();
        }, function (error) {
            _this.alertService.stopLoadingMessage();
            _this.loadingIndicator = false;
            _this.alertService.showStickyMessage("Delete Error", "An error occured while deleting the PLC.\r\nError: \"" + utilities_1.Utilities.getHttpResponseMessage(error) + "\"", alert_service_1.MessageSeverity.error);
        });
    };
    __decorate([
        core_1.Input()
    ], PlcManagementComponent.prototype, "isHideHeader", void 0);
    __decorate([
        core_1.Input()
    ], PlcManagementComponent.prototype, "outletId", void 0);
    __decorate([
        core_1.ViewChild('actionsTemplate')
    ], PlcManagementComponent.prototype, "actionsTemplate", void 0);
    __decorate([
        core_1.ViewChild('dispenserOutletEditor')
    ], PlcManagementComponent.prototype, "header", void 0);
    PlcManagementComponent = __decorate([
        core_1.Component({
            selector: 'plc-management',
            templateUrl: './plc-management.component.html',
            styleUrls: ['./plc-management.component.css']
        })
    ], PlcManagementComponent);
    return PlcManagementComponent;
}());
exports.PlcManagementComponent = PlcManagementComponent;
//# sourceMappingURL=plc-management.component.js.map