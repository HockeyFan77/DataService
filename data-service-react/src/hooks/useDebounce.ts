import { useEffect, useRef, type DependencyList } from "react";
import useTimeout from "./useTimeout";

/**
 * `useDebounce` hook
 *
 * Delays invoking the `callback` until after `delay` milliseconds have passed
 * since the last time one of the `dependencies` changed. Useful for reducing
 * the rate of effectful operations such as API calls or state updates.
 *
 * Shamelessly plagiarized from:
 * https://www.youtube.com/watch?v=0c6znExIqRw&list=PLZlA0Gpn_vH-aEDXnaFNLsqiJWFpIWV03
 *
 * @param callback Function to debounce.
 * @param delay Delay in milliseconds.
 * @param dependencies Triggers debounce when any value changes.
 * @param options Optional debounce configuration.
 * @param options.leading If `true`, invokes `callback` immediately on the first change. (default: `false`).
 * @param options.trailing If `true`, invokes `callback` after `delay` ms of no changes. (default: `true`).
 */
export default function useDebounce(
  callback: () => void,
  delay: number,
  dependencies: DependencyList,
  options?: {
    leading: boolean;  // false
    trailing: boolean; // true
  }
) {
  const { leading = false, trailing = true, } = options ?? {};
  const isFirstCall = useRef(true);
  const pendingTrailingCall = useRef(false);

  const { reset, clear } = useTimeout(() => {
    if (trailing && pendingTrailingCall.current) {
      callback();
      pendingTrailingCall.current = false;
    }
  }, delay, { autoStart: true });

  useEffect(() => {
    if (leading && isFirstCall.current) {
      callback();
      isFirstCall.current = false;
    }
    if (trailing) {
      pendingTrailingCall.current = true;
      reset();
    }
  }, dependencies);

  useEffect(() => {
    return () => {
      clear();
      isFirstCall.current = true;
      pendingTrailingCall.current = false;
    };
  }, []);
}