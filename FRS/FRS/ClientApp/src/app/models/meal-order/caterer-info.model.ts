import { Outlet } from "./outlet.model";

export class CatererInfo {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  public code: string;
public address: string;
  public institutionId: string;
  public status: string;
  public catererOutlets: CatererOutlet[];    
}

export class CatererOutlet {

  constructor() {
  }

  public catererInfoId: string;
  public outletId: string;
  public isRsp: boolean;
  public outletName: string;
  public catererName: string;
  public outletProfileName: string;
  public outletProfileId: string;
  public status: string;
  public outlet: Outlet;
}
