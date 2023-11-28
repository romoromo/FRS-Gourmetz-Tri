import { UserPhonebook } from "./userphonebook.model";
import { AccountService } from "../services/account.service";
import { Permission } from "./permission.model";
import { UserVehicle } from "./uservehicle.model";
import { UserCardId } from "./usercardid.model";
import { Injectable } from "@angular/core";
import * as signalRCore from '@aspnet/signalr';
import { UserGroup, UserGroupMember } from "./userGroup.model";
import { UserWallet } from "./userwallet.model";
import { UserReward } from "./userreward.model";

export class User {
  students: any;
  // Note: Using only optional constructor properties without backing store disables typescript's type checking for the type
  constructor(id?: string, userName?: string, fullName?: string, email?: string, jobTitle?: string, phoneNumber?: string, roles?: string[], institutionId?: string, pin?: string, departmentId?: string, employeeId?: string, registeredDepartment?: string, unitNumber?: string) {

    this.id = id;
    this.userName = userName;
    this.fullName = fullName;
    this.email = email;
    this.jobTitle = jobTitle;
    this.phoneNumber = phoneNumber;
    this.roles = roles;
    this.institutionId = institutionId;
    this.pin = pin;
    this.departmentId = departmentId;
    this.employeeId = employeeId;
    this.registeredDepartment = registeredDepartment;
    this.unitNumber = unitNumber;
  }


  get friendlyName(): string {
    let name = this.fullName || this.userName;

    if (this.jobTitle)
      name = this.jobTitle + " " + name;

    return name;
  }

  public id: string;
  public userName: string;
  public fullName: string;
  public email: string;
  public jobTitle: string;
  public phoneNumber: string;
  public isEnabled: boolean;
  public isLockedOut: boolean;
  public roles: string[];
  public institutionId?: string;
  public institutionCode: string;
  public pin?: string;
  public employeeId?: string;
  public departmentId?: string;
  public departmentName?: string;
  public registeredDepartment?: string;
  public provider?: string;
  public key?: string;
  public isExternal: boolean;
  public checked: boolean;
  public userPhonebooks: UserPhonebook[];
  public mobileNo: string;
  public homeNo: string;
  public unitNumber: string;
  public filePath: string;
  public fileId: string;
  public plateNumber: string;
  public vehicle: string;
  public issueDate?: Date;
  public expiryDate?: Date;
  public cardType: string;
  public status: string;
  public isConnected: boolean;
  public userVehicles: UserVehicle[];
  public userCardIds: UserCardId[];
  public userGroupMembers: UserGroupMember[];
  public userGroupIds: string[];
  public userWallets: UserWallet[];
  public userRewards: UserReward[];
  public last2FAValidatedTime: string;
  public isAD: boolean;
  public userOutlets: UserOutlet[];
  public userCaterers: UserCaterer[];
}

export class UserOutlet {
  public id: string;
  public userId: string;
  public outletId: string;
  public outletName: string;
}

export class UserCaterer {
  public id: string;
  public userId: string;
  public catererId: string;
  public catererName: string;
}

@Injectable()
export class FRSHubConnections {
  userHubConnection: signalRCore.HubConnection;
  forceStop: boolean;
  sessionInterval: any;
  numOfTries: number;
}

export class UserReportFilter {
  public keyword: string;
  public departmentId: string;
  public userGroupId: string;
  public createdStart?: Date;
  public createdEnd?: Date;
  public lastLoginStart?: Date;
  public lastLoginEnd?: Date;
  public deletedDate?: Date;
  public status?: boolean;
}

export class UserRoleReportFilter {
  public keyword: string;
  public permissions: string[];
  public roleId: string;
  public createdStart?: Date;
  public createdEnd?: Date;
  public lastUpdatedStart?: Date;
  public lastUpdatedEnd?: Date;
}
