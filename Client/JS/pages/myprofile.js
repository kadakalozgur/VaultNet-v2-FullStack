// Profil fotoğrafı modal kısmı

const openPhotoModalButton = document.getElementById("open-photo-modal-button");
const cancelPhotoModalButton = document.getElementById("cancel-photo-modal-button");
const savePhotoModalButton = document.getElementById("save-photo-button");
const fileLabelPhotoModal = document.getElementById("file-label-photo-modal");
const fileInput = document.getElementById("customFile");
const previewImagePhotoModal = document.getElementById("previewAvatar");
const photoModalAlert = document.getElementById("photo-modal-alert");

openPhotoModalButton.addEventListener('click' ,function(){
    $('#changePhotoModal').modal('show');
});

cancelPhotoModalButton.addEventListener("click", function() {
    $('#changePhotoModal').modal('hide');
    fileInput.value = "";
    fileLabelPhotoModal.innerText = "Choose file...";
    previewImagePhotoModal.src = "Media/Icons/avatar-icon.png";
    hideAlert(photoModalAlert);
});

fileInput.addEventListener("change", function() {

    hideAlert(photoModalAlert);

    const file = fileInput.files[0];
    
    if (file) {

        if(!file.type.startsWith('image/')){
            fileInput.value = "";
            showAlert(photoModalAlert, "Please select a valid image file (JPG, PNG...).", "warning");
            return;
        }

        fileLabelPhotoModal.innerText = file.name;

        const imageSource = URL.createObjectURL(file);
        previewImagePhotoModal.src = imageSource;

    }
});

savePhotoModalButton.addEventListener('click', async function(e) {
    
    e.preventDefault();

    hideAlert(photoModalAlert);

    const file = fileInput.files[0];

    if (!file) {
        showAlert(photoModalAlert, "Please select a valid image file.", "warning");
        return;
    }

    savePhotoModalButton.disabled = true;

    try{

        await updateProfilePicture(file);

        sessionStorage.removeItem("user_profile");

        $('#changePhotoModal').modal('hide');

        location.reload();

    }catch(error){
        showAlert(photoModalAlert,error.message,"danger");
    } finally{
        savePhotoModalButton.disabled = false;
        fileInput.value = "";
        fileLabelPhotoModal.innerText = "Choose file...";
        previewImagePhotoModal.src = "Media/Icons/avatar-icon.png";     
    }

});

// E-mail değiştirme modal kısmı

const openEmailModalButton = document.getElementById("open-email-modal-button");
const cancelEmailModalButton = document.getElementById("cancel-email-modal-button");
const saveEmailModalButton = document.getElementById("save-email-button");
const newEmailInput = document.getElementById("new-email-input");
const emailModalAlert = document.getElementById("email-modal-alert");

openEmailModalButton.addEventListener('click', function() {
    $('#changeEmailModal').modal('show');
});

cancelEmailModalButton.addEventListener("click", function() {
    $('#changeEmailModal').modal('hide');
    newEmailInput.value = "";
    hideAlert(emailModalAlert);
});

saveEmailModalButton.addEventListener('click', async function(e) {

    e.preventDefault();

    hideAlert(emailModalAlert);

    const newEmail = newEmailInput.value.trim().toLowerCase().replaceAll(" ", "")

    if (!newEmail) {
        showAlert(emailModalAlert, "Please enter a valid email address.", "warning");
        return;
    }

    saveEmailModalButton.disabled = true;

    try{

        await updateEmail(newEmail);

        sessionStorage.removeItem("user_profile");

        $('#changeEmailModal').modal('hide');

        location.reload();

    }catch(error){
        showAlert(emailModalAlert,error.message,"danger");
    }finally{
        saveEmailModalButton.disabled = false;
        newEmailInput.value = "";
    }

});

// Şifre değiştirme modal kısmı

const openPasswordModalButton = document.getElementById("open-password-modal-button");
const currentPasswordInput = document.getElementById("current-password-input");
const newPasswordInput = document.getElementById("new-password-input");
const confirmPasswordInput = document.getElementById("confirm-password-input");
const toggleCurrentPasswordButton = document.getElementById("toggle-current-password");
const toggleNewPasswordButton = document.getElementById("toggle-new-password");
const toggleConfirmPasswordButton = document.getElementById("toggle-confirm-password");
const cancelPasswordModalButton = document.getElementById("cancel-password-modal-button");
const savePasswordModalButton = document.getElementById("save-password-button");
const passwordModalAlert = document.getElementById("password-modal-alert");

openPasswordModalButton.addEventListener('click', function() {
    $('#changePasswordModal').modal('show');
});

cancelPasswordModalButton.addEventListener("click", function() {
    $('#changePasswordModal').modal('hide');
    currentPasswordInput.value = "";
    newPasswordInput.value = "";
    confirmPasswordInput.value = "";

    // Göz İconları ve input tipleri
    currentPasswordInput.type = 'password';
    toggleCurrentPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
    newPasswordInput.type = 'password';
    toggleNewPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
    confirmPasswordInput.type = 'password';
    toggleConfirmPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";

    hideAlert(passwordModalAlert);

});

toggleCurrentPasswordButton.addEventListener('click',function(){

    if(currentPasswordInput.type == 'password'){
        currentPasswordInput.type = 'text';
        toggleCurrentPasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')";
        toggleCurrentPasswordButton.style.backgroundSize = "20px";
    }

    else{
        currentPasswordInput.type = 'password';
        toggleCurrentPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
        toggleCurrentPasswordButton.style.backgroundSize = "20px";
    }

});

toggleNewPasswordButton.addEventListener('click',function(){

    if(newPasswordInput.type == 'password'){
        newPasswordInput.type = 'text';
        toggleNewPasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')";
        toggleNewPasswordButton.style.backgroundSize = "20px";
        confirmPasswordInput.type = 'text';
        toggleConfirmPasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')";
        toggleConfirmPasswordButton.style.backgroundSize = "20px";
    }

    else{
        newPasswordInput.type = 'password';
        toggleNewPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
        toggleNewPasswordButton.style.backgroundSize = "20px";
        confirmPasswordInput.type = 'password';
        toggleConfirmPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
        toggleConfirmPasswordButton.style.backgroundSize = "20px";
    }

});

toggleConfirmPasswordButton.addEventListener('click',function(){

    if(confirmPasswordInput.type == 'password'){
        newPasswordInput.type = 'text';
        toggleNewPasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')";
        toggleNewPasswordButton.style.backgroundSize = "20px";
        confirmPasswordInput.type = 'text';
        toggleConfirmPasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')";
        toggleConfirmPasswordButton.style.backgroundSize = "20px";
    }

    else{
        newPasswordInput.type = 'password';
        toggleNewPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
        toggleNewPasswordButton.style.backgroundSize = "20px";
        confirmPasswordInput.type = 'password';
        toggleConfirmPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
        toggleConfirmPasswordButton.style.backgroundSize = "20px";
    }

});

savePasswordModalButton.addEventListener('click', async function(e) {

    e.preventDefault();

    hideAlert(passwordModalAlert);

    const currentPassword = currentPasswordInput.value;
    const newPassword = newPasswordInput.value;
    const confirmPassword = confirmPasswordInput.value;

    if(!currentPassword || !newPassword || !confirmPassword){
        showAlert(passwordModalAlert, "Please fill in all fields completely.", "warning");
        return;
    }

    if(newPassword.length < 8) {
        showAlert(passwordModalAlert, "Password must be at least 8 characters long.", "warning");
        return;
    }

    if(newPassword.length > 64) {
        showAlert(passwordModalAlert, "Passwords can be a maximum of 64 characters long.", "warning");
        return;
    }

    if(currentPassword == newPassword){
        showAlert(passwordModalAlert, "Your new password cannot be the same as your old password.", "warning");
        return;
    }

    if(newPassword != confirmPassword){
        showAlert(passwordModalAlert, "The new password and password confirmation do not match.", "warning");
        return;
    }

    savePasswordModalButton.disabled = true;

    try{

        await changePassword(currentPassword,newPassword)

        $('#changePasswordModal').modal('hide'); 
        
        currentPasswordInput.value = "";
        newPasswordInput.value = "";
        confirmPasswordInput.value = "";

        // Göz İconları ve input tipleri
        currentPasswordInput.type = 'password';
        toggleCurrentPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
        newPasswordInput.type = 'password';
        toggleNewPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
        confirmPasswordInput.type = 'password';
        toggleConfirmPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";


    }catch(error){
        showAlert(passwordModalAlert,error.message,"danger");
    }finally{
        savePasswordModalButton.disabled = false;    
    }

});


