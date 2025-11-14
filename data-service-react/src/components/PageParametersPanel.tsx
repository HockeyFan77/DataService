import { Panel } from 'primereact/panel';
import { createContext, type ReactNode } from 'react';

export const PageParametersContext = createContext<boolean | null>(null);

type PageParametersPanelProps = {
  title?: string;
  children: ReactNode;
};

export function PageParametersPanel({ title = 'Parameters', children }: PageParametersPanelProps) {
  return (
    <PageParametersContext.Provider value={true}>
      <Panel header={title} toggleable className="mb-3">
        <div className="p-fluid">{children}</div>
      </Panel>
    </PageParametersContext.Provider>
  );
}