# API Response Handling with ResultDto

This project now uses a consistent API response structure with automatic toast notifications based on the `resultDescription` from the API.

## API Response Structure

All API responses follow this structure:
```typescript
interface ResultDto<T> {
  data: T;
  errors: { [key: string]: string[] } | null;
  resultCode: ResultCodeEnum;
  resultDescription: string;
}
```

Example API response:
```json
{
  "data": "087e2467-0b5b-4ec7-9345-20cebd441bdd",
  "errors": null,
  "resultCode": 0,
  "resultDescription": "Action done successfully."
}
```

## Usage

### 1. Using ApiResponseService for Automatic Toast Handling

The `ApiResponseService` automatically shows toast notifications using the `resultDescription` from the API:

```typescript
import { ApiResponseService } from '../services/api-response.service';

constructor(
  private apiResponseService: ApiResponseService,
  private someDataService: SomeDataService
) {}

// Automatic success/error toasts
saveData(data: any): void {
  this.apiResponseService.handleResponse(
    this.someDataService.create(data)
  ).subscribe({
    next: (result) => {
      // Success toast is shown automatically with resultDescription
      console.log('Data saved:', result);
    },
    error: () => {
      // Error toast is shown automatically
      console.log('Operation failed');
    }
  });
}
```

### 2. Configuration Options

You can customize the toast behavior:

```typescript
// Don't show success toast (useful for data loading)
this.apiResponseService.handleResponse(
  this.dataService.getData(),
  { showSuccessToast: false }
).subscribe(...);

// Custom messages override resultDescription
this.apiResponseService.handleResponse(
  this.dataService.deleteData(id),
  { 
    successMessage: 'Item deleted successfully!',
    errorMessage: 'Failed to delete item'
  }
).subscribe(...);

// Suppress all toasts for manual handling
this.apiResponseService.handleResponse(
  this.dataService.someAction(),
  { suppressToast: true }
).subscribe(...);
```

### 3. Manual Toast Handling

For more control, you can handle responses manually:

```typescript
this.apiResponseService.handleResponseManual(
  this.dataService.someAction()
).subscribe({
  next: (result) => {
    if (this.apiResponseService.isSuccess(result)) {
      // Custom success handling
      this.apiResponseService.showToastFromResult(result, 'Custom success message');
    } else {
      // Custom error handling
      this.apiResponseService.showToastFromResult(result);
    }
  }
});
```

### 4. Toast Types Based on ResultCode

The service automatically chooses the appropriate toast type:

- `Success` (0) → Success toast (green)
- `NotFound` (100) → Warning toast (yellow)
- `Error` (101) → Error toast (red)
- `RepeatedData` (2) → Warning toast (yellow)
- `InvalidData`, `NullData`, `NotDelete` → Error toast (red)

## Migration from Old Pattern

### Before:
```typescript
this.dataService.getData().subscribe({
  next: (result) => {
    if (result.isSuccess) {
      this.data = result.data;
      this.toastService.success('Data loaded successfully');
    } else {
      this.toastService.error('Failed to load data');
    }
  },
  error: () => {
    this.toastService.error('Something went wrong');
  }
});
```

### After:
```typescript
this.apiResponseService.handleResponse(
  this.dataService.getData(),
  { showSuccessToast: false } // Don't show toast for data loading
).subscribe({
  next: (data) => {
    this.data = data; // Direct access to data
  },
  error: () => {
    // Error handling is done automatically
  }
});
```

## Benefits

1. **Consistent Toast Messages**: All toasts use the `resultDescription` from the API
2. **Reduced Boilerplate**: No need to manually check `isSuccess` or handle toasts
3. **Centralized Error Handling**: Consistent error presentation across the app
4. **Type Safety**: Direct access to typed data without wrapper objects
5. **Configurable**: Options to customize toast behavior per use case
