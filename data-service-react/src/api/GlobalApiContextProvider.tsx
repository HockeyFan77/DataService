import React, { useState, type ReactNode } from 'react';
import type { NullableApiContext } from './ApiContext';
import { GlobalApiContext } from './GlobalApiContext';

export const GlobalApiContextProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [context, setContext] = useState<NullableApiContext>(null);

  return (
    <GlobalApiContext.Provider value={{ context, setContext }}>
      {children}
    </GlobalApiContext.Provider>
  );
};