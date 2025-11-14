// https://www.youtube.com/watch?v=vrIxu-kfAUo&list=PLZlA0Gpn_vH-aEDXnaFNLsqiJWFpIWV03&index=3

import { useCallback, useEffect, useState } from "react";

export default function useAsync(callback: () => void, dependencies = []) {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState();
  const [value, setValue] = useState();

  const callbackMemoized = useCallback(() => {
    setLoading(true);
    setError(undefined);
    setValue(undefined);
    callback()
      .then(setValue)
      .catch(setError)
      .finally(() => setLoading(false));
  }, dependencies);

  useEffect(() => {
    callbackMemoized();
  }, [callbackMemoized]);

  return [ loading, error, value ];
}