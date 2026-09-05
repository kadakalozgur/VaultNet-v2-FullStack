const emailInput = document.getElementById("email-input");
const passwordInput = document.getElementById("password-input");
const togglePasswordButton = document.getElementById("toggle-password");
const loginButton = document.getElementById("login-button");
const alertBox = document.getElementById("login-alert");

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

loginButton.addEventListener('click',async function(e){

    e.preventDefault(); // Butonun sayfayı yenilemesini kapattım.

    hideAlert(alertBox);

    const email = emailInput.value.trim().toLowerCase().replaceAll(" ", "");
    const password = passwordInput.value;

    if(!email || !password){
        showAlert(alertBox,"Please fill in all fields completely.","warning");
        return;
    }

    loginButton.disabled = true;

    try{

        await login(email,password);

        window.location.href = "dashboard.html";

    }catch(error){
        showAlert(alertBox,error.message,"danger");
        loginButton.disabled = false;
    }
    
});
