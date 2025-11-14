import { useEffect, useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
//import { useSearchParams } from 'react-router-dom';
import { StringParam } from 'use-query-params';
import { useQueryParamsCI } from '../hooks/useQueryParamsCI';
import { useQuery } from '@tanstack/react-query';

import { Button } from 'primereact/button';
import { Column } from 'primereact/column';
import { DataTable, type DataTableSelectionSingleChangeEvent } from 'primereact/datatable';
import { Dialog } from 'primereact/dialog';
import { FloatLabel } from 'primereact/floatlabel';
import { InputSwitch } from 'primereact/inputswitch';
import { InputText } from 'primereact/inputtext';
import { MultiSelect } from 'primereact/multiselect';
import { Splitter, SplitterPanel } from 'primereact/splitter';

import { apiFetch } from '../api/apiFetch';

//import type { NullableApiContext } from '../api/ApiContext';
import { isValidApiContext } from "../api/api-contexts";
import { ApiContextRegistry } from '../api/ApiContextRegistry';
import { useSyncedApiContext } from '../api/useSyncedApiContext';
import { ApiContextDropdown } from '../components/ApiContextDropdown';
import { PageContainer } from './PageContainer';

type DbDatabase = {
  abbr: string;
  name: string;
};
type DbObject = {
  db: string;
  sch: string;
  name: string;
  id: number;
  type: string;
  crdate: string;
  moddate: string;
  bdb: string;
  bsch: string;
  bname: string;
}
type DbObjectType = {
  abbr: string;
  name: string;
};
const DB_OBJECT_TYPES: DbObjectType[] = [
  { abbr: 'F', name: 'Function' },
  { abbr: 'G', name: 'Trigger' },
  { abbr: 'P', name: 'Procedure' },
  { abbr: 'T', name: 'Table' },
  { abbr: 'V', name: 'View' },
];

export default function DbObjectsPage() {

  const { context: selectedContext, setContext: setSelectedContext } = useSyncedApiContext();
  const [selectedDatabases, setSelectedDatabases] = useState<DbDatabase[]>([]);
  const [objNameSearch, setObjNameSearch] = useState('');
  const [selectedObjTypes, setSelectedObjTypes] = useState<DbObjectType[]>(DB_OBJECT_TYPES);
  const [objColumnSearch, setObjColumnSearch] = useState('');
  const [objTextSearch, setObjTextSearch] = useState('');
  const [objExactSearch, setObjExactSearch] = useState(false);

  const { params: queryParams/*, setQueryParams*/ } = useQueryParamsCI({
    context: StringParam,
  });
  const contextParam = (queryParams.context ?? '').toLowerCase();
  const urlContext = ApiContextRegistry.getByKeyOrNone(contextParam);
  const isValidContext = isValidApiContext(selectedContext);

  const location = useLocation();
  const pageUrl = `${window.location.origin}${location.pathname}${location.search}`;

  const dbDatabasesQuery = useQuery<DbDatabase[]>({
    queryKey: ['dbdatabases'],
    queryFn: async () => {
      const response = await apiFetch('dbdatabases', {
        context: selectedContext?.key
      });
      return response.data?.databases as DbDatabase[];
    },
    enabled: isValidContext,
  });
  const dbDatabases = dbDatabasesQuery?.data || [];
  const dbNameToAbbr = Object.fromEntries(dbDatabases.map(db => [db.name, db.abbr]));
  const getDbAbbr = (name: string): string => dbNameToAbbr[name] || '';

  const dbObjectsQuery = useQuery<DbObject[]>({
    queryKey: ['dbobjects'],
    queryFn: async () => {
      const objTypes = selectedObjTypes.map((t) => t.abbr).join(';') || 'F;G;P;T;V';
      const dbAbbrs = selectedDatabases.map((db) => db.abbr).join(';');
      const response = await apiFetch('dbobjects', {
        context: selectedContext?.key,
        params: {
          searchdbs: dbAbbrs,
          objnamesearch: objNameSearch,
          objtypes: objTypes,
          objcolumnsearch: objColumnSearch,
          objtextsearch: objTextSearch,
          objexactsearch: objExactSearch ? 1 : 0
        }
      });
      return response.data?.dbobjects as DbObject[];
    },
    enabled: false,
  });
  const dbObjects = dbObjectsQuery?.data || [];

  const [selectedDbObject, setSelectedDbObject] = useState<DbObject | null>(null);
  const [dialogVisible, setDialogVisible] = useState(false);

  const isRefreshEnabled = isValidContext && !dbDatabasesQuery.isLoading && !dbObjectsQuery.isLoading && objNameSearch !== '';

  useEffect(() => {
    if (urlContext && (!selectedContext || selectedContext.key !== urlContext.key)) {
      setSelectedContext(urlContext);
    }
  }, [contextParam]);

  useEffect(() => {
    if (!isValidContext) {
      setSelectedDatabases([]);
    }
  }, [selectedContext]);

  // useEffect(() => {
  //   if (!loading && companyId && companies.length > 0) {
  //     const found = companies.find(c => String(c.id) === companyId);
  //     setSelectedCompany(found ?? null);
  //   }
  // }, [loading, companyId, companies]);

  //setQueryParams({ context: 'qa' }, 'replaceIn'); // or 'pushIn' if you want browser history entries

  const objNameAndLinkBodyTemplate = (dbobj: DbObject) => {
    const dbabbr = getDbAbbr(dbobj.db);
    return (
      <Link to={`/dbobject?context=${selectedContext?.key}&objdb=${dbabbr}&objid=${dbobj.id}`} className="text-primary hover:underline flex items-center gap-2">
        <i className="pi pi-link"/>
        {dbobj.name}
      </Link>
    );
  };

  return (
    <PageContainer>
      <Splitter layout="horizontal" className="flex-1 min-h-0 w-full h-full">
        <SplitterPanel size={20} minSize={10} className="min-w-1/6 max-w-3/10 overflow-hidden">
          <div className="p-4 h-full w-full flex flex-col gap-4">

            <div className="pt-4 w-full overflow-hidden">
              <ApiContextDropdown
                className="w-full"
                selectedContext={selectedContext}
                onSelectedContextChange={setSelectedContext}
                includeNone/>
            </div>
            <div className="mt-4 pt-4 w-full overflow-hidden">
              <FloatLabel>
                <MultiSelect inputId="dbdatabases" className="w-full"
                  display="chip" showClear showSelectAll
                  placeholder="Select databases to search"
                  selectAllLabel="Select all"
                  value={selectedDatabases}
                  disabled={!isValidContext}
                  loading={dbDatabasesQuery.isLoading}
                  options={dbDatabases}
                  optionLabel="abbr"
                  dataKey="abbr"
                  onChange={(e) => setSelectedDatabases(e.value)}
                  itemTemplate={(item: DbDatabase | undefined) => item ? <div>{`${item.abbr} = ${item.name}`}</div> : null}/>
                <label htmlFor="dbdatabases">Databases to search</label>
              </FloatLabel>
            </div>
            <div className="mt-4 pt-4 w-full">
              <FloatLabel>
                <InputText id="objnamesearch" className="w-full"
                  placeholder="Object name search text"
                  value={objNameSearch}
                  onChange={(e) => setObjNameSearch(e.target.value)}/>
                <label htmlFor="objnamesearch">Object name search text</label>
              </FloatLabel>
            </div>
            <div className="mt-4 pt-4 w-full overflow-hidden">
              <FloatLabel>
                <MultiSelect inputId="objtypes" className="w-full"
                  display="chip" showClear showSelectAll
                  placeholder="Select object types to search"
                  selectAllLabel="Select all"
                  value={selectedObjTypes}
                  options={DB_OBJECT_TYPES}
                  optionLabel="abbr"
                  dataKey="abbr"
                  onChange={(e) => setSelectedObjTypes(Array.isArray(e.value) ? e.value : [])}
                  itemTemplate={(item: DbObjectType | undefined) => item ? <div>{`${item.abbr} = ${item.name}`}</div> : null}/>
                <label htmlFor="objtypes">Object types to search</label>
              </FloatLabel>
            </div>
            <div className="mt-4 pt-4 w-full">
              <FloatLabel>
                <InputText id="objcolumnsearch" className="w-full"
                  placeholder="Column name search text"
                  value={objColumnSearch}
                  onChange={(e) => setObjColumnSearch(e.target.value)}/>
                <label htmlFor="objcolumnsearch">Column name search text</label>
              </FloatLabel>
            </div>
            <div className="mt-4 pt-4 w-full">
              <FloatLabel>
                <InputText id="objtextsearch" className="w-full"
                  placeholder="Object text search text"
                  value={objTextSearch}
                  onChange={(e) => setObjTextSearch(e.target.value)}/>
                <label htmlFor="objtextsearch">Object text search text</label>
              </FloatLabel>
            </div>
            <div className="mt-4 pt-4 flex items-center justify-between w-full">
              <label htmlFor="objexactsearch" className="whitespace-nowrap">Exact match on search text</label>
              <InputSwitch inputId="objexactsearch" className="w-full"
                checked={objExactSearch}
                onChange={(e) => setObjExactSearch(e.value)}/>
            </div>

            <div className="mt-6">
              <Button label="Refresh" icon="pi pi-refresh" disabled={!isRefreshEnabled} onClick={() => dbObjectsQuery.refetch()}/>
            </div>

          </div>
        </SplitterPanel>
        <SplitterPanel size={80} minSize={30}>
          <div className="p-2 flex flex-col h-full w-full">
            <h1 className="mb-2">DB Objects</h1>
            <a href={pageUrl} target="_blank" rel="noopener noreferrer">
              Open in new tab
            </a>
            {dbObjectsQuery.isError ? (
              <div style={{color: 'red' }}>Error loading database objects: {dbObjectsQuery.error instanceof Error ? dbObjectsQuery.error.message : String(dbObjectsQuery.error)}</div>
            ) : (
              <div className="flex-1 min-h-0">
                <DataTable
                  className="w-full h-full"
                  value={dbObjects}
                  loading={dbObjectsQuery.isLoading}
                  size="small"
                  scrollable
                  scrollHeight="flex"
                  selectionMode="single"
                  selection={selectedDbObject}
                  onSelectionChange={(e: DataTableSelectionSingleChangeEvent<DbObject[]>) => {
                    setSelectedDbObject(e.value as DbObject | null);
                    setDialogVisible(true);
                  }}
                  dataKey="id">
                  <Column field="db" header="Database"/>
                  <Column field="sch" header="Schema"/>
                  <Column body={objNameAndLinkBodyTemplate} header="Name"/>
                  <Column field="id" header="ID"/>
                  <Column field="type" header="Type"/>
                  <Column field="crdate" header="Created"/>
                  <Column field="moddate" header="Modified"/>
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
          {selectedDbObject && (
            <div>
              <p>ID: {selectedDbObject.id}</p>
              <p>Name: {selectedDbObject.name}</p>
            </div>
          )}
      </Dialog>
    </PageContainer>
  );

}