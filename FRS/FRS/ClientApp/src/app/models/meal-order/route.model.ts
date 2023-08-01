
export class Route {

  constructor(id?: string, label?: string) {

    this.id = id;
    this.label = label;
  }

  public id: string;
  public label: string;
  public details: string;
  public pickup: Date;
  public institutionId: string;

  public nodes: RouteNode[];
}

export class RouteNode {

  constructor() {
  }

  public routeId: string;
  public storeId: string;
  public order: number;
  public interval: Date;
}
