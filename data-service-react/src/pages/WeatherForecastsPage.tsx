//import { useEffect, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
//import { useSearchParams } from 'react-router-dom';
import { apiFetch } from '../api/apiFetch';

import { Splitter, SplitterPanel } from 'primereact/splitter';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';

export default function WeatherForecastsPage() {

  // NOTE: Disabling eslint warnings for usage of the "any" type!!!!
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  // const [forecasts, setForecasts] = useState<any[]>([]);
  // const [loading, setLoading] = useState(true);
  // // e  slint - disable-next-line @typescript-eslint/no-explicit-any
  // const [selectedCompany, setSelectedCompany] = useState<any | null>(null);
  // const [searchParams] = useSearchParams();
  // const companyId = searchParams.get('companyId');

  // useEffect(() => {
  //   fetch(`/weatherforecast`)
  //     .then(res => res.json())
  //     .then(data => {
  //       setForecasts(data ?? []);
  //       setLoading(false);
  //     })
  //     .catch(() => setLoading(false));
  // }, []);

  // useEffect(() => {
  //   if (!loading && companyId && companies.length > 0) {
  //     const found = companies.find(c => String(c.id) === companyId);
  //     setSelectedCompany(found ?? null);
  //   }
  // }, [loading, companyId, companies]);

  const { data: forecastData, isLoading } = useQuery({
    queryKey: ["weather-forecasts"],
    staleTime: 10000,
    queryFn: () => apiFetch('weatherforecast'),
  });

  return (
    <div className="flex flex-1 flex-col">
      <Splitter layout="horizontal" className="flex-1">
        <SplitterPanel size={20} minSize={10}>
        <div className="p-4">Left Panel (Resizable)</div>
        <div>Parameters go here</div>
        </SplitterPanel>
        <SplitterPanel size={80} minSize={30}>
          <div className="p-4 h-full flex flex-col">
            <h1 className="mb-4">Companies</h1>
            <div className="flex-1 overflow-auto">
              <DataTable
                value={forecastData?.data}
                loading={isLoading}
                scrollable
                scrollHeight="flex"
                selectionMode="single">
                <Column field="date" header="Date"/>
                <Column field="summary" header="Summary"/>
                <Column field="temperatureC" header="Temperature (Celcius)"/>
                <Column field="temperatureF" header="temperature (Fahrenheit)"/>
              </DataTable>
            </div>
          </div>
        </SplitterPanel>
      </Splitter>
    </div>
  );

}