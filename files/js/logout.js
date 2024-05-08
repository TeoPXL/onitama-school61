const logoutButton = document.querySelector('.top-button-logout');

function logout(event) {

    event.preventDefault();

    localStorage.removeItem('user');
    localStorage.removeItem('token');
    localStorage.removeItem('email'); 

    window.location.href = "index.html";
}
logoutButton.addEventListener('click', logout);

