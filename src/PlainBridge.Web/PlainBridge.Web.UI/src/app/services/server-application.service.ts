import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ResultDto, ServerApplicationDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ServerApplicationService {
  private readonly apiUrl = `${environment.apiUrl}/ServerApplication`;

  constructor(private http: HttpClient) {}

  getAllApplications(): Observable<ResultDto<ServerApplicationDto[]>> {
    return this.http.get<ResultDto<ServerApplicationDto[]>>(this.apiUrl);
  }

  getApplication(id: number): Observable<ResultDto<ServerApplicationDto>> {
    return this.http.get<ResultDto<ServerApplicationDto>>(`${this.apiUrl}/${id}`);
  }

  createApplication(application: ServerApplicationDto): Observable<ResultDto<string>> {
    return this.http.post<ResultDto<string>>(this.apiUrl, application);
  }

  updateApplication(id: number, application: ServerApplicationDto): Observable<ResultDto<ServerApplicationDto>> {
    return this.http.patch<ResultDto<ServerApplicationDto>>(`${this.apiUrl}/${id}`, application);
  }

  deleteApplication(id: number): Observable<ResultDto<any>> {
    return this.http.delete<ResultDto<any>>(`${this.apiUrl}/${id}`);
  }

  // Toggle active state
  patchIsActive(id: number, isActive: boolean): Observable<ResultDto<ServerApplicationDto>> {
    return this.http.patch<ResultDto<ServerApplicationDto>>(`${this.apiUrl}/UpdateState/${id}/${isActive}`, { });
  }
}
