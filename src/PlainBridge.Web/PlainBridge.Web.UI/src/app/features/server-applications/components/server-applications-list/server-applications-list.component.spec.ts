import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ServerApplicationsListComponent } from './server-applications-list.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { of, throwError } from 'rxjs';
import { ServerApplicationService } from '../../../../services/server-application.service';
import { ApiResponseService } from '../../../../services/api-response.service';

class ServerServiceStub { 
  getAllApplications() { return of({ resultCode: 0, data: [] }); }
  deleteApplication() { return of({ resultCode: 0, data: {} }); }
  patchIsActive() { return of({ resultCode: 0, data: {} }); }
}
class ApiResponseServiceStub { handleResponse(obs: any) { return obs.pipe((x: any) => x); } }

describe('ServerApplicationsListComponent', () => {
  let component: ServerApplicationsListComponent;
  let fixture: ComponentFixture<ServerApplicationsListComponent>;
  let service: ServerServiceStub;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatTableModule, MatPaginatorModule, MatSortModule],
      declarations: [ServerApplicationsListComponent],
      providers: [
        { provide: ServerApplicationService, useClass: ServerServiceStub },
        { provide: ApiResponseService, useClass: ApiResponseServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(ServerApplicationsListComponent);
    component = fixture.componentInstance;
    service = TestBed.inject(ServerApplicationService) as any;
    fixture.detectChanges();
  });

  it('loads data on init', () => {
    expect(component.loading).toBeFalse();
  });

  it('optimistic toggle and revert on error', () => {
    const row: any = { id: 1, isActive: 0 };
    spyOn(service, 'patchIsActive').and.returnValue(throwError(() => ({ status: 500 })) as any);
    component.onToggleIsActive(row, true);
    expect(row.isActive).toBe(0);
  });
});

