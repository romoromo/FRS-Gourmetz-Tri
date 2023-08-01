import { Component, OnInit, OnDestroy, Input } from "@angular/core";

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AuthService } from "../../services/auth.service";
import { ConfigurationService } from '../../services/configuration.service';
import { Utilities } from '../../services/utilities';
import { ResetPassword } from '../../models/user-login.model';
import { ActivatedRoute } from "@angular/router";
import { LocalStoreManager } from "src/app/services/local-store-manager.service";
import { window } from "rxjs/operators";

@Component({
  selector: "app-reset-password",
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})

export class ResetPasswordComponent implements OnInit, OnDestroy {

  userLogin = new ResetPassword();
  isLoading = false;
  formResetToggle = true;
  resetUrl = '';
  @Input()
  userId: string;

  @Input()
  code: string;

  constructor(private alertService: AlertService, private authService: AuthService, private configurations: ConfigurationService,
    private route: ActivatedRoute, private storeManager: LocalStoreManager) {
    this.resetUrl = storeManager.getData('resetUrl');
    var id = this.route.snapshot.queryParamMap.get("userId");
    this.route.queryParamMap.subscribe(queryParams => {
      id = queryParams.get("userId");
    });
  }


  ngOnInit() {
    this.authService.reLoginDelegate = () => null;
    this.route.paramMap.subscribe(params => {
      this.userLogin.code = params.get("code");
      this.userLogin.userId = params.get("userId");
    })
    
  }


  ngOnDestroy() {

  }




  showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  resetPassword() {
    if (!this.resetUrl) {
      this.resetUrl = location.href;
    }

    this.isLoading = true;
    this.alertService.startLoadingMessage("", "Submitting...");
    var queryParamSection = this.resetUrl.split('?')[1];
    var params = queryParamSection.split('&');
    this.userLogin.userId = params[0].split('=')[1];
    this.userLogin.code = params[1].split('=')[1];
    this.authService.resetPassword(this.userLogin)
      .subscribe(
        response => {
          setTimeout(() => {
            this.alertService.stopLoadingMessage();
            this.isLoading = false;
            if (response.isSuccess) {
              this.alertService.showMessage("Success", response.message, MessageSeverity.success);
              setTimeout(() => {
                location.replace('/login');
                //this.userLogin = new ResetPassword(); 
              }, 2000);
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
    this.formResetToggle = false;

    setTimeout(() => {
      this.formResetToggle = true;
    });
  }
}
