import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of, throwError } from 'rxjs';
import { HostApplicationsListComponent } from './host-applications-list.component';
import { HostApplicationService } from '../../../../services/host-application.service';
import { ApiResponseService } from '../../../../services/api-response.service';
import { map } from 'rxjs/operators';
import { RowStateEnum } from '../../../../models';
import { MatDialog } from '@angular/material/dialog';

class HostSvc {
  getAllApplications() { return of({ resultCode: 0, data: [{ id: 1, name: 'A', appId: 'x', domain: 'd', internalUrl: 'u', isActive: 1 }] }); }
  deleteApplication() { return of({ resultCode: 0, data: {} }); }
  patchIsActive() { return of({ resultCode: 0, data: { id: 1, state: 0 } }); }
}
class ApiResp { handleResponse(obs: any) { return obs.pipe(map((r: any) => r.data)); } }
class DialogStub {
  open() {
    return { afterClosed: () => of(true) } as any;
  }
}

describe('HostApplicationsListComponent CRUD', () => {
  let component: HostApplicationsListComponent;
  let fixture: ComponentFixture<HostApplicationsListComponent>;
  let svc: HostSvc;
  let dialog: DialogStub;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HostApplicationsListComponent],
      providers: [
        { provide: HostApplicationService, useClass: HostSvc },
        { provide: ApiResponseService, useClass: ApiResp },
        { provide: MatDialog, useClass: DialogStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(HostApplicationsListComponent);
    component = fixture.componentInstance;
    svc = TestBed.inject(HostApplicationService) as any;
    dialog = TestBed.inject(MatDialog) as any;
    fixture.detectChanges();
  });

  it('fetches list on init', () => {
    expect(component.loading).toBeFalse();
    expect(component.dataSource.data.length).toBeGreaterThan(0);
  });

  it('applies filter', () => {
    const ev = { target: { value: 'a' } } as any;
    component.applyFilter(ev);
    expect(component.dataSource.filter).toBe('a');
  });

  it('delete flow calls service and refreshes', () => {
    spyOn(dialog, 'open').and.returnValue({ afterClosed: () => of(true) } as any);
    const spyDel = spyOn(svc, 'deleteApplication').and.returnValue(of({ resultCode: 0, data: {} }) as any);
    const spyFetch = spyOn(component, 'fetchHostApplications').and.callThrough();
    component.deleteHostApplication({ id: 1, name: 'A' } as any);
    expect(spyDel).toHaveBeenCalled();
    expect(spyFetch).toHaveBeenCalled();
  });

  it('delete flow error path shows handled error (no refresh)', () => {
    spyOn(dialog, 'open').and.returnValue({ afterClosed: () => of(true) } as any);
    const spyDel = spyOn(svc, 'deleteApplication').and.returnValue(throwError(() => ({ status: 500 })) as any);
    const spyFetch = spyOn(component, 'fetchHostApplications');
    component.deleteHostApplication({ id: 99, name: 'ERR' } as any);
    expect(spyDel).toHaveBeenCalled();
    expect(spyFetch).not.toHaveBeenCalled();
  });

  it('toggle IsActive optimistic update success path', () => {
    const row: any = { id: 1, isActive: 1 };
    spyOn(svc, 'patchIsActive').and.returnValue(of({ resultCode: 0, data: { id: 1, state: 0 } }) as any);
    component.onToggleIsActive(row, false);
    expect(row.isActive).toBe(RowStateEnum.Inactive);
  });

  it('toggle IsActive optimistic update error reverts', () => {
    const row: any = { id: 2, isActive: 1 };
    spyOn(svc, 'patchIsActive').and.returnValue(throwError(() => ({ status: 500 })) as any);
    component.onToggleIsActive(row, false);
    expect(row.isActive).toBe(1);
  });
});

