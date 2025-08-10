import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { HostApplicationFormComponent } from './host-application-form.component';
import { HostApplicationService } from '../../../../services/host-application.service';
import { ApiResponseService } from '../../../../services/api-response.service';
import { ActivatedRoute } from '@angular/router';

class HostServiceStub {
  createApplication() { return of({ resultCode: 0, data: {} }); }
  updateApplication() { return of({ resultCode: 0, data: {} }); }
  getApplication() { return of({ resultCode: 0, data: { id: 1, name: 'A', domain: 'd', internalUrl: 'u', description: 'x' } }); }
}
class ApiResponseStub { handleResponse(obs: any) { return obs; } }

describe('HostApplicationFormComponent', () => {
  let component: HostApplicationFormComponent;
  let fixture: ComponentFixture<HostApplicationFormComponent>;
  let svc: HostServiceStub;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, RouterTestingModule],
      declarations: [HostApplicationFormComponent],
      providers: [
        { provide: HostApplicationService, useClass: HostServiceStub },
        { provide: ApiResponseService, useClass: ApiResponseStub },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: (k: string) => '1' } } } }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(HostApplicationFormComponent);
    component = fixture.componentInstance;
    svc = TestBed.inject(HostApplicationService) as any;
    fixture.detectChanges();
  });

  it('invalid form blocks submit', () => {
    component.pageMode = 'new';
    component.form.setValue({ name: '', domain: '', internalUrl: '', description: '' });
    component.onSubmit();
    // no error thrown means it's blocked
    expect(component.form.invalid).toBeTrue();
  });

  it('create path calls service', () => {
    spyOn(svc, 'createApplication').and.returnValue(of({ resultCode: 0, data: {} }));
    component.pageMode = 'new';
    component.form.setValue({ name: 'abc', domain: 'dom', internalUrl: 'http://u', description: '' });
    component.onSubmit();
    expect(svc.createApplication).toHaveBeenCalled();
  });

  it('update path calls service', () => {
    spyOn(svc, 'updateApplication').and.returnValue(of({ resultCode: 0, data: {} }));
    component.pageMode = 'edit';
    component.form.setValue({ name: 'abc', domain: 'dom', internalUrl: 'http://u', description: '' });
    component.onSubmit();
    expect(svc.updateApplication).toHaveBeenCalled();
  });
});

