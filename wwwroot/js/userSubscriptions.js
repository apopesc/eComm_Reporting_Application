
const checkBoxEnum = Object.freeze({ "none": 0, "yes": 1, "no": 2, "both": 3 }) //creating javascript enum

$(document).ready(function () {

    //Getting the previously loaded table if there is one
    var controllerUrl = '/SubscriptionGroups/GetInitialTable';

    $.ajax({
        type: "POST",
        url: controllerUrl,
        dataType: "json",
        success: successFunction,
        error: errorFunction
    });

    function successFunction(tableData) {
        if (typeof tableData === 'string') { //If there is an error pulling it from the database
            alert(tableData);
        } else {
            if (tableData != null) {
                createTable(tableData);
            }
        }
    }

    function errorFunction(error) {
        alert("Error Loading Previously Loaded User Table: " + error);
    }

    $('#SubscriptionGroups_Link').addClass('selected-nav-option');

    var groupIDValues = [];
    $('.hidden-groupIDs input').each(function () {
        groupIDValues.push($(this).val());
    });
    var groupDropdownValues = [];
    $("#groupDropdown option").each(function(){
        groupDropdownValues.push($(this).val());
    });
    var masterGroupDropdownValues = [];
    $("#masterGroupDropdown option").each(function () {
        masterGroupDropdownValues.push($(this).val());
    });


    $('#groupDropdown').multiselect({
        nonSelectedText: 'Select a group...',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
    });
    $('#masterGroupDropdown').multiselect({
        nonSelectedText: 'Select a master group...',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
    });


    $("#btnViewUserData").click(function () {

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

        if (selectedMasterGroup == "" || selectedGroup == "" || selectedCheckBoxVal == 0) {

            alert("Please enter a value for Master Group, Group, and Is Active");

        } else {

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
            var controllerUrl = '/SubscriptionGroups/GetTableData';

            var filterData = {
                isActive: selectedCheckBoxVal,
                groupsList: selectedGroup_List,
                masterGroupsList: selectedMasterGroup_List
            }

            $.ajax({
                type: "POST",
                url: controllerUrl,
                dataType: "json",
                success: successFunc,
                error: errorFunc,
                data: {'filterData': filterData}
            });

            function successFunc(tableData) {
                if (typeof tableData === 'string') { //If there is an error saving it to the database
                    alert(tableData);
                } else {
                    createTable(tableData);
                }
            }

            function errorFunc(error) {
                alert("Error Sending Filter Data to the Subscriptions Controller: " + error);
            }
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


    //Triggering change event when the table item is edited
    $('#userSubscriptionData').on('focus', '.userSubscriptionsEntry_Email', function () {
        before = $(this).html();
    }).on('blur keyup paste', '.userSubscriptionsEntry_Email', function () {
        if (before != $(this).html()) { $(this).trigger('change'); }
    });
    //Using the change event to add the edited class to the table row
    $('#userSubscriptionData').on('change', '.userSubscriptionsEntry_Email', function () {
        if (!($(this).closest("tr").hasClass("edited"))) {
            $(this).closest("tr").addClass("edited");
        }
    });

    $('#userSubscriptionData').on('focus', '.userSubscriptionsEntry_isActive', function () {
        before = $(this).html();
    }).on('blur keyup paste', '.userSubscriptionsEntry_isActive', function () {
        if (before != $(this).html()) { $(this).trigger('change'); }
    });
    $('#userSubscriptionData').on('change', '.userSubscriptionsEntry_isActive', function () {
        if (!($(this).closest("tr").hasClass("edited"))) {
            $(this).closest("tr").addClass("edited");
        }
    });

    $('#userSubscriptionData').on('focus', '.userSubscriptionsEntry_Group', function () {
        before = $(this).html();
    }).on('blur keyup paste', '.userSubscriptionsEntry_Group', function () {
        if (before != $(this).html()) { $(this).trigger('change'); }
    });
    $('#userSubscriptionData').on('change', '.userSubscriptionsEntry_Group', function () {
        if (!($(this).closest("tr").hasClass("edited"))) {
            $(this).closest("tr").addClass("edited");
        }
    });

    $('#userSubscriptionData').on('focus', '.userSubscriptionsEntry_GroupID', function () {
        before = $(this).html();
    }).on('blur keyup paste', '.userSubscriptionsEntry_GroupID', function () {
        if (before != $(this).html()) { $(this).trigger('change'); }
    });
    $('#userSubscriptionData').on('change', '.userSubscriptionsEntry_GroupID', function () {
        if (!($(this).closest("tr").hasClass("edited"))) {
            $(this).closest("tr").addClass("edited");
        }
    });

    $('#userSubscriptionData').on('focus', '.userSubscriptionsEntry_masterGroup', function () {
        before = $(this).html();
    }).on('blur keyup paste', '.userSubscriptionsEntry_masterGroup', function () {
        if (before != $(this).html()) { $(this).trigger('change'); }
    });
    $('#userSubscriptionData').on('change', '.userSubscriptionsEntry_masterGroup', function () {
        if (!($(this).closest("tr").hasClass("edited"))) {
            $(this).closest("tr").addClass("edited");
        }
    });


    $('#btnSubmitUser').click(function () {

        var editedUsersList = [];
        var isValidEdit = true;

        $('.userSubscriptionsTable tr').each(function () {
            $currentRow = $(this).closest("tr");
            if ($currentRow.hasClass("edited")) { //Getting data from rows only with the 'edited' class
                //Add input validation here to make sure users don't make crazy edits

                var _ID = $currentRow.attr('id');
                var _userEmail = $currentRow.find(".userSubscriptionsEntry_Email").text();
                var _isActive = $currentRow.find(".userSubscriptionsEntry_isActive").text();
                var _group = $currentRow.find(".userSubscriptionsEntry_Group").text();
                var _groupID = $currentRow.find(".userSubscriptionsEntry_GroupID").text();
                var _masterGroup = $currentRow.find(".userSubscriptionsEntry_masterGroup").text();

                //Input validation for the edited users
                var isValidEmail = ValidateEmail(_userEmail);
                if (isValidEmail == false) {
                    alert(_userEmail + " is an invalid Email.");
                    isValidEdit = false;
                    return false;

                } else if (!(_isActive == 'Y' || _isActive == 'N')) {
                    alert(_userEmail + " has an invalid Is Active value. Please enter Y or N.");
                    isValidEdit = false;
                    return false;

                } else if (!(groupDropdownValues.includes(_group))){
                    alert(_userEmail + " has an invalid Group value. Please enter an existing Group.");
                    isValidEdit = false;
                    return false;

                } else if (!(masterGroupDropdownValues.includes(_masterGroup))){
                    alert(_userEmail + " has an invalid Master Group value. Please enter an existing Master Group.");
                    isValidEdit = false;
                    return false;

                } else if (!(groupIDValues.includes(_groupID))) {
                    alert(_userEmail + " has an invalid Group ID value. Please enter an existing Group ID.");
                    isValidEdit = false;
                    return false;
                }

                let editedUser = {
                    ID: _ID,
                    userEmail: _userEmail,
                    isActive: _isActive,
                    group: _group,
                    groupID: _groupID,
                    masterGroup: _masterGroup
                }
                editedUsersList.push(editedUser);
            }
        });

        if (editedUsersList.length > 0 && isValidEdit == true) {

            var controllerUrl  = '/SubscriptionGroups/EditUser';

            $.ajax({
                dataType: 'json',
                type: 'POST',
                url: controllerUrl,
                data: { 'editedUsersList': editedUsersList },
                success: successFunc,
                error: errorFunc
            });

            function successFunc(editedUserData) {
                alert(editedUserData);
            }

            function errorFunc(error) {
                alert("Error Saving Edited Users: " + error);
            }

        } else  if (isValidEdit == true){
            alert("Please load a table of users and make an edit before submitting.");
        }

    });


    function createTable(tableData) {
        //Clearing table initially
        $('#userSubscriptionData').empty();

        var subTable = $('<table>').addClass('userSubscriptionsTable');

        //Temporarily hard coding table headers---------------------------------------------------------
        let Hrow = $('<tr>').addClass('userSubscriptionsRow_Header')
        let tableHeader_Icons = $('<th>').addClass('userSubscriptionsHeader').text(''); //Invisible header for icons
        Hrow.append(tableHeader_Icons);
        let tableHeader1 = $('<th>').addClass('userSubscriptionsHeader').text('User Email');
        Hrow.append(tableHeader1); //Adding it to the row
        let tableHeader2 = $('<th>').addClass('userSubscriptionsHeader').text('Is Active');
        Hrow.append(tableHeader2);
        let tableHeader3 = $('<th>').addClass('userSubscriptionsHeader').text('Group');
        Hrow.append(tableHeader3);
        let tableHeader4 = $('<th>').addClass('userSubscriptionsHeader').text('Group ID');
        Hrow.append(tableHeader4);
        let tableHeader5 = $('<th>').addClass('userSubscriptionsHeader').text('Master Group');
        Hrow.append(tableHeader5);
        subTable.append(Hrow); //Adding the row to the table

        //----------------------------------------------------------------------------------------------
        for (i = 0; i < tableData.length; i++) {

            if (i == tableData.length - 1) { //This is so that the bottom border isn't added to the last row (it pops out of the table otherwise)
                var row = $('<tr>').addClass('userSubscriptionsRow_Last');
                row.prop('id', tableData[i].id);
            } else {
                var row = $('<tr>').addClass('userSubscriptionsRow');
                row.prop('id', tableData[i].id);
            }

            //Adding the icons to each row ------------------------------------------------------------
            let tableEntry_Icon = $('<td>').addClass('userSubscriptionsEntry_Icon');
            let deleteIcon = $('<button>').addClass('deleteBtn');
            let deleteLink = $('<i>').addClass('fa fa-trash');
            deleteIcon.append(deleteLink);
            tableEntry_Icon.append(deleteIcon);
            row.append(tableEntry_Icon); //This line needs to get deleted if edit is back

            let tableEntry1 = $('<td contenteditable = "true">').addClass('userSubscriptionsEntry_Email').text(tableData[i].userEmail);
            row.append(tableEntry1); //adding element to the row
            let tableEntry2 = $('<td contenteditable = "true">').addClass('userSubscriptionsEntry_isActive').text(tableData[i].isActive);
            row.append(tableEntry2);
            let tableEntry3 = $('<td contenteditable = "true">').addClass('userSubscriptionsEntry_Group').text(tableData[i].group);
            row.append(tableEntry3);
            let tableEntry4 = $('<td contenteditable = "true">').addClass('userSubscriptionsEntry_GroupID').text(tableData[i].groupID);
            row.append(tableEntry4);
            let tableEntry5 = $('<td contenteditable = "true">').addClass('userSubscriptionsEntry_masterGroup').text(tableData[i].masterGroup);
            row.append(tableEntry5);

            subTable.append(row); //adding row to the table
        }
        $('#userSubscriptionData').append(subTable);
    }


    function ValidateEmail(mail) {
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
            return (true);
        }
        return (false);
    }

});