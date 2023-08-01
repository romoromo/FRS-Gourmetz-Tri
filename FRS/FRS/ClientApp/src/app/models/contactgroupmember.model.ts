
export class ContactGroupMember {

  constructor(id?: string, name?: string, email?: string, contactGroupId?: string) {

    this.id = id;
    this.name = name;
    this.email = email;
    this.contactGroupId = contactGroupId;
  }

  public id: string;
  public name: string;
  public email: string;
  public contactGroupId: string;

  public department: string;
  public company: string;
  public designation: string;
  public phoneNumber: string;
  public homeNo: string;
  public mobileNo: string;
  public institutionId: string;
  public filePath: string;
  public userId: string;
}
