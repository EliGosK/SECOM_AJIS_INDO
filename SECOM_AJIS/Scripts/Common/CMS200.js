/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

var mygrid;
//var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;

var CMS200_CMS220ShowMode;

/* ------ Main ------ */
$(document).ready(function () {



    /* --- Get parameter data --- */

    var parameter = "";
    if (mycond_cms200.ContractCode != null && mycond_cms200.ContractCode != "") {
        parameter = {
            "ContractCode": mycond_cms200.ContractCode,
            "OCC": mycond_cms200.OCC
        };
    }

    if ($.find("#mygrid_ContractBilling").length > 0) {
        //$("#mygrid_ContractBilling").LoadDataToGrid(mygrid, pageRow, true, "/Common/CMS200_GetBillingTempList", parameter, "View_dtTbt_BillingTempListForView", false);
        mygrid = $("#mygrid_ContractBilling").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS200_GetBillingTempList", parameter, "View_dtTbt_BillingTempListForView", false);
    }

    /* ========= add hyper link =========== */
    initialHyperLink();

    /* ========= add event header button =========== */
    initialHeaderButton();

    // Set null value to "-"
    $("#divAll").SetEmptyViewData();

    if (mycond_cms200.txtRentalAttachImportanceFlag == true) {
        $("#ChkRentalAttachImportanceFlag").attr("checked", true);
    }
    if (mycond_cms200.txtSaleAttachImportanceFlag == true) {
        $("#ChkSaleAttachImportanceFlag").attr("checked", true);
    }
});


function initialHyperLink() {

    var url;

    /* ========= add hyper link =========== */
    //// ex. $(selector).attr("href","url");

    /* === lnkCustomerCodeC --- to cms220 (pop-up) === */
    if ($("#lnkCustomerCodeC").text().length > 0) {

        $("#lnkCustomerCodeC").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                ContractTargetCode: obj.ContractTargetCode
            };

            // **
            CMS200_CMS220ShowMode = "Contract";

            $("#dlgCMS200").OpenCMS220Dialog("CMS200");
        });

    }
    else {
        $("#lnkCustomerCodeC").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* === lnkPurchaserC --- to cms220 (pop-up) === */
    if ($("#lnkPurchaserC").text().length > 0) {

        $("#lnkPurchaserC").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                PurchaserCustCode: obj.PurchaserCustCode
            };

            // **
            CMS200_CMS220ShowMode = "Purchaser";

            $("#dlgCMS200").OpenCMS220Dialog("CMS200");
        });

    }
    else {
        $("#lnkPurchaserC").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* === lnkCustomerCodeR --- to cms220 (pop-up) === */
    if ($("#lnkCustomerCodeR").text().length > 0) {

        $("#lnkCustomerCodeR").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                RealCustomerCode: obj.RealCustomerCode
            };


            // **
            CMS200_CMS220ShowMode = "Customer";

            $("#dlgCMS200").OpenCMS220Dialog("CMS200");
        });

    }
    else {
        $("#lnkCustomerCodeR").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* === lnkSiteCode --- to cms220 (pop-up) === */
    if ($("#lnkSiteCode").text().length > 0) {

        $("#lnkSiteCode").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                SiteCode: obj.SiteCode
            };

            // **
            CMS200_CMS220ShowMode = "Site";

            $("#dlgCMS200").OpenCMS220Dialog("CMS200");
        });

    }
    else {
        $("#lnkSiteCode").parent().html("<div class='usr-label label-view'>-</div>");
    }


}

function initialHeaderButton() {

    var myurl = "";
    $("#btnContractBasic").click(function () {
        //go to CMS120

        //myurl = generate_url("/Common/CMS120") + "?strContractCode=" + mycond_cms200.ContractCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj = { "strContractCode": mycond_cms200.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, false);

    });

    $("#btnHistoryDigest").click(function () {
        // go to CMS150

        //myurl = generate_url("/Common/CMS150") + "?ContractCode=" + mycond_cms200.ContractCode + "&ServiceTypeCode=" + mycond_cms200.ServiceTypeCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj = { "ContractCode": mycond_cms200.ContractCode, "ServiceTypeCode": mycond_cms200.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS150", obj, false);

    });

    $("#btnSalesContractBasic").click(function () {
        //go to CMS160

        //myurl = generate_url("/Common/CMS160") + "?strContractCode=" + mycond_cms200.ContractCode + "&strOCC=" + mycond_cms200.OCC;
        //alert(myurl);
        //window.location.href = myurl;

        var obj = { "strContractCode": mycond_cms200.ContractCode, "strOCC": mycond_cms200.OCC };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, false);

    });

    //------------For phase 2--------------------------------

    $("#btnHeader_Installation").click(function () {
        // go to CMS180
        var obj_myParam = { "ContractCode": mycond_cms200.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj_myParam, false);

    });

    $("#btnHeader_BillingBasic").click(function () {
        // go to CMS420
        var obj_myParam = { "ContractCode": mycond_cms200.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj_myParam, false);

    });

    //----------------------------------------------------
}

function CMS220Object() {
    return {
        "ContractCode": mycond_cms200.ContractCode,
        "OCC": mycond_cms200.OCC,
        "ContractTargetCode": mycond_cms200.ContractTargetCode,
        "PurchaserCustCode": mycond_cms200.PurchaserCustCode,
        "RealCustomerCode": mycond_cms200.RealCustomerCode,
        "SiteCode": mycond_cms200.SiteCode,
        "Mode": CMS200_CMS220ShowMode

    };
}
