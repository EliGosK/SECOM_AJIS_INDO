
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../Base/GridControl.js" />
/// <reference path="Dialog.js" />

var otherGrid;
var supportGrid;
var ExpectGrid;
var wipGrid;
var attachGrid;
var systemGrid;
var StockGrid;
var objProjectCode = { strProjectCode: CTS260ProjectCode };
var CTS260_gridAttach;
var isInitAttachGrid = false;
$(document).ready(function () {

    //    var selectOption = $('.selDiv').children('.opts');
    //    var _this = $(this).next().children(".opts");
    $("#pjOverallBudgetAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedBudgetAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedInstallationFee").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedInstallationFeeUsd").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedInstrumentPrice").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjReceivedInstrumentPriceUsd").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjLastOrderAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjStockedOutInstrumentAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);
    $("#pjInstallationOrderedAmount").BindNumericBox(12, 2, 0, 999999999999.99, 0);

    



    $("#ddlProjectBranch").change(function () {
        var Branch = $("#ddlProjectBranch option:selected").val();
        if ($("#StockOutGridPlane").length > 0) {
            if (Branch != "") {
                Branch = Branch.split(":");
                $("#StockOutGridPlane").LoadDataToGrid(StockGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/contract/CTS260_GetStockOutForView", { strProjectCode: CTS260ProjectCode, "BranchNo": Branch[1] }, "dtTbt_ProjectStockoutBranchIntrumentDetailForView", false);
            }
            else {
                $("#StockOutGridPlane").LoadDataToGrid(StockGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/contract/CTS260_GetStockOutForView", {}, "dtTbt_ProjectStockoutBranchIntrumentDetailForView", false);

            }
        }
    });

    initGrid();
    $("#btnViewContractDetail").click(function () {

        $("#dlgBox").OpenCTS261Dialog("CTS260");

    });

    //================ GRID ATTACH ========================================     
    //CTS260_gridAttach = $("#CTS260_gridAttachDocList").InitialGrid(10, false, "/Contract/CTS260_IntialGridAttachedDocList");
    InitLoadAttachList(); //Modify by Jutarat A. on 22032014

    SpecialGridControl(CTS260_gridAttach, ["downloadButton"]);
    BindOnLoadedEvent(CTS260_gridAttach, CTS260_gridAttachBinding);
    //RefreshAttachList(); //Comment by Jutarat A. on 22032014
    //====================================================================

    if (objProjectCode.strProjectCode == "") {
        $("#ProjectInfo").hide();
        $("#SecomInfoSection").hide();
        $("#ProjectCode").SetReadOnly(false);
    }
    else {
        $("#ProjectCode").val(objProjectCode.strProjectCode);

        RetrieveProjectInformation();
        $("#ProjectCode").SetReadOnly(true);
        $("#btnRetrieveProject").SetDisabled(true);
        $("#btnSearchProject").SetDisabled(true);
    }

    $("#btnRetrieveProject").click(function () {
        objProjectCode.strProjectCode = $("#ProjectCode").val();
        RetrieveProjectInformation();
    });
    $("#btnSearchProject").click(function () {
        $("#dlgBox").OpenCMS290Dialog("CTS260");
    });
    $("#btnClearProject").click(function () {
        objProjectCode.strProjectCode = "";
        $("#ProjectInfo").hide();
        $("#SecomInfoSection").hide();
        $("#ProjectCode").SetReadOnly(false);
        $("#ProjectCode").val("");
        $("#ProjectCode").ResetToNormalControl();
        $("#btnRetrieveProject").SetDisabled(false);
        $("#btnSearchProject").SetDisabled(false);

        /* --- Set condition --- */
        SEARCH_CONDITION = null;
        /* --------------------- */
    });
});

function CMS290Response(obj) {
    $("#dlgBox").CloseDialog();
    if ((obj != null) && (obj.ProjectCode != null) && (obj.ProjectCode.length > 0)) {
        objProjectCode.strProjectCode = obj.ProjectCode;
        $("#ProjectCode").val(obj.ProjectCode);

        RetrieveProjectInformation();
    }
}

function RetrieveProjectInformation() {

    master_event.LockWindow(true);
    call_ajax_method_json("/contract/CTS260_GetProjectForView", objProjectCode, function (res, ctrls) {
        if (ctrls != undefined) {
            VaridateCtrl(["ProjectCode"], ctrls);
        }
        else if (res != null) {
            $("#ProjectInfo").show();
            $("#SecomInfoSection").show();
            $("#ProjectCode").SetReadOnly(true);
            $("#btnRetrieveProject").SetDisabled(true);
            $("#btnSearchProject").SetDisabled(true);

            $("#ProjectCode").val(res[0].ProjectCode);
            $("#ProjectInfo").bindJSON_Prefix("pj", res[0]);
            $("input.numeric-box:text").SetNumericValue_Prefix("pj", res[0], 2);
            $("#SecomInfoSection").bindJSON_Prefix("pj", res[0]);

            $("#pjBiddingDate").val(ConvertDateToTextFormat(ConvertDateObject(res[0].BiddingDate)));


            if (res[0].OverallBudgetAmountCurrencyType == undefined)
                res[0].OverallBudgetAmountCurrencyType = C_CURRENCY_LOCAL;
            if (res[0].ReceivedBudgetAmountCurrencyType == undefined)
                res[0].ReceivedBudgetAmountCurrencyType = C_CURRENCY_LOCAL;
            if (res[0].LastOrderAmountCurrencyType == undefined)
                res[0].LastOrderAmountCurrencyType = C_CURRENCY_LOCAL;
            if (res[0].StockedOutInstrumentAmountCurrencyType == undefined)
                res[0].StockedOutInstrumentAmountCurrencyType = C_CURRENCY_LOCAL;
            if (res[0].InstallationOrderedAmountCurrencyType == undefined)
                res[0].InstallationOrderedAmountCurrencyType = C_CURRENCY_LOCAL;

            $("#pjOverallBudgetAmount").SetNumericCurrency(res[0].OverallBudgetAmountCurrencyType);
            $("#pjReceivedBudgetAmount").SetNumericCurrency(res[0].ReceivedBudgetAmountCurrencyType);
            $("#pjReceivedInstallationFee").SetNumericCurrency(C_CURRENCY_LOCAL);
            $("#pjReceivedInstallationFeeUsd").SetNumericCurrency(C_CURRENCY_US);
            $("#pjReceivedInstrumentPrice").SetNumericCurrency(C_CURRENCY_LOCAL);
            $("#pjReceivedInstrumentPriceUsd").SetNumericCurrency(C_CURRENCY_US);
            $("#pjLastOrderAmount").SetNumericCurrency(res[0].LastOrderAmountCurrencyType);
            $("#pjStockedOutInstrumentAmount").SetNumericCurrency(res[0].StockedOutInstrumentAmountCurrencyType);
            $("#pjInstallationOrderedAmount").SetNumericCurrency(res[0].InstallationOrderedAmountCurrencyType);

            if (res[0].OverallBudgetAmountCurrencyType == C_CURRENCY_US)
                $("#pjOverallBudgetAmount").val(res[0].OverallBudgetAmountUsd);
            if (res[0].ReceivedBudgetAmountCurrencyType == C_CURRENCY_US)
                $("#pjReceivedBudgetAmount").val(res[0].ReceivedBudgetAmountUsd);
            if (res[0].LastOrderAmountCurrencyType == C_CURRENCY_US)
                $("#pjLastOrderAmount").val(res[0].LastOrderAmountUsd);
            if (res[0].StockedOutInstrumentAmountCurrencyType == C_CURRENCY_US)
                $("#pjLastOrderAmount").val(res[0].StockedOutInstrumentAmountUsd);
            if (res[0].InstallationOrderedAmountCurrencyType == C_CURRENCY_US)
                $("#pjLastOrderAmount").val(res[0].InstallationOrderedAmountUsd);

            call_ajax_method_json("/contract/CTS260_GetProjectPurchaser", objProjectCode, function (res) {
                $("#ProjectPurchaserSection").bindJSON_Prefix("CP", res[0]);

                //$("input[type='text']").attr('readonly', 'readonly');
                $("#ProjectInfo").SetEnableView(false);
                $("#SecomInfoSection").SetEnableView(false);
                $("#ddlProjectBranch").attr("disabled", false);
                $("#btnViewContractDetail").attr("disabled", false);

                //Modify by Jutarat A. on 17012013
                CTS260ProjectCode = objProjectCode.strProjectCode;

                call_ajax_method_json("/contract/CTS240_GetStockOutMemo", { ProjectCode: CTS260ProjectCode }, function (result, controls) {
                    if (result != null) {
                        $("#pjStockoutMemo").val(result);
                    }

                    RetrieveProjectData(objProjectCode);
                });
                //End Add

            });

            master_event.LockWindow(false);

            /* --- Set condition --- */
            SEARCH_CONDITION = {
                ProjectCode: objProjectCode.strProjectCode
            };
            /* --------------------- */
        }

    });
}

//Add by Jutarat A. on 17012013
function RetrieveProjectData(objProjectCode) {
    $("#OtherGridPlane").LoadDataToGrid(otherGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/contract/CTS260_OtherRelateForView", objProjectCode, "tbt_ProjectOtherRalatedCompany", false, function () {

        $("#SupportGridPlane").LoadDataToGrid(supportGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/contract/CTS260_GetSupportStaffForView", objProjectCode, "dtTbt_ProjectSupportStaffDetailForView", false, function () {

            $("#ExpectGridPlane").LoadDataToGrid(ExpectGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/contract/CTS260_GetExpectIntrumentDetailForView", objProjectCode, "dtTbt_ProjectExpectedInstrumentDetailsForView", false, function () {

                $("#systemGridPlane").LoadDataToGrid(systemGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/contract/CTS260_GetSystemDetailForView", objProjectCode, "dtTbt_ProjectSystemDetailForView", false, function () {

                    $("#WipGridPlane").LoadDataToGrid(wipGrid, ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/contract/CTS260_ProjectWIPForView", objProjectCode, "dtTbt_ProjectStockoutIntrumentForView", false, function () {

                        call_ajax_method_json("/contract/CTS240_RegenBranchCombo", { ProjectCode: objProjectCode.strProjectCode }, function (data) {
                            regenerate_combo("#ddlProjectBranch", data);
                        });
        
                    });

                });

            });

        });

    });
}
//End Add

function CTS261Object() {

    return objProjectCode;
}

$.fn.SetNumericValue_Prefix = function (prefix, obj, dec) {
    var strProp;
    this.each(function () {
        strProp = $(this).attr("id");
        strProp = strProp.substring(prefix.length);
        $(this).val(SetNumericValue(obj[strProp], dec));
        $(this).setComma();
    });

}
function initGrid() {
    if ($("#OtherGridPlane").length > 0) {
        otherGrid = $("#OtherGridPlane").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, false, false, "/contract/CTS260_OtherRelateForView", objProjectCode, "tbt_ProjectOtherRalatedCompany", false);

        BindOnLoadedEvent(otherGrid, function () {
            for (var i = 0; i < otherGrid.getRowsNum(); i++) {
                var row_id = otherGrid.getRowId(i);

            }
            otherGrid.setSizes();
        });
        //SpecialGridControl(otherGrid, []);
    }
    if ($("#SupportGridPlane").length > 0) {
        supportGrid = $("#SupportGridPlane").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, false, false, "/contract/CTS260_GetSupportStaffForView", objProjectCode, "dtTbt_ProjectSupportStaffDetailForView", false);

        BindOnLoadedEvent(supportGrid, function () {
            for (var i = 0; i < supportGrid.getRowsNum(); i++) {
                var row_id = supportGrid.getRowId(i);

            }
            supportGrid.setSizes();
        });
    }
    if ($("#ExpectGridPlane").length > 0) {
        ExpectGrid = $("#ExpectGridPlane").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, false, false, "/contract/CTS260_GetExpectIntrumentDetailForView", objProjectCode, "dtTbt_ProjectExpectedInstrumentDetailsForView", false);
        BindOnLoadedEvent(ExpectGrid, function () {
            for (var i = 0; i < ExpectGrid.getRowsNum(); i++) {
                var row_id = ExpectGrid.getRowId(i);
            }
            ExpectGrid.setSizes();
        });
    }
    if ($("#systemGridPlane").length > 0) {
        systemGrid = $("#systemGridPlane").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, false, false, "/contract/CTS260_GetSystemDetailForView", objProjectCode, "dtTbt_ProjectSystemDetailForView", false);

        BindOnLoadedEvent(systemGrid, function () {
            for (var i = 0; i < systemGrid.getRowsNum(); i++) {
                var row_id = systemGrid.getRowId(i);
            }
            systemGrid.setSizes();
        });
    }
    if ($("#WipGridPlane").length > 0) {
        wipGrid = $("#WipGridPlane").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, false, false, "/contract/CTS260_ProjectWIPForView", objProjectCode, "dtTbt_ProjectStockoutIntrumentForView", false);

        BindOnLoadedEvent(wipGrid, function () {
            for (var i = 0; i < wipGrid.getRowsNum(); i++) {
                var row_id = wipGrid.getRowId(i);
            }
            wipGrid.setSizes();
        });
    }
    if ($("#StockOutGridPlane").length > 0) {
        //  var objStock = { strProjectCode: CTS260ProjectCode, "BranchNo": $("#ddlProjectBranch option:selected").val() };
        StockGrid = $("#StockOutGridPlane").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, false, "/contract/CTS260_stock");
        BindOnLoadedEvent(StockGrid, function () {
            for (var i = 0; i < StockGrid.getRowsNum(); i++) {
                var row_id = StockGrid.getRowId(i);
            }
            StockGrid.setSizes();
        });
    }
    if ($("#AttachGridPlane").length > 0) {
        attachGrid = $("#AttachGridPlane").LoadDataToGridWithInitial(ROWS_PER_PAGE_FOR_VIEWPAGE, true, false, "/contract/CTS260_AttachGrid", objProjectCode, "dtAttachFileNameID", false);
        BindOnLoadedEvent(attachGrid, function () {
            for (var i = 0; i < attachGrid.getRowsNum(); i++) {
                var row_id = attachGrid.getRowId(i);

                GenerateDownloadButton(attachGrid, "btnDownload", row_id, "Download", true);
                BindGridButtonClickEvent("btnDownload", row_id, function (rid) {
                    var col = attachGrid.getColIndexById('AttachFileID');
                    var fileID = attachGrid.cells(rid, col).getValue();
                    var link = generate_url("/contract/CTS260_Download") + "?ReleateID=" + CTS260ProjectCode + "&AttachFileID=" + fileID;
                    window.location.href(link);
                });
            }
            attachGrid.setSizes();
        });
    }
}

function btnRemoveAttach_click(row_id) {
    var _colID = CTS260_gridAttach.getColIndexById("AttachFileID");
    var _targID = CTS260_gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };
    call_ajax_method_json("/Contract/CTS260_RemoveAttach", obj, function (result, controls) {
        if (result != null) {
            RefreshAttachList();
        }
    });
}

//Add by Jutarat A. on 22032014
function InitLoadAttachList() {

    CTS260_gridAttach = $("#CTS260_gridAttachDocList").LoadDataToGridWithInitial(0, false, false, "/Contract/CTS260_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, false, null,
                            function () {
                                isInitAttachGrid = true;
                            });
}
//End Add

function RefreshAttachList() {

    //if (CTS260_gridAttach != null) {
    if (CTS260_gridAttach != undefined && isInitAttachGrid) { //Modify by Jutarat A. on 22032014
        $('#CTS260_gridAttachDocList').LoadDataToGrid(CTS260_gridAttach, 0, false, "/Contract/CTS260_LoadGridAttachedDocList", "", "dtAttachFileForGridView", false, null, null)
    }
}

function CTS260_gridAttachBinding() {
//    if (isInitAttachGrid) {     

//        for (var i = 0; i < CTS260_gridAttach.getRowsNum(); i++) {
//            var row_id = CTS260_gridAttach.getRowId(i);
//            
//            GenerateDownloadButton(CTS260_gridAttach, "btnDownloadAttach", row_id, "downloadButton", true);           
//            BindGridButtonClickEvent("btnDownloadAttach", row_id, btnDownloadAttach_clicked);
//        }
//    } else {
//        isInitAttachGrid = true;
    //    }

    if (CTS260_gridAttach != undefined) {

        for (var i = 0; i < CTS260_gridAttach.getRowsNum(); i++) {
            var row_id = CTS260_gridAttach.getRowId(i);
            GenerateDownloadButton(CTS260_gridAttach, "btnDownloadAttach", row_id, "downloadButton", true);
            BindGridButtonClickEvent("btnDownloadAttach", row_id, btnDownloadAttach_clicked);
        }
    }
    

    CTS260_gridAttach.setSizes();
}

function btnDownloadAttach_clicked(row_id) {
    var _colID = CTS260_gridAttach.getColIndexById("AttachFileID");
    var _targID = CTS260_gridAttach.cells(row_id, _colID).getValue();

    var obj = {
        AttachID: _targID
    };

    //Modify by Jutarat A. on 31012013
//    var key = ajax_method.GetKeyURL(null);
//    var link = ajax_method.GenerateURL("/Contract/CTS260_DownloadAttach" + "?AttachID=" + _targID + "&k=" + key);

//    window.open(link, "download");
    download_method.CallDownloadController("ifDownload", "/Contract/CTS260_DownloadAttach", obj);
    //End Modify
}

