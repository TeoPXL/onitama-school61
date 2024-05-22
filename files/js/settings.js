document.addEventListener('DOMContentLoaded', function() {
    const settingInputs = document.querySelectorAll('.setting-input');

    settingInputs.forEach(element => {
        const setting = element.getAttribute('onitama-setting');
        const storedSetting = localStorage.getItem(setting);
        switch (setting) {
            case 'force-remote-api':
                element.checked = storedSetting === 'true';
                break;
            case 'force-dev-api':
                element.checked = storedSetting === 'true';
                break;
            case 'simulate-offline-mode':
                element.checked = storedSetting === 'true';
                break;
            case 'suppress-api-errors':
                element.checked = storedSetting === 'true';
                break;
            case 'suppress-all-errors':
                element.checked = storedSetting === 'true';
                break;
            case 'toggle-lightmode':
                element.checked = storedSetting === 'true';
                applyTheme();
                break;
        }
    });

    settingInputs.forEach(element => element.addEventListener('click', () => {
        const setting = element.getAttribute("onitama-setting");
        localStorage.setItem(setting, element.checked.toString());

        if (setting === 'toggle-lightmode') {
            applyTheme();
        }
    }));
});

function applyTheme() {
    const themeClass = localStorage.getItem('toggle-lightmode') === 'true' ? 'light' : 'dark';
    document.documentElement.className = themeClass;
}





