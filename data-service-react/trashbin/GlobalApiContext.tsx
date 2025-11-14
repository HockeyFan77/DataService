import { createContext } from 'react';
import type { ApiContext } from '../src/api/ApiContext';

export const GlobalApiContext = createContext<ApiContext | null>(null);