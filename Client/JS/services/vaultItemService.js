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

async function getVaultItems(pageNumber = 1) {
    
    try{

        const response = await fetch(`${CONFIG.API_BASE_URL}${CONFIG.ENDPOINTS.GET_VAULT_ITEMS}?pageNumber=${pageNumber}`,{

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