// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('#groupDropdown').multiselect();
    $('#groupIDDropdown').multiselect();
    $('#masterGroupDropdown').multiselect();

    $("#btnViewData").click(function () {
        var selectedGroupID = $('#groupIDDropdown').find(":selected").text();
        alert("View Data Clicked");
    })
});