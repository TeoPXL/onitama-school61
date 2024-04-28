let classicTableElements = document.querySelectorAll(".classic-table");
let blitzTableElements = document.querySelectorAll(".blitz-table");
const tableButtons = document.querySelectorAll('.table-button-small');
const classicButton = document.querySelector('.classic-button');
const topButtonLogin = document.querySelector(".top-button-login");
const topButtonUser = document.querySelector(".top-button-user");

if(user === null ){
    window.location.href = "login.html";
} else {
    topButtonUser.querySelector(".top-button-text").textContent = user.warriorName;
    topButtonLogin.classList.add("hidden");
    topButtonUser.classList.remove("hidden"); 
}

function loadClassicTables (){
    fetch(currentApi + "/api/Tables/with-available-seats", {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': "Bearer " + token
        }
    }).then(response => {
        if (response.status === 401) {
            throw_floating_error("Your token expired! Try logging in again.", "401", "#c60025");
            topButtonLogin.classList.remove("hidden");
            topButtonUser.classList.add("hidden"); 
        }
        if (!response.ok) {
            return response.json().then(errorData => {
                throw_floating_error(errorData.message, "500", "#c60025");
            });
        }
        return response.json();
    }).then(data => {
        console.log(data);
        const tablesToRemove = 3 - Math.max(data.length, 3);
        for (let i = 1; i <= tablesToRemove; i++) {
            const classicTables = classicTableElements[3 - i];
            classicTables.classList.add('table-item-hidden');
        }
        //Currently the value "3" is a placeholder until we figure out how to distinguish game types.
        for (let i = 1; i <= 3; i++) {
            const blitzTables = blitzTableElements[3 - i];
            blitzTables.classList.add('table-item-hidden');
        }
    
        for (let i = 0; i < Math.min(data.length, 3); i++) {
            const table = data[i];
            console.log(i);
            const element = classicTableElements[i];
            const maxPlayers = table.preferences.numberOfPlayers;
            const seatedPlayers = table.seatedPlayers.length;
            const owner = table.seatedPlayers[0].name;
            element.classList.remove('table-item-loading');
            element.querySelector('.table-title').textContent = owner;
            element.querySelector('.table-players').textContent = seatedPlayers + "/" + maxPlayers + " players";
            element.querySelector('.table-button').textContent = "Join table";
            element.querySelector('.table-button').setAttribute("table-id", table.id);
            console.log(owner);
            
        }
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, "500", "#c60025");
    });
}

classicButton.addEventListener('click', () => {
    const response = fetch(currentApi + "/api/Tables", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify({ numberOfPlayers: 2, playMatSize: 5, moveCardSet: 0 })
    }).then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw_floating_error(errorData.message, '500', "#c60025");
            });
        }
        return response.json();
    }).then(data => {
        console.log(data);
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, '500', "#c60025");
    });
});

tableButtons.forEach(element => element.addEventListener('click', () => {
    const tableId = element.getAttribute("table-id");
    localStorage.setItem("tableId", tableId);

    const response = fetch(currentApi + "/api/Tables/" + tableId + "/join", {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        }
    }).then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw_floating_error(errorData.message, '405', "#c60025");
            });
        }
        return response.json();
    }).then(data => {
        console.log(data);
        setTimeout(() => {
            window.location.href = "game/classic.html";
        }, 250);
    }).catch(error => {
        console.log(error);
        //throw_floating_error(error, '500', "#c60025");
    });

    
}));