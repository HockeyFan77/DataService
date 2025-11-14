import { useEffect, useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
//import { useSearchParams } from 'react-router-dom';
import { NumberParam, StringParam } from 'use-query-params';
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

export default function TemplatePage() {

  const { context: selectedContext, setContext: setSelectedContext } = useSyncedApiContext();
  const isValidContext = isValidApiContext(selectedContext);

  const [queryParams/*, setQueryParams*/] = useQueryParamsCI({
    context: StringParam,
    objdb: StringParam,
    objid: NumberParam
  });

  // const dbObjectDetailsQuery = useQuery<DbObjectDetails>({
  //   queryKey: ['dbobject'],
  //   queryFn: async () => {
  //     const response = await apiFetch('dbobject', {
  //       context: queryParams.context || '',
  //       params: {
  //         objdb: queryParams.objdb,
  //         objid: queryParams.objid,
  //         // objsch: 'dbo',
  //         // objname: 'awperson'
  //       }
  //     });
  //     return response.data?.dbobject as DbObjectDetails;
  //   },
  //   enabled: true, // only enable if params are valid TODO!!
  // });
  // const dbObjectDetails = dbObjectDetailsQuery?.data;

  const location = useLocation();
  const pageUrl = `${window.location.origin}${location.pathname}${location.search}`;

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

  //setQueryParams({ context: 'qa' }, 'replaceIn'); // or 'pushIn' if you want browser history entries

  return (
    <PageContainer>
      <div></div>
    </PageContainer>
  );

}