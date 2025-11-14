import { useCallback, useEffect, useRef } from "react";

/**
 * `useTimeout` hook
 *
 * Runs `callback` after a specified `delay`. Offers `reset` and `clear` methods to
 * manually control the timer. Automatically starts on mount unless `autoStart` is false.
 *
 * Shamelessly plagiarized from:
 * https://www.youtube.com/watch?v=0c6znExIqRw&list=PLZlA0Gpn_vH-aEDXnaFNLsqiJWFpIWV03
 *
 * @param callback Function to be called after the delay.
 * @param delay Delay in milliseconds.
 * @param options Optional timeout configuration.
 * @param options.autoStart If true, starts the timeout on mount (default: true).
 * @returns Object with `reset` and `clear` functions.
 */
export default function useTimeout(
  callback: () => void,
  delay: number,
  options?: {
    autoStart?: boolean
  }
) {
  const { autoStart = true } = options ?? {};
  const callbackRef = useRef<() => void>(callback);
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    callbackRef.current = callback;
  }, [callback]);

  const clear = useCallback(() => {
    if (timeoutRef.current !== null) {
      clearTimeout(timeoutRef.current);
      timeoutRef.current = null;
    }
  }, []);

  const set = useCallback(() => {
    clear();
    timeoutRef.current = setTimeout(() => callbackRef.current(), delay);
  }, [delay, clear]);

  const reset = useCallback(() => {
    set();
  }, [set]);

  useEffect(() => {
    if (autoStart) {
      set();
      return clear;
    }
  }, [delay, set, clear, autoStart]);

  return { reset, clear };
}