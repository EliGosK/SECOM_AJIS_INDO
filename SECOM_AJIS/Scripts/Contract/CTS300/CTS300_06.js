
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>

/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

var hidViewIncidentID = null;
var IsInit06 = false;

$(document).ready(function () {
    $('#btnViewEditIncidentDetail').click(btnViewEditIncidentDetail_clicked);
    $('#btnRegisterNextIncident').click(btnRegisterNextIncident_clicked);
    MarkAsInit06();
});

function MarkAsInit06() {
    IsInit06 = true;
    LoadParameter();
}

function btnViewEditIncidentDetail_clicked() {
    var obj = {
        strIncidentID: hidViewIncidentID
    };
    ajax_method.CallScreenControllerWithAuthority("/Contract/CTS330", obj, true);
}

function btnRegisterNextIncident_clicked() {
    lastCondition = null;
    SetScreenToDefault();
}

function SetDefault_06() {
    $('#IncidentNo').val('');
    $('#IncidentStatus').val('');
    $('#hidViewIncidentNo').val('');
    hidViewIncidentID = null;

    SetShow_06(false);
}

function SetObject_06(obj) {
    $('#IncidentNo').val(obj.IncidentNo);
    $('#IncidentStatus').val(obj.RegisStatus);
    //$('#hidViewIncidentID').val(obj.IncidentID);
    hidViewIncidentID = obj.IncidentID;

    if ((obj.IncidentID == null) || (obj.IncidentID == undefined) || (obj.IncidentID == 0)) {
        $('#btnViewEditIncidentDetail').attr("disabled", true);
    } else {
        $('#btnViewEditIncidentDetail').removeAttr("disabled");
    }
}

function SetShow_06(isShow) {
    if (isShow) {
        $('#divResultRegister').show();
    } else {
        $('#divResultRegister').hide();
    }
}