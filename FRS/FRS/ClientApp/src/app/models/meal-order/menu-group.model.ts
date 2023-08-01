
export class MenuGroup {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public outletId: string;
  public outletName: string;
  public startDate: Date;
  public endDate: Date;
  public isPublished: boolean;
  public menuGroupDishCycles: MenuGroupDishCycle[];
  public classes: MenuGroupClass[];
}

export class MenuGroupDishCycle {
  public id: string;
  public dishCycleId: string;
  public dishCycleLabel: string;
  public menuGroupId: string;
  public menuGroupName: string;
}

export class MenuGroupClass {
  public id: string;
  public classId: string;
  public className: string;
  public menuGroupId: string;
  public menuGroupName: string;
}
