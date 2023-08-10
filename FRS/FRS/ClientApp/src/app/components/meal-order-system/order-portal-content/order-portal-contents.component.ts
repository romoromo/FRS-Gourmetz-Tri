import { Component, OnInit, OnDestroy, ViewChild, Inject } from '@angular/core';

import { Filter } from 'src/app/models/sieve-filter.model';
import { DomSanitizer } from '@angular/platform-browser';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AccountService } from 'src/app/services/account.service';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { OrderPortalService } from 'src/app/services/order-portal.service';
import { MAT_DIALOG_DATA } from '@angular/material';


@Component({
  selector: 'order-portal-contents',
  templateUrl: './order-portal-contents.component.html',
  styleUrls: ['./order-portal-contents.component.css']
})
export class OrderPortalContentComponent implements OnInit {
  orderPortalContent: any;
  outletId: string;

  constructor(private alertService: AlertService, private accountService: AccountService, public configurations: ConfigurationService,
    private orderPortalService: OrderPortalService, private sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.dishCycle) != typeof (undefined)) {
      this.outletId = data.outletId;
    }
  }

  ngOnInit() {
    this.getOrderPortalContentFirst();
  }

  getOrderPortalContentFirst() {
    this.orderPortalService.getOrderPortalContentFirst(this.outletId)
      .subscribe(result => {
        this.orderPortalContent = result;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
            MessageSeverity.error);
        })
  }

  //getOrderPortalContents() {
  //  let filter = new Filter();
  //  filter.sorts = 'order';
  //  filter.filters = '(IsActive)==true';
  //  this.orderPortalService.getOrderPortalContentsByFilter(filter)
  //    .subscribe(results => {
  //      this.orderPortalContents = results.pagedData;
  //    },
  //      error => {
  //        //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
  //        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving settings.\r\n"`,
  //          MessageSeverity.error);
  //      })
  //}

  getSafeHTML(html) {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }

  save() {
    //let settings: OrderPortalContent[] = [];
    //Object.assign(settings, this.orderPortalContents);
    //settings.forEach((setting, index, settings) => {
    //  setting.template = this.getSafeHTML(setting.template);
    //});
    
    this.orderPortalService.updateOrderPortalContent(this.orderPortalContent).subscribe(response => {
      this.alertService.showMessage("Success", `Settings are saved successfully.`, MessageSeverity.success);
    }, error => {
        this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
        this.alertService.showStickyMessage(error, null, MessageSeverity.error);
    });
  }

  //getOrderPortalContent() {
  //  let filter = new Filter();
  //  filter.filters = '(IsActive)==true';
  //  this.orderPortalService.getOrderPortalContentById(filter)
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
