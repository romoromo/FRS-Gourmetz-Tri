
export class ClassLevel {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public year: string;
  public institutionId: string;
  public outletId: string;
  public outletName: string;
  public mealSessionId: string;
  public mealSessionDetailId: string;
}
