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

        var selectedGroupID = $('#addNew_groupIDDropdown').find(":selected").text();
        var selectedGroup = $('#addNew_groupDropdown').find(":selected").text();
        var selectedMasterGroup = $('#addNew_masterGroupDropdown').find(":selected").text();

        var enteredIsActive;

        if ($('#addNew_checkYes').is(':checked')) {
            enteredIsActive = 'Y';
        } else if ($('#addNew_checkNo').is(':checked')) {
            enteredIsActive = 'N';
        } else {
            enteredIsActive =  '';
        }

        if (isValidEmail == false) {
            alert("Please enter a valid email address before submitting.");
        } else if (enteredIsActive == '') {
            alert("Please enter a value for Is Active.");
        } else if (selectedGroupID == '' || selectedGroup == '' || selectedMasterGroup == '') {
            alert("Please enter a value for Group ID, Group, and MasterGroup");
        } else {

        }
    });
});