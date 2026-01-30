import { Component, ViewChild, Inject, OnInit, OnDestroy } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Student, StudentCard, StudentInterestGroup, StudentRestriction } from 'src/app/models/meal-order/student.model';
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
import { Utilities } from '../../../services/utilities';
import { UserEdit } from '../../../models/user-edit.model';
import { FileService } from 'src/app/services/file.service';


@Component({
  selector: 'student-editor',
  templateUrl: './student-editor.component.html',
  styleUrls: ['./student-editor.component.css']
})
export class StudentEditorComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private isNewStudent = false;
  private isChangePassword = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingStudentName: string;
  private studentEdit: Student = new Student();
  private allPermissions: Permission[] = [];
  outlets = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  private validation = { name: false, gender: false, classLevel: false, class: false, fas: false };
  private isAddAccount = false;

  public classLevels = [];
  public batches = [];
  public classes = [];
  public userTypes = [];
  public restrictions = [];
  public restrictionTypes = [];
  public interestGroups = [];
  public outletId: string;

  public searchForm: FormControl = new FormControl();
  studentId: any;
  public users = [];

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  private studentCardOwner: Student;


  @ViewChild('f')
  private form;

    public fileUploadResponse: { dbPath: ""; fileId: null; fileName: "" };

  constructor(private alertService: AlertService, private studentService: StudentService, private accountService: AccountService, private classService: ClassService,
    private userService: UserService, private restrictionService: RestrictionService, private deliveryService: DeliveryService,
    public dialogRef: MatDialogRef<StudentEditorComponent>, public dialog: MatDialog, private fileService: FileService, 
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.outletId = data.outletId;
    this.editStudent(data.student);

    //this.getOutlets();
    //this.getClassLevels();
    this.getClassLevels(false, this.studentEdit.outletId);

    this.getClassBatches();
    //this.getUserTypes();
    this.getRestrictions();
    this.getInterestGroups();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  getOutlets() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.deliveryService.getOutletsSimpleByFilter(filter)
      .subscribe(results => {
        this.outlets = results.pagedData;
        this.getClassLevels(false, this.studentEdit.outletId);
      },
        error => {
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving outlets.\r\n"`,
            MessageSeverity.error);
        })
  }

  getRestrictionTypes() {
    let filter = new Filter();
    filter.sorts = 'label';
    //let f = this.menuCycle.catererId ? '(CatererId)==' + this.menuCycle.catererId + ',' : '';
    filter.filters = '(IsActive)==true';
    //filter.filters = f + '(IsActive)==true';
    return this.restrictionService.getRestrictionTypesByFilter(filter);
  }

  getRestrictions() {
    this.getRestrictionTypes().subscribe(res => {
        this.restrictionTypes = res.pagedData;
        let filter = new Filter();
        filter.filters = '(IsActive)==true';
      this.restrictionService.getRestrictionsByFilter(filter)
        .subscribe(results => {
          this.restrictions = results.pagedData;
          this.restrictionTypes.forEach((rt, index, ps) => {
            rt.restrictions = [];
            rt.restrictions = this.restrictions.filter(e => e.restrictionTypeId == rt.id);
            rt.restrictions.forEach((p, index, ps) => {
              (<any>p).checked = this.studentEdit.restrictions != null && this.studentEdit.restrictions.findIndex(f => f.restrictionId == p.id) > -1;
            });
          });
        },
          error => {
            this.alertService.showStickyMessage("Get Error", `An error occured while retrieving restriction types.\r\n"`,
              MessageSeverity.error);
          });

      console.log(this.restrictionTypes);
        },
      error => {
        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving the records.\r\n"`,
          MessageSeverity.error);
      });
  }

  getInterestGroups() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.studentService.getInterestGroupsByFilter(filter)
      .subscribe(results => {
        this.interestGroups = results.pagedData;
        this.interestGroups.forEach((p, index, ps) => {
          (<any>p).checked = this.studentEdit.interestGroups != null && this.studentEdit.interestGroups.findIndex(f => f.interestGroupId == p.id) > -1;
        });
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving interest groups.\r\n"`,
            MessageSeverity.error);
        })
  }

  getClassBatches() {
    let filter = new Filter();
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    this.subscription.add(this.classService.getClassBatchesByFilter(filter)
      .subscribe(results => {
        this.batches = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving batches.\r\n"`,
            MessageSeverity.error);
        }));
  }

  getClassLevels(isClearValue: boolean, id?: string) {
    if (id) {
      if (isClearValue) {
        this.studentEdit.classLevelId = '';
        this.studentEdit.classId = '';
      }
      let filter = new Filter();
      filter.filters = '(IsActive)==true,(OutletId)==' + id;
      this.subscription.add(this.classService.getClassLevelsByFilter(filter)
        .subscribe(results => {
          this.classLevels = results.pagedData;
          this.getClasses(false, this.studentEdit.classLevelId);
        },
          error => {
            //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            this.alertService.showStickyMessage("Get Error", `An error occured while retrieving levels.\r\n"`,
              MessageSeverity.error);
          }));
    } else {
      this.classLevels = [];
    }
  }

  getClasses(isClearValue: boolean, classLevelId?: string) {
    if (classLevelId) {
      if (isClearValue) this.studentEdit.classId = '';
      let filter = new Filter();
      filter.filters = '(IsActive)==true,(ClassLevelId)==' + classLevelId;

      this.subscription.add(this.classService.getClassesByFilter(filter)
        .subscribe(results => {
          this.classes = results.pagedData;
        },
          error => {
            //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            this.alertService.showStickyMessage("Get Error", `An error occured while retrieving classes.\r\n"`,
              MessageSeverity.error);
          }));
    } else {
      this.classes = [];
    }
  }

  onOutletChange(outId?: string) {
    this.getClassLevels(true, outId);
  }

  onClassLevelChange(classLevelId?: string) {
    this.getClasses(true, classLevelId);
  }

  //getUserTypes() {
  //  let filter = new Filter();
  //  filter.filters = '(IsActive)==true';
  //  this.subscription.add(this.userService.getUserTypesByFilter(filter)
  //    .subscribe(results => {
  //      this.userTypes = results.pagedData;
  //    },
  //      error => {
  //        //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
  //        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving user types.\r\n"`,
  //          MessageSeverity.error);
  //      }));
  //}

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  private validate() {
    this.validation = { name: false, gender: false, classLevel: false, class: false, fas: false };;
    if (this.studentEdit.name && this.studentEdit.name.trim().length == 0) {
      this.validation.name = true;
      //this.name.control.setErrors({});
    }

    if (!this.studentEdit.gender) {
      this.validation.gender = true;
      //this.value.control.setErrors({});
    }

    if (!this.studentEdit.classLevelId) {
      this.validation.classLevel = true;
      //this.description.control.setErrors({});
    }

    if (!this.studentEdit.classId) {
      this.validation.class = true;
      //this.description.control.setErrors({});
    }

    if (!this.studentEdit.isFAS) {
      this.validation.fas = true;
      //this.description.control.setErrors({});
    }

    if (this.validation.name || this.validation.gender || this.validation.classLevel || this.validation.class || this.validation.fas) {
      this.showValidationErrors = true;
      return false;
    }

    return true;
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    let selectedRestrictions = this.restrictions.filter(f => f.checked);
    this.studentEdit.restrictions = [];
    this.restrictions.forEach((p, index, ps) => {
      if (p.checked) {
        let sr = new StudentRestriction();
        sr.studentId = this.studentEdit.id;
        sr.restrictionId = p.id;
        this.studentEdit.restrictions.push(sr);
      }

    });

    let selectedInterestGroups = this.restrictions.filter(f => f.checked);
    this.studentEdit.interestGroups = [];
    this.interestGroups.forEach((p, index, ps) => {
      if (p.checked) {
        let sr = new StudentInterestGroup();
        sr.studentId = this.studentEdit.id;
        sr.interestGroupId = p.id;
        this.studentEdit.interestGroups.push(sr);
      }

    });

    if (this.studentEdit.username) {
      this.studentEdit.username = this.studentEdit.username.replace(/\s/g, '');
    }

    if (this.studentEdit.email) {
      this.studentEdit.email = this.studentEdit.email.replace(/\s/g, '');
    }

    if (this.isNewStudent) {
      this.studentService.newStudent(this.studentEdit).subscribe(student => this.saveSuccessHelper(student), error => this.saveFailedHelper(error));
    }
    else {
      this.studentService.updateStudent(this.studentEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  invalidate() {
    return false;
  }

  private saveSuccessHelper(student?: Student) {
    if (student)
      Object.assign(this.studentEdit, student);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewStudent)
      this.alertService.showMessage("Success", `Student \"${this.studentEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to student \"${this.studentEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.studentEdit = new Student();
    this.resetForm();


    //if (!this.isNewStudent && this.accountService.currentUser.facilities.some(r => r == this.editingStudentName))
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
    this.studentEdit = new Student();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close(true);
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


  newStudent() {
    this.isNewStudent = true;
    this.showValidationErrors = true;
    this.isChangePassword = false;
    this.editingStudentName = null;
    this.selectedValues = {};
    this.studentEdit = new Student();
    this.studentEdit.gender = 'Male';
    this.studentEdit.isFAS = false;
    this.studentEdit.outletId = this.outletId;
    if (!this.studentEdit.studentCards) this.studentEdit.studentCards = [];
    console.log('editStudent', this.studentEdit);
    return this.studentEdit;
  }

  editStudent(student: Student) {
    if (student && student.id) {
      this.isNewStudent = false;
      this.showValidationErrors = true;

      this.editingStudentName = student.name;
      this.selectedValues = {};
      this.studentEdit = new Student();
      Object.assign(this.studentEdit, student);
      this.isAddAccount = this.studentEdit.userId && parseInt(this.studentEdit.userId) > 0;
      if (!this.studentEdit.studentCards) this.studentEdit.studentCards = [];
      if (!this.studentEdit.outletId) this.studentEdit.outletId = this.outletId;

      console.log('editStudent', this.studentEdit);
      return this.studentEdit;
    }
    else {
      return this.newStudent();
    }
  }

  private changePassword() {
    this.isChangePassword = true;
  }

  resetPassword(row: Student) {
    this.alertService.showDialog('Are you sure you want to reset the password for \"' + row.email + '\"?', DialogType.confirm, () => this.resetPasswordHelper(row));
  }


  resetPasswordHelper(row: Student) {

    this.alertService.startLoadingMessage("Resetting...");

    this.subscription.add(this.accountService.resetPassword(row.email)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();

        if (results && results.isSuccess) {
          this.alertService.showMessage('A link to reset the password has been sent to the email.');
        }
      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Reset Password Error", `An error occured while resetting the password.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  public deletePasswordFromUser(student: Student) {
    let userEdit = <Student>student;

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
        if (typeof (this.studentEdit) == typeof (undefined)) {
          this.studentEdit = new Student();
        }

        if (typeof (this.studentEdit.studentCards) == typeof (undefined)) {
          this.studentEdit.studentCards = [];
        }

        let existingCards: StudentCard[] = [];
        if (this.studentEdit.studentCards) {
          existingCards = this.studentEdit.studentCards.filter(e => e.cardId == result.cardId);
        }

        this.studentService.getStudentByCardId(result.cardId)
          .subscribe(results => {
            console.log("results: ", results)
            this.studentCardOwner = results

            console.log("student card owner: ", this.studentCardOwner)

            if (this.studentCardOwner) {
              var promptMessage = 'This Card ( ' + result.cardId + ' ) is being used by ' + this.studentCardOwner.id + '.' + this.studentCardOwner.name + ' ( ' + this.studentCardOwner.outletName + ':' + this.studentCardOwner.className + '). Do you want to proceed with transferring the card number?';

              this.alertService.showDialog(promptMessage, DialogType.confirm, () => {

                if (existingCards && existingCards.length > 0) {
                  alert('Card exists.');
                } else {
                  result.studentId = this.studentEdit.id;
                  this.studentEdit.studentCards.push(result);
                  let c = this.studentEdit.studentCards.length - 1;
                  this.activateCardId(this.studentEdit.studentCards[c], c);
                }

              }, null, 'Yes', 'No', 'No')
            } else {
              if (existingCards && existingCards.length > 0) {
                alert('Card exists.');
              } else {
                result.studentId = this.studentEdit.id;
                this.studentEdit.studentCards.push(result);
                let c = this.studentEdit.studentCards.length - 1;
                this.activateCardId(this.studentEdit.studentCards[c], c);
              }
            }

          },
            error => {
              //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
              this.alertService.showStickyMessage("Get Error", `An error occured while retrieving previous card owner.\r\n"`,
                MessageSeverity.error);
            })
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
        if (typeof (this.studentEdit) == typeof (undefined)) {
          this.studentEdit = new Student();
        }

        if (typeof (this.studentEdit.studentCards) == typeof (undefined)) {
          this.studentEdit.studentCards = [];
        }

        this.studentService.getStudentByCardId(result.cardId)
          .subscribe(results => {
            console.log("results: ", results)
            this.studentCardOwner = results

            console.log("student card owner: ", this.studentCardOwner)

            if (this.studentCardOwner) {
              var promptMessage = 'This Card ( ' + result.cardId + ' ) is being used by ' + this.studentCardOwner.id + '.' + this.studentCardOwner.name + ' ( ' + this.studentCardOwner.outletName + ':' + this.studentCardOwner.className + '). Do you want to proceed with transferring the card number?';

              this.alertService.showDialog(promptMessage, DialogType.confirm, () => {

                result.studentId = this.studentEdit.id;
                this.studentEdit.studentCards[i] = result;

              }, null, 'Yes', 'No', 'No')
            } else {
              result.studentId = this.studentEdit.id;
              this.studentEdit.studentCards[i] = result;
            }


          },
            error => {
              //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
              this.alertService.showStickyMessage("Get Error", `An error occured while retrieving previous card owner.\r\n"`,
                MessageSeverity.error);
            })
      }
    });
  }

  activateCardId(row, i) {
    for (var j = 0; j < this.studentEdit.studentCards.length; j++) {
      this.studentEdit.studentCards[j].status = "INACTIVE";
    }

    this.studentEdit.studentCards[i].status = "ACTIVE";
  }

  deleteCardId(row: StudentCard) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.cardId + '\" ?', DialogType.confirm, () => this.deleteCardIdHelper(row));
  }

  deleteCardIdHelper(row: StudentCard) {

    this.studentEdit.studentCards = this.studentEdit.studentCards.filter(item => item !== row);
  }

  get canManageStudents() {
    return true; //this.accountService.userHasPermission(Permission.manageStudentsPermission)
  }

   public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.studentEdit.photoPath = this.fileUploadResponse
      ? this.fileUploadResponse.dbPath
      : null;
  };

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  removePhoto() {
    this.studentEdit.photoPath = null;
    this.studentEdit.photoId = null;
    this.studentEdit.photoName = null;
  }
}
