const localApi = "https://localhost:5051";
const remoteApi = "https://onitama61.azurewebsites.net";
const chosenApi = JSON.parse(localStorage.getItem("api"));
const user = JSON.parse(localStorage.getItem("user"));
const token = localStorage.getItem("token");
const floatingError = document.querySelector('.floating-error');

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

fetch(localApi+"/ping")
    .then(response => {
        if (response.ok) {
            localApiExists = true;
        }
    })
    .catch(error => {
        console.error("Error while trying to reach local API:", error);
    });

fetch(remoteApi+"/ping")
    .then(response => {
        if (response.ok) {
            remoteApiExists = true;
        }
    })
    .catch(error => {
        console.error("Error while trying to reach remote API:", error);
    });
    setTimeout(() => {
        if(localApiExists == false && remoteApiExists == false){
            throw_floating_error('Could not make a connection to both local and remote API! Try waiting for a cold start. You can dismiss this message if you intend to play offline.', '504', '#303031');
        }
    }, 2000);
    

if(localApiExists){
    currentApi = localApi;
} else {
    currentApi = remoteApi;
}

if(chosenApi !== null){
    currentApi = chosenApi;
}
