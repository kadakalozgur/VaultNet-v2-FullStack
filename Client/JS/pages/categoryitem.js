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

updateCategoryConfirmButton.addEventListener("click", async function(e) {

    e.preventDefault();

    hideAlert(updateCategoryModalAlert);

    const urlParams = new URLSearchParams(window.location.search);
    const categoryId = urlParams.get('id');

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

    const urlParams = new URLSearchParams(window.location.search);
    const categoryId = urlParams.get('id');

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