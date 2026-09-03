const passwordInput = document.getElementById("password-input");
const confirmPasswordInput = document.getElementById("confirm-password-input");
const togglePasswordButton1 = document.getElementById("toggle-password");
const togglePasswordButton2 = document.getElementById("toggle-password-2");

togglePasswordButton1.addEventListener('click',function(){

    if(passwordInput.type == 'password'){
        passwordInput.type = 'text';
        confirmPasswordInput.type = 'text';
        togglePasswordButton1.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')"
        togglePasswordButton1.style.backgroundSize = "20px";
        togglePasswordButton2.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')"
        togglePasswordButton2.style.backgroundSize = "20px";
    }

    else{
        passwordInput.type = 'password';
        confirmPasswordInput.type = 'password';
        togglePasswordButton1.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')"
        togglePasswordButton1.style.backgroundSize = "20px";
        togglePasswordButton2.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')"
        togglePasswordButton2.style.backgroundSize = "20px";
    }

});

togglePasswordButton2.addEventListener('click',function(){

    if(confirmPasswordInput.type == 'password'){
        passwordInput.type = 'text';
        confirmPasswordInput.type = 'text';
        togglePasswordButton1.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')"
        togglePasswordButton1.style.backgroundSize = "20px";
        togglePasswordButton2.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')"
        togglePasswordButton2.style.backgroundSize = "20px";
    }

    else{
        passwordInput.type = 'password';
        confirmPasswordInput.type = 'password';
        togglePasswordButton1.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')"
        togglePasswordButton1.style.backgroundSize = "20px";
        togglePasswordButton2.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')"
        togglePasswordButton2.style.backgroundSize = "20px";
    }

});
