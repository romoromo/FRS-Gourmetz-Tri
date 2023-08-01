import { Component, ViewChild, Inject, OnInit, OnDestroy } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Student, StudentCard, StudentInterestGroup, StudentRestriction } from 'src/app/models/meal-order/student.model';
import { StudentGroup, StudentGroupDetail } from 'src/app/models/meal-order/student-group.model';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { CommonFilter, Filter } from 'src/app/models/sieve-filter.model';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { ClassLevel } from 'src/app/models/meal-order/class-level.model';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/services/meal-order/user.service';
import { UserCardIdEditorComponent } from '../../controls/usercardid/usercardid-editor.component';
import { UserCardId } from 'src/app/models/usercardid.model';
import { RestrictionService } from 'src/app/services/meal-order/restriction.service';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { FormControl } from '@angular/forms';
import { StudentSelectorComponent } from './student-selector/student-selector.component'


@Component({
  selector: 'student-group-editor',
  templateUrl: './student-group-editor.component.html',
  styleUrls: ['./student-group-editor.component.css']
})
export class StudentGroupEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isNewGroup = false;
  private isChangePassword = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingStudentName: string;
  private groupEdit: StudentGroup = new StudentGroup();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private validation = { name: false, gender: false, classLevel: false, class: false, fas: false };
  private isAddAccount = false;
  private outletId;

  public students = [];

  public searchForm: FormControl = new FormControl();
  groupId: any;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private studentService: StudentService, private accountService: AccountService, private classService: ClassService,
    private userService: UserService, private restrictionService: RestrictionService, private deliveryService: DeliveryService,
    public dialogRef: MatDialogRef<StudentGroupEditorComponent>, public dialog: MatDialog, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outletId) != typeof (undefined)) {
      this.outletId = data.outletId;
    }
    if (typeof (data.group) != typeof (undefined) && data.group.id) {
      this.editGroup(data.group);
    } else {
      this.newGroup();
    }

    this.getStudents();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  getStudents() {
    let filter = new Filter();
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.studentService.getStudentsByFilter(filter)
      .subscribe(results => {
        this.students = results.pagedData;
        console.log("students: ", this.students)
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving students.\r\n"`,
            MessageSeverity.error);
        })
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  //private validate() {
  //  this.validation = { name: false, gender: false, classLevel: false, class: false, fas: false };;
  //  if (this.studentEdit.name && this.studentEdit.name.trim().length == 0) {
  //    this.validation.name = true;
  //    //this.name.control.setErrors({});
  //  }

  //  if (!this.studentEdit.gender) {
  //    this.validation.gender = true;
  //    //this.value.control.setErrors({});
  //  }

  //  if (!this.studentEdit.classLevelId) {
  //    this.validation.classLevel = true;
  //    //this.description.control.setErrors({});
  //  }

  //  if (!this.studentEdit.classId) {
  //    this.validation.class = true;
  //    //this.description.control.setErrors({});
  //  }

  //  if (!this.studentEdit.isFAS) {
  //    this.validation.fas = true;
  //    //this.description.control.setErrors({});
  //  }

  //  if (this.validation.name || this.validation.gender || this.validation.classLevel || this.validation.class || this.validation.fas) {
  //    this.showValidationErrors = true;
  //    return false;
  //  }

  //  return true;
  //}

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.groupEdit.outletId = this.outletId;

    if (this.isNewGroup) {
      this.studentService.newStudentGroup(this.groupEdit).subscribe(group => this.saveSuccessHelper(group), error => this.saveFailedHelper(error));
    }
    else {
      this.studentService.updateStudentGroup(this.groupEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  invalidate() {
    return false;
  }

  private saveSuccessHelper(group?: StudentGroup) {
    if (group)
      Object.assign(this.groupEdit, group);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewGroup)
      this.alertService.showMessage("Success", `Student group \"${this.groupEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to student group \"${this.groupEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.groupEdit = new StudentGroup();
    this.resetForm();

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
    this.groupEdit = new StudentGroup();

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


  newGroup() {
    this.isNewGroup = true;
    this.editingStudentName = null;
    this.selectedValues = {};
    this.groupEdit = new StudentGroup();
    return this.groupEdit;
  }

  editGroup(group: StudentGroup) {
    if (group) {
      this.isNewGroup = false;
      this.showValidationErrors = true;

      this.selectedValues = {};
      this.groupEdit = new StudentGroup();
      Object.assign(this.groupEdit, group);
      return this.groupEdit;
    }
    else {
      return this.newGroup();
    }
  }

  addStudentSel() {
    const dialogRef = this.dialog.open(StudentSelectorComponent, {
      data: { header: "Students", outletId: this.outletId, students: this.groupEdit.sgdetails },
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result.isCancel) {
        console.log("early selstore:", this.groupEdit.sgdetails);
        console.log("change selstore:", result.selectedStudents);
        this.groupEdit.sgdetails = [];
        result.selectedStudents.forEach(student => {
          var detail = new StudentGroupDetail();
          detail.studentId = student.id;
          detail.studentGroupId = this.groupEdit.id;
          this.groupEdit.sgdetails.push(detail);
        })
        console.log("final sgdetails: ", this.groupEdit)
      }
    });
  }

  getName(id) {
    let student = this.students.find(x => x.id === id);
    if (student) {
      return student.name
    } else {
      return '';
    }
  }

  get canManageStudents() {
    return true; //this.accountService.userHasPermission(Permission.manageStudentsPermission)
  }
}
