import { useId } from 'react';
import { Dropdown, type DropdownProps } from 'primereact/dropdown';
import { FloatLabel } from 'primereact/floatlabel';

import type { NullableApiContext } from '../api/ApiContext';
import { ApiContextRegistry } from '../api/ApiContextRegistry';

type ApiContextDropdownProps = Omit<DropdownProps, 'value' | 'options' | 'onChange' | 'optionLabel' | 'dataKey'> & {
  selectedContext: NullableApiContext;
  onSelectedContextChange?: (context: NullableApiContext) => void;
  includeNone?: boolean;
};

export function ApiContextDropdown({
  selectedContext,
  onSelectedContextChange,
  includeNone = false,
  ...props
}: ApiContextDropdownProps) {
  const options = includeNone ? ApiContextRegistry.getAll() : ApiContextRegistry.getAllButNone();
  const dropdownId = useId();

  return (
    <FloatLabel>
      <Dropdown
        inputId={dropdownId}
        dataKey="key"
        value={selectedContext}
        options={options}
        onChange={e => onSelectedContextChange?.(e.value)}
        optionLabel="name"
        placeholder="Select Context"
        {...props}
      />
      <label htmlFor={dropdownId}>Select a context</label>
    </FloatLabel>
  );
}