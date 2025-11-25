
export class AssetType {

  constructor(id?: string, name?: string) {

    this.id = id;
    this.name = name;
  }

  public id: string;
  public name: string;
  
  public fileId: string;
  public filePath: string;
  public fileName: string;
  public institutionId: string;
}
