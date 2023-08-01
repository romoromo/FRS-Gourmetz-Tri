import { Permission } from './permission.model';
import { SignageComponentType } from './signage-component-type.model';


export class SignageComponent {

  public id: string;
  public name: string;
  public description: string;

  public fixWidth: number;
  public fixHeight: number;
  public isRatio: boolean;

  public backgroundColor: string;
  public backgroundImage: string;

  public componentType: string;

  public configurations: string;

  public previewWidth: number;
  public previewHeight: number;

  componentTypeObj: SignageComponentType;
    prevComponentType: string;
    configurationsObj: any;
    x: number;
    y: number;
    width: number;
    height: number;
    order: number;
  componentId: string;

  isActive: any;
}
