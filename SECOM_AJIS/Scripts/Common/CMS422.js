 /// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/// <reference path="../Base/object/ajax_method.js" />

$(document).ready(function () {

    IntialPage();

});

function IntialPage() {

    // Set null value to "-"  ***
    $("#divAll").SetEmptyViewData();
}
function CMS422Initial() {
    ChangeDialogButtonText(["Close"], [$('#btnClose_cms422').val()]);
}