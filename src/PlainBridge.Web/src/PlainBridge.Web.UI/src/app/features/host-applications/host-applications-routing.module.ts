import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HostApplicationsListComponent } from './components/host-applications-list/host-applications-list.component';
import { HostApplicationFormComponent } from './components/host-application-form/host-application-form.component';

const routes: Routes = [
  {
    path: '',
    component: HostApplicationsListComponent
  },
  {
    path: 'new',
    component: HostApplicationFormComponent
  },
  {
    path: ':id/edit',
    component: HostApplicationFormComponent
  },
  {
    path: ':id/detail',
    component: HostApplicationFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HostApplicationsRoutingModule { }
