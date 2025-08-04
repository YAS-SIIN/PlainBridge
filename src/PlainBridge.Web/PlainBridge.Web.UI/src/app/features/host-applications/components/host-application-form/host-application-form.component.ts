import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HostApplicationService } from '../../../../services/host-application.service';
import { HostApplicationDto } from '../../../../models';
import { ToastService } from '../../../../services/toast.service';

@Component({
  selector: 'app-host-application-form',
  standalone: false,
  templateUrl: './host-application-form.component.html',
  styleUrls: ['./host-application-form.component.css']
})
export class HostApplicationFormComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private hostApplicationService: HostApplicationService,
    private toastService: ToastService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required]],
      url: ['', [Validators.required, Validators.pattern('https?://.+')]],
      port: [80, [Validators.required, Validators.min(1)]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.loadHostApplication(Number(id));
    }
  }

  private loadHostApplication(id: number): void {
    this.loading = true;
    this.hostApplicationService.getApplication(id).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          this.form.patchValue(result.data);
        }
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Error loading host application');
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }
    const hostApplication: HostApplicationDto = { ...this.form.value };

    if (this.isEditMode) {
      this.updateHostApplication(hostApplication);
    } else {
      this.createHostApplication(hostApplication);
    }
  }

  private createHostApplication(hostApplication: HostApplicationDto): void {
    this.loading = true;
    this.hostApplicationService.createApplication(hostApplication).subscribe({
      next: () => {
        this.toastService.success('Host application created successfully');
        this.router.navigate(['/host-applications']);
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Error creating host application');
        this.loading = false;
      }
    });
  }

  private updateHostApplication(hostApplication: HostApplicationDto): void {
    this.loading = true;
    const id = this.route.snapshot.paramMap.get('id')!;
    this.hostApplicationService.updateApplication(Number(id), hostApplication).subscribe({
      next: () => {
        this.toastService.success('Host application updated successfully');
        this.router.navigate(['/host-applications']);
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Error updating host application');
        this.loading = false;
      }
    });
  }}
