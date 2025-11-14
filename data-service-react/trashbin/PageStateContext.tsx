import type { ReactNode } from 'react';
import { createContext, useContext, useState } from 'react';

import { Button } from 'primereact/button';
import { Splitter, SplitterPanel } from 'primereact/splitter';

import type { PageDef } from '../pages/PageMap';
import { pageMap } from '../pages/PageMap';

export interface PageStateContextType {
  page: string;
  setPage: (page: string) => void;
  pageDef: PageDef | undefined;
}

const PageStateContext = createContext<PageStateContextType | undefined>(undefined);

export function usePageState() {
  const ctx = useContext(PageStateContext);
  if (!ctx) throw new Error('usePageState must be used within PageStateContext.Provider');
  return ctx;
}

export function PageStateProvider({ children }: { children: ReactNode }) {
  const [page, setPage] = useState('home');
  const pageDef = page === 'home'
    ? { key: 'home', title: 'Home', content: <HomeContent onNavigate={setPage} /> }
    : pageMap.get(page);

  return (
    <PageStateContext.Provider value={{
        page,
        setPage,
        pageDef
    }}>
      {children}
    </PageStateContext.Provider>
  );
}

function HomeContent({ onNavigate }: { onNavigate: (page: string) => void }) {

  return (
    <Splitter layout="horizontal" style={{ height: '100%' }}>
      <SplitterPanel size={30} minSize={10}>
        <div className="p-4">
          <h2 className="mb-2">Navigation</h2>
          <ul className="space-y-2">
            {
              [...pageMap.values()].map(pageDef => (
                <li key={pageDef.key}>
                  <Button label={pageDef.title} onClick={() => onNavigate(pageDef.key)} />
                </li>
              ))
            }
          </ul>
        </div>
      </SplitterPanel>
      <SplitterPanel>
        <div className="p-4 h-full text-gray-400">Select a page.</div>
      </SplitterPanel>
    </Splitter>
  );

}