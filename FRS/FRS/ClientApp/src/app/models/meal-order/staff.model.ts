import { UserCardId } from "../usercardid.model";

export class Staff {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public gender: string;
  public departmentId: string;
  public departmentName: string;
  public staffTypeId: string;
  public staffTypeName: string;

  public userId: string;
  public username: string;
  public email: string;
  public fullName: string;
  public newPassword: string;
  public currentPassword: string;
  public confirmPassword: string;

  public cards: UserCardId[];
}
