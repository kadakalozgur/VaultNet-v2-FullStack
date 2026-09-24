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