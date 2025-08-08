import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { ToastService } from '../../../../services/toast.service';
import { ConfirmationDialogComponent, ConfirmationDialogData } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { HostApplicationService } from '../../../../services/host-application.service';
import { ApiResponseService } from '../../../../services/api-response.service';
import { HostApplicationDto, RowStateEnum } from '../../../../models';

@Component({
  selector: 'app-host-applications-list',
  standalone: false,
  templateUrl: './host-applications-list.component.html',
  styleUrls: ['./host-applications-list.component.css']
})
export class HostApplicationsListComponent implements OnInit {
  displayedColumns: string[] = ['name', 'url', 'port', 'isActive',  'actions'];
  dataSource: MatTableDataSource<HostApplicationDto> = new MatTableDataSource();
  loading = true;
  pendingIds = new Set<number>();

  @ViewChild(MatPaginator) paginator: MatPaginator | null = null;
  @ViewChild(MatSort) sort: MatSort | null = null;

  constructor(
    private hostApplicationService: HostApplicationService,
    private dialog: MatDialog,
    private apiResponseService: ApiResponseService
  ) {}

  ngOnInit(): void {
    this.fetchHostApplications();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  fetchHostApplications(): void {
    this.loading = true;
    this.apiResponseService.handleResponse(
      this.hostApplicationService.getAllApplications(),
      { showSuccessToast: false } // Don't show toast for loading data
    ).subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  deleteHostApplication(hostApplication: HostApplicationDto): void {
    const dialogData: ConfirmationDialogData = {
      title: 'Delete Confirmation',
      message: `Are you sure you want to delete host application "${hostApplication.name}"?`,
      confirmText: 'Delete',
      cancelText: 'Cancel'
    };

    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: dialogData,
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiResponseService.handleResponse(
          this.hostApplicationService.deleteApplication(hostApplication.id)
        ).subscribe({
          next: () => {
            this.fetchHostApplications();
          },
          error: () => {
            // Error handling is done by ApiResponseService
          }
        });
      }
    });
  }

  onToggleIsActive(row: HostApplicationDto, isActive: boolean): void {
    // optimistic update
    const prev = row.isActive;
      row.isActive = isActive ? RowStateEnum.Active : RowStateEnum.Inactive;
    this.pendingIds.add(row.id);

    this.apiResponseService.handleResponse(
      this.hostApplicationService.patchIsActive(row.id, isActive),
      { successMessage: 'Updated', errorMessage: 'Failed to update status' }
    ).subscribe({
      next: (updated) => {
        // If backend returns state instead of isActive, normalize
        if (typeof (updated as any).state !== 'undefined') {
           row.isActive = (updated as any).state === 1 ? RowStateEnum.Active : RowStateEnum.Inactive;
        }
        this.pendingIds.delete(row.id);
      },
      error: () => {
        // revert on error
        row.isActive = prev;
        this.pendingIds.delete(row.id);
      }
    });
  }
}
