import { Directive, HostListener } from '@angular/core';

@Directive({
  selector: "input[type='number']"
})
export class IgnoreWheelDirective {
  @HostListener('wheel', ['$event'])
  onWheel(event: Event) {
    console.log('Scroll Disabled on Number Field!!!');
    event.preventDefault();
  }

}
