function showAlert(alertBox,message,type){

    alertBox.className = `alert alert-${type} animate__animated animate__fadeIn`;
    alertBox.textContent = message;
    alertBox.classList.remove("d-none");

}

function hideAlert(alertBox){

    alertBox.classList.add("d-none");
    
}