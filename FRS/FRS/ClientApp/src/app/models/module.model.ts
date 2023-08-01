
export class Module {

  constructor(id?: string, name?: string, route?: string) {

    this.id = id;
    this.name = name;
    this.route = route;
  }

  public id: string;
  public name: string;
  public route: string;
  public uri?: string;
  public moduleParameters: ModuleParameter[];
}

export class ModuleParameter {

  constructor(id?: string, name?: string, url?: string) {

    this.id = id;
    this.name = name;
    this.url = url;
  }

  public id: string;
  public name: string;
  public url: string;
}
