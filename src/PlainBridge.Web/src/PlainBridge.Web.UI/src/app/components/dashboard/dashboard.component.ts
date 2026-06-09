import { Component, OnInit } from '@angular/core';
import { HostApplicationService } from '../../services/host-application.service';
import { ServerApplicationService } from '../../services/server-application.service';


@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  hostApplicationsCount = 0;
  serverApplicationsCount = 0;
  usersCount = 0;
  loading = false;

  constructor(
    private hostApplicationService: HostApplicationService,
    private serverApplicationService: ServerApplicationService
  ) { }

  ngOnInit(): void {
    this.loadDashboardData();
  }

  private loadDashboardData() {
    // this.loading = true;

    // Load host applications count
    this.hostApplicationService.getAllApplications().subscribe({
      next: (result) => { 
        if (result.resultCode === 0) {
          this.hostApplicationsCount = result.data.length;
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading host applications:', error);
        this.loading = false;
      }
    });

    // Load server applications count
    this.serverApplicationService.getAllApplications().subscribe({
      next: (result) => {
        if (result.resultCode === 0) {
          this.serverApplicationsCount = result.data.length;
        }
        this.loading = false;
      },
      error: (error) => {
        debugger;
        console.error('Error loading server applications:', error);
        this.loading = false;
      }
    });

  }
}
