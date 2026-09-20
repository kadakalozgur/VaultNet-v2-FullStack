const passwordInput = document.getElementById("generated-password");
const copyButton = document.getElementById("copy-password-btn");
const generateButton = document.getElementById("generate-btn");
const lengthSelect = document.getElementById("password-length-select");   
const uppercaseCheckbox = document.getElementById("include-uppercase");
const lowercaseCheckbox = document.getElementById("include-lowercase");
const numbersCheckbox = document.getElementById("include-numbers");
const symbolsCheckbox = document.getElementById("include-symbols");

const charSets = {
        uppercase: "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
        lowercase: "abcdefghijklmnopqrstuvwxyz",
        numbers: "0123456789",
        symbols: "!@#$%^&*_-+="
};

generateButton.addEventListener('click', function(){

    let length = parseInt(lengthSelect.value);
    let allowedChars = "";   

    if(uppercaseCheckbox.checked){
        allowedChars += charSets.uppercase;
    }

    if(lowercaseCheckbox.checked){
        allowedChars += charSets.lowercase;
    }

    if(numbersCheckbox.checked){
        allowedChars += charSets.numbers;
    }

    if(symbolsCheckbox.checked){
        allowedChars += charSets.symbols;
    }

    if(allowedChars === ""){
        allowedChars = charSets.lowercase;
        lowercaseCheckbox.checked = true;
    }

    let password = "";

    for(let i=0;i<length;i++){
        const randomIndex = Math.floor(Math.random() * allowedChars.length);
        password += allowedChars[randomIndex];
    }

    passwordInput.value = password;

});

copyButton.addEventListener('click',function(){
    copyToClipboard("generated-password", true, copyButton);
});