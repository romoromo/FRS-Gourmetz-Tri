
export class MealType {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public price: number;
  public institutionId: string;
  public chargeType: string;
  public cuisineId: string;
  public cuisineName: string;
  public dishNames: string;
  public catererId: string;
  public isSubMenu: boolean;
  public dishes: MealTypeDish[];
}

export class MealTypeDish {

  constructor(id?: string, name?: string) {
  }

  public dishId: string;
  public mealTypeId: string;
  public mealTypeName: string;
  public dishName: string;
  public checked: boolean;
}
