$(document).ready(function () {
    $('#Admin_Link').addClass('selected-nav-option');

    $('#addNewMasterGroup').multiselect({
        nonSelectedText: 'Select a master group...',
        enableCaseInsensitiveFiltering: true
    });

    $('#groupsTable').DataTable({
        "lengthMenu": [5, 8, 15, 25]
    });
});