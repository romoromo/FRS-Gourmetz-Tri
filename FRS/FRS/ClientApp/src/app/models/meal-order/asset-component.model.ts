
export class AssetComponent {

  constructor(id?: string) {

    this.id = id;
  }

  public id: string;
  public catererAssetId: string;
 
  public description: string;
  public qty: number;
  public remarks: string;

  public fileId: string;
  public fileName: string;
  public filePath: string;

  public catererId: string;
}
