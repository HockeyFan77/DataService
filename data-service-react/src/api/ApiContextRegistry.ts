import type { ApiContext, NullableApiContext } from "./ApiContext";
import { API_CONTEXT_KEY_NONE, API_CONTEXTS } from "./api-contexts";

class ApiContexts {
  private contextMap: Map<string, ApiContext> = new Map();

  constructor(contexts: ApiContext[]) {
    this.contextMap = new Map(contexts.map(ctx => [ctx.key.toLowerCase(), ctx]));
    Object.freeze(this.contextMap)
  }
  getByKey(key: string): NullableApiContext {
    return this.contextMap.get(key.toLowerCase()) || null;
  }
  getByKeyOrNone(key: string): NullableApiContext {
    return this.contextMap.get(key.toLowerCase()) || this.getNone();
  }
  getNone(): NullableApiContext {
    return this.getByKey(API_CONTEXT_KEY_NONE);
  }
  getAll(): NullableApiContext[] {
    return Array.from(this.contextMap.values());
  }
  getAllButNone(): NullableApiContext[] {
    return Array.from(this.contextMap.values()).filter(ctx => ctx.key !== API_CONTEXT_KEY_NONE);
  }
}

export const ApiContextRegistry = new ApiContexts(API_CONTEXTS);