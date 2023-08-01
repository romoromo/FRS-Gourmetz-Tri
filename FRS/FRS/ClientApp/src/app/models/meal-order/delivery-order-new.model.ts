
export class DeliveryOrderNew {

  public id: string;
  public doNumber: string;
  public institutionId: string;

  public deliveryAddress: string;

  public catererInfoId?: string;

  public catererInfoName?: string;

  public fromStoreId?: string;

  public fromStoreName?: string;

  public toStoreId?: string;

  public toStoreName?: string;

  public deliveryDetails: any;

  public completed: Boolean;

  public mealSessionDetailId: string;

  public routeId: string;

  public vehicleNumber: string;

  public createdDate: Date;

  public pickupTime: Date;

  public loadingTime: Date;

  public receivingTime: Date;

  public routeName: string;
}
