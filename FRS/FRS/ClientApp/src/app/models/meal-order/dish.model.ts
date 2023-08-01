import { BentoBoxType } from "./bento-box-type.model";

export class Dish {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.code = name;
  }

  public id: string;
  public code: string;
  public label: string;
  public rrPrice: string;
  public cost: string;
  public dishTypeId: string;
  public dishTypeName: string;
  public cuisineId: string;
  public cuisineName: string;
  public isEnabled: boolean;

  public fileId: string;
  public fileName: string;
  public filePath: string;

  public productionPictureId: string;
  public productionPictureName: string;
  public productionPicturePath: string;

  public parentDishId: string;
  public bentoBoxTypeId: string;
  public bentoBoxTypeCode: string;
  public bentoBoxTypeDetails: string;
  public bentoBoxTypePicture: string;
  public checked: boolean;
  public parentDishLabel: string;
  public cookedWeight: number;
  public calories: number;
  public protein: number;
  public sugar: number;
  public totalFat: number;
  public totalCarb: number;
  public catererId: string;
  public dishPeriods: DishPeriod[];
  public subDishes: DishDetail[];
  public restrictions: DishRestriction[];
  public dishComponents: DishComponent[];
}

export class DishComponent {

  constructor(id?: string, name?: string) {
  }

  public dishId: string;
  public id: string;
  public category: string;
  public component: string;
  public weight: number;
  public quantity: number;
  public sapProductCode: string;
  public editMode: boolean;
}

export class DishPeriod {

  constructor(id?: string, name?: string) {
  }

  public dishId: string;
  public periodId: string;
  public dishName: string;
  public periodName: string;
}

export class DishRestriction {

  constructor() {
  }

  public dishId: string;
  public restrictionId: string;
  public checked: boolean;
}

export class DishDetail {

  constructor() {
  }

  public dishId: string;
  public parentDishId: string;
  public code: string;
  public label: string;
  public checked: boolean;
  public cookedWeight: number;
  public dish: Dish;
}
