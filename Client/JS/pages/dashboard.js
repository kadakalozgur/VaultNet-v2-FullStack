document.addEventListener("DOMContentLoaded", async () => {

  const totalCountLabel = document.getElementById("total-item-count");
  const favoriteCountLabel = document.getElementById("total-favorite-item-count");

  try {
    
    const result = await getDashboardStats();

    if (totalCountLabel) {
      totalCountLabel.textContent = result.data.totalCount;
    }

    if (favoriteCountLabel) {
      favoriteCountLabel.textContent = result.data.favoriteCount;
    }

  } catch (error) {

    console.error(error.message);

    if (totalCountLabel){
        totalCountLabel.textContent = "-";
    } 

    if (favoriteCountLabel){
        favoriteCountLabel.textContent = "-";
    } 

  }

});