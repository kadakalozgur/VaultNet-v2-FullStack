// Sayfadaki category kartlarının yönetildiği kısım

// Alttaki fonksiyon için gerekli olan global değişkenler.
let categories = [];
let currentPage = 1;
let totalPages = 1;
let currentSearchQuery = "";

async function loadCategoryPage(pageNumber = 1, searchQuery = "") {
    
    try {

        currentSearchQuery = searchQuery;

        const result = await getCategories(pageNumber, currentSearchQuery);

        if (result.isSuccess) {
            categories = result.data.items;
            currentPage = result.data.currentPage;
            totalPages = result.data.totalPages;

            showCardCategories();
            loadPageNumberButtons();
        }

    } catch (error) {
        console.error(error.message);
    }
}

function showCardCategories() {
    
    const container = document.getElementById("categories-container");
    
    container.innerHTML = "";

    if (!categories || categories.length === 0) {

        container.innerHTML = `
            <div class="col-12 text-center mt-5">
                <p class="text-muted">No categories found.</p>
            </div>`;

        return;

    }

    categories.forEach(category => {

        const updatedDate = new Date(category.updatedAt).toLocaleDateString('en-US', { 
            year: 'numeric', 
            month: 'short', 
            day: 'numeric' 
        });

        const createdDate = new Date(category.createdAt).toLocaleDateString('en-US', { 
            year: 'numeric', 
            month: 'short', 
            day: 'numeric' 
        });

        const cardHtml = `
            <div class="col-12 col-md-6 custom-category-grid mb-4 animate__animated animate__fadeIn">
                <div class="card h-100 border-0 shadow-sm vault-card p-2" style="border-radius: 16px; transition: transform 0.2s; background-color: #ffffff; position: relative;">
                    <div class="card-body p-3 d-flex flex-column">
                        
                        <div class="d-flex align-items-center mb-3 w-100" style="padding-right: 28px;">
                            <div class="mr-3 d-flex align-items-center justify-content-center flex-shrink-0" style="background-color: #f3f4f6; border-radius: 12px; width: 48px; height: 48px; padding: 10px;">
                                <img src="Media/Icons/folder-icon-active.png" alt="icon" style="max-width: 100%; max-height: 100%; border-radius: 6px;">
                            </div>
                            
                            <div style="flex: 1; min-width: 0;">
                                <h5 class="card-title mb-0 font-weight-bold text-truncate" style="color: #111827; font-size: 16px;">
                                    ${category.categoryName}
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
                            <button class="btn btn-sm d-flex align-items-center justify-content-center px-4 py-2 font-weight-bold" style="background-color: #eff6ff; color: #2563eb; border-radius: 20px; border: none;" onclick="window.location.href='categoryitem.html?id=${category.id}'">
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

    container.innerHTML = ""

    if(totalPages <= 1){
        return ;
    }

    let html = "";

    html += `
        <button class="page-btn" onclick="loadCategoryPage(${currentPage - 1}, '${currentSearchQuery}')" ${currentPage === 1 ? 'disabled' : ''}>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="15 18 9 12 15 6"></polyline>
            </svg>
        </button>
    `;

    const maxPageShow = 5;
    let startPage = Math.max(1, currentPage - 2);
    let endPage = Math.min(totalPages, currentPage + 2);

    if(currentPage <= 2){
        endPage = Math.min(totalPages, maxPageShow);
    }

    if(currentPage >= totalPages - 1){
        startPage = Math.max(1, totalPages - maxPageShow + 1);
    }

    for(let i = startPage; i <= endPage; i++){
        const activeClass = i == currentPage ? "active" : "";
        html += `<button class="page-btn ${activeClass}" onclick="loadCategoryPage(${i}, '${currentSearchQuery}')">${i}</button>`;
    }

    html += `
        <button class="page-btn" onclick="loadCategoryPage(${currentPage + 1}, '${currentSearchQuery}')" ${currentPage === totalPages ? 'disabled' : ''}>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="9 18 15 12 9 6"></polyline>
            </svg>
        </button>
    `;

    container.innerHTML = html;

}

document.addEventListener("DOMContentLoaded", async function() {
    loadCategoryPage(1);
});

// Arama input kısmı enter dinleyicisi kısmı

const searchInput = document.getElementById("category-search-input");

searchInput.addEventListener("keydown", async function(e) {

    if (e.key === "Enter") {

        e.preventDefault(); 

        const query = searchInput.value.trim();

        await loadCategoryPage(1, query); 

    }

});


// Kategori Oluşturma Modal Kısmı

const openCreateCategoryModalButton = document.getElementById("open-add-category-modal-button");
const cancelCreateCategoryModalButton = document.getElementById("cancel-add-category-modal-button");
const saveCreateCategoryModalButton = document.getElementById("save-category-button");
const createCategoryModalAlert = document.getElementById("add-category-alert");
const categoryNameInput = document.getElementById("add-category-name-input");

openCreateCategoryModalButton.addEventListener('click' ,function(){
    $('#addCategoryModal').modal('show');
});

cancelCreateCategoryModalButton.addEventListener('click' ,function(){
    $('#addCategoryModal').modal('hide');
    categoryNameInput.value = "";
    hideAlert(createCategoryModalAlert);
});

saveCreateCategoryModalButton.addEventListener('click' ,async function(e){

    e.preventDefault();
    
    hideAlert(createCategoryModalAlert);

    const categoryName = categoryNameInput.value.trim();

    if(!categoryName){
        showAlert(createCategoryModalAlert, "Please fill in all the required fields.", "warning");
        return;
    }

    if (categoryName.length > 100) {
        showAlert(createCategoryModalAlert, "Category name can be a maximum of 100 characters.", "warning");
        return;
    }

    categoryNameInput.disabled = true;
    saveCreateCategoryModalButton.disabled = true;

    try{

        await createCategory(categoryName);

        $('#addCategoryModal').modal('hide');

        categoryNameInput.value = "";
        searchInput.value = "";

        await loadCategoryPage(1);


    }catch(error){
        showAlert(createCategoryModalAlert,error.message,"danger");
    }finally{
        categoryNameInput.disabled = false;
        saveCreateCategoryModalButton.disabled = false;
    }

});