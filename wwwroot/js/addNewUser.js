$(document).ready(function () {

    $('#addNew_groupDropdown').multiselect();
    $('#addNew_groupIDDropdown').multiselect();
    $('#addNew_masterGroupDropdown').multiselect();

    function ValidateEmail(mail) {
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
            return (true)
        }
        return (false)
    }
    $("#addUserSubmit").click(function () {
        var enteredEmail = $("#addNew_UserEmail").val();
        var isValidEmail = ValidateEmail(enteredEmail);
        if (isValidEmail == false) {
            alert("Please enter a valid email address before submitting")
        } else {

        }
    });
});