import { useState } from 'react';
//import { useSearchParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';

import { Button } from 'primereact/button';
import { Column } from 'primereact/column';
import { DataTable, type DataTableSelectionSingleChangeEvent } from 'primereact/datatable';
import { Dialog } from 'primereact/dialog';
import { Splitter, SplitterPanel } from 'primereact/splitter';

import { PageContainer } from './PageContainer';

import { apiFetch } from '../api/apiFetch';

//import type { ApiContext } from '../api/ApiContext';
import { useSyncedApiContext } from '../api/useSyncedApiContext';
import { ApiContextDropdown } from '../components/ApiContextDropdown';

import type { Company } from '../models/Company';

export default function CompaniesPage() {

  const { context: selectedContext, setContext: setSelectedContext } = useSyncedApiContext();
  const { data, isLoading, isError, error, refetch } = useQuery<Company[]>({
    queryKey: ['companies'],
    queryFn: async () => {
      const response = await apiFetch('/companies', { context: selectedContext?.key });
      return response.data?.companies as Company[];
    },
    enabled: false, // prevent fetching during mount
  });

  const [selectedCompany, setSelectedCompany] = useState<Company | null>(null);
  // const [searchParams] = useSearchParams();
  // const companyId = searchParams.get('companyId');
  const [dialogVisible, setDialogVisible] = useState(false);

  // useEffect(() => {
  //   if (!loading && companyId && companies.length > 0) {
  //     const found = companies.find(c => String(c.id) === companyId);
  //     setSelectedCompany(found ?? null);
  //   }
  // }, [loading, companyId, companies]);

  // to disable the Refresh button when a context is not selected
  // disabled={!selectedContext?.context}

  return (
    <PageContainer>
      <Splitter layout="horizontal" className="flex-1 min-h-0 w-full h-full">
        <SplitterPanel size={20} minSize={10}>
          <div className="p-2 h-full w-full">
            <ApiContextDropdown
              className="w-full"
              selectedContext={selectedContext}
              onSelectedContextChange={setSelectedContext}
              includeNone/>
            <div className="mt-2">
              <Button label="Refresh" icon="pi pi-refresh" onClick={() => refetch()}/>
            </div>
          </div>
        </SplitterPanel>
        <SplitterPanel size={80} minSize={30}>
          <div className="p-2 flex flex-col h-full w-full">
            <h1 className="mb-2">Companies</h1>
            {isError ? (
              <div style={{color: 'red' }}>Error loading companies: {error instanceof Error ? error.message : String(error)}</div>
            ) : (
              <div className="flex-1 min-h-0">
                <DataTable
                  className="w-full h-full"
                  value={data ?? []}
                  loading={isLoading}
                  size='small'
                  scrollable
                  scrollHeight="flex"
                  selection={selectedCompany}
                  onSelectionChange={(e: DataTableSelectionSingleChangeEvent<Company[]>) => {
                    setSelectedCompany(e.value as Company | null);
                    setDialogVisible(true);
                  }}
                  selectionMode="single"
                  dataKey="id">
                  <Column field="id" header="ID"/>
                  <Column field="name" header="Name"/>
                  <Column field="earnings" header="Earnings" body={row => row.earnings !== undefined ? `$${row.earnings.toLocaleString()}` : ''}/>
                </DataTable>
              </div>
            )}
          </div>
        </SplitterPanel>
      </Splitter>

      <Dialog
        header="Details"
        visible={dialogVisible}
        onHide={() => setDialogVisible(false)}>
          {selectedCompany && (
            <div>
              <p>ID: {selectedCompany.id}</p>
              <p>ID: {selectedCompany.name}</p>
            </div>
          )}
        </Dialog>
    </PageContainer>
  );

}