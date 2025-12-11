
export class SortingArea {

  constructor(id?: string, code?: string) {

    this.id = id;
    this.code = code;
  }

  public id: string;
  public code: string;
  public description: string;

  public catererId: string;

  public routeId: string;
  public routeColor: string;
  public routeDetail: string;
}
