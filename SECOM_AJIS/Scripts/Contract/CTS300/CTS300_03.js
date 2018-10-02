
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>

/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

var CTS300_RowPerPage = ROWS_PER_PAGE_FOR_VIEWPAGE;
var CTS300_gridCustomerGroup;
var IsInit03 = false;
var InitCustomerGrid = false;
$(document).ready(function () {

    CTS300_gridCustomerGroup = $("#CTS300_gridCustomerGroup").InitialGrid(CTS300_RowPerPage, false, "/Contract/CTS300_IntialGridCustomerGroup", function () {
        CTS300_gridCustomerGroup.setSizes();
        InitCustomerGrid = true;
        MarkAsInit03();
    });

});

function MarkAsInit03() {
    IsInit03 = InitCustomerGrid;
    LoadParameter();
}

function SetViewMode_03(viewMode) {
    $('#divCustomerInformation').SetViewMode(viewMode);
}

function SetDefault_03() {
    $("#divCustomerInformation").clearForm();
    SetShow_03(false);
}

function SetShow_03(isShow) {
    if (isShow) {
        $('#divCustomerInformation').show();
    } else {
        $('#divCustomerInformation').hide();
    }
}

function BindData_03(dataObj) {
    $('#divCustomerInformation').bindJSON(dataObj);
}

function BindGrid_03(url, obj, obj_name, func, prefunc) {
    $('#CTS300_gridCustomerGroup').LoadDataToGrid(CTS300_gridCustomerGroup, 0, false, url, obj, obj_name, false, function () {
        CTS300_gridCustomerGroup.setSizes();
        if (func != null) {
            func();
        }
    }, prefunc);
}