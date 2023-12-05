import { AfterViewInit, Component, OnChanges, SimpleChanges, ViewChild, ViewEncapsulation } from '@angular/core';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from "../../services/account.service";
import { Role } from '../../models/role.model';
import { Permission, PermissionNames, PermissionTree, PermissionValues } from '../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Ng2TreeSettings, TreeComponent, TreeModel } from 'ng2-tree';
import { Utilities } from 'src/app/services/utilities';
import { IActionMapping, ITreeOptions } from 'angular-tree-component';

@Component({
  selector: 'role-editor',
  templateUrl: './role-editor.component.html',
  styleUrls: ['./role-editor.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class RoleEditorComponent implements AfterViewInit {
  //@ViewChild('treeComponent') treeComponent: TreeComponent;
  @ViewChild('tree') treeComponent;
  nodes = [];

  treeSettings: Ng2TreeSettings = {
    showCheckboxes: true,
    enableCheckboxes: true,
    rootIsVisible: true
  };

  actionMapping: IActionMapping = {
    mouse: {
      click: (tree, node) => this.check(node, !node.data.checked)
    }
  };

  options: ITreeOptions = {
    //useCheckbox: true,
    //displayField: 'description',
    useTriState: false,
    actionMapping: this.actionMapping
  };

  public check(node, checked) {

    //if (node.children && node.children.length > 0) {
    //  //means a parent is being checked; check all children
    //  this.updateChildNodeCheckbox(node, checked);
    //  this.updateCheckedIndeterminate(node);
    //} else {
    //  //means a leaf node; just check if all siblings are checked
    //  node.data.checked = checked;
    //  this.updateCheckedIndeterminate(node.realParent);
    //}
    //if (checked) {
    //  this.addRemovePermission(node.id, node.data.data.value, true);
    //} else {
    //  this.addRemovePermission(node.id, node.data.data.value, false);
    //}
    
    this.updateChildNodeCheckbox(node, checked);
    this.updateParentNodeCheckbox(node.realParent);

  }
  public updateChildNodeCheckbox(node, checked) {
    node.data.checked = checked;
    if (checked) {
      this.addRemovePermission(node.id, node.data.value, true);
    } else {
      this.addRemovePermission(node.id, node.data.value, false);
    }

    if (checked) {
      node.data.indeterminate = false;
    }
    if (node.children) {
      node.children.forEach((child) => this.updateChildNodeCheckbox(child, checked));
    }
  }
  public updateParentNodeCheckbox(node) {
    if (!node) {
      return;
    }
    //this.updateCheckedIndeterminate(node);
    if (node.data.checked) {
      this.addRemovePermission(node.id, node.data.value, true);
    } else {
      this.addRemovePermission(node.id, node.data.value, false);
    }
    if (node.realParent)
      this.updateParentNodeCheckbox(node.realParent);
  }

  updateCheckedIndeterminate(node) {
    let allChildrenChecked = true;
    let noChildChecked = true;

    for (const child of node.children) {
      if (!child.data.checked || child.data.indeterminate) {
        allChildrenChecked = false;
      }
      if (child.data.checked) {
        noChildChecked = false;
      }
    }

    if (allChildrenChecked) {
      node.data.checked = true;
      node.data.indeterminate = false;
    } else if (noChildChecked) {
      node.data.checked = false;
      node.data.indeterminate = false;
    } else {
      node.data.checked = true;
      node.data.indeterminate = true;
    }
  }

  loadingIndicator: boolean;
  rootLocation: PermissionTree;
  private selectedPermissions: Permission[] = [];

  private isNewRole = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingRoleName: string;
  private roleEdit: Role = new Role();
  private allPermissions: Permission[] = [];
  private allPermissionsTree: PermissionTree;
  private selectedValues: { [key: string]: boolean; } = {};

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;


  @ViewChild('f')
  private form;



  constructor(private alertService: AlertService, private accountService: AccountService) {

  }


  nodeUnchecked(event) {
    console.log('nodeUnchecked');
    console.log(event);
    if (event && event.node && event.node.id > 0) {
      this.addRemovePermission(event.node.id, event.node.data.value, false);
    }

  }
  nodeChecked(event) {
    console.log('nodeChecked');
    console.log(event);
    if (event && event.node && event.node.id > 0) {
      this.addRemovePermission(event.node.data.name, event.node.data.value, true);
    }
  }


  addRemovePermission(name: string, value: string, isAdd: boolean) {
    let el = new Permission('' as PermissionNames, value as PermissionValues);
    if (isAdd) {
      if (!this.selectedPermissions || this.selectedPermissions.length == 0) {
        this.selectedPermissions.push(el);
      } else {
        let indx = this.selectedPermissions.findIndex(e => e.value == value);
        if (indx < 0) {
          this.selectedPermissions.push(el);
        }
      }
    } else {
      let indx = this.selectedPermissions.findIndex(e => e.value == value);
      if (indx > -1) {
        this.selectedPermissions.splice(indx, 1);
      }
    }

  }

  ngAfterViewInit() {
    if (this.treeComponent && this.treeComponent.treeModel && this.treeComponent.treeModel.roots && this.treeComponent.treeModel.roots.length > 0) {
      this.treeComponent.treeModel.roots[0].expand();
    }
  }

  loadTree() {
    this.nodes = null;
    this.rootLocation = this.allPermissionsTree;
    console.log(this.allPermissionsTree);
    this.nodes = this.allPermissionsTree.children;

    if (this.treeComponent && this.treeComponent.treeModel && this.treeComponent.treeModel.roots) {
      setTimeout(() => {
        this.treeComponent.treeModel.roots[0].expand();
      }, 500);


    }


    this.setChildrenCheckboxes(this.nodes);
    //this.tree.settings = {
    //  'static': true,
    //  'rightMenu': true,
    //  'leftMenu': true,
    //  'cssClasses': {
    //    'expanded': 'fa fa-caret-down fa-sm',
    //    'collapsed': 'fa fa-caret-right fa-sm',
    //    'leaf': 'fa fa-sm',
    //    'empty': 'fa fa-caret-right disabled'
    //  },
    //  'templates': {
    //    'node': '<i class="fa fa-sm"></i> ',
    //    'leaf': '<i class="fa fa-sm"></i> ',
    //    'leftMenu': '<i class="fa fa-navicon fa-sm"></i> '
    //  }
    //};
  }

  setChildrenCheckboxes(children) {
    if (children) {
      children.forEach((item, indx) => {
        if (item.children && item.children.length > 0) {
          this.setChildrenCheckboxes(item.children);
        }

        if (this.selectedPermissions.findIndex(f => f.value == item.value) > -1) {
          //this.treeComponent.treeModel.getNodeById(item.id).setIsSelected(true);
          item.checked = true;
        } else {
          item.checked = false;
          //this.treeComponent.treeModel.getNodeById(item.id).setIsSelected(false);
        }
      });
    }

  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    this.selectedPermissions = Utilities.getUniqueValues(this.selectedPermissions, 'value'); 
    console.log(this.selectedPermissions)
    this.roleEdit.permissions = this.selectedPermissions;//this.getSelectedPermissions();

    if (this.isNewRole) {
      this.accountService.newRole(this.roleEdit).subscribe(role => this.saveSuccessHelper(role), error => this.saveFailedHelper(error));
    }
    else {
      this.accountService.updateRole(this.roleEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(role?: Role) {
    if (role)
      Object.assign(this.roleEdit, role);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewRole)
      this.alertService.showMessage("Success", `Role \"${this.roleEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to role \"${this.roleEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.roleEdit = new Role();
    this.resetForm();


    if (!this.isNewRole && this.accountService.currentUser.roles.some(r => r == this.editingRoleName))
      this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();
  }


  private refreshLoggedInUser() {
    this.accountService.refreshLoggedInUser()
      .subscribe(user => { },
        error => {
          this.alertService.resetStickyMessage();
          this.alertService.showStickyMessage("Refresh failed", "An error occured while refreshing logged in user information from the server", MessageSeverity.error);
        });
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
    this.roleEdit = new Role();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();
  }



  private selectAll() {
    this.allPermissions.forEach(p => this.selectedValues[p.value] = true);
  }


  private selectNone() {
    this.allPermissions.forEach(p => this.selectedValues[p.value] = false);
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

  private getSelectedPermissionsFromTree() {
    if (this.treeComponent && this.treeComponent.treeModel && this.treeComponent.treeModel.roots) {
      this.treeComponent.treeModel.roots[0];
    }
    
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


  newRole(allPermissions: Permission[], allPermissionsTree?: PermissionTree) {
    this.isNewRole = true;
    this.showValidationErrors = true;

    this.editingRoleName = null;
    this.allPermissions = allPermissions;
    this.allPermissionsTree = allPermissionsTree;
    this.selectedValues = {};
    this.roleEdit = new Role();
    return this.roleEdit;
  }

  editRole(role: Role, allPermissions: Permission[], allPermissionsTree?: PermissionTree) {
    if (role) {
      this.isNewRole = false;
      this.showValidationErrors = true;

      this.editingRoleName = role.name;
      this.allPermissions = allPermissions;
      this.allPermissionsTree = allPermissionsTree;
      this.selectedValues = {};
      //role.permissions.forEach(p => this.selectedValues[p.value] = true);

      this.selectedPermissions = [];
      if (role.permissions) {
        role.permissions.forEach((item, indx) => {
          if (item) {
            this.selectedPermissions.push(new Permission('' as PermissionNames, item.value as PermissionValues));
          }
        });
      }

      this.roleEdit = new Role();
      Object.assign(this.roleEdit, role);

      return this.roleEdit;
    }
    else {
      return this.newRole(allPermissions);
    }
  }



  get canManageRoles() {
    return this.accountService.userHasPermission(Permission.manageRolesPermission)
  }
}
