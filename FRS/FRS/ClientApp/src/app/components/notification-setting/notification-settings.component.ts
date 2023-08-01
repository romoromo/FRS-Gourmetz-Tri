import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { ConfigurationService } from '../../services/configuration.service';
import { AppTranslationService } from "../../services/app-translation.service";
import { BootstrapSelectDirective } from "../../directives/bootstrap-select.directive";
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';
import { Permission } from '../../models/permission.model';
import { NotificationService } from 'src/app/services/notification.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { DomSanitizer } from '@angular/platform-browser';
import { NotificationSetting } from 'src/app/models/notification.model';


@Component({
  selector: 'notification-settings',
  templateUrl: './notification-settings.component.html',
  styleUrls: ['./notification-settings.component.css']
})
export class NotificationSettingComponent implements OnInit {
  notificationSettings: any;

  constructor(private alertService: AlertService, private accountService: AccountService, public configurations: ConfigurationService,
    private notificationService: NotificationService, private sanitizer: DomSanitizer  ) {
  }

  ngOnInit() {
    this.getNotificationSettings();
  }

  getNotificationSettings() {
    let filter = new Filter();
    //filter.sorts = 'id';
    filter.filters = '(IsActive)==true';
    this.notificationService.getNotificationSettingsByFilter(filter)
      .subscribe(results => {
        this.notificationSettings = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving settings.\r\n"`,
            MessageSeverity.error);
        })
  }

  getSafeHTML(html) {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }

  save() {
    //let settings: NotificationSetting[] = [];
    //Object.assign(settings, this.notificationSettings);
    //settings.forEach((setting, index, settings) => {
    //  setting.template = this.getSafeHTML(setting.template);
    //});
    
    this.notificationService.bulkUpdateNotificationSetting(this.notificationSettings).subscribe(response => {
      this.alertService.showMessage("Success", `Settings are saved successfully.`, MessageSeverity.success);
    }, error => {
        this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
        this.alertService.showStickyMessage(error, null, MessageSeverity.error);
    });
  }

  //getNotificationSetting() {
  //  let filter = new Filter();
  //  filter.filters = '(IsActive)==true';
  //  this.notificationService.getNotificationSettingById(filter)
  //    .subscribe(results => {
  //      this.notificationEvents = results.pagedData;
  //    },
  //      error => {
  //        //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
  //        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving notification events.\r\n"`,
  //          MessageSeverity.error);
  //      })
  //}
}
