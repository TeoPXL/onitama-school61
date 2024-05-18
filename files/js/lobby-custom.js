
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