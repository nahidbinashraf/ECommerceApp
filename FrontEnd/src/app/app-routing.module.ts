import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { ProductsComponent } from './products/products.component';

const routes: Routes = [];

@NgModule({
  imports: [RouterModule.forRoot([
    {path:'home', component:HomeComponent},
    {path:'products', component:ProductsComponent},
    {path:'login', component:LoginComponent},
    {path:'', component:HomeComponent, pathMatch:"full"},
    {path:'**', redirectTo:'/home'}
  ])],
  exports: [RouterModule]
})
export class AppRoutingModule 
{

}
