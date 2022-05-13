$(document).ready(function () {
    $('#SierraReports_Link').addClass('selected-nav-option');

    $('#sierraGroup').multiselect({
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

    $('#sierraFolderDropdown').multiselect({
        nonSelectedText: 'Select a folder...',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true
    });

    $('#sierraReportDropdown').multiselect({
        nonSelectedText: 'Select a report name...',
        enableCaseInsensitiveFiltering: true
    });

    $('#fileFormat').multiselect({
        enableCaseInsensitiveFiltering: true
    });

    $('#schedule').multiselect({
        enableCaseInsensitiveFiltering: true
    });

    $('#sierraReportDropdown').multiselect('disable');

    var initialSelectedReport = $('#selectedReport').val();
    var initialSelectedFolder = $('#selectedFolder').val();

    if (initialSelectedReport != "" && initialSelectedFolder != "") {
        $("#loadMe").modal({
            backdrop: "static", //remove ability to close modal with click
            keyboard: false, //remove option to close with keyboard
            show: true //Display loader!
        });

        //adding the selected report name
        $('#hiddenSelectedReport').attr('name', initialSelectedReport);
        $('#hiddenSelectedReport').attr('folder', initialSelectedFolder);

        var controllerUrl = '/SierraReports/GetSierraReportParameters';

        var token = $("#RequestVerificationToken").val();

        var reportData = {
            reportName: initialSelectedReport,
            reportFolder: initialSelectedFolder
        }

        $.ajax({
            type: "POST",
            url: controllerUrl,
            headers: { 'RequestVerificationToken': token },
            dataType: "json",
            success: successFunc,
            error: errorFunc,
            data: { 'reportData': reportData }
        });

        function successFunc(paramData) {
            if (paramData.success == false) { //If there is an error saving it to the database
                alert(paramData.message);
                setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
            } else {

                $('#sierraFolderDropdown').val(initialSelectedFolder);
                $('#sierraFolderDropdown').multiselect('refresh');

                selectedFolder(initialSelectedReport);

                createParams(paramData.reportParams);

                setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
            }
        }

        function errorFunc(error) {
            alert("Error Getting Report Parameters: " + error);
            setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
        }
    }

    $('.filter-data').on('change', '#sierraReportDropdown', function () {
        $("#loadMe").modal({
            backdrop: "static", //remove ability to close modal with click
            keyboard: false, //remove option to close with keyboard
            show: true //Display loader!
        });

        var selectedReport = $('#sierraReportDropdown').val();
        var selectedReportFolder = $('#sierraReportDropdown option:selected').prop('title');

        //adding the selected report name
        $('#hiddenSelectedReport').attr('name', selectedReport);
        $('#hiddenSelectedReport').attr('folder', selectedReportFolder);

        if (selectedReport == null) {
            alert("Please select a folder, then a report to view it's parameters.");
            setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
        } else {

            var controllerUrl = '/SierraReports/GetSierraReportParameters';

            var token = $("#RequestVerificationToken").val();

            var reportData = {
                reportName: selectedReport,
                reportFolder: selectedReportFolder
            }

            $.ajax({
                type: "POST",
                url: controllerUrl,
                headers: { 'RequestVerificationToken': token },
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: { 'reportData': reportData }
            });

            function successFunc(paramData) {
                if (paramData.success == false) { //If there is an error saving it to the database
                    alert(paramData.message);
                    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
                } else {
                    createParams(paramData.reportParams);
                    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
                }
            }

            function errorFunc(error) {
                alert("Error Getting Report Parameters: " + error);
                setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
            }
        }
    });

    $('#saveSubscription').on('click', '#saveSierraSubscription', function () {
        var subscriptionName = $('#subscriptionName').val();

        var isValidString = false;

        if (/^[a-zA-Z0-9- ]*$/.test(subscriptionName)) {
            isValidString = true;
        }

        var groupNames = $('#sierraGroup').val();
        var reportName = $('#sierraReportDropdown').val();
        var groupIDs = [];
        $('#sierraGroup').find("option:selected").each(function () {
            var groupID = $(this).prop('title');
            groupIDs.push(groupID)
        });
        var fileFormat = $('#fileFormat').val();
        var schedule = $('#schedule').val();
        var dynamicParams = {};

        if (subscriptionName == '') {
            alert("Please enter a value for Subscription Name");
        } else if (groupIDs.length == 0) {
            alert("Please select a value for Group ID");
        } else if (groupNames.length == 0) {
            alert("Please select a value for Group Name");
        } else if (reportName.length == 0) {
            alert("Please select a value for Report Name");
        } else if (isValidString == false) {
            alert("Subscription Name contains special characters, you can only enter values a-z, A-Z, 0-9, space( ), and hyphen(-).");
        } else {

            $('#hiddenParamNames > input').each(function () {
                var inputID = this.value;
                var dynamicParamVal = $('#' + inputID).val();

                if (dynamicParamVal !== null) {

                    if (dynamicParamVal.includes('selectAll')) {
                        dynamicParams[inputID] = 'ALL';
                    } else {
                        dynamicParams[inputID] = dynamicParamVal.toString();
                    }

                } else {
                    dynamicParams[inputID] = dynamicParamVal;
                }

            });

            var groupNames_String = groupNames.toString();
            var groupIDs_String = groupIDs.toString();

            var controllerUrl = '/SierraReports/SaveSierraReportSubscription';

            var token = $("#RequestVerificationToken").val();

            var savedReportSubModel = {
                subscriptionID: 0,
                subscriptionName: subscriptionName,
                reportName: reportName,
                groupNames: groupNames_String,
                groupIDs: groupIDs_String,
                fileFormat: fileFormat,
                schedule: schedule,
                dynamicParams: dynamicParams
            };

            $.ajax({
                type: "POST",
                url: controllerUrl,
                headers: { 'RequestVerificationToken': token },
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: { 'reportSub': savedReportSubModel }
            });

            function successFunc(returnedData) {
                if (returnedData.result == 'Redirect') {
                    alert(returnedData.message + subscriptionName);
                    window.location = returnedData.url;
                } else if (returnedData.result == 'Error') {
                    alert(returnedData.message);
                }
            }

            function errorFunc(error) {
                alert("Error Getting Report Subscription Data: " + error);
            }
        }

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

        var token = $("#RequestVerificationToken").val();

        $.ajax({
            type: "POST",
            url: controllerUrl,
            headers: { 'RequestVerificationToken': token },
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

function createParams(paramData) {
    $('#dynamicParams').empty();
    $('#saveSubscription').empty();
    $('#hiddenParamNames').empty();

    for (let i = 0; i < paramData.parameters.length; i++) {

        //adding parameter name to hidden list on the page (used for input validation on save)
        var hiddenParam = $('<input type="hidden">').prop('value', paramData.parameters[i].name);
        $('#hiddenParamNames').append(hiddenParam);

        //Building out the dynamic dropdowns
        var row = $('<div>').addClass('addnew-row');

        if (paramData.parameters[i].type == "Dropdown" || paramData.parameters[i].type == "MultiDropdown") {

            var sub_row = $('<div>').addClass('addnew-dropdown');

            var dropdownLabel = $('<label>').addClass('filter-label').text(paramData.parameters[i].name);
            dropdownLabel.prop('for', paramData.parameters[i].name);

            var dropdown;
            if (paramData.parameters[i].type == "MultiDropdown") {
                dropdown = $('<select multiple>').addClass('dynamic-dropdown');
            } else {
                dropdown = $('<select>').addClass('dynamic-dropdown');
            }

            dropdown.prop('id', paramData.parameters[i].name);
            dropdown.prop('name', paramData.parameters[i].name);

            //This is to prevent errors in case there is an instance where values and labels are diff length. (avoiding out of index)
            var length = 0;
            if (paramData.parameters[i].values.length > paramData.parameters[i].labels.length) {
                length = paramData.parameters[i].labels.length;
            } else {
                length = paramData.parameters[i].values.length;
            }

            if (paramData.parameters[i].type == "Dropdown") {
                var defaultDropdownOption = $('<option disabled selected>').text("Select a Value...");
                dropdown.append(defaultDropdownOption);
            }

            else if (paramData.parameters[i].type == "MultiDropdown") {
                dropdown.addClass('multiselect_dynamic');
            }

            for (let j = 0; j < length; j++) {
                var dropdownOption = $('<option>').text(paramData.parameters[i].labels[j]);
                dropdownOption.prop('value', paramData.parameters[i].values[j]);

                dropdown.append(dropdownOption);
            }

            sub_row.append(dropdownLabel);
            sub_row.append(dropdown);
            row.append(sub_row);

        } else if (paramData.parameters[i].type == "Textbox") {
            row.addClass("textbox");

            var sub_row = $('<div>').addClass('addnew-textbox');
            var textboxLabel = $('<label>').addClass('filter-label').text(paramData.parameters[i].name);
            textboxLabel.prop('for', paramData.parameters[i].name);

            //values at 0 because textboxes will have a list only with one val in it.
            var textbox = $('<input type=text>').addClass('subscription-textbox').val(paramData.parameters[i].values[0]);
            textbox.prop('id', paramData.parameters[i].name);
            textbox.prop('name', paramData.parameters[i].name);

            sub_row.append(textboxLabel);
            sub_row.append(textbox);
            row.append(sub_row);

        } else if (paramData.parameters[i].type == "DateTime") {
            row.addClass("textbox");
            row.addClass("date-input");

            var sub_row = $('<div>').addClass('addnew-textbox');
            var dateTimeLabel = $('<label>').addClass('filter-label').text(paramData.parameters[i].name);
            dateTimeLabel.prop('for', paramData.parameters[i].name);

            var dateTime = $('<input type="date">').addClass('subscription-textbox');
            dateTime.prop('id', paramData.parameters[i].name);
            dateTime.prop('name', paramData.parameters[i].name);

            sub_row.append(dateTimeLabel);
            sub_row.append(dateTime);
            row.append(sub_row);
        }
        $('#dynamicParams').append(row);

        if (paramData.parameters[i].type == "MultiDropdown") {

            $('#' + paramData.parameters[i].name).multiselect({
                nonSelectedText: 'Select a value...',
                enableCaseInsensitiveFiltering: true,
                includeSelectAllOption: true
            });
        }
        if (paramData.parameters[i].defaultVal != null) {
            $('#' + paramData.parameters[i].name).val(paramData.parameters[i].defaultVal);
            $('#' + paramData.parameters[i].name).multiselect('refresh');
        }
    }

    var saveSubscriptionBtn = $('<button>').addClass('btnAddPage').text("Save Sierra Report Subscription");
    saveSubscriptionBtn.prop('id', 'saveSierraSubscription');

    $('#saveSubscription').append(saveSubscriptionBtn);

    $('.dynamic-dropdown').multiselect({
        nonSelectedText: 'Select a Value...',
        enableCaseInsensitiveFiltering: true
    });
}