const logoutButton = document.getElementById("logout-button");

logoutButton.addEventListener('click',async function(e){

    e.preventDefault();

    try{

        await logout();

        localStorage.removeItem("isLoggedIn");
 
    }catch(error){
        console.error("Error:", error.message);
    }finally{
        sessionStorage.removeItem("user_profile");
        window.location.href = "index.html";
    }

});