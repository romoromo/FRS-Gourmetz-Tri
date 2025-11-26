
export class CatererAsset {

  constructor(id?: string) {

    this.id = id;
  }

  public id: string;
  public assetTypeId: string;
  public description: string;
  public assetQRCode: string;

  public fileId: string;
  public fileName: string;
  public filePath: string;

  public catererId: string;
}
