import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { AppConfig } from '../models/app-config';
 
@Injectable({
  providedIn: 'root'
})
export class AppConfigService {
  private readonly http = inject(HttpClient);

  private config: AppConfig | null = null;

async load(): Promise<void> {

  try {
    const config = await firstValueFrom(
      this.http.get<AppConfig>('/assets/config.json')
    );

    this.config = config;

  } catch (error) {
    console.error('[AppConfig] Failed to load configuration:', error);
    throw error;
  }
}

get settings(): AppConfig {

  if (!this.config) {
    throw new Error('Application configuration has not been loaded.');
  }

  return this.config;
}
}