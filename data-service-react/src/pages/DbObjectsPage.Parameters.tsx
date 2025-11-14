import { PageParametersPanel } from '../components/PageParametersPanel';
import { PageParameter } from '../components/PageParameter';

import { InputText } from 'primereact/inputtext';
import { InputNumber } from 'primereact/inputnumber';
import { Checkbox } from 'primereact/checkbox';

// export function MyPageParams({ filters, setFilters }) {
//   return (
//     <PageParametersPanel title="Search Filters">
//       <PageParameter label="Name" id="name">
//         <InputText
//           id="name"
//           value={filters.name}
//           onChange={(e) => setFilters((f) => ({ ...f, name: e.target.value }))}
//         />
//       </PageParameter>

//       <PageParameter label="Age" id="age">
//         <InputNumber
//           id="age"
//           value={filters.age}
//           onValueChange={(e) => setFilters((f) => ({ ...f, age: e.value ?? 0 }))}
//         />
//       </PageParameter>

//       <PageParameter label="Active" id="active">
//         <Checkbox
//           inputId="active"
//           checked={filters.active}
//           onChange={(e) => setFilters((f) => ({ ...f, active: e.checked }))}
//         />
//       </PageParameter>
//     </PageParametersPanel>
//   );
// }