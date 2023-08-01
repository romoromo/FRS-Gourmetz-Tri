import { PointModel } from "./point.model";
import { LineModel } from "./line.model";

export class MapModel {
  public id: string;
  public code: string;
  public label: string;

  public floorId: string;

  public map_url: string;

  public points: PointModel[];
  public lines: LineModel[];
}
