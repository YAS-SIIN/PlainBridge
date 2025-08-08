import { Injectable } from '@angular/core';
import { Observable, map, catchError, throwError } from 'rxjs';
import { ToastService } from './toast.service';
import { ResultDto, ResultCodeEnum } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ApiResponseService {
  constructor(private toastService: ToastService) {}

  /**
   * Handles API response and shows appropriate toast notification
   * @param response$ Observable of ResultDto response
   * @param options Configuration options
   * @returns Observable with processed data
   */
  handleResponse<T>(
    response$: Observable<ResultDto<T>>, 
    options?: {
      showSuccessToast?: boolean;
      showErrorToast?: boolean;
      successMessage?: string;
      errorMessage?: string;
      suppressToast?: boolean;
    }
  ): Observable<T> {
    const config = {
      showSuccessToast: true,
      showErrorToast: true,
      suppressToast: false,
      ...options
    };

    return response$.pipe(
      map((result: ResultDto<T>) => {
        if (result.resultCode === ResultCodeEnum.Success) {
          // Show success toast using resultDescription from API
          if (config.showSuccessToast && !config.suppressToast) {
            const message = config.successMessage || result.resultDescription || 'Operation completed successfully';
            this.toastService.success(message);
          }
          return result.data;
        } else {
          // Handle non-success responses
          const errorMessage = config.errorMessage || result.resultDescription || 'Operation failed';
          
          if (config.showErrorToast && !config.suppressToast) {
            if (result.resultCode === ResultCodeEnum.NotFound) {
              this.toastService.warning(errorMessage);
            } else {
              this.toastService.error(errorMessage);
            }
          }
          
          throw new Error(errorMessage);
        }
      }),
      catchError((error) => {
        // Handle HTTP errors or other exceptions
        const errorMessage = config.errorMessage || error.message || 'An unexpected error occurred';
        
        if (config.showErrorToast && !config.suppressToast) {
          this.toastService.error(errorMessage);
        }
        
        return throwError(() => error);
      })
    );
  }

  /**
   * Handles API response without automatically showing toast (for manual handling)
   * @param response$ Observable of ResultDto response
   * @returns Observable with the full ResultDto
   */
  handleResponseManual<T>(response$: Observable<ResultDto<T>>): Observable<ResultDto<T>> {
    return response$;
  }

  /**
   * Shows appropriate toast based on ResultDto
   * @param result The API result
   * @param customMessage Optional custom message
   */
  showToastFromResult<T>(result: ResultDto<T>, customMessage?: string): void {
    const message = customMessage || result.resultDescription || 'Operation completed';
    
    switch (result.resultCode) {
      case ResultCodeEnum.Success:
        this.toastService.success(message);
        break;
      case ResultCodeEnum.NotFound:
        this.toastService.warning(message);
        break;
      case ResultCodeEnum.Error:
      case ResultCodeEnum.InvalidData:
      case ResultCodeEnum.NullData:
      case ResultCodeEnum.NotDelete:
        this.toastService.error(message);
        break;
      case ResultCodeEnum.RepeatedData:
        this.toastService.warning(message);
        break;
      default:
        this.toastService.info(message);
        break;
    }
  }
 
}
