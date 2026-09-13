async function updateProfilePicture(photoFile) {

    try{

        const formData = new FormData();
        formData.append("photo", photoFile);

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.UPDATE_PROFILE_PICTURE,{
            method: "PUT",
            credentials: "include",
            body: formData
        }); 

        const result = await response.json();

        if (!response.ok) {
            throw new Error(getApiErrorMessage(result));
        }

        return result;
        
    } catch(error){
        throw error;
    }
    
}

async function updateEmail(newEmailAddress) {
    
    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.UPDATE_EMAIL_ADDRESS,{

            method: "PUT",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({newEmail: newEmailAddress})
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

async function changePassword(currentPassword,newPassword){

    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.CHANGE_PASSWORD,{

            method: "PUT",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                currentPassword: currentPassword,
                newPassword: newPassword
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

async function deleteAccount(currentPassword) {
    
    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.DELETE_ACCOUNT,{

            method: "DELETE",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                currentPassword: currentPassword
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

async function removeProfilePicture() {
    
    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.REMOVE_PROFILE_PICTURE,{

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