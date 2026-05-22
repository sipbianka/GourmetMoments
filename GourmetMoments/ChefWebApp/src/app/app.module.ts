import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { App } from './app';
import { Chef } from './chef/chef';
import { Home } from './home/home';

@NgModule({
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    App,
    Home,
    Chef
  ]
})
export class AppModule { }
