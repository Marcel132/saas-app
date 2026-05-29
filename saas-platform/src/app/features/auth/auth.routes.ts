import { Routes } from "@angular/router";
import { LoginPage } from "./pages/login-page/login-page";
import { RegisterPage } from "./pages/register-page/register-page";

export const authRoutes : Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: "full"
  },
  {
    path: 'login',
    component: LoginPage
  },
  {
    path: 'register',
    component: RegisterPage
  }
]
