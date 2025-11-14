// https://www.youtube.com/watch?v=vrIxu-kfAUo&list=PLZlA0Gpn_vH-aEDXnaFNLsqiJWFpIWV03&index=3

import useAsync from "./useAsync";

const DEFAULT_OPTIONS = {
  headers: { "Content-Type": "application/json" },
};

export default function useFetch(url: string, options = {}, dependencies = []) {
  return useAsync(() => {
    return fetch(url, { ...DEFAULT_OPTIONS, ...options }).then(res => {
      if (res.ok) return res.json();
      return res.json().then(json => Promise.reject(json));
    })
  }, dependencies);
}