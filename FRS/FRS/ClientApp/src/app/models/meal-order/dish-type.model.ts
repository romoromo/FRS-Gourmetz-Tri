
export class DishType {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public institutionId: string;
  public catererId: string;
  public dishTypePeriods: DishTypePeriod[];
}

export class DishTypePeriod {

  constructor(id?: string, name?: string) {
  }

  public dishTypeId: string;
  public periodId: string;
  public dishTypeName: string;
  public periodName: string;
}
