const loginButton = document.querySelector('.login-button');
const usernameInput = document.getElementById('username-input');
const emailInput = document.getElementById('email-input');
const passwordInput = document.getElementById('password-input');
const confirmPasswordInput = document.getElementById('confirm-password-input');
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
    const confirmPassword = confirmPasswordInput.value;
    const username = usernameInput.value;
    console.log(email, password, username);

    if (username === '') {
        throw_error('Please fill in your username.');
        return;
    }
    if (email === '') {
        throw_error('Please fill in your email address.');
        return;
    }
    if (password === '') {
        throw_error('Please fill in your password.');
        return;
    }
    if (password.length < 6) {
        throw_error('Your password must be at least 6 characters long.');
        return;
    }
    if (password !== confirmPassword) {
        throw_error('Both passwords need to be the same.');
        return;
    }

    const response = fetch(currentApi + "/api/Authentication/register", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email: email, password: password, wariorName: username })
    }).then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw_error(errorData.message);
            });
        }
    }).then(() => {
        window.location.href = "login.html";
    }).catch(error => {
        console.log(error);
    });


});

errorButton.addEventListener('click', () => {
    errorMessage.classList.add('invisible');
});