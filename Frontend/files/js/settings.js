const settingInputs = document.querySelectorAll('.setting-input');
settingInputs.forEach(element => {
    switch (element.getAttribute('onitama-setting')) {
        case 'force-remote-api':
            if(userSettings['force-remote-api'] == 'true'){
                element.checked = true;
            }
            break;

        case 'force-dev-api':
            if(userSettings['force-dev-api'] == 'true'){
                element.checked = true;
            }
            break;
        
        case 'simulate-offline-mode':
            if(userSettings['simulate-offline-mode'] == 'true'){
                element.checked = true;
            }
            break;

        case 'suppress-api-errors':
            if(userSettings['suppress-api-errors'] == 'true'){
                element.checked = true;
            }
            break;

        case 'suppress-all-errors':
            if(userSettings['suppress-all-errors'] == 'true'){
                element.checked = true;
            }
            break;
        default:
            break;
    }
});
settingInputs.forEach(element => element.addEventListener('click', () => {
    const setting = element.getAttribute("onitama-setting");
    localStorage.setItem(setting, element.checked);
}));