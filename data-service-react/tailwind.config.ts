/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
  layerOrder: [
    'tailwind-base',
    'primereact',
    'tailwind-utilities'
  ]
}