var url_string = window.location.href;
url = new URL(url_string);
var isCopyScreen = url.searchParams.get("copy");

$(document).ready(function () {
    
    $('#MarMaxxReports_Link').addClass('selected-nav-option');

    $('#marMaxxGroup').multiselect({
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
    $('#marMaxxReportName').multiselect({
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

    $('#marMaxxGroup').val(selectedGroupIDs);
    $('#marMaxxGroup').multiselect('refresh');

    //Getting report folder name first before getting all dynamic parameter dropdown data
    var selectedReportName = $('#marMaxxReportName option[disabled]:selected').val();

    if ($('#folderName').length) { //If the folder name has been pulled successfully
        var selectedFolderName = $('#folderName').val();
        getDynamicReportParams(selectedReportName, selectedFolderName);
    } else {
        alert("Can't find report/folder. Please delete this report subscription record and create a new one to use updated report data.");
    }

    $('#dynamicParams').on('change', '#Banner', function () {
        if ($('#Department_No').length) {
            if ($('#Banner :selected').length == 0) {

                $('#Department_No').multiselect('deselectAll', false);
                $('#Department_No').multiselect('updateButtonText');

                $('#Class_Number').multiselect('deselectAll', false);
                $('#Class_Number').multiselect('updateButtonText');

                $('#Category').multiselect('deselectAll', false);
                $('#Category').multiselect('updateButtonText');

                $('#Department_No').multiselect('disable');
                $('#Class_Number').multiselect('disable');
                $('#Category').multiselect('disable');

            } else {
                var selectedBannerValues = $('#Banner').val();

                var controllerUrl = '/MarMaxxReports/GetDepartmentData';

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
                    data: {
                        'reportData': reportData,
                        'selectedBanners': selectedBannerValues
                    }
                });

                function successFunc(dropdownData) {
                    var data = [];

                    data.push({ label: "(ALL)", value: "selectAll" });

                    for (i = 0; i < dropdownData.values.length; i++) {
                        data.push({ label: dropdownData.labels[i], value: dropdownData.values[i] });
                    }

                    $("#Department_No").multiselect('dataprovider', data);
                    $('#Department_No').multiselect('enable');
                }

                function errorFunc(error) {
                    alert("Error Retrieving Department Numbers: " + error);
                }
            }
        }
    });

    $('#dynamicParams').on('change', '#Department_No', function () {
        if ($('#Class_Number').length) {
            if ($('#Department_No :selected').length == 0) {

                $('#Class_Number').multiselect('deselectAll', false);
                $('#Class_Number').multiselect('updateButtonText');

                $('#Category').multiselect('deselectAll', false);
                $('#Category').multiselect('updateButtonText');

                $('#Class_Number').multiselect('disable');
                $('#Category').multiselect('disable');

            } else {

                $("#loadMe").modal({
                    backdrop: "static", //remove ability to close modal with click
                    keyboard: false, //remove option to close with keyboard
                    show: true //Display loader!
                });

                var selectedDepartmentValues = $('#Department_No').val();

                if (selectedDepartmentValues.includes('selectAll')) {
                    if (selectedDepartmentValues.length > 1) {
                        var index = selectedDepartmentValues.indexOf('selectAll');
                        var deselectValues = selectedDepartmentValues;
                        deselectValues.splice(index, 1);
                        $('#Department_No').multiselect('deselect', deselectValues);
                    }
                    selectedDepartmentValues = [];
                    $("#Department_No option").each(function () {
                        var thisOptionValue = $(this).val();
                        if (thisOptionValue != 'selectAll') {
                            selectedDepartmentValues.push(thisOptionValue);
                        }
                    });
                }

                var controllerUrl = '/MarMaxxReports/GetClassData';

                var reportData = {
                    reportName: selectedReportName,
                    reportFolder: selectedFolderName
                }

                var departmentData = {
                    'reportData': reportData,
                    'selectedDepartments': selectedDepartmentValues
                }

                var json_DepartmentData = JSON.stringify(departmentData);

                $.ajax({
                    type: "POST",
                    url: controllerUrl,
                    dataType: "json",
                    contentType: "application/json",
                    success: successFunc,
                    error: errorFunc,
                    data: json_DepartmentData
                });

                function successFunc(dropdownData) {
                    var data = [];

                    data.push({ label: "(ALL)", value: "selectAll" });

                    for (i = 0; i < dropdownData.values.length; i++) {
                        data.push({ label: dropdownData.labels[i], value: dropdownData.values[i] });
                    }

                    $("#Class_Number").multiselect('dataprovider', data);
                    $('#Class_Number').multiselect('enable');

                    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
                }

                function errorFunc(error) {
                    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
                    alert("Error Retrieving Classes: " + error);
                }
            }
        }
    });

    $('#dynamicParams').on('change', '#Class_Number', function () {
        if ($('#Category').length) {
            if ($('#Class_Number :selected').length == 0) {

                $('#Category').multiselect('deselectAll', false);
                $('#Category').multiselect('updateButtonText');

                $('#Category').multiselect('disable');

            } else {

                $("#loadMe").modal({
                    backdrop: "static", //remove ability to close modal with click
                    keyboard: false, //remove option to close with keyboard
                    show: true //Display loader!
                });

                var selectedDepartmentValues = $('#Department_No').val();
                var selectedClassValues = $('#Class_Number').val();

                if (selectedDepartmentValues.includes('selectAll')) {
                    $("#Department_No option").each(function () {
                        var thisOptionValue = $(this).val();
                        if (thisOptionValue != 'selectAll') {
                            selectedDepartmentValues.push(thisOptionValue);
                        }
                    });
                }

                if (selectedClassValues.includes('selectAll')) {
                    if (selectedClassValues.length > 1) {
                        var index = selectedClassValues.indexOf('selectAll');
                        var deselectValues = selectedClassValues;
                        deselectValues.splice(index, 1);
                        $('#Class_Number').multiselect('deselect', deselectValues);
                    }
                    selectedClassValues = [];
                    $("#Class_Number option").each(function () {
                        var thisOptionValue = $(this).val();
                        if (thisOptionValue != 'selectAll') {
                            selectedClassValues.push(thisOptionValue);
                        }
                    });
                }

                var controllerUrl = '/MarMaxxReports/GetCategoryData';

                var reportData = {
                    reportName: selectedReportName,
                    reportFolder: selectedFolderName
                }

                var classData = {
                    'reportData': reportData,
                    'selectedDepartments': selectedDepartmentValues,
                    'selectedClasses': selectedClassValues
                };

                var json_ClassData = JSON.stringify(classData);

                $.ajax({
                    type: "POST",
                    url: controllerUrl,
                    dataType: "json",
                    contentType: "application/json",
                    success: successFunc,
                    error: errorFunc,
                    data: json_ClassData
                });

                function successFunc(dropdownData) {
                    var data = [];

                    data.push({ label: "(ALL)", value: "selectAll" });
                    for (i = 0; i < dropdownData.values.length; i++) {
                        data.push({ label: dropdownData.labels[i], value: dropdownData.values[i] });
                    }

                    $("#Category").multiselect('dataprovider', data);
                    $('#Category').multiselect('enable');
                    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
                }

                function errorFunc(error) {
                    setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
                    alert("Error Retrieving Categories: " + error);
                }
            }
        }
    });

    $('#saveSubscription').on('click', '#saveMarmaxxSubscription', function () {
        var subscriptionID;

        if (isCopyScreen == 'false') {
            subscriptionID = $('#subscriptionID').val();
        } else {
            subscriptionID = "0";
        }

        var subscriptionName = $('#subscriptionName').val();

        var groupNames = [];
        $('#marMaxxGroup').find("option:selected").each(function () {
            var groupName = $(this).prop('title');
            groupNames.push(groupName);
        });
            
        var groupIDs = $('#marMaxxGroup').val();

        var fileFormat = $('#fileFormat').val();
        var schedule = $('#schedule').val();
        
        var dynamicParams = {};

        if (subscriptionName == '') {
            alert("Please enter a value for Subscription Name");
        } else if (groupIDs.length == 0) {
            alert("Please select a value for Group ID");
        } else if (groupNames.length == 0) {
            alert("Please select a value for Group Name");
        } else {
            $('#hiddenParamNames > input').each(function () {
                var inputID = this.value;
                var dynamicParamVal = $('#' + inputID).val();

                if (dynamicParamVal !== null) {
                    if (inputID == 'Department_No') {
                        if (dynamicParamVal.includes('selectAll')) {
                            isAllDept = true;
                            dynamicParams[inputID] = 'ALL';
                        } else {
                            dynamicParams[inputID] = dynamicParamVal.toString();
                        }
                    } else if (inputID == 'Class_Number') {
                        if (dynamicParamVal.includes('selectAll')) {
                            if (isAllDept == true) {
                                isAllClass = true;
                                dynamicParams[inputID] = 'ALL';
                            } else {
                                dynamicParamVal = [];
                                $("#Class_Number option").each(function () {
                                    var thisOptionValue = $(this).val();
                                    if (thisOptionValue != 'selectAll') {
                                        dynamicParamVal.push(thisOptionValue);
                                    }
                                });
                                dynamicParams[inputID] = dynamicParamVal.toString();
                            }
                        } else {
                            dynamicParams[inputID] = dynamicParamVal.toString();
                        }
                    } else if (inputID == 'Category') {
                        if (dynamicParamVal.includes('selectAll')) {
                            if (isAllDept == true && isAllClass == true) {
                                dynamicParams[inputID] = 'ALL';
                            } else {
                                dynamicParamVal = [];
                                $("#Category option").each(function () {
                                    var thisOptionValue = $(this).val();
                                    if (thisOptionValue != 'selectAll') {
                                        dynamicParamVal.push(thisOptionValue);
                                    }
                                });
                                dynamicParams[inputID] = dynamicParamVal.toString();
                            }

                        } else {
                            dynamicParams[inputID] = dynamicParamVal.toString();
                        }
                    } else {
                        dynamicParams[inputID] = dynamicParamVal.toString();
                    }
                } else {
                    dynamicParams[inputID] = dynamicParamVal;
                }

            });

            var groupNames_String = groupNames.toString();
            var groupIDs_String = groupIDs.toString();

            if (isCopyScreen == 'false') {
                var controllerUrl = '/MarMaxxReports/SaveEditedMarmaxxReportSubscription';
            } else {
                var controllerUrl = '/MarMaxxReports/SaveMarmaxxReportSubscription';
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
                alert(returnedData.message + subscriptionName);
                if (returnedData.result == 'Redirect') {
                    window.location = returnedData.url;
                }
            }

            function errorFunc(error) {
                alert("Error Getting Report Subscription Data: " + error);
            }
        }
    });

    $('#dynamicParams').on('change', '#Category', function () {
        var selectedCategoryValues = $('#Category').val();

        if (selectedCategoryValues.includes('selectAll')) {
            if (selectedCategoryValues.length > 1) {
                var index = selectedCategoryValues.indexOf('selectAll');
                var deselectValues = selectedCategoryValues;
                deselectValues.splice(index, 1);
                $('#Category').multiselect('deselect', deselectValues);
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
            createParams(paramData);
            if (paramData.parameters.length > 0) {
                selectDynamicParams();
            }
            setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
        }
    }

    function errorFunc(error) {
        setTimeout(function () { $("#loadMe").modal("hide"); }, 500);
        alert("Error Getting Report Parameters: " + error);
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

        if (paramData.parameters[i].name == "Department_No" || paramData.parameters[i].name == "Class_Number" || paramData.parameters[i].name == "Category") {
            $('#' + paramData.parameters[i].name).multiselect({
                enableCaseInsensitiveFiltering: true,
                buttonText: function (options, select) {
                    if (options.length > 0) {
                        var labels = [];
                        options.each(function () {
                            if ($(this).attr('label') !== undefined) {

                                if ($(this).attr('value') == "selectAll") {
                                    labels = [];
                                    labels.push('(ALL)');
                                    return false;
                                } else {
                                    labels.push($(this).attr('label'));
                                }
                            }
                            else {
                                labels.push($(this).html());
                            }
                        });
                        return labels.join(', ') + '';
                    } else {
                        return 'Select a Value...';
                    }
                }
            });

            $('#' + paramData.parameters[i].name).multiselect('disable');
        }
    }

    var saveSubscriptionBtn = $('<button>').addClass('btnAddPage');
    saveSubscriptionBtn.prop('id', 'saveMarmaxxSubscription');

    if (isCopyScreen == 'false') {
        saveSubscriptionBtn.text("Save Edited MarMaxx Report Subscription"); 
    } else {
        saveSubscriptionBtn.text("Save New MarMaxx Report Subscription");
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

                    if ($("#" + paramName).prop("disabled")) { //If it is a disabled multi-select (such as Dept, Class, Etc..)
                        var data = [
                            { label: selectedDynamicParamVals[paramName], value: selectedDynamicParamVals[paramName]}
                        ];
                        $("#" + paramName).multiselect('dataprovider', data);
                        $("#" + paramName).val(selectedDynamicParamVals[paramName]);
                        $("#" + paramName).multiselect("refresh");
                        $("#" + paramName).multiselect('disable');

                    } else { //Not disabled multi-select
                        $("#" + paramName).val(selectedValues);
                        $("#" + paramName).multiselect('refresh');
                    }

                } else { // Single Select Box
                    $("#" + paramName).val(selectedDynamicParamVals[paramName]);
                    $("#" + paramName).multiselect('refresh');
                }

            } else if ($("#" + paramName).is("input")) { //Text box or date box
                $("#" + paramName).val(selectedDynamicParamVals[paramName]);
            }
        }
    }
}