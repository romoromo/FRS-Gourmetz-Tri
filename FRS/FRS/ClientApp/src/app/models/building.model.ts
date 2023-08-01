import { Floor } from "./floor.model";

export class Building {
  public id: string;
  public code: string;
  public label: string;

  floors: Floor[];
}
