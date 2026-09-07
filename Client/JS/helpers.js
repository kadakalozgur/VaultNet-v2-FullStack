function showAlert(alertBox,message,type){

    alertBox.className = `alert alert-${type} animate__animated animate__fadeIn`;
    alertBox.textContent = message;
    alertBox.classList.remove("d-none");

}

function hideAlert(alertBox){

    alertBox.classList.add("d-none");
    
}

// API'den gelen .net'e özel validation errorlarını ui'a basmak için oluşturduğum fonksiyon
function getApiErrorMessage(result){

    if (result.errors && Object.keys(result.errors).length > 0) {
        return Object.values(result.errors)[0][0];
    }
    
    return result.message || "An unknown error occurred.";

}

// Sayfa yüklendiği an arkadan User bilgilerini çektiğim kısım
document.addEventListener("DOMContentLoaded", async function() {

    if (typeof getProfileWithCache !== "function"){
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
        }

    } catch (error) {
        console.error(error.message);
    }
    
});