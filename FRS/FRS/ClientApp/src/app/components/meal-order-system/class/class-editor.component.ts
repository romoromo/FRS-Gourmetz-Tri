import { Component, ViewChild, Inject, OnInit, OnDestroy } from "@angular/core";

import { AlertService, MessageSeverity } from "../../../services/alert.service";
import { AccountService } from "../../../services/account.service";
import { Permission } from "../../../models/permission.model";
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from "@angular/material";
import { Class } from "src/app/models/meal-order/class.model";
import { ClassService } from "src/app/services/meal-order/class.service";
import { Filter } from "src/app/models/sieve-filter.model";
import { Subscription } from "rxjs";

@Component({
  selector: "class-editor",
  templateUrl: "./class-editor.component.html",
  styleUrls: ["./class-editor.component.css"],
})
export class ClassEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private isNewClass = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingClassName: string;
  private classEdit: Class = new Class();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean } = {};
  public formResetToggle = true;
  public classLevels = [];
  private outletId: string;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild("f")
  private form;

  constructor(
    private alertService: AlertService,
    private classService: ClassService,
    private accountService: AccountService,
    public dialogRef: MatDialogRef<ClassEditorComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (typeof data.classModel != typeof undefined && data.classModel.id) {
      this.editClass(data.classModel);
    } else {
      this.newClass();
    }
    this.outletId = data.outletId;
    this.getClassLevels();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  getClassLevels() {
    let filter = new Filter();
    let f = this.outletId ? "(OutletId)==" + this.outletId + "," : "";
    filter.filters = f + "(IsActive)==true";
    this.subscription.add(
      this.classService.getClassLevelsByFilter(filter).subscribe(
        (results) => {
          this.classLevels = results.pagedData;
        },
        (error) => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage(
            "Get Error",
            `An error occured while retrieving class levels.\r\n"`,
            MessageSeverity.error
          );
        }
      )
    );
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.isNewClass) {
      this.classService.newClass(this.classEdit).subscribe(
        (classModel) => this.saveSuccessHelper(classModel),
        (error) => this.saveFailedHelper(error)
      );
    } else {
      this.classService.updateClass(this.classEdit).subscribe(
        (response) => this.saveSuccessHelper(),
        (error) => this.saveFailedHelper(error)
      );
    }
  }

  private invalidate() {
    return false;
  }

  private saveSuccessHelper(classModel?: Class) {
    if (classModel) Object.assign(this.classEdit, classModel);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewClass)
      this.alertService.showMessage(
        "Success",
        `Class \"${this.classEdit.name}\" was created successfully`,
        MessageSeverity.success
      );
    else
      this.alertService.showMessage(
        "Success",
        `Changes to class \"${this.classEdit.name}\" was saved successfully`,
        MessageSeverity.success
      );

    this.classEdit = new Class();
    this.resetForm();

    //if (!this.isNewClass && this.accountService.currentUser.facilities.some(r => r == this.editingClassName))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback) this.changesSavedCallback();

    this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage(
      "Save Error",
      "The below errors occured while saving your changes:",
      MessageSeverity.error
    );
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback) this.changesFailedCallback();
  }

  private cancel() {
    this.classEdit = new Class();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback) this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {
    if (!replace) {
      this.form.reset();
    } else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  newClass() {
    this.isNewClass = true;
    this.showValidationErrors = true;

    this.editingClassName = null;
    this.selectedValues = {};
    this.classEdit = new Class();

    return this.classEdit;
  }

  editClass(classModel: Class) {
    if (classModel) {
      this.isNewClass = false;
      this.showValidationErrors = true;

      this.editingClassName = classModel.name;
      this.selectedValues = {};
      this.classEdit = new Class();
      Object.assign(this.classEdit, classModel);

      return this.classEdit;
    } else {
      return this.newClass();
    }
  }

  get canManageClasses() {
    return this.accountService.userHasPermission(
      Permission.manageMOSOutletMgtClassesPermission
    );
  }
}
