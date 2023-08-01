import { Component, OnInit, OnDestroy, TemplateRef, ViewChild, Input, AfterViewInit, Output, EventEmitter } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AppTranslationService } from "../../../services/app-translation.service";
import { DashboardService } from "../../../services/dashboard.service";
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';


@Component({
  selector: 'report-container',
  templateUrl: './report-container.component.html',
  styleUrls: ['./report-container.component.css']
})
export class ReportContainerComponent implements OnInit, AfterViewInit {
  private RESERVATION = 'Reservation Report';
  private VEHICLE_LOG = 'Vehicle';
  private reportType: string = this.RESERVATION;
  private types = [this.RESERVATION, this.VEHICLE_LOG];

  constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService) {
  }

  ngOnInit() {

  }

  ngAfterViewInit() {

  }

  get canViewReports() {
    return this.accountService.userHasPermission(Permission.viewReportsPermission)
  }
}
