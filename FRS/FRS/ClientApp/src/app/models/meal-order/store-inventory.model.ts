
export class StoreInventory {

  constructor(id?: string) {

    this.id = id;
  }

  public id: string;
  public storeInfoId: string;
  public deliveryOrderId: string;
  public timeReceived: Date;
  public remarks: string;
  public deliveredBy: string;
  public storeInventoryDetails: StoreInventoryDetail[];
}

export class StoreInventoryDetail {

  constructor() {
  }

  public id: string;
  public storeInventoryId: string;
  public dishId: string;
  public cartonId: string;
  public storeInfoId: string;
  public timeReceived: Date;
  public remarks: string;
  public qtyExpected: number;
  public qtyReceived: number;
}
