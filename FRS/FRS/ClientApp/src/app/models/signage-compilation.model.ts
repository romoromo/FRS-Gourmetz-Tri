import { Permission } from './permission.model';
import { SignageComponent } from './signage-component.model';


export class SignageCompilation {

  public id: string;
  public name: string;
  public description: string;

  public width: number;
  public height: number;

  public backgroundColor: string;
  public backgroundImage: string;


  compilationId: string;

  components: SignageComponent[];

  isActive: any;

  interval: number;
}
