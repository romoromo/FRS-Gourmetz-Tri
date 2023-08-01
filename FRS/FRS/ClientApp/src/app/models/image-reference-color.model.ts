
export class ImageReferenceColor {

  constructor(id?: string, name?: string, colorCode?: string) {

    this.id = id;
    this.name = name;
    this.colorCode = colorCode;
  }

  public id: string;
  public name: string;
  public colorCode: string;
  public institutionId: string;
}
