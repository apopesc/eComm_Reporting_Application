﻿$(document).ready(function () {
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

        if (enteredMasterGroup == null) {
            alert("Please select a value for Master Group");
        } else if (enteredGroupID == '') {
            alert("Please enter a value for Group ID");
        } else if (enteredGroupName == '') {
            alert("Please enter a value for Group Name");
        } else if (enteredGroupID.includes(",")) {
            alert("Group ID can not have a comma ( , ) in it's value. Please enter a Group ID without a comma.");
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
                groupTable.row('.selected').remove().draw(false);
                alert(response + selectedGroup);
            }

            function errorFunc(error) {
                alert(error);
            }
        } else { //User clicks no

        }
    });

    $('#addMasterGroupSubmit').click(function () {
        var newMasterGroup = $("#masterGroup").val();
        if (newMasterGroup == '') {
            alert("Please enter a value for Master Group");
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
                masterGroupTable.row('.selected').remove().draw(false);
                $('#addNewMasterGroup option[value="' + selectedMasterGroup + '"]').remove();
                $('#addNewMasterGroup').multiselect('rebuild');

                alert(response + selectedGroup);
            }

            function errorFunc(error) {
                alert(error);
            }
        } else { //User clicks no

        }
    });
});