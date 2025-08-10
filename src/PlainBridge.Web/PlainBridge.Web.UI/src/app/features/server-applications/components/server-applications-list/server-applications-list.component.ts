import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { ServerApplicationService } from '../../../../services/server-application.service';
import { ConfirmationDialogComponent, ConfirmationDialogData } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { ServerApplicationDto } from '../../../../models/server-application.models';
import { ApiResponseService } from '../../../../services/api-response.service';
import { RowStateEnum } from '../../../../models';

@Component({
  selector: 'app-server-applications-list',
  standalone: false,
  templateUrl: './server-applications-list.component.html',
  styleUrls: ['./server-applications-list.component.css']
})
export class ServerApplicationsListComponent implements OnInit {
  displayedColumns: string[] = ['appId', 'name', 'serverApplicationAppId', 'internalPort', 'isActive', 'actions'];
  dataSource: MatTableDataSource<ServerApplicationDto> = new MatTableDataSource();
  loading = true;

  @ViewChild(MatPaginator) paginator: MatPaginator | null = null;
  @ViewChild(MatSort) sort: MatSort | null = null;

  constructor(
    private serverApplicationService: ServerApplicationService,
    private dialog: MatDialog,
    private apiResponseService: ApiResponseService
  ) { }

  ngOnInit(): void {
    this.fetchServerApplications();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  fetchServerApplications(): void {
    this.loading = true;
    this.apiResponseService.handleResponse(
      this.serverApplicationService.getAllApplications(),
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

  deleteServerApplication(serverApplication: ServerApplicationDto): void {
    const dialogData: ConfirmationDialogData = {
      title: 'Delete Confirmation',
      message: `Are you sure you want to delete server application "${serverApplication.name}"?`,
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
          this.serverApplicationService.deleteApplication(serverApplication.id),
          { successMessage: 'Deleted', errorMessage: 'Failed to delete status' }
        ).subscribe({
          next: () => {
            this.fetchServerApplications();
          },
          error: () => {
            // Error handling is done by ApiResponseService
          }
        });
      }
    });
  }

  onToggleIsActive(row: ServerApplicationDto, isActive: boolean): void {
    const prev = row.isActive;
    row.isActive = isActive ? RowStateEnum.Active : RowStateEnum.Inactive;

    this.apiResponseService.handleResponse(
      this.serverApplicationService.patchIsActive(row.id, isActive),
      { successMessage: 'Updated', errorMessage: 'Failed to update status' }
    ).subscribe({
      next: (updated) => {
        if (typeof (updated as any).state !== 'undefined') {
          row.isActive = (updated as any).state === 1 ? RowStateEnum.Active : RowStateEnum.Inactive;
        }
      },
      error: () => {
        row.isActive = prev;
      }
    });
  }
}
