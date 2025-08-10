import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of, throwError } from 'rxjs';
import { ServerApplicationsListComponent } from './server-applications-list.component';
import { ServerApplicationService } from '../../../../services/server-application.service';
import { ApiResponseService } from '../../../../services/api-response.service';
import { map } from 'rxjs/operators';
import { RowStateEnum } from '../../../../models';
import { MatDialog } from '@angular/material/dialog';

class ServerSvc {
  getAllApplications() { return of({ resultCode: 0, data: [{ id: 1, name: 'S', appId: 'y', serverApplicationAppId: 'z', internalPort: 8080, isActive: 1 }] }); }
  deleteApplication() { return of({ resultCode: 0, data: {} }); }
  patchIsActive() { return of({ resultCode: 0, data: { id: 1, state: 0 } }); }
}
class ApiResp { handleResponse(obs: any) { return obs.pipe(map((r: any) => r.data)); } }
class DialogStub {
  open() { return { afterClosed: () => of(true) } as any; }
}

describe('ServerApplicationsListComponent CRUD', () => {
  let component: ServerApplicationsListComponent;
  let fixture: ComponentFixture<ServerApplicationsListComponent>;
  let svc: ServerSvc;
  let dialog: DialogStub;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ServerApplicationsListComponent],
      providers: [
        { provide: ServerApplicationService, useClass: ServerSvc },
        { provide: ApiResponseService, useClass: ApiResp },
        { provide: MatDialog, useClass: DialogStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(ServerApplicationsListComponent);
    component = fixture.componentInstance;
    svc = TestBed.inject(ServerApplicationService) as any;
    dialog = TestBed.inject(MatDialog) as any;
    fixture.detectChanges();
  });

  it('fetches list on init', () => {
    expect(component.loading).toBeFalse();
    expect(component.dataSource.data.length).toBeGreaterThan(0);
  });

  it('applies filter', () => {
    const ev = { target: { value: 's' } } as any;
    component.applyFilter(ev);
    expect(component.dataSource.filter).toBe('s');
  });

  it('delete flow calls service and refreshes', () => {
    spyOn(dialog, 'open').and.returnValue({ afterClosed: () => of(true) } as any);
    const spyDel = spyOn(svc, 'deleteApplication').and.returnValue(of({ resultCode: 0, data: {} }) as any);
    const spyFetch = spyOn(component, 'fetchServerApplications').and.callThrough();
    component.deleteServerApplication({ id: 1, name: 'S' } as any);
    expect(spyDel).toHaveBeenCalled();
    expect(spyFetch).toHaveBeenCalled();
  });

  it('delete flow error path shows handled error (no refresh)', () => {
    spyOn(dialog, 'open').and.returnValue({ afterClosed: () => of(true) } as any);
    const spyDel = spyOn(svc, 'deleteApplication').and.returnValue(throwError(() => ({ status: 500 })) as any);
    const spyFetch = spyOn(component, 'fetchServerApplications');
    component.deleteServerApplication({ id: 99, name: 'ERR' } as any);
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

