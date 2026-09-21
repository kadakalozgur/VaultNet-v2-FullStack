// Kategori Oluşturma Modal Kısmı

const openCreateCategoryModalButton = document.getElementById("open-add-category-modal-button");

openCreateCategoryModalButton.addEventListener('click' ,function(){
    $('#addCategoryModal').modal('show');
});