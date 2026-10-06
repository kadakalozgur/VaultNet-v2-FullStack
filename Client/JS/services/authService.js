async function login(email,password){

    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.LOGIN,
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include",
                body: JSON.stringify({
                    email: email,
                    password: password
                })
            }
        );

        const result = await response.json();

        if(!response.ok){
            throw new Error(getApiErrorMessage(result));
        }  

        return result;

    }catch(error){
        throw error;
    }
}

async function register(email,password) {
    
    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.REGISTER,
            {
                method: "POST",
                headers: {
                   "Content-Type": "application/json"
                },
                body: JSON.stringify({
                        email: email,
                        password: password
                    })
            }
        );

        const result = await response.json();

        if(!response.ok){
            throw new Error(getApiErrorMessage(result));
        }

        return result ;

    }catch(error){
        throw error;
    }
}

async function logout() {
    
    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.LOGOUT,
            {
                method: "POST",
                credentials: "include"
            }
        );

        const result = await response.json();

        if(!response.ok){
            throw new Error(getApiErrorMessage(result));
        }

        return result ;

    }catch(error){
        throw error;
    }

}

async function logoutAllDevices() {

    try {
        
        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.LOGOUT_ALL_DEVICES, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            credentials: "include"
        });

        if (response.status === 401) {
            throw new Error("Your session has expired. Please log in again.");
        }

        const result = await response.json();

        if (!response.ok) {
            throw new Error(getApiErrorMessage(result));
        }

        return result;

    } catch (error) {
        throw error;
    }
}