import { Component, ViewChild, ElementRef, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'search-box',
  templateUrl: './search-box.component.html',
  styleUrls: ['./search-box.component.css']
})
export class SearchBoxComponent {

  @Input()
  placeholder: string = "Search...";

  @Output()
  onEnter = new EventEmitter<string>();

  @Output()
  searchChange = new EventEmitter<string>();

  @ViewChild("searchInput")
  searchInput: ElementRef;


  onValueChange(value: string) {
    setTimeout(() => this.searchChange.emit(value));
  }


  clear() {
    this.searchInput.nativeElement.value = '';
    this.onValueChange(this.searchInput.nativeElement.value);
    this.onEnterPress();
  }

  onEnterPress() {
    setTimeout(() => this.onEnter.emit());
  }
}
