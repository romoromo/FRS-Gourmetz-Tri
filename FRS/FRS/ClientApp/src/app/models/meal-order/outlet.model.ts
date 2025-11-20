import { CatererOutlet } from "./caterer-info.model";
import { MealSession } from "./meal-session.model";
import { StoreInfo } from "./store-info.model";
import { extend } from "webdriver-js-extender";

export class OutletProfile {

  constructor(id?: string, label?: string) {

    this.id = id;
    this.label = label;
  }

  public id: string;
  public label: string;
  public catererId: string;
  public outlets: Outlet[];    
}

export class OutletSimple {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public address: string;
  public locationId: string;
  public locationName: string;
  public outletProfileId: string;
  public daysToFreezeOrdering: string;
  public createdBy: string;

  public passcode: string;
}

export class Outlet extends OutletSimple {

  constructor(id?: string, name?: string) {
    super(id, name);
  }

  public catererIds: string[];
  public catererOutlets: CatererOutlet[];
  public mealSessions: MealSession[];
  public stores: StoreInfo[];
}

export class OutletTerm {

  constructor(id?: string, label?: string) {

    this.id = id;
    this.label = label;
  }

  public id: string;
  public label: string;
  public outletId: string;
}
