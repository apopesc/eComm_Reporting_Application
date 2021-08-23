$(document).ready(function () {

    $('#addNew_groupDropdown').multiselect({
        enableCaseInsensitiveFiltering: true
        //buttonWidth: 250 For Changing the width of options in the dropdown - may need later
    });
    $('#addNew_groupIDDropdown').multiselect({
        enableCaseInsensitiveFiltering: true
        //buttonWidth: 250 For Changing the width of options in the dropdown - may need later
    });
    $('#addNew_masterGroupDropdown').multiselect({
        enableCaseInsensitiveFiltering: true
        //buttonWidth: 250 For Changing the width of options in the dropdown - may need later
    });

    function ValidateEmail(mail) {
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
            return (true);
        }
        return (false);
    }

    $("#addUserSubmit").click(function () {
        var enteredEmail = $("#addNew_UserEmail").val();
        var isValidEmail = ValidateEmail(enteredEmail);

        var enteredGroupID = $('#addNew_groupIDDropdown').val();
        var enteredGroup = $('#addNew_groupDropdown').val();
        var enteredMasterGroup = $('#addNew_masterGroupDropdown').val();

        var enteredIsActive;

        if ($('#addNew_checkYes').is(':checked')) {
            enteredIsActive = 'Y';
        } else if ($('#addNew_checkNo').is(':checked')) {
            enteredIsActive = 'N';
        } else {
            enteredIsActive = '';
        }

        if (isValidEmail == false) {
            alert("Please enter a valid email address before submitting.");
        } else if (enteredIsActive == '') {
            alert("Please enter a value for Is Active.");
        } else if (enteredGroupID == null || enteredGroup == null || enteredMasterGroup == null) {
            alert("Please enter a value for Group ID, Group, and MasterGroup");
        } else {

            //Posting the collected data to the SubscriptionsGroupsController
            var controllerUrl = '/SubscriptionGroups/AddUserToDB';

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: {
                    userEmail: enteredEmail,
                    isActive: enteredIsActive,
                    selectedGroupID: enteredGroupID,
                    selectedGroup: enteredGroup,
                    selectedMasterGroup: enteredMasterGroup
                }
            });

            function successFunc(returnedData) {
                alert(returnedData);
                //Need to clear dropdowns too
                $('#addNew_checkYes').prop('checked', false);
                $('#addNew_checkNo').prop('checked', false);
                $("#addNew_UserEmail").val('');
            }
            function errorFunc(error) {
                alert("Error Sending Filter Data to the Subscriptions Controller: " + error);
            }
        }
    });
});