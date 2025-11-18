import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Usertable } from './usertable';

describe('Usertable', () => {
  let component: Usertable;
  let fixture: ComponentFixture<Usertable>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Usertable]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Usertable);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
