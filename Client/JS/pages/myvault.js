// Sayfadaki item kartlarının yönetildiği kısım

// Alttaki fonksiyon için gerekli olan global değişkenler.
let vaultItems = [];
let currentPage = 1;
let totalPages = 1;
let currentSearchQuery = "";

// Sayfa urlsinden favoriler sayfasında olup olmadığıma baktım. Favoriler sayfasında isem ona göre API'ye istek atıcam.
const isFavoritePage = window.location.pathname.toLowerCase().includes('favorites');

async function loadVaultItemPage(pageNumber = 1, searchQuery = "") {
    
    try{

        currentSearchQuery = searchQuery;

        const result = await getVaultItems(pageNumber,currentSearchQuery,isFavoritePage);

        if(result.isSuccess){
            vaultItems = result.data.items;
            currentPage = result.data.currentPage;
            totalPages = result.data.totalPages;

            showCardVaultItems();
            loadPageNumberButtons();
        }

    }catch(error){
        console.error(error.message);
    }
    
}

function showCardVaultItems() {
    
    const container = document.getElementById("vault-items-container");
    container.innerHTML = "";

    if (!vaultItems || vaultItems.length === 0) {

        container.innerHTML = `
            <div class="col-12 text-center mt-5">
                <p class="text-muted">No items found in your vault.</p>
            </div>`;

        return;

    }

    vaultItems.forEach(item => {

        const iconUrl = item.domain 
            ? `https://www.google.com/s2/favicons?domain=${item.domain}&sz=64` 
            : `Media/Icons/items-default-icon.png`; 

        const updatedDate = new Date(item.updatedAt).toLocaleDateString('en-US', { 
            year: 'numeric', 
            month: 'short', 
            day: 'numeric' 
        });

        const createdDate = new Date(item.createdAt).toLocaleDateString('en-US', { 
            year: 'numeric', 
            month: 'short', 
            day: 'numeric' 
        });

        // Bootstrap hazır classları ile yaptım.
        const cardHtml = `
            <div class="col-12 col-md-6 custom-vault-grid mb-4 animate__animated animate__fadeIn">
                <div class="card h-100 border-0 shadow-sm vault-card p-2" style="border-radius: 16px; transition: transform 0.2s; background-color: #ffffff; position: relative;">
                    <div class="card-body p-3 d-flex flex-column">
                        
                        <button class="btn btn-sm btn-link p-0 shadow-none" 
                                style="text-decoration: none; position: absolute; top: 16px; right: 16px;"
                                onclick="toggleFavoriteItem('${item.id}', this)">
                            <span class="d-inline-block" style="font-size: 30px; line-height: 1; color: ${item.isFavorite ? '#4965fa' : '#d1d5db'};">★</span>
                        </button>

                        <div class="d-flex align-items-center mb-3 w-100" style="padding-right: 28px;">
                            <div class="mr-3 d-flex align-items-center justify-content-center flex-shrink-0" style="background-color: #f3f4f6; border-radius: 12px; width: 48px; height: 48px; padding: 10px;">
                                <img src="${iconUrl}" alt="icon" style="max-width: 100%; max-height: 100%; border-radius: 6px;">
                            </div>
                            
                            <div style="flex: 1; min-width: 0;">
                                <h5 class="card-title mb-0 font-weight-bold text-truncate" style="color: #111827; font-size: 16px;">
                                    ${item.title}
                                </h5>
                            </div>
                        </div>
                        
                        <div class="mb-1 mt-2">
                            <div class="d-flex align-items-center mb-1">
                                <img src="Media/Icons/updated-icon.png" alt="Updated" style="width: 14px; height: 14px; flex-shrink: 0;" class="mr-2 opacity-50">
                                <span class="text-truncate" style="font-size: 13px; color: #6b7280; font-weight: 500; min-width: 0;">${updatedDate}</span>
                            </div>
                            <div class="d-flex align-items-center">
                                <img src="Media/Icons/calendar-icon.png" alt="Created" style="width: 14px; height: 14px; flex-shrink: 0;" class="mr-2 opacity-50">
                                <span class="text-truncate" style="font-size: 13px; color: #6b7280; font-weight: 500; min-width: 0;">${createdDate}</span>
                            </div>
                        </div>
                        
                        <div class="d-flex justify-content-end mt-auto pt-2">
                            <button class="btn btn-sm d-flex align-items-center justify-content-center px-4 py-2 font-weight-bold" style="background-color: #eff6ff; color: #2563eb; border-radius: 20px; border: none;" onclick="window.location.href='vaultitem.html?id=${item.id}'">
                                <img src="Media/Icons/view-icon.png" alt="View" style="width: 18px; height: 18px; flex-shrink: 0;" class="mr-2">
                                View
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        container.insertAdjacentHTML('beforeend', cardHtml);

    });

}

async function toggleFavoriteItem(itemId,button) {
    
    const starSpan = button.querySelector("span");
    const previousColor = starSpan.style.color;

    function animationEnd(){
        starSpan.classList.remove("animate__animated", "animate__bounceIn");
        starSpan.removeEventListener("animationend", animationEnd);
    }

    starSpan.addEventListener("animationend", animationEnd);
    starSpan.classList.add("animate__animated", "animate__bounceIn");

    const isCurrentlyFavorite = starSpan.style.color === "rgb(73, 101, 250)" || starSpan.style.color === "#4965fa";
    starSpan.style.color = isCurrentlyFavorite ? "#d1d5db" : "#4965fa";

    button.disabled = true ;

    try{

        await toggleFavoriteItemAsync(itemId);

        const targetItem = vaultItems.find(item => item.id === itemId);

        if (targetItem) {
            targetItem.isFavorite = !isCurrentlyFavorite;
        }

        if(isFavoritePage && isCurrentlyFavorite){
            
            const targetPage = (vaultItems.length === 1 && currentPage > 1) ? currentPage - 1 : currentPage;

            await loadVaultItemPage(targetPage, currentSearchQuery);

        }

    }catch(error){
        starSpan.style.color = previousColor;
        console.error(error.message);
    }finally{
        if (document.body.contains(button)) {
            button.disabled = false;
        }
    }

}

function loadPageNumberButtons() {

    const container = document.getElementById("pagination-container");
    container.innerHTML = ""

    if(totalPages <= 1){
        return ;
    }

    let html = "";

    html += `
        <button class="page-btn" onclick="loadVaultItemPage(${currentPage - 1}, '${currentSearchQuery}')" ${currentPage === 1 ? 'disabled' : ''}>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="15 18 9 12 15 6"></polyline>
            </svg>
        </button>
    `;

    const maxPageShow = 5 ;
    let startPage = Math.max(1,currentPage - 2);
    let endPage = Math.min(totalPages,currentPage + 2);

    if(currentPage <= 2){
        endPage = Math.min(totalPages,maxPageShow);
    }

    if(currentPage >= totalPages - 1){
        startPage = Math.max(1, totalPages - maxPageShow + 1);
    }

    for(let i = startPage; i <= endPage; i++){
        const activeClass = i == currentPage ? "active" : "";
        html += `<button class="page-btn ${activeClass}" onclick="loadVaultItemPage(${i}, '${currentSearchQuery}')">${i}</button>`;
    }

    html += `
        <button class="page-btn" onclick="loadVaultItemPage(${currentPage + 1}, '${currentSearchQuery}')" ${currentPage === totalPages ? 'disabled' : ''}>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="9 18 15 12 9 6"></polyline>
            </svg>
        </button>
    `;

    container.innerHTML = html;

}

document.addEventListener("DOMContentLoaded", async function() {
    loadVaultItemPage(1);
    await loadCategoriesForSelectBox();

    hideGlobalLoader();
});

// Arama input kısmı enter dinleyicisi kısmı

const searchInput = document.getElementById("vault-search-input");

searchInput.addEventListener("keydown", async function(e) {

    if (e.key === "Enter") {

        e.preventDefault(); 

        const query = searchInput.value.trim();

        await loadVaultItemPage(1, query); 
        
    }
});

// Add item modal kısmı

const openAddItemModalButton = document.getElementById("open-add-item-modal-button");

// Favorites sayfası ile MyVault bu scripti kullanıyor. Favorites sayfasında modal olmadığı için bu kısım gereksiz O yüzden if bloğuna aldım.
if(openAddItemModalButton){

const cancelAddItemModalButton = document.getElementById("cancel-add-modal-button");
const saveItemButton = document.getElementById("save-item-button");
const toggleAddPasswordButton = document.getElementById("toggle-add-password");
const addItemModalAlert = document.getElementById("add-item-alert");
const addTitleInput = document.getElementById("add-title-input");
const addUsernameInput = document.getElementById("add-username-input");
const addPasswordInput = document.getElementById("add-password-input");
const addDomainInput = document.getElementById("add-domain-input");
const addCategorySelect = document.getElementById("add-category-select");

openAddItemModalButton.addEventListener('click' ,function(){
    $('#addItemModal').modal('show');
});

cancelAddItemModalButton.addEventListener("click", function() {
    $('#addItemModal').modal('hide');
    addTitleInput.value = "";
    addUsernameInput.value = "";
    addPasswordInput.value = "";
    addDomainInput.value = "";
    addCategorySelect.value = "";

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
    const categoryId = addCategorySelect.value || null;

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
    cancelAddItemModalButton.disabled = true;
    addCategorySelect.disabled = true;
    
    try{

        await addVaultItem(title,usernameOrEmail,password,website,categoryId);

        $('#addItemModal').modal('hide'); 
        
        addTitleInput.value = "";
        addUsernameInput.value = "";
        addPasswordInput.value = "";
        addDomainInput.value = "";
        searchInput.value = "";
        addCategorySelect.value = "";

        addPasswordInput.type = 'password';
        toggleAddPasswordButton.style.backgroundImage = "url('../Media/Icons/open-eye-icon.png')";

        await loadVaultItemPage(1);

    }catch(error){
        showAlert(addItemModalAlert,error.message,"danger");
    }finally{
        addTitleInput.disabled = false;
        addUsernameInput.disabled = false;
        addPasswordInput.disabled = false;
        addDomainInput.disabled = false;
        saveItemButton.disabled = false;
        cancelAddItemModalButton.disabled = false;
        addCategorySelect.disabled = false;
    }

});

}

async function loadCategoriesForSelectBox() {
    
    try{

        const selectBox = document.getElementById("add-category-select");

        if(!selectBox){
            return;
        }

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




