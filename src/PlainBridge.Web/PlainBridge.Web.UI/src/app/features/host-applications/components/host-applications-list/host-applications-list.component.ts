import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { ServerApplicationService } from '../../../../services/server-application.service'; 
import { ToastService } from '../../../../services/toast.service';
import { ConfirmationDialogComponent, ConfirmationDialogData } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { ServerApplicationDto } from '../../../../models/server-application.models';
import { HostApplicationService } from '../../../../services/host-application.service';

@Component({
  selector: 'app-host-applications-list',
  standalone: false,
  templateUrl: './host-applications-list.component.html',
  styleUrls: ['./host-applications-list.component.css']
})
export class HostApplicationsListComponent implements OnInit {
  displayedColumns: string[] = ['name', 'url', 'port', 'isActive',  'actions'];
  dataSource: MatTableDataSource<ServerApplicationDto> = new MatTableDataSource();
  loading = true;

  @ViewChild(MatPaginator) paginator: MatPaginator | null = null;
  @ViewChild(MatSort) sort: MatSort | null = null;

  constructor(
    private hostApplicationService: HostApplicationService,
    private toastService: ToastService,
    private dialog: MatDialog
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
    this.hostApplicationService.getAllApplications().subscribe({
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

  deleteHostApplication(hostApplication: ServerApplicationDto): void {
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
        this.hostApplicationService.deleteApplication(hostApplication.id).subscribe({
          next: () => {
            this.toastService.success('Host application deleted successfully');
            this.fetchHostApplications();
          },
          error: () => {
            this.toastService.error('Error deleting host application');
          }
        });
      }
    });
  }
}
