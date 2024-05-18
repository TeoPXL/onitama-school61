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
        const games = data;
        loadGames(games);
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, "500", "#c60025");
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
    for (let i = 0; i < compItems.length; i++) {
        const game = compItems[i];
        let string = game.warrior1 + " vs " + game.warrior2;
        let code = game.tableid;
        compElements[i].querySelector(".comp-item-text").textContent = string;
        compElements[i].classList.remove("comp-item-loading");
        compElements[i].setAttribute('onitama-tableid', code);
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
            logoElement.textContent = 'Onitama 61 (Local)';
            break;
    
        case remoteApi:
            logoElement.textContent = 'Onitama 61 (Remote)';
            break;
    
        case devApi:
            logoElement.textContent = 'Onitama 61 (Dev)';
            break;
    
        default:
            logoElement.textContent = 'Onitama 61 (None)';
            break;
    }
}

