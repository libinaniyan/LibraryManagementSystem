import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddBooksComponent } from './books/add-books/add-books.component';
import { GetBooksComponent } from './books/get-books/get-books.component';

const routes: Routes = [
  {path:'get-books' , component:GetBooksComponent},
  {path:'add-books' , component:AddBooksComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
