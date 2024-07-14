module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml'
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
    plugins: [],
}