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

async function getProfileWithCache() {
    
    const cachedData = sessionStorage.getItem("user_profile");

    if(cachedData){
        return JSON.parse(cachedData);
    }

    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.GET_USER_PROFILE, {
            method: "GET",
            credentials: "include"
        });

        const result = await response.json();

        if (!response.ok) {
            throw new Error(getApiErrorMessage(result));
        }

        sessionStorage.setItem("user_profile", JSON.stringify(result));

        return result;

    }catch(error){
        throw error;
    }

}