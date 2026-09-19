async function addVaultItem(itemName,usernameOrEmail,password,website = null) {
    
    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.ADD_VAULT_ITEM,{

            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Title: itemName,
                UsernameOrEmail: usernameOrEmail,
                Password: password,
                Domain: website
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

async function getVaultItems(pageNumber = 1, searchQuery = null, isFavorite = false) {
    
    try{

        let url =  `${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.GET_VAULT_ITEMS}?pageNumber=${pageNumber}`;

        if(searchQuery && searchQuery.trim() != ""){
            url += `&searchQuery=${encodeURIComponent(searchQuery.trim())}`;
        }

        if(isFavorite) {
            url += `&isFavorite=true`;
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

async function getVaultItemById(itemId) {
    
    try{

        const response = await fetch(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.GET_VAULT_ITEM_BY_ID}/${itemId}`,{

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

        const response = await fetch(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.DELETE_VAULT_ITEM}/${itemId}`,{

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

        return result;

    }catch(error){
        throw error;
    }

}

async function updateVaultItem(itemId,title,usernameOrEmail,password,domain) {
    
    try{

        const response = await fetch(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.UPDATE_VAULT_ITEM}/${itemId}`,{

            method: "PUT",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Title: title,
                UsernameOrEmail: usernameOrEmail,
                Password: password,
                Domain: domain
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

        const response = await fetch(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.TOGGLE_FAVORITE_VAULT_ITEM}/${itemId}`,{

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

        return result;

    }catch(error){
        throw error;
    }

}

async function getDashboardStats() {
    
    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.GET_DASHBOARD_STATS,{

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