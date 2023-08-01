import { Component, OnInit, ViewChild } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { ConfigurationService } from '../../services/configuration.service';
import { Permission } from 'src/app/models/permission.model';
import { AccountService } from 'src/app/services/account.service';
import { AuthService } from 'src/app/services/auth.service';
import { ActivatedRoute, NavigationStart, Router, NavigationEnd } from "@angular/router";
import { Utilities } from 'src/app/services/utilities';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { ReservationService } from 'src/app/services/reservation.service';

@Component({
    selector: 'feedback',
    templateUrl: './feedback.component.html',
    styleUrls: ['./feedback.component.css'],
    animations: [fadeInOut]
})
export class FeedbackComponent implements OnInit{
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  private feedback = { reservationId: '', userId: '', feedback: '', comment: '' };
  choices: any[] = [
    { name: 'VERY GOOD', icon: 'sentiment_very_satisfied' },
    { name: 'GOOD', icon: 'sentiment_satisfied' },
    { name: 'POOR', icon: 'sentiment_dissatisfied' },
    { name: 'VERY POOR', icon: 'sentiment_very_dissatisfied' }];

  private isSaving = false;

  @ViewChild('f')
  private form;
  public formResetToggle = true;
  showValidationErrors = true;

  constructor(private alertService: AlertService, public configurations: ConfigurationService, private accountService: AccountService, private authService: AuthService,
    public router: Router, private route: ActivatedRoute, private reservationService: ReservationService  ) {
    
  }

  ngOnInit() {
    if (this.router.url.indexOf('/feedback') > -1) {
      var queryParams: string = '';
      var queryParamSec = this.router.url.split('?');
      if (queryParamSec && queryParamSec.length > 0) {
        queryParams = queryParamSec[1];
        var queryStrings = Utilities.getQueryParamsFromString(queryParams);
        if (queryStrings) {
          this.feedback.reservationId = queryStrings['reservationId'];
          this.feedback.userId = queryStrings['userId'];
        }
      }
      
    }
  }

  private save() {
    if (!this.feedback.feedback) {
      this.alertService.showMessage("Error", 'Please select one choice.', MessageSeverity.error);
      return;
    }
    this.isSaving = true;
    this.alertService.startLoadingMessage("Submitting feedback...");
    this.reservationService.validateFeedback(this.feedback)
      .subscribe(data => {
        this.isSaving = false;
        this.alertService.stopLoadingMessage();
        let message = data.isSuccess ? data.message : 'Failed to submit your feedback for this event - ' + data.message;
        if (data.isSuccess) {
          this.form.reset();
          this.alertService.showMessage("Success", message, MessageSeverity.success);
        } else {
          this.alertService.showMessage("Error", message, MessageSeverity.error);
        }
      },
        error => this.saveFailedHelper(error));
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Submit Error", "The below errors occured while submitting your feedback:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }
}
