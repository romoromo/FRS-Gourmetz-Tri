
export class OrderPortalContent {

  constructor(id?: string, name?: string) {

    this.id = id;
  }

  public id: string;
  public outletId: string;
  public description: string;
  public announcement: string;
  public effectiveStartDate: Date;
  public effectiveEndDate: Date;
  public putletId: string;
  public banners: OrderPortalBanner[];
}

export class OrderPortalBanner {

  constructor(id?: string, name?: string) {
  }

  public id: string;
  public url: string;
  public title: string;
  public subtitle: string;
  public order: number;
  public imageId: string;
  public orderPortalContentId: string;
  public fileName: string;
  public filePath: string;
}
