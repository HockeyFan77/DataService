// https://www.youtube.com/watch?v=0c6znExIqRw&list=PLZlA0Gpn_vH-aEDXnaFNLsqiJWFpIWV03

import { useState } from "react";

export default function useToggle(defaultValue: boolean): [boolean, (value?: boolean) => void] {
  const [value, setValue] = useState<boolean>(defaultValue);

  function toggleValue(value?: boolean): void {
    setValue(currentValue => typeof value === 'boolean' ? value : !currentValue);
  }

  return [value, toggleValue];
}