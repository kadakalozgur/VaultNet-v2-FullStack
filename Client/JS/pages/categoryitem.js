document.addEventListener("DOMContentLoaded", async function () {

    const urlParams = new URLSearchParams(window.location.search);
    const categoryId = urlParams.get('id');

    if (!categoryId) {
        window.location.href = "categories.html";
        return;
    }

    await loadCategoryIdDetails(categoryId);

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