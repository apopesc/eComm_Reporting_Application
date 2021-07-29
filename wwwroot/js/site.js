

$(document).ready(function () {
    $('#groupDropdown').multiselect();
    $('#groupIDDropdown').multiselect();
    $('#masterGroupDropdown').multiselect();

    $("#btnViewData").click(function () {
        var selectedGroupID = $('#groupIDDropdown').find(":selected").text();
        var selectedGroup = $('#groupDropdown').find(":selected").text();
        var selectedMasterGroup = $('#masterGroupDropdown').find(":selected").text();
        //Some additional logic to get the checkbox values - might need to use an enum: yes selected, no selected, both selected

        //Have to put AJAX Call here to pass to a controller function (remember to put a URL in the AJAX call that points to the controller)
        alert("Data Being Passed to Controller| Group ID-" + selectedGroupID + "| Group-" + selectedGroup + "| MasterGroup-" + selectedMasterGroup);
    })
});