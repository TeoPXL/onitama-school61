const topButtonLogin = document.querySelector(".top-button-login");
const topButtonUser = document.querySelector(".top-button-user");
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
        {
            warrior1: "Username 9",
            warrior2: "Username 10",
            code: "23589hgseg",
        },
    ],
};

if (user !== null) {
    topButtonLogin.classList.add("hidden");
    topButtonUser.querySelector(".top-button-text").textContent =
        user.warriorName;
    topButtonUser.classList.remove("hidden"); 
}


//Temporary artificial delay. This is to show the loading animation.
setTimeout(() => {
    compItems.items.forEach((compitem, index) => {
        let string = compitem.warrior1 + " vs " + compitem.warrior2;
        let code = compitem.code;
        compElements[index].querySelector(".comp-item-text").textContent = string;
        compElements[index].classList.remove("comp-item-loading");
    });
}, 1500);

linkElements.forEach((element) => {
    element.addEventListener("click", (event) => {
        if (element.classList.contains("link-login") && user === null) {
            event.preventDefault();
            window.location.href = "login.html";
        }
    });
});
