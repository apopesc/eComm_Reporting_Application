
const checkBoxEnum = Object.freeze({"none":0, "yes":1, "no":2, "both":3}) //creating javascript enum

$(document).ready(function () {
    $('#groupDropdown').multiselect();
    $('#groupIDDropdown').multiselect();
    $('#masterGroupDropdown').multiselect();

    $("#btnViewData").click(function () {
        var selectedGroupID = $('#groupIDDropdown').find(":selected").text();
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
        var selectedGroupID_List = selectedGroupID.split('  ');
        
        for (let i = 0; i < selectedGroupID_List.length; i++) {
            selectedGroupID_List[i] = selectedGroupID_List[i].trim();
        }

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
                selectedGroupIDs: selectedGroupID_List,
                selectedGroups: selectedGroup_List,
                selectedMasterGroups: selectedMasterGroup_List
            }
        });

        function successFunc(tableData, status) {
            alert("Data Succesfully Passed to Subscriptions Controller");
            var subTable = $('<table>').addClass('userSubscriptionsTable');

            //Temporarily hard coding table headers-------------------------------------------------
            var Hrow = $('<tr>').addClass('userSubscriptionsRow')
            var tableHeader1 = $('<th>').addClass('userSubscriptionsHeader').text('User_Email');
            Hrow.append(tableHeader1); //Adding it to the row
            var tableHeader2 = $('<th>').addClass('userSubscriptionsHeader').text('Is_Active');
            Hrow.append(tableHeader2);
            var tableHeader3 = $('<th>').addClass('userSubscriptionsHeader').text('Group');
            Hrow.append(tableHeader3);
            var tableHeader4 = $('<th>').addClass('userSubscriptionsHeader').text('Group_ID');
            Hrow.append(tableHeader4);
            var tableHeader5 = $('<th>').addClass('userSubscriptionsHeader').text('MasterGroup');
            Hrow.append(tableHeader5);
            subTable.append(Hrow); //Adding the row to the table

            //-------------------------------------------------------------------------------------
            for (i = 0; i < tableData.length; i++) {
                var row = $('<tr>').addClass('userSubscriptionsRow');
                var tableEntry1 = $('<td>').addClass('userSubscriptionsEntry').text(tableData[i].userEmail);
                row.append(tableEntry1);
                var tableEntry2 = $('<td>').addClass('userSubscriptionsEntry').text(tableData[i].isActive);
                row.append(tableEntry2);
                var tableEntry3 = $('<td>').addClass('userSubscriptionsEntry').text(tableData[i].group);
                row.append(tableEntry3);
                var tableEntry4 = $('<td>').addClass('userSubscriptionsEntry').text(tableData[i].groupID);
                row.append(tableEntry4);
                var tableEntry5 = $('<td>').addClass('userSubscriptionsEntry').text(tableData[i].masterGroup);
                row.append(tableEntry5);
                subTable.append(row);
            }
            $('#userSubscriptionData').append(subTable);
            //data would normally contain the table data, and be used to populate the table
        }

        function errorFunc() {
            alert("Error Sending Filter Data to the Subscriptions Controller");
        }

    })
});