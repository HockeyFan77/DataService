import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';

import { GlobalApiContextProvider } from './api/GlobalApiContextProvider.tsx';

import './index.css';

import App from './App.tsx';

const queryClient = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <GlobalApiContextProvider>
        <App />
        <ReactQueryDevtools initialIsOpen={false}/>
      </GlobalApiContextProvider>
    </QueryClientProvider>
  </StrictMode>,
);
