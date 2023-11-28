"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var coreSignalR = require("@aspnet/signalr");
var signalR = require("@aspnet/signalr");
var AccountService = /** @class */ (function () {
    function AccountService(router, http, authService, accountEndpoint, departmentService, commonEndpoint, configurations) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.accountEndpoint = accountEndpoint;
        this.departmentService = departmentService;
        this.commonEndpoint = commonEndpoint;
        this.configurations = configurations;
        this._rolesChanged = new rxjs_1.Subject();
        this.connections = {};
        this._accountUserUrl = "/api/account/users";
        this._accountRoleUrl = "/api/account/roles";
    }
    AccountService_1 = AccountService;
    Object.defineProperty(AccountService.prototype, "accountUserUrl", {
        get: function () { return this.configurations.baseUrl + this._accountUserUrl; },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountService.prototype, "accountRoleUrl", {
        get: function () { return this.configurations.baseUrl + this._accountRoleUrl; },
        enumerable: true,
        configurable: true
    });
    AccountService.prototype.loadConfig = function () {
        return this.accountEndpoint.loadConfig();
    };
    AccountService.prototype.getUser = function (userId) {
        return this.accountEndpoint.getUserEndpoint(userId);
    };
    AccountService.prototype.getUserAndRoles = function (userId, institutionId) {
        return rxjs_1.forkJoin(this.accountEndpoint.getUserEndpoint(userId), this.accountEndpoint.getRolesEndpoint(null, null, institutionId), this.departmentService.getDepartments(null, null, institutionId), this.accountEndpoint.getUserPhonebooksEndpoint());
    };
    AccountService.prototype.getUsers = function (page, pageSize, institutionId) {
        return this.accountEndpoint.getUsersEndpoint(page, pageSize, institutionId);
    };
    AccountService.prototype.getUsersAndRoles = function (page, pageSize, institutionId) {
        return rxjs_1.forkJoin(this.accountEndpoint.getUsersEndpoint(page, pageSize, institutionId), this.accountEndpoint.getRolesEndpoint(null, null, institutionId), this.departmentService.getDepartments(null, null, institutionId), this.accountEndpoint.getUserPhonebooksEndpoint());
    };
    AccountService.prototype.getUsersByFilter = function (userFilter) {
        return this.accountEndpoint.getUsersSieveEndpoint(userFilter);
    };
    AccountService.prototype.getUsersAndRolesByFilters = function (userFilter) {
        return rxjs_1.forkJoin(this.accountEndpoint.getUsersSieveEndpoint(userFilter), this.accountEndpoint.getRolesEndpoint(null, null, userFilter.institutionId), this.departmentService.getDepartments(null, null, userFilter.institutionId), this.accountEndpoint.getUserPhonebooksEndpoint());
    };
    AccountService.prototype.updateUser = function (user) {
        var _this = this;
        if (user.id) {
            return this.accountEndpoint.getUpdateUserEndpoint(user, user.id);
        }
        else {
            return this.accountEndpoint.getUserByUserNameEndpoint(user.userName).pipe(operators_1.mergeMap(function (foundUser) {
                user.id = foundUser.id;
                return _this.accountEndpoint.getUpdateUserEndpoint(user, user.id);
            }));
        }
    };
    AccountService.prototype.changePassowrd = function (userId, currentPassword, newPassword) {
        return this.accountEndpoint.getUpdateUserEndpoint({ currentPassword: currentPassword, newPassword: newPassword }, userId);
    };
    AccountService.prototype.newUser = function (user) {
        return this.accountEndpoint.getNewUserEndpoint(user);
    };
    AccountService.prototype.registerUser = function (user) {
        return this.accountEndpoint.getRegisterUserEndpoint(user);
    };
    AccountService.prototype.getUserPreferences = function () {
        return this.accountEndpoint.getUserPreferencesEndpoint();
    };
    AccountService.prototype.updateUserPreferences = function (configuration) {
        return this.accountEndpoint.getUpdateUserPreferencesEndpoint(configuration);
    };
    AccountService.prototype.deleteUser = function (userOrUserId) {
        var _this = this;
        if (typeof userOrUserId === 'string' || userOrUserId instanceof String ||
            typeof userOrUserId === 'number' || userOrUserId instanceof Number) {
            return this.accountEndpoint.getDeleteUserEndpoint(userOrUserId).pipe(operators_1.tap(function (data) { return _this.onRolesUserCountChanged(data.roles); }));
        }
        else {
            if (userOrUserId.id) {
                return this.deleteUser(userOrUserId.id);
            }
            else {
                return this.accountEndpoint.getUserByUserNameEndpoint(userOrUserId.userName).pipe(operators_1.mergeMap(function (user) { return _this.deleteUser(user.id); }));
            }
        }
    };
    AccountService.prototype.resetPassword = function (userId) {
        return this.accountEndpoint.getResetUserEndpoint(userId);
    };
    AccountService.prototype.unblockUser = function (userId) {
        return this.accountEndpoint.getUnblockUserEndpoint(userId);
    };
    AccountService.prototype.changePassword = function (userEdit, userId) {
        return this.accountEndpoint.getChangePasswordUserEndpoint(userEdit, userId);
    };
    AccountService.prototype.getUserReportByFilter = function (filter) {
        return this.commonEndpoint.getSieve(this.accountUserUrl + '/report/sieve/list', filter);
    };
    AccountService.prototype.downloadUserReport = function (filter) {
        return this.commonEndpoint.getFile(this.accountUserUrl + '/report/export', filter);
    };
    //role report
    AccountService.prototype.getUserRoleReportByFilter = function (filter) {
        return this.commonEndpoint.getSieve(this.accountRoleUrl + '/report/sieve/list', filter);
    };
    AccountService.prototype.downloadUserRoleReport = function (filter) {
        return this.commonEndpoint.getFile(this.accountRoleUrl + '/report/export', filter);
    };
    AccountService.prototype.getWallet = function (userId) {
        return this.accountEndpoint.getWalletByUserId(userId);
    };
    AccountService.prototype.transactWallet = function (model) {
        return this.accountEndpoint.getTransactWalletEndpoint(model);
    };
    AccountService.prototype.transactReward = function (model) {
        return this.accountEndpoint.getTransactRewardEndpoint(model);
    };
    AccountService.prototype.userHasPermission = function (permissionValue) {
        return this.permissions.some(function (p) { return p == permissionValue; });
    };
    AccountService.prototype.userHasPermissions = function (permissionValues) {
        var isAllowed = true; //default allow menu as some have no acl yet
        if (permissionValues) {
            var _loop_1 = function (pv) {
                isAllowed = this_1.permissions.some(function (p) { return p == pv; });
                if (!isAllowed)
                    return "break";
            };
            var this_1 = this;
            for (var _i = 0, permissionValues_1 = permissionValues; _i < permissionValues_1.length; _i++) {
                var pv = permissionValues_1[_i];
                var state_1 = _loop_1(pv);
                if (state_1 === "break")
                    break;
            }
        }
        return isAllowed;
    };
    AccountService.prototype.refreshLoggedInUser = function () {
        return this.authService.refreshLogin();
    };
    AccountService.prototype.getRoles = function (page, pageSize, institutionId) {
        return this.accountEndpoint.getRolesEndpoint(page, pageSize, institutionId);
    };
    AccountService.prototype.getRolesAndPermissionsByFilters = function (filter) {
        return rxjs_1.forkJoin(this.accountEndpoint.getRolesSieveEndpoint(filter), this.accountEndpoint.getPermissionsTreeEndpoint());
    };
    AccountService.prototype.getRolesAndPermissionListByFilters = function (filter) {
        return rxjs_1.forkJoin(this.accountEndpoint.getRolesSieveEndpoint(filter), this.accountEndpoint.getPermissionsEndpoint());
    };
    AccountService.prototype.getRolesAndPermissions = function (page, pageSize, institutionId) {
        return rxjs_1.forkJoin(this.accountEndpoint.getPermissionsEndpoint(), this.accountEndpoint.getPermissionsTreeEndpoint());
    };
    AccountService.prototype.updateRole = function (role) {
        var _this = this;
        if (role.id) {
            return this.accountEndpoint.getUpdateRoleEndpoint(role, role.id).pipe(operators_1.tap(function (data) { return _this.onRolesChanged([role], AccountService_1.roleModifiedOperation); }));
        }
        else {
            return this.accountEndpoint.getRoleByRoleNameEndpoint(role.name).pipe(operators_1.mergeMap(function (foundRole) {
                role.id = foundRole.id;
                return _this.accountEndpoint.getUpdateRoleEndpoint(role, role.id);
            }), operators_1.tap(function (data) { return _this.onRolesChanged([role], AccountService_1.roleModifiedOperation); }));
        }
    };
    AccountService.prototype.newRole = function (role) {
        var _this = this;
        return this.accountEndpoint.getNewRoleEndpoint(role).pipe(operators_1.tap(function (data) { return _this.onRolesChanged([role], AccountService_1.roleAddedOperation); }));
    };
    AccountService.prototype.deleteRole = function (roleOrRoleId) {
        var _this = this;
        if (typeof roleOrRoleId === 'number' || typeof roleOrRoleId === 'string' || roleOrRoleId instanceof String) {
            return this.accountEndpoint.getDeleteRoleEndpoint(roleOrRoleId).pipe(operators_1.tap(function (data) { return _this.onRolesChanged([data], AccountService_1.roleDeletedOperation); }));
        }
        else {
            if (roleOrRoleId.id) {
                return this.deleteRole(roleOrRoleId.id);
            }
            else {
                return this.accountEndpoint.getRoleByRoleNameEndpoint(roleOrRoleId.name).pipe(operators_1.mergeMap(function (role) { return _this.deleteRole(role.id); }));
            }
        }
    };
    AccountService.prototype.getPermissions = function () {
        return this.accountEndpoint.getPermissionsEndpoint();
    };
    AccountService.prototype.onRolesChanged = function (roles, op) {
        this._rolesChanged.next({ roles: roles, operation: op });
    };
    AccountService.prototype.onRolesUserCountChanged = function (roles) {
        return this.onRolesChanged(roles, AccountService_1.roleModifiedOperation);
    };
    AccountService.prototype.getRolesChangedEvent = function () {
        return this._rolesChanged.asObservable();
    };
    Object.defineProperty(AccountService.prototype, "permissions", {
        get: function () {
            return this.authService.userPermissions;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(AccountService.prototype, "currentUser", {
        get: function () {
            return this.authService.currentUser;
        },
        enumerable: true,
        configurable: true
    });
    AccountService.prototype.signalRConnection = function (url, skipAuth) {
        var connection;
        console.log(this.connections);
        //if (this.connections.hasOwnProperty(url)) {
        //  connection = this.connections[url];
        //}
        //else {
        if (this.authService.currentUser != null || skipAuth) {
            connection = new coreSignalR.HubConnectionBuilder()
                .withUrl(url, {
                transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
            })
                .configureLogging(coreSignalR.LogLevel.Trace)
                .configureLogging({
                log: function (logLevel, message) {
                    console.log(url + " - " + new Date().toISOString() + ": " + message);
                }
            })
                .build();
            this.connections[url] = connection;
        }
        //}
        if (connection.state !== coreSignalR.HubConnectionState.Connected) {
            connection.start().catch(function (err) {
                console.log(err);
            });
        }
        return connection;
    };
    AccountService.prototype.disconnectSignalRConnection = function (connection, url) {
        if (connection != null) {
            connection.stop().catch(function (err) {
                console.log(err);
            });
            if (this.connections.hasOwnProperty(url)) {
                //connection = this.connections[url];
                delete this.connections[url];
            }
        }
    };
    Object.defineProperty(AccountService.prototype, "pibSignalRConnection", {
        get: function () {
            if (this.authService.currentUser != null) {
                if (this.pibConnection == null) {
                    this.pibConnection = new signalR.HubConnectionBuilder()
                        .withUrl("http://localhost:91/signalr/patientHub", {
                        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
                    })
                        .build();
                }
                if (this.pibConnection.state !== signalR.HubConnectionState.Connected) {
                    this.pibConnection.start().catch(function (err) {
                        console.log(err);
                    });
                }
                return this.pibConnection;
            }
            return null;
        },
        enumerable: true,
        configurable: true
    });
    AccountService.prototype.getUserPhonebooks = function (userId, page, pageSize) {
        return this.accountEndpoint.getUserPhonebooksEndpoint(userId, page, pageSize);
    };
    AccountService.prototype.updateUserPhonebook = function (userPhonebook) {
        if (userPhonebook.id) {
            return this.accountEndpoint.getUpdateUserPhonebookEndpoint(userPhonebook, userPhonebook.id);
        }
    };
    AccountService.prototype.newUserPhonebook = function (userPhonebook) {
        return this.accountEndpoint.getNewUserPhonebookEndpoint(userPhonebook);
    };
    AccountService.prototype.deleteUserPhonebook = function (userPhonebookOrUserPhonebookId) {
        return this.accountEndpoint.getDeleteUserPhonebookEndpoint(userPhonebookOrUserPhonebookId);
    };
    AccountService.prototype.getUserVehicles = function (userId, page, pageSize) {
        return this.accountEndpoint.getUserVehiclesEndpoint(userId, page, pageSize);
    };
    AccountService.prototype.newUserVehicle = function (vehicle) {
        return this.accountEndpoint.getNewUserVehicleEndpoint(vehicle);
    };
    AccountService.prototype.updateUserVehicle = function (vehicle) {
        if (vehicle.id) {
            return this.accountEndpoint.getUpdateUserVehicleEndpoint(vehicle, vehicle.id);
        }
    };
    AccountService.prototype.deleteUserVehicle = function (id) {
        return this.accountEndpoint.getDeleteUserVehicleEndpoint(id);
    };
    AccountService.prototype.getUserCardIds = function (userId, page, pageSize) {
        return this.accountEndpoint.getUserCardIdsEndpoint(userId, page, pageSize);
    };
    AccountService.prototype.newUserCardId = function (cardId) {
        return this.accountEndpoint.getNewUserCardIdEndpoint(cardId);
    };
    AccountService.prototype.updateUserCardId = function (cardId) {
        if (cardId.id) {
            return this.accountEndpoint.getUpdateUserCardIdEndpoint(cardId, cardId.id);
        }
    };
    AccountService.prototype.deleteUserCardId = function (id) {
        return this.accountEndpoint.getDeleteUserCardIdEndpoint(id);
    };
    var AccountService_1;
    AccountService.roleAddedOperation = "add";
    AccountService.roleDeletedOperation = "delete";
    AccountService.roleModifiedOperation = "modify";
    AccountService = AccountService_1 = __decorate([
        core_1.Injectable()
    ], AccountService);
    return AccountService;
}());
exports.AccountService = AccountService;
//# sourceMappingURL=account.service.js.map