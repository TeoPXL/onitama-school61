
const topButtonLogin = document.querySelector(".top-button-login");
const topButtonUser = document.querySelector(".top-button-user");
let customCards = JSON.parse(localStorage.getItem("custom-cards"));
let selectedCards = JSON.parse(localStorage.getItem("selected-cards"));
if(customCards == null){
    customCards = [];
}
if(selectedCards == null){
    selectedCards = [];
}
selectedCards = selectedCards.filter(element => element !== null);

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

updateCardList();
class Card {
    name;
    grid;
    type;
    id;
    selected = false;

    constructor(name, grid, type, id){
        this.name = name;
        this.grid = grid;
        this.type = type;
        this.id = id;
    }
}

senseisPath.forEach(el => {
    console.log(el);
    let id = cards.length;
    let card = new Card(el.Name, el.Grid, 'senseispath', id);
    cards.push(card);
});

original.forEach(el => {
    console.log(el);
    let id = cards.length;
    let card = new Card(el.Name, el.Grid, 'original', id);
    cards.push(card);
});

customCards.forEach(el => {
    console.log(el);
    let id = cards.length;
    let card = new Card(el.name, el.grid, 'custom', id);
    cards.push(card);
});

function updateSelectedCards(){
    selectedCards.forEach(selectedCard => {
        cards.forEach(card => {
            if(card.name == selectedCard.name){
                card.selected = true;
                console.log('true');
            }
        });
    });
}
updateSelectedCards();


function updateCards(){
    document.querySelector('.original-list').innerHTML = "";
    document.querySelector('.senseispath-list').innerHTML = "";
    document.querySelector('.custom-list').innerHTML = "";
    const cardCreator = document.createElement('div');
    cardCreator.classList.add('card-selection-card');
    cardCreator.classList.add('card-create');
    const cardCreatorButton = document.createElement('div');
    cardCreatorButton.textContent = "Create a new card";
    cardCreatorButton.classList.add('card-selection-card-button');
    cardCreator.appendChild(cardCreatorButton);
    document.querySelector('.custom-list').appendChild(cardCreator);

    cards.forEach(card => {
        console.log(card);
        const parent = document.createElement('div');
        parent.classList.add('card-selection-card');
        if(card.selected == true){
            parent.classList.add('selected-card');
        }
        parent.setAttribute('onitama-card-id', card.id);
    
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

        if(card.type == 'custom'){
            document.querySelector('.custom-list').appendChild(parent);
        }
    });
    addEventListeners();
}
updateCards();


document.querySelectorAll('.card-creation-block').forEach(el => el.addEventListener('click', () => {
    el.classList.toggle('card-creation-block-active');
}));

document.querySelector('.custom-deck-button').addEventListener('click', () => {
    document.querySelector('.card-selection').classList.remove('card-selection-hidden');
});

document.querySelector('.card-selection-close-button').addEventListener('click', () => {
    document.querySelector('.card-selection').classList.add('card-selection-hidden');
});

document.querySelector('.card-create').addEventListener('click', () => {
    document.querySelector('.card-creation').classList.remove('card-creation-hidden');
});

document.querySelector('.card-creation-close-button').addEventListener('click', () => {
    document.querySelector('.card-creation').classList.add('card-creation-hidden');
});

document.querySelector('.custom-create-button').addEventListener('click', () => {
    if(selectedCards.length < 5){
        throw_floating_error("You must select at least 5 cards", "", "");
        return;
    }
});

document.querySelector('.card-creation-confirm-button').addEventListener('click', () => {
    const cardBlocks = document.querySelectorAll('.card-creation-block-item');
    let blockString = "";
    let pawnCount = 0;
    cardBlocks.forEach(block => {
        if(block.classList.contains('card-creation-block-active')){
            blockString += "1";
            pawnCount++;
        } else if(block.classList.contains('card-creation-king')){
            blockString += "2";
        } else {
            blockString += "0";
        }
    });
    let gridSize = 5;
    let blockGrid = [];

    for (let i = 0; i < blockString.length; i += gridSize) {
        blockGrid.push(blockString.slice(i, i + gridSize));
    }

    const cardName = document.querySelector('.card-creation-input').value;
    if(pawnCount < 1){
        throw_floating_error("You must select at least one possible field", "", "")
        return;
    }
    if(cardName == ""){
        throw_floating_error("You must provide a name for your card", "", "")
        return;
    }
    for (let i = 0; i < cards.length; i++) {
        const card = cards[i];
        if(card.name == cardName){
            throw_floating_error("Your card's name must be unique", "", "")
            return;
        }
    }
    let id = cards.length;
    let newCard = new Card(cardName, blockGrid, "custom", id);
    cards.push(newCard);
    customCards.push(newCard);
    localStorage.setItem('custom-cards', JSON.stringify(customCards));
    console.log(newCard);
    updateCards();
    document.querySelector('.card-creation').classList.add('card-creation-hidden');
});

function isEqual(obj1, obj2) {
    return obj1.name == obj2.name;
}

function addEventListeners(){
    document.querySelectorAll('.card-selection-card').forEach(el => el.addEventListener('click', () => {
        el.classList.toggle('selected-card');
        let id = el.getAttribute('onitama-card-id');
        if(selectedCards.some(element => isEqual(element, cards[id]))){
            for (let i = 0; i < selectedCards.length; i++) {
                const card = selectedCards[i];
                if(card.name == cards[id].name){
                    selectedCards.splice(i, 1);;
                }
            }
        } else {
            selectedCards.push(cards[id]);
        }
        localStorage.setItem('selected-cards', JSON.stringify(selectedCards));
        updateCardList();
    }));
}

function updateCardList(){
    document.querySelector('.custom-deck-list').innerHTML = "";
    selectedCards.forEach(card => {
        const element = document.createElement('div');
        element.classList.add('custom-deck-card');
        element.textContent = card.name;
    
        document.querySelector('.custom-deck-list').appendChild(element);
    });
}




