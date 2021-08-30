$(document).ready(function () {
    $('#marMaxxFolderDropdown').multiselect({
        nonSelectedText: 'Select a folder...',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
        //buttonWidth: 250 For Changing the width of options in the dropdown - may need later
    });
    $('#marMaxxReportDropdown').multiselect({
        nonSelectedText: 'Select a report name...',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
        //buttonWidth: 250 For Changing the width of options in the dropdown - may need later
    });
});