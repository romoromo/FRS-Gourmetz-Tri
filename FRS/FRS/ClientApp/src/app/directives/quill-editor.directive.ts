import { Directive, ElementRef, AfterViewInit, EventEmitter, Output } from '@angular/core';
import * as Quill from 'quill';

@Directive({
  selector: '[quillEditor]'
})
export class QuillEditorDirective implements AfterViewInit {
  @Output() contentChange = new EventEmitter<string>();

  constructor(private elementRef: ElementRef) { }

  ngAfterViewInit() {
    const editorElement = this.elementRef.nativeElement;
    const quill = new Quill(editorElement, {
      theme: 'snow'
      // You can add more options here as per your requirements
    });
  }
}
