const localApi = "https://localhost:5051";
const remoteApi = "https://onitama61.azurewebsites.net";
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
}
