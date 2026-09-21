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


    }catch(error){
        showAlert(createCategoryModalAlert,error.message,"danger");
    }finally{
        categoryNameInput.disabled = false;
        saveCreateCategoryModalButton.disabled = false;
    }

});