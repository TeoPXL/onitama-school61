const loginButton = document.querySelector('.login-button');
const emailInput = document.getElementById('email-input');
const passwordInput = document.getElementById('password-input');
const errorMessage = document.querySelector('.error-message');
const errorSubtitle = document.querySelector('.error-subtitle');
const errorButton = document.querySelector('.error-button');

function throw_error(error) {
    errorSubtitle.textContent = error;
    errorMessage.classList.remove('invisible');
}

loginButton.addEventListener('click', () => {
    const email = emailInput.value;
    const password = passwordInput.value;
    console.log(email, password);

    if (email === '') {
        throw_error('Please fill in your email address.');
        return;
    }
    if (password === '') {
        throw_error('Please fill in your password.');
        return;
    }

    const response = fetch(currentApi + "/api/Authentication/token", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email: email, password: password })
    }).then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw_error(errorData.message);
            });
        }
        return response.json();
    }).then(data => {
        localStorage.setItem('user', JSON.stringify(data.user));
        localStorage.setItem('token', data.token);
    }).then(() => {
        window.location.href = "lobby.html";
    }).catch(error => {
        console.log(error);
    });

});

errorButton.addEventListener('click', () => {
    errorMessage.classList.add('invisible');
});