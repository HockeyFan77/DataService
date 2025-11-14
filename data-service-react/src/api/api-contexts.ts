import type { ApiContext, NullableApiContext } from "./ApiContext";

export const API_CONTEXT_KEY_NONE = "-none-";
export const API_CONTEXT_NONE: ApiContext = { key: API_CONTEXT_KEY_NONE, name: "None" };
export const API_CONTEXT_DEV: ApiContext = { key: "dev", name: "Development" };
export const API_CONTEXT_QA: ApiContext = { key: "qa", name: "QA / Testing" };
export const API_CONTEXT_PROD: ApiContext = { key: "prod", name: "Production" };

export const API_CONTEXTS: ApiContext[] = [
  API_CONTEXT_NONE,
  API_CONTEXT_DEV,
  API_CONTEXT_QA,
  API_CONTEXT_PROD
];

export const isValidApiContext = (context: NullableApiContext): boolean => !!(context && context.key !== API_CONTEXT_KEY_NONE);