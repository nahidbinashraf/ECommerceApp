import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private _http: HttpClient, private _router: Router) { }

  private baseUrlLogin: string = "http://localhost:13414/api/Account";
  private loginStatus = new BehaviorSubject<boolean>(this.checkLoginStatus());
  private UserName = new BehaviorSubject<string>(localStorage.getItem("userName"));
  private UserRole = new BehaviorSubject<string>(localStorage.getItem("userRole"));

  login(username: string, password: string) {
    this._http.post<any>(this.baseUrlLogin + "/login", { username, password }).pipe(
      map(_result => {
        //check result has token
        if (_result && _result.token) {
          this.loginStatus.next(true);
          localStorage.setItem("loginStatus", "1");
          localStorage.setItem("jwt", _result.token);
          localStorage.setItem("userName", _result.userName);
          localStorage.setItem("userRole", _result.roleName);
          localStorage.setItem("expire", _result.expiration);
        }

        return _result;
      })

    );
  }
  logOut() {
    this.loginStatus.next(false);
    localStorage.removeItem("jwt");
    localStorage.removeItem("userName");
    localStorage.removeItem("userRole");
    localStorage.removeItem("expire");
    localStorage.setItem("loginStatus", "0");
    this._router.navigate(['/login']);
    console.log("Logout successfully");
  }
  checkLoginStatus(): boolean {
    return false;
  }

  getIsLoggedIn() {
    return this.loginStatus.asObservable();
  }
  getUserName() {
    this.UserName.asObservable();
  }
  getUserRole() {
    this.UserRole.asObservable();
  }
}
