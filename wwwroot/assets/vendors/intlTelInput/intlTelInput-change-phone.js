var CurrentPhoneNumber = document.querySelector("#CurrentPhoneNumber");
var NewPhoneNumber = document.querySelector("#NewPhoneNumber");

var itiCurrent = window.intlTelInput(CurrentPhoneNumber, {

    dropdownContainer: document.body,
    formatOnDisplay: false,
    geoIpLookup: function (callback) {
        fetch("https://ipapi.co/json")
            .then(function (res) { return res.json(); })
            .then(function (data) { callback(data.country_code); })
            .catch(function () { callback("us"); });
    },
    hiddenInput: "full_number",
    initialCountry: "ca",
    placeholderNumberType: "MOBILE",

    showFlags: true,
    utilsScript: "~/assets/js/utils.js"
}); 



var itiNew = window.intlTelInput(NewPhoneNumber, {

    dropdownContainer: document.body,
    formatOnDisplay: false,
    geoIpLookup: function (callback) {
        fetch("https://ipapi.co/json")
            .then(function (res) { return res.json(); })
            .then(function (data) { callback(data.country_code); })
            .catch(function () { callback("us"); });
    },
    hiddenInput: "full_number",
    initialCountry: "ca",
    placeholderNumberType: "MOBILE",

    showFlags: true,
    utilsScript: "~/assets/js/utils.js"
}); 



// Validate and format phone number on submit
$(document).on('submit', '#ChangePhoneForm', function () {
    debugger;
    var valid = true;
    var isValidNumber = itiNew.isValidNumber();

    if (!isValidNumber) {
        if ($("#NewPhoneNumber").val() != "") {
            $("#ValPhoneNumber").text('Invalid phone number.');
            valid = false;
        }
        else if ($("#NewPhoneNumber").val() == "") {
            $("#ValPhoneNumber").text('Please provide a new phone number!.');
            valid = false;
        }
 
    }
    else {
        $("#ValPhoneNumber").html('');
    }
    return valid;
});

$(NewPhoneNumber).on("keydown", function (event) {
    var key = event.which || event.keyCode;
    var keyChar = String.fromCharCode(key);

    // Allow numeric digits and specific non-alphabetic characters
    var allowedChars = /^[0-9+\-() ]$/;

    // Check if the pressed key is an alphabetic character
    if (!allowedChars.test(keyChar) && key >= 65 && key <= 90) {
        event.preventDefault(); // Prevent key press for alphabetic characters
    }
});