const passwordInput = document.getElementById("password-input");
const togglePasswordButton = document.getElementById("toggle-password");

togglePasswordButton.addEventListener('click',function(){

    if(passwordInput.type == 'password'){
        passwordInput.type = 'text';
        togglePasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')"
        togglePasswordButton.style.backgroundSize = "20px";
    }

    else{
        passwordInput.type = 'password';
        togglePasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')"
        togglePasswordButton.style.backgroundSize = "20px";
    }

});
