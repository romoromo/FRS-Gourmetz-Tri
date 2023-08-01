import { Component, ViewChild, Inject, ViewEncapsulation } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { UserGroupService } from "../../../services/userGroup.service";
import { UserGroup, UserGroupLocation } from '../../../models/userGroup.model';
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Ng2TreeSettings, TreeComponent, TreeModel } from 'ng2-tree';
import { LocationService } from 'src/app/services/location.service';
import { CalendarFilter } from 'src/app/models/reservation.model';
import { Utilities } from 'src/app/services/utilities';
import { LocationTree } from 'src/app/models/location-tree.model';
import { LocationTreeFilter } from 'src/app/models/location.model';


@Component({
  selector: 'user-group-editor',
  templateUrl: './user-group-editor.component.html',
  styleUrls: ['./user-group-editor.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class UserGroupEditorComponent {
  @ViewChild('treeComponent') treeComponent: TreeComponent;
  tree: any;
  treeSettings: Ng2TreeSettings = {
    showCheckboxes: true,
    enableCheckboxes: true
  };

  private isNewUserGroup = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingUserGroupName: string;
  private userGroupEdit: UserGroup = new UserGroup();
  private selectedLocations: UserGroupLocation[] = [];
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  loadingIndicator: boolean;
  rootLocation: LocationTree;
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private userGroupService: UserGroupService, private accountService: AccountService,
    private locationService: LocationService,
    public dialogRef: MatDialogRef<UserGroupEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.selectedLocations = [];
    if (typeof (data.userGroup) != typeof (undefined)) {
      if (data.userGroup.id) {
        this.editUserGroup(data.userGroup);
      } else {
        this.newUserGroup();
      }

      this.loadTree(data.userGroup.institutionId);
    }
  }

  nodeUnchecked(event) {
    if (event && event.node && event.node.id > 0) {
      this.addRemoveLocation(event.node.id, false);
    }
    
  }
  nodeChecked(event) {
    if (event && event.node && event.node.id > 0) {
      this.addRemoveLocation(event.node.id, true);
    }
  }

  addRemoveLocation(id: string, isAdd: boolean) {
    let el = new UserGroupLocation('0', this.userGroupEdit.id, id);
    if (isAdd) {
      if (!this.selectedLocations || this.selectedLocations.length == 0) {
        this.selectedLocations.push(el);
      } else {
        let indx = this.selectedLocations.findIndex(e=> e.locationId == id);
        if (indx < 0) {
          this.selectedLocations.push(el);
        }
      }
    } else {
      let indx = this.selectedLocations.findIndex(e => e.locationId == id);
      if (indx > -1) {
        this.selectedLocations.splice(indx, 1);
      }
    }
  }

  loadTree(id) {
    this.tree = null;
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;
    var filter = new LocationTreeFilter();
    filter.institutionId = id ? id : this.accountService.currentUser.institutionId;
    this.locationService.getLocationTree(filter)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        if (results && results.children) {
          this.rootLocation = results.children[0];
          this.tree = results;
          this.setChildrenCheckboxes(this.tree.children);
          this.tree.settings = {
            'static': true,
            'rightMenu': true,
            'leftMenu': true,
            'cssClasses': {
              'expanded': 'fa fa-caret-down fa-sm',
              'collapsed': 'fa fa-caret-right fa-sm',
              'leaf': 'fa fa-sm',
              'empty': 'fa fa-caret-right disabled'
            },
            'templates': {
              'node': '<i class="fa fa-bank fa-sm"></i> ',
              'leaf': '<i class="fa fa-building-o fa-sm"></i> ',
              'leftMenu': '<i class="fa fa-navicon fa-sm"></i> '
            }
          };
        }
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve locations from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  setChildrenCheckboxes(children) {
    if (children) {
      children.forEach((item, indx) => {
        if (item.children && item.children.length > 0) {
          this.setChildrenCheckboxes(item.children);
        }

        if (this.selectedLocations.findIndex(f => f.locationId == item.id) > -1) {
          item.settings = {
            checked: true
          }
        } else {
          item.settings = {
            checked: false
          }
        }
      });
    }
    
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    //this.selectedLocations = this.selectedLocations.filter((item, index) => this.selectedLocations.indexOf(item) === index);
     //Array.from(new Set(this.selectedLocations));
    this.selectedLocations = Utilities.getUniqueValues(this.selectedLocations, 'locationId'); 
    
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.userGroupEdit.institutionId = this.accountService.currentUser.institutionId;
    this.userGroupEdit.locations = this.selectedLocations;
    if (this.isNewUserGroup) {
      this.userGroupService.newUserGroup(this.userGroupEdit).subscribe(userGroup => this.saveSuccessHelper(userGroup), error => this.saveFailedHelper(error));
    }
    else {
      this.userGroupService.updateUserGroup(this.userGroupEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(userGroup?: UserGroup) {
    if (userGroup)
      Object.assign(this.userGroupEdit, userGroup);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewUserGroup)
      this.alertService.showMessage("Success", `User Group \"${this.userGroupEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to user group \"${this.userGroupEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.userGroupEdit = new UserGroup();
    this.resetForm();


    //if (!this.isNewUserGroup && this.accountService.currentUser.facilities.some(r => r == this.editingUserGroupName))
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
    this.userGroupEdit = new UserGroup();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
  }

  private toggleGroup(groupName: string) {
    let firstMemberValue: boolean;

    this.allPermissions.forEach(p => {
      if (p.groupName != groupName)
        return;

      if (firstMemberValue == null)
        firstMemberValue = this.selectedValues[p.value] == true;

      this.selectedValues[p.value] = !firstMemberValue;
    });
  }


  private getSelectedPermissions() {
    return this.allPermissions.filter(p => this.selectedValues[p.value] == true);
  }


  resetForm(replace = false) {

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


  newUserGroup() {
    this.isNewUserGroup = true;
    this.showValidationErrors = true;

    this.editingUserGroupName = null;
    this.selectedValues = {};
    this.userGroupEdit = new UserGroup();

    return this.userGroupEdit;
  }

  editUserGroup(userGroup: UserGroup) {
    if (userGroup) {
      this.isNewUserGroup = false;
      this.showValidationErrors = true;

      this.editingUserGroupName = userGroup.name;
      this.selectedValues = {};
      this.userGroupEdit = new UserGroup();
      Object.assign(this.userGroupEdit, userGroup);
      this.selectedLocations = [];
      if (userGroup.locations) {
        userGroup.locations.forEach((item, indx) => {
          this.selectedLocations.push(new UserGroupLocation('0', item.userGroupId, item.locationId));
        });

        //userGroup.locations.forEach((item, indx) => {
        //  this.addRemoveLocation(item.locationId, true);
        //});
      }
      return this.userGroupEdit;
    }
    else {
      return this.newUserGroup();
    }
  }



  get canManageUserGroups() {
    return this.accountService.userHasPermission(Permission.manageUserGroupsPermission)
  }
}
