import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { HostApplicationsListComponent } from './host-applications-list.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { of, throwError } from 'rxjs';
import { HostApplicationService } from '../../../../services/host-application.service';
import { ApiResponseService } from '../../../../services/api-response.service';
import { MatDialog } from '@angular/material/dialog';
import { RowStateEnum } from '../../../../models';

class HostServiceStub {
  getAllApplications() { return of({ resultCode: 0, data: [] }); }
  deleteApplication() { return of({ resultCode: 0, data: {} }); }
  patchIsActive() { return of({ resultCode: 0, data: {} }); }
}
class ApiResponseServiceStub { handleResponse(obs: any) { return obs.pipe((x: any) => x); } }
class DialogStub {
  open() {
    return { afterClosed: () => of(true) } as any;
  }
}
describe('HostApplicationsListComponent', () => {
  let component: HostApplicationsListComponent;
  let fixture: ComponentFixture<HostApplicationsListComponent>;
  let service: HostServiceStub;
  let dialog: DialogStub;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatTableModule, MatPaginatorModule, MatSortModule],
      declarations: [HostApplicationsListComponent],
      providers: [
        { provide: HostApplicationService, useClass: HostServiceStub },
        { provide: ApiResponseService, useClass: ApiResponseServiceStub },
        { provide: MatDialog, useClass: DialogStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(HostApplicationsListComponent);
    component = fixture.componentInstance;
    service = TestBed.inject(HostApplicationService) as any;
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
    const spyDel = spyOn(service, 'deleteApplication').and.returnValue(of({ resultCode: 0, data: {} }) as any);
    const spyFetch = spyOn(component, 'fetchHostApplications').and.callThrough();
    component.deleteHostApplication({ id: 1, name: 'A' } as any);
    expect(spyDel).toHaveBeenCalled();
    expect(spyFetch).toHaveBeenCalled();
  });

  it('delete flow error path shows handled error (no refresh)', () => {
    spyOn(dialog, 'open').and.returnValue({ afterClosed: () => of(true) } as any);
    const spyDel = spyOn(service, 'deleteApplication').and.returnValue(throwError(() => ({ status: 500 })) as any);
    const spyFetch = spyOn(component, 'fetchHostApplications');
    component.deleteHostApplication({ id: 99, name: 'ERR' } as any);
    expect(spyDel).toHaveBeenCalled();
    expect(spyFetch).not.toHaveBeenCalled();
  });

  it('optimistic toggle and revert on error', () => {
    const row: any = { id: 1, isActive: 0 };
    spyOn(service, 'patchIsActive').and.returnValue(throwError(() => ({ status: 500 })) as any);
    component.onToggleIsActive(row, true);
    expect(row.isActive).toBe(0);
  });
 
  it('toggle IsActive optimistic update success path', () => {
    const row: any = { id: 1, isActive: 1 };
    spyOn(service, 'patchIsActive').and.returnValue(of({ resultCode: 0, data: { id: 1, state: 0 } }) as any);
    component.onToggleIsActive(row, false);
    expect(row.isActive).toBe(RowStateEnum.Inactive);
  });

  it('toggle IsActive optimistic update error reverts', () => {
    const row: any = { id: 2, isActive: 1 };
    spyOn(service, 'patchIsActive').and.returnValue(throwError(() => ({ status: 500 })) as any);
    component.onToggleIsActive(row, false);
    expect(row.isActive).toBe(1);
  });
});

