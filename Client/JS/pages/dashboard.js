document.addEventListener("DOMContentLoaded", async () => {

  const totalCountLabel = document.getElementById("total-item-count");
  const favoriteCountLabel = document.getElementById("total-favorite-item-count");
  const recentItemsContainer = document.getElementById("recent-items-container");

  try {
    
    const result = await getDashboardStats();

    if (totalCountLabel) {
      totalCountLabel.textContent = result.data.totalCount;
    }

    if (favoriteCountLabel) {
      favoriteCountLabel.textContent = result.data.favoriteCount;
    }

    if (recentItemsContainer) {
      
      const recentItems = result.data.recentItems;

      if (!recentItems || recentItems.length === 0) {

        recentItemsContainer.innerHTML = `
          <div class="text-center text-muted py-3" style="font-size: 14px;">
            No items added yet.
          </div>`;

        return;

      }

      recentItems.forEach((item, index) => {
        
        const isLastItem = index === recentItems.length - 1;
        const borderClass = isLastItem ? "" : "border-bottom";
        const borderStyle = isLastItem ? "" : "style='border-color: #f1f5f9 !important'";

        const iconSrc = item.domain 
          ? `https://www.google.com/s2/favicons?domain=${item.domain}&sz=64`
          : `Media/Icons/items-default-icon.png`;

        const createdDate = new Date(item.createdAt).toLocaleDateString('en-US', { 
            year: 'numeric', 
            month: 'short', 
            day: 'numeric' 
            
        });

        const itemHtml = `
          <div class="d-flex align-items-center justify-content-between py-3 ${borderClass}" ${borderStyle}>
            
            <div class="d-flex align-items-center" style="width: 40%">
              <div class="d-flex align-items-center justify-content-center flex-shrink-0 bg-white" style="width: 42px; height: 42px; border-radius: 12px; border: 1px solid #e2e8f0; overflow: hidden;">
                <img src="${iconSrc}" alt="Icon" style="width: 20px; height: 20px; object-fit: contain;" />
              </div>
              <span class="ml-3 font-weight-bold text-dark text-truncate" style="font-size: 15px" title="${item.title}">${item.title}</span>
            </div>

            <div class="text-muted text-center" style="width: 30%; font-size: 13px">
              ${createdDate}
            </div>

            <div class="text-right" style="width: 30%">
              <button class="btn btn-sm font-weight-bold px-3 py-2" onclick="window.location.href='vaultitem.html?id=${item.id}'" style="background-color: #f1f5f9; color: #4965fa; border-radius: 10px; font-size: 13px">
                View
              </button>
            </div>

          </div>
        `;

        recentItemsContainer.insertAdjacentHTML('beforeend', itemHtml);

      });

    }

  } catch (error) {

    console.error(error.message);

    if (totalCountLabel){
        totalCountLabel.textContent = "-";
    } 

    if (favoriteCountLabel){
        favoriteCountLabel.textContent = "-";
    } 

  }finally{
    hideGlobalLoader();
  }

});