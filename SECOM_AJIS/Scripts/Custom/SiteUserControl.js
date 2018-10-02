/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>

/*--- Initialize ---*/
$(document).ready(function () {
    $("#divSpecifySite button[id=btnRetrieveSiteData]").click(btnRetrieveSiteData_click);
    $("#divSpecifySite button[id=btnSearchSite]").click(btnSearchSite_click);
});

/*--- Events ---*/
function btnRetrieveSiteData_click() {
    alert("btnRetrieveSiteData > click");
}

function btnSearchSite_click() {
    OpenCMS260Dialog("#dlgCMS260", do_something);
}

/*--- Methods ---*/
function do_something(data) {
    alert(data);
}