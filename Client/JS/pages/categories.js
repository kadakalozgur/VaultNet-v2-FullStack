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