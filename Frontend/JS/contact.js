const form = document.getElementById('contact-form');

form.addEventListener('submit', function(event) {

    event.preventDefault(); 

    const nameValue = document.getElementById('name').value;
    const emailValue = document.getElementById('email').value;
    const messageValue = document.getElementById('message').value;

    const nameError = document.getElementById('name-error');
    const emailError = document.getElementById('email-error');
    const messageError = document.getElementById('message-error');
    const successMessage = document.getElementById('success-message');

    nameError.textContent = "";
    emailError.textContent = "";
    messageError.textContent = "";
    successMessage.textContent = "";

    let isValid = true;

    if (nameValue === "") {
        nameError.textContent = "Please enter your full name.";
        isValid = false; 
    }

    if (emailValue === "" || !emailValue.includes('@')) {
        emailError.textContent = "Please enter a valid email address containing an @.";
        isValid = false;
    }

    if (messageValue.length < 10) {
        messageError.textContent = "Your message must be at least 10 characters long.";
        isValid = false;
    }

    if (isValid === true) {
        successMessage.textContent = "Thank you! Your message has been sent successfully.";
        form.reset();
    }
});