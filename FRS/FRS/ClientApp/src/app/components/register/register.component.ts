import { Component, OnInit, OnDestroy, Input, ViewChild } from "@angular/core";

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AuthService } from "../../services/auth.service";
import { ConfigurationService } from '../../services/configuration.service';
import { Utilities } from '../../services/utilities';
import { User } from "src/app/models/user.model";
import { AccountService } from "src/app/services/account.service";
import { UserEdit } from "src/app/models/user-edit.model";
import { ActivatedRoute, NavigationStart, Router, NavigationEnd } from "@angular/router";
import { Department } from "src/app/models/department.model";
import { FormControl } from "@angular/forms";
import { DepartmentService } from "src/app/services/department.service";

@Component({
  selector: "app-register",
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit, OnDestroy {

  user = new User();
  private userEdit: UserEdit;
  public formResetToggle = true;
  showValidationErrors = true;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  private isNewUser = false;
  private isSaving = false;
  private qrCodeUrl = '';
  private isExternalLogin = false;
  private showDepartmentList = false;
  departmentList: Department[];
  fcDepartments = new FormControl();

  @ViewChild('f')
  private form;

  //ViewChilds Required because ngIf hides template variables from global scope
  @ViewChild('userName')
  private userName;

  @ViewChild('userPassword')
  private userPassword;

  @ViewChild('email')
  private email;

  @ViewChild('currentPassword')
  private currentPassword;

  @ViewChild('newPassword')
  private newPassword;

  @ViewChild('confirmPassword')
  private confirmPassword;

  @ViewChild('roles')
  private roles;

  @ViewChild('rolesSelector')
  private rolesSelector;


  constructor(private alertService: AlertService, private authService: AuthService, private accountService: AccountService, private configurations: ConfigurationService,
    public router: Router, private route: ActivatedRoute, private departmentService: DepartmentService) {

  }


  ngOnInit() {
    this.isExternalLogin = false;
    this.userEdit = new UserEdit();
    this.qrCodeUrl = this.configurations.baseUrl + '';
    this.accountService.loadConfig().subscribe(config => {
      this.qrCodeUrl = config.androidQRUrl;
    },
      error => { });

    if (this.router.url.indexOf('/register') > -1) {
      this.isExternalLogin = false;
      this.showDepartmentList = false;
      var queryParams: string = '';
      var queryParamSection: string[] = [];
      if (this.router.url.indexOf('?') > -1) {
        queryParams = this.router.url.split('?')[1];
        queryParamSection = queryParams.split('&');
      }

      if (queryParamSection && queryParamSection.length > 0) {
        var queryStrings = Utilities.getQueryParamsFromString(queryParams);
        this.userEdit.institutionCode = queryStrings['institutioncode'];
        this.showDepartmentList = true;
        this.departmentService.getDepartments(-1, -1, null, this.userEdit.institutionCode)
          .subscribe(results => {
            this.alertService.stopLoadingMessage();
            this.departmentList = results;
            this.showDepartmentList = this.departmentList && this.departmentList.length > 0;
          },
            error => {
              this.alertService.stopLoadingMessage();
              this.alertService.showStickyMessage("Load Error", `Unable to retrieve departments from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
                MessageSeverity.error);
            });

        if (queryStrings['provider'] != null && queryStrings['key'] != null) {
          this.isExternalLogin = true;
          this.userEdit.email = queryStrings['email'];
          this.userEdit.provider = queryStrings['provider'];
          this.userEdit.key = queryStrings['key'];
          this.userEdit.isExternal = true;
        }
      }
    }

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
    this.accountService.registerUser(this.userEdit).subscribe(user => this.saveSuccessHelper(user), error => this.saveFailedHelper(error));
  }


  private saveSuccessHelper(user?: User) {
    let isExternalLogin = this.userEdit.newPassword == null;
    if (user)
      Object.assign(this.userEdit, user);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    Object.assign(this.user, this.userEdit);
    this.userEdit = new UserEdit();
    this.resetForm();

    let message = isExternalLogin ? 'You have successfully registered.' : 'You have successfully registered. Please confirm the link sent to your email.';
    this.alertService.showMessage("Success", message, MessageSeverity.success);


    if (this.changesSavedCallback)
      this.changesSavedCallback();
  }


  private saveFailedHelper(error: any) {
    console.log(JSON.stringify(error));
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes" , MessageSeverity.error);
    this.alertService.showStickyMessage(JSON.stringify(error.error), null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }

  resetForm(replace = false) {
    this.isExternalLogin = false;

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

}
