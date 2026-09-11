// Add item modal kısmı

const openAddItemModalButton = document.getElementById("open-add-item-modal-button");
const cancelAddItemModalButton = document.getElementById("cancel-add-modal-button");
const saveItemButton = document.getElementById("save-item-button");
const toggleAddPasswordButton = document.getElementById("toggle-add-password");
const addItemModalAlert = document.getElementById("add-item-alert");
const addTitleInput = document.getElementById("add-title-input");
const addUsernameInput = document.getElementById("add-username-input");
const addPasswordInput = document.getElementById("add-password-input");
const addDomainInput = document.getElementById("add-domain-input");

openAddItemModalButton.addEventListener('click' ,function(){
    $('#addItemModal').modal('show');
});

cancelAddItemModalButton.addEventListener("click", function() {
    $('#addItemModal').modal('hide');
    addTitleInput.value = "";
    addUsernameInput.value = "";
    addPasswordInput.value = "";
    addDomainInput.value = "";

    addPasswordInput.type = 'password';
    toggleAddPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
    
    hideAlert(addItemModalAlert);
});

toggleAddPasswordButton.addEventListener('click',function(){

    if(addPasswordInput.type == 'password'){
        addPasswordInput.type = 'text';
        toggleAddPasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')";
        toggleAddPasswordButton.style.backgroundSize = "20px";
    }

    else{
        addPasswordInput.type = 'password';
        toggleAddPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
        toggleAddPasswordButton.style.backgroundSize = "20px";
    }

});

