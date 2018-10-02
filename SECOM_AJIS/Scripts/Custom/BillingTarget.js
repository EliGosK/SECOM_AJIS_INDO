/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/AutoComplete.js"/>
/// <reference path="../Base/RadioButton.js" />

/*--- Initialize ---*/
$(document).ready(function () {
    //alert($("#divSpecifyInfo input[name=BillGroup]").length);
    radio_mapping_control("#divSpecifyInfo input[name=BillGroup]",swap_billgroup_control);
     
});

function swap_billgroup_control(id) {
    var txtBillTargetCode = $("#divSpecifyInfo input[id=txtBillTargetCode]");
    var txtBillClientCode = $("#divSpecifyInfo input[id=txtBillClientCode]");
    if (id == "rdoBillTargetCode") {
        txtBillTargetCode.removeAttr("readonly");
        txtBillClientCode.attr("readonly", "readonly");

        txtBillClientCode.val("");
    }
    else if (id == "rdoBillClientCode") {
        txtBillTargetCode.attr("readonly", "readonly");
        txtBillClientCode.removeAttr("readonly");

        txtBillTargetCode.val("");
    }
}