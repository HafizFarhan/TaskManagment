var PhoneNumberInput = document.querySelector("#PhoneNumber");
var iti = window.intlTelInput(PhoneNumberInput, {

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
$(document).on('click', '#registerSubmit', function () {
    debugger;

    var isValidNumber = iti.isValidNumber();

    if (!isValidNumber) {
        if ($("#PhoneNumber").val() != "") {
            $("#ValPhoneNumber").text('Invalid phone number.');
        }


        // Display an error message or handle invalid phone numbers
    }
    else {
        $("#ValPhoneNumber").html('');
    }
});

$(PhoneNumberInput).on("keydown", function (event) {
    var key = event.which || event.keyCode;
    var keyChar = String.fromCharCode(key);

    // Allow numeric digits and specific non-alphabetic characters
    var allowedChars = /^[0-9+\-() ]$/;

    // Check if the pressed key is an alphabetic character
    if (!allowedChars.test(keyChar) && key >= 65 && key <= 90) {
        event.preventDefault(); // Prevent key press for alphabetic characters
    }
});