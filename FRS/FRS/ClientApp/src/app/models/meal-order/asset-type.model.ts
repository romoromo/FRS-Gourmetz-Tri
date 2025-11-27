
export class CatererAssetType {

  constructor(id?: string, code?: string) {

    this.id = id;
    this.code = code;
  }

  public id: string;
  public code: string;
  public description: string;
  public catererInfoId: string;

  public fileId: string;
  public fileName: string;
  public filePath: string;
}
