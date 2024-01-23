import { Component, OnInit, ViewChild, Input, ViewEncapsulation, OnDestroy } from '@angular/core';

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import { Utilities } from '../../services/utilities';
import { User, UserOutlet, UserCaterer } from '../../models/user.model';
import { UserEdit } from '../../models/user-edit.model';
import { Role } from '../../models/role.model';
import { Permission } from '../../models/permission.model';
import { DepartmentService } from 'src/app/services/department.service';
import { Department } from 'src/app/models/department.model';
import { UserVehicleEditorComponent } from './uservehicle/uservehicle-editor.component';
import { UserCardIdEditorComponent } from './usercardid/usercardid-editor.component';
import { UserVehicle } from 'src/app/models/uservehicle.model';
import { UserCardId } from 'src/app/models/usercardid.model';
import { MatDialog } from '@angular/material';
import { FileService } from 'src/app/services/file.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { UserGroup, UserGroupMember } from 'src/app/models/userGroup.model';
import { UserGroupService } from 'src/app/services/userGroup.service';
import { Subscription } from 'rxjs';
import { WalletOperation } from 'src/app/models/userwallet.model';
import { StudentService } from '../../services/meal-order/student.service';
import { FormControl } from '@angular/forms';
import { Outlet } from 'src/app/models/meal-order/outlet.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { CatererInfo } from 'src/app/models/meal-order/caterer-info.model';


@Component({
  selector: 'user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class UserInfoComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private isEditMode = false;
  private isNewUser = false;
  private isSaving = false;
  private isChangePassword = false;
  private isEditingSelf = false;
  private showValidationErrors = false;
  private editingUserName: string;
  private uniqueId: string = Utilities.uniqueId();
  private user: User = new User();
  private userEdit: UserEdit;
  private allRoles: Role[] = [];
  private allDepartments: Department[] = [];
  private allUserGroups: UserGroup[] = [];
  private allUserOutlets: Outlet[] = [];
  private allUserCaterers: CatererInfo[] = [];
  private allStatuses: string[] = ['AVAILABLE', 'ENGAGED', 'OFFDUTY', 'CUSTOM'];
  private selectedStatus: string;
  private selectedUserGroupIds: string[];
  private selectedUserOutletIds: string[];
  private selectedUserCatererIds: string[];
  
  public formResetToggle = true;
  private topUpAmount:number = 0;
  private walletAmount: number = 0;
  public isTopUp = true;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @Input()
  isViewOnly: boolean;

  @Input()
  isGeneralEditor = false;





  @ViewChild('f')
  private form;

  //ViewChilds Required because ngIf hides template variables from global scope
  @ViewChild('userName')
  private userName;

  @ViewChild('userPassword')
  private userPassword;

  @ViewChild('email')
  private email;

  @ViewChild('currentPassword')
  private currentPassword;

  @ViewChild('newPassword')
  private newPassword;

  @ViewChild('confirmPassword')
  private confirmPassword;

  @ViewChild('roles')
  private roles;

  @ViewChild('rolesSelector')
  private rolesSelector;

  @ViewChild('outletsSelector')
  private outletsSelector;
  //@ViewChild('userGroupsSelector')
  //private userGroupsSelector;

  @ViewChild('departmentsSelector')
  private departmentsSelector;

  public fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  students: any;

  public searchForm: FormControl = new FormControl();
    studentId: any;

  constructor(private alertService: AlertService, private accountService: AccountService, private departmentService: DepartmentService,
    private fileService: FileService, public dialog: MatDialog, private userGroupService: UserGroupService, private studentService: StudentService,
    private deliveryService: DeliveryService) {

    this.getStudents();
  }

  addStudent() {
    if (!this.studentId) return;

    let source = this.students.find(x => x.id == this.studentId);

    if (!source) return;

    this.userEdit.students = this.userEdit.students || [];

    let dest = this.userEdit.students.find(x => x.studentId == this.studentId);

    if (!dest) {
      this.userEdit.students.push({
        id: 0,
        studentId: this.studentId,
        userId: this.userEdit.id,
        name: source.name,
        isActive: true
      });
    } else {
      dest.isActive = true;
    }

    this.studentId = null;
  }

  getStudents() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.studentService.getStudentsByFilter(filter)
      .subscribe(results => {
        this.students = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving carton types.\r\n"`,
            MessageSeverity.error);
        })
  }

  ngOnInit() {
    this.walletAmount = 0;
    this.topUpAmount = 0;
    this.isTopUp = false;
    //this.loadDepartmets();
    this.loadUserGroups();
    this.loadUserOutlets();
    this.loadUserCaterers();
    if (!this.isGeneralEditor) {
      this.loadCurrentUserData();
      
    }
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  private loadCurrentUserData() {
    this.alertService.startLoadingMessage();

    if (this.canViewAllRoles) {
      this.subscription.add(this.accountService.getUserAndRoles(this.accountService.currentUser.id, this.accountService.currentUser.institutionId).subscribe(results => this.onCurrentUserDataLoadSuccessful(results[0], results[1], results[2]), error => this.onCurrentUserDataLoadFailed(error)));
    }
    else {
      var department: Department[] = [];
      department.push(new Department(this.accountService.currentUser.departmentId, this.accountService.currentUser.departmentName));
      this.subscription.add(this.accountService.getUser(this.accountService.currentUser.id).subscribe(user => this.onCurrentUserDataLoadSuccessful(user, user.roles.map(x => new Role(x)), department), error => this.onCurrentUserDataLoadFailed(error)));
    }
  }

  private loadUserGroups() {
    let filter = new Filter(-1, -1);
    filter.filters = '(IsActive)==true,(InstitutionId)==' + this.accountService.currentUser.institutionId;

    this.subscription.add(this.userGroupService.getUserGroupsByFilter(filter)
      .subscribe(results => {
        this.allUserGroups = results.pagedData;

        //if (this.allUserGroups == null || this.allUserGroups.length != this.allUserGroups.length)
        //  setTimeout(() => this.userGroupsSelector.refresh());

      },
        error => {
          this.alertService.showMessage("Load Error", "Loading user groups from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);
        }));
  }

  private loadUserOutlets() {
    let filter = new Filter(-1, -1);
    filter.filters = '(IsActive)==true';

    this.subscription.add(this.deliveryService.getOutletsSimpleByFilter(filter)
      .subscribe(results => {
        var allUserOutlets = results.pagedData;
        allUserOutlets.map(cg => {
          let outlet = new Outlet(cg.id, cg.name);
          this.allUserOutlets.push(outlet);
        });

        this.allUserOutlets = [...this.allUserOutlets];
      },
        error => {
          this.alertService.showMessage("Load Error", "Loading user outlets from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);
        }));
  }

  private loadUserCaterers() {
    let filter = new Filter(-1, -1);
    filter.filters = '(IsActive)==true';

    this.subscription.add(this.deliveryService.getCatererInfosSimpleByFilter(filter)
      .subscribe(results => {
        var allUserCaterers = results.pagedData;
        allUserCaterers.map(cg => {
          let caterer = new CatererInfo(cg.id, cg.name);
          this.allUserCaterers.push(caterer);
        });

        this.allUserCaterers = [...this.allUserCaterers];
      },
        error => {
          //this.alertService.showMessage("Load Error", "Loading user outlets from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);
        }));
  }

  private loadDepartmets() {
    this.subscription.add(this.departmentService.getDepartments(1, 10, this.accountService.currentUser.institutionId)
      .subscribe(results => {
        this.allDepartments = results;
      },
        error => {
          this.alertService.showMessage("Load Error", "Loading departments from the server failed!", MessageSeverity.warn);
          this.alertService.logError(error);
        }));
  }

  selectUserGroup(ev) {
    console.log(ev);
    this.selectedUserGroupIds = ev;
  }

  private onCurrentUserDataLoadSuccessful(user: User, roles: Role[], departments: Department[]) {
    this.alertService.stopLoadingMessage();
    this.user = user;
    this.allRoles = roles;
    this.allDepartments = departments;

    this.selectedUserGroupIds = user.userGroupIds;
    this.selectedUserOutletIds = [];
    if (user.userOutlets) {
      user.userOutlets.map(cg => {
        this.selectedUserOutletIds.push(cg.outletId);
      });
    }

    this.selectedUserCatererIds = [];
    if (user.userCaterers) {
      user.userCaterers.map(cg => {
        this.selectedUserCatererIds.push(cg.catererId);
      });
    }

    this.getWalletByUserId(user.id);
  }

  selectUserOutlet(ev) {
    console.log(ev);
    //this.selectedUserOutletIds = ev;

    if (ev) {
      this.userEdit.userOutlets = [];
      ev.forEach((ug, index) => {
        if (ug) {
          let m = new UserOutlet();
          m.outletId = ug;
          m.userId = this.userEdit.id;
          this.userEdit.userOutlets.push(m);
        }
      });

    }
  }

  selectUserCaterer(ev) {
    console.log(ev);
    //this.selectedUserOutletIds = ev;

    if (ev) {
      this.userEdit.userCaterers = [];
      ev.forEach((ug, index) => {
        if (ug) {
          let m = new UserCaterer();
          m.catererId = ug;
          m.userId = this.userEdit.id;
          this.userEdit.userCaterers.push(m);
        }
      });

    }
  }

  private onCurrentUserDataLoadFailed(error: any) {
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Load Error", `Unable to retrieve user data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
      MessageSeverity.error);

    this.user = new User();
  }



  private getRoleByName(name: string) {
    return this.allRoles.find((r) => r.name == name)
  }

  private getOutletById(id: string) {
    console.log('calling outlets 2...');
    return this.allUserOutlets.find((r) => r.id == id)
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  public deletePasswordFromUser(user: UserEdit | User) {
    let userEdit = <UserEdit>user;

    delete userEdit.currentPassword;
    delete userEdit.newPassword;
    delete userEdit.confirmPassword;
  }


  private edit() {
    if (!this.isGeneralEditor) {
      this.isEditingSelf = true;
      this.userEdit = new UserEdit();
      Object.assign(this.userEdit, this.user);
    }
    else {
      if (!this.userEdit)
        this.userEdit = new UserEdit();

      this.isEditingSelf = this.accountService.currentUser ? this.userEdit.id == this.accountService.currentUser.id : false;
    }

    this.isEditMode = true;
    this.showValidationErrors = true;
    this.isChangePassword = false;

    if (this.userEdit) {
      if (this.allStatuses.findIndex(e => e == this.userEdit.status) < 0) {
        this.selectedStatus = 'CUSTOM';
      } else {
        this.selectedStatus = this.userEdit.status;
      }
    }

    this.selectedUserGroupIds = this.userEdit.userGroupIds;
    this.selectedUserOutletIds = [];
    if (this.userEdit.userOutlets) {
      this.userEdit.userOutlets.map(cg => {
        this.selectedUserOutletIds.push(cg.outletId);
      });
    }

    this.selectedUserCatererIds = [];
    if (this.userEdit.userCaterers){
      this.userEdit.userCaterers.map(cg => {
        this.selectedUserCatererIds.push(cg.catererId);
      });
    }

    console.log(this.allUserOutlets);
    console.log('calling outlets...');
    setTimeout(() => this.outletsSelector.refresh());
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    if (this.selectedUserGroupIds) {
      this.userEdit.userGroupMembers = [];
      this.selectedUserGroupIds.forEach((ug, index) => {
        if (ug) {
          let m = new UserGroupMember();
          m.userGroupId = ug;
          m.userId = this.userEdit.id;
          this.userEdit.userGroupMembers.push(m);
        }
      });
      
    }

    //if (this.selectedUserOutletIds) {
    //  this.userEdit.userOutlets = [];
    //  this.selectedUserOutletIds.forEach((ug, index) => {
    //    if (ug) {
    //      let m = new UserOutlet();
    //      m.outletId = ug;
    //      m.userId = this.userEdit.id;
    //      this.userEdit.userOutlets.push(m);
    //    }
    //  });

    //}

    if (this.userEdit.email) {
      this.userEdit.email = this.userEdit.email.replace(/\s/g, '');
    }

    if (this.userEdit.userName) {
      this.userEdit.userName = this.userEdit.userName.replace(/\s/g, '');
    }
    
    if (!this.userEdit.institutionId) {
      this.userEdit.institutionId = this.accountService.currentUser.institutionId;
    }

    if (this.isNewUser) {
      this.subscription.add(this.accountService.newUser(this.userEdit).subscribe(user => this.saveSuccessHelper(user), error => this.saveFailedHelper(error)));
    }
    else {
      this.subscription.add(this.accountService.updateUser(this.userEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error)));
    }
  }


  private saveSuccessHelper(user?: User) {
    this.testIsRoleUserCountChanged(this.user, this.userEdit);

    if (user)
      Object.assign(this.userEdit, user);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.isChangePassword = false;
    this.showValidationErrors = false;

    this.deletePasswordFromUser(this.userEdit);
    Object.assign(this.user, this.userEdit);
    this.userEdit = new UserEdit();
    this.resetForm();


    if (this.isGeneralEditor) {
      if (this.isNewUser)
        this.alertService.showMessage("Success", `User \"${this.user.userName}\" was created successfully`, MessageSeverity.success);
      else if (!this.isEditingSelf)
        this.alertService.showMessage("Success", `Changes to user \"${this.user.userName}\" was saved successfully`, MessageSeverity.success);
    }

    if (this.isEditingSelf) {
      this.alertService.showMessage("Success", "Changes to your User Profile was saved successfully", MessageSeverity.success);
      this.refreshLoggedInUser();
    }

    this.isEditMode = false;


    if (this.changesSavedCallback)
      this.changesSavedCallback();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }



  private testIsRoleUserCountChanged(currentUser: User, editedUser: User) {

    let rolesAdded = this.isNewUser || currentUser.roles == null ? editedUser.roles : editedUser.roles.filter(role => currentUser.roles.indexOf(role) == -1);
    let rolesRemoved = this.isNewUser || currentUser.roles == null ? [] : currentUser.roles.filter(role => editedUser.roles.indexOf(role) == -1);

    let modifiedRoles = rolesAdded.concat(rolesRemoved);

    if (modifiedRoles.length)
      setTimeout(() => this.accountService.onRolesUserCountChanged(modifiedRoles));
  }



  private cancel() {
    if (this.isGeneralEditor)
      this.userEdit = this.user = new UserEdit();
    else
      this.userEdit = new UserEdit();

    this.showValidationErrors = false;
    this.resetForm();

    //this.alertService.showMessage("Cancelled", "Operation cancelled by user", MessageSeverity.default);
    this.alertService.resetStickyMessage();

    if (!this.isGeneralEditor)
      this.isEditMode = false;

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();
  }


  private close() {
    this.userEdit = this.user = new UserEdit();
    this.showValidationErrors = false;
    this.resetForm();
    this.isEditMode = false;

    if (this.changesSavedCallback)
      this.changesSavedCallback();
  }



  private refreshLoggedInUser() {
    this.subscription.add(this.accountService.refreshLoggedInUser()
      .subscribe(user => {
        this.loadCurrentUserData();
      },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("Refresh failed", "An error occured while refreshing logged in user information from the server", MessageSeverity.error);
        }));
  }


  private changePassword() {
    this.isChangePassword = true;
  }


  private unlockUser() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Unblocking user...");


    this.subscription.add(this.accountService.unblockUser(this.userEdit.id)
      .subscribe(response => {
        this.isSaving = false;
        this.userEdit.isLockedOut = false;
        this.alertService.stopLoadingMessage();
        this.alertService.showMessage("Success", "User has been successfully unblocked", MessageSeverity.success);
      },
        error => {
          this.isSaving = false;
          this.alertService.stopLoadingMessage();
          this.alertService.showStickyMessage("Unblock Error", "The below errors occured while unblocking the user:", MessageSeverity.error);
          this.alertService.showStickyMessage(error, null, MessageSeverity.error);
        }));
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


  newUser(allRoles: Role[], allDepartments: Department[]) {
    this.isGeneralEditor = true;
    this.isNewUser = true;

    this.allRoles = [...allRoles];
    this.allDepartments = [...allDepartments];
    this.editingUserName = null;
    this.user = this.userEdit = new UserEdit();
    this.userEdit.isEnabled = true;
    this.walletAmount = 0;
    this.topUpAmount = 0;
    this.isTopUp = false;
    this.edit();

    return this.userEdit;
  }

  editUser(user: User, allRoles: Role[], allDepartments: Department[]) {
    if (user) {
      this.isGeneralEditor = true;
      this.isNewUser = false;

      this.setRolesDepartments(user, allRoles, allDepartments);
      this.editingUserName = user.userName;
      this.user = new User();
      this.userEdit = new UserEdit();
      Object.assign(this.user, user);
      Object.assign(this.userEdit, user);
      this.getWalletByUserId(user.id);
      this.isTopUp = false;
      this.edit();

      if (user.students && this.students) {
        for (let s of user.students) {
          let source = this.students.find(x => x.id == s.studentId);

          if (source) s.name = source.name;
        }
      }

      return this.userEdit;
    }
    else {
      return this.newUser(allRoles, allDepartments);
    }
  }


  displayUser(user: User, allRoles?: Role[], allDepartments?: Department[]) {

    this.user = new User();
    Object.assign(this.user, user);
    this.deletePasswordFromUser(this.user);
    this.setRolesDepartments(user, allRoles, allDepartments);

    this.isEditMode = false;
  }



  private setRolesDepartments(user: User, allRoles?: Role[], allDepartments?: Department[]) {
    this.allDepartments = allDepartments ? [...allDepartments] : [];
    this.allRoles = allRoles ? [...allRoles] : [];

    if (user.roles) {
      for (let ur of user.roles) {
        if (!this.allRoles.some(r => r.name == ur))
          this.allRoles.unshift(new Role(ur));
      }
    }

    if (allRoles == null || this.allRoles.length != allRoles.length)
      setTimeout(() => this.rolesSelector.refresh());

    if (allDepartments == null || this.allDepartments.length != allDepartments.length)
      setTimeout(() => this.departmentsSelector.refresh());


    
  }

  addVehicle() {
    var dialog = this.dialog.open(UserVehicleEditorComponent, {
      data: { header: "New vehicle", userVehicle: new UserVehicle(), isUserInfo: true },
      width: '400px',
      panelClass: 'user-info-modal',
      backdropClass: 'user-info-backdrop',
    });

    dialog.afterClosed().subscribe(result => {
      if (result && result.plateNumber) {
        if (typeof (this.userEdit) == typeof (undefined)) {
          this.userEdit = new UserEdit();
        }

        if (typeof (this.userEdit.userVehicles) == typeof (undefined)) {
          this.userEdit.userVehicles = [];
        }

        result.userId = this.userEdit.id;
        this.userEdit.userVehicles.push(result);
      }
    });
  }

  deleteVehicle(row: UserVehicle) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.plateNumber + '\" ?', DialogType.confirm, () => this.deleteVehicleHelper(row));
  }

  deleteVehicleHelper(row: UserVehicle) {

    this.userEdit.userVehicles = this.userEdit.userVehicles.filter(item => item !== row);
  }

  addCardId() {
    var dialog = this.dialog.open(UserCardIdEditorComponent, {
      data: { header: "New CardId", userCardId: new UserCardId(), isUserInfo: true },
      width: '400px',
      panelClass: 'user-info-modal',
      backdropClass: 'user-info-backdrop',
    });

    dialog.afterClosed().subscribe(result => {
      console.log("resulting data: ", result)
      if (result && result.cardId) {
        if (typeof (this.userEdit) == typeof (undefined)) {
          this.userEdit = new UserEdit();
        }

        if (typeof (this.userEdit.userCardIds) == typeof (undefined)) {
          this.userEdit.userCardIds = [];
        }

        result.userId = this.userEdit.id;
        this.userEdit.userCardIds.push(result);
      }
    });
  }

  editCardId(row,i) {
    var dialog = this.dialog.open(UserCardIdEditorComponent, {
      data: { header: "Edit CardId", userCardId: row, isUserInfo: true },
      width: '400px',
      panelClass: 'user-info-modal',
      backdropClass: 'user-info-backdrop',
    });

    dialog.afterClosed().subscribe(result => {
      console.log("resulting data: ", result)
      if (result && result.cardId) {
        if (typeof (this.userEdit) == typeof (undefined)) {
          this.userEdit = new UserEdit();
        }

        if (typeof (this.userEdit.userCardIds) == typeof (undefined)) {
          this.userEdit.userCardIds = [];
        }

        result.userId = this.userEdit.id;
        this.userEdit.userCardIds[i] = result;
      }
    });
  }

  activateCardId(row, i) {
    for (var j = 0; j < this.userEdit.userCardIds.length; j++) {
      this.userEdit.userCardIds[j].status = "INACTIVE";
    }

    this.userEdit.userCardIds[i].status = "ACTIVE";
  }

  deleteCardId(row: UserCardId) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.cardId + '\" ?', DialogType.confirm, () => this.deleteCardIdHelper(row));
  }

  deleteCardIdHelper(row: UserCardId) {

    this.userEdit.userCardIds = this.userEdit.userCardIds.filter(item => item !== row);
  }

  getWalletByUserId(userId) {
    this.subscription.add(this.accountService.getWallet(userId).subscribe(response => {
      console.log(response);
      if (response && response.length > 0) {
        this.walletAmount = response[0].balance;

        let u = this.isEditMode ? this.userEdit : this.user;
        if (u.userWallets && u.userWallets.length > 0) {
          u.userWallets[0].concurrencyStamp = response[0].concurrencyStamp;
        }
      }
      
    }, error => {
      console.log(error);
    }));
  }

  topUpWallet() {
    let wallet = new WalletOperation();
    let u = this.isEditMode ? this.userEdit : this.user;
    if (u.userWallets && u.userWallets.length > 0) {
      wallet.walletId = u.userWallets[0].id;
      wallet.concurrencyStamp = u.userWallets[0].concurrencyStamp;
      wallet.transactionType = "CREDIT";
      wallet.description = "TOP UP";
    }
   
    wallet.amount = this.topUpAmount;
    this.subscription.add(this.accountService.transactWallet(wallet).subscribe(response =>
    {
      console.log(response);
      if (response.isSuccess) {
        this.alertService.showMessage("Success", "Successful Cash In!", MessageSeverity.success);
      } else {
        this.alertService.showMessage("Error", response.message, MessageSeverity.error);
      }
      
      this.topUpAmount = 0;
      this.getWalletByUserId(this.isEditMode ? this.userEdit.id : this.user.id);
    }, error => {
        console.log(error);
    }));
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    this.userEdit.filePath = this.fileUploadResponse ? this.fileUploadResponse.dbPath : null;
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  changeStatus(ev) {
    if (ev == 'CUSTOM') {
      this.selectedStatus = 'CUSTOM';
      if (this.userEdit && this.user) {
        if (this.allStatuses.findIndex(e => e == this.user.status) < 0) {
          this.userEdit.status = this.user.status;
        } else {
          this.userEdit.status = '';
        }
      }
    } else {
      if (this.userEdit) {
        this.userEdit.status = ev;
      }
    }
  }

  get canViewAllRoles() {
    return this.accountService.userHasPermission(Permission.viewRolesPermission);
  }

  get canAssignRoles() {
    return this.accountService.userHasPermission(Permission.assignRolesPermission);
  }

  get canManageDepartments() {
    return this.accountService.userHasPermission(Permission.manageDepartmentsPermission);
  }

  get canAssignUserGroups() {
    return this.accountService.userHasPermission(Permission.manageUserGroupsPermission);
  }
}
