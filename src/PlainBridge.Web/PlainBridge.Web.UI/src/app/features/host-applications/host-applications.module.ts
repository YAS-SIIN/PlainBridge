import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

// Angular Material imports
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatGridListModule } from '@angular/material/grid-list'; 

import { HostApplicationsRoutingModule } from './host-applications-routing.module';
import { HostApplicationsListComponent } from './components/host-applications-list/host-applications-list.component';
import { HostApplicationFormComponent } from './components/host-application-form/host-application-form.component';

@NgModule({
  declarations: [
    HostApplicationsListComponent,
    HostApplicationFormComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HostApplicationsRoutingModule,
    // Material modules
    MatExpansionModule,    
    MatGridListModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatCheckboxModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule
  ]
})
export class HostApplicationsModule { }
