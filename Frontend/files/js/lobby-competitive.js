let classicTableElements = document.querySelectorAll(".comp-table");
let alreadyJoinedTable;
let alreadyJoinedTableType = "";
const tableButtons = document.querySelectorAll('.table-button-small');
const classicButton = document.querySelector('.classic-button');
const topButtonLogin = document.querySelector(".top-button-login");
const topButtonUser = document.querySelector(".top-button-user");

setInterval(() => {
    localStorage.removeItem("gameId");
}, 250);

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
        let classicTableResults = [];
        data.forEach(table => {
            if(table.preferences.tableType == "competitive"){
                classicTableResults.push(table);
            }
        });
        const classicTablesToRemove = 11 - classicTableResults.length; 
        for (let i = 1; i <= classicTablesToRemove; i++) {
            const classicTables = classicTableElements[11 - i];
            classicTables.classList.add('table-item-hidden');
        }
        //Classic tables
        for (let i = 0; i < Math.min(classicTableResults.length, 11); i++) {
            const table = classicTableResults[i];
            console.log(i);
            const element = classicTableElements[i];
            const maxPlayers = table.preferences.numberOfPlayers;
            const seatedPlayers = table.seatedPlayers.length;
            const ownerId = table.ownerId;
            let owner;
            for (let k = 0; k < table.seatedPlayers.length; k++) {
                owner = table.seatedPlayers[k].name;
                
            }
            element.classList.remove('table-item-loading');
            element.querySelector('.table-title').textContent = owner;
            const tablePlayers = element.querySelector('.table-players');
            tablePlayers.textContent = seatedPlayers + "/" + maxPlayers + " players";
            switch (table.preferences.moveCardSet) {
                case "1":
                    tablePlayers.textContent = tablePlayers.textContent + " | Sensei's Path";
                    break;

                case "2":
                    tablePlayers.textContent = tablePlayers.textContent + " | Custom cards";
                    break;
            
                default:
                    tablePlayers.textContent = tablePlayers.textContent + " | Original";
                    break;
            }
            element.querySelector('.table-button').textContent = "Join table";
            element.querySelector('.table-button').setAttribute("table-id", table.id);
            console.log(owner);
            
        }
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, "500", "#c60025");
    });

    //Tables loaded, but let's check if there are any tables that we are actually in.
    checkAllTables();

}

function checkAllTables(){
    fetch(currentApi + "/api/Tables/all", {
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
        //All tables here
        //console.log("All tables:");
        //console.log(data);
        for (let i = 0; i < data.length; i++) {
            const table = data[i];
            if(table.hasAvailableSeat == true){
                for (let j = 0; j < table.seatedPlayers.length; j++) {
                    const player = table.seatedPlayers[j];
                    if(player.id == user.id){
                        console.log("User already in a table");
                        document.querySelector('.main').classList.add('no-pointer');
                        alreadyJoinedTable = table.id;
                        document.querySelector('.floating-message').classList.remove('floating-message-hidden');
                    }
                }
            }
            
        }
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, "500", "#c60025");
    });
}

classicButton.addEventListener('click', () => {
    const response = fetch(currentApi + "/api/Tables/competitive", {
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
        localStorage.setItem("tableId", data.id);
        setTimeout(() => {
            window.location.href = "../game/play.html";
        }, 250);
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, '500', "#c60025");
    });
});

tableButtons.forEach(element => element.addEventListener('click', () => {
    const tableId = element.getAttribute("table-id");
    localStorage.setItem("tableId", tableId);

    const response = fetch(currentApi + "/api/Tables/" + tableId + "/join", {
        method: 'POST',
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
            window.location.href = "game/play.html";
        }, 250);
    }).catch(error => {
        console.log(error);
        //throw_floating_error(error, '500', "#c60025");
    });

}));

document.querySelector('.button-leave').addEventListener('click', () => {
    //Leave table
    const response = fetch(currentApi + "/api/Tables/" + alreadyJoinedTable + "/leave", {
        method: 'POST',
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
        return response;
    }).then(data => {
        let elements = [];
            elements = classicTableElements;
        console.log(data);
        document.querySelector('.floating-message').classList.add('floating-message-hidden');
        document.querySelector('.main').classList.remove('no-pointer');
        for (let i = 0; i < elements.length; i++) {
            const table = elements[i];
            if(table.querySelector('.table-button').getAttribute('table-id') == alreadyJoinedTable){
                table.classList.add('table-item-hidden');
            }
        }
    }).catch(error => {
        console.log(error);
        //throw_floating_error(error, '500', "#c60025");
    });
    
});

document.querySelector('.button-join').addEventListener('click', () => {
    //Join table
    localStorage.setItem("tableId", alreadyJoinedTable);
    setTimeout(() => {
        document.querySelector('.floating-message').classList.add('floating-message-hidden');
        document.querySelector('.main').classList.remove('no-pointer');
        window.location.href = "game/classic.html";
    }, 250);
});