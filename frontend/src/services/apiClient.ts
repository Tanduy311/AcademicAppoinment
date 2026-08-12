export class ApiError extends Error {
  status: number;
  payload: unknown;

  constructor(status: number, message: string, payload: unknown) {
    super(message);
    this.status = status;
    this.payload = payload;
  }
}

const isProd = import.meta.env.PROD;
const envUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5168';
const rawBaseUrl = isProd ? '' : envUrl;
const baseUrl = rawBaseUrl.replace(/\/$/, '');

function getToken() {
  return localStorage.getItem('aa_token');
}

export async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers ?? {});
  const token = getToken();

  if (token) {
    headers.set('Authorization', `Bearer ${token}`);
  }

  if (init.body && !(init.body instanceof FormData) && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json');
  }

  const response = await fetch(`${baseUrl}${path}`, {
    ...init,
    headers,
  });

  const contentType = response.headers.get('content-type') ?? '';
  const payload = contentType.includes('application/json')
    ? await response.json()
    : await response.text();

  if (!response.ok) {
    const message =
      (payload as { message?: string; title?: string })?.message ??
      (payload as { message?: string; title?: string })?.title ??
      response.statusText ??
      'Request failed';
    throw new ApiError(response.status, message, payload);
  }

  return payload as T;
}
