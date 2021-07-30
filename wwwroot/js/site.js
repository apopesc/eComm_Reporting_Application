
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

        //Creating data object to be passed to the controller
        var filterData = {
            isActive: selectedCheckBoxVal,
            selectedGroupIDs: selectedGroupID_List,
            selectedGroups: selectedGroup_List,
            selectedMasterGroups: selectedMasterGroup_List
        }

        //Posting collected filter data back to the SubscriptionsGroupsController
        var controllerUrl = '/SubscriptionGroups/ReceiveFilters';

        $.ajax({
            type: "POST",
            url: controllerUrl,
            contentType: 'application/json',
            dataType: "json",
            success: successFunc,
            error: errorFunc,
            data: JSON.stringify(filterData)
        });

        function successFunc(data, status) {
            alert(data);
        }

        function errorFunc() {
            alert("Error Sending Filter Data to the Subscriptions Controller");
        }

        //Have to put AJAX Call here to pass to a controller function (remember to put a URL in the AJAX call that points to the controller)
    })
});