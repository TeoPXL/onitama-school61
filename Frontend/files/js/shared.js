const localApi = "https://localhost:5051";
const remoteApi = "https://onitama61b.azurewebsites.net";
const devApi = "https://172.30.202.134:5051";
const chosenApi = JSON.parse(localStorage.getItem("api"));
const user = JSON.parse(localStorage.getItem("user"));
const token = localStorage.getItem("token");
const floatingError = document.querySelector('.floating-error');

//User settings
const userSettings = {
    "force-remote-api": localStorage.getItem('force-remote-api'),
    "force-dev-api": localStorage.getItem('force-dev-api'),
    "simulate-offline-mode": localStorage.getItem('simulate-offline-mode'),
    "suppress-api-errors": localStorage.getItem('suppress-api-errors'),
};

let currentApi = "";
let localApiExists = false;
let remoteApiExists = false;
let devApiExists = false;

function throw_floating_error(error, code, color){
    if(!floatingError.classList.contains('floating-error-hidden')){
        return;
    }
    console.log(error);
    floatingError.style.background = color;
    floatingError.querySelector('.floating-error-title').textContent = "Error: " + code;
    floatingError.querySelector('.floating-error-subtitle').textContent = error;
    floatingError.classList.remove('floating-error-hidden');
}

function throw_floating_message(error, code){
    if(!floatingError.classList.contains('floating-error-hidden')){
        return;
    }
    console.log(error);
    floatingError.style.background = 'var(--color-background-gradient)';
    floatingError.querySelector('.floating-error-title').textContent = code;
    floatingError.querySelector('.floating-error-subtitle').textContent = error;
    floatingError.classList.remove('floating-error-hidden');
}
//window.throw_floating_error = throw_floating_error;

floatingError.querySelector('.floating-error-button').addEventListener('click', () => {
    floatingError.classList.add('floating-error-hidden');
});

async function pingApi(api, timeout = 750) {
    try {
        const response = await Promise.race([
            fetch(api + "/ping"),
            new Promise((_, reject) => setTimeout(() => reject(new Error('Timeout')), timeout))
        ]);
        if (!response.ok) {
            throw new Error("API Error: " + response.status);
        }
        return true;
    } catch (error) {
        console.error("Error while pinging API:", error);
        return false;
    }
}
async function checkApis(){
    try {
        const [localResponse, remoteResponse, devResponse] = await Promise.all([
            pingApi(localApi),
            pingApi(remoteApi),
            pingApi(devApi)
        ]);

        if (localResponse === true && userSettings['force-remote-api'] != 'true' && userSettings['force-dev-api'] != 'true') {
            currentApi = localApi;
            console.log('%cUsing local api', 'font-size: 24px; font-weight: bold;');
        } else if (remoteResponse === true && userSettings['force-dev-api'] != 'true') {
            currentApi = remoteApi;
            console.log('%cUsing remote api', 'font-size: 24px; font-weight: bold;');
        } else if (devResponse === true) {
            currentApi = devApi;
            console.log('%cUsing DEV api', 'font-size: 24px; font-weight: bold;');
            console.log("Do not use this during production!");
        } else {
            throw new Error("Both the local and remote APIs are not accessible. This could be due to the remote API having a cold start. Try waiting.");
        }

        await refreshToken();
        if(window.loadClassicTables){
            loadClassicTables();
        }
        if(window.loadOpenGames){
            loadOpenGames();
        }
        if(window.showApi){
            showApi();
        }

    } catch(error) {
        console.error("Error while trying to reach APIs:", error);
        if(userSettings['suppress-api-errors'] !== 'true'){
            setTimeout(() => {
                throw_floating_error(error.message, '504', '#303031');
            }, 2000);
        }
    }
}

checkApis();

//Refresh user token
async function refreshToken(){
    try {
        const response = await fetch(currentApi + "/api/Authentication/refresh", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify({})
        });
        
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error("Your token has expired already!");
        }
        
        const data = await response.json();
        console.log(data);
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify(data.user));
    } catch(error) {
        //console.log(error);
    }
    setTimeout(refreshToken, 30 * 60 * 1000);
}