/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/// <reference path="../../Scripts/Common/Dialog.js"/>

var mygrid_CTGroup;
var mygrid_PUGroup;
var mygrid_RCGroup;
var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;

$(document).ready(function () {


    // Get parameter data
    var objParam = "";
    if (typeof (CMS220Object) == "function") {
        objParam = CMS220Object();
    }
    //alert("4444");
    var parameter = "";


    if (objParam.Mode == "Contract") {
        parameter = { "strCustCode": objParam.ContractTargetCode };
    }
    else if (objParam.Mode == "Purchaser") {
        parameter = { "strCustCode": objParam.PurchaserCustCode };
    }
    else if (objParam.Mode == "Customer") {
        parameter = { "strCustCode": objParam.RealCustomerCode };
    }


    //alert(parameter.strCustCode);


    /* Load customer data group */
    if ($.find("#mygrid_CTGroup").length > 0) {

        mygrid_CTGroup = $("#mygrid_CTGroup").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS220_GetCustomerGroup", parameter, "dtCustomerGroupForView", false);
    }
    if ($.find("#mygrid_PUGroup").length > 0) {

        mygrid_PUGroup = $("#mygrid_PUGroup").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS220_GetCustomerGroup", parameter, "dtCustomerGroupForView", false);

    }
    if ($.find("#mygrid_RCGroup").length > 0) {

        mygrid_RCGroup = $("#mygrid_RCGroup").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS220_GetCustomerGroup", parameter, "dtCustomerGroupForView", false);

    }


    // Set null value to "-"
    $("#divAll_CMS220").SetEmptyViewData();

    if (txtAttachImportanceFlag == true) {
        $("#ChkAttachImportanceFlag").attr("checked", true);
    }
});

/* --- Methods --- */
function CMS220Initial() {
    ChangeDialogButtonText(["Close"], [$('#btnClose_CMS220').val()]);
}

