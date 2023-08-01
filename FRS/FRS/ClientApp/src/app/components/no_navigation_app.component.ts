import { Component, ViewEncapsulation, OnInit, OnDestroy, ViewChildren, AfterViewInit, QueryList, ElementRef } from "@angular/core";
import { Router, NavigationStart, NavigationEnd, ActivatedRoute } from '@angular/router';
import { ToastaService, ToastaConfig, ToastOptions, ToastData } from 'ngx-toasta';
import { ModalDirective } from 'ngx-bootstrap/modal';
import 'rxjs/add/operator/skip';
import 'rxjs/add/operator/debounceTime';

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

@Component({
  selector: "no-nav-app-root",
  templateUrl: './no_navigation_app.component.html',
  styleUrls: ['./no-navigation_app.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class NoNavigationAppComponent {
  stickyToasties: number[] = [];
  messageSubscription: any;
  stickMessageSubscription: any;

  constructor(public configurations: ConfigurationService,
    private alertService: AlertService, private translationService: AppTranslationService, private toastaService: ToastaService,
    private toastaConfig: ToastaConfig) { }

  ngOnInit() {
    this.ngOnDestroy();

    this.translationService.addLanguages(["en", "fr", "de", "pt", "ar", "ko"]);
    this.translationService.setDefaultLanguage('en');


    this.toastaConfig.theme = 'bootstrap';
    this.toastaConfig.position = 'top-right';
    this.toastaConfig.limit = 100;
    this.toastaConfig.showClose = true;

    this.messageSubscription = this.alertService.getMessageEvent().subscribe(message => this.showToast(message, false));
    this.stickMessageSubscription = this.alertService.getStickyMessageEvent().subscribe(message => this.showToast(message, true));
  }

  ngOnDestroy() {
    if (this.messageSubscription)
      this.messageSubscription.unsubscribe();
    if (this.stickMessageSubscription)
      this.stickMessageSubscription.unsubscribe();
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
}
