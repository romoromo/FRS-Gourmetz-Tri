import { Component, OnInit, OnDestroy, Input, ViewChild } from "@angular/core";

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AuthService } from "../../services/auth.service";
import { ConfigurationService } from '../../services/configuration.service';
import { Utilities } from '../../services/utilities';
import { UserLogin } from '../../models/user-login.model';
import { Router } from "@angular/router";
@Component({
  selector: "app-forgot-password",
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})

export class ForgotPasswordComponent implements OnInit, OnDestroy {

  userLogin = new UserLogin();
  isLoading = false;
  formResetToggle = true;
  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private authService: AuthService, private configurations: ConfigurationService,
    public router: Router  ) {

  }


  ngOnInit() {

    
  }


  ngOnDestroy() {

  }




  showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


setInstitutionCode() {
    this.userLogin.institutionCode = '';
    var queryParams: string = '';
    var queryParamSection: string[] = [];
    if (this.router.url.indexOf('?') > -1) {
      queryParams = this.router.url.split('?')[1];
      queryParamSection = queryParams.split('&');
    }
    if (queryParamSection && queryParamSection.length > 0) {
      var queryStrings = Utilities.getQueryParamsFromString(queryParams);
      this.userLogin.institutionCode = queryStrings['institutioncode'];
    }

  }

  forgotPassword() {
    this.setInstitutionCode();
    this.isLoading = true;
    this.alertService.startLoadingMessage("", "Submitting...");

    this.authService.forgotPassword(this.userLogin.email, this.userLogin.institutionCode)
      .subscribe(
        response => {
          setTimeout(() => {
            this.alertService.stopLoadingMessage();
            this.isLoading = false;
            if (response.isSuccess) {
              this.reset();
              this.alertService.showStickyMessage("Success", response.message, MessageSeverity.success);
            } else {
              this.alertService.showStickyMessage("Unable to reset password", response.message, MessageSeverity.error);
            }
            
          }, 500);
        },
        error => {

          this.alertService.stopLoadingMessage();
          this.alertService.showStickyMessage("Unable to reset password", "An error occured while resetting password, please try again later.\nError: " + Utilities.getResponseBody(error), MessageSeverity.error);


          setTimeout(() => {
            this.isLoading = false;
          }, 500);
        });
  }


  reset() {
    this.form.reset();
    this.formResetToggle = false;

    setTimeout(() => {
      this.formResetToggle = true;
    });
  }
}
