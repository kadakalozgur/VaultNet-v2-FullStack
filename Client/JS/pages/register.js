const emailInput = document.getElementById("email-input");
const passwordInput = document.getElementById("password-input");
const confirmPasswordInput = document.getElementById("confirm-password-input");
const togglePasswordButton1 = document.getElementById("toggle-password");
const togglePasswordButton2 = document.getElementById("toggle-password-2");
const registerButton = document.getElementById("register-button");
const alertBox = document.getElementById("register-alert");

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

registerButton.addEventListener('click',async function(e){

    e.preventDefault(); // Butonun sayfayı yenilemesini kapattım.

    hideAlert(alertBox);

    const email = emailInput.value.trim().toLowerCase().replaceAll(" ", "");
    const password = passwordInput.value;
    const confirmPassword = confirmPasswordInput.value;

    if(!email || !password || !confirmPassword){   
        showAlert(alertBox,"Please fill in all fields completely.","warning");
        return;
    }

    if(password != confirmPassword){
        showAlert(alertBox,"The passwords don't match.","danger");
        return;
    }

    registerButton.disabled = true;

    try{

        await register(email,password);

        window.location.href = "index.html";

    }catch(error){
        showAlert(alertBox,error.message,"danger");
        registerButton.disabled = false;
    }
    
});

