const topButtonLogin = document.querySelector('.top-button-login');
const topButtonUser = document.querySelector('.top-button-user');
const user = JSON.parse(localStorage.getItem('user'));
const token = localStorage.getItem('token');

if (user !== null) {
    topButtonLogin.classList.add('hidden');
    topButtonUser.querySelector('.top-button-text').textContent = user.warriorName;
    topButtonUser.classList.remove('hidden');
}