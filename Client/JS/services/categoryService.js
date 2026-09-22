async function createCategory (categoryName) {
    
    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.CREATE_CATEGORY,{

            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                CategoryName: categoryName
            })
        });

        const result = await response.json();

        if (!response.ok) {
            throw new Error(getApiErrorMessage(result));
        }

        return result;

    }catch(error){
        throw error;
    }

}

async function getCategories(pageNumber = 1,searchQuery = null) {
    
    try{

        let url =  `${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.GET_CATEGORIES}?pageNumber=${pageNumber}`;

        if(searchQuery && searchQuery.trim() != ""){
            url += `&searchQuery=${encodeURIComponent(searchQuery.trim())}`;
        }

        const response = await fetch(url,{

            method: "GET",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });

        const result = await response.json();

        if (!response.ok) {
            throw new Error(getApiErrorMessage(result));
        }

        return result;


    }catch(error){
        throw error;
    }

}