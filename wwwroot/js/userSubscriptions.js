
const checkBoxEnum = Object.freeze({ "none": 0, "yes": 1, "no": 2, "both": 3 }) //creating javascript enum

$(document).ready(function () {

    $('#groupDropdown').multiselect( { includeSelectAllOption:true } );
    $('#masterGroupDropdown').multiselect( { includeSelectAllOption:true } );

    $('#btnSubmit').prop('disabled', true);
    $('#btnSubmit').css('cursor', 'not-allowed');

    $("#btnViewData").click(function () {
        $('#btnSubmit').prop('disabled', false);
        $('#btnSubmit').css('cursor', 'pointer');

        var selectedGroup = $('#groupDropdown').find(":selected").text();
        var selectedMasterGroup = $('#masterGroupDropdown').find(":selected").text();

        var selectedCheckBoxVal = 0;

        if ($('#checkYes').is(':checked') && !($('#checkNo').is(':checked'))) { //Only yes is selected
            selectedCheckBoxVal = checkBoxEnum.yes; //1
        } else if ($('#checkNo').is(':checked') && !($('#checkYes').is(':checked'))) { //Only no is selected
            selectedCheckBoxVal = checkBoxEnum.no; //2
        } else if ($('#checkYes').is(':checked') && $('#checkNo').is(':checked')) { //Both are selected
            selectedCheckBoxVal = checkBoxEnum.both; //3
        } else { //Neither are selected
            selectedCheckBoxVal = checkBoxEnum.none; //0
        }

        //Turning the dropdown selected values into lists, removing whitespace
        var selectedGroup_List = selectedGroup.split('  ');
        for (let i = 0; i < selectedGroup_List.length; i++) {
            selectedGroup_List[i] = selectedGroup_List[i].trim();
        }

        var selectedMasterGroup_List = selectedMasterGroup.split('  ');
        for (let i = 0; i < selectedMasterGroup_List.length; i++) {
            selectedMasterGroup_List[i] = selectedMasterGroup_List[i].trim();
        }

        //Posting collected filter data back to the SubscriptionsGroupsController
        var controllerUrl = '/SubscriptionGroups/ReceiveFilters';

        $.ajax({
            type: "POST",
            url: controllerUrl,
            dataType: "json",
            success: successFunc,
            error: errorFunc,
            data: {
                isActive: selectedCheckBoxVal,
                selectedGroups: selectedGroup_List,
                selectedMasterGroups: selectedMasterGroup_List
            }
        });

        function successFunc(tableData) {
            if (typeof tableData === 'string') { //If there is an error saving it to the database
                alert(tableData);
            } else {
                //Clearing table initially
                $('#userSubscriptionData').empty();

                var subTable = $('<table>').addClass('userSubscriptionsTable');

                //Temporarily hard coding table headers---------------------------------------------------------
                let Hrow = $('<tr>').addClass('userSubscriptionsRow_Header')
                let tableHeader_Icons = $('<th>').addClass('userSubscriptionsHeader').text(''); //Invisible header for icons
                Hrow.append(tableHeader_Icons);
                let tableHeader1 = $('<th>').addClass('userSubscriptionsHeader').text('User_Email');
                Hrow.append(tableHeader1); //Adding it to the row
                let tableHeader2 = $('<th>').addClass('userSubscriptionsHeader').text('Is_Active');
                Hrow.append(tableHeader2);
                let tableHeader3 = $('<th>').addClass('userSubscriptionsHeader').text('Group');
                Hrow.append(tableHeader3);
                let tableHeader4 = $('<th>').addClass('userSubscriptionsHeader').text('Group_ID');
                Hrow.append(tableHeader4);
                let tableHeader5 = $('<th>').addClass('userSubscriptionsHeader').text('MasterGroup');
                Hrow.append(tableHeader5);
                subTable.append(Hrow); //Adding the row to the table

                //----------------------------------------------------------------------------------------------
                for (i = 0; i < tableData.length; i++) {

                    if (i == tableData.length - 1) { //This is so that the bottom border isn't added to the last row (it pops out of the table otherwise)
                        var row = $('<tr>').addClass('userSubscriptionsRow_Last').attr('id', tableData[i].userEmail); //adding the class for styling and the ID for potential later use
                    } else {
                        var row = $('<tr>').addClass('userSubscriptionsRow').attr('id', tableData[i].userEmail);
                    }

                    //Adding the icons to each row ------------------------------------------------------------
                    let tableEntry_Icons = $('<td>').addClass('userSubscriptionsEntry_Icons');
                    let deleteIcon = $('<button>').addClass('deleteBtn');
                    let deleteLink = $('<i>').addClass('fa fa-trash');
                    deleteIcon.append(deleteLink);
                    tableEntry_Icons.append(deleteIcon);

                    let editIcon = $('<button>').addClass('editBtn');
                    let editLink = $('<i>').addClass('fas fa-pencil-alt');
                    editIcon.append(editLink);
                    tableEntry_Icons.append(editIcon);
                    row.append(tableEntry_Icons);
                    //-----------------------------------------------------------------------------------------

                    let tableEntry1 = $('<td>').addClass('userSubscriptionsEntry_Email').text(tableData[i].userEmail);
                    row.append(tableEntry1); //adding element to the row
                    let tableEntry2 = $('<td>').addClass('userSubscriptionsEntry_isActive').text(tableData[i].isActive);
                    row.append(tableEntry2);
                    let tableEntry3 = $('<td>').addClass('userSubscriptionsEntry_Group').text(tableData[i].group);
                    row.append(tableEntry3);
                    let tableEntry4 = $('<td>').addClass('userSubscriptionsEntry_GroupID').text(tableData[i].groupID);
                    row.append(tableEntry4);
                    let tableEntry5 = $('<td>').addClass('userSubscriptionsEntry_masterGroup').text(tableData[i].masterGroup);
                    row.append(tableEntry5);

                    subTable.append(row); //adding row to the table
                }
                $('#userSubscriptionData').append(subTable);
            }
        }

        function errorFunc(error) {
            alert("Error Sending Filter Data to the Subscriptions Controller: " + error);
        }

    });


    $('#userSubscriptionData').on('click', '.deleteBtn', function () { //Need to use on click for a dynamically generated element

        let selectedEmail = $(this).closest("tr")
            .find(".userSubscriptionsEntry_Email")
            .text();

        let selectedActive = $(this).closest("tr")
            .find(".userSubscriptionsEntry_isActive")
            .text();

        let selectedGroup = $(this).closest("tr")
            .find(".userSubscriptionsEntry_Group")
            .text();

        let selectedGroupID = $(this).closest("tr")
            .find(".userSubscriptionsEntry_GroupID")
            .text();

        let selectedMasterGroup = $(this).closest("tr")
            .find(".userSubscriptionsEntry_masterGroup")
            .text();

        alert("Selected Item to be Deleted - Email: " + selectedEmail + ", Active: "
            + selectedActive + ", Group: " + selectedGroup + ", Group ID: " + selectedGroupID + ", Master Group: " + selectedMasterGroup);
    });


    $('#userSubscriptionData').on('click', '.editBtn', function () { //Need to use on click for a dynamically generated element

        let selectedEmail = $(this).closest("tr")
            .find(".userSubscriptionsEntry_Email")
            .text();

        let selectedActive = $(this).closest("tr")
            .find(".userSubscriptionsEntry_isActive")
            .text();

        let selectedGroup = $(this).closest("tr")
            .find(".userSubscriptionsEntry_Group")
            .text();

        let selectedGroupID = $(this).closest("tr")
            .find(".userSubscriptionsEntry_GroupID")
            .text();

        let selectedMasterGroup = $(this).closest("tr")
            .find(".userSubscriptionsEntry_masterGroup")
            .text();

        alert("Selected Item to be Edited - Email: " + selectedEmail + ", Active: "
            + selectedActive + ", Group: " + selectedGroup + ", Group ID: " + selectedGroupID + ", Master Group: " + selectedMasterGroup);
    });

});