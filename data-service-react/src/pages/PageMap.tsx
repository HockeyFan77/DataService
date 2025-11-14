import type { JSX } from 'react';
import HomePage from './_HomePage';
import FirstPage from './FirstPage';
import WeatherForecastsPage from './WeatherForecastsPage';
import CompaniesPage from './CompaniesPage';
import DbObjectsPage from './DbObjectsPage';
import DbObjectPage from './DbObjectPage';
import UseQueryParamsCITestPage from './UseQueryParamsCITestPage';

const pageDefs = [
  {
    key: 'home',
    title: 'Home',
    content: <HomePage />,
  },
  {
    key: 'page1',
    title: 'Vite + React',
    content: <FirstPage />,
  },
  {
    key: 'weather',
    title: 'Weather Forecasts',
    content: <WeatherForecastsPage />,
  },
  {
    key: 'companies',
    title: 'Companies',
    content: <CompaniesPage />,
  },
  {
    key: 'dbobjects',
    title: 'Database Objects',
    content: <DbObjectsPage />,
  },
  {
    key: 'dbobject',
    title: 'Database Object',
    content: <DbObjectPage />,
  },
  {
    key: 'testUseQueryParamsCI',
    title: 'useQueryParamsCI Test',
    content: <UseQueryParamsCITestPage />,
  },
] as const;

export interface PageDef {
  key: string;
  title: string;
  content: JSX.Element;
}

type PageDefKey = typeof pageDefs[number]['key']; // 'home' | 'dbobjects'
type PageMapType = Map<PageDefKey, PageDef>;

export const pageMap: PageMapType = new Map<PageDefKey, PageDef>(
  pageDefs.map(def => [def.key, def])
);