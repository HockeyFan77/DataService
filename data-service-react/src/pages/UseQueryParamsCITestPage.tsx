import { useState, type JSX } from 'react';
import { ArrayParam, NumberParam, StringParam } from 'use-query-params';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { MultiSelect } from 'primereact/multiselect';
import { Panel } from 'primereact/panel';
import { SelectButton } from 'primereact/selectbutton';
import { useQueryParamsCI } from '../hooks/useQueryParamsCI';
import { PageContainer } from './PageContainer';

// Helper to display params as JSON
function ParamViewer({ label, value }: { label: string; value: any }) {
  return (
    <div className="my-2">
      <strong>{label}</strong>
      <pre className="p-2 text-sm overflow-auto">
        {JSON.stringify(value, null, 2)}
      </pre>
    </div>
  );
}

function ExampleCaseInsensitive({ active = false }: { active?: boolean }) {
  const { params, setParams } = useQueryParamsCI({
    userid: { queryParamConfig: NumberParam },
  });

  return (
    <Panel header="Case-Insensitive Parsing" hidden={!active}>
      <Button label="Increment UserId" onClick={() => setParams({ userid: (params.userid ?? 0) + 1 })} />
      <ParamViewer label="params" value={params} />
    </Panel>
  );
}

function ExampleConflictVariants({ active = false }: { active?: boolean }) {
  const { params, setParams } = useQueryParamsCI({
    foo: { queryParamConfig: NumberParam },
  });

  return (
    <Panel header="Remove Case Conflicts" hidden={!active}>
      <Button label="Set foo = 123" onClick={() => setParams({ foo: 123 })} />
      <ParamViewer label="params" value={params} />
    </Panel>
  );
}

function ExampleMixedParams({ active = false }: { active?: boolean }) {
  const { params, setParams } = useQueryParamsCI({
    user: { queryParamConfig: StringParam },
    token: { queryParamConfig: StringParam },
  });

  return (
    <Panel header="Mixed Param Keys" hidden={!active}>
      <Button label="Set user = &quot;updated&quot;" onClick={() => setParams({ user: 'updated' })} />
      <ParamViewer label="params" value={params} />
    </Panel>
  );
}

function ExampleRoundtrip({ active = false }: { active?: boolean }) {
  const { params, setParams } = useQueryParamsCI({
    query: { queryParamConfig: StringParam },
  });
  const [localInput, setLocalInput] = useState('');

  return (
    <Panel header="Roundtrip Test" hidden={!active}>
      <InputText
        value={localInput}
        onChange={(e) => setLocalInput(e.target.value)}
        placeholder="Enter query value" />
      <Button label="Set Query" onClick={() => setParams({ query: localInput })} />
      <ParamViewer label="params" value={params} />
    </Panel>
  );
}

function ExampleMultiValue({ active = false }: { active?: boolean }) {
  const { params, setParams } = useQueryParamsCI({
    tags: { queryParamConfig: ArrayParam },
  });

  const tagOptions = ['books', 'electronics', 'toys', 'clothing'];

  return (
    <Panel header="Multi-Value Params (Array)" hidden={!active}>
      <MultiSelect
        className="w-full mb-2"
        placeholder="Select tags"
        options={tagOptions}
        value={params.tags ?? []}
        onChange={(e) => setParams({ tags: e.value })}/>
      <ParamViewer label="params" value={params} />
    </Panel>
  );
}

export default function UseQueryParamsCITestPage() {

  const { params, setParams } = useQueryParamsCI({
    panel: { queryParamConfig: StringParam, syncMode: 'live' },
  });
  const activeExample = params.panel ?? 'case';
  const changeExample = (value: string) => setParams({ panel: value });
  const resetAll = () => setParams({ panel: params.panel }, 'replace');

  const examples: Record<string, (props: { active: boolean }) => JSX.Element> = {
    case: ({ active }) => <ExampleCaseInsensitive active={active} />,
    conflict: ({ active }) => <ExampleConflictVariants active={active} />,
    mixed: ({ active }) => <ExampleMixedParams active={active} />,
    roundtrip: ({ active }) => <ExampleRoundtrip active={active} />,
    multivalue: ({ active }) => <ExampleMultiValue active={active} />,
  };

  return (
    <PageContainer>
      <Panel header="useQueryParamsCI Test Page" className="mb-2">
        <div className="flex flex-col sm:flex-row items-start sm:items-center gap-2">
          <SelectButton
            allowEmpty={false}
            value={activeExample}
            onChange={(e) => changeExample(e.value)}
            options={Object.keys(examples)} />
          <Button label="Reset All" onClick={resetAll} severity="secondary" />
        </div>
      </Panel>
      {Object.entries(examples).map(([key, ExampleComponent]) => (
        <ExampleComponent key={key} active={activeExample === key} />
      ))}
   </PageContainer>
  );

}