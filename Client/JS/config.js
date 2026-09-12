const CONFIG = {

    API_BASE_URL: "http://localhost:5203/api", 

    ENDPOINTS: {
        REGISTER: "/Auth/register",
        LOGIN: "/Auth/login",
        LOGOUT: "/Auth/logout",
        UPDATE_PROFILE_PICTURE: "/UserProfile/update-profile-picture",
        GET_USER_PROFILE : "/UserProfile/profile",
        UPDATE_EMAIL_ADDRESS: "/UserProfile/update-email-address",
        CHANGE_PASSWORD: "/UserProfile/change-password",
        DELETE_ACCOUNT: "/UserProfile/delete-account",
        ADD_VAULT_ITEM: "/Vault/add-vault-item",
        GET_VAULT_ITEMS: "/Vault/get-vault-items",
        GET_VAULT_ITEM_BY_ID: "/Vault"
    }

};