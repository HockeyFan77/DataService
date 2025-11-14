// https://www.youtube.com/watch?v=vrIxu-kfAUo&list=PLZlA0Gpn_vH-aEDXnaFNLsqiJWFpIWV03&index=3

import { useCallback, useEffect, useState } from "react";

export function useLocalStorage<T>(key: string, storageValue: T | (() => T)) {
  if (typeof window === 'undefined') throw new Error('useLocalStorage can only be used in the browser.');
  return useStorage<T>(key, storageValue, window.localStorage);
}

export function useSessionStorage<T>(key: string, storageValue: T | (() => T)) {
  if (typeof window === 'undefined') throw new Error('useSessionStorage can only be used in the browser.');
  return useStorage<T>(key, storageValue, window.sessionStorage);
}

function useStorage<T>(key: string, storageValue: T | (() => T), storageObject: Storage): [T, React.Dispatch<React.SetStateAction<T>>, () => void] {
  const [value, setValue] = useState<T>(() => {
    const jsonValue = storageObject.getItem(key);
    if (jsonValue != null) return JSON.parse(jsonValue);
    if (typeof storageValue === 'function') {
      return (storageValue as () => T)();
    } else {
      return storageValue;
    }
  });

  useEffect(() => {
    if (value === undefined) return storageObject.removeItem(key);
    storageObject.setItem(key, JSON.stringify(value));
  }, [key, value, storageObject]);

  const remove = useCallback(() => {
    setValue(undefined as unknown as T);
  }, []);

  return [value, setValue, remove];
}