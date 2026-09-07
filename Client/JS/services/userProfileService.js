async function updateProfilePicture(photoFile) {

    try{

        const formData = new FormData();
        formData.append("photo", photoFile);

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.UPDATE_PROFILE_PICTURE,{
            method: "POST",
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