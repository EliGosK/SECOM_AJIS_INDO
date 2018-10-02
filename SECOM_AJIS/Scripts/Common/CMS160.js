/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>


var mygrid_subcontractor_Install;
var mygrid_subcontractor_ChangeInstall;
var mygrid_instrumentDeatail;
var mygrid_saleContractDetail;

var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;

var CMS160_CMS220ShowMode;

/* ------ Main ------ */
$(document).ready(function () {

    // TODO : Comment for next phase-------------------------------------------
    //    $("#btnViewIncidentList").attr("disabled", true);
    //    $("#btnViewARList").attr("disabled", true);
    //    $("#btnRegisterIncident").attr("disabled", true);
    //    $("#btnRegisterAR").attr("disabled", true);
    //    $("#btnView_deposit_fee_information").attr("disabled", true);
    //--------------------------------------------------------------------------



    initialPage();



    /* ----- Get data to grid ----- */
    if ($.find("#mygrid_subcontractor_Install").length > 0) {

        var parameter = { "strContractCode": mycond_cms160.ContractCode, "strOCC": mycond_cms160.OCC };
        //$("#mygrid_subcontractor_Install").LoadDataToGrid(mygrid_subcontractor_Install, pageRow, true, "/Common/CMS160_GetSubContractList", parameter, "dtTbt_SaleInstSubcontractorListForView", false);

        mygrid_subcontractor_Install = $("#mygrid_subcontractor_Install").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS160_GetSubContractList", parameter, "dtTbt_SaleInstSubcontractorListForView", false);
    }

    if ($.find("#mygrid_subcontractor_ChangeInstall").length > 0) {

        var parameter = { "strContractCode": mycond_cms160.ContractCode, "strOCC": mycond_cms160.OCC };
        //$("#mygrid_subcontractor_ChangeInstall").LoadDataToGrid(mygrid_subcontractor_ChangeInstall, pageRow, true, "/Common/CMS160_GetSubContractList", parameter, "dtTbt_SaleInstSubcontractorListForView", false);

        mygrid_subcontractor_ChangeInstall = $("#mygrid_subcontractor_ChangeInstall").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS160_GetSubContractList", parameter, "dtTbt_SaleInstSubcontractorListForView", false);
    }

    if ($.find("#mygrid_instrumentDeatail").length > 0) {

        var parameter = { "strContractCode": mycond_cms160.ContractCode, "strOCC": mycond_cms160.OCC };
        //$("#mygrid_instrumentDeatail").LoadDataToGrid(mygrid_instrumentDeatail, pageRow, true, "/Common/CMS160_GetInstrumentDetail", parameter, "dtSaleInstruDetailListForView", false);

        mygrid_instrumentDeatail = $("#mygrid_instrumentDeatail").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS160_GetInstrumentDetail", parameter, "dtSaleInstruDetailListForView", false);
    }

    if ($.find("#mygrid_saleContractDetail").length > 0) {

        var parameter = { "strContractCode": mycond_cms160.ContractCode, "strOCC": mycond_cms160.OCC };
        //$("#mygrid_saleContractDetail").LoadDataToGrid(mygrid_saleContractDetail, pageRow, false, "/Common/CMS160_GetSaleContractDetail", parameter, "doCMS160_SaleContractDetail", false);

        mygrid_saleContractDetail = $("#mygrid_saleContractDetail").LoadDataToGridWithInitial(0, false, false, "/Common/CMS160_GetSaleContractDetail", parameter, "doCMS160_SaleContractDetail", false);

        /* ===== binding event when finish load data ===== */

        BindOnLoadedEvent(mygrid_saleContractDetail, function (gen_ctrl) {
            var colTitleIndex = mygrid_saleContractDetail.getColIndexById('Title');
            var colDiscountNameIndex = mygrid_saleContractDetail.getColIndexById('DiscountName');

            var lblList = [];
            var lblProductPrice = $("#lblProductPrice").val() + "<br/>(1)";
            var lblInstallationFee = $("#lblInstallationFee").val() + "<br/>(2)";
            var lblSalePrice = $("#lblSalePrice").val() + "<br/>(1)+(2)";
            lblList.push(lblProductPrice);
            lblList.push(lblInstallationFee);
            lblList.push(lblSalePrice);

            //if (mygrid_saleContractDetail.getRowsNum() >= 3) {
            //    mygrid_saleContractDetail.cells2(0, colTitleIndex).setValue(lblProductPrice);
            //    mygrid_saleContractDetail.cells2(1, colTitleIndex).setValue(lblInstallationFee);
            //    mygrid_saleContractDetail.cells2(2, colTitleIndex).setValue(lblSalePrice);
            //}

            /* --- enable column span --- */
            //mygrid_saleContractDetail.enableColSpan(true);
            for (var i = 0; i < mygrid_saleContractDetail.getRowsNum(); i++) {

                //if (mygrid_saleContractDetail.getRowsNum() <= 3) {
                //    mygrid_saleContractDetail.cells2(i, colTitleIndex).setValue(lblList[i]);
                //}

                var rowId = mygrid_saleContractDetail.getRowId(i);


            }

            mygrid_saleContractDetail.setSizes();

        });



    }

    /* --- More --- */
    $("#lnkMore").click(function () {
        $("#MoreSalesman").show();
        $("#Less").show();

        $("#More").hide();

    });

    /* --- Less --- */
    $("#lnkLess").click(function () {
        $("#MoreSalesman").hide();
        $("#Less").hide();

        $("#More").show();
    });

    /* ========= add event header button =========== */
    initialHeaderButton();

    /* ========= add hyper link =========== */
    initialHyperLink();

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

    if (mycond_cms160.txtAttachImportanceFlag == true) {
        $("#ChkAttachImportanceFlag").attr("checked", true);
    }
});

function initialPage() {
    $("#MoreSalesman").hide();
    $("#Less").hide();
}

function initialHeaderButton() {

    // HistoryDigest
    $("#btnHistoryDigest").click(function () {
        // go to CMS150

        var obj = { "ContractCode": mycond_cms160.ContractCode, "ServiceTypeCode": mycond_cms160.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS150", obj, false);

    });

    // ContractBillingTransfer
    $("#btnContractBillingTransfer").click(function () {
        // go to CMS200

        var obj = { "strContractCode": mycond_cms160.ContractCode, "strServiceTypeCode": mycond_cms160.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS200", obj, false);

    });

    //------------For phase 2--------------------------------

    $("#btnHeader_Installation").click(function () {
        // go to CMS180
        var obj_myParam = { "ContractCode": mycond_cms160.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj_myParam, false);

    });

    $("#btnHeader_BillingBasic").click(function () {
        // go to CMS420
        var obj_myParam = { "ContractCode": mycond_cms160.ContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj_myParam, false);

    });

    //----------------------------------------------------

}

function initialHyperLink() {

    var url;


    /* === lnkCustomerCodeC --- to cms220 (pop-up) === */
    if ($("#lnkCustomerCodeC").text().length > 0) {

        $("#lnkCustomerCodeC").show();

        $("#lnkCustomerCodeC").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                PurchaserCustCode: obj.PurchaserCustCode
            };

            CMS160_CMS220ShowMode = "Purchaser";

            $("#dlgCMS160").OpenCMS220Dialog("CMS160");
        });



    }
    else {
        $("#lnkCustomerCodeC").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* === lnkCustomerCodeR --- to cms220 (pop-up) === */

    if ($("#lnkCustomerCodeR").text().length > 0) {

        $("#lnkCustomerCodeR").show();

        $("#lnkCustomerCodeR").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                RealCustomerCode: obj.RealCustomerCode
            };

            // **
            CMS160_CMS220ShowMode = "Customer";

            $("#dlgCMS160").OpenCMS220Dialog("CMS160");
        });

    }
    else {

        $("#lnkCustomerCodeR").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* === lnkSiteCode --- to cms220 (pop-up) === */

    if ($("#lnkSiteCode").text().length > 0) {

        $("#lnkSiteCode").show();

        $("#lnkSiteCode").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                SiteCode: obj.SiteCode
            };

            //**
            CMS160_CMS220ShowMode = "Site";

            $("#dlgCMS160").OpenCMS220Dialog("CMS160");
        });

    }
    else {

        $("#lnkSiteCode").parent().html("<div class='usr-label label-view'>-</div>");
    }




    /* ============== lnkQuotation_code : Pop-up QUS011  ============== */

    if ($("#lnkQuotation_code").text().length > 0) {

        $("#lnkQuotation_code").show();

        $("#lnkQuotation_code").click(function () {
            $("#dlgCMS160").OpenQUS011Dialog();
        });
    }
    else {

        $("#lnkQuotation_code").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* ============== lnkDistributed_origin_code : New window CMS160 ============== */

    if ($("#lnkDistributed_origin_code").text().length > 0) {

        $("#lnkDistributed_origin_code").show();

        $("#lnkDistributed_origin_code").click(function () {

            //var myUrl = generate_url("/Common/CMS160") + "?strContractCode=" + mycond_cms160.DistributedOriginCode + "&strOCC=" + mycond_cms160.OCC;
            //alert(myUrl);
            //window.open(myUrl, 'CMS160', 'width=1024,height=1024,menubar=yes,status=yes');

            var obj = { "strContractCode": mycond_cms160.DistributedOriginCode, "strOCC": mycond_cms160.OCC };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, true);

        });

    }
    else {

        $("#lnkDistributed_origin_code").parent().html("<div class='usr-label label-view'>-</div>");
    }


    /* ============== lnkOnline_contract_code : New window CMS120  ============== */

    if ($("#lnkOnline_contract_code").text().length > 0) {

        $("#lnkOnline_contract_code").show();

        $("#lnkOnline_contract_code").click(function () {

            //var myUrl = generate_url("/Common/CMS120") + "?strContractCode=" + mycond_cms160.OnlineContractCode;
            //alert(myUrl);
            //window.open(myUrl, 'CMS120', 'width=1024,height=1024,menubar=yes,status=yes');

            var obj = { "strContractCode": mycond_cms160.OnlineContractCode };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, true);

        });

    }
    else {

        $("#lnkOnline_contract_code").parent().html("<div class='usr-label label-view'>-</div>");
    }

    //Add by Jutarat A. on 17082012
    /* ============== lnkConnectionContractCode : New window CMS120  ============== */

    if ($("#lnkConnectionContractCode").text().length > 0) {

        $("#lnkConnectionContractCode").show();

        $("#lnkConnectionContractCode").click(function () {
            var obj = { "strContractCode": mycond_cms160.ConnectionContractCode };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, true);

        });

    }
    else {

        $("#lnkConnectionContractCode").parent().html("<div class='usr-label label-view'>-</div>");
    }
    //End Add

    /* ============== lnkInstallation_slip_no_Install : New window CMS180  ============== */

    if ($("#lnkInstallation_slip_no_Install").text().length > 0) {

        $("#lnkInstallation_slip_no_Install").show();

        $("#lnkInstallation_slip_no_Install").click(function () {
            var obj = { "InstallationSlipNo": mycond_cms160.InstallationSlipNo };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, true);
        });

    }
    else {

        $("#lnkInstallation_slip_no_Install").parent().html("<div class='usr-label label-view'>-</div>");
    }



    /* ============== lnkInstallation_slip_no_Change : New window CMS180  ============== */

    if ($("#lnkInstallation_slip_no_Change").text().length > 0) {

        $("#lnkInstallation_slip_no_Change").show();

        $("#lnkInstallation_slip_no_Change").click(function () {

            var obj = { "InstallationSlipNo": mycond_cms160.InstallationSlipNo };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, false);

        });


    }
    else {
        $("#lnkInstallation_slip_no_Change").parent().html("<div class='usr-label label-view'>-</div>");
    }






}

function initialRightButton() {

    $("#btnView_contract_signer_history").click(function () {
        // Popup CMS300 : View customer history

        $("#dlgCMS160").OpenCMS300Dialog();


    });

    $("#btnViewIncidentList").click(function () {
        // New windos CTS320 : Incident list

        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: mycond_cms160.ContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS320", obj, true);
    });

    $("#btnViewARList").click(function () {
        // New window CTS370 : AR list

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: mycond_cms160.ContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS370", obj, true);
    });

    $("#btnRegisterIncident").click(function () {
        // New window CTS300 : Register new incident

        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: mycond_cms160.ContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS300", obj, true);
    });

    $("#btnRegisterAR").click(function () {
        // New window CTS350 : Register new AR

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: mycond_cms160.ContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS350", obj, true);
    });

    //$("#btnView_deposit_fee_information").click(function () {
    //    //Next phase
    //    alert("Perform in next phase");
    //});
}

function CMS220Object() {
    return {
        "ContractCode": mycond_cms160.ContractCode,
        "OCC": mycond_cms160.OCC,
        "PurchaserCustCode": mycond_cms160.PurchaserCustCode,
        "RealCustomerCode": mycond_cms160.RealCustomerCode,
        "SiteCode": mycond_cms160.SiteCode,
        "Mode": CMS160_CMS220ShowMode

    };
}

function QUS011Object() {
    return {
        QuotationTargetCode: mycond_cms160.QuotationTargetCode,
        Alphabet: mycond_cms160.Alphabet,
        HideQuotationTarget: true
    };
}


function CMS300Object() {
    return {
        "ContractCode": mycond_cms160.ContractCode,
        "ServiceTypeCode": null,
        "OCC": null,
        "CSCustCode": null,
        "RCCustCode": null,
        "SiteCode": null
    };
}
