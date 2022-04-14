var sierraTable;
var expandableRowEntries = [];
var expandableRowIDs = new Set();

$(document).ready(function () {
    $('#SierraReports_Link').addClass('selected-nav-option');

    $('#sierraFolderDropdown').multiselect({
        nonSelectedText: 'Select a folder...',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true
    });
    $('#sierraReportDropdown').multiselect({
        nonSelectedText: 'Select a report name...',
        enableCaseInsensitiveFiltering: true
    });

    $('#sierraReportDropdown').multiselect('disable');

    $("#loadMe").modal({
        backdrop: "static", //remove ability to close modal with click
        keyboard: false, //remove option to close with keyboard
        show: true //Display loader!
    });

    //Getting the previously loaded table if there is one
    var controllerUrl = '/SierraReports/GetInitialTable';

    $.ajax({
        type: "POST",
        url: controllerUrl,
        dataType: "json",
        success: successFunction,
        error: errorFunction
    });

    function successFunction(tableData) {
        if (typeof tableData === 'string') { //If there is an error pulling it from the database
            alert(tableData);
            setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
        } else {
            if (tableData.tableParams != null) {

                var loadedTableData = {
                    tableParams: tableData.tableParams,
                    rowData: tableData.rowData
                }

                $('#sierraFolderDropdown').val(tableData.report.reportFolder);
                $('#sierraFolderDropdown').multiselect('refresh');

                selectedFolder(tableData.report.reportName);

                createTable(loadedTableData);
                setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
            } else {
                setTimeout(function () { $("#loadMe").modal("hide"); }, 1000);
            }
        }
    }

    function errorFunction(error) {
        alert("Error Loading Previously Loaded User Table: " + error);
        setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
    }


    $('.filter-data').on('change', '#sierraReportDropdown', function () {
        if ($('#sierraReportDropdown :selected').length > 0) { //Nothing is selected in the dropdown (last value is deselected)

            $("#loadMe").modal({
                backdrop: "static", //remove ability to close modal with click
                keyboard: false, //remove option to close with keyboard
                show: true //Display loader!
            });

            var selectedReport = $('#sierraReportDropdown').val();
            var selectedReportFolder = $('#sierraReportDropdown option:selected').prop('title');

            var reportData = {
                reportName: selectedReport,
                reportFolder: selectedReportFolder
            };

            var controllerUrl = '/SierraReports/GetSierraTableData';

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: {
                    'reportData': reportData
                }
            });

            function successFunc(tableData) {
                if (typeof tableData === 'string') { //If there is an error saving it to the database
                    alert(tableData);
                    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
                } else {
                    createTable(tableData); //Creating the dynamic table and displaying it on the screen
                    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
                }
            }

            function errorFunc(error) {
                alert("Error Getting Report Subscription Data: " + error);
                setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
            }
        }
    });

    $('#sierraSubscriptionData').on('click', '#expandBtn', function () {
        var $selectedRow = $(this).closest("tr");
        var dataTable_row = sierraTable.row($selectedRow);

        var selectedParameter = $(this).attr('class');

        if ($(this).hasClass("shown_child")) {

            $('table > tbody  > tr > td > #expandBtn').each(function () {
                $(this).removeClass("shown_child");
            });

            dataTable_row.child.hide();

        } else {
            for (const rowID of expandableRowIDs) {

                let childRows = "";

                for (let g = 0; g < expandableRowEntries.length; g++) {
                    if (expandableRowEntries[g].rowID == rowID) {
                        var paramName = expandableRowEntries[g].parameter_name;

                        if (paramName == selectedParameter) {
                            var childEntry = '<tr id="' + paramName + '"><td class="expanded_row"><b>' + paramName + ':</b>' + expandableRowEntries[g].data + '</td></tr>';
                            childRows = childRows + childEntry;
                        }
                    }
                }

                sierraTable.row($selectedRow).child(childRows, 'child-row').show();

            }
            $(this).addClass("shown_child");
        }
    });

    $('#addNewDiv').click(function () {
        var selectedReportName = $('#sierraReportDropdown').val();
        window.location = "/SierraReports/AddNewReportSub?selectedReportName=" + selectedReportName;
    });

});

function selectedFolder(selectedVal = "") {
    if ($('#sierraFolderDropdown :selected').length == 0) { //Nothing is selected in the dropdown (last value is deselected)

        var data = [{ label: 'Select a report name...', value: '', disabled: true, selected: true }];
        $("#sierraReportDropdown").multiselect('dataprovider', data);
        $('#sierraReportDropdown').multiselect('disable');

    } else { //Something is selected in the dropdown
        var controllerUrl = '/SierraReports/GetReportNameValues';

        var folderPathList = $('#sierraFolderDropdown').val();

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

            $("#sierraReportDropdown").multiselect('dataprovider', data);
            $('#sierraReportDropdown').multiselect('enable');
            if (selectedVal != "") {
                $('#sierraReportDropdown').val(selectedVal);
                $('#sierraReportDropdown').multiselect('refresh');
            }
        }

        function errorFunc(error) {
            alert("Error Retrieving Report Names: " + error);
        }
    }
}

function createTable(tableData) {
    //Clearing table initially
    $('#sierraSubscriptionData').empty();
    $('#sierraSubscriptionData').addClass("horizontal-scroll");

    var subTable = $('<table>').addClass('sierraSubscriptionsTable');
    subTable.prop('id', 'sierraSubscriptionsTable');

    let header = $('<thead>');
    //Adding the headers to the table
    let Hrow = $('<tr>').addClass('sierraSubscriptionsRow_Header');

    let tableHeader_Icons = $('<th>').addClass('sierraSubscriptionsHeader').text(''); //Invisible header for icons
    Hrow.append(tableHeader_Icons);

    for (let i = 0; i < tableData.tableParams.length; i++) {
        let tableHeader = $('<th>').addClass('sierraSubscriptionsHeader').text(tableData.tableParams[i].name);
        Hrow.append(tableHeader);
    }
    header.append(Hrow);
    subTable.append(header); //Adding the row to the table



    let body = $('<tbody>');
    //Adding the data under the headers
    for (let j = 0; j < tableData.rowData.length; j++) {

        if (j == tableData.rowData.length - 1) { //This is so that the bottom border isn't added to the last row (it pops out of the table otherwise)
            var row = $('<tr>').addClass('sierraSubscriptionsRow_Last');
        } else {
            var row = $('<tr>').addClass('sierraSubscriptionsRow');
        }
        row.prop('id', tableData.rowData[j].subscriptionID);

        let tableEntry_Icons = $('<td>').addClass('sierraSubscriptionsEntry_Icon');
        let deleteIcon = $('<button>').addClass('deleteBtn');
        let deleteLink = $('<i>').addClass('fa fa-trash');
        deleteIcon.append(deleteLink);
        tableEntry_Icons.append(deleteIcon);

        let editIcon = $('<button>').addClass('editBtn');
        let editLink = $('<i>').addClass('fas fa-pencil-alt');
        editIcon.append(editLink);
        tableEntry_Icons.append(editIcon);

        let copyIcon = $('<button>').addClass('copyBtn');
        let copyLink = $('<i>').addClass('fas fa-copy');
        copyIcon.append(copyLink);
        tableEntry_Icons.append(copyIcon);

        row.append(tableEntry_Icons);

        let tableEntry1 = $('<td>').addClass('sierraSubscriptionsEntry_SubscriptionID subscriptionsTableEntry').text(tableData.rowData[j].subscriptionID);
        row.append(tableEntry1); //adding element to the row
        let tableEntry2 = $('<td>').addClass('sierraSubscriptionsEntry_SubscriptionName').text(tableData.rowData[j].subscriptionName);
        row.append(tableEntry2);
        let tableEntry3 = $('<td>').addClass('sierraSubscriptionsEntry_ReportName').text(tableData.rowData[j].reportName);
        row.append(tableEntry3);
        let tableEntry4 = $('<td>').addClass('sierraSubscriptionsEntry_GroupName').text(tableData.rowData[j].groupNames);
        row.append(tableEntry4);
        let tableEntry5 = $('<td>').addClass('sierraSubscriptionsEntry_GroupID').text(tableData.rowData[j].groupIDs);
        row.append(tableEntry5);
        let tableEntry6 = $('<td>').addClass('sierraSubscriptionsEntry_FileFormat').text(tableData.rowData[j].fileFormat);
        row.append(tableEntry6);
        let tableEntry7 = $('<td>').addClass('sierraSubscriptionsEntry_Schedule').text(tableData.rowData[j].schedule);
        row.append(tableEntry7);

        for (i = 5; i < tableData.tableParams.length; i++) {
            for (var paramName in tableData.rowData[j].dynamicParams) {

                if (paramName == tableData.tableParams[i].name) {
                    let parameterData = tableData.rowData[j].dynamicParams[paramName];

                    let tableEntry = $('<td>').addClass('sierraSubscriptionEntry_Dynamic')

                    if (parameterData != null) {
                        let commaCount = parameterData.split(",").length - 1;

                        if (commaCount > 1) {
                            let substringIndex = getPosition(parameterData, ',', 2);

                            let expandableRowEntry = { rowID: tableData.rowData[j].subscriptionID, data: parameterData, parameter_name: paramName };
                            expandableRowEntries.push(expandableRowEntry);
                            expandableRowIDs.add(tableData.rowData[j].subscriptionID);

                            parameterData = parameterData.substring(0, substringIndex + 1);
                            tableEntry.text(parameterData);

                            var button = $('<a id = "expandBtn" class = "' + paramName + '">(...)</a>');
                            button.appendTo(tableEntry);
                        } else {
                            tableEntry.text(parameterData);
                        }
                    } else {
                        tableEntry.text(parameterData);
                    }

                    row.append(tableEntry);
                }

            }
        }

        body.append(row);

    }
    subTable.append(body);
    $('#sierraSubscriptionData').append(subTable);

    sierraTable = $('#sierraSubscriptionsTable').DataTable({
        "lengthMenu": [5, 8, 15, 25]
    });
}

function getPosition(string, subString, index) {
    return string.split(subString, index).join(subString).length;
}