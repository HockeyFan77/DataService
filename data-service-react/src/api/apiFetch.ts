import axios from 'axios';
import type { AxiosRequestConfig, AxiosResponse } from 'axios';

//const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

// function makeApiUrl(
//   endpoint: string,
//   context?: string,
//   queryParams?: Record<string, string | number | boolean | undefined | null>): string {
//   //const url = endpoint.startsWith('/') ? `/api${endpoint}` : `/api/${endpoint}`;
//   //const url = endpoint.startsWith('/api') ? endpoint : `/api${endpoint.startsWith('/') ? '' : '/'}${endpoint}`;
//   return `/api/${context ? context + '/' : ''}${endpoint}`;
// }

type ApiFetchOptions = {
  context?: string,
  params?: Record<string, string | number | boolean | undefined | null>,
};

export async function apiFetch(
  endpoint: string,
  apiOptions: ApiFetchOptions = {},
  axiosOptions: AxiosRequestConfig = {},
): Promise<AxiosResponse> {
  const path = [
    '/api',
    // context ? encodeURIComponent(context) : null,
    endpoint.replace(/^\/+/, ''),
  ]
  .filter(Boolean)
  .join('/');

  const query = new URLSearchParams();
  if (apiOptions?.context) {
    query.append('ctx', apiOptions.context!);
  }
  for (const [key, value] of Object.entries(apiOptions?.params || {})) {
    if (Array.isArray(value)) {
      query.append(key, value.join(',')); // customize as needed
    } else if (value !== null && value !== undefined) {
      query.append(key, String(value));
    }
  }

  const url = query.toString() ? `${path}?${query.toString()}` : path;

  const headers = {
     ...(axiosOptions.headers || {}),
     //...(context ? { 'X-Api-Context': context } : {}),
     'Content-Type': 'application/json',
  };

  return axios({
    url,
    method: axiosOptions.method || 'get',
   ...axiosOptions,
    headers,
  });
}