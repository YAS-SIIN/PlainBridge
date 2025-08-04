import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ServerApplicationService } from '../../../../services/server-application.service';
import { ServerApplicationDto } from '../../../../models/server-application.models';
import { ToastService } from '../../../../services/toast.service';

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

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private serverApplicationService: ServerApplicationService,
    private toastService: ToastService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      path: ['', [Validators.required]],
      isActive: [true],
      hostApplicationId: ['', [Validators.required]]
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
    this.serverApplicationService.getApplication(id).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          this.form.patchValue(result.data);
        }
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Error loading server application');
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
    this.serverApplicationService.createApplication(serverApplication).subscribe({
      next: () => {
        this.toastService.success('Server application created successfully');
        this.router.navigate(['/server-applications']);
      },
      error: () => {
        this.toastService.error('Error creating server application');
        this.loading = false;
      }
    });
  }

  private updateServerApplication(serverApplication: ServerApplicationDto): void {
    this.loading = true;
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.serverApplicationService.updateApplication(id, serverApplication).subscribe({
      next: () => {
        this.toastService.success('Server application updated successfully');
        this.router.navigate(['/server-applications']);
      },
      error: () => {
        this.toastService.error('Error updating server application');
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
  get name() { return this.form.get('name'); }
  get description() { return this.form.get('description'); }
  get path() { return this.form.get('path'); }
  get isActive() { return this.form.get('isActive'); }
  get hostApplicationId() { return this.form.get('hostApplicationId'); }
}
