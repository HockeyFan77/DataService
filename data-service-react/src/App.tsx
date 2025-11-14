import { BrowserRouter, Routes, Route, Navigate, Outlet, useNavigate } from 'react-router-dom';
import { QueryParamProvider } from 'use-query-params';
import { ReactRouter6Adapter } from 'use-query-params/adapters/react-router-6';

import { Button } from 'primereact/button';
import { Toolbar } from 'primereact/toolbar';

import { pageMap } from './pages/PageMap';
import { useSyncedApiContext } from './api/useSyncedApiContext';

function App() {
  return (
    <BrowserRouter basename="/app">
      <QueryParamProvider adapter={ReactRouter6Adapter}>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<Navigate to="/home" />} />
            {[...pageMap.values()].map(pageDef => (
              <Route key={pageDef.key} path={pageDef.key} element={pageDef.content} />
            ))}
            <Route path="*" element={<div className="p-4 text-gray-400">Page not found.</div>} />
          </Route>
        </Routes>
      </QueryParamProvider>
    </BrowserRouter>
  );
}

function Layout() {
  const navigate = useNavigate();

  const path = window.location.pathname.replace(/^\//, '') || 'home';
  const pageDef = pageMap.get(path) || { title: 'Home' };

  const { context: selectedContext } = useSyncedApiContext();

  return (
    <div className="flex flex-col min-h-screen h-screen w-screen overflow-hidden">
      <Toolbar start={
        <>
        <Button label="Home" onClick={() => navigate('/home')} />
        <span className="ml-4" id="page-title">{pageDef?.title ?? 'Page not found.'}</span>
        <span className="ml-4">{selectedContext?.name ?? 'No context selected.'}</span>
        </>
      }/>
      <div id="page-content" className="p-2 flex flex-1 flex-col min-h-0">
        <Outlet />
      </div>
    </div>
  );
}

export default App;