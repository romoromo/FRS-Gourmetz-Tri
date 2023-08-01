import { ContactGroupMember } from "./contactgroupmember.model";

export class ContactGroup {

  constructor(id?: string, name?: string, description?: string) {

    this.id = id;
    this.name = name;
    this.description = description;
  }

  public id: string;
  public name: string;
  public description: string;
  public departmentIds: number[];
  public departmentNames: string;
  public checked: boolean;
  public institutionId: string;
  public members: ContactGroupMember[];
}
