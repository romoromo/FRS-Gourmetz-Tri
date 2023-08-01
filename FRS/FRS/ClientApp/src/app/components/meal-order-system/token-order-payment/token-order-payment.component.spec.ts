import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TokenOrderPaymentComponent } from './token-order-payment.component';
 
import { RouterTestingModule } from '@angular/router/testing';
 
 
describe('TokenOrderPaymentComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        TokenOrderPaymentComponent
      ],
    }).compileComponents();
  }));

  it('should create the app', () => {
    const fixture = TestBed.createComponent(TokenOrderPaymentComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have as title 'democlient'`, () => {
    const fixture = TestBed.createComponent(TokenOrderPaymentComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app.title).toEqual('democlient');
  });

  it('should render title in a h1 tag', () => {
    const fixture = TestBed.createComponent(TokenOrderPaymentComponent);
    fixture.detectChanges();
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('h1').textContent).toContain('Welcome to democlient!');
  });
});
