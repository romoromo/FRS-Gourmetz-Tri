import { Component, OnInit, OnDestroy, Input } from "@angular/core";

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AuthService } from "../../services/auth.service";
import { ConfigurationService } from '../../services/configuration.service';
import { Utilities } from '../../services/utilities';
import { UserLogin, UserLogin2FA } from '../../models/user-login.model';
import { Router } from "@angular/router";
import { FRSHubConnections } from "src/app/models/user.model";
import { AccountService } from "src/app/services/account.service";
import * as signalRCore from '@aspnet/signalr';
import { AppTranslationService } from "src/app/services/app-translation.service";

@Component({
  selector: "multi-factor-login",
  templateUrl: './multi-factor-login.component.html',
  styleUrls: ['./multi-factor-login.component.css']
})

export class MultiFactorLoginComponent implements OnInit, OnDestroy {

  userLogin = new UserLogin2FA();
  isLoading = false;
  isResending = false;
  formResetToggle = true;
  modalClosedCallback: () => void;
  loginStatusSubscription: any;
  @Input()
  isModal = false;

  constructor(private alertService: AlertService, public router: Router, private translationService: AppTranslationService, public connections: FRSHubConnections, private authService: AuthService, private configurations: ConfigurationService) {

  }

  ngOnInit() {
    if (this.loginStatusSubscription)
      this.loginStatusSubscription.unsubscribe();

  }

  ngOnDestroy() {
    if (this.loginStatusSubscription)
      this.loginStatusSubscription.unsubscribe();
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
    this.alertService.resetStickyMessage();
    this.alertService.resetToastMessage();
    this.isLoading = true;
    this.alertService.startLoadingMessage("", "Validating confirmation code...");
    
    this.authService.login2FA(this.authService.currentUser.id, this.userLogin.code)
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

            this.authService.redirectToHome();
          }, 500);
        },
        error => {

          this.alertService.stopLoadingMessage();

          if (Utilities.checkNoNetwork(error)) {
            this.alertService.showStickyMessage(Utilities.noNetworkMessageCaption, Utilities.noNetworkMessageDetail, MessageSeverity.error);
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

  resend() {
    this.alertService.resetStickyMessage();
    this.alertService.resetToastMessage();
    this.isResending = true;
    this.alertService.startLoadingMessage("", "Resending confirmation code...");

    this.authService.getResendCode2FA(this.authService.currentUser.id)
      .subscribe(
        user => {
          setTimeout(() => {
            this.alertService.stopLoadingMessage();
            this.isResending = false;
            this.alertService.showMessage("Resend", "Code is sent.", MessageSeverity.success);
          }, 500);
        },
        error => {

          this.alertService.stopLoadingMessage();

          if (Utilities.checkNoNetwork(error)) {
            this.alertService.showStickyMessage(Utilities.noNetworkMessageCaption, Utilities.noNetworkMessageDetail, MessageSeverity.error);
          }
          else {
            this.alertService.showStickyMessage("Unable to resend code", "An error occured while sending, please try again later.\nError: " + Utilities.getResponseBody(error), MessageSeverity.error);
          }

          setTimeout(() => {
            this.isResending = false;
          }, 500);
        });
  }

  reset() {
    this.formResetToggle = false;

    setTimeout(() => {
      this.formResetToggle = true;
    });
  }
}
