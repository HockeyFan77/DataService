import { useNavigate } from 'react-router-dom';

import { Button } from 'primereact/button';
import { Splitter, SplitterPanel } from 'primereact/splitter';

import { pageMap } from './PageMap';

export default function HomePage() {
  const navigate = useNavigate();

  return (
    <div className="flex flex-1 flex-col">
      <Splitter layout="horizontal" className="flex-1">
        <SplitterPanel size={30} minSize={10}>
          <div className="p-2">
            <h2 className="mb-2">Navigation</h2>
            <ul className="space-y-2">
              {[...pageMap.values()].map(pageDef => (
                <li key={pageDef.key}>
                  <Button link={true} label={pageDef.title} onClick={() => navigate(`/${pageDef.key}`)} />
                </li>
              ))}
            </ul>
          </div>
        </SplitterPanel>
        <SplitterPanel>
          <div className="p-4 h-full text-gray-400">Select a page.</div>
        </SplitterPanel>
      </Splitter>
    </div>
  );

}