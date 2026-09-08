// Profil fotoğrafı modal kısmı

const openPhotoModalButton = document.getElementById("open-photo-modal-button");
const cancelPhotoModalButton = document.getElementById("cancel-photo-modal-button");
const savePhotoModalButton = document.getElementById("save-photo-button");
const fileLabelPhotoModal = document.getElementById("file-label-photo-modal");
const fileInput = document.getElementById("customFile");
const previewImagePhotoModal = document.getElementById("previewAvatar");

openPhotoModalButton.addEventListener('click' ,function(){
    $('#changePhotoModal').modal('show');
});

cancelPhotoModalButton.addEventListener("click", function() {
    $('#changePhotoModal').modal('hide');
    fileInput.value = "";
    fileLabelPhotoModal.innerText = "Choose file...";
    previewImagePhotoModal.src = "Media/Icons/avatar-icon.png";
});

fileInput.addEventListener("change", function() {

    const file = fileInput.files[0];
    
    if (file) {

        if(!file.type.startsWith('image/')){
            fileInput.value = "";
            alert("Please select a valid image file (JPG, PNG...).");
            return;
        }

        fileLabelPhotoModal.innerText = file.name;

        const imageSource = URL.createObjectURL(file);
        previewImagePhotoModal.src = imageSource;

    }
});

savePhotoModalButton.addEventListener('click', async function(e) {
    
    e.preventDefault();

    const file = fileInput.files[0];

    savePhotoModalButton.disabled = true;

    try{

        await updateProfilePicture(file);

        sessionStorage.removeItem("user_profile");

        $('#changePhotoModal').modal('hide');

        location.reload();

    }catch(error){
        alert(error.message);
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

        sessionStorage.removeItem("user-profile");

        $('#changeEmailModal').modal('hide');

        location.reload();

    }catch(error){
        showAlert(emailModalAlert,error.message,"danger");
    }finally{
        saveEmailModalButton.disabled = false;
        newEmailInput.value = "";
    }

});
