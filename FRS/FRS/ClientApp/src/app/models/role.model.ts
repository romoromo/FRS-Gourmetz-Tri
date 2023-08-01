import { Permission } from './permission.model';


export class Role {

  constructor(name?: string, description?: string, institutionId?: string, permissions?: Permission[]) {

    this.name = name;
    this.description = description;
    this.permissions = permissions;
    this.institutionId = institutionId;
  }

  public id: string;
  public name: string;
  public description: string;
  public usersCount: string;
  public institutionId: string;
  public permissions: Permission[];
}
