"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.SignageComponentTypeWrapperComponent = void 0;
var core_1 = require("@angular/core");
var signage_component_type_directive_1 = require("../../../directives/signage-component-type.directive");
var SignageComponentTypeWrapperComponent = /** @class */ (function () {
    function SignageComponentTypeWrapperComponent(componentFactoryResolver) {
        this.componentFactoryResolver = componentFactoryResolver;
    }
    SignageComponentTypeWrapperComponent.prototype.ngOnInit = function () {
        this.loadComponent(this.type, this.configurations, this.preview);
    };
    SignageComponentTypeWrapperComponent.prototype.ngOnDestroy = function () {
    };
    SignageComponentTypeWrapperComponent.prototype.loadComponent = function (type, configurations, preview) {
        var componentFactory = this.componentFactoryResolver.resolveComponentFactory(type);
        var viewContainerRef = this.componentTypeHost.viewContainerRef;
        viewContainerRef.clear();
        this.componentRef = viewContainerRef.createComponent(componentFactory);
        this.componentRef.instance.configurations = configurations || {};
        this.componentRef.instance.preview = preview;
    };
    SignageComponentTypeWrapperComponent.prototype.getConfigurations = function () {
        if (this.componentRef)
            return this.componentRef.instance.getConfigurations();
    };
    __decorate([
        core_1.Input()
    ], SignageComponentTypeWrapperComponent.prototype, "type", void 0);
    __decorate([
        core_1.Input()
    ], SignageComponentTypeWrapperComponent.prototype, "configurations", void 0);
    __decorate([
        core_1.Input()
    ], SignageComponentTypeWrapperComponent.prototype, "preview", void 0);
    __decorate([
        core_1.ViewChild(signage_component_type_directive_1.SignageComponentTypeDirective)
    ], SignageComponentTypeWrapperComponent.prototype, "componentTypeHost", void 0);
    SignageComponentTypeWrapperComponent = __decorate([
        core_1.Component({
            selector: 'signage-component-type-wrapper',
            template: "<ng-template componentTypeHost></ng-template>"
        })
    ], SignageComponentTypeWrapperComponent);
    return SignageComponentTypeWrapperComponent;
}());
exports.SignageComponentTypeWrapperComponent = SignageComponentTypeWrapperComponent;
/*
Copyright Google LLC. All Rights Reserved.
Use of this source code is governed by an MIT-style license that
can be found in the LICENSE file at https://angular.io/license
*/
//# sourceMappingURL=signage-component-type-wrapper.component.js.map