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
});