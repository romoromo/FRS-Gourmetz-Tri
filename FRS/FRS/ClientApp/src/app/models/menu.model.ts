import { PermissionValues } from "./permission.model";

export class Menu {

  constructor(label?: string, active?: string) {
    this.label = label;
    this.active = active;
  }

  public id: string;
  public label: string;
  public active: string;
  public link: string;
  public logoClass: string;
  public permissions: PermissionValues[];
}

