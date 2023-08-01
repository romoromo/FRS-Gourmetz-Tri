import { ServiceContractAsset } from "./asset.model";

export class ServiceContract {

  constructor() {
  }

  public id: string;
  public identifier: string;
  public reference: string;
  public coverage: string;
  public details: string;
  public renewalAlert: number;
  public startDate?: Date;
  public endDate?: Date;
  public serviceContractAssets: ServiceContractAsset[];
}
