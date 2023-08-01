export class BaseAsset {
  public serialNumber: string;
  public assetModelId: string;
  public assetModelName: string;
  public assetTypeName: string;
  public purchaseDate?: Date;
  public warrantyStart?: Date;
  public warrantyEnd?: Date;
  public hidden: boolean;
  public assetId: string;
}

export class Asset extends BaseAsset {
  public id: string;

  public locationId: string;
  public institutionId: string;
  public locationName: string;
  public institutionName: string;

  public poNumber: string;
    selected: boolean;
}

export class LocationAsset extends BaseAsset {
  public locationId: string;

}

export class ServiceContractAsset extends BaseAsset {
  public serviceContractId: string;

}
