"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.UserLogin2FA = exports.ResetPassword = exports.UserLogin = void 0;
var UserLogin = /** @class */ (function () {
    function UserLogin(email, password, rememberMe) {
        this.email = email;
        this.password = password;
        this.rememberMe = rememberMe;
    }
    return UserLogin;
}());
exports.UserLogin = UserLogin;
var ResetPassword = /** @class */ (function () {
    function ResetPassword(code, userId, password) {
        this.userId = userId;
        this.password = password;
        this.code = code;
    }
    return ResetPassword;
}());
exports.ResetPassword = ResetPassword;
var UserLogin2FA = /** @class */ (function () {
    function UserLogin2FA(userId, code) {
        this.userId = userId;
        this.code = code;
    }
    return UserLogin2FA;
}());
exports.UserLogin2FA = UserLogin2FA;
//# sourceMappingURL=user-login.model.js.map