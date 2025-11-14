import { useEffect, useRef, useState } from 'react';
import { useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { useQueryParams, ArrayParam } from 'use-query-params';
import type { DecodedValueMap, QueryParamConfig, /*QueryParamConfigMap,*/ SetQuery } from 'use-query-params';

/**
 * Controls how the query parameter is updated.
 *
 * - `'live'` parameters sync immediately to the URL.
 * - `'deferred'` parameters are stored locally and can be sync'd to the URL via `commit()`.
 * - `'local'` parameters are stored locally (internal to the component).
 */
type ParamMode = 'live' | 'deferred'; // | 'local';

/**
 * Extended query param config type supporting synchronization mode.
 *
 * This type wraps a standard `QueryParamConfig<T>` and adds an optional
 * `syncMode` property to control whether a parameter is synchronized
 * live or deferred.
 *
 * @template T - The type of the parameter's value.
 */
export type SyncQueryParamConfig<T = any> = QueryParamConfig<T> & {
  /**
   * Optional synchronization mode for this parameter.
   * Defaults to `'deferred'` if omitted.
   */
  syncMode?: ParamMode;
};

/**
 * A map of query parameter names to `SyncQueryParamConfig` entries.
 *
 * This extends the standard `QueryParamConfigMap` to allow each parameter
 * to specify its synchronization behavior independently.
 */
export type SyncQueryParamConfigMap = Record<string, SyncQueryParamConfig<any>>;
// export type SyncQueryParamConfigMap = {
//   [paramName: string]: SyncQueryParamConfig<any>;
// };

/**
 * The result tuple returned by `useQueryParamsCI`, with support for synchronization
 * modes like `'live'` and `'deferred'`.
 *
 * This is our own equivalent of the (unexported) UseQueryParamsResult<T> with
 * extra flavor.
 *
 * @template T - The parameter config map type.
 */
export type UseQueryParamsCIResult<T extends SyncQueryParamConfigMap> = {
  /** Decoded query parameter values using the canonical casing from config */
  params: DecodedValueMap<T>;

  /** Setter function for updating query parameters */
  setParams: SetQuery<T>;

  /** Commits any 'deferred' parameter values to the URL */
  commit: () => void;
};

/**
 * Internal helper: Find the actual query param name (case-insensitively)
 */
function resolveParamName(expectedKey: string, allKeys: string[]): string | null {
  const lowerExpected = expectedKey.toLowerCase();
  return allKeys.find(k => k.toLowerCase() === lowerExpected) ?? null;
}

/**
 * A typed, case-insensitive wrapper around `useQueryParams`.
 *
 * - Automatically matches query parameter names case-insensitively.
 * - Ensures only one variant of a given parameter (e.g. `bar=1`, not `bar=1&BAR=2`) for
 * all non `ArrayParam` parameter types.
 * - Preserves the casing defined in `paramConfig`.
 * - Supports multi-value parameters (e.g., `ArrayParam`).
 * - Supports `syncMode: 'live' | 'deferred'` per parameter.
 *
 * @template T - The `SyncQueryParamConfigMap` type defining parameter config.
 * @param paramConfig - Configuration for the query parameters.
 * @returns An object containing:
 *   - `params`: All current query param values, keyed using the casing from `paramConfig`.
 *   - `setParams`: A setter that updates params using the same casing and removes case variants.
 *   - `commit: A function to sync 'deferred' params to the URL.
 */
export function useQueryParamsCI<T extends SyncQueryParamConfigMap>(
  paramConfig: T
): UseQueryParamsCIResult<T> {

  const [searchParams] = useSearchParams();
  const [params, setParams] = useQueryParams(paramConfig);
  const navigate = useNavigate();
  const location = useLocation();
  const normalizedOnce = useRef(false);
  const [deferredCache, setDeferredCache] = useState<Partial<DecodedValueMap<T>>>({});

  const allKeysInSearchParams = Array.from(searchParams.keys());

  // Map real param keys to case-insensitive matches
  const resolvedKeyMap: Record<string, string> = {};
  for (const key of Object.keys(paramConfig)) {
    const actualKey = resolveParamName(key, allKeysInSearchParams);
    if (actualKey) resolvedKeyMap[key] = actualKey;
  }

  // Build an accessor that uses actual casing in the current URL
  const resolvedParams = {} as DecodedValueMap<T>;
  for (const key of Object.keys(paramConfig)) {
    const actualKey = resolvedKeyMap[key] ?? key;
    resolvedParams[key as keyof T] = params[actualKey as keyof typeof params];
  }

  // Wrapper setter to remap lowercase keys to their actual param names.
  // Includes support for syncMode.
  const setParamsCI: SetQuery<T> = (newParams, updateType) => {
    const remapped: DecodedValueMap<T> = { ...params };

    for (const key in newParams) {
      if (!Object.prototype.hasOwnProperty.call(newParams, key)) continue;

      const configEntry = paramConfig[key];
      const mode = configEntry?.syncMode ?? 'deferred';

      if (mode === 'live') {
        // Purge all variants from remapped before applying live update
        for (const existingKey of allKeysInSearchParams) {
          if (existingKey.toLowerCase() === key.toLowerCase()) {
            delete remapped[existingKey as keyof DecodedValueMap<T>];
          }
        }
        remapped[key as keyof DecodedValueMap<T>] = newParams[key as keyof typeof newParams];
      } else {
        // Deferred mode: update local cache
        setDeferredCache(prev => (
          { ...prev, [key]: newParams[key as keyof typeof newParams] }
        ));
      }

      // for (const existingKey of allKeysInSearchParams) {
      //   if (existingKey.toLowerCase() === key.toLowerCase()) {
      //     delete remapped[existingKey as keyof DecodedValueMap<T>];
      //   }
      // }
      // remapped[key as keyof DecodedValueMap<T>] = newParams[key as keyof typeof newParams];
    }

    // Only update URL if there are live params to sync
    setParams(remapped, updateType);
  };

  // Sync deferred params
  const syncParams = () => {
    if (Object.keys(deferredCache).length === 0) return;

    const remapped: DecodedValueMap<T> = { ...params };
    for (const key of Object.keys(deferredCache) as (keyof DecodedValueMap<T>)[]) {
      if (!Object.prototype.hasOwnProperty.call(deferredCache, key)) continue;
      for (const existingKey of allKeysInSearchParams) {
        const lowerKey = (key as string).toLowerCase();
        if (existingKey.toLowerCase() === lowerKey) {
          delete remapped[existingKey as keyof DecodedValueMap<T>];
        }
      }
      const value = deferredCache[key];
      if (value !== undefined) {
        remapped[key] = value;
      }
    }

    setParams(remapped);
    setDeferredCache({});
  };

  useEffect(() => {
    if (normalizedOnce.current) return;
    normalizedOnce.current = true;

    const currentParams = new URLSearchParams(location.search);
    const normalizedParams = new URLSearchParams();

    // Build lowercase -> canonical casing map
    const lowerToCanonical: Record<string, string> = {}; // lowercase -> canonical casing from paramConfig
    for (const key of Object.keys(paramConfig)) {
      lowerToCanonical[key.toLowerCase()] = key;
    }

    const addedSingleValueKeys = new Set<string>();

    for (const [rawKey, value] of currentParams.entries()) {
      const lowerKey = rawKey.toLowerCase();
      const canonicalKey = lowerToCanonical[lowerKey];

      // If this key is controlled by paramConfig, only add it under its canonical name
      if (canonicalKey) {
        const isArrayParam = paramConfig[canonicalKey] === ArrayParam;

        if (isArrayParam) {
          // Append mulitple values for ArrayParam
          normalizedParams.append(canonicalKey, value);
        } else if (!addedSingleValueKeys.has(canonicalKey)) {
          normalizedParams.set(canonicalKey, value);
          addedSingleValueKeys.add(canonicalKey);
        }
      } else {
        // Not controlled → preserve as-is
        normalizedParams.append(rawKey, value);
      }
    }

    const normalizedQueryString = normalizedParams.toString();
    if (normalizedQueryString !== currentParams.toString()) {
      navigate(`${location.pathname}?${normalizedQueryString}`, { replace: true });
    }
  }, [location, navigate, paramConfig]);

  return {
    params: resolvedParams,
    setParams: setParamsCI,
    commit: syncParams
  };

}