// URL'den idyi çektiğim kısım.
const urlParams = new URLSearchParams(window.location.search);
const itemId = urlParams.get('id');

document.addEventListener("DOMContentLoaded", async function () {

    if (!itemId) {
        window.location.href = "myvault.html";
        return;
    }

    await loadVaultItemDetails(itemId);

    // Kategorileri selectbox'a dolduran fonksiyon.
    await loadCategoriesForSelectBox();

});

// Itemin mevcut kategorisini tutmak için
let currentItemCategoryId = null;

async function loadVaultItemDetails(itemId) {
   
    try{

        const result = await getVaultItemById(itemId);

        if(result.isSuccess){

            const item = result.data;

            currentItemCategoryId = item.categoryId

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

// Update item modal kısmı

const openUpdateItemModalButton = document.getElementById("open-update-item-modal-button");
const cancelUpdateItemModalButton = document.getElementById("cancel-update-modal-button");
const updateItemButton = document.getElementById("update-item-button");
const toggleUpdatePasswordButton = document.getElementById("toggle-update-password");
const updateItemModalAlert = document.getElementById("update-item-alert");
const updateTitleInput = document.getElementById("update-title-input");
const updateUsernameInput = document.getElementById("update-username-input");
const updatePasswordInput = document.getElementById("update-password-input");
const updateDomainInput = document.getElementById("update-domain-input");
const categorySelect = document.getElementById("update-category-select");

openUpdateItemModalButton.addEventListener('click' ,function(){

    updateTitleInput.value = document.getElementById("title-label").innerText;
    updateUsernameInput.value = document.getElementById("detail-username").value;
    updatePasswordInput.value = document.getElementById("detail-password").value;
    updateDomainInput.value = document.getElementById("detail-domain").value;

    if (currentItemCategoryId) {
        categorySelect.value = currentItemCategoryId;
    } 

    else {
        categorySelect.value = "";
    }

    $('#updateItemModal').modal('show');
});

cancelUpdateItemModalButton.addEventListener("click", function() {
    $('#updateItemModal').modal('hide');
    updateTitleInput.value = "";
    updateUsernameInput.value = "";
    updatePasswordInput.value = "";
    updateDomainInput.value = "";
    categorySelect.value = "";

    updatePasswordInput.type = 'password';
    toggleUpdatePasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
    
    hideAlert(updateItemModalAlert);
});

toggleUpdatePasswordButton.addEventListener('click',function(){

    if(updatePasswordInput.type == 'password'){
        updatePasswordInput.type = 'text';
        toggleUpdatePasswordButton.style.backgroundImage = "url('../Media/Icons/close-eye-icon.png')";
        toggleUpdatePasswordButton.style.backgroundSize = "20px";
    }

    else{
        updatePasswordInput.type = 'password';
        toggleUpdatePasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";
        toggleUpdatePasswordButton.style.backgroundSize = "20px";
    }

});

updateItemButton.addEventListener('click', async function(e) {

    e.preventDefault();
    hideAlert(updateItemModalAlert);

    if (!itemId){
        return;
    }

    const title = updateTitleInput.value.trim();
    const usernameOrEmail = updateUsernameInput.value.trim();
    const password = updatePasswordInput.value;
    const domain = updateDomainInput.value.trim();
    const categoryId = categorySelect.value;

    const currentTitle = document.getElementById("title-label").innerText;
    const currentUsername = document.getElementById("detail-username").value;
    const currentPassword = document.getElementById("detail-password").value;
    const currentDomain = document.getElementById("detail-domain").value;

    if (title === currentTitle && usernameOrEmail === currentUsername && password === currentPassword && domain === currentDomain && categoryId === currentItemCategoryId) {
        showAlert(updateItemModalAlert, "No changes were made to update.", "warning");
        return;
    }

    if (!title || !usernameOrEmail || !password) {
        showAlert(updateItemModalAlert, "Please fill in all required fields.", "warning");
        return;
    }

    if (title.length > 100) {
        showAlert(updateItemModalAlert, "Title can be a maximum of 100 characters.", "warning");
        return;
    }

    if (usernameOrEmail.length > 150) {
        showAlert(updateItemModalAlert, "Username or email can be a maximum of 150 characters.", "warning");
        return;
    }

    if (password.length > 256) {
        showAlert(updateItemModalAlert, "Password can be a maximum of 256 characters.", "warning");
        return;
    }

    if (domain.length > 150) {
        showAlert(updateItemModalAlert, "Domain can be a maximum of 150 characters.", "warning");
        return;
    }

    updateItemButton.disabled = true;
    cancelUpdateItemModalButton.disabled = true;

    try {

        await updateVaultItem(itemId, title, usernameOrEmail, password, domain,categoryId);
        
        $('#updateItemModal').modal('hide');

        window.location.reload(); 
        
    } catch (error) {
        showAlert(updateItemModalAlert, error.message, "danger");
    } finally {
        updateItemButton.disabled = false;
        cancelUpdateItemModalButton.disabled = false;
    }

});

async function loadCategoriesForSelectBox() {
    
    try{

        const selectBox = document.getElementById("update-category-select");

        const result = await getAllCategories();

        if (result.isSuccess && result.data) {

            let optionsHtml = `<option value="">No Category (Uncategorized)</option>`;
            
            result.data.forEach(category => {
                optionsHtml += `<option value="${category.id}">${category.categoryName}</option>`;
            });

            selectBox.innerHTML = optionsHtml;

        }

    }catch(error){
        console.error(error.message);
    }

}

