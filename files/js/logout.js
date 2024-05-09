const logoutButton = document.querySelector('.top-button-logout');

logoutButton.addEventListener('click', (event) => {
    event.preventDefault();

    localStorage.removeItem('user');
    localStorage.removeItem('token');
    localStorage.removeItem('email'); 

    window.location.href = "index.html";
});



