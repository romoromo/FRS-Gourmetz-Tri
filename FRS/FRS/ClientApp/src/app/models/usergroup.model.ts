
export class UserGroup {

  constructor(id?: string, name?: string, description?: string) {

    this.id = id;
    this.name = name;
    this.description = description;
  }

  public id: string;
  public name: string;
  public description: string;
  public institutionId: string;

  public members: UserGroupMember[];
  public locations: UserGroupLocation[];
}


export class UserGroupMember {

  constructor(id?: string, userGroupId?: string, userId?: string) {

    this.id = id;
    this.userGroupId = userGroupId;
    this.userId = userId;
  }

  public id: string;
  public userGroupId: string;
  public userId: string;
  public name: string;
}

export class UserGroupLocation {

  constructor(id?: string, userGroupId?: string, locationId?: string) {

    this.id = id;
    this.userGroupId = userGroupId;
    this.locationId = locationId;
  }

  public id: string;
  public userGroupId: string;
  public locationId: string;
}
