import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  userName: FormControl;
  passWord: FormControl;
  returnURL: string;
  errorMessage: string;
  inValidLoggedIn: boolean;
  constructor(
    private _accountService: AccountService,
    private _router: Router,
    private _route: ActivatedRoute,
    private _formBuild: FormBuilder
  ) {}

  ngOnInit(): void {
    this.userName = new FormControl('', [Validators.required]);
    this.passWord = new FormControl('', [Validators.required]);

    this.returnURL = this._route.snapshot['returnUrl'] || '/';
    this.loginForm = this._formBuild.group({
      userName: this.userName,
      password: this.passWord,
    });
  }
  submitLogin() {
    let _loginData = this.loginForm.value;
    this._accountService
      .login(_loginData.userName, _loginData.password)
      .subscribe(
        (result) => {
          let token = (<any>result).token;
          console.log(token);
          console.log(result.userName);
          console.log(result.roleName);
          console.log('User logged in successfully');
          this.inValidLoggedIn = false;
          this._router.navigateByUrl(this.returnURL);
        },
        (error) => {
          this.errorMessage = 'Invalid user name or password ! Please log in';
          this.inValidLoggedIn = true;
        }
      );
  }
}
