import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ResultDto, UserDto, ChangeUserPasswordDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl = `${environment.apiUrl}/User`;

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<ResultDto<UserDto[]>> {
    return this.http.get<ResultDto<UserDto[]>>(this.apiUrl);
  }

  getCurrentUser(): Observable<ResultDto<UserDto>> {
    return this.http.get<ResultDto<UserDto>>(`${this.apiUrl}/GetCurrentUser`);
  }

  createUser(user: UserDto): Observable<ResultDto<string>> {
    return this.http.post<ResultDto<string>>(this.apiUrl, user);
  }

  updateUser(id: number, user: UserDto): Observable<ResultDto<any>> {
    return this.http.patch<ResultDto<any>>(`${this.apiUrl}/${id}`, user);
  }

  changePassword(changePasswordDto: ChangeUserPasswordDto): Observable<ResultDto<any>> {
    return this.http.patch<ResultDto<any>>(`${this.apiUrl}/ChangePassword`, changePasswordDto);
  }
}
