import { Permission } from './permission.model';
import { Facility } from './facility.model';
import { ContactGroup } from './contactgroup.model';
import { timeInterval } from 'rxjs/operators';

export class EmsProfile {

  //constructor(
  //  code?: string,
  //  label?: string,
  //  isScreenOn?: boolean,

  //  rotation?: string,
  //  resolution?: string,

  //  brightness?: string,
  //  volume?: string,

  //  module_path?: string,
  //  module_path_value?: string,

  //  communicatorMenuId?: string,
  //) {

  //  this.code = code;
  //  this.label = label;
  //  this.isScreenOn = isScreenOn;
  //  this.rotation = rotation;
  //  this.resolution = resolution;
  //  this.brightness = brightness;
  //  this.volume = volume;
  //  this.module_path = module_path;
  //  this.module_path_value = module_path_value;
  //  this.communicatorMenuId = communicatorMenuId;

  //}

  public id: string;
  public code: string;
  public label: string;
  public screenStatus: number;

  public rotation: string;
  public resolution: string;

  public brightness: number;
  public volume: number;

  public module_path: string;
  public module_path_value: string;

  public communicatorMenuId: string;
}
