document.addEventListener('DOMContentLoaded', applyTheme);

function applyTheme() {
    let themeClass = 'dark'; // Default theme
    if (localStorage.getItem('toggle-metal') === 'true') {
        themeClass = 'metal';
    } else if (localStorage.getItem('toggle-sahara') === 'true') {
        themeClass = 'sahara';
    } else if (localStorage.getItem('toggle-aqua') === 'true') {
        themeClass = 'aqua';
    }
    document.documentElement.className = themeClass;
    console.log("Theme set to:", themeClass);
}