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
  selector: "app-attendance-signin",
  templateUrl: './attendance-signin.component.html',
  styleUrls: ['./attendance-signin.component.css']
})

export class AttendanceSignInComponent implements OnInit, OnDestroy {

  private reservationId = '';
  private userNameEmail = '';
  private name = '';
  private company = '';
  private designation = '';
  public formResetToggle = true;
  showValidationErrors = true;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  private event: Reservation;

  private isSaving = false;

  @ViewChild('f')
  private form;

  //ViewChilds Required because ngIf hides template variables from global scope
  @ViewChild('userName')
  private userName;

  constructor(private alertService: AlertService, private authService: AuthService, private reservationService: ReservationService, private configurations: ConfigurationService,
    public router: Router, private route: ActivatedRoute) {
    
  }


  ngOnInit() {
    if (this.router.url.indexOf('/attendance') > -1) {
      this.authService.reLoginDelegate = () => null;
      var queryParams = this.router.url.split('?')[1];
      var queryParamSection = queryParams.split('&');
      this.reservationId = queryParamSection[0].split('=')[1];
      //this.userNameEmail = queryParamSection[1].split('=')[1];
      this.reservationService.getReservation(this.reservationId).subscribe(reservation => {
        this.event = reservation.data;
      });
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
    this.reservationService.validateAttendance(this.reservationId, this.userNameEmail, this.name, this.company, this.designation)
      .subscribe(data => {
        let message = data.isSuccess && data.data.isUserValid ? data.data.message : 'Failed to add your attendance to the event - ' + data.data.message;
        if (data.isSuccess && data.data.isUserValid) {
          this.form.reset();
          this.alertService.showMessage("Success", message, MessageSeverity.success);
        } else {
          this.alertService.showMessage("Error", message, MessageSeverity.error);
        }


        this.isSaving = false;
        this.alertService.stopLoadingMessage();
      },
      error => this.saveFailedHelper(error));
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }

  //resetForm(replace = false) {

  //  if (!replace) {
  //    this.form.reset();
  //  }
  //  else {
  //    this.formResetToggle = false;

  //    setTimeout(() => {
  //      this.formResetToggle = true;
  //    });
  //  }
  //}

}
