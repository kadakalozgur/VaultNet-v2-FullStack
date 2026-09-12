// Sayfadaki item kartlarının yönetildiği kısım

// Alttaki fonksiyon için gerekli olan global değişkenler.
let vaultItems = [];
let currentPage = 1;
let totalPages = 1;

async function loadVaultItemPage(pageNumber = 1) {
    
    try{

        const result = await getVaultItems(pageNumber);

        if(result.IsSuccess){
            vaultItems = result.data.items;
            currentPage = result.data.currentPage;
            totalPages = result.data.totalPages;
        }

    }catch(error){
        console.error(error.message);
    }
    
}

document.addEventListener("DOMContentLoaded", async function() {
    loadVaultItemPage(1);
});

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

saveItemButton.addEventListener('click', async function(e) {

    e.preventDefault();

    hideAlert(addItemModalAlert);

    const title = addTitleInput.value.trim();
    const usernameOrEmail = addUsernameInput.value.trim();
    const password = addPasswordInput.value;
    const website = addDomainInput.value.trim();

    if(!title || !usernameOrEmail || !password || !password.trim()){
        showAlert(addItemModalAlert, "Please fill in all the required fields..", "warning");
        return;
    }

    if (title.length > 100) {
        showAlert(addItemModalAlert, "Title can be a maximum of 100 characters.", "warning");
        return;
    }

    if (usernameOrEmail.length > 150) {
        showAlert(addItemModalAlert, "Username or email can be a maximum of 150 characters.", "warning");
        return;
    }

    if (password.length > 256) {
        showAlert(addItemModalAlert, "Password can be a maximum of 256 characters.", "warning");
        return;
    }

    if (website.length > 150) {
        showAlert(addItemModalAlert, "Domain can be a maximum of 150 characters.", "warning");
        return;
    }

    addTitleInput.disabled = true;
    addUsernameInput.disabled = true;
    addPasswordInput.disabled = true;
    addDomainInput.disabled = true;
    saveItemButton.disabled = true;

    try{

        await addVaultItem(title,usernameOrEmail,password,website);

        $('#addItemModal').modal('hide'); 
        
        addTitleInput.value = "";
        addUsernameInput.value = "";
        addPasswordInput.value = "";
        addDomainInput.value = "";

        addPasswordInput.type = 'password';
        toggleAddPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";

    }catch(error){
        showAlert(addItemModalAlert,error.message,"danger");
    }finally{
        addTitleInput.disabled = false;
        addUsernameInput.disabled = false;
        addPasswordInput.disabled = false;
        addDomainInput.disabled = false;
        saveItemButton.disabled = false;
    }

});


