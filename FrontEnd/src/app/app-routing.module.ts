import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';

const routes: Routes = [];

@NgModule({
  imports: [RouterModule.forRoot([
    {path:'home', component:HomeComponent},
    {path:'login', component:LoginComponent},
    {path:'', component:HomeComponent, pathMatch:"full"},
    {path:'**', redirectTo:'/home'}
  ])],
  exports: [RouterModule]
})
export class AppRoutingModule 
{

}
