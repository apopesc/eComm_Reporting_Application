$(document).ready(function () {

    $('#SubscriptionGroups_Link').addClass('selected-nav-option');

    $('#addNew_groupDropdown').multiselect({
        enableCaseInsensitiveFiltering: true,
        enableClickableOptGroups: true,
        enableCollapsibleOptGroups: true,
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
            var controllerUrl = '/SubscriptionGroups/AddUserSubToDB';

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
                alert("Success adding user: " + enteredEmail);
                if (returnedData.result == 'Redirect') {
                    window.location = returnedData.url;
                }
            }

            function errorFunc(error) {
                alert("Error Sending Filter Data to the Subscriptions Controller: " + error);
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
                var dropdownEntry = { label: '<b>ID: </b>' + groupID, children: [{ label: "<b>Name: </b>" + dropdownData[groupID], value: dropdownData[groupID], title: groupID }] };
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