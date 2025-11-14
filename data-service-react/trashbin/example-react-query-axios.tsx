import { useQuery, QueryClient, QueryClientProvider } from '@tanstack/react-query';
import axios from 'axios';

import type { Company } from '../src/models/Company';

const queryClient = new QueryClient();

function Companies() {
  const { data, isLoading, error } = useQuery<Company[]>({
    queryKey: ['companies'],
    queryFn: async () => {
      const response = await axios.get('/api/companies');
      return response.data;
    },
  });

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <ul>
      {data?.map((company) => (
        <li key={company.id}>{company.name}</li>
      ))}
    </ul>
  );
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Companies />
    </QueryClientProvider>
  );
}