import { UserVehicle } from "./uservehicle.model";

export class UserPhonebook {

  constructor(id?: string, name?: string, email?: string) {

    this.id = id;
    this.name = name;
    this.email = email;
  }

  public id: string;
  public name: string;
  public email: string;
  public phoneNumber: string;
  public homeNo: string;
  public mobileNo: string;
  public designation: string;
  public filePath: string;
  public userId: string;
  public checked: boolean;
  public plateNumber: string;
  public vehicle: string;
  public issueDate?: Date;
  public expiryDate?: Date;
  public cardType: string;
  public selectedVehicle: UserVehicle;
  public vehicles: UserVehicle[];
}
