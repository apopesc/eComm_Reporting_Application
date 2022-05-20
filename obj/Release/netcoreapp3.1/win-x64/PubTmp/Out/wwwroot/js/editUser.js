$(document).ready(function () {

    $('#SubscriptionGroups_Link').addClass('selected-nav-option');

    $('#edit_groupDropdown').multiselect({
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
    
    $('#edit_masterGroupDropdown').multiselect({
        nonSelectedText: 'Select a master group...',
        enableCaseInsensitiveFiltering: true
    });

    //Getting group IDs and Names from hidden parameters

    var selectedMasterGroupsString = "";
    var selectedGroupIDsString = "";
    var selectedIsActive = "";

    if ($("#selectedMasterGroups").length) {
        selectedMasterGroupsString = $("#selectedMasterGroups").val();
    }
    if ($("#selectedGroupIDs").length) {
        selectedGroupIDsString = $("#selectedGroupIDs").val();
    }
    if ($("#isActive").length) {
        selectedIsActive = $("#isActive").val();
    }

    var selectedMasterGroups = selectedMasterGroupsString.split(",");

    $('#edit_masterGroupDropdown').val(selectedMasterGroups);
    $('#edit_masterGroupDropdown').multiselect('refresh');

    $('#edit_groupDropdown').prop('title', selectedGroupIDsString);
    $('#edit_groupDropdown').multiselect('refresh');

    if (selectedIsActive == "Y") {
        $('#edit_checkYes').prop("checked",true)
    }
    else if (selectedIsActive == "N") {
        $('#edit_checkNo').prop("checked", true)
    }

    function ValidateEmail(mail) {
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
            return (true);
        }
        return (false);
    }

    $("#editUserSubmit").click(function () {
        var userID = $('#userID').val();
        var enteredEmail = $("#userEmail").val();
        var isValidEmail = ValidateEmail(enteredEmail);

        var isTJXEmail = false;

        if (isValidEmail == true) {
            var substringIndex = enteredEmail.indexOf("@") + 1;
            var emailDomain = enteredEmail.substring(substringIndex, enteredEmail.length);

            if (emailDomain == "tjx.com" || emailDomain == "tjxcanada.ca" || emailDomain == "tjxeurope.com") {
                isTJXEmail = true;
            }

        }

        var enteredGroups = $('#edit_groupDropdown').val();
        var enteredMasterGroups = $('#edit_masterGroupDropdown').val();
        var enteredGroupIDs = [];

        if ($('#edit_groupDropdown').prop('disabled')) {
            var groupIDsString = $("#selectedGroupIDs").val();
            enteredGroupIDs = groupIDsString.split(',');
            var groupsString = $("#selectedGroupNames").val();
            enteredGroups = groupsString.split(',');
        } else {
            $('#edit_groupDropdown').find("option:selected").each(function () {
                var groupID = $(this).prop('title');
                enteredGroupIDs.push(groupID)
            });
        }

        var enteredIsActive;

        if ($('#edit_checkYes').is(':checked')) {
            enteredIsActive = 'Y';
        } else if ($('#edit_checkNo').is(':checked')) {
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
        } else if (isTJXEmail == false) {
            alert("Only users with a TJX email address can be saved. (@tjx.com, @tjxcanada.ca, or @tjxeurope.com)")
        } else {

            //Posting the collected data to the SubscriptionsGroupsController
            var controllerUrl = '/SubscriptionGroups/EditUserSubToDB';

            var token = $("#RequestVerificationToken").val();

            var groupNames_String = enteredGroups.toString();
            var groupIDs_String = enteredGroupIDs.toString();
            var masterGroups_String = enteredMasterGroups.toString();

            $.ajax({
                type: "POST",
                url: controllerUrl,
                headers: { 'RequestVerificationToken': token },
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: {
                    ID: userID,
                    userEmail: enteredEmail,
                    isActive: enteredIsActive,
                    selectedGroupIDs: groupIDs_String,
                    selectedGroups: groupNames_String,
                    selectedMasterGroups: masterGroups_String
                }
            });

            function successFunc(returnedData) {
                alert("Success editing user: " + enteredEmail);
                if (returnedData.result == 'Redirect') {
                    window.location = returnedData.url;
                }
            }

            function errorFunc(error) {
                alert("Error Saving Edited User: " + error);
            }
        }
    });
});

function selectedMasterGroup() {
    if ($('#edit_masterGroupDropdown :selected').length == 0) { //Nothing is selected in the dropdown (last value is deselected)

        var data = [];
        $("#edit_groupDropdown").multiselect('dataprovider', data);
        $('#edit_groupDropdown').multiselect('disable');

    } else { //Something is selected in the dropdown
        var controllerUrl = '/SubscriptionGroups/GetGroupValues';
        var masterGroupList = $('#edit_masterGroupDropdown').val();

        var token = $("#RequestVerificationToken").val();

        $.ajax({
            type: "POST",
            url: controllerUrl,
            headers: { 'RequestVerificationToken': token },
            dataType: "json",
            success: successFunc,
            error: errorFunc,
            data: { 'masterGroupList': masterGroupList }
        });

        function successFunc(dropdownData) {
            //var data = [{label: 'Group ID', value: 'demoOption', disabled: true, children: [{ label: 'Group Name', value: '', disabled: true }]}];
            if (dropdownData.success == true) {
                var data = [];

                for (var groupID in dropdownData.groups) {
                    var dropdownEntry = { label: "<b>ID: </b>" + groupID + " </br><b>Name: </b>" + dropdownData.groups[groupID], value: dropdownData.groups[groupID], title: groupID };
                    data.push(dropdownEntry);
                }

                $("#edit_groupDropdown").multiselect('dataprovider', data);
                $('#edit_groupDropdown').multiselect('enable');
            } else {
                alert("Error Retrieving Groups" + dropdownData.message);
            }

        }

        function errorFunc(error) {
            alert("Error Retrieving Report Names: " + error);
        }
    }
}