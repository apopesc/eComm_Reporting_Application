﻿$(document).ready(function () {
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
        var paramName = $(this).attr("name");
        var paramValue = $(this).val();
        selectedDynamicParamVals[paramName] = paramValue;
    });

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
                var selectedDepartmentValues = $('#Department_No').val();

                var controllerUrl = '/MarMaxxReports/GetClassData';

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
                        'selectedDepartments': selectedDepartmentValues
                    }
                });

                function successFunc(dropdownData) {
                    var data = [];

                    for (i = 0; i < dropdownData.values.length; i++) {
                        data.push({ label: dropdownData.labels[i], value: dropdownData.values[i] });
                    }

                    $("#Class_Number").multiselect('dataprovider', data);
                    $('#Class_Number').multiselect('enable');
                }

                function errorFunc(error) {
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
                var selectedDepartmentValues = $('#Department_No').val();
                var selectedClassValues = $('#Class_Number').val();

                var controllerUrl = '/MarMaxxReports/GetCategoryData';

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
                        'selectedDepartments': selectedDepartmentValues,
                        'selectedClasses': selectedClassValues
                    }
                });

                function successFunc(dropdownData) {
                    var data = [];

                    for (i = 0; i < dropdownData.values.length; i++) {
                        data.push({ label: dropdownData.labels[i], value: dropdownData.values[i] });
                    }

                    $("#Category").multiselect('dataprovider', data);
                    $('#Category').multiselect('enable');
                }

                function errorFunc(error) {
                    alert("Error Retrieving Categories: " + error);
                }
            }
        }
    });
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
            createParams(paramData);
        }
    }

    function errorFunc(error) {
        alert("Error Getting Report Parameters: " + error);
    }
}

function createParams(paramData) {
    $('#dynamicParams').empty();

    for (let i = 0; i < paramData.parameters.length; i++) {

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

        if (paramData.parameters[i].name == "Department_No" || paramData.parameters[i].name == "Class_Number" || paramData.parameters[i].name == "Category") {
            $('#' + paramData.parameters[i].name).multiselect('disable');
        }
    }

    $('.dynamic-dropdown').multiselect({
        nonSelectedText: 'Select a Value...',
        enableCaseInsensitiveFiltering: true
    });
}