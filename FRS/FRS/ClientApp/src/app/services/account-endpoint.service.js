"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var http_1 = require("@angular/common/http");
var operators_1 = require("rxjs/operators");
var endpoint_factory_service_1 = require("./endpoint-factory.service");
var AccountEndpoint = /** @class */ (function (_super) {
    __extends(AccountEndpoint, _super);
    function AccountEndpoint(http, configurations, injector) {
        var _this = _super.call(this, http, configurations, injector) || this;
        _this.configurations = configurations;
        _this._usersUrl = "/api/account/users";
        _this._usersRegisterUrl = "/api/account/users/register";
        _this._userByUserNameUrl = "/api/account/users/username";
        _this._currentUserUrl = "/api/account/users/me";
        _this._currentUserPreferencesUrl = "/api/account/users/me/preferences";
        _this._unblockUserUrl = "/api/account/users/unblock";
        _this._resetUserPasswordUrl = "/api/account/users/forgotpassword";
        _this._rolesUrl = "/api/account/roles";
        _this._roleByRoleNameUrl = "/api/account/roles/name";
        _this._permissionsUrl = "/api/account/permissions";
        _this._permissionsTreeUrl = "/api/account/permissionstree";
        _this._userPhonebooksUrl = "/api/account/phonebooks";
        _this._userVehiclesUrl = "/api/account/vehicles";
        _this._userCardIdsUrl = "/api/account/cardids";
        _this._configUrl = "/api/configuration";
        _this._walletTransactUrl = "/api/account/wallet/operation";
        _this._rewardTransactUrl = "/api/account/reward/operation";
        _this._walletUrl = "/api/account/wallet";
        return _this;
    }
    Object.defineProperty(AccountEndpoint.prototype, "usersRegisterUrl", {
        get: function () { return this.configurations.baseUrl + this._usersRegisterUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "usersUrl", {
        get: function () { return this.configurations.baseUrl + this._usersUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "userByUserNameUrl", {
        get: function () { return this.configurations.baseUrl + this._userByUserNameUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "currentUserUrl", {
        get: function () { return this.configurations.baseUrl + this._currentUserUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "currentUserPreferencesUrl", {
        get: function () { return this.configurations.baseUrl + this._currentUserPreferencesUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "unblockUserUrl", {
        get: function () { return this.configurations.baseUrl + this._unblockUserUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "resetUserPasswordUrl", {
        get: function () { return this.configurations.baseUrl + this._resetUserPasswordUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "rolesUrl", {
        get: function () { return this.configurations.baseUrl + this._rolesUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "roleByRoleNameUrl", {
        get: function () { return this.configurations.baseUrl + this._roleByRoleNameUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "permissionsUrl", {
        get: function () { return this.configurations.baseUrl + this._permissionsUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "permissionsTreeUrl", {
        get: function () { return this.configurations.baseUrl + this._permissionsTreeUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "userPhonebooksUrl", {
        get: function () { return this.configurations.baseUrl + this._userPhonebooksUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "userVehiclesUrl", {
        get: function () { return this.configurations.baseUrl + this._userVehiclesUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "userCardIdsUrl", {
        get: function () { return this.configurations.baseUrl + this._userCardIdsUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "configUrl", {
        get: function () { return this.configurations.baseUrl + this._configUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "walletTransactUrl", {
        get: function () { return this.configurations.baseUrl + this._walletTransactUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "walletByUserUrl", {
        get: function () { return this.configurations.baseUrl + this._walletUrl + '/user'; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountEndpoint.prototype, "rewardTransactUrl", {
        get: function () { return this.configurations.baseUrl + this._rewardTransactUrl; },
        enumerable: true,
        configurable: true
    });
    AccountEndpoint.prototype.loadConfig = function () {
        var _this = this;
        return this.http.get(this.configUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.loadConfig(); });
        }));
    };
    AccountEndpoint.prototype.getUserEndpoint = function (userId) {
        var _this = this;
        var endpointUrl = userId ? this.usersUrl + "/" + userId : this.currentUserUrl;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUserEndpoint(userId); });
        }));
    };
    AccountEndpoint.prototype.getUserByUserNameEndpoint = function (userName) {
        var _this = this;
        var endpointUrl = this.userByUserNameUrl + "/" + userName;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUserByUserNameEndpoint(userName); });
        }));
    };
    AccountEndpoint.prototype.getUsersEndpoint = function (page, pageSize, institutionId) {
        var _this = this;
        var endpointUrl = page && pageSize ? this.usersUrl + "/" + page + "/" + pageSize : this.usersUrl + "?institutionId=" + institutionId;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUsersEndpoint(page, pageSize, institutionId); });
        }));
    };
    AccountEndpoint.prototype.getUsersSieveEndpoint = function (filter) {
        return this.getSieve(this.usersUrl + '/sieve/list', filter);
    };
    AccountEndpoint.prototype.getNewUserEndpoint = function (userObject) {
        var _this = this;
        return this.http.post(this.usersUrl, JSON.stringify(userObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getNewUserEndpoint(userObject); });
        }));
    };
    AccountEndpoint.prototype.getRegisterUserEndpoint = function (userObject) {
        var _this = this;
        return this.http.post(this.usersRegisterUrl, JSON.stringify(userObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getRegisterUserEndpoint(userObject); });
        }));
    };
    AccountEndpoint.prototype.getUpdateUserEndpoint = function (userObject, userId) {
        var _this = this;
        var endpointUrl = userId ? this.usersUrl + "/" + userId : this.currentUserUrl;
        return this.http.put(endpointUrl, JSON.stringify(userObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateUserEndpoint(userObject, userId); });
        }));
    };
    AccountEndpoint.prototype.getChangePasswordUserEndpoint = function (userObject, userId) {
        var _this = this;
        var endpointUrl = userId ? this.usersUrl + "/" + userId + "/changepassword" : this.currentUserUrl;
        return this.http.put(endpointUrl, JSON.stringify(userObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateUserEndpoint(userObject, userId); });
        }));
    };
    AccountEndpoint.prototype.getPatchUpdateUserEndpoint = function (valueOrPatch, opOrUserId, path, from, userId) {
        var _this = this;
        var endpointUrl;
        var patchDocument;
        if (path) {
            endpointUrl = userId ? this.usersUrl + "/" + userId : this.currentUserUrl;
            patchDocument = from ?
                [{ "value": valueOrPatch, "path": path, "op": opOrUserId, "from": from }] :
                [{ "value": valueOrPatch, "path": path, "op": opOrUserId }];
        }
        else {
            endpointUrl = opOrUserId ? this.usersUrl + "/" + opOrUserId : this.currentUserUrl;
            patchDocument = valueOrPatch;
        }
        return this.http.patch(endpointUrl, JSON.stringify(patchDocument), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getPatchUpdateUserEndpoint(valueOrPatch, opOrUserId, path, from, userId); });
        }));
    };
    AccountEndpoint.prototype.getUserPreferencesEndpoint = function () {
        var _this = this;
        return this.http.get(this.currentUserPreferencesUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUserPreferencesEndpoint(); });
        }));
    };
    AccountEndpoint.prototype.getUpdateUserPreferencesEndpoint = function (configuration) {
        var _this = this;
        return this.http.put(this.currentUserPreferencesUrl, JSON.stringify(configuration), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateUserPreferencesEndpoint(configuration); });
        }));
    };
    AccountEndpoint.prototype.getUnblockUserEndpoint = function (userId) {
        var _this = this;
        var endpointUrl = this.unblockUserUrl + "/" + userId;
        return this.http.put(endpointUrl, null, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUnblockUserEndpoint(userId); });
        }));
    };
    AccountEndpoint.prototype.getResetUserEndpoint = function (email) {
        var _this = this;
        var endpointUrl = "" + this.resetUserPasswordUrl;
        return this.http.post(endpointUrl, JSON.stringify(email), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getResetUserEndpoint(email); });
        }));
    };
    AccountEndpoint.prototype.getDeleteUserEndpoint = function (userId) {
        var _this = this;
        var endpointUrl = this.usersUrl + "/" + userId;
        return this.http.delete(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getDeleteUserEndpoint(userId); });
        }));
    };
    AccountEndpoint.prototype.getRoleEndpoint = function (roleId) {
        var _this = this;
        var endpointUrl = this.rolesUrl + "/" + roleId;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getRoleEndpoint(roleId); });
        }));
    };
    AccountEndpoint.prototype.getRoleByRoleNameEndpoint = function (roleName) {
        var _this = this;
        var endpointUrl = this.roleByRoleNameUrl + "/" + roleName;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getRoleByRoleNameEndpoint(roleName); });
        }));
    };
    AccountEndpoint.prototype.getRolesEndpoint = function (page, pageSize, institutionId) {
        var _this = this;
        var endpointUrl = page && pageSize ? this.rolesUrl + "/" + page + "/" + pageSize : this.rolesUrl + "?institutionId=" + institutionId;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getRolesEndpoint(page, pageSize, institutionId); });
        }));
    };
    AccountEndpoint.prototype.getRolesSieveEndpoint = function (filter) {
        return this.getSieve(this.rolesUrl + '/sieve/list', filter);
    };
    AccountEndpoint.prototype.getNewRoleEndpoint = function (roleObject) {
        var _this = this;
        return this.http.post(this.rolesUrl, JSON.stringify(roleObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getNewRoleEndpoint(roleObject); });
        }));
    };
    AccountEndpoint.prototype.getUpdateRoleEndpoint = function (roleObject, roleId) {
        var _this = this;
        var endpointUrl = this.rolesUrl + "/" + roleId;
        return this.http.put(endpointUrl, JSON.stringify(roleObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateRoleEndpoint(roleObject, roleId); });
        }));
    };
    AccountEndpoint.prototype.getDeleteRoleEndpoint = function (roleId) {
        var _this = this;
        var endpointUrl = this.rolesUrl + "/" + roleId;
        return this.http.delete(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getDeleteRoleEndpoint(roleId); });
        }));
    };
    AccountEndpoint.prototype.getPermissionsEndpoint = function () {
        var _this = this;
        return this.http.get(this.permissionsUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getPermissionsEndpoint(); });
        }));
    };
    AccountEndpoint.prototype.getPermissionsTreeEndpoint = function () {
        var _this = this;
        return this.http.get(this.permissionsTreeUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getPermissionsTreeEndpoint(); });
        }));
    };
    //phonebooks
    AccountEndpoint.prototype.getUserPhonebooksEndpoint = function (userId, page, pageSize) {
        var _this = this;
        var endpointUrl = page && pageSize ? this.userPhonebooksUrl + "/list/" + page + "/" + pageSize + "?userId=" + userId : this.userPhonebooksUrl + "/list?userId=" + userId;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUserPhonebooksEndpoint(userId, page, pageSize); });
        }));
    };
    AccountEndpoint.prototype.getNewUserPhonebookEndpoint = function (userPhonebookObject) {
        var _this = this;
        return this.http.post(this.userPhonebooksUrl, JSON.stringify(userPhonebookObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getNewUserPhonebookEndpoint(userPhonebookObject); });
        }));
    };
    AccountEndpoint.prototype.getUpdateUserPhonebookEndpoint = function (userPhonebookObject, userPhonebookId) {
        var _this = this;
        var endpointUrl = this.userPhonebooksUrl + "/update/" + userPhonebookId;
        return this.http.put(endpointUrl, JSON.stringify(userPhonebookObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateUserPhonebookEndpoint(userPhonebookObject, userPhonebookId); });
        }));
    };
    AccountEndpoint.prototype.getDeleteUserPhonebookEndpoint = function (userPhonebookId) {
        var _this = this;
        var endpointUrl = this.userPhonebooksUrl + "/delete/" + userPhonebookId;
        return this.http.delete(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getDeleteUserPhonebookEndpoint(userPhonebookId); });
        }));
    };
    //vehicles
    AccountEndpoint.prototype.getUserVehiclesEndpoint = function (userId, page, pageSize) {
        var _this = this;
        var endpointUrl = page && pageSize ? this.userVehiclesUrl + "/list/" + page + "/" + pageSize + "?userId=" + userId : this.userVehiclesUrl + "/list?userId=" + userId;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUserPhonebooksEndpoint(userId, page, pageSize); });
        }));
    };
    AccountEndpoint.prototype.getNewUserVehicleEndpoint = function (obj) {
        var _this = this;
        return this.http.post(this.userVehiclesUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getNewUserPhonebookEndpoint(obj); });
        }));
    };
    AccountEndpoint.prototype.getUpdateUserVehicleEndpoint = function (obj, id) {
        var _this = this;
        var endpointUrl = this.userVehiclesUrl + "/update/" + id;
        return this.http.put(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateUserVehicleEndpoint(obj, id); });
        }));
    };
    AccountEndpoint.prototype.getDeleteUserVehicleEndpoint = function (id) {
        var _this = this;
        var endpointUrl = this.userVehiclesUrl + "/delete/" + id;
        return this.http.delete(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getDeleteUserVehicleEndpoint(id); });
        }));
    };
    //card ids
    AccountEndpoint.prototype.getUserCardIdsEndpoint = function (userId, page, pageSize) {
        var _this = this;
        var endpointUrl = page && pageSize ? this.userCardIdsUrl + "/list/" + page + "/" + pageSize + "?userId=" + userId : this.userCardIdsUrl + "/list?userId=" + userId;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUserPhonebooksEndpoint(userId, page, pageSize); });
        }));
    };
    AccountEndpoint.prototype.getNewUserCardIdEndpoint = function (obj) {
        var _this = this;
        return this.http.post(this.userCardIdsUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getNewUserPhonebookEndpoint(obj); });
        }));
    };
    AccountEndpoint.prototype.getNewUserCardIdActivateEndpoint = function (obj) {
        var _this = this;
        return this.http.post(this.userCardIdsUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getNewUserPhonebookEndpoint(obj); });
        }));
    };
    AccountEndpoint.prototype.getUpdateUserCardIdEndpoint = function (obj, id) {
        var _this = this;
        var endpointUrl = this.userCardIdsUrl + "/update/" + id;
        return this.http.put(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateUserCardIdEndpoint(obj, id); });
        }));
    };
    AccountEndpoint.prototype.getDeleteUserCardIdEndpoint = function (id) {
        var _this = this;
        var endpointUrl = this.userCardIdsUrl + "/delete/" + id;
        return this.http.delete(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getDeleteUserCardIdEndpoint(id); });
        }));
    };
    AccountEndpoint.prototype.getWalletByUserId = function (userId) {
        var _this = this;
        var endpointUrl = this.walletByUserUrl + "/" + userId;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getWalletByUserId(userId); });
        }));
    };
    AccountEndpoint.prototype.getTransactWalletEndpoint = function (model) {
        var _this = this;
        var endpointUrl = this.walletTransactUrl + "/" + model.walletId;
        return this.http.put(endpointUrl, JSON.stringify(model), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getTransactWalletEndpoint(model); });
        }));
    };
    AccountEndpoint.prototype.getTransactRewardEndpoint = function (model) {
        var _this = this;
        var endpointUrl = this.rewardTransactUrl + "/" + model.rewardId;
        return this.http.put(endpointUrl, JSON.stringify(model), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getTransactRewardEndpoint(model); });
        }));
    };
    AccountEndpoint.prototype.getSieve = function (url, filter) {
        var _this = this;
        var headers = this.getRequestHeaders();
        var httpParams = { fromObject: filter };
        var options = { params: new http_1.HttpParams(httpParams), headers: headers.headers };
        return this.http.get(url, options).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getSieve(url, filter); });
        }));
    };
    AccountEndpoint = __decorate([
        core_1.Injectable()
    ], AccountEndpoint);
    return AccountEndpoint;
}(endpoint_factory_service_1.EndpointFactory));
exports.AccountEndpoint = AccountEndpoint;
//# sourceMappingURL=account-endpoint.service.js.map