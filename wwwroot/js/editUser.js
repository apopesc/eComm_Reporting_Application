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
    var selectedIsActive = "";

    if ($("#selectedMasterGroups").length) {
        selectedMasterGroupsString = $("#selectedMasterGroups").val();
    }
    if ($("#isActive").length) {
        selectedIsActive = $("#isActive").val();
    }

    var selectedMasterGroups = selectedMasterGroupsString.split(",");

    $('#edit_masterGroupDropdown').val(selectedMasterGroups);
    $('#edit_masterGroupDropdown').multiselect('refresh');

    if (selectedIsActive == "Y") {
        $('#addNew_checkYes').prop("checked",true)
    }
    else if (selectedIsActive == "N") {
        $('#addNew_checkNo').prop("checked", true)
    }

    function ValidateEmail(mail) {
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
            return (true);
        }
        return (false);
    }

    $("#editUserSubmit").click(function () {
        
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

            $("#edit_groupDropdown").multiselect('dataprovider', data);
            $('#edit_groupDropdown').multiselect('enable');
        }

        function errorFunc(error) {
            alert("Error Retrieving Report Names: " + error);
        }
    }
}