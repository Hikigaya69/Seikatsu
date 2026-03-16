/** @type {import('tailwindcss').Config} */
export default {

  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}"
  ],

  theme: {
    extend: {

      colors: {

        graphite: "#353535",
        stormy_teal: "#3c6e71",
        yale_blue: "#284b63",
        alabaster_grey: "#d9d9d9",

      },

    },
  },

  plugins: [],
};