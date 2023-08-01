
export class Menu {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.code = name;
  }

  public id: string;
  public code: string;
  public label: string;
  public startDate: Date;
  public endDate: Date;
  public cuisineId: string;
  public cuisineName: string;
  public catererId: string;
  public menuDishes: MenuDish[];
  public outletMenuDishes: MenuDish[];
  public mealTypeMenuDishes: MealTypeMenuDish[];
}

export class MealTypeMenuDish {
  public mealTypeId: string;
  public mealTypeName: string;
  public menuId: string;
  public menuDishes: MenuDish[];
}

export class MenuDish {

  constructor(id?: string, name?: string) {
  }

  public dishId: string;
  public menuId: string;
  public menuName: string;
  public dishName: string;
  public mealTypeId: string;
  public mealTypeName: string;
  public checked: boolean;
}
export class OutletMenuDish extends MenuDish {
  public outletId: string;
  public mealPeriodId: string;
}
