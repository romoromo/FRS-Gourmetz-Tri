import { Type } from '@angular/core';
export class SignageComponentType {

  constructor(
    id: string,
    component: Type<any>,
    name: string,
    configuration: Type<any>) {
    this.id = id;
    this.component = component;
    this.name = name;
    this.configuration = configuration;
  }

  public id: string;
  public component: Type<any>;
  public name: string;
  public configuration: Type<any>;
}
