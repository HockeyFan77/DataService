import { useContext } from 'react';
import { FloatLabel } from 'primereact/floatlabel';
import { PageParametersContext } from './PageParametersPanel';

type PageParameterProps = {
  label: string;
  id: string;
  children: React.ReactElement;
};

export function PageParameter({ label, id, children }: PageParameterProps) {
  const context = useContext(PageParametersContext);

  if (!context) {
    throw new Error('<PageParameter> must be used inside <PageParametersPanel>');
  }

  return (
    <div className="field mb-3">
      <FloatLabel>
        {children}
        <label htmlFor={id}>{label}</label>
      </FloatLabel>
    </div>
  );
}