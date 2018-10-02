/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>

/// <reference path="../../Scripts/Base/GridControl.js"/>

var mygrid_MaintenanceContract;
var mygrid_CancelContract;
var grdFeeDetail;

var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;

var mycond_CMS210;

var CMS120_CMS220ShowMode;


/* ------ Main ------ */
$(document).ready(function () {


    // TODO : Comment for next phase------------------------------------------
    //$("#btnViewIncidentList").attr("disabled", true);
    //$("#btnViewARList").attr("disabled", true);
    //$("#btnRegisterIncident").attr("disabled", true);
    //$("#btnRegisterAR").attr("disabled", true);
    //$("#btnView_deposit_fee_information").attr("disabled", true);
    //------------------------------------------------------------------------

    /* --- grid -----*/


    if ($.find("#mygrid_MaintenanceContract").length > 0) {

        var parameter = { "strContractCode": CMS120Data.strContractCode, "strOCC": CMS120Data.strOCC };
        //$("#mygrid_MaintenanceContract").LoadDataToGrid(mygrid_MaintenanceContract, pageRow, true, "/Common/CMS120_GetMaintenanceContractTargetList", parameter, "View_dtRelatedContract", false);

        mygrid_MaintenanceContract = $("#mygrid_MaintenanceContract").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS120_GetMaintenanceContractTargetList", parameter, "View_dtRelatedContract", false);

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

    if ($.find("#mygrid_CancelContract").length > 0) {


        var parameter = { "strContractCode": CMS120Data.strContractCode, "strOCC": CMS120Data.strOCC };
        mygrid_CancelContract = $("#mygrid_CancelContract").LoadDataToGridWithInitial(pageRow, true, true, "/Common/CMS120_GetCancelContractMemoDetailList", parameter, "View_dtTbt_CancelContractMemoDetailForView", false);

        //        BindOnLoadedEvent(mygrid_CancelContract, function () {
        //            mygrid_CancelContract.setSizes();
        //        });


    }

    if ($.find("#grdFeeDetail").length > 0) {
        var parameter = { "strContractCode": CMS120Data.strContractCode, "strOCC": CMS120Data.strOCC };
        grdFeeDetail = $("#grdFeeDetail").LoadDataToGridWithInitial(0, false, false, "/Common/CMS120_GetContractFeeDetail", parameter, "CMS120_ContractFeeDetail", false);
    }

    /* ========= add hyper link =========== */
    initialHyperLink();

    /* ========= add event header button =========== */
    initialHeaderButton();

    /* ========= add event rigth button =========== */
    initialRightButton();


    // Set null value to "-"
    $("#divAll").SetEmptyViewData();


    $("#btnView_contract_Document_informationPO").click(function () {
        // Pop-up CMS131
        //var obj = CMS131Object();
        //        var parameter = {
        //            ContractCode: CMS120Data.strContractCode,
        //            OCC: CMS120Constant.C_OCC_PO
        //        };
        CMS120Data.OCCDialog = CMS120Constant.C_OCC_PO;

        $("#dlgCMS130").OpenCMS131Dialog(parameter);
    });
    $("#btnView_contract_Document_informationContract").click(function () {
        // Pop-up CMS131
        //var obj = CMS131Object();
        //        var parameter = {
        //            ContractCode: CMS120Data.strContractCode,
        //            OCC: CMS120Constant.C_OCC_CONTRACT_REPORT
        //        };
        CMS120Data.OCCDialog = CMS120Constant.C_OCC_CONTRACT_REPORT;

        $("#dlgCMS130").OpenCMS131Dialog(parameter);
    });
    $("#btnView_contract_Document_informationLetter").click(function () {
        // Pop-up CMS131
        //var obj = CMS131Object();
        //        var parameter = {
        //            ContractCode: CMS120Data.strContractCode,
        //            OCC: CMS120Constant.C_OCC_START_OPERATION_CONFIRM_LETTER
        //        };
        CMS120Data.OCCDialog = CMS120Constant.C_OCC_START_OPERATION_CONFIRM_LETTER;

        $("#dlgCMS130").OpenCMS131Dialog(parameter);
    });
    if (CMS120Data.txtAttachImportanceFlag == true) {
        $("#ChkAttachImportanceFlag").attr("checked", true);
    }
});

function CMS131Object() {
    return {
        "ContractCode": CMS120Data.strContractCode,
        "OCC": CMS120Data.OCCDialog
    };
}
/* -----------  event ----------- */

function openDetail_1(row_id) {

    mygrid_MaintenanceContract.selectRow(mygrid_MaintenanceContract.getRowIndex(row_id));

    var rowInx = mygrid_MaintenanceContract.getRowIndex(row_id);

    // Get column index
    var colMATargetContractCode = mygrid_MaintenanceContract.getColIndexById('RelatedContractCode');
    var colProductCode = mygrid_MaintenanceContract.getColIndexById('ProductCode');
    var colServiceTypeCode = mygrid_MaintenanceContract.getColIndexById('ServiceTypeCode');

    // ButtonMaintenance

    // open detail cms210 (pop-up)

    var obj =
        {
            ContractCode: CMS120Data.strContractCode,
            MATargetContractCode: mygrid_MaintenanceContract.cells2(rowInx, colMATargetContractCode).getValue(),
            ProductCode: mygrid_MaintenanceContract.cells2(rowInx, colProductCode).getValue(),
            ServiceTypeCode: mygrid_MaintenanceContract.cells2(rowInx, colServiceTypeCode).getValue()
        };

    mycond_CMS210 = obj;

    //alert(mycond_CMS210.MATargetContractCode);

    $("#dlgCMS120").OpenCMS210Dialog(mycond_CMS210);

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

// ind = colum index
function openDetail(id, ind) {

    var rowInx = mygrid_MaintenanceContract.getRowIndex(id);

    // Get column index
    var colMATargetContractCode = mygrid_MaintenanceContract.getColIndexById('RelatedContractCode');
    var colProductCode = mygrid_MaintenanceContract.getColIndexById('ProductCode');
    var colServiceTypeCode = mygrid_MaintenanceContract.getColIndexById('ServiceTypeCode');

    // ButtonMaintenance
    if (ind == mygrid_MaintenanceContract.getColIndexById('ButtonMaintenance')) {
        // open detail cms210 (pop-up)

        var obj =
        {
            ContractCode: CMS120Data.strContractCode,
            MATargetContractCode: mygrid_MaintenanceContract.cells2(rowInx, colMATargetContractCode).getValue(),
            ProductCode: mygrid_MaintenanceContract.cells2(rowInx, colProductCode).getValue(),
            ServiceTypeCode: mygrid_MaintenanceContract.cells2(rowInx, colServiceTypeCode).getValue()
        };

        mycond_CMS210 = obj;

        //alert(mycond_CMS210.MATargetContractCode);

        $("#dlgCMS120").OpenCMS210Dialog(mycond_CMS210);
    }


    var bEnableContractBasic = mygrid_MaintenanceContract.cells2(rowInx, mygrid_MaintenanceContract.getColIndexById('EnableContractBasic')).getValue();
    var bEnableSaleBasic = mygrid_MaintenanceContract.cells2(rowInx, mygrid_MaintenanceContract.getColIndexById('EnableSaleBasic')).getValue();

    // ButtonContractBasic
    if (ind == mygrid_MaintenanceContract.getColIndexById('ButtonContractBasic') && bEnableContractBasic == 1) {
        // open detail cms120 (new widow)

        var strRelatedContractCode = mygrid_MaintenanceContract.cells2(rowInx, colMATargetContractCode).getValue()
        var myUrl = generate_url("/Common/CMS120") + "?strContractCode=" + strRelatedContractCode;
        window.open(myUrl, 'CMS120', 'width=1024,height=1024,menubar=yes,status=yes');

    }


    // ButtonSaleBasic
    if (ind == mygrid_MaintenanceContract.getColIndexById('ButtonSaleBasic') && bEnableSaleBasic == 1) {
        // open detail cms160 (new widow)

        var strRelatedContractCode = mygrid_MaintenanceContract.cells2(rowInx, colMATargetContractCode).getValue()
        var myUrl = generate_url("/Common/CMS160") + "?strContractCode=" + strRelatedContractCode;
        window.open(myUrl, 'CMS160', 'width=1024,height=1024,menubar=yes,status=yes');
    }
}

function initialHyperLink() {

    /* ========= add hyper link =========== */

    /* === lnkCustomerCodeC --- to cms220 (pop-up) === */
    if ($("#lnkCustomerCodeC").text().length > 0) {

        $("#lnkCustomerCodeC").show();

        $("#lnkCustomerCodeC").initial_link(function (val) {
            var obj = CMS220Object();
            var parameter = {
                ContractCode: obj.ContractCode,
                OCC: obj.OCC,
                ContractTargetCode: obj.ContractTargetCode
            };

            CMS120_CMS220ShowMode = "Contract";

            $("#dlgCMS120").OpenCMS220Dialog("CMS120");
        });

    } else {

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
            CMS120_CMS220ShowMode = "Customer";

            $("#dlgCMS120").OpenCMS220Dialog("CMS120");
        });

    } else {
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

            CMS120_CMS220ShowMode = "Site";

            $("#dlgCMS120").OpenCMS220Dialog("CMS120");
        });

    } else {
        $("#lnkSiteCode").parent().html("<div class='usr-label label-view'>-</div>");
    }


    /* === lnkLinkage_sale_contract_code -- to cms160 === */
    if ($("#lnkLinkage_sale_contract_code").text().length > 0) {

        $("#lnkLinkage_sale_contract_code").click(function () {

            //var myUrl = generate_url("/Common/CMS160") + "?strContractCode=" + CMS120Data.RelatedContractCode + "&strOCC=" + CMS120Data.strOCC;
            //window.open(myUrl, 'CMS160', 'width=1024,height=1024,menubar=yes,status=yes');

            var obj = { "strContractCode": CMS120Data.RelatedContractCode, "strOCC": "" }; // old:  "strOCC": CMS120Data.strRelatedOCC
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS160", obj, true);

        });

    } else {
        $("#lnkLinkage_sale_contract_code").parent().html("<div class='usr-label label-view'>-</div>");
    }


    /* === lnkOld_contract_code --- to cms120 === */
    if ($("#lnkOld_contract_code").text().length > 0) {

        $("#lnkOld_contract_code").click(function () {

            //var myUrl = generate_url("/Common/CMS120") + "?strContractCode=" + CMS120Data.OldContractCode;
            //alert(myUrl);
            //window.open(myUrl, 'CMS120', 'width=1024,height=1024,menubar=yes,status=yes');

            var obj = { "strContractCode": CMS120Data.OldContractCode };
            ajax_method.CallScreenControllerWithAuthority("/Common/CMS120", obj, true);

        });

    } else {
        $("#lnkOld_contract_code").parent().html("<div class='usr-label label-view'>-</div>");
    }


}

function initialHeaderButton() {

    var myurl = "";
//    $("#btnSecurityBasic").click(function () {
//        // go to CMS130

//        //myurl = generate_url("/Common/CMS130") + "?strContractCode=" + CMS120Data.strContractCode + "&strOCC=" + CMS120Data.strOCC;
//        //alert(myurl);
//        //window.location.href = myurl;

//        var obj = { "strContractCode": CMS120Data.strContractCode, "strOCC": CMS120Data.strOCC };
//        ajax_method.CallScreenControllerWithAuthority("/Common/CMS130", obj, false);

//    });

//    $("#btnSecurityDetail").click(function () {
//        // go to CMS140

//        //myurl = generate_url("/Common/CMS140") + "?strContractCode=" + CMS120Data.strContractCode + "&strOCC=" + CMS120Data.strOCC;
//        //alert(myurl);
//        //window.location.href = myurl;

//        var obj = { "strContractCode": CMS120Data.strContractCode, "strOCC": CMS120Data.strOCC };
//        ajax_method.CallScreenControllerWithAuthority("/Common/CMS140", obj, false);

//    });

    $("#btnHistoryDigest").click(function () {
        // go to CMS150

        //myurl = generate_url("/Common/CMS150") + "?ContractCode=" + CMS120Data.strContractCode + "&ServiceTypeCode=" + CMS120Data.ServiceTypeCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj = { "ContractCode": CMS120Data.strContractCode, "ServiceTypeCode": CMS120Data.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS150", obj, false);

    });

    $("#btnContractBillingTransfer").click(function () {
        // go to CMS200

        //myurl = generate_url("/Common/CMS200") + "?strContractCode=" + CMS120Data.strContractCode + "&strServiceTypeCode=" + CMS120Data.ServiceTypeCode;
        //alert(myurl);
        //window.location.href = myurl;

        var obj = { "strContractCode": CMS120Data.strContractCode, "strServiceTypeCode": CMS120Data.ServiceTypeCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS200", obj, false);

    });

    //-------------------------------------- For phase 2 -------------------------------------//

    $("#btnHeader_Installation").click(function () {
        // go to CMS180

        var obj = { "ContractCode": CMS120Data.strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS180", obj, false);

    });

    $("#btnHeader_BillingBasic").click(function () {
        // go to CMS420

        var obj = { "ContractCode": CMS120Data.strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS420", obj, false);

    });

    //-------------------------------------- For phase 2 -------------------------------------//

}

function initialRightButton() {

    $("#btnViewCustomerHistory").click(function () {
        // Popup CMS300 : View customer history

        $("#dlgCMS120").OpenCMS300Dialog();


    });

    $("#btnViewIncidentList").click(function () {
        // New windos CTS320 : Incident list
        //alert("New windos CTS320 : Incident list");
        var obj = {
            strIncidentRelevantType: CMS120Data.IncidentRelevantType_Contract,
            strIncidentRelevantCode: CMS120Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS320", obj, true);
    });

    $("#btnViewARList").click(function () {
        // New window CTS370 : AR list
        //alert("New window CTS370 : AR list");
        var obj = {
            strARRelevantType: CMS120Data.ARRelevantType_Contract,
            strARRelevantCode: CMS120Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS370", obj, true);
    });

    $("#btnRegisterIncident").click(function () {
        // New window CTS300 : Register new incident
        //alert("New window CTS300 : Register new incident");
        var obj = {
            strIncidentRelevantType: CMS120Data.IncidentRelevantType_Contract,
            strIncidentRelevantCode: CMS120Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS300", obj, true);
    });

    $("#btnRegisterAR").click(function () {
        // New window CTS350 : Register new AR
        //alert("New window CTS350 : Register new AR");
        var obj = {
            strARRelevantType: CMS120Data.ARRelevantType_Contract,
            strARRelevantCode: CMS120Data.strContractCode
        };

        ajax_method.CallScreenControllerWithAuthority("/Contract/CTS350", obj, true);
    });

    $("#btnView_deposit_fee_information").click(function () {
        // Phase 2
        // Go to CMS430 
        var obj = { "ContractCode": CMS120Data.strContractCode };
        ajax_method.CallScreenControllerWithAuthority("/Common/CMS430", obj, true);
    });
}

function CMS220Object() {
    return {
        "ContractCode": CMS120Data.strContractCode,
        "OCC": CMS120Data.strOCC,
        "ContractTargetCode": CMS120Data.ContractTargetCode,
        "RealCustomerCode": CMS120Data.RealCustomerCode,
        "SiteCode": CMS120Data.SiteCode,
        "OldContractCode": CMS120Data.OldContractCode,
        "Mode": CMS120_CMS220ShowMode
    };

}

function CMS300Object() {
    return {
        "ContractCode": CMS120Data.strContractCode,
        "ServiceTypeCode": CMS120Data.ServiceTypeCode,
        "OCC": null,
        "CSCustCode": null,
        "RCCustCode": null,
        "SiteCode": null
    };
}


// mycond_CMS210
function CMS210Object() {
    return mycond_CMS210;

}

