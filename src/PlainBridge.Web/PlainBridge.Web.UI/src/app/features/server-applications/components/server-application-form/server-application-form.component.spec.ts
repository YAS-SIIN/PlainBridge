import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ServerApplicationFormComponent } from './server-application-form.component';
import { ServerApplicationService } from '../../../../services/server-application.service';
import { ApiResponseService } from '../../../../services/api-response.service';
import { ServerApplicationTypeEnum } from '../../../../models/server-application.models';

class ServerServiceStub {
  createApplication() { return of({ resultCode: 0, data: {} }); }
  updateApplication() { return of({ resultCode: 0, data: {} }); }
  getApplication() { return of({ resultCode: 0, data: { id: 1, name: 'S', description: 'd', internalPort: 80, serverApplicationType: ServerApplicationTypeEnum.SharePort } }); }
}
class ApiResponseStub { handleResponse(obs: any) { return obs; } }

describe('ServerApplicationFormComponent', () => {
  let component: ServerApplicationFormComponent;
  let fixture: ComponentFixture<ServerApplicationFormComponent>;
  let svc: ServerServiceStub;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, RouterTestingModule],
      declarations: [ServerApplicationFormComponent],
      providers: [
        { provide: ServerApplicationService, useClass: ServerServiceStub },
        { provide: ApiResponseService, useClass: ApiResponseStub },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: (k: string) => '1' } } } }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    TestBed.overrideTemplate(ServerApplicationFormComponent, '<form [formGroup]="form"></form>');
    fixture = TestBed.createComponent(ServerApplicationFormComponent);
    component = fixture.componentInstance;
    svc = TestBed.inject(ServerApplicationService) as any;
    fixture.detectChanges();
  });

  it('invalid form blocks submit', () => {
    component.pageMode = 'new';
    component.form.setValue({ name: '', description: '', internalPort: '', serverApplicationType: ServerApplicationTypeEnum.SharePort });
    component.onSubmit();
    expect(component.form.invalid).toBeTrue();
  });

  it('create path calls service', () => {
    spyOn(svc, 'createApplication').and.returnValue(of({ resultCode: 0, data: {} }));
    component.pageMode = 'new';
    component.form.setValue({ name: 'svc', description: '', internalPort: 1234, serverApplicationType: ServerApplicationTypeEnum.UsePort });
    component.onSubmit();
    expect(svc.createApplication).toHaveBeenCalled();
  });

  it('update path calls service', () => {
    spyOn(svc, 'updateApplication').and.returnValue(of({ resultCode: 0, data: {} }));
    component.pageMode = 'edit';
    component.form.setValue({ name: 'svc', description: '', internalPort: 1234, serverApplicationType: ServerApplicationTypeEnum.UsePort });
    component.onSubmit();
    expect(svc.updateApplication).toHaveBeenCalled();
  });
  it('create path error keeps loading false', () => {
    spyOn(svc, 'createApplication').and.returnValue(throwError(() => ({ status: 400 })) as any);
    component.pageMode = 'new';
    component.form.setValue({ name: 'svc', description: '', internalPort: 1234, serverApplicationType: ServerApplicationTypeEnum.UsePort });
    component.onSubmit();
    expect(component.loading).toBeFalse();
  });

  it('update path error keeps loading false', () => {
    spyOn(svc, 'updateApplication').and.returnValue(throwError(() => ({ status: 400 })) as any);
    component.pageMode = 'edit';
    component.form.setValue({ name: 'svc', description: '', internalPort: 1234, serverApplicationType: ServerApplicationTypeEnum.UsePort });
    component.onSubmit();
    expect(component.loading).toBeFalse();
  });
});

