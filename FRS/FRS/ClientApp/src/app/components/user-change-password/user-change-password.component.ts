import { Component, OnInit, OnDestroy, Input, ViewChild } from "@angular/core";

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AuthService } from "../../services/auth.service";
import { ConfigurationService } from '../../services/configuration.service';
import { Utilities } from '../../services/utilities';
import { User } from "src/app/models/user.model";
import { AccountService } from "src/app/services/account.service";
import { UserEdit } from "src/app/models/user-edit.model";
import { ActivatedRoute, NavigationStart, Router, NavigationEnd } from "@angular/router";
import { ReservationService } from "src/app/services/reservation.service";
import { Reservation } from "src/app/models/reservation.model";

@Component({
  selector: "app-user-change-password",
  templateUrl: './user-change-password.component.html',
  styleUrls: ['./user-change-password.component.css']
})

export class UserChangePasswordComponent implements OnInit, OnDestroy {
  public formResetToggle = true;
  showValidationErrors = true;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  private userEdit: UserEdit;
  private isSaving = false;
  private userId: string;
  private callbackUrl: string;
  @ViewChild('f')
  private form;

  //ViewChilds Required because ngIf hides template variables from global scope
  @ViewChild('userName')
  private userName;

  constructor(private alertService: AlertService, private authService: AuthService, private accountService: AccountService, private configurations: ConfigurationService,
    public router: Router, private route: ActivatedRoute) {
    if (this.router.url.indexOf('/changepassword') > -1) {
      this.authService.reLoginDelegate = () => null;
      var queryParams = this.router.url.split('?')[1];
      var queryParamSection = queryParams.split('&');
      this.userId = queryParamSection[0].split('=')[1];
      this.callbackUrl = queryParamSection[1].split('=')[1];
      //this.userNameEmail = queryParamSection[1].split('=')[1];
      this.accountService.getUser(this.userId).subscribe(user => {
        this.userEdit = new UserEdit();
        Object.assign(this.userEdit, user);
      });
    }
  }


  ngOnInit() {
    
    //this.router.events.subscribe(event => {
    //  if (event instanceof NavigationStart) {
    //    let url = (<NavigationStart>event).url;
    //    if (url.toLowerCase().indexOf('/register') > -1) {
    //      debugger;
    //      var queryParamSection = url.split('?')[1];
    //      this.userEdit.email = queryParamSection.split('=')[1];
    //    }
    //  }

    //  if (event instanceof NavigationEnd) {
    //    let url = (<NavigationEnd>event).url;
    //    if (url.toLowerCase().indexOf('/register') > -1) {
    //      debugger;
    //      var queryParamSection = url.split('?')[1];
    //      this.userEdit.email = queryParamSection.split('=')[1];
    //    }
    //  }
    //});

  }


  ngOnDestroy() {
    
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.authService.login(this.userEdit.email, this.userEdit.currentPassword, this.userEdit.institutionCode, false, this.userEdit.isAD)
      .subscribe(
        user => {
          this.accountService.changePassword(this.userEdit, this.userId)
            .subscribe(data => {
              this.alertService.showMessage("Password successfully changed!", "", MessageSeverity.success);
              let url = this.callbackUrl;
              setTimeout(function () {
                window.open(url, '_self');
              }, 2000);
              this.isSaving = false;
              this.alertService.stopLoadingMessage();
            },
              error => this.saveFailedHelper(error));
        },
        error => {

          this.alertService.stopLoadingMessage();
          this.alertService.showStickyMessage("Unable to change the password. Please try again later.", "", MessageSeverity.error);

          setTimeout(function () {
            this.authService.logout();
          }, 2000);
        });

    
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }
}
