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
                sans: ['Cairo', 'sans-serif'],
            },
            colors: {
                background: '#DFDFDF',
                backgroundDivider: '#E9E9E9',
                layoutDivPrimary: '#8AC4AE',
                layoutDivSecondary: '#9BC0BC',
                primaryButtons: '#248277',
                blackText: '#000000'

            },
        },
    },
    plugins: [
        require('flowbite/plugin')
    ],

}