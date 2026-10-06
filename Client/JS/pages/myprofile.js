// Profil fotoğrafı modal kısmı

const openPhotoModalButton = document.getElementById("open-photo-modal-button");
const cancelPhotoModalButton = document.getElementById("cancel-photo-modal-button");
const savePhotoModalButton = document.getElementById("save-photo-button");
const removePhotoButton = document.getElementById("remove-photo-button");
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
    fileInput.disabled = true;
    removePhotoButton.disabled = true;
    cancelPhotoModalButton.disabled = true;

    try{

        await updateProfilePicture(file);

        sessionStorage.removeItem("user_profile");

        $('#changePhotoModal').modal('hide');

        location.reload();

    }catch(error){
        showAlert(photoModalAlert,error.message,"danger");
    } finally{
        savePhotoModalButton.disabled = false;
        fileInput.disabled = false;    
        removePhotoButton.disabled = false;
        cancelPhotoModalButton.disabled = false;
    }

});

removePhotoButton.addEventListener("click", async function (e) {

    e.preventDefault();

    hideAlert(photoModalAlert);

    removePhotoButton.disabled = true;
    savePhotoModalButton.disabled = true;
    cancelPhotoModalButton.disabled = true;
    fileInput.disabled = true;

    try {

        await removeProfilePicture();

        sessionStorage.removeItem("user_profile");

        $('#changePhotoModal').modal('hide');

        window.location.reload();

    } catch (error) {
        showAlert(photoModalAlert, error.message, "danger");
    } finally {
        removePhotoButton.disabled = false;
        savePhotoModalButton.disabled = false;
        cancelPhotoModalButton.disabled = false;
        fileInput.disabled = false;
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
    newEmailInput.disabled = true;
    cancelEmailModalButton.disabled = true;

    try{

        await updateEmail(newEmail);

        sessionStorage.removeItem("user_profile");

        $('#changeEmailModal').modal('hide');

        location.reload();

    }catch(error){
        showAlert(emailModalAlert,error.message,"danger");
    }finally{
        saveEmailModalButton.disabled = false;
        newEmailInput.disabled = false;
        cancelEmailModalButton.disabled = false;
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
    currentPasswordInput.disabled = true;
    newPasswordInput.disabled = true;
    confirmPasswordInput.disabled = true;
    cancelPasswordModalButton.disabled = true;

    try{

        await changePassword(currentPassword,newPassword);

        sessionStorage.removeItem("user_profile");

        $('#changePasswordModal').modal('hide'); 
        
        location.reload();

    }catch(error){
        showAlert(passwordModalAlert,error.message,"danger");
    }finally{
        savePasswordModalButton.disabled = false;    
        currentPasswordInput.disabled = false;
        newPasswordInput.disabled = false;
        confirmPasswordInput.disabled = false;
        cancelPasswordModalButton.disabled = false;
    }

});

// Tüm cihazlardan çıkış yapma modal kısmı

const openLogoutAllModalButton = document.getElementById("open-logout-all-modal-button");
const cancelLogoutAllModalButton = document.getElementById("cancel-logout-all-modal-button");
const confirmLogoutAllButton = document.getElementById("confirm-logout-all-button");
const logoutAllModalAlert = document.getElementById("logout-all-modal-alert");

openLogoutAllModalButton.addEventListener('click', function() {
    $('#logoutAllDevicesModal').modal('show');
});

cancelLogoutAllModalButton.addEventListener("click", function() {
    $('#logoutAllDevicesModal').modal('hide');
    hideAlert(logoutAllModalAlert);
});

confirmLogoutAllButton.addEventListener('click', async function(e) {
    
    e.preventDefault();

    hideAlert(logoutAllModalAlert);

    confirmLogoutAllButton.disabled = true;
    cancelLogoutAllModalButton.disabled = true;

    try {
        
        await logoutAllDevices();

        sessionStorage.clear();

        $('#logoutAllDevicesModal').modal('hide');

        window.location.href = "index.html";

    } catch (error) {
        showAlert(logoutAllModalAlert, error.message, "danger");
    } finally {
        confirmLogoutAllButton.disabled = false;
        cancelLogoutAllModalButton.disabled = false;
    }
});

// Hesabı silme modal kısmı

const openDeleteModalButton = document.getElementById("open-delete-modal-button");
const cancelDeleteModalButton = document.getElementById("cancel-delete-modal-button");
const confirmDeleteButton = document.getElementById("confirm-delete-button");
const deletePasswordInput = document.getElementById("delete-password-input");
const toggleDeletePasswordButton = document.getElementById("toggle-delete-password");
const deleteModalAlert = document.getElementById("delete-modal-alert");

openDeleteModalButton.addEventListener('click', function() {
    $('#deleteAccountModal').modal('show');
});

cancelDeleteModalButton.addEventListener("click", function() {
    $('#deleteAccountModal').modal('hide');
    deletePasswordInput.value = "";

    // Göz İconları ve input tipleri
    deletePasswordInput.type = 'password';
    toggleDeletePasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";

    hideAlert(deleteModalAlert);

});

toggleDeletePasswordButton.addEventListener('click',function(){

    if(deletePasswordInput.type == 'password'){
        deletePasswordInput.type = 'text';
        toggleDeletePasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')"
        toggleDeletePasswordButton.style.backgroundSize = "20px";
    }

    else{
        deletePasswordInput.type = 'password';
        toggleDeletePasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')"
        toggleDeletePasswordButton.style.backgroundSize = "20px";
    }

});

confirmDeleteButton.addEventListener('click', async function(e){

    e.preventDefault();

    hideAlert(deleteModalAlert);

    const currentPassword = deletePasswordInput.value;

    if(!currentPassword){
        showAlert(deleteModalAlert, "Please enter your password to confirm.", "warning");
        return;
    }

    confirmDeleteButton.disabled = true;
    deletePasswordInput.disabled = true;
    cancelDeleteModalButton.disabled = true;

    try{

        await deleteAccount(currentPassword);

        sessionStorage.clear();

        $('#deleteAccountModal').modal('hide');

        window.location.href = "index.html";

    }catch(error){
        showAlert(deleteModalAlert, error.message, "danger");
    }finally{
        confirmDeleteButton.disabled = false;
        deletePasswordInput.disabled = false;
        cancelDeleteModalButton.disabled = false;
    }

});

// Sayfa yüklenince loading ekranını gizlemek için.
document.addEventListener("DOMContentLoaded", function() {
    hideGlobalLoader();
});