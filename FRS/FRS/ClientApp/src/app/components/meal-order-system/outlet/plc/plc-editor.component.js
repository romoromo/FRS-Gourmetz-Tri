"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.PlcEditorComponent = void 0;
var core_1 = require("@angular/core");
var material_1 = require("@angular/material");
var plc_model_1 = require("../../../../models/meal-order/plc.model");
var alert_service_1 = require("../../../../services/alert.service");
var PlcEditorComponent = /** @class */ (function () {
    function PlcEditorComponent(alertService, classService, accountService, dialogRef, deliveryService, data) {
        this.alertService = alertService;
        this.classService = classService;
        this.accountService = accountService;
        this.dialogRef = dialogRef;
        this.deliveryService = deliveryService;
        this.data = data;
        this.isNewPlc = false;
        this.showValidationErrors = true;
        this.plcEdit = new plc_model_1.PlcModel();
        this.allPermissions = [];
        this.outlets = [];
        this.selectedValues = {};
        this.formResetToggle = true;
        this.editPLC(data.plc);
    }
    PlcEditorComponent.prototype.saveSuccessHelper = function (plc) {
        if (plc)
            Object.assign(this.plcEdit, plc);
        this.isSaving = false;
        this.alertService.stopLoadingMessage();
        this.showValidationErrors = false;
        if (this.isNewPlc)
            this.alertService.showMessage("Success", "PLC \"" + this.plcEdit.ipAddress + "\" was created successfully", alert_service_1.MessageSeverity.success);
        else
            this.alertService.showMessage("Success", "Changes to PLC \"" + this.plcEdit.ipAddress + "\" was saved successfully", alert_service_1.MessageSeverity.success);
        this.plcEdit = new plc_model_1.PlcModel();
        this.resetForm();
        if (this.changesSavedCallback)
            this.changesSavedCallback();
        this.dialogRef.close();
    };
    PlcEditorComponent.prototype.save = function () {
        var _this = this;
        debugger;
        this.isSaving = true;
        this.alertService.startLoadingMessage("Saving changes...");
        if (this.isNewPlc) {
            this.classService.newPLC(this.plcEdit).subscribe(function (plc) { return _this.saveSuccessHelper(plc); }, function (error) { return _this.saveFailedHelper(error); });
        }
        else {
            this.classService.updatePLC(this.plcEdit).subscribe(function (response) { return _this.saveSuccessHelper(); }, function (error) { return _this.saveFailedHelper(error); });
        }
    };
    PlcEditorComponent.prototype.saveFailedHelper = function (error) {
        this.isSaving = false;
        this.alertService.stopLoadingMessage();
        this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", alert_service_1.MessageSeverity.error);
        this.alertService.showStickyMessage(error, null, alert_service_1.MessageSeverity.error);
        if (this.changesFailedCallback)
            this.changesFailedCallback();
    };
    PlcEditorComponent.prototype.resetForm = function (replace) {
        var _this = this;
        if (replace === void 0) { replace = false; }
        if (!replace) {
            this.form.reset();
        }
        else {
            this.formResetToggle = false;
            setTimeout(function () {
                _this.formResetToggle = true;
            });
        }
    };
    PlcEditorComponent.prototype.newPLC = function () {
        this.isNewPlc = true;
        this.showValidationErrors = true;
        this.editingPlcCode = null;
        this.selectedValues = {};
        this.plcEdit = new plc_model_1.PlcModel();
        return this.plcEdit;
    };
    PlcEditorComponent.prototype.editPLC = function (plc) {
        if (plc && plc.id) {
            this.isNewPlc = false;
            this.showValidationErrors = true;
            this.editingPlcCode = plc.id;
            this.selectedValues = {};
            this.plcEdit = new plc_model_1.PlcModel();
            Object.assign(this.plcEdit, plc);
        }
        else {
            this.newPLC();
            Object.assign(this.plcEdit, plc);
        }
        return this.plcEdit;
    };
    __decorate([
        core_1.ViewChild('f')
    ], PlcEditorComponent.prototype, "form", void 0);
    PlcEditorComponent = __decorate([
        core_1.Component({
            selector: 'plc-editor',
            templateUrl: './plc-editor.component.html',
            styleUrls: ['./plc-editor.component.css']
        }),
        __param(5, core_1.Inject(material_1.MAT_DIALOG_DATA))
    ], PlcEditorComponent);
    return PlcEditorComponent;
}());
exports.PlcEditorComponent = PlcEditorComponent;
//# sourceMappingURL=plc-editor.component.js.map