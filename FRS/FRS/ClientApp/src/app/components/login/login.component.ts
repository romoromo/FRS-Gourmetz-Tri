import { Component, OnInit, OnDestroy, Input } from "@angular/core";

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AuthService } from "../../services/auth.service";
import { ConfigurationService } from '../../services/configuration.service';
import { Utilities } from '../../services/utilities';
import { UserLogin } from '../../models/user-login.model';
import { Router } from "@angular/router";
import { FRSHubConnections } from "src/app/models/user.model";
import { AccountService } from "src/app/services/account.service";
import * as signalRCore from '@aspnet/signalr';
import { AppTranslationService } from "src/app/services/app-translation.service";

@Component({
  selector: "app-login",
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit, OnDestroy {

  userLogin = new UserLogin();
  isLoading = false;
  formResetToggle = true;
  modalClosedCallback: () => void;
  loginStatusSubscription: any;
  signInGoogleUrl = "/authorization/SignInWithGoogle";
  registerUrl = "/register";
  forgotPasswordUrl = "/forgotpassword";
  @Input()
  isModal = false;

  public languageoptions = [
    { value: 'en', viewValue: 'EN' },
    { value: 'cn', viewValue: 'CN' }
  ];

  selectedlanguage: string = 'en';
  constructor(private alertService: AlertService, public router: Router, private translationService: AppTranslationService, public connections: FRSHubConnections, private authService: AuthService, private configurations: ConfigurationService) {

  }

  setLanguage(ev) {
    this.translationService.changeLanguage(this.selectedlanguage);
  }

  ngOnInit() {
    if (this.loginStatusSubscription)
      this.loginStatusSubscription.unsubscribe();

    //this.authService.logoutRedirectUrl = this.router.url;
    this.userLogin.rememberMe = this.authService.rememberMe;
    this.userLogin.institutionCode = '';
    this.setInstitutionCode();

    //var queryParams: string = '';
    //var queryParamSection: string[] = [];
    //if (this.router.url.indexOf('?') > -1) {
    //  queryParams = this.router.url.split('?')[1];
    //  queryParamSection = queryParams.split('&');
    //}
    //if (queryParamSection && queryParamSection.length > 0) {
    //  var queryStrings = Utilities.getQueryParamsFromString(queryParams);
    //  this.signInGoogleUrl = this.signInGoogleUrl + '?institutioncode=' + queryStrings['institutioncode'];
    //  this.registerUrl = this.registerUrl + '?institutioncode=' + queryStrings['institutioncode'];
    //}

    
    if (this.getShouldRedirect()) {
      this.authService.redirectLoginUser();
    }
    else {
      this.loginStatusSubscription = this.authService.getLoginStatusEvent().subscribe(isLoggedIn => {
        if (this.getShouldRedirect()) {
          this.authService.redirectLoginUser();
        }
      });
    }
  }

  setInstitutionCode() {
    var queryParams: string = '';
    var queryParamSection: string[] = [];
    if (this.router.url.indexOf('?') > -1) {
      queryParams = this.router.url.split('?')[1];
      queryParamSection = queryParams.split('&');
    }
    if (queryParamSection && queryParamSection.length > 0) {
      var queryStrings = Utilities.getQueryParamsFromString(queryParams);
      let code = queryStrings['institutioncode'] ? queryStrings['institutioncode'] : queryStrings['institutionCode'];
      this.signInGoogleUrl = this.signInGoogleUrl + '?institutioncode=' + code;
      this.registerUrl = this.registerUrl + '?institutioncode=' + code;
      this.forgotPasswordUrl = this.forgotPasswordUrl + '?institutioncode=' + code;
      this.userLogin.institutionCode = code;
    }

  }

  redirectToForgotPassword() {
    window.open(this.forgotPasswordUrl, '_blank');
  }

  redirectToSignIn() {
    window.open(this.signInGoogleUrl, '_blank');
  }

  redirectToRegister() {
    window.open(this.registerUrl, '_blank');
  }

  ngOnDestroy() {
    if (this.loginStatusSubscription)
      this.loginStatusSubscription.unsubscribe();
  }

  redirectRegister() {
    this.authService.redirectForRegister();
  }

  getShouldRedirect() {
    return !this.isModal && this.authService.isLoggedIn && !this.authService.isSessionExpired;
  }


  showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  closeModal() {
    if (this.modalClosedCallback) {
      this.modalClosedCallback();
    }
  }


  login() {
    this.setInstitutionCode();
    this.alertService.resetStickyMessage();
    this.alertService.resetToastMessage();
    this.isLoading = true;
    this.alertService.startLoadingMessage("", "Attempting login...");

    this.authService.login(this.userLogin.email, this.userLogin.password, this.userLogin.institutionCode, this.userLogin.rememberMe, this.userLogin.isAD)
      .subscribe(
        user => {
          setTimeout(() => {
            this.alertService.stopLoadingMessage();
            this.isLoading = false;
            this.reset();
            if (this.connections.userHubConnection == null || this.connections.userHubConnection.state !== signalRCore.HubConnectionState.Connected) {
              //this.connections.userHubConnection = this.authService.signalRConnection(`/hub/user?email=${user.email}&status=ONLINE`, true);
              //let status = user.status ? user.status : 'ONLINE';
              this.connections.userHubConnection = this.authService.signalRConnection(`${this.configurations.baseUrl}/hub/user?email=${user.email}`, true, this.connections.userHubConnection);

            }

            if (!this.isModal) {
              this.alertService.showMessage("Login", `Welcome ${user.userName}!`, MessageSeverity.success);
            }
            else {
              this.alertService.showMessage("Login", `Session for ${user.userName} restored!`, MessageSeverity.success);
              setTimeout(() => {
                this.alertService.showStickyMessage("Session Restored", "Please try your last operation again", MessageSeverity.default);
              }, 500);

              this.closeModal();
            }
          }, 500);
        },
        error => {

          this.alertService.stopLoadingMessage();

          if (Utilities.checkNoNetwork(error)) {
            this.alertService.showStickyMessage(Utilities.noNetworkMessageCaption, Utilities.noNetworkMessageDetail, MessageSeverity.error);
            //this.offerAlternateHost();
          }
          else {
            let errorMessage = Utilities.findHttpResponseMessage("error_description", error);

            if (errorMessage)
              this.alertService.showStickyMessage("Unable to login", errorMessage, MessageSeverity.error);
            else
              this.alertService.showStickyMessage("Unable to login", "An error occured while logging in, please try again later.\nError: " + Utilities.getResponseBody(error), MessageSeverity.error);
          }

          setTimeout(() => {
            this.isLoading = false;
          }, 500);
        });
  }


  offerAlternateHost() {

    //if (Utilities.checkIsLocalHost(location.origin) && Utilities.checkIsLocalHost(this.configurations.baseUrl)) {
    //  this.alertService.showDialog("Dear Developer!\nIt appears your backend Web API service is not running...\n" +
    //    "Would you want to temporarily switch to the online Demo API below?(Or specify another)",
    //    DialogType.prompt,
    //    (value: string) => {
    //      this.configurations.baseUrl = value;
    //      this.alertService.showStickyMessage("API Changed!", "The target Web API has been changed to: " + value, MessageSeverity.warn);
    //    },
    //    null,
    //    null,
    //    null,
    //    this.configurations.fallbackBaseUrl);
    //}
  }


  reset() {
    this.formResetToggle = false;

    setTimeout(() => {
      this.formResetToggle = true;
    });
  }
}
