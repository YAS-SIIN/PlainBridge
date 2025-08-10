import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HostApplicationService } from '../../../../services/host-application.service';
import { HostApplicationDto } from '../../../../models';
import { ToastService } from '../../../../services/toast.service';
import { ApiResponseService } from '../../../../services/api-response.service';

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
  isDetailMode = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private hostApplicationService: HostApplicationService,
    private apiResponseService: ApiResponseService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
      domain: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
      internalUrl: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['']
    });
  }

  ngOnInit(): void {
    let pageMode = this.router.url.includes('/detail') ? 'view' : 'edit';
    if (pageMode === 'view') {
      this.isDetailMode = true;
      this.form.disable();
    } else if (pageMode === 'edit') {
      this.isEditMode = true;
    }

    const id = this.route.snapshot.paramMap.get('id');
    if (id) { 
      this.loadHostApplication(Number(id));
    }
  }

  private loadHostApplication(id: number): void {
    this.loading = true;
    this.apiResponseService.handleResponse(
      this.hostApplicationService.getApplication(id),
      { showSuccessToast: false }
    ).subscribe({
      next: (data) => {
        this.form.patchValue(data);
        this.loading = false;
        // Ensure form stays disabled in detail mode after patchValue
        if (this.isDetailMode) {
          this.form.disable();
        }
      },
      error: () => {
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
    this.apiResponseService.handleResponse(
      this.hostApplicationService.createApplication(hostApplication)
    ).subscribe({
      next: () => {
        this.router.navigate(['/host-applications']);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  private updateHostApplication(hostApplication: HostApplicationDto): void {
    this.loading = true;
    const id = this.route.snapshot.paramMap.get('id')!;
    this.apiResponseService.handleResponse(
      this.hostApplicationService.updateApplication(Number(id), hostApplication)
    ).subscribe({
      next: () => {
        this.router.navigate(['/host-applications']);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
