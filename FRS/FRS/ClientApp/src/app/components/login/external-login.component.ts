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

@Component({
  selector: "app-external-login",
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class ExternalLoginComponent implements OnInit, OnDestroy {

  userLogin = new UserLogin();
  isLoading = false;
  formResetToggle = true;
  modalClosedCallback: () => void;
  loginStatusSubscription: any;

  constructor(private alertService: AlertService, public connections: FRSHubConnections, private authService: AuthService, private configurations: ConfigurationService,
    public router: Router, private accountService: AccountService) {
    if (this.router.url.indexOf('/externallogin') > -1) {
      var queryParams = this.router.url.split('?')[1];
      let email = queryParams.split('=')[1];
      this.login(email);
    }
  }


  ngOnInit() {
   
  }


  ngOnDestroy() {
    
  }

  login(email) {
    this.isLoading = true;
    this.alertService.startLoadingMessage("", "Attempting login...");

    this.authService.loginExternal(email)
      .subscribe(
        user => {
          setTimeout(() => {
            this.alertService.stopLoadingMessage();
            this.isLoading = false;
            this.reset();
            this.alertService.showMessage("Login", `Welcome ${user.userName}!`, MessageSeverity.success);

            if (this.connections.userHubConnection == null || this.connections.userHubConnection.state !== signalRCore.HubConnectionState.Connected) {
              //this.connections.userHubConnection = this.authService.signalRConnection(`/hub/user?email=${user.email}&status=ONLINE`, true);
              //let status = user.status ? user.status : 'ONLINE';
              this.connections.userHubConnection = this.authService.signalRConnection(`${this.configurations.baseUrl}/hub/user?email=${user.email}`, true, this.connections.userHubConnection);

            }

          }, 500);
          this.authService.gotoPage('/');
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

  closeModal() {
    if (this.modalClosedCallback) {
      this.modalClosedCallback();
    }
  }

  reset() {
    this.formResetToggle = false;

    setTimeout(() => {
      this.formResetToggle = true;
    });
  }
}
