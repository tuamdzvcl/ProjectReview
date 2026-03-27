import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { TokenService } from '../../../../core/services/token.service';
import { AuthService } from '../../auth.service';

@Component({
  selector: 'app-login-success',
  template: `<p>không chạy đâu nhé
  </p>`
})
export class LoginSuccessComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private token: TokenService
  ) {}

  ngOnInit(): void {
    const key = this.route.snapshot.queryParamMap.get('key');

    if (!key) {
      console.error('Không có key');
      return;
    }
    this.authService.getGoogleResult(key).subscribe({
      next: (res)=>{
        
          console.log("ừ ",res)
          this.token.setToken(res.AccessToken,res.RefreshToken)
        localStorage.setItem('user', JSON.stringify(res.User));
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Lỗi login Google:', err);
      }
    });
}
    
  
}