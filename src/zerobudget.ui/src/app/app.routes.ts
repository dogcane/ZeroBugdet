import { Routes } from '@angular/router';
import { InitialCheckComponent } from './components/auth/initial-check.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterMainUserComponent } from './components/auth/register-main-user/register-main-user.component';
import { MainLayoutComponent } from './components/layout/main-layout/main-layout.component';
import { BucketListComponent } from './components/buckets/bucket-list/bucket-list.component';
import { SpendingListComponent } from './components/spendings/spending-list/spending-list.component';
import { SpendingFormComponent } from './components/spendings/spending-form/spending-form.component';
import { MonthlyViewComponent } from './components/monthly/monthly-view/monthly-view.component';
import { UserListComponent } from './components/users/user-list/user-list.component';
import { authGuard, mainUserGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register-main-user', component: RegisterMainUserComponent },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'monthly', component: MonthlyViewComponent },
      { path: 'buckets', component: BucketListComponent },
      { path: 'spendings', component: SpendingListComponent },
      { path: 'spendings/new', component: SpendingFormComponent },
      { path: 'users', component: UserListComponent, canActivate: [mainUserGuard] }
    ]
  },
  { path: '', component: InitialCheckComponent }
];
