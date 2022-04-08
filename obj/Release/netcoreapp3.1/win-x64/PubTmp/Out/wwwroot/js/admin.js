$(document).ready(function () {
    $('#Admin_Link').addClass('selected-nav-option');

    $('#addNewMasterGroup').multiselect({
        nonSelectedText: 'Select a master group...',
        enableCaseInsensitiveFiltering: true
    });

    var groupTable = $('#groupsTable').DataTable({
        "lengthMenu": [5, 8, 15, 25]
    });

    var masterGroupTable = $('#masterGroupsTable').DataTable({
        "lengthMenu": [5, 8, 15, 25]
    });

    $("#addGroupSubmit").click(function () {
        var enteredMasterGroup = $("#addNewMasterGroup").val();
        var enteredGroupID = $("#addNewGroupID").val();
        var enteredGroupName = $("#addNewGroupName").val();

        var validGroupName = /^[a-zA-Z0-9- ]*$/.test(enteredGroupName);
        var validGroupID = /^[a-zA-Z0-9- ]*$/.test(enteredGroupID);

        if (enteredMasterGroup == null) {
            alert("Please select a value for Master Group");
        } else if (enteredGroupID == '') {
            alert("Please enter a value for Group ID");
        } else if (enteredGroupName == '') {
            alert("Please enter a value for Group Name");
        }  else if (validGroupName == false) {
            alert("Group Name contains special characters, you can only enter values a-z, A-Z, 0-9, space( ), and hyphen(-).");
        } else if (validGroupID == false) {
            alert("Group ID contains special characters, you can only enter values a-z, A-Z, 0-9, space( ), and hyphen(-).");
        } else {
            var controllerUrl = '/Admin/AddNewGroup';

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: {
                    masterGroup: enteredMasterGroup,
                    groupID: enteredGroupID,
                    groupName: enteredGroupName
                }
            });

            function successFunc(returnedData) {
                if (returnedData.success == true) {
                    alert("Success adding group: " + returnedData.new_groupName);
                    $('#groupsTable').prepend('<tr class="newRow"><td><button class="deleteBtn"><i class="fa fa-trash"></i></button></td> <td class="masterGroupEntry">' + returnedData.saved_masterGroup + '</td> <td class="groupIDEntry">' + returnedData.new_groupID + '</td> <td class="groupNameEntry">' + returnedData.new_groupName + '</td> </tr>');
                } else {
                    alert(returnedData.errorMsg)
                }
               
            }

            function errorFunc(error) {
                alert(returnedData.errorMsg)
            }
        }
    });

    $('#groupsTable').on('click', '.deleteBtn', function () { //Need to use on click for a dynamically generated element

        var $selectedRow = $(this).closest("tr");

        let selectedGroup = $selectedRow
            .find(".groupNameEntry")
            .text();

        let selectedGroupID = $selectedRow
            .find(".groupIDEntry")
            .text();

        if (selectedGroupID != null && selectedGroupID != "") {
            if (confirm('Are you sure you want to delete group: ' + selectedGroup + '? You will no longer be able to view user data tied to this group.')) {

                $selectedRow.addClass('selected');
                var controllerUrl = '/Admin/DeleteGroup';

                $.ajax({
                    type: "POST",
                    url: controllerUrl,
                    dataType: "json",
                    success: successFunc,
                    error: errorFunc,
                    data: { 'groupID': selectedGroupID }
                });

                function successFunc(response) {
                    //$selectedRow.remove();
                    if (response.success == true) {
                        groupTable.row('.selected').remove().draw(false);
                        alert(response.message + selectedGroup);
                    } else {
                        alert(response.message);
                    }

                }

                function errorFunc(error) {
                    alert(error);
                }
            } else { //User clicks no

            }
        } else {
            alert("Could not delete group: group ID is null");
        }

        
    });

    $('#addMasterGroupSubmit').click(function () {
        var newMasterGroup = $("#masterGroup").val();

        var validMasterGroup = /^[a-zA-Z0-9- ]*$/.test(newMasterGroup);

        if (newMasterGroup == '') {
            alert("Please enter a value for Master Group");
        } else if (validMasterGroup == false) {
            alert("Master Group contains special characters, you can only enter values a-z, A-Z, 0-9, space( ), and hyphen(-).");
        } else {
            var controllerUrl = '/Admin/AddNewMasterGroup';

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: {
                    masterGroup: newMasterGroup
                }
            });

            function successFunc(returnedData) {
                if (returnedData.success == true) {
                    alert("Success adding master group: " + returnedData.saved_masterGroup);
                    $('#masterGroupsTable').prepend('<tr class="newRow"><td><button class="deleteBtn"><i class="fa fa-trash"></i></button></td> <td class="new_masterGroupEntry">' + returnedData.saved_masterGroup + '</td></tr>');

                    $('#addNewMasterGroup option').eq(1).before($('<option></option>').val(returnedData.saved_masterGroup).text(returnedData.saved_masterGroup));
                    $('#addNewMasterGroup').multiselect('rebuild');

                } else {
                    alert(returnedData.errorMsg)
                }

            }

            function errorFunc(error) {
                alert(returnedData.errorMsg)
            }
        }
    });

    $('#masterGroupsTable').on('click', '.deleteBtn', function () { //Need to use on click for a dynamically generated element

        var $selectedRow = $(this).closest("tr");

        let selectedMasterGroup = $selectedRow
            .find(".new_masterGroupEntry")
            .text();

        if (selectedMasterGroup != null || selectedMasterGroup == "") {
            if (confirm('Are you sure you want to delete master group: ' + selectedMasterGroup + '? You will no longer be able to view user data tied to this group.')) {

                $selectedRow.addClass('selected');
                var controllerUrl = '/Admin/DeleteMasterGroup';

                $.ajax({
                    type: "POST",
                    url: controllerUrl,
                    dataType: "json",
                    success: successFunc,
                    error: errorFunc,
                    data: { 'masterGroup': selectedMasterGroup }
                });

                function successFunc(response) {
                    if (response.success == true) {
                        masterGroupTable.row('.selected').remove().draw(false);
                        $('#addNewMasterGroup option[value="' + selectedMasterGroup + '"]').remove();
                        $('#addNewMasterGroup').multiselect('rebuild');

                        alert(response.message + selectedMasterGroup);
                    } else {
                        alert(response.message);
                    }
                    
                }

                function errorFunc(error) {
                    alert(error);
                }
            } else { //User clicks no

            }
        } else {
            alert("Could not delete master group: the master group field is null");
        }
    });
});