async function addVaultItem(itemName,usernameOrEmail,password,website = null,categoryId = null) {
    
    try{

        const response = await fetchWithAuth(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.ADD_VAULT_ITEM,{

            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Title: itemName,
                UsernameOrEmail: usernameOrEmail,
                Password: password,
                Domain: website,
                CategoryId: categoryId
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

async function getVaultItems(pageNumber = 1, searchQuery = null, isFavorite = false, categoryId = null) {
    
    try{

        let url =  `${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.GET_VAULT_ITEMS}?pageNumber=${pageNumber}`;

        if(searchQuery && searchQuery.trim() != ""){
            url += `&searchQuery=${encodeURIComponent(searchQuery.trim())}`;
        }

        if(isFavorite) {
            url += `&isFavorite=true`;
        }

        if (categoryId) {
            url += `&categoryId=${categoryId}`;
        }

        const response = await fetchWithAuth(url,{

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

async function getVaultItemById(itemId) {
    
    try{

        const response = await fetchWithAuth(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.GET_VAULT_ITEM_BY_ID}/${itemId}`,{

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

async function deleteVaultItem(itemId) {
    
    try{

        const response = await fetchWithAuth(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.DELETE_VAULT_ITEM}/${itemId}`,{

            method: "DELETE",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });

        const result = await response.json();

        if (!response.ok) {
            throw new Error(getApiErrorMessage(result));
        }

        sessionStorage.removeItem("dashboard_stats_cache");

        return result;

    }catch(error){
        throw error;
    }

}

async function updateVaultItem(itemId,title,usernameOrEmail,password,domain,categoryId) {
    
    try{

        const response = await fetchWithAuth(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.UPDATE_VAULT_ITEM}/${itemId}`,{

            method: "PUT",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Title: title,
                UsernameOrEmail: usernameOrEmail,
                Password: password,
                Domain: domain,
                CategoryId: categoryId
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

async function toggleFavoriteItemAsync(itemId) {
    
    try{

        const response = await fetchWithAuth(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.TOGGLE_FAVORITE_VAULT_ITEM}/${itemId}`,{

            method: "PATCH",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });

        const result = await response.json();

        if (!response.ok) {
            throw new Error(getApiErrorMessage(result));
        }

        sessionStorage.removeItem("dashboard_stats_cache");

        return result;

    }catch(error){
        throw error;
    }

}

async function getDashboardStats() {
    
    try{

        const response = await fetchWithAuth(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.GET_DASHBOARD_STATS,{

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