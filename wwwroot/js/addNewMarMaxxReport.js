﻿$(document).ready(function () {
    $('#MarMaxxReports_Link').addClass('selected-nav-option');

    $('#marMaxxGroup').multiselect({
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
        var selectedReportFolder = $('#marMaxxReportDropdown option:selected').prop('title');

        //adding the selected report name
        $('#hiddenSelectedReport').attr('name', selectedReport); 
        $('#hiddenSelectedReport').attr('folder', selectedReportFolder);

        if (selectedReport == null) {
            alert("Please select a folder, then a report to view it's parameters.");
        } else {

            var controllerUrl = '/MarMaxxReports/GetMarMaxxReportParameters';

            var reportData = {
                reportName: selectedReport,
                reportFolder: selectedReportFolder
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
    });

    $('#saveSubscription').on('click', '#saveMarmaxxSubscription', function () {
        var subscriptionName = $('#subscriptionName').val();
        var groupNames = $('#marMaxxGroup').val();
        var reportName = $('#marMaxxReportDropdown').val();
        var groupIDs = [];
        $('#marMaxxGroup').find("option:selected").each(function () {
            var groupID = $(this).prop('title');
            groupIDs.push(groupID)
        });
        var dynamicParams = {};

        if (subscriptionName == '') {
            alert("Please enter a value for Subscription Name");
        } else if (groupIDs.length == 0) {
            alert("Please select a value for Group ID");
        } else if (groupNames.length == 0) {
            alert("Please select a value for Group Name");
        } else if (reportName.length == 0) {
            alert("Please select a value for Report Name");
        } else {
            $('#hiddenParamNames > input').each(function () {
                var inputID = this.value;
                var dynamicParamVal = $('#' + inputID).val();
                if (dynamicParamVal !== null) {
                    dynamicParams[inputID] = dynamicParamVal.toString();
                } else {
                    dynamicParams[inputID] = dynamicParamVal
                }
                
            });

            var groupNames_String = groupNames.toString();
            var groupIDs_String = groupIDs.toString();
            
            var controllerUrl = '/MarMaxxReports/SaveMarmaxxReportSubscription';

            var savedReportSubModel = {
                subscriptionID: 0,
                subscriptionName: subscriptionName,
                reportName: reportName,
                groupNames: groupNames_String,
                groupIDs: groupIDs_String,
                dynamicParams: dynamicParams
            };

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: { 'savedReportSub': savedReportSubModel }
            });

            function successFunc(returnedData) {
                alert("Success adding subscription: " + subscriptionName);
                if (returnedData.result == 'Redirect') {
                    window.location = returnedData.url;
                }
            }

            function errorFunc(error) {
                alert("Error Getting Report Subscription Data: " + error);
            }
        }
        
    });

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
                    reportName: $('#hiddenSelectedReport').attr('name'),
                    reportFolder: $('#hiddenSelectedReport').attr('folder')
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
                    reportName: $('#hiddenSelectedReport').attr('name'),
                    reportFolder: $('#hiddenSelectedReport').attr('folder')
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
                    reportName: $('#hiddenSelectedReport').attr('name'),
                    reportFolder: $('#hiddenSelectedReport').attr('folder')
                };

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
            $('#' + paramData.parameters[i].name).multiselect({ includeSelectAllOption: true });
            $('#' + paramData.parameters[i].name).multiselect('disable');
        }

        if (paramData.parameters[i].name == "Banner") {
            $('#' + paramData.parameters[i].name).multiselect({ includeSelectAllOption: true });
        }
    }

    var saveSubscriptionBtn = $('<button>').addClass('btnAddPage').text("Save MarMaxx Report Subscription");
    saveSubscriptionBtn.prop('id', 'saveMarmaxxSubscription');

    $('#saveSubscription').append(saveSubscriptionBtn);

    $('.dynamic-dropdown').multiselect({
        nonSelectedText: 'Select a Value...',
        enableCaseInsensitiveFiltering: true
    });
}