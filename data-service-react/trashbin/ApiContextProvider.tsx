// import { useState } from 'react';
// import type { ReactNode } from 'react';

// import { GlobalApiContext } from './GlobalApiContext';
// import type { ApiContext } from './ApiContext';

// export const ApiContextProvider: React.FC<{
//     apiContexts: ApiContext[];
//     children: ReactNode;
//   }> = ({ apiContexts, children }) => {
//   const [selectedContext, setSelectedContext] = useState<ApiContext | null>(null);

//   return (
//     <GlobalApiContext.Provider value={{ apiContexts, selectedContext, setSelectedContext }}>
//       {children}
//     </GlobalApiContext.Provider>
//   );
// };