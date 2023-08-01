
export class MealPeriod {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public sequence: number;
  public institutionId: string;
  public outletProfileId: string;
  public outletProfileName: string;
  public catererId: string;
  public startDate: Date;
  public endDate?: Date;
  public checked: boolean;
}
