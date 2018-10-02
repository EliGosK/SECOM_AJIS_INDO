
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>

/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

var IsInit04 = false;

$(document).ready(function () {
    MarkAsInit04();
});

function MarkAsInit04() {
    IsInit04 = true;
    LoadParameter();
}

function SetViewMode_04(viewMode) {
    $('#divSiteInformation').SetViewMode(viewMode);
}

function SetDefault_04() {
    $("#divSiteInformation").clearForm();
    SetShow_04(false);
}

function SetShow_04(isShow) {
    if (isShow) {
        $('#divSiteInformation').show();
    } else {
        $('#divSiteInformation').hide();
    }
}

function BindData_04(dataObj) {
    $('#divSiteInformation').bindJSON(dataObj);
}