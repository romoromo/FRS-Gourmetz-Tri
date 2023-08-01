export class Facility {

  constructor(name?: string, description?: string, capacity?: Number, institutionName?: string, link?: string) {

    this.name = name;
    this.description = description;
    this.institutionName = institutionName;
    this.link = link;
    this.capacity = capacity;
  }

  public id: string;
  public name: string;
  public description: string;
  public institutionName: string;
  public capacity: Number;
  public filePath: string;
  public fileName: string;
  public institutionId: number;
  public link: string;
}

export class UserInfoVal {
  constructor(
    id: number,
    title: string,
    color: string
  ) {

  }
  public id: number;
  public title: string;
  public color: string;

}
