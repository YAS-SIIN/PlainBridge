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
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';

import { ServerApplicationsRoutingModule } from './server-applications-routing.module';
import { ServerApplicationsListComponent } from './components/server-applications-list/server-applications-list.component';
import { ServerApplicationFormComponent } from './components/server-application-form/server-application-form.component';

@NgModule({
  declarations: [
    ServerApplicationsListComponent,
    ServerApplicationFormComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ServerApplicationsRoutingModule,
    
    // Material modules
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
    MatProgressSpinnerModule,
    MatSelectModule,
    MatTooltipModule
  ]
})
export class ServerApplicationsModule { }
