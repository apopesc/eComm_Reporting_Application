$(document).ready(function () {
    $('#MarMaxxReports_Link').addClass('selected-nav-option');

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

    $('#btnViewMarmaxxData').click(function(){
        var selectedReport = $('#marMaxxReportDropdown').val();
        var selectedReportFolder = $('#marMaxxReportDropdown option:selected').prop('title');

        if (selectedReport == null) {
            alert("Please select a report to view subscription data");
        } else {

            var controllerUrl = '/MarMaxxReports/GetMarMaxxTableData';

            var reportData = {
                reportName: selectedReport,
                reportFolder: selectedReportFolder
            };

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: { 'reportData': reportData }
            });

            function successFunc(tableData) {
                if (typeof tableData === 'string') { //If there is an error saving it to the database
                    alert(tableData);
                } else {
                    createTable(tableData); //Creating the dynamic table and displaying it on the screen
                }
            }

            function errorFunc(error) {
                alert("Error Getting Report Subscription Data: " + error);
            }
        }
    });

    $('#marMaxxSubscriptionData').on('click', '.deleteBtn', function () {
        var $selectedRow = $(this).closest("tr");
        var _ID = $selectedRow.attr('id');

        let selectedSubscription = $selectedRow
            .find(".marMaxxSubscriptionsEntry_SubscriptionName")
            .text();

        if (confirm('Are you sure you want to delete subscription: ' + selectedSubscription + '?')) {
            var controllerUrl = '/MarMaxxReports/DeleteMarmaxxReportSubscription';

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: { 'ID': _ID }
            });

            function successFunc(response) {
                $selectedRow.remove();
                alert(response + selectedSubscription);
            }

            function errorFunc(error) {
                alert(error);
            }
        } else { //User clicks no

        }
    });

    $('#marMaxxSubscriptionData').on('click', '.editBtn', function () {
        var $selectedRow = $(this).closest("tr");
        var _ID = $selectedRow.attr('id');

        window.location = "/MarMaxxReports/EditReportSub?ID="+_ID;
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
            var data = [{label: 'Select a report name...', value:'', disabled:true, selected:true}];
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

function createTable(tableData) {
    //Clearing table initially
    $('#marMaxxSubscriptionData').empty();
    $('#marMaxxSubscriptionData').addClass("horizontal-scroll");

    var subTable = $('<table>').addClass('marMaxxSubscriptionsTable');
    subTable.prop('id', 'marMaxxSubscriptionsTable');

    let header = $('<thead>');
    //Adding the headers to the table
    let Hrow = $('<tr>').addClass('marMaxxSubscriptionsRow_Header');

    let tableHeader_Icons = $('<th>').addClass('marMaxxSubscriptionsHeader').text(''); //Invisible header for icons
    Hrow.append(tableHeader_Icons);

    for (let i = 0; i < tableData.tableParams.length; i++) {
        let tableHeader = $('<th>').addClass('marMaxxSubscriptionsHeader').text(tableData.tableParams[i].name);
        Hrow.append(tableHeader);
    }
    header.append(Hrow);
    subTable.append(header); //Adding the row to the table

    let body = $('<tbody>');
    //Adding the data under the headers
    for (let j = 0; j < tableData.rowData.length; j++) {

        if (j == tableData.rowData.length - 1) { //This is so that the bottom border isn't added to the last row (it pops out of the table otherwise)
            var row = $('<tr>').addClass('marMaxxSubscriptionsRow_Last');
        } else {
            var row = $('<tr>').addClass('marMaxxSubscriptionsRow');
        }
        row.prop('id', tableData.rowData[j].subscriptionID);

        let tableEntry_Icons = $('<td>').addClass('marMaxxSubscriptionsEntry_Icon');
        let deleteIcon = $('<button>').addClass('deleteBtn');
        let deleteLink = $('<i>').addClass('fa fa-trash');
        deleteIcon.append(deleteLink);
        tableEntry_Icons.append(deleteIcon);

        let editIcon = $('<button>').addClass('editBtn');
        let editLink = $('<i>').addClass('fas fa-pencil-alt');
        editIcon.append(editLink);
        tableEntry_Icons.append(editIcon);

        row.append(tableEntry_Icons); 

        let tableEntry1 = $('<td>').addClass('marMaxxSubscriptionsEntry_SubscriptionID').text(tableData.rowData[j].subscriptionID);
        row.append(tableEntry1); //adding element to the row
        let tableEntry2 = $('<td>').addClass('marMaxxSubscriptionsEntry_SubscriptionName').text(tableData.rowData[j].subscriptionName);
        row.append(tableEntry2);
        let tableEntry3 = $('<td>').addClass('marMaxxSubscriptionsEntry_ReportName').text(tableData.rowData[j].reportName);
        row.append(tableEntry3);
        let tableEntry4 = $('<td>').addClass('marMaxxSubscriptionsEntry_GroupName').text(tableData.rowData[j].groupNames);
        row.append(tableEntry4);
        let tableEntry5 = $('<td>').addClass('marMaxxSubscriptionsEntry_GroupID').text(tableData.rowData[j].groupIDs);
        row.append(tableEntry5);

        for (i = 5; i < tableData.tableParams.length; i++) {
            for (var paramName in tableData.rowData[j].dynamicParams) {

                if (paramName == tableData.tableParams[i].name) {
                    let tableEntry = $('<td>').addClass('marMaxxSubscriptionEntry_Dynamic').text(tableData.rowData[j].dynamicParams[paramName]);
                    row.append(tableEntry);
                }

            }
        }

        body.append(row);
    }
    subTable.append(body);
    $('#marMaxxSubscriptionData').append(subTable);

    $('#marMaxxSubscriptionsTable').DataTable({
        "lengthMenu": [5, 8, 15, 25]
    });
}