var url_string = window.location.href;
url = new URL(url_string);
var isCopyScreen = url.searchParams.get("copy");

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
                        labels.push($(this).attr('title'));
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

    $('#sierraReportName').multiselect({
        nonSelectedText: 'Select a report name...',
        enableCaseInsensitiveFiltering: true
    });

    $('#fileFormat').multiselect({
        enableCaseInsensitiveFiltering: true
    });

    $('#schedule').multiselect({
        enableCaseInsensitiveFiltering: true
    });

    //Getting group IDs and Names from hidden parameters
    var selectedGroupIDsString = "";
    $('.hidden-selected-groups input').each(function () {
        if ($(this).attr("id") == "selectedGroupIDs") {
            selectedGroupIDsString = $(this).val();
        }
    });

    //setting selected values of group IDs and group names in the dropdowns
    var selectedGroupIDs = selectedGroupIDsString.split(",");

    $('#sierraGroup').val(selectedGroupIDs);
    $('#sierraGroup').multiselect('refresh');

    var selectedReportName = $('#sierraReportName option[disabled]:selected').val();

    if ($('#folderName').length) { //If the folder name has been pulled successfully
        var selectedFolderName = $('#folderName').val();
        getDynamicReportParams(selectedReportName, selectedFolderName);                     //GETTING THE DYNAMIC REPORT PARAMETERS 
    } else {
        alert("Can't find report/folder. Please delete this report subscription record and create a new one to use updated report data.");
    }

    $('#saveSubscription').on('click', '#saveSierraSubscription', function () {
        var subscriptionID;

        if (isCopyScreen == 'false') {
            subscriptionID = $('#subscriptionID').val();
        } else {
            subscriptionID = "0";
        }

        var subscriptionName = $('#subscriptionName').val();

        var isValidString = false;

        if (/^[a-zA-Z0-9- ]*$/.test(subscriptionName)) {
            isValidString = true;
        }

        var groupNames = [];
        $('#sierraGroup').find("option:selected").each(function () {
            var groupName = $(this).prop('title');
            groupNames.push(groupName);
        });
        var groupIDs = $('#sierraGroup').val();
        var fileFormat = $('#fileFormat').val();
        var schedule = $('#schedule').val();
        var dynamicParams = {};

        if (subscriptionName == '') {
            alert("Please enter a value for Subscription Name");
        } else if (groupIDs.length == 0) {
            alert("Please select a value for Group ID");
        } else if (groupNames.length == 0) {
            alert("Please select a value for Group Name");
        } else if (isValidString == false) {
            alert("Subscription Name contains special characters, you can only enter values a-z, A-Z, 0-9, space( ), and hyphen(-).");
        } else {
            $('#hiddenParamNames > input').each(function () {
                var inputID = this.value;
                var dynamicParamVal = $('#' + inputID).val();

                if (dynamicParamVal !== null) {
                    dynamicParams[inputID] = dynamicParamVal.toString();
                } else {
                    dynamicParams[inputID] = dynamicParamVal;
                }

            });

            var groupNames_String = groupNames.toString();
            var groupIDs_String = groupIDs.toString();

            if (isCopyScreen == 'false') {
                var controllerUrl = '/SierraReports/SaveEditedSierraReportSubscription';
            } else {
                var controllerUrl = '/SierraReports/SaveSierraReportSubscription';
            }

            var editedReportSubModel = {
                subscriptionID: parseInt(subscriptionID),
                subscriptionName: subscriptionName,
                reportName: selectedReportName,
                groupNames: groupNames_String,
                groupIDs: groupIDs_String,
                fileFormat: fileFormat,
                schedule: schedule,
                dynamicParams: dynamicParams
            };

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: { 'reportSub': editedReportSubModel }
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
                alert("Error Saving Report Subscription Data: " + error);
            }
        }
    });

});


function getDynamicReportParams(selectedReportName, selectedFolderName) {
    $("#loadMe").modal({
        backdrop: "static", //remove ability to close modal with click
        keyboard: false, //remove option to close with keyboard
        show: true //Display loader!
    });

    var controllerUrl = '/SierraReports/GetSierraReportParameters';

    var reportData = {
        reportName: selectedReportName,
        reportFolder: selectedFolderName
    }

    if (selectedReportName == null || selectedReportName == "") {
        alert("Could not load report parameters: Report Name is empty.");
    } else if (selectedFolderName == null || selectedFolderName == "") {
        alert("Could not load report parameters: Folder is empty.");
    } else {
        $.ajax({
            type: "POST",
            url: controllerUrl,
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
                if (paramData.reportParams.parameters.length > 0) {
                    selectDynamicParams();                    //SELECTING THE DYNAMIC PARAMS
                } else {
                    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
                }
            }
        }

        function errorFunc(error) {
            alert("Error Getting Report Parameters: " + error);
            setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
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
        if (i == 0) {
            row.addClass('first-row');
        }

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

            var textbox = $('<input type=text>').addClass('subscription-textbox');
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
    }

    var saveSubscriptionBtn = $('<button>').addClass('btnAddPage');
    saveSubscriptionBtn.prop('id', 'saveSierraSubscription');

    if (isCopyScreen == 'false') {
        saveSubscriptionBtn.text("Save Edited Sierra Report Subscription");
    } else {
        saveSubscriptionBtn.text("Save New Sierra Report Subscription");
    }

    $('#saveSubscription').append(saveSubscriptionBtn);

    $('.dynamic-dropdown').multiselect({
        nonSelectedText: 'Select a Value...',
        enableCaseInsensitiveFiltering: true
    });
}

function selectDynamicParams() {

    var selectedDynamicParamVals = {};

    //Getting the selected dynamic parameter values from hidden
    $('.hidden-dynamic-params input').each(function () {
        var paramName = $(this).attr("name");
        var paramValue = $(this).val();
        selectedDynamicParamVals[paramName] = paramValue;
    });

    for (var paramName in selectedDynamicParamVals) {

        if ($("#" + paramName).length) {
            if ($("#" + paramName).is("select")) {

                if ($("#" + paramName).prop("multiple")) { // Multi Select Box

                    var selectedValues = selectedDynamicParamVals[paramName].split(',');

                    $("#" + paramName).val(selectedValues);
                    $("#" + paramName).multiselect('refresh');

                } else { // Single Select Box
                    $("#" + paramName).val(selectedDynamicParamVals[paramName]);
                    $("#" + paramName).multiselect('refresh');
                }

            } else if ($("#" + paramName).is("input")) { //Text box or date box
                $("#" + paramName).val(selectedDynamicParamVals[paramName]);
            }
        }
    }

    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
}
