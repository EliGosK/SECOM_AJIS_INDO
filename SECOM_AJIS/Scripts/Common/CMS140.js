/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

var mygrid_MaintenanceContract;
var mygrid_InstrumentDeatail;
var mygrid_SentryGuardDetail;
var mygrid_FacilityDetail;

var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;

var mycond_CMS210;

var CMS140_CMS220ShowMode;

/* ------ Main ------ */
$(document).ready(function () {

    // TODO : Comment for next phase ---------------
    //$("#btnViewIncidentList").attr("disabled", true);
    //$("#btnViewARList").attr("disabled", true);
    //$("#btnRegisterIncident").attr("disabled", true);
    //$("#btnRegisterAR").attr("disabled", true);
    //---------------------------------------------


    /* --- grid -----*/
    if ($.find("#mygrid_MaintenanceContract").length > 0) {

        var parameter = { "strContractCode": CMS140Data.strContractCode, "strOCC": CMS140Data.strOCC };
        //$("#mygrid_MaintenanceContract").LoadDataToGrid(mygrid_MaintenanceContract, pageRow, true, "/Common/CMS140_GetMaintenanceContractTargetList", parameter, "View_dtRelatedContract", false);

        mygrid_MaintenanceContract = $("#mygrid_MaintenanceContract").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS140_GetMaintenanceContractTargetList", parameter, "View_dtRelatedContract", false);


        /* ===== binding on row select===== */
        //mygrid_MaintenanceContract.attachEvent("onRowSelect", openDetail);

        SpecialGridControl(mygrid_MaintenanceContract, ["ButtonMaintenance", "ButtonContractBasic", "ButtonSaleBasic"]);

        /* ===== binding event when finish load data ===== */
        BindOnLoadedEvent(mygrid_MaintenanceContract, function (gen_ctrl) {
            var colInx_ButtonMaintenance = mygrid_MaintenanceContract.getColIndexById('ButtonMaintenance');
            var colInx_ButtonContractBasic = mygrid_MaintenanceContract.getColIndexById('ButtonContractBasic');
            var colInx_ButtonSaleBasic = mygrid_MaintenanceContract.getColIndexById('ButtonSaleBasic');

            for (var i = 0; i < mygrid_MaintenanceContract.getRowsNum(); i++) {

                var row_id = mygrid_MaintenanceContract.getRowId(i);
                var bEnableContractBasic = mygrid_MaintenanceContract.cells2(i, mygrid_MaintenanceContract.getColIndexById('EnableContractBasic')).getValue();
                var bEnableSaleBasic = mygrid_MaintenanceContract.cells2(i, mygrid_MaintenanceContract.getColIndexById('EnableSaleBasic')).getValue();

                if (gen_ctrl == true) {
                    // View maintenance check-up resule (alway enable)
                    GenerateDetailButton(mygrid_MaintenanceContract, "btnDetail1", row_id, "ButtonMaintenance", true);

                    // ButtonContractBasic
                    if (bEnableContractBasic == 1) {
                        GenerateDetailButton(mygrid_MaintenanceContract, "btnDetail2", row_id, "ButtonContractBasic", true);
                    }
                    else {
                        GenerateDetailButton(mygrid_MaintenanceContract, "btnDetail2", row_id, "ButtonContractBasic", false);
                    }

                    // ButtonSaleBasic
                    if (bEnableSaleBasic == 1) {
                        GenerateDetailButton(mygrid_MaintenanceContract, "btnDetail3", row_id, "ButtonSaleBasic", true);
                    }
                    else {
                        GenerateDetailButton(mygrid_MaintenanceContract, "btnDetail3", row_id, "ButtonSaleBasic", false);
                    }
                }

                // Bind button grid event
                BindGridButtonClickEvent("btnDetail1", row_id, openDetail_1);


                // ButtonContractBasic
                if (bEnableContractBasic == 1) {
                    // Bind button grid event
                    BindGridButtonClickEvent("btnDetail2", row_id, openDetail_2);
                }

                // ButtonSaleBasic
                if (bEnableSaleBasic == 1) {
                    // Bind button grid event
                    BindGridButtonClickEvent("btnDetail3", row_id, openDetail_3);
                }

            }

            mygrid_MaintenanceContract.setSizes();

        });

    }

    if ($.find("#mygrid_InstrumentDeatail").length > 0) {

        var parameter = { "strContractCode": CMS140Data.strContractCode, "strOCC": CMS140Data.strOCC };
        //$("#mygrid_InstrumentDeatail").LoadDataToGrid(mygrid_InstrumentDeatail, pageRow, true, "/Common/CMS140_GetInstrumentDetail", parameter, "dtTbt_RentalInstrumentDetailsListForView", false);

        mygrid_InstrumentDeatail = $("#mygrid_InstrumentDeatail").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS140_GetInstrumentDetail", parameter, "dtTbt_RentalInstrumentDetailsListForView", false);
    }

    if ($.find("#mygrid_SentryGuardDetail").length > 0) {

        var parameter = { "strContractCode": CMS140Data.strContractCode, "strOCC": CMS140Data.strOCC };
        //$("#mygrid_SentryGuardDetail").LoadDataToGrid(mygrid_SentryGuardDetail, pageRow, true, "/Common/CMS140_GetSentryGuardDetail", parameter, "dtTbt_RentalSentryGuardDetailsListForView", false);

        mygrid_SentryGuardDetail = $("#mygrid_SentryGuardDetail").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS140_GetSentryGuardDetail", parameter, "dtTbt_RentalSentryGuardDetailsListForView", false);
    }


    if ($.find("#mygrid_FacilityDetail").length > 0) {

        var parameter = { "strContractCode": CMS140Data.strContractCode, "strOCC": CMS140Data.strOCC };
        mygrid_FacilityDetail = $("#mygrid_FacilityDetail").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS140_GetFacilityDetail", parameter, "dtTbt_RentalInstrumentDetailsListForView", false);
    }



    /* ========= add hyper link =========== */
    initialHyperLink();

    /* ========= add event header button =========== */
    initialHeaderButton();

    /* ========= add event rigth button =========== */
    initialRightButton();


    // Set null value to "-"
    $("#divAll").SetEmptyViewData();


    if (CMS140Data.txtAttachImportanceFlag == true) {
        $("#ChkAttachImportanceFlag").attr("checked", true);
    }
});


/* -----------  event ----------- */

function openDetail_1(row_id) {

    mygrid_MaintenanceContract.selectRow(mygrid_MaintenanceContract.getRowIndex(row_id));

    var rowInx = mygrid_MaintenanceContract.getRowIndex(row_id);


    // Get column index
    var colMATargetContractCode = mygrid_MaintenanceContract.getColIndexById('RelatedContractCode');
    var colProductCode = mygrid_MaintenanceContract.getColIndexById('ProductCode');
    var colServiceTypeCode = mygrid_MaintenanceContract.getColIndexById('ServiceTypeCode');

    var obj =
        {
            ContractCode: CMS140Data.strContractCode,
            MATargetContractCode: mygrid_MaintenanceContract.cells2(rowInx, colMATargetContractCode).getValue(),
            ProductCode: mygrid_MaintenanceContract.cells2(rowInx, colProductCode).getValue(),
            ServiceTypeCode: mygrid_MaintenanceContract.cells2(rowInx, colServiceTypeCode).getValue()
        };

    mycond_CMS210 = obj;

    //alert(mycond_CMS210.MATargetContractCode);

    $("#dlgCMS140").OpenCMS210Dialog(mycond_CMS210);
}

function openDetail_2(row_id) {

    mygrid_MaintenanceContract.selectRow(mygrid_MaintenanceContract.getRowIndex(row_id));

    var rowInx = mygrid_MaintenanceContract.getRowIndex(row_id);


    // Get column index
    var colMATargetContractCode = mygrid_MaintenanceContract.getColIndexById('RelatedContractCode');
    var colProductCode = mygrid_MaintenanceContract.getColIndexById('ProductCode');
    var colServiceTypeCode = mygrid_MaintenanceContract.getColIndexById('ServiceTypeCode');

    var bEnableContractBasic = mygrid_MaintenanceContract.cells2(rowInx, mygrid_MaintenanceContract.getColIndexById('EnableContractBasic')).getValue();
    var bEnableSaleBasic = mygrid_MaintenanceContract.cells2(rowInx, mygrid_MaintenanceContract.getColIndexById('EnableSaleBasic')).getValue();

    // ButtonContractBasic
    if (bEnableContractBasic == 1) {
        // open detail cms120 (new widow)

        var strRelatedContractCode = mygrid_MaintenanceContract.cells2(rowInx, colMATargetContractCode).getValue();

        //var myUrl = generate_url("/Common/CMS120") + "?strContractCode=" + strRelatedContractCode;
        //window.open(myUrl, 'CMS120', 'width=1024,height=1024,menubar=yes,status=yes');


        var obj = { "strContractCode": strRelatedContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, true);

    }
}

function openDetail_3(row_id) {

    mygrid_MaintenanceContract.selectRow(mygrid_MaintenanceContract.getRowIndex(row_id));

    var rowInx = mygrid_MaintenanceContract.getRowIndex(row_id);


    // Get column index
    var colMATargetContractCode = mygrid_MaintenanceContract.getColIndexById('RelatedContractCode');
    var colProductCode = mygrid_MaintenanceContract.getColIndexById('ProductCode');
    var colServiceTypeCode = mygrid_MaintenanceContract.getColIndexById('ServiceTypeCode');
    var colRelatedOCC = mygrid_MaintenanceContract.getColIndexById('RelatedOCC');

    var bEnableContractBasic = mygrid_MaintenanceContract.cells2(rowInx, mygrid_MaintenanceContract.getColIndexById('EnableContractBasic')).getValue();
    var bEnableSaleBasic = mygrid_MaintenanceContract.cells2(rowInx, mygrid_MaintenanceContract.getColIndexById('EnableSaleBasic')).getValue();

    // ButtonSaleBasic
    if (bEnableSaleBasic == 1) {
        // open detail cms160 (new widow)

        var strRelatedContractCode = mygrid_MaintenanceContract.cells2(rowInx, colMATargetContractCode).getValue();
        var strRelatedOCC = mygrid_MaintenanceContract.cells2(rowInx, colRelatedOCC).getValue();

        var obj = { "strContractCode": strRelatedContractCode, "strOCC": strRelatedOCC };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, true);

    }
}


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

            // **
            CMS140_CMS220ShowMode = "Contract";

            $("#dlgCMS140").OpenCMS220Dialog("CMS140");
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

            // **
            CMS140_CMS220ShowMode = "Customer";

            $("#dlgCMS140").OpenCMS220Dialog("CMS140");
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
            CMS140_CMS220ShowMode = "Site";

            $("#dlgCMS140").OpenCMS220Dialog("CMS140");
        });

    }
    else {
        $("#lnkSiteCode").parent().html("<div class='usr-label label-view'>-</div>");
    }


}

function initialHeaderButton() {

    var myurl = "";

    $("#btnContractBasic").click(function () {
        // go to CMS120

        //myurl = generate_url("/Common/CMS120") + "?strContractCode=" + CMS140Data.strContractCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj_myParam = { "strContractCode": CMS140Data.strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj_myParam, false);

    });

//    $("#btnSecurityBasic").click(function () {
//        // go to CMS130

//        //myurl = generate_url("/Common/CMS130") + "?strContractCode=" + CMS140Data.strContractCode + "&strOCC=" + CMS140Data.strOCC ;
//        //alert(myurl);
//        //window.location.href = myurl;

//        var obj_myParam = { "strContractCode": CMS140Data.strContractCode, "strOCC": CMS140Data.strOCC };
//        ajax_method.CallScreenControllerWithAuthority("/Common/CMS130", obj_myParam, false);


//    });

    $("#btnHistoryDigest").click(function () {
        // go to CMS150

        //myurl = generate_url("/Common/CMS150") + "?ContractCode=" + CMS140Data.strContractCode + "&ServiceTypeCode=" + CMS140Data.ServiceTypeCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj_myParam = { "ContractCode": CMS140Data.strContractCode, "ServiceTypeCode": CMS140Data.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS150", obj_myParam, false);

    });

    $("#btnContractBillingTransfer").click(function () {
        // go to CMS200

        //myurl = generate_url("/Common/CMS200") + "?strContractCode=" + CMS140Data.strContractCode + "&strServiceTypeCode=" + CMS140Data.ServiceTypeCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj_myParam = { "strContractCode": CMS140Data.strContractCode, "strServiceTypeCode": CMS140Data.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS200", obj_myParam, false);

    });

    //------------For phase 2--------------------------------

    $("#btnHeader_Installation").click(function () {
        // go to CMS180
        var obj_myParam = { "ContractCode": CMS140Data.strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj_myParam, false);

    });

    $("#btnHeader_BillingBasic").click(function () {
        // go to CMS420
        var obj_myParam = { "ContractCode": CMS140Data.strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj_myParam, false);

    });

    //----------------------------------------------------

}

function initialRightButton() {

    $("#btnViewIncidentList").click(function () {
        // New windos CTS320 : Incident list

        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: CMS140Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS320", obj, true);
    });

    $("#btnViewARList").click(function () {
        // New window CTS370 : AR list

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: CMS140Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS370", obj, true);
    });

    $("#btnRegisterIncident").click(function () {
        // New window CTS300 : Register new incident


        var obj = {
            strIncidentRelevantType: $("#C_INCIDENT_RELEVANT_TYPE_CONTRACT").val(),
            strIncidentRelevantCode: CMS140Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS300", obj, true);
    });

    $("#btnRegisterAR").click(function () {
        // New window CTS350 : Register new AR

        var obj = {
            strARRelevantType: $("#C_AR_RELEVANT_TYPE_CONTRACT").val(),
            strARRelevantCode: CMS140Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS350", obj, true);
    });


}

function CMS220Object() {
    return {
        "ContractCode": CMS140Data.strContractCode,
        "OCC": CMS140Data.strOCC,
        "ContractTargetCode": CMS140Data.ContractTargetCode,
        "RealCustomerCode": CMS140Data.RealCustomerCode,
        "SiteCode": CMS140Data.SiteCode,
        "Mode": CMS140_CMS220ShowMode

    };
}

// mycond_CMS210
function CMS210Object() {
    return mycond_CMS210;

}
