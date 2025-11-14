import React, { createContext } from 'react';
import type { NullableApiContext } from './ApiContext';

interface ApiContextState {
  context: NullableApiContext;
  setContext: React.Dispatch<React.SetStateAction<NullableApiContext>>;
}
export const GlobalApiContext = createContext<ApiContextState | null>(null);