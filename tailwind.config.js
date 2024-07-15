module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml'
    ],
    theme: {
        extend: {
            fontFamily: {
                sans: ['Cairo', 'sans-serif'],
            },
            colors: {
                background: '#DFDFDF',
                backgroundDivider: '#E9E9E9',
                secondaryColor: '#8AC4AE',
                third: '#9BC0BC',
                'primaryColor': '#248277',
                mainText: '#000000'
            },
        },
    },
    plugins: [],
}