import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EnteramountComponent } from './enteramount.component';

describe('EnteramountComponent', () => {
  let component: EnteramountComponent;
  let fixture: ComponentFixture<EnteramountComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EnteramountComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EnteramountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
