import { async, ComponentFixture, TestBed } from '@angular/core/testing';
 
import { TokenOrderPaymentThankyouComponent } from './token-order-payment-thankyou.component';

describe('TokenOrderPaymentThankyouComponent', () => {
  let component: TokenOrderPaymentThankyouComponent;
  let fixture: ComponentFixture<TokenOrderPaymentThankyouComponent>;
    
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TokenOrderPaymentThankyouComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TokenOrderPaymentThankyouComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
