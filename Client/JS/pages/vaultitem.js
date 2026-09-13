document.addEventListener("DOMContentLoaded", async function () {

    const urlParams = new URLSearchParams(window.location.search);
    const itemId = urlParams.get('id');

    if (!itemId) {
        window.location.href = "myvault.html";
        return;
    }

    await loadVaultItemDetails(itemId);

});

async function loadVaultItemDetails(itemId) {
   
    try{

        const result = await getVaultItemById(itemId);

        if(result.isSuccess){

            const item = result.data;

            document.getElementById("title-label").textContent = item.title;
            document.getElementById("detail-username").value = item.usernameOrEmail;
            document.getElementById("detail-password").value = item.password;
            document.getElementById("detail-domain").value = item.domain;

            document.getElementById("item-icon").src = item.domain 
                ? `https://www.google.com/s2/favicons?domain=${item.domain}&sz=128` 
                : "Media/Icons/items-default-icon.png";

            const createdDate = new Date(item.createdAt).toLocaleDateString('en-US', { 
                year: 'numeric', 
                month: 'short', 
                day: 'numeric' 
            });

            const updatedDate = new Date(item.updatedAt).toLocaleDateString('en-US', { 
                year: 'numeric', 
                month: 'short', 
                day: 'numeric' 
            });

            document.getElementById("detail-created-date").textContent = createdDate;
            document.getElementById("detail-updated-date").textContent = updatedDate;    

        }

    }catch(error){
        console.error(error.message);
    }

}

// Şifreyi gösterme-gizleme kısmı

const togglePasswordButton = document.getElementById("toggle-password-button");
const passwordInput = document.getElementById("detail-password");

togglePasswordButton.addEventListener('click', function(){

    if(passwordInput.type == 'password'){

        passwordInput.type = 'text';
        togglePasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-blue-icon.png')"
        togglePasswordButton.style.backgroundSize = "18px";

    }

    else{

        passwordInput.type = 'password';
        togglePasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-blue-icon.png')"
        togglePasswordButton.style.backgroundSize = "18px";

    }

});

// Item silme modal kısmı

const openDeleteItemModalButton = document.getElementById("open-delete-item-modal-button");
const deleteItemConfirmButton = document.getElementById("confirm-delete-item-button");
const deleteItemCancelButton = document.getElementById("cancel-delete-item-modal-button");
const deleteItemModalAlert = document.getElementById("delete-item-modal-alert");

openDeleteItemModalButton.addEventListener('click' ,function(){
    $('#deleteItemModal').modal('show');
});

deleteItemCancelButton.addEventListener("click", function() {
    $('#deleteItemModal').modal('hide');
    hideAlert(deleteItemModalAlert);
});

deleteItemConfirmButton.addEventListener('click' ,async function(e){
    
    e.preventDefault();

    hideAlert(deleteItemModalAlert);

    const urlParams = new URLSearchParams(window.location.search);
    const itemId = urlParams.get('id');

    if (!itemId) {
        return;
    }

    deleteItemConfirmButton.disabled = true;
    deleteItemCancelButton.disabled = true;

    try{

        await deleteVaultItem(itemId);

        window.location.href = "myvault.html";

    }catch(error){
        showAlert(deleteItemModalAlert,error.message,"danger");
    }finally{
        deleteItemConfirmButton.disabled = false;
        deleteItemCancelButton.disabled = false;
    }

});

