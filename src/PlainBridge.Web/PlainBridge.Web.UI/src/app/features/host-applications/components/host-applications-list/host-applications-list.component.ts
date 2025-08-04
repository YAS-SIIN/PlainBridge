import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { HostApplicationService } from '../../../../services/host-application.service';
import { HostApplicationDto } from '../../../../models';
import { ToastService } from '../../../../services/toast.service';
import { ConfirmationDialogComponent, ConfirmationDialogData } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-host-applications-list',
  standalone: false,
  templateUrl: './host-applications-list.component.html',
  styleUrls: ['./host-applications-list.component.css']
})
export class HostApplicationsListComponent implements OnInit {
  displayedColumns: string[] = ['name', 'url', 'port', 'isActive', 'actions'];
  dataSource: MatTableDataSource<HostApplicationDto> = new MatTableDataSource();
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
      next: (result) => {
        if (result.isSuccess) {
          this.dataSource.data = result.data;
        }
        this.loading = false;
      },
      error: (error) => {
        this.toastService.error('Error fetching host applications');
        this.loading = false;
      }
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  deleteHostApplication(hostApplication: HostApplicationDto): void {
    const dialogData: ConfirmationDialogData = {
      title: 'Delete Confirmation',
      message: `Are you sure you want to delete host application ${hostApplication.name}?`
    };
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: dialogData
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
