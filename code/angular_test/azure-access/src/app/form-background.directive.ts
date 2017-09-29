import {
  Directive,
  Renderer,
  ElementRef
} from '@angular/core';

@Directive({
  selector: '[appFormBackground]'
})
export class FormBackgroundDirective {
  constructor(private el: ElementRef, private renderer: Renderer) {
    renderer.setElementStyle(el.nativeElement, 'backgroundColor', 'lightgrey');
  }
}
