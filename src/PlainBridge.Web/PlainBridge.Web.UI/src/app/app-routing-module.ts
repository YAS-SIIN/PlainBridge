import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { LayoutComponent } from './components/layout/layout.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';

const routes: Routes = [
  {
    path: 'sign-in-oidc',
    loadComponent: () => import('./features/sign-in-oidc/sign-in-oidc.component').then(m => m.SignInOidcComponent)
  },
  {
    path: 'register',
    loadChildren: () => import('./features/users/users.module').then(m => m.UsersModule)
  },
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: '',
        redirectTo: '/dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'host-applications',
        loadChildren: () => import('./features/host-applications/host-applications.module').then(m => m.HostApplicationsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'server-applications',
        loadChildren: () => import('./features/server-applications/server-applications.module').then(m => m.ServerApplicationsModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'profile',
        loadChildren: () => import('./features/profile/profile.module').then(m => m.ProfileModule),
        canActivate: [AuthGuard]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
