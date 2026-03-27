import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventCreateTypeComponent } from './event-create-type.component';

describe('EventCreateTypeComponent', () => {
  let component: EventCreateTypeComponent;
  let fixture: ComponentFixture<EventCreateTypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventCreateTypeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EventCreateTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
