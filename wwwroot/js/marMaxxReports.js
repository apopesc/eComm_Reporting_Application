$(document).ready(function () {
    $('#MarMaxxReports_Link').addClass('selected-nav-option');

    $('#marMaxxFolderDropdown').multiselect({
        nonSelectedText: 'Select a folder...',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true
    });
    $('#marMaxxReportDropdown').multiselect({
        nonSelectedText: 'Select a report name...',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true
    });

    $('#marMaxxReportDropdown').multiselect('disable');

    $('#btnViewMarmaxxData').click(function(){
        var selectedFolderPaths = $('#marMaxxFolderDropdown').val();
        var selectedReports = $('#marMaxxReportDropdown').val();

        if (selectedFolderPaths.length == 0 || selectedReports.length == 0) {
            alert("Please select value(s) for Folder and Report Name");
        } else {

            //Generate table
            var selectedFolders = [];

            var selectedFolderNames_Text = $('#marMaxxFolderDropdown').find(":selected").text();
            var selectedFolderNames = selectedFolderNames_Text.split('  ');

            for (let i = 0; i < selectedFolderNames.length; i++) {
                selectedFolderNames[i] = selectedFolderNames[i].trim();

                //Using the name and the path to create a selected folder object and adding it to the list
                var selectedFolder = {
                    folderName: selectedFolderNames[i],
                    folderPath: selectedFolderPaths[i]
                };
                selectedFolders.push(selectedFolder);
            }

            var controllerUrl = '/MarMaxxReports/GetMarMaxxTableData';

            var filterData = {
                folders: selectedFolders,
                reportNames: selectedReports
            };

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: { 'filterData': filterData }
            });

            function successFunc(tableData) {
                if (typeof tableData === 'string') { //If there is an error saving it to the database
                    alert(tableData);
                } else {
                    //createTable(tableData);
                    alert("Success Grabbing Parameters");
                }
            }

            function errorFunc(error) {
                alert("Error Sending Filter Data to the Subscriptions Controller: " + error);
            }

        }
    });
});

function selectedFolder() {
    if ($('#marMaxxFolderDropdown :selected').length == 0) { //Nothing is selected in the dropdown (last value is deselected)
        $('#marMaxxReportDropdown').multiselect("deselectAll", false).multiselect("refresh");
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
            var data = [];
            for (i = 0; i < dropdownData.length; i++) {
                data.push({ label: dropdownData[i], value: dropdownData[i] });
            }

            $("#marMaxxReportDropdown").multiselect('dataprovider', data);
            $('#marMaxxReportDropdown').multiselect('enable');
        }

        function errorFunc(error) {
            alert("Error Retrieving Report Names: " + error);
        }
    }
}