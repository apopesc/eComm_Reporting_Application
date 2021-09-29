$(document).ready(function () {
    $('#MarMaxxReports_Link').addClass('selected-nav-option');


    $('#marMaxxGroupID').multiselect({
        nonSelectedText: 'Select a Group ID...',
        enableCaseInsensitiveFiltering: true
    });
    $('#marMaxxGroupName').multiselect({
        nonSelectedText: 'Select a Group Name...',
        enableCaseInsensitiveFiltering: true
    });
    $('#marMaxxFolderDropdown').multiselect({
        nonSelectedText: 'Select a folder...',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true
    });
    $('#marMaxxReportDropdown').multiselect({
        nonSelectedText: 'Select a report name...',
        enableCaseInsensitiveFiltering: true
    });

    $('#marMaxxReportDropdown').multiselect('disable');

    $('#viewReportParams').click(function () {
        var selectedReport = $('#marMaxxReportDropdown').val();

        if (selectedReport == null) {
            alert("Please select a report to view it's parameters.");
        } else {

            var controllerUrl = '/MarMaxxReports/GetMarMaxxReportParameters';

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: { 'reportName': selectedReport }
            });

            function successFunc(filterData) {
                if (typeof filterData === 'string') { //If there is an error saving it to the database
                    alert(filterData);
                } else {
                    //This is where filters would populate on the screen with report viewer
                    alert(filterData);
                }
            }

            function errorFunc(error) {
                alert("Error Getting Report Parameters: " + error);
            }
        }
    });

});

function selectedFolder() {
    if ($('#marMaxxFolderDropdown :selected').length == 0) { //Nothing is selected in the dropdown (last value is deselected)
        var data = [{ label: 'Select a report name...', value: '', disabled: true, selected: true }];
        $("#marMaxxReportDropdown").multiselect('dataprovider', data);
        $('#marMaxxReportDropdown').multiselect('disable');

    } else { //Something is selected in the dropdown
        var controllerUrl = '/MarMaxxReports/GetReportNameValues';

        var folderPathList = $('#marMaxxFolderDropdown').val();

        $.ajax({
            type: "POST",
            url: controllerUrl,
            dataType: "json",
            success: successFunc,
            error: errorFunc,
            data: { 'folderPathList': folderPathList }
        });

        function successFunc(dropdownData) {
            //manage the list of dropdown values in the front end -> just use controller to get the dropdown values for each selected folder
            var data = [{ label: 'Select a report name...', value: '', disabled: true, selected: true }];
            for (i = 0; i < dropdownData.length; i++) {
                data.push({ label: dropdownData[i].reportName, value: dropdownData[i].reportName, title: dropdownData[i].reportFolder });
            }

            $("#marMaxxReportDropdown").multiselect('dataprovider', data);
            $('#marMaxxReportDropdown').multiselect('enable');
        }

        function errorFunc(error) {
            alert("Error Retrieving Report Names: " + error);
        }
    }
}