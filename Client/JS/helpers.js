function showAlert(alertBox,message,type){

    alertBox.className = `alert alert-${type} animate__animated animate__fadeIn`;
    alertBox.textContent = message;
    alertBox.classList.remove("d-none");

}

function hideAlert(alertBox){

    alertBox.classList.add("d-none");
    
}

// Bu kısımda çıkış yapıldıktan sonra tarayıcının geri-ileri tuşları ile gezilmesini engelledim.
// BFCache olayını hallettim kısacası.
const currentPath = window.location.pathname.toLowerCase();

if (!currentPath.includes("index.html") && !currentPath.includes("register.html") && currentPath !== "/" && currentPath !== "") {
    
    if (localStorage.getItem("isLoggedIn") !== "true") {
       // window.location.replace("index.html");
    }

    window.addEventListener('pageshow', function (event) {
        if (localStorage.getItem("isLoggedIn") !== "true") {
          //  window.location.replace("index.html");
        }
    });

}

// API'den gelen .net'e özel validation errorlarını ui'a basmak için oluşturduğum fonksiyon
function getApiErrorMessage(result){

    if (result.errors && Object.keys(result.errors).length > 0) {
        return Object.values(result.errors)[0][0];
    }
    
    return result.message || "An unknown error occurred.";

}

// Alttaki fonksiyon için yardımcı fonksiyon
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

// Sayfa yüklendiği an arkadan User bilgilerini çektiğim kısım
document.addEventListener("DOMContentLoaded", async function() {

    if (localStorage.getItem("isLoggedIn") !== "true") {
        return;
    }

    try {

        const response = await getProfileWithCache();
        const userData = response.data;

        if (userData) {

            if (userData.profilePictureUrl) {

                document.querySelectorAll('.profile-avatar-img').forEach(img => {
                    img.src = userData.profilePictureUrl;
                });

            }

            else{

                document.querySelectorAll('.profile-avatar-img').forEach(img => {
                    img.src = img.src = "Media/Icons/avatar-icon.png";
                });

            }

            if (userData.email) {
                document.querySelectorAll('.profile-email-text').forEach(el => {
                    el.textContent = userData.email;
                });
            }

            if (userData.createdAt) {

                const dateObj = new Date(userData.createdAt);
                
                const formattedDate = dateObj.toLocaleDateString('en-US', { 
                    year: 'numeric', 
                    month: 'short', 
                    day: 'numeric' 
                });
                
                document.querySelectorAll('.profile-created-date').forEach(el => {
                    el.textContent = formattedDate;
                });
                
            }

        }

    } catch (error) {
        console.error(error.message);
    }
    
});

async function copyToClipboard(elementId, isInput = true,clickedButton = null){
    
    const element = document.getElementById(elementId);

    if (!element){
        return;
    }

    const textToCopy = isInput ? element.value : element.innerText;

    try {

        await navigator.clipboard.writeText(textToCopy);
        
        if (clickedButton) {
            const icon = clickedButton.querySelector('img');
            
            if (icon) {
                icon.classList.remove('animate__animated', 'animate__flip');
                void icon.offsetWidth; 
                icon.classList.add('animate__animated', 'animate__flip');
            }
        }
        
    } catch (error) {
        console.error("Error");
    }

}

// Sayfalar açıldığında gösterilen loading ekranını kapatmak için genel fonksiyon.
function hideGlobalLoader(){

    const globalLoader = document.getElementById("global-loader");

    if (globalLoader) {
        globalLoader.classList.add("hidden");
    }
    
}

// Refresh Token için gerekli kısım.

let isRefreshing = false;
let refreshPromise = null;

async function fetchWithAuth(endpoint, options = {}) {

    const url = CONFIG.API_BASE_URL + endpoint;

    options.credentials = "include";
    
    if (!options.headers) {
        options.headers = {};
    }

    let response = await fetch(url, options);

    if (response.status === 401) {

        try {

            if (!isRefreshing) {

                isRefreshing = true;

                refreshPromise = fetch(CONFIG.API_BASE_URL + CONFIG.ENDPOINTS.REFRESH, {
                    method: "POST",
                    credentials: "include"
                }).finally(() => {
                    isRefreshing = false;
                });

            }

            const refreshResponse = await refreshPromise;
               
            if (refreshResponse.ok) {
                response = await fetch(url, options);
            } 
            
            else {
                forceLogout();
            }

        } catch (error) {
            forceLogout();
            throw error;
        }

    }

    return response;

}

function forceLogout() {
    localStorage.removeItem("isLoggedIn");
    sessionStorage.removeItem("user_profile");
    sessionStorage.removeItem("dashboard_stats_cache");   
    window.location.href = "index.html"; 
}