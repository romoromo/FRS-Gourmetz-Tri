import { Component, ViewChild, Inject, OnInit, OnDestroy } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { DateAdapter, MatDatepickerInputEvent, MatDialog, MatDialogRef, MAT_DATE_FORMATS, MAT_DIALOG_DATA } from '@angular/material';
import { Student, StudentCard, StudentInterestGroup, StudentRestriction } from 'src/app/models/meal-order/student.model';
import { StudentGroup, StudentGroupDetail, StudentGroupSession, StudentGroupType } from 'src/app/models/meal-order/student-group.model';
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
import { MomentUtcDateAdapter } from 'src/app/helpers/moment-utc-adapter';
import { MenuService } from 'src/app/services/meal-order/menu.service';
import { FileService } from 'src/app/services/file.service';
import { getBaseUrl } from 'src/app/app.module';

export const CUSTOM_DATE_FORMAT = {
  parse: {
    dateInput: 'DD/MM/YYYY',
  },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MMMM YYYY',
    dateA11yLabel: 'DD/MM/YYYY',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'student-group-editor',
  templateUrl: './student-group-editor.component.html',
  styleUrls: ['./student-group-editor.component.css'],
  providers: [
    { provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMAT },
    { provide: DateAdapter, useClass: MomentUtcDateAdapter },
  ]
})
export class StudentGroupEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isNewGroup = false;
  private isChangePassword = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingStudentName: string;
  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  private groupEdit: StudentGroup = new StudentGroup();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private validation = { name: false, gender: false, classLevel: false, class: false, fas: false };
  private isAddAccount = false;
  private outletId;
  mealSessionDetails: any[] = [];
  mealSessions: any[] = [];
  terms: number[] = []; //[ 1, 2, 3, 4];
  start = new Date();
  end = new Date();
  original_type = '';
  original_mealSessionId = '';
  original_start = new Date();
  original_end = new Date();

  public students = [];
  public types = [StudentGroupType.OTHERS, StudentGroupType.MEAL_PLAN];
  public searchForm: FormControl = new FormControl();
  groupId: any;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private studentService: StudentService, private accountService: AccountService, private classService: ClassService,
    private fileService: FileService, private restrictionService: RestrictionService, private deliveryService: DeliveryService,
    public dialogRef: MatDialogRef<StudentGroupEditorComponent>, public dialog: MatDialog, private menuService: MenuService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.outletId) != typeof (undefined)) {
      this.outletId = data.outletId;
    }
    if (typeof (data.group) != typeof (undefined) && data.group.id) {
      this.editGroup(data.group);
    } else {
      this.newGroup();
    }

    //this.getStudents();
    this.getTerms();
  }

  ngOnInit() {

    this.onChangeDeliveryDate();

    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  getDishCode() {
    this.studentService.generateDishCode(this.outletId)
      .subscribe(results => {
        console.log(results);
        this.groupEdit.code = results.code;
      },
        error => {
        })
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

  getTerms() {
    let filter = new Filter();
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.studentService.getOutletTermsByFilter(filter)
      .subscribe(results => {
        this.terms = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving record.\r\n"`,
          //  MessageSeverity.error);
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

    this.groupEdit.sessions = [];
    this.mealSessions.forEach((p, index, ps) => {
      if (p.checked) {
        let sr = new StudentGroupSession();
        sr.studentGroupId = this.groupEdit.id;
        sr.mealSessionId = p.mealSessionId;
        this.groupEdit.sessions.push(sr);
      }

    });

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
    this.groupEdit.deliveryStartDate = this.getCutoffDate();
    this.groupEdit.deliveryEndDate = this.getCutoffDate();
    if (!this.groupEdit.code) {
      this.getDishCode();
    }
    return this.groupEdit;
  }

  editGroup(group: StudentGroup) {
    if (group) {
      this.isNewGroup = false;
      this.showValidationErrors = true;

      this.selectedValues = {};
      this.groupEdit = new StudentGroup();
      Object.assign(this.groupEdit, group);
      this.original_type = group.type;
      this.original_mealSessionId = group.mealSessionId;
      this.original_start = new Date(group.deliveryStartDate);
      this.original_end = new Date(group.deliveryEndDate);
      if (!this.groupEdit.price) {
        this.groupEdit.price = 0;
      }
      if (!this.groupEdit.code) {
        this.getDishCode();
      }
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
          detail.studentGroupId = this.groupEdit.id ? this.groupEdit.id : '0';
          detail.name = student.name;
          this.groupEdit.sgdetails.push(detail);
        })
        console.log("final sgdetails: ", this.groupEdit)
      }
    });
  }

  getCutoffDate() {
    let now = new Date();
    now.setHours(0, 0, 0, 0);
    now.setDate(now.getDate() + 3);
    return now;
  }

  getName(id) {
    let student = this.students.find(x => x.id === id);
    if (student) {
      return student.name
    } else {
      return '';
    }
  }

  onChangeDate(type: string, event: MatDatepickerInputEvent<Date>) {
    if (type == 'start') {
      this.start = new Date(event.value);
      this.groupEdit.startDate = new Date(event.value);
    }
    if (type == 'end') {
      this.end = new Date(event.value);
      this.groupEdit.endDate = new Date(event.value);
    }

  }

  onTypeChange(type: string) {
    if (this.groupEdit.id && type != StudentGroupType.MEAL_PLAN && this.original_type == StudentGroupType.MEAL_PLAN) {
      this.alertService.showDialog('Changing the type will remove all meal plans for the students in this group. Do you wish to proceed?',
        DialogType.confirm, () => { this.groupEdit.type = StudentGroupType.OTHERS }, () => { this.groupEdit.type = StudentGroupType.MEAL_PLAN });
    } 
  }

  onMealSessionChange(mealSessionId: string) {
    if (this.groupEdit.id && mealSessionId != this.original_mealSessionId) {
      this.alertService.showDialog('Changing the session will remove all meal plans for the students in this group and would require you to regenerate the meal plan. Do you wish to proceed?',
        DialogType.confirm, () => { this.groupEdit.mealSessionId = mealSessionId }, () => { this.groupEdit.mealSessionId = this.original_mealSessionId });
    }
  }

  onChangeDeliveryDate(event?: MatDatepickerInputEvent<Date>, type?: string) {

    if (type == 'from') {
      this.groupEdit.deliveryStartDate = event ? new Date(event.value) : new Date();
    }

    if (type == 'to') {
      this.groupEdit.deliveryEndDate = event ? new Date(event.value) : new Date();
    }

    let start = new Date(this.groupEdit.deliveryStartDate),
      end = new Date(this.groupEdit.deliveryEndDate);
    if (this.groupEdit.id && (((start).toDateString() != this.original_start.toDateString()) ||
      ((end).toDateString() != this.original_end.toDateString()))) {
      this.alertService.showDialog('Changing the dates will remove all meal plans for the students in this group and would require you to regenerate the meal plan. Do you wish to proceed?',
        DialogType.confirm, () => {
          this.groupEdit.deliveryStartDate = this.groupEdit.deliveryStartDate ? new Date(this.groupEdit.deliveryStartDate) : new Date();
          this.groupEdit.deliveryEndDate = this.groupEdit.deliveryEndDate ? new Date(this.groupEdit.deliveryEndDate) : new Date();
          
      }, () => {
          this.groupEdit.deliveryStartDate = this.original_start;
          this.groupEdit.deliveryEndDate = this.original_end;
      });
    }
    this.getMealSessions(start, end);
    
  }

  getMealSessions(d: Date, dTo: Date) {
    this.menuService.getOutletSessionsByFilter(this.outletId, (d).toDateString(), (dTo).toDateString())
      .subscribe(results => {
        this.mealSessionDetails = results;
        let mealSessions = [];
        
        if (this.mealSessionDetails) {
          this.mealSessionDetails.forEach((d, i, details) => {
            let indx = mealSessions && mealSessions.length > 0 ? mealSessions.findIndex(e => e.mealSessionId == d.mealSessionId) : -1;
            if (indx < 0) {
              mealSessions.push({ mealSessionId: d.mealSessionId, mealSessionName: d.mealSessionName });
            }
          })
        }

        this.mealSessions = mealSessions;
        this.mealSessions.forEach((p, index, ps) => {
          (<any>p).checked = this.groupEdit.sessions != null && this.groupEdit.sessions.findIndex(f => f.mealSessionId == p.mealSessionId) > -1;
        });

      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
            MessageSeverity.error);
        })
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.groupEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
    this.groupEdit.fileName = this.fileUploadResponse ? this.fileUploadResponse.fileName : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  downloadFile() {
    window.open(getBaseUrl() + '/gateway/Download/FileByPath?filePath=/' + encodeURIComponent(this.groupEdit.filePath), '_blank');
  }

  get canManageStudents() {
    return true; //this.accountService.userHasPermission(Permission.manageStudentsPermission)
  }
}
