import { Component, ViewEncapsulation, OnInit, OnDestroy, ViewChildren, AfterViewInit, QueryList, ElementRef } from "@angular/core";
import { Router, NavigationStart, NavigationEnd, ActivatedRoute } from '@angular/router';
import { ToastaService, ToastaConfig, ToastOptions, ToastData } from 'ngx-toasta';
import { ModalDirective } from 'ngx-bootstrap/modal';
import 'rxjs/add/operator/skip';
import 'rxjs/add/operator/debounceTime';
import * as signalR from '@aspnet/signalr';

import { AlertService, AlertDialog, DialogType, AlertMessage, MessageSeverity } from '../services/alert.service';
import { NotificationService } from "../services/notification.service";
import { AppTranslationService } from "../services/app-translation.service";
import { AccountService } from '../services/account.service';
import { LocalStoreManager } from '../services/local-store-manager.service';
import { AppTitleService } from '../services/app-title.service';
import { AuthService } from '../services/auth.service';
import { ConfigurationService } from '../services/configuration.service';
import { Permission } from '../models/permission.model';
import { LoginComponent } from "../components/login/login.component";
import { ExternalLoginComponent } from "./login/external-login.component";
import { FRSHubConnections } from "../models/user.model";
import { Filter } from "../models/sieve-filter.model";
import { Notification } from '../models/notification.model';
import { getBaseUrl } from "../app.module";
var alertify: any = require('../assets/scripts/alertify.js');
//var oldSignalR: any = require('../assets/scripts/jquery.signalR-2.3.0.min.js');

@Component({
  selector: "app-root",
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit, AfterViewInit {

  messageSubscription: any;
  stickMessageSubscription: any;
  dialogSubscription: any;

  isRegister = false;
  isForgotPassword = false;
  isResetPassword = false;
  isAttendance = false;
  isPIBDeviceTemplate = false;
  isAppLoaded: boolean;
  isUserLoggedIn: boolean;
  shouldShowLoginModal: boolean;
  removePrebootScreen: boolean;
  newNotificationCount = 0;
  appTitle = "Communal OS";
  resetUserId = '';
  resetCode = '';
  appLogo = require("../assets/images/booking-icon.png");
  initLink = '/';
  stickyToasties: number[] = [];

  dataLoadingConsecutiveFailurs = 0;
  notificationsLoadingSubscription: any;

  @ViewChildren('loginModal,loginControl')
  modalLoginControls: QueryList<any>;

  loginModal: ModalDirective;
  loginControl: LoginComponent;

  loginStatusSubscription: any;
  interval: any;
  menu: any;

    private connection: signalR.HubConnection;

  get notificationsTitle() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    if (this.newNotificationCount)
      return `${gT("app.Notifications")} (${this.newNotificationCount} ${gT("app.New")})`;
    else
      return gT("app.Notifications");
  }


  constructor(private storageManager: LocalStoreManager, private toastaService: ToastaService, private toastaConfig: ToastaConfig,
    private accountService: AccountService, private alertService: AlertService, private notificationService: NotificationService, private appTitleService: AppTitleService,
    private authService: AuthService, private translationService: AppTranslationService, public configurations: ConfigurationService, public router: Router,
    public activatedRoute: ActivatedRoute, private connections: FRSHubConnections) {
    this.configurations.loadMenus()
      .then(menu => {
        this.menu = menu;
        console.log(this.menu);
      });

    //if (this.accountService.currentUser == null || this.authService.isSessionExpired) {
    //  console.log(this.authService.refreshToken);
    //  console.log(this.authService.idToken);
    //  this.accountService.refreshLoggedInUser().subscribe(user => {
    //    console.log(user);
    //    if (user == null) {
    //      this.authService.logout();
    //      this.authService.redirectForLogin();
    //    }
        
    //  },
    //    error => {
    //      console.log(error);
    //    });
    //}

    
    //if (this.accountService.currentUser == null || this.authService.isSessionExpired) {
    //  this.authService.logout();
    //  this.authService.redirectForLogin();
    //}

    storageManager.initialiseStorageSyncListener();

    translationService.addLanguages(["en", "fr", "de", "pt", "ar", "ko"]);
    translationService.setDefaultLanguage('en');


    this.toastaConfig.theme = 'bootstrap';
    this.toastaConfig.position = 'top-right';
    this.toastaConfig.limit = 100;
    this.toastaConfig.showClose = true;

    this.appTitleService.appName = this.appTitle;

    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        console.log('NavigationStart');
        //reset all loading messages
        this.alertService.resetStickyMessage();
        this.alertService.resetToastMessage();
        this.alertService.stopLoadingMessage();
        var ev = (<NavigationStart>event);
        let url = (<NavigationStart>event).url;
        if (url.toLowerCase().indexOf('/resetpassword?') > -1 && !this.isResetPassword) {
          this.isResetPassword = true;
          this.storageManager.deleteData('resetUrl');
          this.storageManager.saveSessionData(url, 'resetUrl');
          //var queryParamSection = url.split('?')[1];
          //var params = queryParamSection.split('&');
          //this.resetUserId = params[0].split('=')[1];
          //this.resetCode = params[1].split('=')[1];
          //this.authService.resetUserId = this.resetUserId;
          //this.authService.resetCode = this.resetCode;
          location.replace(this.authService.resetPasswordUrl);
          //this.router.navigate([this.authService.resetPasswordUrl], { queryParams: { userId: this.resetUserId, code: this.resetCode } });
          //this.authService.gotoPage(this.authService.resetPasswordUrl)
        }
      }
    });

    //this.activatedRoute.queryParams.debounceTime(500).skip(1).subscribe(params => {
    //  console.log(params);
    //});

    //this.activatedRoute.queryParams
    //  .filter(queryParams => Object.keys(queryParams).length > 0 === window.location.href.includes('?'))
    //  .subscribe(queryParams =>
    //    console.log(queryParams)
    //  );
  }


  ngAfterViewInit() {

    //this.router.events.subscribe(event => {
    //  if (event instanceof NavigationStart || event instanceof NavigationEnd) {
    //    let url = (<NavigationStart>event).url;

    //    if (url == '/register') {
    //      this.authService.logout();
    //      this.isRegister = true;
    //      this.authService.reLoginDelegate = () => null;
    //    } else if (url == '/forgotpassword') {
    //      this.authService.logout();
    //      this.isForgotPassword = true;
    //      this.authService.reLoginDelegate = () => null;
    //    }


    //  }
    //});
    if (!this.isResetPassword) {
      this.modalLoginControls.changes.subscribe((controls: QueryList<any>) => {
        controls.forEach(control => {
          if (control) {
            if (control instanceof LoginComponent) {
              this.loginControl = control;
              this.loginControl.modalClosedCallback = () => this.loginModal.hide();
            }
            else {
              this.loginModal = control;
              this.loginModal.show();
            }
          }
        });
      });
    }
  }


  onLoginModalShown() {
    this.logout();
    //this.alertService.showStickyMessage("Session Expired", "Your Session has expired. Please log in again", MessageSeverity.info);
  }


  onLoginModalHidden() {
    this.alertService.resetStickyMessage();
    this.loginControl.reset();
    this.shouldShowLoginModal = false;

    if (this.authService.isSessionExpired)
      this.logout();
      //this.alertService.showStickyMessage("Session Expired", "Your Session has expired. Please log in again to renew your session", MessageSeverity.warn);
  }


  onLoginModalHide() {
    this.alertService.resetStickyMessage();
  }


  ngOnInit() {
    
    if (this.loginStatusSubscription)
      this.loginStatusSubscription.unsubscribe();
    if (this.messageSubscription)
      this.messageSubscription.unsubscribe();
    if (this.stickMessageSubscription)
      this.stickMessageSubscription.unsubscribe();

    this.shouldShowLoginModal = false;
    this.initLink = this.router.url;
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart || event instanceof NavigationEnd) {
        let url = (<NavigationStart>event).url.toLowerCase();
        if (url == '/register') {
          this.authService.logout();
          this.isRegister = true;
          this.authService.reLoginDelegate = () => null;
        } else if (url == '/forgotpassword') {
          this.authService.logout();
          this.isForgotPassword = true;
          this.authService.reLoginDelegate = () => null;
        } else if (url == '/resetpassword') {
          this.authService.logout();
          this.isResetPassword = true;
          this.authService.reLoginDelegate = () => null;
        } else if (url == '/attendance') {
          this.authService.logout();
          this.isAttendance = true;
          this.authService.reLoginDelegate = () => null;
        } else if (url == '/pibdevicetemplate') {
          this.authService.logout();
          this.isPIBDeviceTemplate = true;
          this.authService.reLoginDelegate = () => this.shouldShowLoginModal = false;
        }


      }
    });

    if (!this.isRegister && !this.isAttendance && !this.isForgotPassword && !this.isResetPassword && !this.isPIBDeviceTemplate) {
      this.isUserLoggedIn = this.authService.isLoggedIn;

      // 1 sec to ensure all the effort to get the css animation working is appreciated :|, Preboot screen is removed .5 sec later
      setTimeout(() => this.isAppLoaded = true, 1000);
      setTimeout(() => this.removePrebootScreen = true, 1500);

      setTimeout(() => {
        if (this.isUserLoggedIn) {
          this.alertService.resetStickyMessage();

          if (!this.authService.isSessionExpired) {
            //this.alertService.showMessage("Login", `Welcome back ${this.userName}!`, MessageSeverity.default);
          }
          else {
            this.logout();
            //this.shouldShowLoginModal = true;
            //this.alertService.showStickyMessage("Session Expired", "Your Session has expired. Please log in again", MessageSeverity.warn);
          }
        }
      }, 2000);


      this.dialogSubscription = this.alertService.getDialogEvent().subscribe(alert => this.showDialog(alert));
      this.messageSubscription = this.alertService.getMessageEvent().subscribe(message => this.showToast(message, false));
      this.stickMessageSubscription = this.alertService.getStickyMessageEvent().subscribe(message => this.showToast(message, true));

      this.authService.reLoginDelegate = () => this.shouldShowLoginModal = true;

      this.loginStatusSubscription = this.authService.getLoginStatusEvent().subscribe(isLoggedIn => {
        this.isUserLoggedIn = isLoggedIn;


          if (this.isUserLoggedIn) {
              this.initUserHub();
          this.initNotificationsLoading();
        }
        else {
          this.unsubscribeNotifications();
        }

        setTimeout(() => {
          if (!this.isUserLoggedIn && !this.isRegister && !this.isAttendance && !this.isPIBDeviceTemplate) {
            this.alertService.showMessage("Session Ended!", "", MessageSeverity.default);
            this.logout();
          }
        }, 500);
      });

      this.router.events.subscribe(event => {
        if (event instanceof NavigationStart) {
          let url = (<NavigationStart>event).url;

          if (url !== url.toLowerCase()) {
            this.router.navigateByUrl((<NavigationStart>event).url.toLowerCase());
          }
        }
      });

      this.connections.sessionInterval = setInterval(() => {
        //console.log('I am running every 30 seconds - ' + new Date().toISOString());
        //refresh session when access token expired
        if (this.accountService.currentUser == null || this.authService.isSessionExpired) {
          this.accountService.refreshLoggedInUser().subscribe(user => { console.log(user); },
            error => {
              this.alertService.resetStickyMessage();
              this.alertService.showStickyMessage("Refresh failed", "An error occured while refreshing logged in user information from the server. Please log in again.", MessageSeverity.error);
            });
        }
        //if (this.isUserLoggedIn) {
        //  if (this.accountService.currentUser == null || this.authService.isSessionExpired) {
        //    this.authService.logout();
        //    this.authService.redirectForLogin();
        //    clearInterval(this.connections.sessionInterval);
        //  }
        //}
      }, 5 * 60 * 1000);
    }
  }


  ngOnDestroy() {
    this.unsubscribeNotifications();
    if (this.loginStatusSubscription)
      this.loginStatusSubscription.unsubscribe();
    if (this.messageSubscription)
      this.messageSubscription.unsubscribe();
    if (this.stickMessageSubscription)
      this.stickMessageSubscription.unsubscribe();
    if (this.dialogSubscription)
      this.dialogSubscription.unsubscribe();

  }


  private unsubscribeNotifications() {
    if (this.notificationsLoadingSubscription)
      this.notificationsLoadingSubscription.unsubscribe();
  }

    initUserHub() {
      this.connection = this.authService.signalRConnection(this.configurations.baseUrl + "/hub/user?email=" + this.accountService.currentUser.email, true, this.connection);
        if (this.connection != null) {
            this.connection.on("NotificationTriggered", (notification: Notification) => {
                var notifications = this.notificationService.recentNotifications;
                if (!notifications) notifications = [];
                notifications.push(notification);
                this.notificationService.recentNotifications = notifications;
                this.newNotificationCount++;
            });
        }
    }

  initNotificationsLoading() {
      var filter = new Filter(1, 10);
      filter.sorts = 'date';
      filter.page = 1;
      filter.filters = `(UserId)==${this.accountService.currentUser.id},(IsActive)==true,(IsRead)==false`;
      this.notificationsLoadingSubscription = this.notificationService.getNewNotifications(filter)
      .subscribe(notifications => {
        this.dataLoadingConsecutiveFailurs = 0;
        this.newNotificationCount = notifications.filter(n => !n.isRead).length;
      },
        error => {
          this.alertService.logError(error);

          if (this.dataLoadingConsecutiveFailurs++ < 20)
            setTimeout(() => this.initNotificationsLoading(), 5000);
          //else
          //  this.alertService.showStickyMessage("Load Error", "Loading new notifications from the server failed!", MessageSeverity.error);
        });
  }


  markNotificationsAsRead() {

    let recentNotifications = this.notificationService.recentNotifications;

    if (recentNotifications.length) {
      this.notificationService.readUnreadNotification(recentNotifications.map(n => n.id), true)
        .subscribe(response => {
          for (let n of recentNotifications) {
            n.isRead = true;
          }

          this.newNotificationCount = recentNotifications.filter(n => !n.isRead).length;
        },
          error => {
            this.alertService.logError(error);
            this.alertService.showMessage("Notification Error", "Marking read notifications failed", MessageSeverity.error);

          });
    }
  }



  showDialog(dialog: AlertDialog) {

    alertify.set({
      labels: {
        ok: dialog.okLabel || "OK",
        cancel: dialog.cancelLabel || "Cancel"
      }
    });

    switch (dialog.type) {
      case DialogType.alert:
        alertify.alert(dialog.message);

        break
      case DialogType.confirm:
        alertify
          .confirm(dialog.message, (e) => {
            if (e) {
              dialog.okCallback();
            }
            else {
              if (dialog.cancelCallback)
                dialog.cancelCallback();
            }
          });

        break;
      case DialogType.prompt:
        alertify
          .prompt(dialog.message, (e, val) => {
            if (e) {
              dialog.okCallback(val);
            }
            else {
              if (dialog.cancelCallback)
                dialog.cancelCallback();
            }
          }, dialog.defaultValue);

        break;
    }
  }





  showToast(message: AlertMessage, isSticky: boolean) {

    if (message == null) {
      for (let id of this.stickyToasties.slice(0)) {
        this.toastaService.clear(id);
      }

      return;
    }

    let toastOptions: ToastOptions = {
      title: message.summary,
      msg: message.detail,
      timeout: isSticky ? 0 : 4000
    };


    if (isSticky) {
      toastOptions.onAdd = (toast: ToastData) => this.stickyToasties.push(toast.id);

      toastOptions.onRemove = (toast: ToastData) => {
        let index = this.stickyToasties.indexOf(toast.id, 0);

        if (index > -1) {
          this.stickyToasties.splice(index, 1);
        }

        toast.onAdd = null;
        toast.onRemove = null;
      };
    }


    switch (message.severity) {
      case MessageSeverity.default: this.toastaService.default(toastOptions); break;
      case MessageSeverity.info: this.toastaService.info(toastOptions); break;
      case MessageSeverity.success: this.toastaService.success(toastOptions); break;
      case MessageSeverity.error: this.toastaService.error(toastOptions); break;
      case MessageSeverity.warn: this.toastaService.warning(toastOptions); break;
      case MessageSeverity.wait: this.toastaService.wait(toastOptions); break;
    }
  }

  logout() {
    this.authService.logout();
    this.authService.redirectLogoutUser();
  }

  selectMenu(event) {

  }

  getYear() {
    return new Date().getUTCFullYear();
  }

  gotoProfile() {
    window.open(getBaseUrl() + '/profile', '_blank');
  }

  get userName(): string {
    return this.authService.currentUser ? this.authService.currentUser.userName : "";
  }


  get fullName(): string {
    return this.authService.currentUser ? this.authService.currentUser.fullName : "";
  }


  get canViewReservations() {
    return this.accountService.userHasPermission(Permission.addReservationsPermission);
  }

  get canViewFacilities() {
    return this.accountService.userHasPermission(Permission.viewFacilitiesPermission);
  }

  get canViewDevices() {
    return this.accountService.userHasPermission(Permission.viewDevicesPermission);
  }

  get canViewFacilityTypes() {
    return this.accountService.userHasPermission(Permission.viewFacilityTypesPermission);
  }

  get canViewLocations() {
    return this.accountService.userHasPermission(Permission.viewLocationsPermission);
  }

  get canViewPhonebooks() {
    return this.accountService.userHasPermission(Permission.viewUserPhonebooksPermission);
  }

  get canManageContactGroups() {
    return this.accountService.userHasPermission(Permission.manageContactGroupsPermission);
  }

  get canViewReports() {
    return this.accountService.userHasPermission(Permission.viewReportsPermission);
  }

  get canManagePIBTemplates() {
    return this.accountService.userHasPermission(Permission.viewPIBTemplatesPermission);
  }

  get canManagePIBDevices() {
    return this.accountService.userHasPermission(Permission.viewPIBDevicesTemplatesPermission);
  }

  get canViewVehicles() {
    return this.accountService.userHasPermission(Permission.manageUserVehiclesPermission);
  }
}
