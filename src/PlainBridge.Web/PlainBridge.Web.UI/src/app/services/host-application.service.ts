import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ResultDto, HostApplicationDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class HostApplicationService {
  private readonly apiUrl = `${environment.apiUrl}/HostApplication`;

  constructor(private http: HttpClient) {}

  getAllApplications(): Observable<ResultDto<HostApplicationDto[]>> {
    return this.http.get<ResultDto<HostApplicationDto[]>>(this.apiUrl);
  }

  getApplication(id: number): Observable<ResultDto<HostApplicationDto>> {
    return this.http.get<ResultDto<HostApplicationDto>>(`${this.apiUrl}/${id}`);
  }

  createApplication(application: HostApplicationDto): Observable<ResultDto<string>> {
    return this.http.post<ResultDto<string>>(this.apiUrl, application);
  }

  updateApplication(id: number, application: HostApplicationDto): Observable<ResultDto<HostApplicationDto>> {
    return this.http.put<ResultDto<HostApplicationDto>>(`${this.apiUrl}/${id}`, application);
  }

  deleteApplication(id: number): Observable<ResultDto<any>> {
    return this.http.delete<ResultDto<any>>(`${this.apiUrl}/${id}`);
  }
}
