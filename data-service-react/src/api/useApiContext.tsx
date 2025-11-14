import { useState } from "react";
import type { NullableApiContext } from "./ApiContext";

export function useApiContext(initialContext: NullableApiContext) {
  const [context, setContext] = useState<NullableApiContext>(initialContext);
  return { context, setContext};
}