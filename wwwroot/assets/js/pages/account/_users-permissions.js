$(document).ready(function () {
    $('.user-dropdown').select2();
});

function showInviteUserModal(button) {
    var confirmationModal = new bootstrap.Modal(document.getElementById("InviteUserModal"));
    confirmationModal.show();
}

function setModalText(buttonId) {
    let title, message, loadingTitle, handler;
    if (buttonId === 'bulkdeleteBtn') {
        title = 'Delete Confirmation';
        message = 'Are you sure you want to delete selected user?';
        loadingTitle = 'Deleting';
        handler = "/manage/account/users-permissions/?handler=startbulkdelete";
    }// Add more actions as needed

    return { title, message, loadingTitle, handler };
}

function inviteUser(event) {
    isFormSubmitted = true;
    var isFormValid = validateForm()
    if (!isFormValid) {
        event.preventDefault();
    }
    else {
        $("#registerForm").submit();
    }
}

function validateForm() {
    if (!isFormSubmitted)
        return;
    var isFormValid = true;
    var fullName = $("#Input\\.FullName").val();
    if (!fullName || fullName.trim() === "") {
        $("#ValFullName").text("Full Name is required");
        isFormValid = false;
    } else {
        $("#ValFullName").text("");
    }
    var email = $("#Input\\.Email").val();
    if (!email || !isValidEmail(email)) {
        $("#ValEmail").text("Please enter a valid email address");
        isFormValid = false;
    } else {
        $("#ValEmail").text("");
    }
    var phoneNumber = $("#PhoneNumber").val();
    if (!phoneNumber || !isValidPhoneNumber(phoneNumber)) {
        $("#ValPhoneNumber").text("Please enter a valid phone number");
        isFormValid = false;
    } else {
        $("#ValPhoneNumber").text("");
    }
    var selectedRole = $("#role").val();
    if (!selectedRole || selectedRole === "") {
        $("#ValRole").text("Please select a role");
        isFormValid = false;
    } else {
        $("#ValRole").text("");
    }
    return isFormValid;
}
