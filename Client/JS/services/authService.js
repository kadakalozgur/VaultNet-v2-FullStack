async function login(email,password){

    try{

        const response = await fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.LOGIN,
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
            throw new Error(result.message || "Login failed.");
        }  

        if (result.data && result.data.token) {
                sessionStorage.setItem("token", result.data.token);
        }

        return result;

    }catch(error){
        console.error("Error: ", error.message);
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
            throw new Error(result.message || "Registration failed.");
        }

        return result ;

    }catch(error){
        console.error("Error: ",error.message);
        throw error;
    }
}