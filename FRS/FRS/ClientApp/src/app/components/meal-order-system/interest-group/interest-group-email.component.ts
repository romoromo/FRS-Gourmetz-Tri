import { Component, ViewChild, Inject } from '@angular/core';
import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { InterestGroup } from 'src/app/models/meal-order/interest-group.model';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { Utilities } from 'src/app/services/utilities';
import { Filter } from 'src/app/models/sieve-filter.model';
import { EmailService } from 'src/app/services/meal-order/email.service';


@Component({
  selector: 'interest-group-email',
  templateUrl: './interest-group-email.component.html',
  styleUrls: ['./interest-group-email.component.css']
})
export class InterestGroupEmailComponent {

  public id: string;
  public templateId: string;
  emailTemplates: any[] = [];

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private studentService: StudentService, private accountService: AccountService,
    public dialogRef: MatDialogRef<InterestGroupEmailComponent>, private emailService: EmailService, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.id) != typeof (undefined)) {
      this.id = data.id;
    }

    if (typeof (data.templateId) != typeof (undefined)) {
      this.templateId = data.templateId;
    }

    if (typeof (data.outletId) != typeof (undefined)) {
      this.getEmailTemplates(data.outletId);
    }
  }

  getEmailTemplates(outletId) {
    let filter = new Filter();
    filter.filters = '(IsActive)==true,(OutletId)==' + (outletId ? outletId: '');

    this.emailService.getEmailTemplatesByFilter(filter)
      .subscribe(results => {
        this.emailTemplates = results.pagedData;
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.alertService.startLoadingMessage("Sending email...");
    this.studentService.sendEmailToStudents(this.templateId, this.id)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.alertService.showMessage("Success", `Emails sent!`, MessageSeverity.success);
        this.dialogRef.close();
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.alertService.showStickyMessage("Sending Error", `An error occured while sending emails.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  private cancel() {
    this.alertService.resetStickyMessage();
    this.dialogRef.close();
  }

  get canManageInterestGroups() {
    return true; //this.accountService.userHasPermission(Permission.manageInterestGroupsPermission)
  }
}
