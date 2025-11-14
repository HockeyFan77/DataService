# React + TypeScript + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh

## Expanding the ESLint configuration

If you are developing a production application, we recommend updating the configuration to enable type-aware lint rules:

```js
export default tseslint.config({
  extends: [
    // Remove ...tseslint.configs.recommended and replace with this
    ...tseslint.configs.recommendedTypeChecked,
    // Alternatively, use this for stricter rules
    ...tseslint.configs.strictTypeChecked,
    // Optionally, add this for stylistic rules
    ...tseslint.configs.stylisticTypeChecked,
  ],
  languageOptions: {
    // other options...
    parserOptions: {
      project: ['./tsconfig.node.json', './tsconfig.app.json'],
      tsconfigRootDir: import.meta.dirname,
    },
  },
})
```

You can also install [eslint-plugin-react-x](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-x) and [eslint-plugin-react-dom](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-dom) for React-specific lint rules:

```js
// eslint.config.js
import reactX from 'eslint-plugin-react-x'
import reactDom from 'eslint-plugin-react-dom'

export default tseslint.config({
  plugins: {
    // Add the react-x and react-dom plugins
    'react-x': reactX,
    'react-dom': reactDom,
  },
  rules: {
    // other rules...
    // Enable its recommended typescript rules
    ...reactX.configs['recommended-typescript'].rules,
    ...reactDom.configs.recommended.rules,
  },
})
```

# AdventureWorks Database
[AdventureWorks ER Diagram](https://www.dbdiagrams.com/online-diagrams/adventureworks/)

# Adding in TanStack Query (React Query) + Axios

## TanStack Query
[TanStack Query](https://tanstack.com/query/latest/docs/framework/react/)

## Axios
[Axios](https://axios-http.com/docs/intro/)

## WebDev Simplified
[Learn React Query In 50 Minutes](https://www.youtube.com/watch?v=r8Dg0KVnfMA&t)


# React Component Lifecycle

+----------------------------------+
|          Start                   |
|  (Initial Mount Trigger)         |
+-----------------+----------------+
                  |
                  v
+----------------------------------+
| 1. React calls component         | <--- This is the "render" phase:
|    function: MyCoolPage()        |      - Hooks (`useState`, `useEffect`) run
|    (calculate JSX output)        |      - State values are preserved internally
+-----------------+----------------+
                  |
                  v
+----------------------------------+
| 2. React diffs virtual DOM       |
|    and updates real DOM          |
+-----------------+----------------+
                  |
                  v
+----------------------------------+
| 3. Browser paints updates        |
+-----------------+----------------+
                  |
                  v
+----------------------------------+
| 4. React runs `useEffect()`      |
|    callbacks (side effects)      |
+-----------------+----------------+
                  |
                  v
+----------------------------------+
|         Idle / Wait              |
|  (User Interaction, etc.)        |
+-----------------+----------------+
                  |
                  v
+----------------------------------+
| 5. State update or new props     | <--- This triggers an Update cycle
+-----------------+----------------+
                  |
                  v
+----------------------------------+
| Repeat steps 1-4:                |
| - Component re-renders           |
| - Effects re-run if deps changed |
+-----------------+----------------+
                  |
                  v
+----------------------------------+
| 6. Component unmounts            | <--- Happens when component is removed
+-----------------+----------------+
                  |
                  v
+----------------------------------+
| React runs cleanup from last     |
| `useEffect` (return fn)          |
+-----------------+----------------+
                  |
                  v
                [End]
