
const topButtonLogin = document.querySelector(".top-button-login");
const topButtonUser = document.querySelector(".top-button-user");
localStorage.removeItem("gameId");

if(user === null ){
    window.location.href = "login.html";
} else {
    topButtonUser.querySelector(".top-button-text").textContent = user.warriorName;
    topButtonLogin.classList.add("hidden");
    topButtonUser.classList.remove("hidden"); 
}

const senseisPath = [
    {
        "Name": "Fox",
        "Grid": [
            "00000",
            "00010",
            "00210",
            "00010",
            "00000"
        ]
    },
    {
        "Name": "Dog",
        "Grid": [
            "00000",
            "01000",
            "01200",
            "01000",
            "00000"
        ]
    },
    {
        "Name": "Giraffe",
        "Grid": [
            "01010",
            "00000",
            "00200",
            "00100",
            "00000"
        ]
    },
    {
        "Name": "Panda",
        "Grid": [
            "00000",
            "00110",
            "00200",
            "01000",
            "00000"
        ]
    },
    {
        "Name": "Bear",
        "Grid": [
            "00000",
            "01100",
            "00200",
            "00010",
            "00000"
        ]
    },
    {
        "Name": "Kirin",
        "Grid": [
            "01010",
            "00000",
            "00200",
            "00000",
            "00100"
        ]
    },
    {
        "Name": "Sea snake",
        "Grid": [
            "00000",
            "00100",
            "00201",
            "01000",
            "00000"
        ]
    },
    {
        "Name": "Viper",
        "Grid": [
            "00000",
            "00100",
            "10200",
            "00010",
            "00000"
        ]
    },
    {
        "Name": "Phoenix",
        "Grid": [
            "00000",
            "01010",
            "10201",
            "00000",
            "00000"
        ]
    },
    {
        "Name": "Mouse",
        "Grid": [
            "00000",
            "00100",
            "00210",
            "01000",
            "00000"
        ]
    },
    {
        "Name": "Rat",
        "Grid": [
            "00000",
            "00100",
            "01200",
            "00010",
            "00000"
        ]
    },
    {
        "Name": "Turtle",
        "Grid": [
            "00000",
            "00000",
            "10201",
            "01010",
            "00000"
        ]
    },
    {
        "Name": "Tanuki",
        "Grid": [
            "00000",
            "00101",
            "00200",
            "01000",
            "00000"
        ]
    },
    {
        "Name": "Iguana",
        "Grid": [
            "00000",
            "10100",
            "00200",
            "00010",
            "00000"
        ]
    },
    {
        "Name": "Sable",
        "Grid": [
            "00000",
            "00010",
            "10200",
            "01000",
            "00000"
        ]
    },
    {
        "Name": "Otter",
        "Grid": [
            "00000",
            "01000",
            "00201",
            "00010",
            "00000"
        ]
    }
];

const original = [
    {
        "Name": "Tiger",
        "Grid": [
            "00100",
            "00000",
            "00200",
            "00100",
            "00000"
        ]
    },
    {
        "Name": "Dragon",
        "Grid": [
            "00000",
            "10001",
            "00200",
            "01010",
            "00000"
        ]
    },
    {
        "Name": "Frog",
        "Grid": [
            "00000",
            "01000",
            "10200",
            "00010",
            "00000"
        ]
    },
    {
        "Name": "Rabbit",
        "Grid": [
            "00000",
            "00010",
            "00201",
            "01000",
            "00000"
        ]
    },
    {
        "Name": "Crab",
        "Grid": [
            "00000",
            "00100",
            "10201",
            "00000",
            "00000"
        ]
    },
    {
        "Name": "Elephant",
        "Grid": [
            "00000",
            "01010",
            "01210",
            "00000",
            "00000"
        ]
    },
    {
        "Name": "Goose",
        "Grid": [
            "00000",
            "01000",
            "01210",
            "00010",
            "00000"
        ]
    },
    {
        "Name": "Rooster",
        "Grid": [
            "00000",
            "00010",
            "01210",
            "01000",
            "00000"
        ]
    },
    {
        "Name": "Monkey",
        "Grid": [
            "00000",
            "01010",
            "00200",
            "01010",
            "00000"
        ]
    },
    {
        "Name": "Mantis",
        "Grid": [
            "00000",
            "01010",
            "00200",
            "00100",
            "00000"
        ]
    },
    {
        "Name": "Horse",
        "Grid": [
            "00000",
            "00100",
            "01200",
            "00100",
            "00000"
        ]
    },
    {
        "Name": "Ox",
        "Grid": [
            "00000",
            "00100",
            "00210",
            "00100",
            "00000"
        ]
    },
    {
        "Name": "Crane",
        "Grid": [
            "00000",
            "00100",
            "00200",
            "01010",
            "00000"
        ]
    },
    {
        "Name": "Boar",
        "Grid": [
            "00000",
            "00100",
            "01210",
            "00000",
            "00000"
        ]
    },
    {
        "Name": "Eel",
        "Grid": [
            "00000",
            "01000",
            "00210",
            "01000",
            "00000"
        ]
    },
    {
        "Name": "Cobra",
        "Grid": [
            "00000",
            "00010",
            "01200",
            "00010",
            "00000"
        ]
    }
];



let cards = [];

class Card {
    name;
    grid;
    type;

    constructor(name, grid, type){
        this.name = name;
        this.grid = grid;
        this.type = type;
    }
}

senseisPath.forEach(el => {
    console.log(el);
    let card = new Card(el.Name, el.Grid, 'senseispath');
    cards.push(card);
});

original.forEach(el => {
    console.log(el);
    let card = new Card(el.Name, el.Grid, 'original');
    cards.push(card);
});

cards.forEach(card => {
    console.log(card);
    const parent = document.createElement('div');
    parent.classList.add('card-selection-card');

    const container = document.createElement('div');
    container.classList.add('card-selection-card-grid');

    card.grid.forEach(row => {
        row.split('').forEach(block => {
            const cardBlock = document.createElement('div');
            cardBlock.classList.add('card-selection-card-block');
            if(block == '1'){
                cardBlock.classList.add('card-selection-card-pawn');
            } else if(block == '2'){
                cardBlock.classList.add('card-selection-card-king');
            }
            container.appendChild(cardBlock);
        });
    });

    const name = document.createElement('div');
    name.classList.add('card-selection-card-name');
    name.textContent = card.name;

    parent.appendChild(container);
    parent.appendChild(name);

    if(card.type == 'original'){
        document.querySelector('.original-list').appendChild(parent);
    }

    if(card.type == 'senseispath'){
        document.querySelector('.senseispath-list').appendChild(parent);
    }
});

document.querySelector('.custom-deck-button').addEventListener('click', () => {
    //Open card selection menu
    document.querySelector('.card-selection').classList.remove('card-selection-hidden');
});

document.querySelector('.card-selection-close-button').addEventListener('click', () => {
    //Open card selection menu
    document.querySelector('.card-selection').classList.add('card-selection-hidden');
});

document.querySelector('.card-create').addEventListener('click', () => {
    //Open card selection menu
    document.querySelector('.card-creation').classList.remove('card-creation-hidden');
});

document.querySelector('.card-creation-close-button').addEventListener('click', () => {
    //Open card selection menu
    document.querySelector('.card-creation').classList.add('card-creation-hidden');
});

