import { useEffect, useRef, useState } from 'react';
import type { QueryParamConfig } from 'use-query-params';
import { useQueryParamsCI } from './useQueryParamsCI';
import useDebounce from './useDebounce'; // ← import your custom debounce hook

/**
 * Controls how the query parameter is updated.
 *
 * - `'live'` parameters sync immediately to the URL.
 * - `'deferred'` parameters are stored locally and can be sync'd to the URL via `commit()`.
 * - `'local'` parameters are stored locally (internal to the component).
 */
type ParamMode = 'live' | 'deferred' | 'local';

/**
 * Configuration for a single query parameter.
 *
 * @template T The type of the parameter's value.
 */
export type ControlledQueryParam<T = any> = {
  /**
   * Specifies the type of the query parameter for the `useTypedQueryParams` hook.
   * @type {QueryParamConfig<T>}
   */
  type: QueryParamConfig<T>;
  /**
   * Controls how the query parameter is updated (default: `'deferred'`).
   * @type {ParamMode}
   */
  mode?: ParamMode;
  /**
   * If greater than zero, debounce updates to this parameter by this many milliseconds (default: 0).
   * @type {number}
   */
  debounceMs?: number;
};

export type ControlledParamsConfig<T extends Record<string, ControlledQueryParam>> = T;

/**
 * Hook for managing query parameters with support for live/deferred/local modes and optional debouncing.
 */
export function useControlledQueryParams<T extends Record<string, ControlledQueryParam>>(
  paramsConfig: T
) {
  const paramShape = Object.fromEntries(
    Object.entries(paramsConfig).map(([key, val]) => [key, val.type])
  ) as { [K in keyof T]: QueryParamConfig<any> };

  const [queryParams, setQueryParams] = useQueryParamsCI(paramShape);

  const getConfig = <K extends keyof T>(key: K): ControlledQueryParam => paramsConfig[key];
  const getMode = <K extends keyof T>(key: K): ParamMode => getConfig(key).mode ?? 'deferred';
  const getDebounceMs = <K extends keyof T>(key: K): number => getConfig(key).debounceMs ?? 0;

  const [localValues, setLocalValues] = useState(() => ({ ...queryParams }));
  const initialValuesRef = useRef({ ...queryParams });

  const pendingLiveValuesRef = useRef<Partial<Record<keyof T, any>>>({});

  // Merge live and local values for output
  const mergedValues = {} as Record<keyof T, any>;
  for (const key in paramsConfig) {
    const mode = getMode(key);
    mergedValues[key] = mode === 'live' ? queryParams[key] : localValues[key];
  }

  // Setters map
  const setParamValue = <K extends keyof T>(key: K, value: any) => {
    const mode = getMode(key);

    if (mode === 'live') {
      const debounceMs = getDebounceMs(key);
      if (debounceMs < 1) {
        // No debounce, immediate update
        setQueryParams({ [key]: value }, 'replaceIn');
      } else {
        pendingLiveValuesRef.current[key] = value;
        setLiveUpdateFlag((prev) => prev + 1); // triggers useDebounce
      }
    } else if (mode === 'deferred') {
      pendingLiveValuesRef.current[key] = value;
      setDeferredUpdateFlag((prev) => prev + 1); // triggers useDebounce
    } else {
      setLocalValues((prev) => ({ ...prev, [key]: value }));
    }
  };

  // Track changes to pending live values to debounce updates
  const [liveUpdateFlag, setLiveUpdateFlag] = useState(0);
  const liveDebouncedKeys = Object.keys(paramsConfig).filter(
    (key) => getMode(key) === 'live' && getDebounceMs(key) > 0
  ) as (keyof T)[];

  useDebounce(
    () => {
      if (Object.keys(pendingLiveValuesRef.current).length > 0) {
        setQueryParams(pendingLiveValuesRef.current, 'replaceIn');
        pendingLiveValuesRef.current = {};
      }
    },
    // Use the *max* debounceMs across live params as a safe fallback
    Math.max(0, ...liveDebouncedKeys.map(getDebounceMs)),
    [liveUpdateFlag],
    { leading: false, trailing: true }
  );

  // Commit only deferred params to the URL
  const commit = () => {
    const toCommit: Partial<Record<keyof T, any>> = {};
    for (const key in paramsConfig) {
      if (getMode(key) === 'deferred') {
        toCommit[key] = localValues[key];
      }
    }
    if (Object.keys(toCommit).length > 0) {
      setQueryParams(toCommit, 'replaceIn');
    }
  };

  // Reset both local and URL values to initial snapshot
  const reset = () => {
    const initial = initialValuesRef.current;

    setLocalValues(initial);

    const liveReset: Partial<Record<keyof T, any>> = {};
    for (const key in paramsConfig) {
      if (getMode(key) === 'live') {
        liveReset[key] = initial[key];
      }
    }

    if (Object.keys(liveReset).length > 0) {
      setQueryParams(liveReset, 'replaceIn');
    }

    pendingLiveValuesRef.current = {};
  };

  // Keep local in sync with queryParams for non-live modes
  useEffect(() => {
    let updated: Partial<Record<keyof T, any>> = {};
    let changed = false;
    for (const key in paramsConfig) {
      const mode = getMode(key);
      if (mode !== 'live' && queryParams[key] !== localValues[key]) {
        updated[key] = queryParams[key];
        changed = true;
      }
    }
    if (changed) {
      setLocalValues((prev) => ({ ...prev, ...updated }));
    }
  }, [queryParams]);

  return {
    values: mergedValues,
    setParamValue,
    commit,
    reset,
  };
}