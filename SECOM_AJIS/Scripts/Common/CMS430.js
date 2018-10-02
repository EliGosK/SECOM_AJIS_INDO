//  jQuery
/// <reference path="../../Scripts/jquery-1.5.1-vsdoc.js"/>

// dhtmlx grid
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgrid.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_data.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/dhtmlxgridcell.js"/>
/// <reference path = "../../Content/js/dhtmlxgrid/ext/dhtmlxgrid_pgn.js"/>
/// <reference path = "../../Scripts/Base/Master.js" />
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Common/Dialog.js"/>
/// <reference path="../../Scripts/json2.js" />


/// <reference path = "../../Scripts/Base/GridControl.js" />




//---------------------------------- tt---------------

var myGridCMS430;
var depositGrid;
var strBlanceDeposit;
$(document).ready(function () {

    if (objCMS430Conts.BillingOCC != undefined && objCMS430Conts.BillingOCC != "") {        
        var obj = {
            ContractCode: objCMS430Conts.ContractCode,
            BillingOCC: objCMS430Conts.BillingOCC
        };
        doSelectOCCInitial(obj);
        return;
    }
    $("#divDepositDetailInformation").hide();
    InitialBillingOCCGrid();
    InitDepositDetailGrid();
    IntialPage();
});

function InitialBillingOCCGrid() {

    myGridCMS430 = $("#gridBillingOCCList").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS430_InitialBillingOCCGrid",
    "", "dtViewBillingOccList", false);

    SpecialGridControl(myGridCMS430, ["Detail"]);
    BindOnLoadedEvent(myGridCMS430,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(myGridCMS430, false) == false) {
            for (var i = 0; i < myGridCMS430.getRowsNum(); i++) {

                var rid = myGridCMS430.getRowId(i);

                if (gen_ctrl == true) {
                    //----------- Generate Detail button        
                    GenerateDetailButton(myGridCMS430, "btnDetail", rid, "Detail", true);
                }

                BindGridButtonClickEvent("btnDetail", rid, doSelectOCCGrid);

            }
            //$("#chkHeader").unbind("click");
            //$("#chkHeader").click(selectAllCheckboxControl);
            myGridCMS430.setSizes();
        }
    });
}

function IntialPage() {

    // Set null value to "-"  ***
    $("#divListofOCCforbillingdepositfee").SetEmptyViewData();
    $("#divDepositDetailInformation").SetEmptyViewData();
}

function InitDepositDetailGrid() {

    depositGrid = $("#gridDepositDetail").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Common/CMS430_InitialDeposiDetailGrid");

    BindOnLoadedEvent(depositGrid,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(depositGrid, false) == false) {
            for (var i = 0; i < depositGrid.getRowsNum(); i++) {

                var rid = depositGrid.getRowId(i);

                if (gen_ctrl == true) {
                    var billingTargetCodeForSlideColInx = depositGrid.getColIndexById('BillingTargetCodeForSlide');
                    var strBillingtargetCodeShort = depositGrid.cells2(depositGrid.getRowIndex(rid), depositGrid.getColIndexById('BillingTargetCode_short')).getValue().toString();
                    var strDepositStatus = depositGrid.cells2(depositGrid.getRowIndex(rid), depositGrid.getColIndexById('DepositStatus')).getValue().toString();
                    if (strDepositStatus == objCMS430Conts.C_DEPOSIT_STATUS_SLIDE) {
                        depositGrid.cells2(i, billingTargetCodeForSlideColInx).setValue(strBillingtargetCodeShort);
                    }
                }

            }
            depositGrid.setSizes();
        }
    });

}

////////////// Even Click Grid ///////////////
function doSelectOCCGrid(row_id) {

    $("#divDepositDetailInformation").show();

    myGridCMS430.selectRow(myGridCMS430.getRowIndex(row_id));

    document.getElementById('divDepositDetailInformation').scrollIntoView();

    var strContractCode = myGridCMS430.cells2(myGridCMS430.getRowIndex(row_id), myGridCMS430.getColIndexById('ContractCode_short')).getValue().toString();
    var strBillingOCC = myGridCMS430.cells2(myGridCMS430.getRowIndex(row_id), myGridCMS430.getColIndexById('BillingOCC')).getValue().toString();
    var tmp = (myGridCMS430.cells2(myGridCMS430.getRowIndex(row_id), myGridCMS430.getColIndexById('TextTransferBalanceDeposit')).getValue()).split(" ");
    strBlanceDeposit = tmp[1].toString();
    var obj = {
        ContractCode: strContractCode,
        BillingOCC: strBillingOCC
    };

    SetValueToDepositInfo(obj);
    //GetViewDepositGrid(obj);
}
function SetValueToDepositInfo(obj) {


    ajax_method.CallScreenController("/Common/CMS430_GetViewDepositDetailControl", obj,
        function (result, controls) {

            if (result != undefined) {
                CMS430_ClearControl();
                $("#CMS430_BillingCode").text(result.BillingCode_Short);
                $("#CMS430_BillingOffice").text(result.OfficeName);
                $("#CMS430_BillingTargetCode").text(result.BillingTargetCode_short);
                $("#CMS430_BillingClientNameEnglish").text(result.FullNameEN);
                $("#CMS430_BillingClientBranchNameEnglish").text(result.BranchNameEN);
                $("#CMS430_BillingClientAddressEnglish").text(result.AddressEN);
                $("#CMS430_BillingClientNameLocal").text(result.FullNameLC);
                $("#CMS430_BillingClientBranchNameLocal").text(result.BranchNameLC);
                $("#CMS430_BillingClientAddressLocal").text(result.AddressLC);
                GetViewDepositGrid(obj);
            }
            else {
                $("#divDepositDetailInformation").hide();
            }
            IntialPage();
        });
}

function CMS430_ClearControl() {
    $("#CMS430_BillingCode").text("");
    $("#CMS430_BillingOffice").text("");
    $("#CMS430_BillingTargetCode").text("");
    $("#CMS430_BillingClientNameEnglish").text("");
    $("#CMS430_BillingClientBranchNameEnglish").text("");
    $("#CMS430_BillingClientAddressEnglish").text("");
    $("#CMS430_BillingClientNameLocal").text("");
    $("#CMS430_BillingClientBranchNameLocal").text("");
    $("#CMS430_BillingClientAddressLocal").text("");
}

function GetViewDepositGrid(obj) {
    if ($("#gridDepositDetail").length > 0) {
        $("#gridDepositDetail").LoadDataToGrid(depositGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Common/CMS430_GetViewDepositDetailGrid", obj, "dtViewDepositDetailInformation", false,
                         function () {
                             //document.getElementById('divDepositDetailInformation').scrollIntoView();
                             //IntialPage();
                         },
                        function () {
                            $("#divDepositDetailInformation").show();
                        });
    }
}

function doSelectOCCInitial(obj) {

    SetValueToDepositInfoWithInitial(obj);    
    $("#divListofOCCforbillingdepositfee").hide();
    $("#divDepositDetailInformation").show();
    $("#divDepositDetailInformation").show();
}

function SetValueToDepositInfoWithInitial(obj) {


    ajax_method.CallScreenController("/Common/CMS430_GetViewDepositDetailControl", obj,
        function (result, controls) {

            if (result != undefined) {
                CMS430_ClearControl();
                $("#CMS430_BillingCode").text(result.BillingCode_Short);
                $("#CMS430_BillingOffice").text(result.OfficeName);
                $("#CMS430_BillingTargetCode").text(result.BillingTargetCode_short);
                $("#CMS430_BillingClientNameEnglish").text(result.FullNameEN);
                $("#CMS430_BillingClientBranchNameEnglish").text(result.BranchNameEN);
                $("#CMS430_BillingClientAddressEnglish").text(result.AddressEN);
                $("#CMS430_BillingClientNameLocal").text(result.FullNameLC);
                $("#CMS430_BillingClientBranchNameLocal").text(result.BranchNameLC);
                $("#CMS430_BillingClientAddressLocal").text(result.AddressLC);
                InitDepositDetailGridWithInitial(obj);
            }
            else {
                $("#divDepositDetailInformation").hide();
            }
            IntialPage();
        });
}
function InitDepositDetailGridWithInitial(obj) {

    depositGrid = $("#gridDepositDetail").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, true, "/Common/CMS430_GetViewDepositDetailGrid",
    obj, "dtViewDepositDetailInformation", false);

    BindOnLoadedEvent(depositGrid,
    function (gen_ctrl) {
        if (CheckFirstRowIsEmpty(depositGrid, false) == false) {
            for (var i = 0; i < depositGrid.getRowsNum(); i++) {

                var rid = depositGrid.getRowId(i);

                if (gen_ctrl == true) {
                    var billingTargetCodeForSlideColInx = depositGrid.getColIndexById('BillingTargetCodeForSlide');
                    var strBillingtargetCodeShort = depositGrid.cells2(depositGrid.getRowIndex(rid), depositGrid.getColIndexById('BillingTargetCode_short')).getValue().toString();
                    var strDepositStatus = depositGrid.cells2(depositGrid.getRowIndex(rid), depositGrid.getColIndexById('DepositStatus')).getValue().toString();
                    if (strDepositStatus == objCMS430Conts.C_DEPOSIT_STATUS_SLIDE) {
                        depositGrid.cells2(i, billingTargetCodeForSlideColInx).setValue(strBillingtargetCodeShort);
                    }
                }

            }
            depositGrid.setSizes();
        }
    });

}
