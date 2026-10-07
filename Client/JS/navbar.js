const logoutButton = document.getElementById("logout-button");

logoutButton.addEventListener('click',async function(e){

    e.preventDefault();

    try{

        await logout();
 
    }catch(error){
        console.error("Error:", error.message);
    }finally{
        forceLogout();
    }

});