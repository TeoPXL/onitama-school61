const localApi = "https://localhost:5051";
const remoteApi = "https://onitama61.azurewebsites.net";
const chosenApi = JSON.parse(localStorage.getItem("api"));
const user = JSON.parse(localStorage.getItem("user"));
const token = localStorage.getItem("token");
const floatingError = document.querySelector('.floating-error');

//User settings
const userSettings = {
    "force-remote-api": localStorage.getItem('force-remote-api'),
    "simulate-offline-mode": localStorage.getItem('simulate-offline-mode'),
    "suppress-api-errors": localStorage.getItem('suppress-api-errors'),
};

let currentApi = "";
let localApiExists = false;
let remoteApiExists = false;

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

floatingError.querySelector('.floating-error-button').addEventListener('click', () => {
    floatingError.classList.add('floating-error-hidden');
});

async function checkApis(){
    try {
        await fetch(localApi+"/ping")
        .then(response => {
            if (response.ok) {
                localApiExists = true;
            } else {
                console.log(response);
            }
        });

        await fetch(remoteApi+"/ping")
            .then(response => {
                if (response.ok) {
                    remoteApiExists = true;
                }
            });

        if(localApiExists == true && userSettings['force-remote-api'] != 'true'){
            currentApi = localApi;
            console.log('Using local api');
        } else if(remoteApiExists) {
            currentApi = remoteApi;
            console.log('Using remote api');
        } else {
            throw new Error("Both the local and remote APIs are not accessible. This could be due to the remote API having a cold start. Try waiting.");
        }
        await refreshToken();
        if(window.loadClassicTables){
            loadClassicTables();
        }

    } catch(error) {
        console.error("Error while trying to reach local API:", error);
        if(userSettings['suppress-api-errors'] != 'true'){
            setTimeout(() => {
                throw_floating_error(error.message, '504', '#303031');
            }, 2000);
        }
    };
    
}

checkApis();

//Refresh user token
async function refreshToken(){

    await fetch(currentApi + "/api/Authentication/refresh", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify({})
    }).then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw_floating_error(errorData.message, '401', "#c60025");
            });
        }
        return response.json();
    }).then(data => {
        console.log(data);
        localStorage.setItem('token', data.token);
    });
    setTimeout(refreshToken, 30 * 60 * 1000);
}

    


