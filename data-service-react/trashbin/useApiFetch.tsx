import { useCallback } from 'react';
import { useApiContext } from './useApiContext';
import { apiFetch } from './ApiUtils';

export function useApiFetch() {
  const apiContextState = useApiContext();
  // need to memoize apiFetch() to prevent infinite renders
  return useCallback((
    url: string,
    options: RequestInit = {},
  ) =>
    apiFetch(url, apiContextState.selectedContext?.context, options),
    [apiContextState.selectedContext?.context]
  );
}