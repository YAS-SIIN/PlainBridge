import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResultDto, HostApplicationDto } from '../models';
import { AppConfigService } from './app-config.service';

@Injectable({
  providedIn: 'root'
})
export class HostApplicationService {
  private readonly appConfig = inject(AppConfigService);
  private readonly apiUrl = `${this.appConfig.settings.apiUrl}/api/HostApplication`;

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
    return this.http.patch<ResultDto<HostApplicationDto>>(`${this.apiUrl}/${id}`, application);
  }

  deleteApplication(id: number): Observable<ResultDto<any>> {
    return this.http.delete<ResultDto<any>>(`${this.apiUrl}/${id}`);
  }

  // Toggle active state
  patchIsActive(id: number, isActive: boolean): Observable<ResultDto<HostApplicationDto>> {
    return this.http.patch<ResultDto<HostApplicationDto>>(`${this.apiUrl}/UpdateState/${id}/${isActive}`, { });
  }
}
