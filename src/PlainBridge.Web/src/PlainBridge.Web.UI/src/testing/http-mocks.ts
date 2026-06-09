import { HttpRequest } from '@angular/common/http';

export type HttpMethod = 'GET' | 'POST' | 'PATCH' | 'DELETE' | 'PUT';

export interface ExpectRequestOptions {
  url: string | RegExp;
  method: HttpMethod;
  headers?: Record<string, string | RegExp>;
  body?: any;
}

export function expectRequest(req: HttpRequest<any>, options: ExpectRequestOptions) {
  expect(req.method).toBe(options.method);
  if (typeof options.url === 'string') {
    expect(req.url).toBe(options.url);
  } else {
    expect(options.url.test(req.url)).toBeTrue();
  }
  if (options.headers) {
    for (const [k, v] of Object.entries(options.headers)) {
      const h = req.headers.get(k);
      if (v instanceof RegExp) {
        expect(v.test(h || '')).toBeTrue();
      } else {
        expect(h).toBe(v);
      }
    }
  }
  if (typeof options.body !== 'undefined') {
    expect(req.body).toEqual(options.body);
  }
}

