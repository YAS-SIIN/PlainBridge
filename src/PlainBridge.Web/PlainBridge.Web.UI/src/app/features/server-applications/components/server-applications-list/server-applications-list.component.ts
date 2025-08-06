import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { ServerApplicationService } from '../../../../services/server-application.service'; 
import { ToastService } from '../../../../services/toast.service';
import { ConfirmationDialogComponent, ConfirmationDialogData } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { ServerApplicationDto } from '../../../../models/server-application.models';

@Component({
  selector: 'app-server-applications-list',
  standalone: false,
  templateUrl: './server-applications-list.component.html',
  styleUrls: ['./server-applications-list.component.css']
})
export class ServerApplicationsListComponent implements OnInit {
  displayedColumns: string[] = [ 'appId', 'serverApplicationAppId', 'name', 'internalPort', 'isActive', 'actions'];
  dataSource: MatTableDataSource<ServerApplicationDto> = new MatTableDataSource();
  loading = true;

  @ViewChild(MatPaginator) paginator: MatPaginator | null = null;
  @ViewChild(MatSort) sort: MatSort | null = null;

  constructor(
    private serverApplicationService: ServerApplicationService,
    private toastService: ToastService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.fetchServerApplications();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  fetchServerApplications(): void {
    this.loading = true;
    this.serverApplicationService.getAllApplications().subscribe({
      next: (result: any) => {
        if (result.isSuccess) {
          this.dataSource.data = result.data;
        }
        this.loading = false;
      },
      error: (error: any) => {
        this.toastService.error('Error fetching server applications');
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
        this.serverApplicationService.deleteApplication(serverApplication.id).subscribe({
          next: () => {
            this.toastService.success('Server application deleted successfully');
            this.fetchServerApplications();
          },
          error: () => {
            this.toastService.error('Error deleting server application');
          }
        });
      }
    });
  }
}
