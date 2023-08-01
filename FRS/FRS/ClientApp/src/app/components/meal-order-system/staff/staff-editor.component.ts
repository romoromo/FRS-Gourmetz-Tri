import { Component, ViewChild, Inject, OnInit, OnDestroy } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Staff } from 'src/app/models/meal-order/staff.model';
import { StaffService } from 'src/app/services/meal-order/staff.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { ClassLevel } from 'src/app/models/meal-order/class-level.model';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/services/meal-order/user.service';
import { UserCardIdEditorComponent } from '../../controls/usercardid/usercardid-editor.component';
import { UserCardId } from 'src/app/models/usercardid.model';
import { DepartmentService } from 'src/app/services/department.service';


@Component({
  selector: 'staff-editor',
  templateUrl: './staff-editor.component.html',
  styleUrls: ['./staff-editor.component.css']
})
export class StaffEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isNewStaff = false;
  private isChangePassword = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingStaffName: string;
  private staffEdit: Staff = new Staff();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private validation = { name: false, gender: false, classLevel: false, class: false, fas: false };
  private isAddAccount = false;

  public classLevels = [];
  public classes = [];
  public staffTypes = [];
  public departments = [];
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private staffService: StaffService, private accountService: AccountService, private classService: ClassService,
    private userService: UserService, private departmentService: DepartmentService,
    public dialogRef: MatDialogRef<StaffEditorComponent>, public dialog: MatDialog, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.staff) != typeof (undefined) && data.staff.id) {
      this.editStaff(data.staff);
    } else {
      this.newStaff();
    }

    this.getStaffTypes();
    this.getDepartments();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  getStaffTypes() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.userService.getStaffTypesByFilter(filter)
      .subscribe(results => {
        this.staffTypes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving staff types.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getDepartments() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.departmentService.getDepartmentsByFilter(filter)
      .subscribe(results => {
        this.departments = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving departments.\r\n"`,
            MessageSeverity.error);
        }));
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  private validate() {
    this.validation = { name: false, gender: false, classLevel: false, class: false, fas: false };;
    if (this.staffEdit.name && this.staffEdit.name.trim().length == 0) {
      this.validation.name = true;
      //this.name.control.setErrors({});
    }

    if (!this.staffEdit.gender) {
      this.validation.gender = true;
      //this.value.control.setErrors({});
    }

    if (this.validation.name || this.validation.gender) {
      this.showValidationErrors = true;
      return false;
    }

    return true;
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    if (this.isNewStaff) {
      this.staffService.newStaff(this.staffEdit).subscribe(staff => this.saveSuccessHelper(staff), error => this.saveFailedHelper(error));
    }
    else {
      this.staffService.updateStaff(this.staffEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  invalidate() {
    return false;
  }

  private saveSuccessHelper(staff?: Staff) {
    if (staff)
      Object.assign(this.staffEdit, staff);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewStaff)
      this.alertService.showMessage("Success", `Staff \"${this.staffEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to staff \"${this.staffEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.staffEdit = new Staff();
    this.resetForm();


    //if (!this.isNewStaff && this.accountService.currentUser.facilities.some(r => r == this.editingStaffName))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }


  private cancel() {
    this.staffEdit = new Staff();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  resetForm(replace = false) {
    this.isChangePassword = false;
    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }


  newStaff() {
    this.isNewStaff = true;
    this.showValidationErrors = true;
    this.isChangePassword = false;
    this.editingStaffName = null;
    this.selectedValues = {};
    this.staffEdit = new Staff();
    this.staffEdit.gender = 'Male';
    if (!this.staffEdit.cards) this.staffEdit.cards = [];
    return this.staffEdit;
  }

  editStaff(staff: Staff) {
    if (staff) {
      this.isNewStaff = false;
      this.showValidationErrors = true;

      this.editingStaffName = staff.name;
      this.selectedValues = {};
      this.staffEdit = new Staff();
      Object.assign(this.staffEdit, staff);
      this.isAddAccount = this.staffEdit.userId && parseInt(this.staffEdit.userId) > 0;
      if (!this.staffEdit.cards) this.staffEdit.cards = [];
      return this.staffEdit;
    }
    else {
      return this.newStaff();
    }
  }

  private changePassword() {
    this.isChangePassword = true;
  }

  public deletePasswordFromUser(staff: Staff) {
    let userEdit = <Staff>staff;

    delete userEdit.currentPassword;
    delete userEdit.newPassword;
    delete userEdit.confirmPassword;
  }

  //cards
  addCardId() {
    var dialog = this.dialog.open(UserCardIdEditorComponent, {
      data: { header: "New Card", userCardId: new UserCardId(), isUserInfo: true },
      width: '400px',
      panelClass: 'user-info-modal',
      backdropClass: 'user-info-backdrop',
      disableClose: true
    });

    dialog.afterClosed().subscribe(result => {
      console.log("resulting data: ", result)
      if (result && result.cardId) {
        if (typeof (this.staffEdit) == typeof (undefined)) {
          this.staffEdit = new Staff();
        }

        if (typeof (this.staffEdit.cards) == typeof (undefined)) {
          this.staffEdit.cards = [];
        }

        result.userId = this.staffEdit.userId;
        this.staffEdit.cards.push(result);
        let c = this.staffEdit.cards.length - 1;
        this.activateCardId(this.staffEdit.cards[c], c);
      }
    });
  }

  editCardId(row, i) {
    var dialog = this.dialog.open(UserCardIdEditorComponent, {
      data: { header: "Edit CardId", userCardId: row, isUserInfo: true },
      width: '400px',
      panelClass: 'user-info-modal',
      backdropClass: 'user-info-backdrop',
      disableClose: true
    });

    dialog.afterClosed().subscribe(result => {
      console.log("resulting data: ", result)
      if (result && result.cardId) {
        if (typeof (this.staffEdit) == typeof (undefined)) {
          this.staffEdit = new Staff();
        }

        if (typeof (this.staffEdit.cards) == typeof (undefined)) {
          this.staffEdit.cards = [];
        }

        result.userId = this.staffEdit.userId;
        this.staffEdit.cards[i] = result;
      }
    });
  }

  activateCardId(row, i) {
    for (var j = 0; j < this.staffEdit.cards.length; j++) {
      this.staffEdit.cards[j].status = "INACTIVE";
    }

    this.staffEdit.cards[i].status = "ACTIVE";
  }

  deleteCardId(row: UserCardId) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.cardId + '\" ?', DialogType.confirm, () => this.deleteCardIdHelper(row));
  }

  deleteCardIdHelper(row: UserCardId) {

    this.staffEdit.cards = this.staffEdit.cards.filter(item => item !== row);
  }

  get canManageStaffs() {
    return true; //this.accountService.userHasPermission(Permission.manageStaffsPermission)
  }
}
