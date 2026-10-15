const CONFIG = {

    API_BASE_URL: "https://vaultnet-v2-api-a4hyamhdh4cqb3b2.southeastasia-01.azurewebsites.net/api", 

    ENDPOINTS: {
        REGISTER: "/Auth/register",
        LOGIN: "/Auth/login",
        LOGOUT: "/Auth/logout",
        LOGOUT_ALL_DEVICES: "/Auth/logout-all",
        REFRESH: "/Auth/refresh",
        UPDATE_PROFILE_PICTURE: "/UserProfile/update-profile-picture",
        REMOVE_PROFILE_PICTURE: "/UserProfile/remove-profile-picture",
        GET_USER_PROFILE : "/UserProfile/profile",
        UPDATE_EMAIL_ADDRESS: "/UserProfile/update-email-address",
        CHANGE_PASSWORD: "/UserProfile/change-password",
        DELETE_ACCOUNT: "/UserProfile/delete-account",
        ADD_VAULT_ITEM: "/Vault/add-vault-item",
        GET_VAULT_ITEMS: "/Vault/get-vault-items",
        GET_VAULT_ITEM_BY_ID: "/Vault",
        DELETE_VAULT_ITEM: "/Vault",
        UPDATE_VAULT_ITEM: "/Vault",
        TOGGLE_FAVORITE_VAULT_ITEM: "/Vault/toggle-item-favorite",
        GET_DASHBOARD_STATS: "/Vault/dashboard-stats",
        CREATE_CATEGORY: "/Category/create-category",
        GET_CATEGORIES: "/Category/get-category-items",
        GET_ALL_CATEGORIES:"/Category/get-user-all-categories",
        GET_CATEGORY_BY_ID: "/Category",
        UPDATE_CATEGORY: "/Category",
        DELETE_CATEGORY: "/Category",
    }

};