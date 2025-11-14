import { useContext, useEffect, useState } from 'react';
import type { NullableApiContext } from './ApiContext';
import { GlobalApiContext } from './GlobalApiContext';

export const useSyncedApiContext = () => {
  const global = useContext(GlobalApiContext);

  // Safety check
  if (!global) {
    throw new Error('useSyncedApiContext must be used within a GlobalApiContextProvider');
  }

  const { context: globalContext, setContext: setGlobalContext } = global;

  const [context, setContext] = useState<NullableApiContext>(globalContext);

  // Sync global -> local (on mount)
  useEffect(() => {
    setContext(globalContext);
  }, [globalContext]);

  // Sync local -> global (on update)
  useEffect(() => {
    if (context !== globalContext) {
      setGlobalContext(context);
    }
  }, [context]);

  return { context, setContext } as const;
};