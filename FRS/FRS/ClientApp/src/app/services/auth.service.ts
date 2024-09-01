import { Injectable } from '@angular/core';
import { Router, NavigationExtras } from "@angular/router";
import { Observable, Subject } from 'rxjs';
import { map } from 'rxjs/operators';

import { LocalStoreManager } from './local-store-manager.service';
import { EndpointFactory } from './endpoint-factory.service';
import { ConfigurationService } from './configuration.service';
import { DBkeys } from './db-Keys';
import { JwtHelper } from './jwt-helper';
import { Utilities } from './utilities';
import { LoginResponse, IdToken, Login2FAResponse, Resend2FAResponse } from '../models/login-response.model';
import { FRSHubConnections, User } from '../models/user.model';
import { Permission, PermissionNames, PermissionValues } from '../models/permission.model';
import { ResetPassword } from '../models/user-login.model';
import * as coreSignalR from '@aspnet/signalr';
import { connect } from 'tls';

@Injectable()
export class AuthService {

  public get isEnableMFA() { return this.configurations.enableMFA; }
  public get isEnableMFAValidation() { return this.configurations.enableMFAValidation; }
  public get loginUrl() { return this.configurations.loginUrl; }
  public get login2FAUrl() { return this.configurations.multifactorUrl; }
  public get homeUrl() { return this.configurations.homeUrl; }
  public get registerUrl() { return this.configurations.registerUrl; }
  public get resetPasswordUrl() { return this.configurations.resetPasswordUrl; }

  public registerRedirectUrl: string;
  public loginRedirectUrl: string;
  public logoutRedirectUrl: string;


  public resetUserId = '';
  public resetCode = '';

  public reLoginDelegate: () => void;

  private previousIsLoggedInCheck = false;
  private _loginStatus = new Subject<boolean>();


  constructor(private router: Router, public connections: FRSHubConnections, private configurations: ConfigurationService, private endpointFactory: EndpointFactory, private localStorage: LocalStoreManager) {
    this.initializeLoginStatus();
  }


  private initializeLoginStatus() {
    this.localStorage.getInitEvent().subscribe(() => {
      this.reevaluateLoginStatus();
    });
  }


  gotoPage(page: string, preserveParams = true) {

    let navigationExtras: NavigationExtras = {
      queryParamsHandling: preserveParams ? "merge" : "", preserveFragment: preserveParams
    };


    this.router.navigate([page], navigationExtras);
  }


  redirectLoginUser() {
    let redirect = this.loginRedirectUrl && this.loginRedirectUrl != '/' && this.loginRedirectUrl != ConfigurationService.defaultHomeUrl ? this.loginRedirectUrl : this.homeUrl;
    this.loginRedirectUrl = null;


    let urlParamsAndFragment = Utilities.splitInTwo(redirect, '#');
    let urlAndParams = Utilities.splitInTwo(urlParamsAndFragment.firstPart, '?');

    let navigationExtras: NavigationExtras = {
      fragment: urlParamsAndFragment.secondPart,
      queryParams: Utilities.getQueryParamsFromString(urlAndParams.secondPart),
      queryParamsHandling: "merge"
    };

    this.router.navigate([urlAndParams.firstPart], navigationExtras);
  }


  redirectLogoutUser() {
    let redirect = this.logoutRedirectUrl ? this.logoutRedirectUrl : this.loginUrl;
    this.logoutRedirectUrl = null;

    this.router.navigate([redirect]);
  }


  redirectForLogin() {
    this.loginRedirectUrl = this.router.url;
    this.router.navigate([this.loginUrl]);
  }

  redirectForRegister() {
    this.registerRedirectUrl = this.router.url;
    this.router.navigate([this.registerUrl]);
  }

  redirectForLogin2FA() {
    this.loginRedirectUrl = this.router.url;
    this.router.navigate([this.login2FAUrl]);
  }

  redirectToHome() {
    this.loginRedirectUrl = this.router.url;
    this.router.navigate([this.homeUrl]);
  }

  reLogin() {

    this.localStorage.deleteData(DBkeys.TOKEN_EXPIRES_IN);

    if (this.reLoginDelegate) {
      this.reLoginDelegate();
    }
    else {
      this.redirectForLogin();
    }
  }


  refreshLogin() {
    console.log('refreshing login.');
    return this.endpointFactory.getRefreshLoginEndpoint<LoginResponse>().pipe(
      map(response => this.processLoginResponse(response, this.rememberMe)));
  }


  login(userName: string, password: string, institutionCode: string, rememberMe?: boolean, isAD?: boolean, isValidateMfa?: boolean) {

    if (this.isLoggedIn)
      this.logout();

    return this.endpointFactory.getLoginEndpoint<LoginResponse>(userName, password, institutionCode, 'false', isAD).pipe(
      map(response => this.processLoginResponse(response, rememberMe, isValidateMfa)));
  }

  login2FA(userId: string, code: string) {
    return this.endpointFactory.getLogin2FAEndpoint<Login2FAResponse>(userId, code).pipe(
      map(response => this.processLogin2FAResponse(response)));
  }

  getResendCode2FA(userId: string) {
    return this.endpointFactory.getResendCode2FAEndpoint<Resend2FAResponse>(userId).pipe(
      map(response => this.processResend2FAResponse(response)));
  }

  loginExternal(userName: string) {

    return this.endpointFactory.getLoginEndpoint<LoginResponse>(userName, null, null, 'true').pipe(
      map(response => this.processLoginResponse(response, false)));
  }

  forgotPassword(email: string, institutionCode?: string) {

    if (this.isLoggedIn)
      this.logout();

    return this.endpointFactory.getForgotPasswordEndpoint<any>(email, institutionCode);
  }

  resetPassword(resetPassword: ResetPassword) {

    if (this.isLoggedIn)
      this.logout();

    return this.endpointFactory.getResetPasswordEndpoint<any>(resetPassword);
  }

  getUserPermissions(roleNames: string[]) {
    return this.endpointFactory.getUserPermissions<PermissionValues[]>(roleNames).pipe(
      map(permissions => {
        console.log(permissions);
        this.saveUserPermissionsDetails(permissions);
        return permissions; // Ensure you return the permissions if needed
      })
    );
  }

  private processLogin2FAResponse(response: Login2FAResponse) {

    if (!response.validated)
      throw new Error("Invalid code");

    var user = this.getCurrentUser(true);
    user.last2FAValidatedTime = (new Date()).toString();

    this.localStorage.savePermanentData(user, DBkeys.CURRENT_USER);
    this.reevaluateLoginStatus(user);

    return user;
  }

  private processResend2FAResponse(response: Resend2FAResponse) {

    if (!response.sent)
      throw new Error(`An error has occurred. Unable to resend code.`);

    return true;
  }

  private processLoginResponse(response: LoginResponse, rememberMe: boolean, isValidateMfa?: boolean) {

    let accessToken = response.access_token;

    if (accessToken == null)
      throw new Error("Received accessToken was empty");

    let idToken = response.id_token;
    let refreshToken = response.refresh_token || this.refreshToken;
    let expiresIn = response.expires_in;

    let tokenExpiryDate = new Date();
    tokenExpiryDate.setSeconds(tokenExpiryDate.getSeconds() + expiresIn);

    let accessTokenExpiry = tokenExpiryDate;

    //console.log(accessTokenExpiry);
    //console.log(new Date());

    let jwtHelper = new JwtHelper();
    let decodedIdToken = <IdToken>jwtHelper.decodeToken(response.id_token);

    let permissions: PermissionValues[] = Array.isArray(decodedIdToken.permission) ? decodedIdToken.permission : [decodedIdToken.permission];

    if (!this.isLoggedIn)
      this.configurations.import(decodedIdToken.configuration);

    let user = new User(
      decodedIdToken.sub,
      decodedIdToken.name,
      decodedIdToken.fullname,
      decodedIdToken.email,
      decodedIdToken.jobtitle,
      decodedIdToken.phone,
      Array.isArray(decodedIdToken.role) ? decodedIdToken.role : [decodedIdToken.role],
      decodedIdToken.institutionId);
    user.isEnabled = true;
    user.last2FAValidatedTime = null;
    this.saveUserDetails(user, permissions, accessToken, idToken, refreshToken, accessTokenExpiry, rememberMe);

    this.reevaluateLoginStatus(user, isValidateMfa);
    this.getUserPermissions(Array.isArray(decodedIdToken.role) ? decodedIdToken.role : [decodedIdToken.role])
      .subscribe({
        next: (permissions) => {
          console.log('Permissions:', permissions);
        },
        error: (error) => {
          console.error('Error getting permissions:', error);
        }
      });

    return user;
  }


  private saveUserDetails(user: User, permissions: PermissionValues[], accessToken: string, idToken: string, refreshToken: string, expiresIn: Date, rememberMe: boolean) {

    //if (rememberMe) {
      this.localStorage.savePermanentData(accessToken, DBkeys.ACCESS_TOKEN);
      this.localStorage.savePermanentData(idToken, DBkeys.ID_TOKEN);
      this.localStorage.savePermanentData(refreshToken, DBkeys.REFRESH_TOKEN);
      this.localStorage.savePermanentData(expiresIn, DBkeys.TOKEN_EXPIRES_IN);
      this.localStorage.savePermanentData(permissions, DBkeys.USER_PERMISSIONS);
      this.localStorage.savePermanentData(user, DBkeys.CURRENT_USER);
    //}
    //else {
    //  this.localStorage.saveSyncedSessionData(accessToken, DBkeys.ACCESS_TOKEN);
    //  this.localStorage.saveSyncedSessionData(idToken, DBkeys.ID_TOKEN);
    //  this.localStorage.saveSyncedSessionData(refreshToken, DBkeys.REFRESH_TOKEN);
    //  this.localStorage.saveSyncedSessionData(expiresIn, DBkeys.TOKEN_EXPIRES_IN);
    //  this.localStorage.saveSyncedSessionData(permissions, DBkeys.USER_PERMISSIONS);
    //  this.localStorage.saveSyncedSessionData(user, DBkeys.CURRENT_USER);
    //}

    this.localStorage.savePermanentData(rememberMe, DBkeys.REMEMBER_ME);
  }

  private saveUserPermissionsDetails(permissions: PermissionValues[]) {
    this.localStorage.savePermanentData(permissions, DBkeys.USER_PERMISSIONS);
  }



  logout(): void {
    if (this.connections.userHubConnection != null) {
      this.disconnectSignalRConnection(this.connections.userHubConnection, '');
    }

    this.localStorage.deleteData(DBkeys.ACCESS_TOKEN);
    this.localStorage.deleteData(DBkeys.ID_TOKEN);
    this.localStorage.deleteData(DBkeys.REFRESH_TOKEN);
    this.localStorage.deleteData(DBkeys.TOKEN_EXPIRES_IN);
    this.localStorage.deleteData(DBkeys.USER_PERMISSIONS);
    this.localStorage.deleteData(DBkeys.CURRENT_USER);

    this.configurations.clearLocalChanges();
    //this.reevaluateLoginStatus();

    if (this.connections.sessionInterval) {
      clearInterval(this.connections.sessionInterval);
    }
  }


  private reevaluateLoginStatus(currentUser?: User, isValidateMfa?: boolean) {
    let d = new Date();
    d.setMinutes(d.getMinutes() - 1); //5 minutes ago

    let user = currentUser || this.localStorage.getDataObject<User>(DBkeys.CURRENT_USER);
    let isLoggedIn = false;
    //console.log('reevaluateLoginStatus', this.isEnableMFA, isValidateMfa, this.isEnableMFA);
    if (this.isEnableMFA) {
      isLoggedIn = user != null && user.last2FAValidatedTime != null;// && (new Date(user.last2FAValidatedTime) > d);
    }
    else {
      isLoggedIn = user != null;
    }

    if (this.previousIsLoggedInCheck != isLoggedIn) {
      setTimeout(() => {
        this._loginStatus.next(isLoggedIn);
      });
    }

    this.previousIsLoggedInCheck = isLoggedIn;
  }


  getLoginStatusEvent(): Observable<boolean> {
    return this._loginStatus.asObservable();
  }

  getCurrentUser(skipEvaluation?: boolean): User {

    let user = this.localStorage.getDataObject<User>(DBkeys.CURRENT_USER);
    if (!skipEvaluation)
      this.reevaluateLoginStatus(user);

    return user;
  }

  get currentUser(): User {

    let user = this.localStorage.getDataObject<User>(DBkeys.CURRENT_USER);
    this.reevaluateLoginStatus(user);

    return user;
  }

  get userPermissions(): PermissionValues[] {
    return this.localStorage.getDataObject<PermissionValues[]>(DBkeys.USER_PERMISSIONS) || [];
  }

  get accessToken(): string {

    this.reevaluateLoginStatus();
    return this.localStorage.getData(DBkeys.ACCESS_TOKEN);
  }

  get accessTokenExpiryDate(): Date {

    this.reevaluateLoginStatus();
    return this.localStorage.getDataObject<Date>(DBkeys.TOKEN_EXPIRES_IN, true);
  }

  get isSessionExpired(): boolean {

    if (this.accessTokenExpiryDate == null) {
      return true;
    }
    //console.log(this.accessTokenExpiryDate);
    //console.log(new Date());
    return !(this.accessTokenExpiryDate.valueOf() > new Date().valueOf());
  }


  get idToken(): string {

    this.reevaluateLoginStatus();
    return this.localStorage.getData(DBkeys.ID_TOKEN);
  }

  get refreshToken(): string {

    this.reevaluateLoginStatus();
    return this.localStorage.getData(DBkeys.REFRESH_TOKEN);
  }

  get isLoggedIn(): boolean {
    return this.isEnableMFA ? this.currentUser != null && this.currentUser.last2FAValidatedTime != null
      : this.currentUser != null;
  }

  get rememberMe(): boolean {
    return this.localStorage.getDataObject<boolean>(DBkeys.REMEMBER_ME) == true;
  }

  signalRConnection(url: string, skipAuth?: boolean, currentConnection?: coreSignalR.HubConnection): coreSignalR.HubConnection {
    let connection: coreSignalR.HubConnection = null;
    console.log(this.connections);
    //if (this.connections.hasOwnProperty(url)) {
    //  connection = this.connections[url];
    //}
    //else {
    if (this.currentUser != null || skipAuth) {
      if (!connection) {
        if (!currentConnection) {
          connection = new coreSignalR.HubConnectionBuilder()
            .withUrl(url, {
              transport: coreSignalR.HttpTransportType.WebSockets | coreSignalR.HttpTransportType.LongPolling
            })
            .configureLogging(coreSignalR.LogLevel.Trace)
            .configureLogging({
              log: function (logLevel, message) {
                //console.log(logLevel);
                console.log(url + " - " + new Date().toISOString() + ": " + message);
              }
            })
            .build();
        } else {
          connection = currentConnection;
        }
      }

      this.connections[url] = connection;
    }
    //}

    connection.keepAliveIntervalInMilliseconds = ConfigurationService.defaultSignalRKeepAliveInterval * 1000 * 60;
    connection.serverTimeoutInMilliseconds = ConfigurationService.defaultSignalRServerTimeout * 1000 * 60;
    this.connections.forceStop = false;
    if (connection.state !== coreSignalR.HubConnectionState.Connected) {
      if (this.connections.numOfTries > 10) {
        this.connections.numOfTries = 0;
        this.disconnectSignalRConnection(connection, '');
      } else {
        this.startConnection(url, connection);
      }
      
    }

    connection.onclose((e) => {
      if (!this.connections.forceStop) {
        if (!this.connections.numOfTries) this.connections.numOfTries = 0;
        //if (e) {
        console.log('FROM ON CLOSE:');
        console.log(e);
        this.connections.forceStop = false;
        this.connections.numOfTries++;
        this.startConnection(url, connection);
        //}
      }
    });


    return connection;
  }

  startConnection(url: string, connection: coreSignalR.HubConnection) {
    let $this = this;
    try {
      if (connection == null) {
        connection = new coreSignalR.HubConnectionBuilder()
          .withUrl(url, {
            transport: coreSignalR.HttpTransportType.WebSockets | coreSignalR.HttpTransportType.LongPolling
          })
          .configureLogging(coreSignalR.LogLevel.Trace)
          .build();
      }

      if (connection.state == coreSignalR.HubConnectionState.Disconnected) {
        connection.start().catch(err => {
          console.log(err);
          setTimeout(function () { $this.startConnection(url, connection) }, 5000);
        });
      }
    } catch (err) {
      setTimeout(function () { $this.startConnection(url, connection) }, 5000);
    }
  }

  disconnectSignalRConnection(connection: coreSignalR.HubConnection, url: string) {
    this.connections.forceStop = true;
    if (connection != null) {
      connection.off('onclose');
      connection.stop().catch(err => {
        console.log(err);
      });

      if (this.connections.hasOwnProperty(url)) {
        //connection = this.connections[url];
        delete this.connections[url];
      }

    }
  }
}
