
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>

/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

var IsInit02 = false;

$(document).ready(function () {
    MarkAsInit02();
});

function MarkAsInit02() {
    IsInit02 = true;
    LoadParameter();
}

function SetViewMode_02(viewMode) {
    $('#divProjectInformation').SetViewMode(viewMode);
}

function SetDefault_02() {
    $("#divProjectInformation").clearForm();
    SetShow_02(false);
}

function SetShow_02(isShow) {
    if (isShow) {
        $('#divProjectInformation').show();
    } else {
        $('#divProjectInformation').hide();
    }
}

function BindData_02(dataObj) {
    $('#divProjectInformation').bindJSON(dataObj);
}