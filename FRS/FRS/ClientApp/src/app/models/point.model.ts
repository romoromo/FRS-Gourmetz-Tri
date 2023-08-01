import { DirectoryListing } from "./directory-listing.model";
import { LineModel } from "./line.model";

export class PointModel {
  public id: string;
  public code: string;
  public label: string;

  public x: number;
  public y: number;

  public isFloorConnector: boolean;

  public noWheelchair: boolean;

  public noSheltered: boolean;

  public MapId: string;

  public isActive: boolean;
  directorys: DirectoryListing[];
    connectors: LineModel[];
}
