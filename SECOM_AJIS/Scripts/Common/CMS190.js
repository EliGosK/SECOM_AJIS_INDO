/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

var CMS190_CMS220ShowMode;

/* ------ Main ------ */
$(document).ready(function () {

    // TODO : Comment for next phase ----------------
    //    $("#btnViewIncidentList").attr("disabled", true);
    //    $("#btnViewARList").attr("disabled", true);
    //    $("#btnRegisterIncident").attr("disabled", true);
    //    $("#btnRegisterAR").attr("disabled", true);
    //-----------------------------------------------

    /* ========= add hyper link =========== */
    initialHyperLink();

    /* ========= add event header button =========== */
    initialHeaderButton();

    /* ========= add event rigth button =========== */
    initialRightButton();

    /* ========= ContractSameSiteList : New window CMS120 or CMS160 =========  */
    $("#contractSameSiteList a").each(function () {

        $(this).click(function () {

            var str = $(this).children("input").val();
            var substr = str.split('--');

            var url = "";
            var obj = "";

            if (substr.length == 2) {

                url = "/Common/" + substr[0];
                obj = { "strContractCode": substr[1] };
            }

            ajax_method.CallScreenControllerWithAuthority(url, obj, true);
            return false;
        });

    });


    // Set null value to "-"
    $("#divAll").SetEmptyViewData();

    if (CMS190Data.txtRentalAttachImportanceFlag == true) {
        $("#ChkRentalAttachImportanceFlag").attr("checked", true);
    }
    if (CMS190Data.txtSaleAttachImportanceFlag == true) {
        $("#ChkSaleAttachImportanceFlag").attr("checked", true);
    }
});


function initialHyperLink() {

    /* ========= add hyper link =========== */

    /* === lnkCustomerCodeC --- to cms220 (pop-up) === */
    if ($("#lnkCustomerCodeC").text().length > 0) {

        $("#lnkCustomerCodeC").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                ContractTargetCode: obj.ContractTargetCode,
                PurchaserCustCode: obj.PurchaserCustCode
            };

            // **
            CMS190_CMS220ShowMode = "Contract";

            $("#dlgCMS190").OpenCMS220Dialog("CMS190");
        });

    }
    else {
        $("#lnkCustomerCodeC").parent().html("<div class='usr-label label-view'>-</div>");
    }

    /* === lnkCustomerCodeC_Purchaser --- to cms220 (pop-up) === */
    if ($("#lnkCustomerCodeC_Purchaser").text().length > 0) {

        $("#lnkCustomerCodeC_Purchaser").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                ContractTargetCode: obj.ContractTargetCode,
                PurchaserCustCode: obj.PurchaserCustCode
            };

            // **
            CMS190_CMS220ShowMode = "Purchaser";

            $("#dlgCMS190").OpenCMS220Dialog("CMS190");
        });

    }
    else {
        $("#lnkCustomerCodeC_Purchaser").parent().html("<div class='usr-label label-view'>-</div>");
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
            CMS190_CMS220ShowMode = "Customer";

            $("#dlgCMS190").OpenCMS220Dialog("CMS190");
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
            CMS190_CMS220ShowMode = "Site";

            $("#dlgCMS190").OpenCMS220Dialog("CMS190");
        });

    }
    else {
        $("#lnkSiteCode").parent().html("<div class='usr-label label-view'>-</div>");
    }




    /* === lnkOld_contract_code --- to cms120 === */
    if ($("#lnkOld_contract_code").text().length > 0) {

        $("#lnkOld_contract_code").click(function () {
            //var myUrl = generate_url("/Common/CMS120") + "?strContractCode=" + CMS190Data.OldContractCode;
            //alert(myUrl);
            //window.open(myUrl, 'CMS120', 'width=1024,height=1024,menubar=yes,status=yes');

            var obj = { "strContractCode": CMS190Data.OldContractCode };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, true);

        });

    }
    else {
        $("#lnkOld_contract_code").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* === lnkMaintenance_contract_code --- to cms120 === */
    if ($("#lnkMaintenance_contract_code").text().length > 0) {

        $("#lnkMaintenance_contract_code").click(function () {
            //var myUrl = generate_url("/Common/CMS120") + "?strContractCode=" + CMS190Data.MaintenanceContractCode;
            //alert(myUrl);
            //window.open(myUrl, 'CMS120', 'width=1024,height=1024,menubar=yes,status=yes');

            var obj = { "strContractCode": CMS190Data.MaintenanceContractCode };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, true);

        });

    }
    else {
        $("#lnkMaintenance_contract_code").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* === lnkOnline_contract_code --- to cms120 === */
    if ($("#lnkOnline_contract_code").text().length > 0) {

        $("#lnkOnline_contract_code").click(function () {
            //var myUrl = generate_url("/Common/CMS120") + "?strContractCode=" + CMS190Data.OnlineContractCode;
            //alert(myUrl);
            //window.open(myUrl, 'CMS120', 'width=1024,height=1024,menubar=yes,status=yes');

            var obj = { "strContractCode": CMS190Data.OnlineContractCode };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, true);

        });

    }
    else {
        $("#lnkOnline_contract_code").parent().html("<div class='usr-label label-view'>-</div>");
    }


}

function initialHeaderButton() {

    var myurl = "";

    $("#btnContractBasic").click(function () {

        // go to CMS120

        //myurl = generate_url("/Common/CMS120") + "?strContractCode=" + CMS190Data.ContractCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj = { "strContractCode": CMS190Data.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, false);

    });

//    $("#btnSecurityBasic").click(function () {

//        // go to CMS130

//        //myurl = generate_url("/Common/CMS130") + "?strContractCode=" + CMS190Data.ContractCode + "&strOCC=" + CMS190Data.OCC;
//        //alert(myurl);
//        //window.location.href = myurl;


//        var obj = { "strContractCode": CMS190Data.ContractCode, "strOCC": CMS190Data.OCC };
//        ajax_method.CallScreenControllerWithAuthority("/Common/CMS130", obj, false);

//    });


//    $("#btnSecurityDetail").click(function () {
//        // go to CMS140

//        //myurl = generate_url("/Common/CMS140") + "?strContractCode=" + CMS190Data.ContractCode + "&strOCC=" + CMS190Data.OCC;
//        //alert(myurl);
//        //window.location.href = myurl;

//        var obj = { "strContractCode": CMS190Data.ContractCode, "strOCC": CMS190Data.OCC };
//        ajax_method.CallScreenControllerWithAuthority("/Common/CMS140", obj, false);

//    });

    $("#btnHistoryDigest").click(function () {
        //alert("btnHistoryDigest : go CMS150");

        //myurl = generate_url("/Common/CMS150") + "?ContractCode=" + CMS190Data.ContractCode + "&ServiceTypeCode=" + CMS190Data.ServiceTypeCode;
        //alert(myurl);
        //window.location.href = myurl;


        var obj = { "ContractCode": CMS190Data.ContractCode, "ServiceTypeCode": CMS190Data.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS150", obj, false);


    });

    $("#btnSalesContractBasic").click(function () {
        //alert("btnSalesContractBasic : go CMS160");

        //myurl = generate_url("/Common/CMS160") + "?strContractCode=" + CMS190Data.ContractCode + "&strOCC=" + CMS190Data.OCC;
        //alert(myurl);
        //window.location.href = myurl;


        var obj = { "strContractCode": CMS190Data.ContractCode, "strOCC": CMS190Data.OCC };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, false);

    });

    $("#btnContractBillingTransfer").click(function () {
        // go to CMS200

        //myurl = generate_url("/Common/CMS200") + "?strContractCode=" + CMS190Data.ContractCode + "&strServiceTypeCode=" + CMS190Data.ServiceTypeCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj = { "strContractCode": CMS190Data.ContractCode, "strServiceTypeCode": CMS190Data.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS200", obj, false);

    });


    //---------------For phase 2-------------------------------

        $("#btnHeader_Installation").click(function () {
            // go to CMS180

            var obj = { "ContractCode": CMS190Data.ContractCode  };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, false);

        });

        $("#btnHeader_BillingBasic").click(function () {
            // go to CMS420

            var obj = { "ContractCode": CMS190Data.ContractCode };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj, false);

        });

    //---------------For phase 2 (end)-------------------------------


}

function initialRightButton() {

    $("#btnViewIncidentList").click(function () {
        // New windos CTS320 : Incident list

        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: CMS190Data.ContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS320", obj, true);
    });

    $("#btnViewARList").click(function () {
        // New window CTS370 : AR list

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: CMS190Data.ContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS370", obj, true);
    });

    $("#btnRegisterIncident").click(function () {
        // New window CTS300 : Register new incident

        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: CMS190Data.ContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS300", obj, true);
        
    });

    $("#btnRegisterAR").click(function () {
        // New window CTS350 : Register new AR

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: CMS190Data.ContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS350", obj, true);

    });




}

function CMS220Object() {
    return {
        "ContractCode": CMS190Data.ContractCode,
        "OCC": CMS190Data.OCC,
        "ContractTargetCode": CMS190Data.ContractTargetCode,
        "PurchaserCustCode": CMS190Data.PurchaserCustCode,
        "RealCustomerCode": CMS190Data.RealCustomerCode,
        "SiteCode": CMS190Data.SiteCode,
        "OldContractCode": CMS190Data.OldContractCode,
        "Mode": CMS190_CMS220ShowMode
    };

}