// Profil fotoğrafı modal kısmı

const openPhotoModalButton = document.getElementById("open-photo-modal-button");
const cancelPhotoModalButton = document.getElementById("cancel-photo-modal-button");
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