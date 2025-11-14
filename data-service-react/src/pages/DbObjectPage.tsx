import { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
//import { useSearchParams } from 'react-router-dom';
import { NumberParam, StringParam } from 'use-query-params';
import { useQueryParamsCI } from '../hooks/useQueryParamsCI';
import { useQuery } from '@tanstack/react-query';

import { Button } from 'primereact/button';
import { Column, type ColumnProps } from 'primereact/column';
import { DataTable, type DataTableProps, type DataTableValueArray, type DataTableSelectionSingleChangeEvent } from 'primereact/datatable';
import { Dialog } from 'primereact/dialog';
import { FloatLabel } from 'primereact/floatlabel';
import { InputSwitch } from 'primereact/inputswitch';
import { InputText } from 'primereact/inputtext';
import { MultiSelect } from 'primereact/multiselect';
import { Panel, type PanelProps } from 'primereact/panel';
import { Splitter, SplitterPanel } from 'primereact/splitter';

import { apiFetch } from '../api/apiFetch';

//import type { NullableApiContext } from '../api/ApiContext';
import { API_CONTEXT_DEV, isValidApiContext } from "../api/api-contexts";
import { ApiContextRegistry } from '../api/ApiContextRegistry';
import { useSyncedApiContext } from '../api/useSyncedApiContext';
import { ApiContextDropdown } from '../components/ApiContextDropdown';
import { PageContainer } from './PageContainer';

type DataTablePanelProps<T extends object = any> = {
  title: string;
  panelProps?: Partial<PanelProps>;
  rows?: T[] | null;
  columns: ColumnProps[];
  tableProps?: Partial<DataTableProps<T[]>>;
};

function DataTablePanel<T extends object = any>({
  title,
  panelProps = {},
  rows,
  columns,
  tableProps = {},
}: DataTablePanelProps<T>) {
  if (!rows || rows.length === 0)
    return null;

  return (
    <div className="flex flex-col mt-2">
      <Panel
        header={title}
        toggleable={panelProps.toggleable ?? true}
        collapsed={panelProps.collapsed ?? false}
        {...panelProps}
        className={`flex flex-col ${panelProps.className ?? ''}`}>
        <div className="min-h-[5rem] max-h-[15rem] overflow-y-auto m-1">
          <DataTable
            value={rows}
            size="small"
            scrollable
            scrollHeight="auto"
            {...(tableProps as DataTableProps<T[]>)}>
            {columns.map((col) => (
              <Column field="col.field" header="col.header" {...col}/>
            ))}
          </DataTable>
        </div>
      </Panel>
    </div>
  );
}

type DbObjectDetails = {
  db?: string,
  sch?: string,
  name?: string,
  id?: number,
  type?: string,
  crdate?: string,
  moddate?: string,
  rowcnt?: number,
  basedb?: string,
  basesch?: string,
  basename?: string,
  columns?: any[] | null,
  pkeys?: any[] | null,
  fkeys?: any[] | null,
  indexes?: any[] | null,
  constraints?: any[] | null,
  triggers?: any[] | null,
  refs?: any[] | null,
  permissions?: any[] | null,
};
export default function DbObjectPage() {

  const { context: selectedContext, setContext: setSelectedContext } = useSyncedApiContext();
  const isValidContext = isValidApiContext(selectedContext);

  const { params: queryParams/*, setQueryParams*/ } = useQueryParamsCI({
    context: StringParam,
    objdb: StringParam,
    objid: NumberParam
  });

  const dbObjectDetailsQuery = useQuery<DbObjectDetails>({
    queryKey: ['dbobject'],
    queryFn: async () => {
      const response = await apiFetch('dbobject', {
        context: queryParams.context || '',
        params: {
          objdb: queryParams.objdb,
          objid: queryParams.objid,
          // objsch: 'dbo',
          // objname: 'awperson'
        }
      });
      return response.data?.dbobject as DbObjectDetails;
    },
    enabled: true, // only enable if params are valid TODO!!
  });
  const dbObjectDetails = dbObjectDetailsQuery?.data;

  // const [selectedDbObject, setSelectedDbObject] = useState<DbObject | null>(null);
  // const [dialogVisible, setDialogVisible] = useState(false);

  // const isRefreshEnabled = isValidContext && !dbDatabasesQuery.isLoading && !dbObjectsQuery.isLoading && objNameSearch !== '';

  // useEffect(() => {
  //   if (urlContext && (!selectedContext || selectedContext.key !== urlContext.key)) {
  //     setSelectedContext(urlContext);
  //   }
  // }, [contextParam]);

  // useEffect(() => {
  //   if (!isValidContext) {
  //     setSelectedDatabases([]);
  //   }
  // }, [selectedContext]);

  // useEffect(() => {
  //   if (!loading && companyId && companies.length > 0) {
  //     const found = companies.find(c => String(c.id) === companyId);
  //     setSelectedCompany(found ?? null);
  //   }
  // }, [loading, companyId, companies]);

  //setQueryParams({ context: 'qa' }, 'replaceIn'); // or 'pushIn' if you want browser history entries

  return (
    <PageContainer>
      <div className="w-full">
        {ObjectHeader(dbObjectDetails)}
        <DataTablePanel
          title="Columns"
          rows={dbObjectDetails?.columns}
          columns={[
            { field: "id", header: "ID" },
            { field: "name", header: "Name" },
            { field: "type", header: "Type" },
            { field: "nullable", header: "Nullable?" },
            { field: "comment", header: "Comment" },
          ]}
          tableProps={{
            dataKey: "id"
          }}
        />
        <DataTablePanel
          title="Primary Keys"
          panelProps={{
            collapsed: true
          }}
          rows={dbObjectDetails?.pkeys}
          columns={[
            { field: "pkname", header: "PK Name" },
            { header: "Key Columns",
              body: (keyCols: any) => keyCols.key.map((k: { pkcolname: string }) => k.pkcolname).join(', ')
            }
          ]}
          tableProps={{
            dataKey: "pkname"
          }}
        />
        <DataTablePanel
          title="Foreign Keys"
          panelProps={{
            collapsed: true
          }}
          rows={dbObjectDetails?.fkeys}
          columns={[
            { field: "fkname", header: "FK Name" },
            { header: "Key Columns",
              body: (keyCols: any) => keyCols.key.map((
                k: { pkcolname: string }
              ) => k.pkcolname).join(', ')
            },
          ]}
          tableProps={{
            dataKey: "fkname"
          }}
        />
        <DataTablePanel
          title="Indexes"
          panelProps={{
            collapsed: true
          }}
          rows={dbObjectDetails?.indexes}
          columns={[
            { field: "ixname", header: "Index Name" },
            { field: "ixdesc", header: "Description" },
            { field: "ixkeys", header: "Keys" },
          ]}
          tableProps={{
            dataKey: "ixname"
          }}
        />
        <DataTablePanel
          title="Constraints"
          panelProps={{
            collapsed: true
          }}
          rows={dbObjectDetails?.constraints}
          columns={[
            { field: "name", header: "Constraint Name" },
            { field: "colname", header: "Column Name" },
            { field: "definition", header: "Definition" },
          ]}
          tableProps={{
            dataKey: "name"
          }}
        />
        <DataTablePanel
          title="Triggers"
          panelProps={{
            collapsed: true
          }}
          rows={dbObjectDetails?.triggers}
          columns={[
            { field: "trgobjid", header: "ID" },
            { field: "sch", header: "Schema" },
            { field: "name", header: "Name" },
            { field: "isdisabled", header: "Disabled" },
            { field: "isafter", header: "IsAfter" },
            { field: "isinsteadof", header: "IsInsteadOf" },
            { field: "isinsert", header: "IsInsert" },
            { field: "isinsertfirst", header: "IsInsertFirst" },
            { field: "isinsertlast", header: "IsInsertLast" },
            { field: "isupdate", header: "IsUpdate" },
            { field: "isupdatefirst", header: "IsUpdateFirst" },
            { field: "isupdatelast", header: "IsUpdateLast" },
            { field: "isdelete", header: "IsDelete" },
            { field: "isdeletefirst", header: "IsDeleteFirst" },
            { field: "isdeletelast", header: "IsDeleteLast" },
          ]}
          tableProps={{
            dataKey: "name"
          }}
        />
        <DataTablePanel
          title="References"
          panelProps={{
            collapsed: true
          }}
          rows={dbObjectDetails?.refs}
          columns={[
            { field: "refobjid", header: "ID" },
            { field: "refbdb", header: "Database" },
            { field: "refobjsch", header: "Schema" },
            { field: "refobjname", header: "Name" },
            { field: "refobjtype", header: "Type" },
            { field: "reftype", header: "Reference Type" },
          ]}
          tableProps={{
            dataKey: "refid"
          }}
        />
        <DataTablePanel
          title="Permissions"
          panelProps={{
            collapsed: true
          }}
          rows={dbObjectDetails?.permissions}
          columns={[
            { field: "owner", header: "Owner" },
            { field: "object", header: "Object" },
            { field: "grantee", header: "Grantee" },
            { field: "grantor", header: "Grantor" },
            { field: "protecttype", header: "Type" },
            { field: "action", header: "Action" },
            { field: "column", header: "Column" },
          ]}
          tableProps={{
            dataKey: "refid"
          }}
        />
      </div>
    </PageContainer>
  );

  function ObjectHeader(objDetails?: any) {
    return (
      <div>
        <Panel header="Object Details">
          <div className="grid grid-cols-2 gap-2 text-sm">
            <div><strong>Database:</strong> {objDetails?.db}</div>
            <div><strong>ID:</strong> {objDetails?.id}</div>
            <div><strong>Schema:</strong> {objDetails?.sch}</div>
            <div><strong>Name:</strong> {objDetails?.name}</div>
          </div>
        </Panel>
      </div>
    );
  }

}