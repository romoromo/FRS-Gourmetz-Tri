// tslint:disable: directive-selector
import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[componentTypeHost]',
})
export class SignageComponentTypeDirective {
  constructor(public viewContainerRef: ViewContainerRef) { }
}
