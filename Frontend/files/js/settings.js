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
            case 'toggle-dark':
                element.checked = storedSetting === 'true';
                applyTheme();
                break;
            case 'toggle-moon':
                element.checked = storedSetting === 'true';
                applyTheme();
                break;
            case 'toggle-sahara':
                element.checked = storedSetting === 'true';
                applyTheme();
                break;
            case 'toggle-aqua':
                element.checked = storedSetting === 'true';
                applyTheme();
                break;
        }
    });

    settingInputs.forEach(element => element.addEventListener('click', () => {
        const setting = element.getAttribute("onitama-setting");
        localStorage.setItem(setting, element.checked.toString());
        
        if (setting.startsWith('toggle')) {  
            settingInputs.forEach(input => {
                if (input !== element && input.getAttribute("onitama-setting").startsWith("toggle")) {
                    input.checked = false;
                    localStorage.setItem(input.getAttribute("onitama-setting"), 'false');
                }
            });
            applyTheme();
        }
    }));
});

function applyTheme() {
    let themeClass = 'dark'; 
    if (localStorage.getItem('toggle-moon') === 'true') {
        themeClass = 'moon';
    } else if (localStorage.getItem('toggle-sahara') === 'true') {
        themeClass = 'sahara';
    } else if (localStorage.getItem('toggle-aqua') === 'true') {
        themeClass = 'aqua';
    }
    document.documentElement.className = themeClass;
    console.log("Theme set to:", themeClass);
}





