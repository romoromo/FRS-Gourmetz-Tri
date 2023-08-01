import { MatDialogRef, MAT_DIALOG_DATA, MatSelect, MatOption } from "@angular/material";
import { Component, Inject, OnInit, AfterViewInit, ViewChild } from "@angular/core";
import { AlertService, MessageSeverity } from "src/app/services/alert.service";
import { PIBService } from "src/app/services/pib.service";
import { PIBTemplate, PIBTemplateLocation } from "src/app/models/department.model";
import { AppTranslationService } from "src/app/services/app-translation.service";
import { Permission } from "src/app/models/permission.model";
import { AccountService } from "src/app/services/account.service";
import { forEach } from "@angular/router/src/utils/collection";
import { locateHostElement } from "@angular/core/src/render3/instructions";
import { Utilities } from "src/app/services/utilities";

@Component({
  selector: 'template-editor-save-dialog',
  templateUrl: './save-dialog.component.html',
  styleUrls: ['./save-dialog.component.css'],
})
export class TemplateSaveDialogPreviewDialog implements OnInit {
  isSaving: boolean;
  isNewTemplate: boolean;
  showValidationErrors: boolean;
  editingTemplateName: any;
  templateEdit: PIBTemplate;
  formResetToggle: boolean = true;
  locations: PIBTemplateLocation[] = [];
  allSelected = false;
  @ViewChild('locSelect') locSelect: MatSelect;

  constructor(
    private translationService: AppTranslationService,
    private alertService: AlertService,
    private pibService: PIBService,
    public dialogRef: MatDialogRef<TemplateSaveDialogPreviewDialog>,
    private accountService: AccountService,
    @Inject(MAT_DIALOG_DATA) public dlgData: any) {
    this.getLocations();
    if (typeof (dlgData.template) != typeof (undefined)) {
      //this.locations = dlgData.locations;
      if (dlgData.template.id) {
        this.editTemplate(dlgData.template);
      } else {
        this.newTemplate(dlgData.template.templateBody);
      }
    }
  }

  ngOnInit() {
  }

  newTemplate(template_body: string) {
    this.isNewTemplate = true;
    this.showValidationErrors = true;

    this.editingTemplateName = null;
    this.templateEdit = new PIBTemplate();
    this.templateEdit.templateBody = template_body;
    return this.templateEdit;
  }

  editTemplate(template: PIBTemplate) {
    if (template) {
      this.isNewTemplate = false;
      this.showValidationErrors = true;

      this.editingTemplateName = template.name;
      this.templateEdit = new PIBTemplate();
      Object.assign(this.templateEdit, template);

      return this.templateEdit;
    }
    else {
      return this.newTemplate(template.templateBody);
    }
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.templateEdit.locations = [];
    if (this.templateEdit.locationIds) {
      this.templateEdit.locationIds.forEach((loc, index, locations) => {
        var locs = this.locations.filter(item => item.location_id === loc);
        if (locs != null && locs.length > 0) {
          var location = locs[0];
          location.pibTemplateId = this.templateEdit.id;
          this.templateEdit.locations.push(location);
        }
      });
    }

    if (this.isNewTemplate) {
      this.pibService.newTemplate(this.templateEdit).subscribe(Template => this.saveSuccessHelper(Template), error => this.saveFailedHelper(error));
    }
    else {
      this.pibService.updateTemplate(this.templateEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  private saveSuccessHelper(Template?: PIBTemplate) {
    if (Template)
      Object.assign(this.templateEdit, Template);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewTemplate)
      this.alertService.showMessage("Success", `Template \"${this.templateEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to Template \"${this.templateEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.templateEdit = new PIBTemplate();


    //if (!this.isNewTemplate && this.accountService.currentUser.facilities.some(r => r == this.editingTemplateName))
    //    this.refreshLoggedInUser();

    //if (this.changesSavedCallback)
    //  this.changesSavedCallback();

    this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    //this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    //if (this.changesFailedCallback)
    //  this.changesFailedCallback();
  }


  private cancel() {
    //this.templateEdit = new PIBTemplate();

    //this.showValidationErrors = false;

    //this.alertService.resetStickyMessage();

    ////if (this.changesCancelledCallback)
    ////  this.changesCancelledCallback();

    this.dialogRef.close({ isCancel: true });
  }

  getLocations() {
    this.alertService.startLoadingMessage("Loading locations...");
    this.pibService.getLocations()
      .subscribe(results => {
        this.locations = results[0];
        this.alertService.stopLoadingMessage();
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\n"`,
            MessageSeverity.error);
        });
  }

  toggleAllSelection() {
    this.allSelected = !this.allSelected;  // to control select-unselect

    if (this.allSelected ) {
      this.locSelect.options.forEach((item: MatOption) => item.select());
    } else {
      this.locSelect.options.forEach((item: MatOption) => { item.deselect() });
    }
    this.locSelect.close();
  }

  get canManageTemplates() {
    return this.accountService.userHasPermission(Permission.managePIBTemplatesPermission)
  }
}
