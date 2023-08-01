export class LocationTree {

  constructor(id: number, value: string, type: string, children: LocationTree[]) {

    this.value = value;
    this.id = id;
    this.children = children;
  }

  public id: number;
  public value: string;
  public type: string;
  public children: LocationTree[];
}

