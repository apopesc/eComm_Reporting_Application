$(document).ready(function () {
    $('#Admin_Link').addClass('selected-nav-option');

    $('#addNewMasterGroup').multiselect({
        nonSelectedText: 'Select a master group...',
        enableCaseInsensitiveFiltering: true
    });

    $('#groupsTable').DataTable({
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
                    $('#groupsTable').prepend('<tr><td><button class="deleteBtn"><i class="fa fa-trash"></i></button></td> <td class="masterGroupEntry">' + returnedData.saved_masterGroup + '</td> <td class="groupIDEntry">' + returnedData.new_groupID + '</td> <td class="groupNameEntry">' + returnedData.new_groupName+'</td> </tr>')
                } else {
                    alert("Failed");
                }
            }

            function errorFunc(error) {
                alert("Error Saving New User: " + error);
            }
        }

    });
});