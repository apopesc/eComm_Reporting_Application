$(document).ready(function () {
    $('#MarMaxxReports_Link').addClass('selected-nav-option');

    $('#marMaxxGroupID').multiselect({
        nonSelectedText: 'Select a group ID...',
        enableCaseInsensitiveFiltering: true
    });
    $('#marMaxxGroupName').multiselect({
        nonSelectedText: 'Select a group name...',
        enableCaseInsensitiveFiltering: true
    });
    $('#marMaxxReportName').multiselect({
        nonSelectedText: 'Select a report name...',
        enableCaseInsensitiveFiltering: true
    });

    //Getting group IDs and Names from hidden parameters
    var selectedGroupNamesString = "";
    var selectedGroupIDsString = "";
    $('.hidden-selected-groups input').each(function () {
        if ($(this).attr("id") == "selectedGroupNames") {
            selectedGroupNamesString = $(this).val();
        }
        else if ($(this).attr("id") == "selectedGroupIDs") {
            selectedGroupIDsString = $(this).val();
        }
    });

    //setting selected values of group IDs and group names in the dropdowns
    var selectedGroupNames = selectedGroupNamesString.split(",");
    var selectedGroupIDs = selectedGroupIDsString.split(",");

    for (let i = 0; i < selectedGroupNames.length; i++) {
        jquerySelector = 'option[value="' + selectedGroupNames[i] + '"]';
        $(jquerySelector, $('#marMaxxGroupName')).prop('selected', true);
    }
    $('#marMaxxGroupName').multiselect('refresh');

    for (let i = 0; i < selectedGroupIDs.length; i++) {
        jquerySelector = 'option[value="' + selectedGroupIDs[i] + '"]';
        $(jquerySelector, $('#marMaxxGroupID')).prop('selected', true);
    }
    $('#marMaxxGroupID').multiselect('refresh');

    //Getting the selected dynamic parameter values from hidden
    var selectedDynamicParamVals = {};
    $('.hidden-dynamic-params input').each(function () {
        if ($(this).attr("id") == "selectedGroupNames") {
            selectedGroupNamesString = $(this).val();
        }
        var paramName = $(this).attr("name");
        var paramValue = $(this).val();
        selectedDynamicParamVals[paramName] = paramValue;
    });

    //Getting report folder name first before getting all dynamic parameter dropdown data
    var controllerUrl = '/MarMaxxReports/getReportFolder';
    var selectedReportName = $('#marMaxxReportName option[disabled]:selected').val();

    $.ajax({
        type: "POST",
        url: controllerUrl,
        dataType: "json",
        success: successFunc,
        error: errorFunc,
        data: { 'reportName': selectedReportName}
    });

    function successFunc(folderName) {
        //manage the list of dropdown values in the front end -> just use controller to get the dropdown values for each selected folder
        if (folderName == "Report folder not found") {
            alert("Report folder not found. Please delete this Subscription record and re-create it to use updated data.");
        } else {
            getDynamicReportParams(selectedReportName, folderName);
        }
    }

    function errorFunc(error) {
        alert("Error Retrieving Report Folder: " + error);
    }

    //Getting the report parameters now
    
});

function getDynamicReportParams(selectedReportName, selectedFolderName) {
    var controllerUrl = '/MarMaxxReports/GetMarMaxxReportParameters';

    var reportData = {
        reportName: selectedReportName,
        reportFolder: selectedFolderName
    }

    $.ajax({
        type: "POST",
        url: controllerUrl,
        dataType: "json",
        success: successFunc,
        error: errorFunc,
        data: { 'reportData': reportData }
    });

    function successFunc(paramData) {
        if (typeof paramData === 'string') { //If there is an error saving it to the database
            alert(paramData);
        } else {
            //need to create dynamic parameters now
        }
    }

    function errorFunc(error) {
        alert("Error Getting Report Parameters: " + error);
    }
}