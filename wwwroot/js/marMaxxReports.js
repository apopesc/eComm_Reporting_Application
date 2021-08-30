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
    
});