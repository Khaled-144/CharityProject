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
                layoutDivThird: '#C7D4D3',
                primaryButtons: '#248277',
                darkedButtons: '#165049',
                lightedButtons: '#79DACF',
                blackText: '#000000',
                spaceCadet: {
                    DEFAULT: '#222E50',
                    light: '#7489C3',
                    dark: '#111727',
                },
                tekhelet: {
                    DEFAULT: '#492484',
                    light: '#9E79DA',
                    dark: '#241242',
                },
                burgundy: {
                    DEFAULT: '#84242F',
                    light: '#DA7984',
                    dark: '#421218',
                },
                tealBlue: {
                    DEFAULT: '#007991',
                    light: '#00A7C9',
                    dark: '#005F73',
                },
                platinum: {
                    DEFAULT: '#DFDFDF',
                    light: '#EEEEEE',
                    dark: '#6F6F6F',
                },
                cambridgeBlue: {
                    DEFAULT: '#8AC4AE',
                    light: '#C4E1D6',
                    dark: '#386E59',
                },
                ashGray: {
                    DEFAULT: '#9BC0BC',
                    light: '#CDE0DE',
                    dark: '#436B67',
                },
                alert: {
                    blue: '#4A90E2',
                    green: '#50E3C2',
                    yellow: '#F5A623',
                    red: '#D0021B',
                },
            },
        },
    },
    plugins: [
        require('flowbite/plugin')
    ],

}
