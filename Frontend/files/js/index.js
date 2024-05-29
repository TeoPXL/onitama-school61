const topButtonLogin = document.querySelector(".top-button-login");
const topButtonUser = document.querySelector(".top-button-user");
const logoElement = document.querySelector(".logo");
const linkElements = document.querySelectorAll("a");
const compElements = document.querySelectorAll(".comp-item-loading");
//Temporary example of API json response
const compItems = {
    items: [
        {
            warrior1: "Username 1",
            warrior2: "Username 2",
            code: "gq-g4j4",
        },
        {
            warrior1: "Username 3",
            warrior2: "Username 4",
            code: "wg974w",
        },
        {
            warrior1: "Username 5",
            warrior2: "Username 6",
            code: "wetgh80w3",
        },
        {
            warrior1: "Username 7",
            warrior2: "Username 8",
            code: "poweitg984w",
        },
    ],
};

function hideCompGames(){
    document.querySelector('.comp-list').classList.add('comp-list-hidden');
}

function loadOpenGames (){
    fetch(currentApi + "/api/Games/all", {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': "Bearer " + token
        }
    }).then(response => {
        if (response.status === 401) {
            document.querySelector('.logo').textContent = document.querySelector('.logo').textContent + " E1";
            throw_floating_error("Your token expired! Try logging in again.", "401", "#c60025");
            topButtonLogin.classList.remove("hidden");
            topButtonUser.classList.add("hidden"); 
        }
        if (!response.ok) {
            return response.json().then(errorData => {
                //throw_floating_error(errorData.message, "500", "#c60025");
            });
        }
        return response.json();
    }).then(data => {
        const games = data;
        loadGames(games);
    }).catch(error => {
        console.log(error);
        //throw_floating_error(error, "500", "#c60025");
        const games = [];
        loadGames(games);
    });

}

if (user !== null) {
    topButtonLogin.classList.add("hidden");
    topButtonUser.querySelector(".top-button-text").textContent =
        user.warriorName;
    topButtonUser.classList.remove("hidden"); 
}


//Temporary artificial delay. This is to show the loading animation.
function loadGames(compItems) {
    document.querySelectorAll('.gamelist-text')[3].textContent = compItems.length + ' active matches';
    for (let i = 0; i < compItems.length; i++) {
        if(i >= compElements.length){
            break;
        }
        const game = compItems[i];
        let string = game.players[0].name + " vs " + game.players[1].name;
        let code = game.id;
        compElements[i].querySelector(".comp-item-text").textContent = string;
        compElements[i].classList.remove("comp-item-loading");
        if(game.gametype == "competitive"){
            compElements[i].querySelector('.comp-item-icon').innerHTML = `
            <svg  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"  fill="none"  stroke="currentColor"  stroke-width="1.25"  stroke-linecap="round"  stroke-linejoin="round"  class="icon icon-tabler icons-tabler-outline icon-tabler-trophy"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M8 21l8 0" /><path d="M12 17l0 4" /><path d="M7 4l10 0" /><path d="M17 4v8a5 5 0 0 1 -10 0v-8" /><path d="M5 9m-2 0a2 2 0 1 0 4 0a2 2 0 1 0 -4 0" /><path d="M19 9m-2 0a2 2 0 1 0 4 0a2 2 0 1 0 -4 0" /></svg>
            `;
        } else if(game.gametype == "blitz"){
            compElements[i].querySelector('.comp-item-icon').innerHTML = `
            <svg  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"  fill="none"  stroke="currentColor"  stroke-width="1.25"  stroke-linecap="round"  stroke-linejoin="round"  class="icon icon-tabler icons-tabler-outline icon-tabler-alarm"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M12 13m-7 0a7 7 0 1 0 14 0a7 7 0 1 0 -14 0" /><path d="M12 10l0 3l2 0" /><path d="M7 4l-2.75 2" /><path d="M17 4l2.75 2" /></svg>
            `;
        }
        compElements[i].querySelector('.comp-item-spectate').setAttribute('onitama-gameid', code);
    }
    const items = document.querySelectorAll(".comp-item-loading");
    if(items.length == 5){
        document.querySelector('.comp-list').classList.add('comp-list-hidden');
    } else {
        items.forEach((element) => {
            element.classList.add('comp-item-hidden');
        });
    }
    
}

document.querySelectorAll('.comp-item-spectate').forEach((el) => el.addEventListener("click", () => {
    const gameId = el.getAttribute('onitama-gameid');
    console.log(gameId);
    localStorage.setItem("gameId", gameId);

    localStorage.removeItem("tableId");
    setTimeout(() => {
        window.location.href = "../game/spectate.html";
    }, 250);
}));

linkElements.forEach((element) => {
    element.addEventListener("click", (event) => {
        if (element.classList.contains("link-login") && user === null) {
            event.preventDefault();
            window.location.href = "login.html";
        }
    });
});

function showApi(){
    switch (currentApi) {
        case localApi:
            logoElement.textContent = 'Onitama 61 (L)';
            break;
    
        case remoteApi:
            logoElement.textContent = 'Onitama 61 (R)';
            break;
    
        case devApi:
            logoElement.textContent = 'Onitama 61 (D)';
            break;
    
        default:
            logoElement.textContent = 'Onitama 61 (N)';
            break;
    }
}

document.querySelector('.gamelist-item-ai').addEventListener('click', () => {
    //First check if user is logged in
    if(user === null) {
        window.location.href = "login.html";
        return;
    }
    //Now start a classic game
    startClassicTableAi();
});

function startClassicTableAi(){
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
        localStorage.setItem("tableId", data.id);
        //Now immediately fill it with AI
        fillTableWithAi(data.id);
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, '500', "#c60025");
    });
}

function fillTableWithAi(tableId){
    const response = fetch(currentApi + "/api/Tables/" + tableId + "/fill-with-ai", {
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
                throw_floating_error(errorData.message, '500', "#c60025");
            });
        }
        return response.json();
    }).then(data => {
        console.log(data);
        //Now redirect the user to the game
        setTimeout(() => {
            window.location.href = "game/play.html";
        }, 250);
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, '500', "#c60025");
    });
}

