import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResultDto, UserDto, ChangeUserPasswordDto, CreateUserDto } from '../models';
import { AppConfigService } from './app-config.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  
  private readonly appConfig = inject(AppConfigService);
  private readonly apiUrl = `${this.appConfig.settings.apiUrl}/api/User`; 
 

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<ResultDto<UserDto[]>> {
    return this.http.get<ResultDto<UserDto[]>>(this.apiUrl);
  }

  getCurrentUser(): Observable<ResultDto<UserDto>> {
    return this.http.get<ResultDto<UserDto>>(`${this.apiUrl}/GetCurrentUser`);
  }

  createUser(user: CreateUserDto): Observable<ResultDto<string>> {
    return this.http.post<ResultDto<string>>(this.apiUrl, user);
  }

  updateUser(id: number, user: UserDto): Observable<ResultDto<any>> {
    return this.http.patch<ResultDto<any>>(`${this.apiUrl}/${id}`, user);
  }

  changePassword(changePasswordDto: ChangeUserPasswordDto): Observable<ResultDto<any>> {
    return this.http.patch<ResultDto<any>>(`${this.apiUrl}/ChangePassword`, changePasswordDto);
  }
}
