// URL'den idyi çektiğim kısım.
const urlParams = new URLSearchParams(window.location.search);
const categoryId = urlParams.get('id');

document.addEventListener("DOMContentLoaded", async function () {

    if (!categoryId) {
        window.location.href = "categories.html";
        return;
    }

    await loadCategoryIdDetails(categoryId);
    await loadCategoryItemPage(1);

    hideGlobalLoader();

});

async function loadCategoryIdDetails(categoryId) {
    
    try{ 

        const result = await getCategoryById(categoryId);

        if(result.isSuccess){
            document.getElementById("category-name").textContent = result.data.categoryName;
        }

    }catch(error){
        console.error(error.message);
    }
}

// Alttaki fonksiyon için gerekli olan global değişkenler.
let categoryVaultItems = [];
let currentPage = 1;
let totalPages = 1;
let currentSearchQuery = "";

async function loadCategoryItemPage(pageNumber = 1, searchQuery = "") {
    
    try {

        currentSearchQuery = searchQuery;
        
        const result = await getVaultItems(pageNumber, currentSearchQuery, false, categoryId);

        if (result.isSuccess) {

            categoryVaultItems = result.data.items;
            currentPage = result.data.currentPage;
            totalPages = result.data.totalPages;

            showCardVaultItems();
            loadPageNumberButtons();

        }

    } catch (error) {
        console.error(error.message);
    }

}

function showCardVaultItems() {

    const container = document.getElementById("category-items-container");

    container.innerHTML = "";

    if (!categoryVaultItems || categoryVaultItems.length === 0) {

        container.innerHTML = `
            <div class="col-12 text-center mt-5">
                <p class="text-muted">No items found in this category.</p>
            </div>`;

        return;

    }

    categoryVaultItems.forEach(item => {

        const safeTitle = escapeHTML(item.title);
        const safeDomain = escapeHTML(item.domain);

        const iconUrl = safeDomain 
            ? `https://www.google.com/s2/favicons?domain=${safeDomain}&sz=64` 
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

        const cardHtml = `
            <div class="col-12 col-md-6 col-lg-4 col-xl-4 custom-vault-grid mb-4 animate__animated animate__fadeIn">
                <div class="card h-100 border-0 shadow-sm vault-card p-2" style="border-radius: 16px; transition: transform 0.2s; background-color: #ffffff;">
                    <div class="card-body p-3 d-flex flex-column">
                        
                        <div class="d-flex align-items-center mb-3 w-100">
                            <div class="mr-3 d-flex align-items-center justify-content-center flex-shrink-0" style="background-color: #f3f4f6; border-radius: 12px; width: 48px; height: 48px; padding: 10px;">
                                <img src="${iconUrl}" alt="icon" style="max-width: 100%; max-height: 100%; border-radius: 6px;">
                            </div>
                            
                            <div style="flex: 1; min-width: 0;">
                                <h5 class="card-title mb-0 font-weight-bold text-truncate" style="color: #111827; font-size: 16px;">
                                    ${safeTitle}
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

function loadPageNumberButtons() {

    const container = document.getElementById("pagination-container");

    container.innerHTML = "";

    if (totalPages <= 1){
        return;
    } 

    let html = "";

    html += `
        <button class="page-btn" onclick="loadCategoryItemPage(${currentPage - 1}, currentSearchQuery)" ${currentPage === 1 ? 'disabled' : ''}>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="15 18 9 12 15 6"></polyline>
            </svg>
        </button>
    `;

    const maxPageShow = 5;
    let startPage = Math.max(1, currentPage - 2);
    let endPage = Math.min(totalPages, currentPage + 2);

    if (currentPage <= 2) {
        endPage = Math.min(totalPages, maxPageShow);
    }

    if (currentPage >= totalPages - 1) {
        startPage = Math.max(1, totalPages - maxPageShow + 1);
    }

    for (let i=startPage;i<=endPage;i++) {
        const activeClass = i == currentPage ? "active" : "";
        html += `<button class="page-btn ${activeClass}" onclick="loadCategoryItemPage(${i}, currentSearchQuery)">${i}</button>`;
    }

    html += `
        <button class="page-btn" onclick="loadCategoryItemPage(${currentPage + 1}, currentSearchQuery)" ${currentPage === totalPages ? 'disabled' : ''}>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="9 18 15 12 9 6"></polyline>
            </svg>
        </button>
    `;

    container.innerHTML = html;

}

// Arama input kısmı enter dinleyicisi kısmı

const searchInput = document.getElementById("category-item-search-input");

searchInput.addEventListener("keydown", async function(e) {

    if (e.key === "Enter") {

        e.preventDefault(); 

        const query = searchInput.value.trim();

        await loadCategoryItemPage(1, query);
        
    }
});

// Update Category Modal Kısmı

const openUpdateCategoryModalButton = document.getElementById("open-update-category-modal-button");
const updateCategoryNameInput = document.getElementById("update-name-input");
const updateCategoryConfirmButton = document.getElementById("update-category-button");
const updateCategoryCancelButton = document.getElementById("cancel-update-category-modal-button");
const updateCategoryModalAlert = document.getElementById("update-category-alert");

openUpdateCategoryModalButton.addEventListener('click' ,function(){
    updateCategoryNameInput.value = document.getElementById("category-name").innerText;
    $('#updateCategoryModal').modal('show');
});

updateCategoryCancelButton.addEventListener("click", function() {
    $('#updateCategoryModal').modal('hide');
    updateCategoryNameInput.value = "";
    hideAlert(updateCategoryModalAlert);
});

updateCategoryConfirmButton.addEventListener("click", async function(e) {

    e.preventDefault();

    hideAlert(updateCategoryModalAlert);

    const name = updateCategoryNameInput.value.trim();
    const currentName = document.getElementById("category-name").innerText.trim();

    if(!name){
        showAlert(updateCategoryModalAlert, "Please fill in all required fields.", "warning");
        return;
    }

    if(name.length > 100){
        showAlert(updateCategoryModalAlert, "Name can be a maximum of 100 characters.", "warning");
        return;
    }

    if(name==currentName){
        showAlert(updateCategoryModalAlert, "No changes were made to update.", "warning");
        return;
    }
    
    updateCategoryConfirmButton.disabled = true;
    updateCategoryCancelButton.disabled = true;
    updateCategoryNameInput.disabled = true;

    try{
        
        await updateCategory(categoryId,name);

        $('#updateCategoryModal').modal('hide');

        window.location.reload(); 

    }catch(error){
        showAlert(updateCategoryModalAlert, error.message, "danger");
    }finally{
        updateCategoryConfirmButton.disabled = false;
        updateCategoryCancelButton.disabled = false;
        updateCategoryNameInput.disabled = false;
    }

});

// Delete Category Modal Kısmı

const openDeleteCategoryModalButton = document.getElementById("open-delete-category-modal-button");
const deleteCategoryConfirmButton = document.getElementById("confirm-delete-category-button");
const deleteCategoryCancelButton = document.getElementById("cancel-delete-category-modal-button");
const deleteCategoryModalAlert = document.getElementById("delete-category-modal-alert");

openDeleteCategoryModalButton.addEventListener('click' ,function(){
    $('#deleteCategoryModal').modal('show');
});

deleteCategoryCancelButton.addEventListener("click", function() {
    $('#deleteCategoryModal').modal('hide');
    hideAlert(deleteCategoryModalAlert);
});

deleteCategoryConfirmButton.addEventListener('click' ,async function(e){

    e.preventDefault();

    hideAlert(deleteCategoryModalAlert);

    deleteCategoryConfirmButton.disabled = true;
    deleteCategoryCancelButton.disabled = true;

    try{

        await deleteCategory(categoryId);

        window.location.href = "categories.html";

    }catch(error){
        showAlert(deleteCategoryModalAlert, error.message, "danger");
    }finally{
        deleteCategoryConfirmButton.disabled = false;
        deleteCategoryCancelButton.disabled = false;
    }

});