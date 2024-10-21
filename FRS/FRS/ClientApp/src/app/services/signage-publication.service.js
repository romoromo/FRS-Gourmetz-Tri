"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.SignagePublicationService = void 0;
var core_1 = require("@angular/core");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var SignagePublicationService = /** @class */ (function () {
    function SignagePublicationService(router, http, authService, commonEndpoint, configurations) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.commonEndpoint = commonEndpoint;
        this.configurations = configurations;
        this._signagePublicationsChanged = new rxjs_1.Subject();
        this._signagePublicationUrl = "/api/signagepublication";
    }
    SignagePublicationService_1 = SignagePublicationService;
    Object.defineProperty(SignagePublicationService.prototype, "signagePublicationUrl", {
        get: function () { return this.configurations.baseUrl + this._signagePublicationUrl; },
        enumerable: false,
        configurable: true
    });
    SignagePublicationService.prototype.onSignagePublicationsChanged = function (signagePublications, op) {
        this._signagePublicationsChanged.next({ signagePublications: signagePublications, operation: op });
    };
    SignagePublicationService.prototype.onSignagePublicationsCountChanged = function (signagePublications) {
        return this.onSignagePublicationsChanged(signagePublications, SignagePublicationService_1.signagePublicationModifiedOperation);
    };
    SignagePublicationService.prototype.getSignagePublicationsChangedEvent = function () {
        return this._signagePublicationsChanged.asObservable();
    };
    SignagePublicationService.prototype.getSignagePublication = function (id) {
        return this.commonEndpoint.get(this.signagePublicationUrl + '/signagepublication/' + id);
    };
    SignagePublicationService.prototype.getSignagePublications = function (page, pageSize) {
        return rxjs_1.forkJoin(this.commonEndpoint.getPagedList(this.signagePublicationUrl + '/signagepublications/list', page, pageSize));
    };
    SignagePublicationService.prototype.getSignagePublicationByInstitutionId = function (institutionId) {
        return rxjs_1.forkJoin(this.commonEndpoint.getByInstitutionId(this.signagePublicationUrl + '/signagepublications/list', institutionId));
    };
    SignagePublicationService.prototype.updateSignagePublication = function (signagePublication) {
        var _this = this;
        if (signagePublication.id) {
            return this.commonEndpoint.getUpdateEndpoint(this.signagePublicationUrl, signagePublication, signagePublication.id).pipe(operators_1.tap(function (data) { return _this.onSignagePublicationsChanged([signagePublication], SignagePublicationService_1.signagePublicationModifiedOperation); }));
        }
    };
    SignagePublicationService.prototype.newSignagePublication = function (signagePublication) {
        var _this = this;
        return this.commonEndpoint.getNewEndpoint(this.signagePublicationUrl, signagePublication).pipe(operators_1.tap(function (data) { return _this.onSignagePublicationsChanged([signagePublication], SignagePublicationService_1.signagePublicationAddedOperation); }));
    };
    SignagePublicationService.prototype.deleteSignagePublication = function (signagePublicationorId) {
        var _this = this;
        if (typeof signagePublicationorId === 'number' || signagePublicationorId instanceof Number ||
            typeof signagePublicationorId === 'string' || signagePublicationorId instanceof String) {
            return this.commonEndpoint.getDeleteEndpoint(this.signagePublicationUrl, signagePublicationorId).pipe(operators_1.tap(function (data) { return _this.onSignagePublicationsChanged([data], SignagePublicationService_1.signagePublicationDeletedOperation); }));
        }
        else {
            if (signagePublicationorId.id) {
                return this.deleteSignagePublication(signagePublicationorId.id);
            }
        }
    };
    var SignagePublicationService_1;
    SignagePublicationService.signagePublicationAddedOperation = "add";
    SignagePublicationService.signagePublicationDeletedOperation = "delete";
    SignagePublicationService.signagePublicationModifiedOperation = "modify";
    SignagePublicationService = SignagePublicationService_1 = __decorate([
        core_1.Injectable()
    ], SignagePublicationService);
    return SignagePublicationService;
}());
exports.SignagePublicationService = SignagePublicationService;
//# sourceMappingURL=signage-publication.service.js.map