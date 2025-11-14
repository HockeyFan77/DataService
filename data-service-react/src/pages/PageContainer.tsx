import { type ReactNode } from "react";

export function PageContainer({ children }: { children: ReactNode }) {
  return (
    <div className="flex flex-col flex-1 min-h-0 min-w-0 h-full w-full overflow-hidden">{children}</div>
  );
}