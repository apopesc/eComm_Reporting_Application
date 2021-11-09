$(document).ready(function () {

    $('#SubscriptionGroups_Link').addClass('selected-nav-option');

    $('#addNew_groupDropdown').multiselect({
        enableCaseInsensitiveFiltering: true,
        enableHTML: true,
        buttonText: function (options, select) {
            if (options.length > 0) {
                var labels = [];
                options.each(function () {
                    if ($(this).attr('label') !== undefined) {
                        labels.push($(this).attr('value'));
                    }
                    else {
                        labels.push($(this).html());
                    }
                });
                return labels.join(', ') + '';
            } else {
                return 'Select a group...';
            }
        }
    });
    
    $('#addNew_masterGroupDropdown').multiselect({
        nonSelectedText: 'Select a master group...',
        enableCaseInsensitiveFiltering: true
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

        var enteredGroups = $('#addNew_groupDropdown').val();
        var enteredMasterGroups = $('#addNew_masterGroupDropdown').val();
        var enteredGroupIDs = [];

        $('#addNew_groupDropdown').find("option:selected").each(function () {
            var groupID = $(this).prop('title');
            enteredGroupIDs.push(groupID)
        });

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
        } else if (enteredGroupIDs.length == 0 || enteredGroups.length == 0 || enteredMasterGroups.length == 0) {
            alert("Please enter a value for Group ID, Group, and MasterGroup");
        } else {

            //Posting the collected data to the SubscriptionsGroupsController
            var controllerUrl = '/SubscriptionGroups/AddUserSubToDB';

            var groupNames_String = enteredGroups.toString();
            var groupIDs_String = enteredGroupIDs.toString();
            var masterGroups_String = enteredMasterGroups.toString();

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: {
                    userEmail: enteredEmail,
                    isActive: enteredIsActive,
                    selectedGroupIDs: groupIDs_String,
                    selectedGroups: groupNames_String,
                    selectedMasterGroups: masterGroups_String
                }
            });

            function successFunc(returnedData) {
                alert("Success adding user: " + enteredEmail);
                if (returnedData.result == 'Redirect') {
                    window.location = returnedData.url;
                }
            }

            function errorFunc(error) {
                alert("Error Saving New User: " + error);
            }
        }
    });
});

function selectedMasterGroup() {
    if ($('#addNew_masterGroupDropdown :selected').length == 0) { //Nothing is selected in the dropdown (last value is deselected)

        var data = [];
        $("#addNew_groupDropdown").multiselect('dataprovider', data);
        $('#addNew_groupDropdown').multiselect('disable');

    } else { //Something is selected in the dropdown
        var controllerUrl = '/SubscriptionGroups/GetGroupValues';
        var masterGroupList = $('#addNew_masterGroupDropdown').val();

        $.ajax({
            type: "POST",
            url: controllerUrl,
            dataType: "json",
            success: successFunc,
            error: errorFunc,
            data: { 'masterGroupList': masterGroupList }
        });

        function successFunc(dropdownData) {
            //var data = [{label: 'Group ID', value: 'demoOption', disabled: true, children: [{ label: 'Group Name', value: '', disabled: true }]}];
            var data = [];

            for (var groupID in dropdownData) {
                var dropdownEntry = { label: "<b>ID: </b>" + groupID + " </br><b>Name: </b>" + dropdownData[groupID], value: dropdownData[groupID], title: groupID };
                data.push(dropdownEntry);
            }

            $("#addNew_groupDropdown").multiselect('dataprovider', data);
            $('#addNew_groupDropdown').multiselect('enable');
        }

        function errorFunc(error) {
            alert("Error Retrieving Report Names: " + error);
        }
    }
}