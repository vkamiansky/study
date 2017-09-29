import {
  Directive,
  HostBinding,
  HostListener,
  ElementRef,
  Renderer
} from '@angular/core';

@Directive({
  selector: '[appHoverShow]'
})
export class HoverShowDirective {
  @HostBinding('class.card-outline-primary') private ishovering: boolean;

  constructor(private el: ElementRef, private renderer: Renderer)
  { }

  @HostListener('mouseover') onMouseOver() {
    let part = this.el.nativeElement.querySelector('.text');
    this.renderer.setElementStyle(part, 'display', 'block');
    this.ishovering = true;
  }
  
  @HostListener('mouseout') onMouseOut() {
    let part = this.el.nativeElement.querySelector('.text');
    this.renderer.setElementStyle(part, 'display', 'none');
    this.ishovering = false;
  }
}
