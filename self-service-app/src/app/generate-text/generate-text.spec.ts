import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenerateText } from './generate-text';

describe('GenerateText', () => {
  let component: GenerateText;
  let fixture: ComponentFixture<GenerateText>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GenerateText]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GenerateText);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
