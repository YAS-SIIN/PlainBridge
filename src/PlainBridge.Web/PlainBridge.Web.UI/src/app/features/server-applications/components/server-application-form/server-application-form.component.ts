import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ServerApplicationService } from '../../../../services/server-application.service';
import { ServerApplicationDto, ServerApplicationTypeEnum } from '../../../../models/server-application.models';
import { ToastService } from '../../../../services/toast.service';
import { ApiResponseService } from '../../../../services/api-response.service';

@Component({
  selector: 'app-server-application-form',
  standalone: false,
  templateUrl: './server-application-form.component.html',
  styleUrls: ['./server-application-form.component.css']
})
export class ServerApplicationFormComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;
  loading = false;
  serverApplicationTypes = [
    { value: ServerApplicationTypeEnum.SharePort, label: 'Share Port' },
    { value: ServerApplicationTypeEnum.UsePort, label: 'Use Port' }
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private serverApplicationService: ServerApplicationService,
    private apiResponseService: ApiResponseService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      serverApplicationAppId: [''],
      internalPort: ['', [Validators.required, Validators.min(1)]],
      serverApplicationType: [ServerApplicationTypeEnum.SharePort, Validators.required]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.loadServerApplication(Number(id));
    }
  }

  private loadServerApplication(id: number): void {
    this.loading = true;
    this.apiResponseService.handleResponse(
      this.serverApplicationService.getApplication(id),
      { showSuccessToast: false }
    ).subscribe({
      next: (result) => {
        if (result) {
          this.form.patchValue(result);
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.markFormGroupTouched();
      return;
    }

    const serverApplication: ServerApplicationDto = { ...this.form.value };

    if (this.isEditMode) {
      this.updateServerApplication(serverApplication);
    } else {
      this.createServerApplication(serverApplication);
    }
  }

  private createServerApplication(serverApplication: ServerApplicationDto): void {
    this.loading = true;
    this.apiResponseService.handleResponse(
      this.serverApplicationService.createApplication(serverApplication)
    ).subscribe({
      next: () => {
        this.router.navigate(['/server-applications']);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  private updateServerApplication(serverApplication: ServerApplicationDto): void {
    this.loading = true;
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.apiResponseService.handleResponse(
      this.serverApplicationService.updateApplication(id, serverApplication)
    ).subscribe({
      next: () => {
        this.router.navigate(['/server-applications']);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.form.controls).forEach(field => {
      const control = this.form.get(field);
      control?.markAsTouched({ onlySelf: true });
    });
  }

  onCancel(): void {
    this.router.navigate(['/server-applications']);
  }

  // Getters for easy access to form controls in template
  get appId() { return this.form.get('appId'); }
  get serverApplicationAppId() { return this.form.get('serverApplicationAppId'); }
  get name() { return this.form.get('name'); }
  get description() { return this.form.get('description'); }
  get internalPort() { return this.form.get('internalPort'); }
  get serverApplicationType() { return this.form.get('serverApplicationType'); }
  get isActive() { return this.form.get('isActive'); }
  get userId() { return this.form.get('userId'); }
  get userName() { return this.form.get('userName'); }
}
