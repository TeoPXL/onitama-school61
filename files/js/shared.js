const localApi = "https://localhost:5051";
const remoteApi = "https://onitama61.azurewebsites.net";
const chosenApi = JSON.parse(localStorage.getItem("api"));
const user = JSON.parse(localStorage.getItem("user"));
const token = localStorage.getItem("token");

let currentApi = "";
let localApiExists = false;
let remoteApiExists = false;

fetch(localApi)
    .then(response => {
        if (response.ok) {
            localApiExists = true;
        }
    })
    .catch(error => {
        console.error("Error while trying to reach local API:", error);
    });

fetch(remoteApi)
    .then(response => {
        if (response.ok) {
            remoteApiExists = true;
        }
    })
    .catch(error => {
        console.error("Error while trying to reach remote API:", error);
    });

if(localApiExists){
    currentApi = localApi;
} else {
    currentApi = remoteApi;
}

if(chosenApi !== null){
    currentApi = chosenApi;
}
