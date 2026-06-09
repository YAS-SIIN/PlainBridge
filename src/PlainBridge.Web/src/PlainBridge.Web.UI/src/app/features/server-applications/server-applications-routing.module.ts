import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ServerApplicationsListComponent } from './components/server-applications-list/server-applications-list.component';
import { ServerApplicationFormComponent } from './components/server-application-form/server-application-form.component';

const routes: Routes = [
  {
    path: '',
    component: ServerApplicationsListComponent
  },
  {
    path: 'new',
    component: ServerApplicationFormComponent
  },
  {
    path: ':id/edit',
    component: ServerApplicationFormComponent
  },
  {
    path: ':id/detail',
    component: ServerApplicationFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServerApplicationsRoutingModule { }
