module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './src/**/*.{js,ts,jsx,tsx}',
        "./node_modules/flowbite/**/*.js",
        './node_modules/@tailus/themer/dist/components/**/*.{js,ts}',
    ],
    theme: {
        extend: {
            fontFamily: {
                sans: ['Cairo', 'sans-serif'], // Use Cairo as your sans-serif font
            },
            colors: {
                background: '#DFDFDF',
                backgroundDivider: '#E9E9E9',
                secondary: '#8AC4AE',
                third: '#9BC0BC',
                primary: '#248277',
            },
        },
    },
    plugins: [
        require('flowbite/plugin')
    ],

}