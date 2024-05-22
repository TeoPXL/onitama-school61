function applyTheme() {
    const themeClass = localStorage.getItem('toggle-lightmode') === 'true' ? 'light' : 'dark';
    document.documentElement.className = themeClass;
}


document.addEventListener('DOMContentLoaded', applyTheme);
