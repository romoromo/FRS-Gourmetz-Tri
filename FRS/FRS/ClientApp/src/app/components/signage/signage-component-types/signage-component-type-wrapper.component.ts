import { Component, Input, OnInit, ViewChild, ComponentFactoryResolver, OnDestroy, Type, ComponentRef, EventEmitter, Output } from '@angular/core';

import { SignageComponentTypeDirective } from '../../../directives/signage-component-type.directive';
import { SignageComponentType } from '../../../models/signage-component-type.model';

@Component({
  selector: 'signage-component-type-wrapper',
  template: `<ng-template componentTypeHost></ng-template>`
})
export class SignageComponentTypeWrapperComponent implements OnInit, OnDestroy {
  componentRef: ComponentRef<any>;

  @Input() type: Type<any>;
  @Input() configurations: any;
  @Input() preview: boolean;
  @ViewChild(SignageComponentTypeDirective) componentTypeHost: SignageComponentTypeDirective;

  constructor(private componentFactoryResolver: ComponentFactoryResolver) { }

  ngOnInit() {
    this.loadComponent(this.type, this.configurations, this.preview);
  }

  ngOnDestroy() {
  }

  loadComponent(type: Type<any>, configurations: any, preview?: boolean) {
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(type);

    const viewContainerRef = this.componentTypeHost.viewContainerRef;
    viewContainerRef.clear();

    this.componentRef = viewContainerRef.createComponent(componentFactory);
    this.componentRef.instance.configurations = configurations || {};
    this.componentRef.instance.preview = preview;
  }

  getConfigurations() {
    if (this.componentRef) return this.componentRef.instance.getConfigurations();
  }
}


/*
Copyright Google LLC. All Rights Reserved.
Use of this source code is governed by an MIT-style license that
can be found in the LICENSE file at https://angular.io/license
*/
