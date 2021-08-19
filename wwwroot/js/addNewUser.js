$(document).ready(function () {

    $('#addNew_groupDropdown').multiselect();
    $('#addNew_groupIDDropdown').multiselect();
    $('#addNew_masterGroupDropdown').multiselect();

    function ValidateEmail(mail) {
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
            return (true);
        }
        return (false);
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
            enteredIsActive = '';
        }

        if (isValidEmail == false) {
            alert("Please enter a valid email address before submitting.");
        } else if (enteredIsActive == '') {
            alert("Please enter a value for Is Active.");
        } else if (selectedGroupID == '' || selectedGroup == '' || selectedMasterGroup == '') {
            alert("Please enter a value for Group ID, Group, and MasterGroup");
        } else {

            //Turning the dropdown selected values into lists, removing whitespace
            var selectedGroupID_List = selectedGroupID.split('  ');
            for (let i = 0; i < selectedGroupID_List.length; i++) {
                selectedGroupID_List[i] = selectedGroupID_List[i].trim();
            }

            var selectedGroup_List = selectedGroup.split('  ');
            for (let i = 0; i < selectedGroup_List.length; i++) {
                selectedGroup_List[i] = selectedGroup_List[i].trim();
            }

            var selectedMasterGroup_List = selectedMasterGroup.split('  ');
            for (let i = 0; i < selectedMasterGroup_List.length; i++) {
                selectedMasterGroup_List[i] = selectedMasterGroup_List[i].trim();
            }

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
                    selectedGroupIDs: selectedGroupID_List,
                    selectedGroups: selectedGroup_List,
                    selectedMasterGroups: selectedMasterGroup_List
                }
            });

            function successFunc(returnedData) {
                alert(returnedData);
            }
            function errorFunc(error) {
                alert("Error Sending Filter Data to the Subscriptions Controller: " + error);
            }
        }
    });
});