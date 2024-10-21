"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.EndpointFactory = void 0;
var core_1 = require("@angular/core");
var http_1 = require("@angular/common/http");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var auth_service_1 = require("./auth.service");
var configuration_service_1 = require("./configuration.service");
var EndpointFactory = /** @class */ (function () {
    function EndpointFactory(http, configurations, injector) {
        this.http = http;
        this.configurations = configurations;
        this.injector = injector;
        this._loginUrl = "/connect/token";
        this._login2FAUrl = "/login/multi-fa";
        this._resend2FAUrl = "/login/multi-fa/resend";
        this._forgotPasswordUrl = "/api/account/forgotpassword";
        this._resetPasswordUrl = "/api/account/resetpassword";
        this._rolePermissionsUrl = "/api/account/roles/permissions";
    }
    EndpointFactory_1 = EndpointFactory;
    Object.defineProperty(EndpointFactory.prototype, "loginUrl", {
        get: function () { return this.configurations.baseUrl + this._loginUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(EndpointFactory.prototype, "forgotPasswordUrl", {
        get: function () { return this.configurations.baseUrl + this._forgotPasswordUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(EndpointFactory.prototype, "resetPasswordUrl", {
        get: function () { return this.configurations.baseUrl + this._resetPasswordUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(EndpointFactory.prototype, "login2FAUrl", {
        get: function () { return this.configurations.baseUrl + this._login2FAUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(EndpointFactory.prototype, "resend2FAUrl", {
        get: function () { return this.configurations.baseUrl + this._resend2FAUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(EndpointFactory.prototype, "rolePermissionsUrl", {
        get: function () { return this.configurations.baseUrl + this._rolePermissionsUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(EndpointFactory.prototype, "authService", {
        get: function () {
            if (!this._authService)
                this._authService = this.injector.get(auth_service_1.AuthService);
            return this._authService;
        },
        enumerable: false,
        configurable: true
    });
    EndpointFactory.prototype.getUserPermissions = function (roleNames) {
        var _this = this;
        return this.http.post("" + this.rolePermissionsUrl, JSON.stringify(roleNames), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUserPermissions(roleNames); });
        }));
    };
    EndpointFactory.prototype.getForgotPasswordEndpoint = function (email, institutionCode) {
        var _this = this;
        return this.http.post(this.forgotPasswordUrl + '?email=' + email + '&institutioncode=' + institutionCode, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getForgotPasswordEndpoint(email, institutionCode); });
        }));
    };
    EndpointFactory.prototype.getResetPasswordEndpoint = function (resetPassword) {
        var _this = this;
        return this.http.post(this.resetPasswordUrl, JSON.stringify(resetPassword), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getResetPasswordEndpoint(resetPassword); });
        }));
    };
    EndpointFactory.prototype.getLoginEndpoint = function (userName, password, institutionCode, isExternalLogin, isAD) {
        if (isExternalLogin === void 0) { isExternalLogin = 'false'; }
        if (isAD === void 0) { isAD = false; }
        var header = new http_1.HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        var params = new http_1.HttpParams()
            .append('username', userName)
            .append('password', password)
            .append('grant_type', 'password')
            .append('isExternalLogin', isExternalLogin)
            .append('institutionCode', institutionCode)
            .append('isAD', isAD.toString())
            .append('needConfirmationCode', "true")
            .append('mfa', this.configurations.enableMFA ? 'true' : 'false')
            .append('mfaValidation', this.configurations.enableMFAValidation ? 'true' : 'false')
            .append('scope', 'openid email phone profile offline_access roles');
        var requestBody = params.toString();
        return this.http.post(this.loginUrl, requestBody, { headers: header });
    };
    EndpointFactory.prototype.getLogin2FAEndpoint = function (userId, code) {
        var header = new http_1.HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        var params = new http_1.HttpParams()
            .append('userId', userId)
            .append('code', code);
        var requestBody = params.toString();
        return this.http.post(this.login2FAUrl, requestBody, { headers: header });
    };
    EndpointFactory.prototype.getResendCode2FAEndpoint = function (userId) {
        var header = new http_1.HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        var params = new http_1.HttpParams()
            .append('userId', userId);
        var requestBody = params.toString();
        return this.http.post(this.resend2FAUrl, requestBody, { headers: header });
    };
    EndpointFactory.prototype.getRefreshLoginEndpoint = function () {
        var _this = this;
        var header = new http_1.HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded', 'Accept': 'application/json' });
        var params = new http_1.HttpParams()
            .append('refresh_token', this.authService.refreshToken)
            .append('id_token', this.authService.idToken)
            .append('grant_type', 'refresh_token')
            .append('scope', 'openid email phone profile offline_access roles');
        var requestBody = params.toString();
        //return this.http.post<T>(this.loginUrl, requestBody, { headers: header });
        return this.http.post(this.loginUrl, requestBody, { headers: header }).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getRefreshLoginEndpoint(); });
        }));
    };
    EndpointFactory.prototype.getRequestHeaders = function (content_type) {
        var headers = new http_1.HttpHeaders({
            'Authorization': 'Bearer ' + this.authService.accessToken,
            'Content-Type': content_type ? content_type : 'application/json',
            'Accept': "application/vnd.iman.v" + EndpointFactory_1.apiVersion + "+json, application/json, text/plain, */*",
            'App-Version': configuration_service_1.ConfigurationService.appVersion,
            'ApiKey': configuration_service_1.ConfigurationService.appVersion,
            'ClientId': configuration_service_1.ConfigurationService.appVersion,
            //'Access-Control-Allow-Origin' : '*'
        });
        return { headers: headers };
    };
    EndpointFactory.prototype.handleError = function (error, continuation) {
        var _this = this;
        console.log(error);
        console.log(this.isRefreshingLogin);
        if (error.status == 401) {
            if (this.isRefreshingLogin) {
                return this.pauseTask(continuation);
            }
            this.isRefreshingLogin = true;
            return this.authService.refreshLogin().pipe(operators_1.mergeMap(function (data) {
                _this.isRefreshingLogin = false;
                _this.resumeTasks(true);
                return continuation();
            }), operators_1.catchError(function (refreshLoginError) {
                _this.isRefreshingLogin = false;
                _this.resumeTasks(false);
                if (refreshLoginError.status == 401 || (refreshLoginError.url && refreshLoginError.url.toLowerCase().includes(_this.loginUrl.toLowerCase()))) {
                    _this.authService.reLogin();
                    return rxjs_1.throwError('session expired');
                }
                else {
                    return rxjs_1.throwError(refreshLoginError || 'server error');
                }
            }));
        }
        if (error.url && error.url.toLowerCase().includes(this.loginUrl.toLowerCase())) {
            this.authService.reLogin();
            return rxjs_1.throwError((error.error && error.error.error_description) ? "session expired (" + error.error.error_description + ")" : 'session expired');
        }
        else {
            return rxjs_1.throwError(error);
        }
    };
    EndpointFactory.prototype.pauseTask = function (continuation) {
        if (!this.taskPauser)
            this.taskPauser = new rxjs_1.Subject();
        return this.taskPauser.pipe(operators_1.switchMap(function (continueOp) {
            return continueOp ? continuation() : rxjs_1.throwError('session expired');
        }));
    };
    EndpointFactory.prototype.resumeTasks = function (continueOp) {
        var _this = this;
        setTimeout(function () {
            if (_this.taskPauser) {
                _this.taskPauser.next(continueOp);
                _this.taskPauser.complete();
                _this.taskPauser = null;
            }
        });
    };
    var EndpointFactory_1;
    EndpointFactory.apiVersion = "1";
    EndpointFactory = EndpointFactory_1 = __decorate([
        core_1.Injectable()
    ], EndpointFactory);
    return EndpointFactory;
}());
exports.EndpointFactory = EndpointFactory;
//# sourceMappingURL=endpoint-factory.service.js.map