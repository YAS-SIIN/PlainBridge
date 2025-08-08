import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router'; 
import { UserRegistrationComponent } from './components/user-registration/user-registration.component';

const routes: Routes = [ 
  {
    path: '',
    component: UserRegistrationComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule { }
