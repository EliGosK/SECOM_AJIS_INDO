/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/// <reference path="../Base/object/ajax_method.js" />


var mygrid_Subcontract;
var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;

var CMS130_CMS220ShowMode;

/* ------ Main ------ */
$(document).ready(function () {


    // TODO : Comment for next phase---------------------------------------
    //$("#btnViewIncidentList").attr("disabled", true);
    //$("#btnViewARList").attr("disabled", true);
    //$("#btnRegisterIncident").attr("disabled", true);
    //$("#btnRegisterAR").attr("disabled", true);
    ////$("#btnViewContractDocumentInfo").attr("disabled", true);
    //----------------------------------------------------------------------



    if ($.find("#mygrid_Subcontract").length > 0) {
        var parameter = { "strContractCode": CMS130Data.strContractCode, "strOCC": CMS130Data.strOCC };

        mygrid_Subcontract = $("#mygrid_Subcontract").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS130_GetSubContractList", parameter, "dtTbt_RentalInstSubContractorListForView", false);
    }

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

    /* ========= add hyper link =========== */
    initialHyperLink();

    /* ========= add event header button =========== */
    initialHeaderButton();

    /* ========= add event rigth button =========== */
    initialRightButton();


    // Set null value to "-"
    $("#divAll").SetEmptyViewData();

    if (CMS130Data.txtAttachImportanceFlag == true) {
        $("#ChkAttachImportanceFlag").attr("checked", true);
    }
});


function initialHyperLink() {

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

            //**
            CMS130_CMS220ShowMode = "Contract";

            $("#dlgCMS130").OpenCMS220Dialog("CMS130");
        });

    }
    else {
        $("#lnkCustomerCodeC").parent().html("<div class='usr-label label-view'>-</div>");
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


            //**
            CMS130_CMS220ShowMode = "Customer";

            $("#dlgCMS130").OpenCMS220Dialog("CMS130");
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

            //**
            CMS130_CMS220ShowMode = "Site";

            $("#dlgCMS130").OpenCMS220Dialog("CMS130");
        });

    }
    else {
        $("#lnkSiteCode").parent().html("<div class='usr-label label-view'>-</div>");
    }




    /* === lnkQuotationCode --- to QUS012 (pop-up) === */
    if ($("#lnkQuotationCode").text().length > 0) {

        $("#lnkQuotationCode").initial_link(function (val) {
            var obj = QUS012Object();
            var parameter = {
                QuotationTargetCode: obj.QuotationTargetCode,
                Alphabet: obj.Alphabet,
                HideQuotationTarget: true
            };

            $("#dlgCMS130").OpenQUS012Dialog("CMS130");
        });

    }
    else {
        $("#lnkQuotationCode").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* === lnkInstallation_slip_no --- to CMS180 (new window) === */
    if ($("#lnkInstallation_slip_no").text().length > 0) {

        $("#lnkInstallation_slip_no").click(function () {
            var obj = { "InstallationSlipNo": CMS130Data.InstallationSlipNo };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, true);
        });

    }
    else {
        $("#lnkInstallation_slip_no").parent().html("<div class='usr-label label-view'>-</div>");
    }


}

function initialHeaderButton() {

    var myurl = "";
    $("#btnContractBasic").click(function () {

        // go to CMS120

        //myurl = generate_url("/Common/CMS120") + "?strContractCode=" + CMS130Data.strContractCode;
        //alert(myurl);
        //window.location.href = myurl;


        var obj = { "strContractCode": CMS130Data.strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, false);

    });

//    $("#btnSecurityDetail").click(function () {
//        // go to CMS140

//        //myurl = generate_url("/Common/CMS140") + "?strContractCode=" + CMS130Data.strContractCode + "&strOCC=" + CMS130Data.strOCC;
//        //alert(myurl);
//        //window.location.href = myurl;

//        var obj = { "strContractCode": CMS130Data.strContractCode, "strOCC": CMS130Data.strOCC };
//        ajax_method.CallScreenControllerWithAuthority("/Common/CMS140", obj, false);

//    });

    $("#btnHistoryDigest").click(function () {
        // go to CMS150

        //myurl = generate_url("/Common/CMS150") + "?ContractCode=" + CMS130Data.strContractCode + "&ServiceTypeCode=" + CMS130Data.ServiceTypeCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj = { "ContractCode": CMS130Data.strContractCode, "ServiceTypeCode": CMS130Data.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS150", obj, false);


    });

    $("#btnContractBillingTransfer").click(function () {
        // go to CMS200

        //myurl = generate_url("/Common/CMS200") + "?strContractCode=" + CMS130Data.strContractCode + "&strServiceTypeCode=" + CMS130Data.ServiceTypeCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj = { "strContractCode": CMS130Data.strContractCode, "strServiceTypeCode": CMS130Data.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS200", obj, false);


    });

    //----------------------For phase 2----------------------------

    $("#btnHeader_Installation").click(function () {
        // go to CMS180

        var obj = { "ContractCode": CMS130Data.strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, false);

    });

    $("#btnHeader_BillingBasic").click(function () {
        // go to CMS420

        var obj = { "ContractCode": CMS130Data.strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj, false);

    });

    //----------------------For phase 2----------------------------



}

function initialRightButton() {

    $("#btnViewIncidentList").click(function () {
        // New windos CTS320 : Incident list
       
        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: CMS130Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS320", obj, true);

    });

    $("#btnViewARList").click(function () {
        // New window CTS370 : AR list

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: CMS130Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS370", obj, true);
       
    });

    $("#btnRegisterIncident").click(function () {
        // New window CTS300 : Register new incident

        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: CMS130Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS300", obj, true);
    });

    $("#btnRegisterAR").click(function () {
        // New window CTS350 : Register new AR

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: CMS130Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS350", obj, true);
    });

    $("#btnViewContractDocumentInfo").click(function () {
        // Pop-up CMS131
        var obj = CMS131Object();
        var parameter = {
            ContractCode: obj.ContractCode,
            OCC: obj.OCC
        };

        $("#dlgCMS130").OpenCMS131Dialog(parameter);
    });


}


function CMS220Object() {
    return {
        "ContractCode": CMS130Data.strContractCode,
        "OCC": CMS130Data.strOCC,
        "ContractTargetCode": CMS130Data.ContractTargetCode,
        "RealCustomerCode": CMS130Data.RealCustomerCode,
        "SiteCode": CMS130Data.SiteCode,
        "Mode": CMS130_CMS220ShowMode

    };
}

function CMS131Object() {
    return {
        "ContractCode": CMS130Data.strContractCode,
        "OCC": CMS130Data.strOCC
    };
}


function QUS012Object() {
    return {
        "QuotationTargetCode": CMS130Data.QuotationTargetCode,
        "Alphabet": CMS130Data.QuotationAlphabet,
        "HideQuotationTarget": true
    };
}

