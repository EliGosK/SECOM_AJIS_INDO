
/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/control_events.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>
/// <reference path="../../Scripts/Base/DateTimePicker.js"/>

/// <reference path="../Base/object/ajax_method.js" />
/// <reference path="../Base/object/command_event.js" />
/// <reference path="../Base/object/master_event.js" />
/// <reference path="../json.js" />
/// <reference path="../json2.js" />

var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;
var grdDebtActualTableGrid;
var grdListOfUnPaidBillingTargetByBillingOfficeGrid;
var grdListOfUnPaidInvoiceByBillingTargetGrid;
var grdListOfUnPaidByBillingTargetGrid;

var _RawdtpMonthYear;
var _strOfficeCode;
var _strOfficeName;

var _strBillingTargetCode;
var _strInvoiceNo;
var _strInvoiceOCC;

var _doOfficeDataDo;
var _doGetBillingOfficeDebtSummaryList;

// add by Jirawat Jannet on 2016-10-10
var _doNewGetBillingOfficeDebtSummaryList;

var _bolFirstLoad = true;

var _UnPaidBLByBLOffice_RowIndex;

var conYes = "";
var conNo = "";

$(document).ready(function () {
    // Initial grid 1
    if ($.find("#ICS030_DebtActualTableGrid").length > 0) {

        //Comment by Jutarat A. on 05032012
        //grdDebtActualTableGrid = $("#ICS030_DebtActualTableGrid").InitialGrid(0, false, "/Income/ICS030_InitialDebtActualTableGrid", function () {
        //SpecialGridControl(grdDebtActualTableGrid, ["ICS030_DebtActualTableGrid"]);
        //BindOnLoadedEvent(grdDebtActualTableGrid, function () {

        //    grdDebtActualTableGrid.setSizes();
        //});
        //End Comment

        if (_bolFirstLoad == true) {
            _bolFirstLoad = false;
            change_MonthYear_FirstTime();
        }
        //}); //Comment by Jutarat A. on 05032012
    }

    //Add by Jutarat A. on 04032014
    var curPage;
    var startRow;
    var endRow;
    //End Add;

    // Initial grid 2
    if ($.find("#ICS030_ListOfUnPaidBillingTargetByBillingOfficeGrid").length > 0) {
        grdListOfUnPaidBillingTargetByBillingOfficeGrid = $("#ICS030_ListOfUnPaidBillingTargetByBillingOfficeGrid").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE
            , true, "/Income/ICS030_InitialListOfUnPaidBillingTargetByBillingOfficeGrid");

        //SpecialGridControl(grdListOfUnPaidBillingTargetByBillingOfficeGrid, ["ICS030_ListOfUnPaidBillingTargetByBillingOfficeGrid"]);
        BindOnLoadedEvent(grdListOfUnPaidBillingTargetByBillingOfficeGrid, function () {
            var colBillingTargetCodeShort = grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById('BillingTargetCodeShort');
            var colUnpaidInvoice = grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById('UnpaidInvoiceString');
            var colUnpaidDetail = grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById('UnpaidDetailString');

            var colOldestBillingTargetExpectedPaymentDateFlag = grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById('OldestBillingTargetExpectedPaymentDateFlag');
            var colOldestInvoiceExpectedPaymentDateFlag = grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById('OldestInvoiceExpectedPaymentDateFlag');

            //Add by Jutarat A. on 04032014
            curPage = grdListOfUnPaidBillingTargetByBillingOfficeGrid.currentPage;
            startRow = ((curPage * ROWS_PER_PAGE_FOR_VIEWPAGE) - ROWS_PER_PAGE_FOR_VIEWPAGE);
            endRow = ((curPage * ROWS_PER_PAGE_FOR_VIEWPAGE) - 1);
            //End Add

            //for (var i = 0; i < grdListOfUnPaidBillingTargetByBillingOfficeGrid.getRowsNum(); i++) {
            for (var i = startRow; i <= endRow && i < grdListOfUnPaidBillingTargetByBillingOfficeGrid.getRowsNum(); i++) { //Modify by Jutarat A. on 04032014

                var rowId = grdListOfUnPaidBillingTargetByBillingOfficeGrid.getRowId(i);
                var billingTargetCodeShort = grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(i, colBillingTargetCodeShort).getValue();

                //Modify by Warakorn M. on 19/05/2014
                /*//Unpaid invoice
                var unpaidInvoiceValue = grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(i, colUnpaidInvoice).getValue();
                if (unpaidInvoiceValue != "" && unpaidInvoiceValue != "0") {
                var tagUnpaidInvoice = '<a href="#" onclick="UnPaidBillingTargetUnpaidInvoiceClick(\'' + i + '\',\'' + billingTargetCodeShort + '\')">' + unpaidInvoiceValue + '</a>';
                grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(i, colUnpaidInvoice).setValue(tagUnpaidInvoice);
                }

                //Unpaid detail
                var unpaidDetailValue = grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(i, colUnpaidDetail).getValue();
                if (unpaidDetailValue != "" && unpaidDetailValue != "0") {
                var tagUnpaidDetail = '<a href="#" onclick="UnPaidBillingTargetUnpaidDetailClick(\'' + i + '\',\'' + billingTargetCodeShort + '\')">' + unpaidDetailValue + '</a>';
                grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(i, colUnpaidDetail).setValue(tagUnpaidDetail);
                }
                */
                var strBillingTargetDebtSummaryByOffice = grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(i, grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById("ToJson")).getValue();
                var objBillingTargetDebtSummaryByOffice = JSON.parse(htmlDecode(strBillingTargetDebtSummaryByOffice));

                //Unpaid invoice
                if (objBillingTargetDebtSummaryByOffice.UnpaidInvoice != 0) {
                    var tagUnpaidInvoice = '<a href="#" onclick="UnPaidBillingTargetUnpaidInvoiceClick(\'' + i + '\',\'' + billingTargetCodeShort + '\')">' + objBillingTargetDebtSummaryByOffice.UnpaidInvoiceString + '</a>';
                    grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(i, colUnpaidInvoice).setValue(tagUnpaidInvoice);
                }

                //Unpaid detail
                if (objBillingTargetDebtSummaryByOffice.UnpaidDetail != 0) {
                    var tagUnpaidDetail = '<a href="#" onclick="UnPaidBillingTargetUnpaidDetailClick(\'' + i + '\',\'' + billingTargetCodeShort + '\')">' + objBillingTargetDebtSummaryByOffice.UnpaidDetailString + '</a>';
                    grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(i, colUnpaidDetail).setValue(tagUnpaidDetail);
                }

                //Set row color
                if (grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells(rowId, colOldestBillingTargetExpectedPaymentDateFlag).getValue() == '1'
                    || grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells(rowId, colOldestInvoiceExpectedPaymentDateFlag).getValue() == '1') {
                    grdListOfUnPaidBillingTargetByBillingOfficeGrid.setRowColor(rowId, "#ff9999");
                }
            }
            grdListOfUnPaidBillingTargetByBillingOfficeGrid.setSizes();
        });

    }


    // Initial grid 3
    if ($.find("#ICS030_ListOfUnPaidInvoiceByBillingTargetGrid").length > 0) {
        grdListOfUnPaidInvoiceByBillingTargetGrid = $("#ICS030_ListOfUnPaidInvoiceByBillingTargetGrid").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE
            , true, "/Income/ICS030_InitialListOfUnPaidInvoiceByBillingTargetGrid");

        SpecialGridControl(grdListOfUnPaidInvoiceByBillingTargetGrid, ["ViewRegisterDebtTracingInfo"]);
        BindOnLoadedEvent(grdListOfUnPaidInvoiceByBillingTargetGrid, function () {
            //view col
            var colInx = grdListOfUnPaidInvoiceByBillingTargetGrid.getColIndexById('ViewRegisterDebtTracingInfo');

            //no of billing detail col
            var colInvoiceNo = grdListOfUnPaidInvoiceByBillingTargetGrid.getColIndexById('InvoiceNo');
            var colInvoiceOCC = grdListOfUnPaidInvoiceByBillingTargetGrid.getColIndexById('InvoiceOCC');
            var colNoOfBillingDetail = grdListOfUnPaidInvoiceByBillingTargetGrid.getColIndexById('NoOfBillingDetailString');
            var colOldestInvoiceExpectedPaymentDateFlag = grdListOfUnPaidInvoiceByBillingTargetGrid.getColIndexById('OldestInvoiceExpectedPaymentDateFlag');
            var colUnpaidAmount = grdListOfUnPaidInvoiceByBillingTargetGrid.getColIndexById('UnpaidAmount');
            var colUnpaidAmountUsd = grdListOfUnPaidInvoiceByBillingTargetGrid.getColIndexById('UnpaidAmountUsd');

            var TotalUnpaidAmount = 0;
            var TotalUnpaidAmountUsd = 0;

            //Add by Jutarat A. on 04032014
            curPage = grdListOfUnPaidInvoiceByBillingTargetGrid.currentPage;
            startRow = ((curPage * ROWS_PER_PAGE_FOR_VIEWPAGE) - ROWS_PER_PAGE_FOR_VIEWPAGE);
            endRow = ((curPage * ROWS_PER_PAGE_FOR_VIEWPAGE) - 1);
            //End Add

            //for (var i = 0; i < grdListOfUnPaidInvoiceByBillingTargetGrid.getRowsNum(); i++) {
            for (var i = startRow; i <= endRow && i < grdListOfUnPaidInvoiceByBillingTargetGrid.getRowsNum(); i++) { //Modify by Jutarat A. on 04032014

                var rowId = grdListOfUnPaidInvoiceByBillingTargetGrid.getRowId(i);

                //View register debt tracing info
                GenerateSelectButton(grdListOfUnPaidInvoiceByBillingTargetGrid, "btnViewRegisterDebtTracingInfo", rowId, "ViewRegisterDebtTracingInfo", true);
                BindGridButtonClickEvent("btnViewRegisterDebtTracingInfo", rowId, btnOpenICS032FromUnPaidInvoiceGrid);



                //Modify by Warakorn M. on 19/05/2014
                /*//No of billing detail
                var noOfBillingDetail = grdListOfUnPaidInvoiceByBillingTargetGrid.cells2(i, colNoOfBillingDetail).getValue();
                if (noOfBillingDetail != "" && noOfBillingDetail != "0") {
                var tagNoOfBillingDetail = '<a href="#" onclick="ListOfUnPaidInvoiceNoOfBillingClick(\'' + i + '\',\'' + grdListOfUnPaidInvoiceByBillingTargetGrid.cells2(i, colInvoiceNo).getValue() + '\',\'' + grdListOfUnPaidInvoiceByBillingTargetGrid.cells2(i, colInvoiceOCC).getValue() + '\')">' + noOfBillingDetail + '</a>';
                grdListOfUnPaidInvoiceByBillingTargetGrid.cells2(i, colNoOfBillingDetail).setValue(tagNoOfBillingDetail);
                }
                */

                var strListOfUnPaidInvoiceByBillingTarget = grdListOfUnPaidInvoiceByBillingTargetGrid.cells2(i, grdListOfUnPaidInvoiceByBillingTargetGrid.getColIndexById("ToJson")).getValue();
                var objListOfUnPaidInvoiceByBillingTarget = JSON.parse(htmlDecode(strListOfUnPaidInvoiceByBillingTarget));
                //No of billing detail
                if (objListOfUnPaidInvoiceByBillingTarget.NoOfBillingDetail != 0) {
                    var tagNoOfBillingDetail = '<a href="#" onclick="ListOfUnPaidInvoiceNoOfBillingClick(\'' + i + '\',\'' + objListOfUnPaidInvoiceByBillingTarget.InvoiceNo + '\',\'' + objListOfUnPaidInvoiceByBillingTarget.InvoiceOCC + '\')">' + objListOfUnPaidInvoiceByBillingTarget.NoOfBillingDetailString + '</a>';
                    grdListOfUnPaidInvoiceByBillingTargetGrid.cells2(i, colNoOfBillingDetail).setValue(tagNoOfBillingDetail);
                }


                //Set row color
                if (grdListOfUnPaidInvoiceByBillingTargetGrid.cells(rowId, colOldestInvoiceExpectedPaymentDateFlag).getValue() == '1') {
                    grdListOfUnPaidInvoiceByBillingTargetGrid.setRowColor(rowId, "#ff9999");
                }

                //Sum total
                var unpaidAmount = grdListOfUnPaidInvoiceByBillingTargetGrid.cells2(i, colUnpaidAmount).getValue();
                var unpaidAmountUsd = grdListOfUnPaidInvoiceByBillingTargetGrid.cells2(i, colUnpaidAmountUsd).getValue();
                if (unpaidAmount != undefined)
                    TotalUnpaidAmount = TotalUnpaidAmount + unpaidAmount;
                if (unpaidAmountUsd != undefined)
                    TotalUnpaidAmountUsd = TotalUnpaidAmountUsd + unpaidAmountUsd;
            }
            grdListOfUnPaidInvoiceByBillingTargetGrid.setSizes();

            //Set total
            $("#txtUnPaidInvoiceTotalUnpaidAmount").val(parseFloat(TotalUnpaidAmount).toFixed(2));
            $("#txtUnPaidInvoiceTotalUnpaidAmount").NumericCurrency().val(ICS030_ViewBag.C_CURRENCY_LOCAL);

            $("#txtUnPaidInvoiceTotalUnpaidAmountUsd").val(parseFloat(TotalUnpaidAmountUsd).toFixed(2));
            $("#txtUnPaidInvoiceTotalUnpaidAmountUsd").NumericCurrency().val(ICS030_ViewBag.C_CURRENCY_US);

            $("#txtUnPaidInvoiceTotalUnpaidAmount").setComma();
            $("#txtUnPaidInvoiceTotalUnpaidAmountUsd").setComma();
        });
    }


    // Initial grid 4
    if ($.find("#ICS030_ListOfUnPaidByBillingTargetGrid").length > 0) {
        grdListOfUnPaidByBillingTargetGrid = $("#ICS030_ListOfUnPaidByBillingTargetGrid").InitialGrid(ROWS_PER_PAGE_FOR_VIEWPAGE, true
            , "/Income/ICS030_InitialListOfUnPaidByBillingTargetGrid");

        SpecialGridControl(grdListOfUnPaidByBillingTargetGrid, ["FirstFee", "CreditNoteIssue", "ViewRegisterDebtTracingInfo2"]);
        BindOnLoadedEvent(grdListOfUnPaidByBillingTargetGrid, function () {
            var colViewRegisterDebtTracingInfo = grdListOfUnPaidByBillingTargetGrid.getColIndexById('ViewRegisterDebtTracingInfo2');
            var colFirstFeeFlag = grdListOfUnPaidByBillingTargetGrid.getColIndexById("FirstFeeFlag");
            var colCreditNoteIssuedFlag = grdListOfUnPaidByBillingTargetGrid.getColIndexById("CreditNoteIssuedFlag");
            var colBillingTypeCode = grdListOfUnPaidByBillingTargetGrid.getColIndexById("BillingTypeCode");
            var colBillingType = grdListOfUnPaidByBillingTargetGrid.getColIndexById("BillingType");
            var colPaymentStatusDesc = grdListOfUnPaidByBillingTargetGrid.getColIndexById("PaymentStatusDesc");
            var colPaymentStatus = grdListOfUnPaidByBillingTargetGrid.getColIndexById("PaymentStatus");
            var colBillingAmount = grdListOfUnPaidByBillingTargetGrid.getColIndexById("BillingAmount");
            var colBillingAmountUsd = grdListOfUnPaidByBillingTargetGrid.getColIndexById("BillingAmountUsd");

            var TotalUnpaidAmount = 0;
            var TotalUnpaidAmountUsd = 0;

            //Add by Jutarat A. on 04032014
            curPage = grdListOfUnPaidByBillingTargetGrid.currentPage;
            startRow = ((curPage * ROWS_PER_PAGE_FOR_VIEWPAGE) - ROWS_PER_PAGE_FOR_VIEWPAGE);
            endRow = ((curPage * ROWS_PER_PAGE_FOR_VIEWPAGE) - 1);
            //End Add

            //for (var i = 0; i < grdListOfUnPaidByBillingTargetGrid.getRowsNum(); i++) {
            for (var i = startRow; i <= endRow && i < grdListOfUnPaidByBillingTargetGrid.getRowsNum(); i++) { //Modify by Jutarat A. on 04032014

                var rowId = grdListOfUnPaidByBillingTargetGrid.getRowId(i);

                //First fee
                var dummyFirstFee = grdListOfUnPaidByBillingTargetGrid.cells(rowId, colFirstFeeFlag).getValue();
                if (dummyFirstFee == '1') {
                    GenerateFirstFeeButton(grdListOfUnPaidByBillingTargetGrid, "btnFirstFee", rowId, "FirstFee", true);
                }

                //Credit note issued
                var dummyResaultRegister = grdListOfUnPaidByBillingTargetGrid.cells(rowId, colCreditNoteIssuedFlag).getValue();
                if (dummyResaultRegister == '1') {
                    GenerateCreditNoteButton(grdListOfUnPaidByBillingTargetGrid, "btnCreditNote", rowId, "CreditNoteIssue", true);
                }

                //View register debt tracing info
                GenerateSelectButton(grdListOfUnPaidByBillingTargetGrid, "btnViewRegisterDebtTracingInfoByBilling", rowId, "ViewRegisterDebtTracingInfo2", true);
                BindGridButtonClickEvent("btnViewRegisterDebtTracingInfoByBilling", rowId, btnOpenICS032FromUnPaidByBillingGrid);


                //Tool tip
                var colToolTip1 = grdListOfUnPaidByBillingTargetGrid.cells(rowId, colBillingType).getValue();
                grdListOfUnPaidByBillingTargetGrid.cellById(rowId, colBillingTypeCode).setAttribute("title", colToolTip1);

                var colToolTip2 = grdListOfUnPaidByBillingTargetGrid.cells(rowId, colPaymentStatusDesc).getValue()
                grdListOfUnPaidByBillingTargetGrid.cellById(rowId, colPaymentStatus).setAttribute("title", colToolTip2);

                //Sum total
                var billingAmount = grdListOfUnPaidByBillingTargetGrid.cells2(i, colBillingAmount).getValue();
                var billingAmountUsd = grdListOfUnPaidByBillingTargetGrid.cells2(i, colBillingAmountUsd).getValue();
                if (billingAmount != undefined)
                    TotalUnpaidAmount = TotalUnpaidAmount + billingAmount;
                if (billingAmountUsd != undefined)
                    TotalUnpaidAmountUsd = TotalUnpaidAmountUsd + billingAmountUsd;

            }
            grdListOfUnPaidByBillingTargetGrid.setSizes();

            //Set total
            $("#txtUnPaidByBillingTotalUnpaidAmount").val(parseFloat(TotalUnpaidAmount).toFixed(2));
            $("#txtUnPaidByBillingTotalUnpaidAmountUsd").val(parseFloat(TotalUnpaidAmountUsd).toFixed(2));

            $("#txtUnPaidByBillingTotalUnpaidAmount").NumericCurrency().val(ICS030_ViewBag.C_CURRENCY_LOCAL);
            $("#txtUnPaidByBillingTotalUnpaidAmountUsd").NumericCurrency().val(ICS030_ViewBag.C_CURRENCY_US);

            $("#txtUnPaidByBillingTotalUnpaidAmount").setComma();
            $("#txtUnPaidByBillingTotalUnpaidAmountUsd").setComma();
        });
    }

    //Init Object Event
    // 1 Div Panel Body
    //    $("#btnSearch").click(btn_Search_click);
    //    $("#btnClear").click(btn_Clear_click);
    //    $("#btnDownloadFile").click(btn_Download_File_click);

    $("#cboMonthYear").change(change_MonthYear);

    //Initial Page
    InitialPage();
});

// reset form
function ResetDebtActualTableValue() {
    DeleteAllRow(grdDebtActualTableGrid);
}

function ResetListOfUnPaidBillingTargetByBillingOfficeValue() {
    $("#txtUnPaidBillingBillingOffice").val("");
    $("#txtUnPaidBillingBillingOfficeCode").val("");
    DeleteAllRow(grdListOfUnPaidBillingTargetByBillingOfficeGrid);
}

function ResetListOfUnPaidInvoiceByBillingTargetValue() {
    $("#txtUnPaidInvoiceBillingTargetCode").val("");
    $("#txtUnPaidInvoiceBillingOffice").val("");
    $("#txtUnPaidInvoiceBillingOfficeCode").val("");
    $("#txtUnPaidInvoiceBillingClientNameEN").val("");
    $("#txtUnPaidInvoiceBillingClientNameLC").val("");
    $("#txtUnPaidInvoiceBillingClientAddressEN").val("");
    $("#txtUnPaidInvoiceBillingClientAddressLC").val("");
    $("#txtUnPaidInvoiceBillingClientTelNo").val("");
    $("#txtUnPaidInvoiceBillingClientContactPerson").val("");
    $("#txtUnPaidInvoiceTotalUnpaidAmount").val("");
    $("#txtUnPaidInvoiceTotalUnpaidAmount").NumericCurrency().val(ICS030_ViewBag.C_CURRENCY_LOCAL);
    $("#txtUnPaidInvoiceTotalUnpaidAmountUsd").val("");
    $("#txtUnPaidInvoiceTotalUnpaidAmountUsd").NumericCurrency().val(ICS030_ViewBag.C_CURRENCY_US);

    DeleteAllRow(grdListOfUnPaidInvoiceByBillingTargetGrid);
}

function ResetListOfUnPaidByBillingTargetValue() {
    $("#txtUnPaidByBillingBillingTargetCode").val("");
    $("#txtUnPaidByBillingBillingOffice").val("");
    $("#txtUnPaidByBillingBillingOfficeCode").val("");
    $("#txtUnPaidByBillingBillingClientNameEN").val("");
    $("#txtUnPaidByBillingBillingClientNameLC").val("");
    $("#txtUnPaidByBillingBillingClientAddressEN").val("");
    $("#txtUnPaidByBillingBillingClientAddressLC").val("");
    $("#txtUnPaidByBillingBillingClientTelNo").val("");
    $("#txtUnPaidByBillingBillingClientContactPerson").val("");
    $("#txtUnPaidByBillingTotalUnpaidAmount").val("");
    $("#txtUnPaidByBillingTotalUnpaidAmount").NumericCurrency().val(ICS030_ViewBag.C_CURRENCY_LOCAL);
    $("#txtUnPaidByBillingTotalUnpaidAmountUsd").val("");
    $("#txtUnPaidByBillingTotalUnpaidAmountUsd").NumericCurrency().val(ICS030_ViewBag.C_CURRENCY_US);

    DeleteAllRow(grdListOfUnPaidByBillingTargetGrid);
}

//Modify by Jutarat A. on 05032014
//function change_MonthYear() {
//    var TotalUnpaidAmountString = 0;
//    var TotalUnpaidDetailString = 0;
//    var TotalTargetAmountAllString = 0;
//    var TotalTargetDetailAllString = 0;
//    var TotalAmountCompareToTargetAllString = 0;
//    var TotalDetailCompareToTargetAllString = 0;
//    var TotalUnpaidAmount2MonthString = 0;
//    var TotalUnpaidDetail2MonthString = 0;
//    var TotalTargetAmount2MonthString = 0;
//    var TotalTargetDetail2MonthString = 0;
//    var TotalAmountCompareToTarget2MonthString = 0;
//    var TotalDetailCompareToTarget2MonthString = 0;
//    var TotalUnpaidAmount6MonthString = 0;
//    var TotalUnpaidDetail6MonthString = 0;

//    $("#divDebtActualSummary").show();
//    $("#divDebtActualTable").show();

//    $("#divListOfUnPaidBillingTargetByBillingOffice").hide();
//    $("#divListOfUnPaidInvoiceByBillingTarget").hide();
//    $("#divListOfUnPaidByBillingTarget").hide();
//    $("#divListOfUnPaidByBillingTargetLabel1").hide();
//    $("#divListOfUnPaidByBillingTargetLabel2").hide();
//    // delete all value on screen of
//    ResetDebtActualTableValue();
//    ResetListOfUnPaidBillingTargetByBillingOfficeValue();
//    ResetListOfUnPaidInvoiceByBillingTargetValue();
//    ResetListOfUnPaidByBillingTargetValue();

//    _RawdtpMonthYear = $("#cboMonthYear").val();


//    // check all input on Server
//    var obj = GetUserAdjustData();
//    ajax_method.CallScreenController("/Income/ICS030_LoadGetBillingOfficeDebtSummaryData", obj, function (result, controls, isWarning) {
//        if (result != undefined) {

//            // goto Idel state
//            // Wired design
//            //setVisableTable(conYes);
//            //setFormMode(conModeView);
//            //            bolViewMode = true;

//            conYes = result.conYes;
//            conNo = result.conNo;

//            _doGetBillingOfficeDebtSummaryList = result.doGetBillingOfficeDebtSummaryList;
//            _doOfficeDataDo = result.doOfficeDataDo;

//            if (_doGetBillingOfficeDebtSummaryList != null) {
//                for (var i = 0; i < _doGetBillingOfficeDebtSummaryList.length; ++i) {
//                    TotalUnpaidAmountString = TotalUnpaidAmountString + _doGetBillingOfficeDebtSummaryList[i].UnpaidAmount;
//                    TotalUnpaidDetailString = TotalUnpaidDetailString + _doGetBillingOfficeDebtSummaryList[i].UnpaidDetail;
//                    TotalTargetAmountAllString = TotalTargetAmountAllString + _doGetBillingOfficeDebtSummaryList[i].TargetAmountAll;
//                    TotalTargetDetailAllString = TotalTargetDetailAllString + _doGetBillingOfficeDebtSummaryList[i].TargetDetailAll;
//                    TotalAmountCompareToTargetAllString = TotalAmountCompareToTargetAllString + _doGetBillingOfficeDebtSummaryList[i].AmountCompareToTargetAll;
//                    TotalDetailCompareToTargetAllString = TotalDetailCompareToTargetAllString + _doGetBillingOfficeDebtSummaryList[i].DetailCompareToTargetAll;
//                    TotalUnpaidAmount2MonthString = TotalUnpaidAmount2MonthString + _doGetBillingOfficeDebtSummaryList[i].UnpaidAmount2Month;
//                    TotalUnpaidDetail2MonthString = TotalUnpaidDetail2MonthString + _doGetBillingOfficeDebtSummaryList[i].UnpaidDetail2Month;
//                    TotalTargetAmount2MonthString = TotalTargetAmount2MonthString + _doGetBillingOfficeDebtSummaryList[i].TargetAmount2Month;
//                    TotalTargetDetail2MonthString = TotalTargetDetail2MonthString + _doGetBillingOfficeDebtSummaryList[i].TargetDetail2Month;
//                    TotalAmountCompareToTarget2MonthString = TotalAmountCompareToTarget2MonthString + _doGetBillingOfficeDebtSummaryList[i].AmountCompareToTarget2Month;
//                    TotalDetailCompareToTarget2MonthString = TotalDetailCompareToTarget2MonthString + _doGetBillingOfficeDebtSummaryList[i].DetailCompareToTarget2Month;
//                    TotalUnpaidAmount6MonthString = TotalUnpaidAmount6MonthString + _doGetBillingOfficeDebtSummaryList[i].UnpaidAmount6Month;
//                    TotalUnpaidDetail6MonthString = TotalUnpaidDetail6MonthString + _doGetBillingOfficeDebtSummaryList[i].UnpaidDetail6Month;

//                    Add_DebtActualTableGridBlankLine(i + 1, _doGetBillingOfficeDebtSummaryList[i], _doOfficeDataDo);
//                }
//                //TotalTargetAmountAllString = (TotalTargetAmountAllString / _doGetBillingOfficeDebtSummaryList.length);
//                //TotalTargetDetailAllString = (TotalTargetDetailAllString / _doGetBillingOfficeDebtSummaryList.length);
//            };

//            Add_DebtActualTableGridTotalLine(TotalUnpaidAmountString, TotalUnpaidDetailString, TotalTargetAmountAllString
//                , TotalTargetDetailAllString, TotalAmountCompareToTargetAllString, TotalDetailCompareToTargetAllString
//                , TotalUnpaidAmount2MonthString, TotalUnpaidDetail2MonthString, TotalTargetAmount2MonthString
//                , TotalTargetDetail2MonthString, TotalAmountCompareToTarget2MonthString, TotalDetailCompareToTarget2MonthString
//                , TotalUnpaidAmount6MonthString, TotalUnpaidDetail6MonthString);

//            master_event.ScrollWindow("#divDebtActualTable");
//        }
//        else if (controls != undefined) {
//            VaridateCtrl(controls, controls);
//        }
//    });
//}


//function change_MonthYear_FirstTime() {
//    var TotalUnpaidAmountString = 0;
//    var TotalUnpaidDetailString = 0;
//    var TotalTargetAmountAllString = 0;
//    var TotalTargetDetailAllString = 0;
//    var TotalAmountCompareToTargetAllString = 0;
//    var TotalDetailCompareToTargetAllString = 0;
//    var TotalUnpaidAmount2MonthString = 0;
//    var TotalUnpaidDetail2MonthString = 0;
//    var TotalTargetAmount2MonthString = 0;
//    var TotalTargetDetail2MonthString = 0;
//    var TotalAmountCompareToTarget2MonthString = 0;
//    var TotalDetailCompareToTarget2MonthString = 0;
//    var TotalUnpaidAmount6MonthString = 0;
//    var TotalUnpaidDetail6MonthString = 0;

//    $("#divDebtActualSummary").show();
//    $("#divDebtActualTable").show();

//    $("#divListOfUnPaidBillingTargetByBillingOffice").hide();
//    $("#divListOfUnPaidInvoiceByBillingTarget").hide();
//    $("#divListOfUnPaidByBillingTarget").hide();
//    $("#divListOfUnPaidByBillingTargetLabel1").hide();
//    $("#divListOfUnPaidByBillingTargetLabel2").hide();

//    _RawdtpMonthYear = $("#cboMonthYear").val();


//    // check all input on Server
//    var obj = GetUserAdjustData();
//    ajax_method.CallScreenController("/Income/ICS030_LoadGetBillingOfficeDebtSummaryData", obj, function (result, controls, isWarning) {
//        if (result != undefined) {

//            //setFormMode(conModeView);

//            conYes = result.conYes;
//            conNo = result.conNo;

//            _doGetBillingOfficeDebtSummaryList = result.doGetBillingOfficeDebtSummaryList;
//            _doOfficeDataDo = result.doOfficeDataDo;

//            if (_doGetBillingOfficeDebtSummaryList != null) {
//                for (var i = 0; i < _doGetBillingOfficeDebtSummaryList.length; ++i) {
//                    TotalUnpaidAmountString = TotalUnpaidAmountString + _doGetBillingOfficeDebtSummaryList[i].UnpaidAmount;
//                    TotalUnpaidDetailString = TotalUnpaidDetailString + _doGetBillingOfficeDebtSummaryList[i].UnpaidDetail;
//                    TotalTargetAmountAllString = TotalTargetAmountAllString + _doGetBillingOfficeDebtSummaryList[i].TargetAmountAll;
//                    TotalTargetDetailAllString = TotalTargetDetailAllString + _doGetBillingOfficeDebtSummaryList[i].TargetDetailAll;
//                    TotalAmountCompareToTargetAllString = TotalAmountCompareToTargetAllString + _doGetBillingOfficeDebtSummaryList[i].AmountCompareToTargetAll;
//                    TotalDetailCompareToTargetAllString = TotalDetailCompareToTargetAllString + _doGetBillingOfficeDebtSummaryList[i].DetailCompareToTargetAll;
//                    TotalUnpaidAmount2MonthString = TotalUnpaidAmount2MonthString + _doGetBillingOfficeDebtSummaryList[i].UnpaidAmount2Month;
//                    TotalUnpaidDetail2MonthString = TotalUnpaidDetail2MonthString + _doGetBillingOfficeDebtSummaryList[i].UnpaidDetail2Month;
//                    TotalTargetAmount2MonthString = TotalTargetAmount2MonthString + _doGetBillingOfficeDebtSummaryList[i].TargetAmount2Month;
//                    TotalTargetDetail2MonthString = TotalTargetDetail2MonthString + _doGetBillingOfficeDebtSummaryList[i].TargetDetail2Month;
//                    TotalAmountCompareToTarget2MonthString = TotalAmountCompareToTarget2MonthString + _doGetBillingOfficeDebtSummaryList[i].AmountCompareToTarget2Month;
//                    TotalDetailCompareToTarget2MonthString = TotalDetailCompareToTarget2MonthString + _doGetBillingOfficeDebtSummaryList[i].DetailCompareToTarget2Month;
//                    TotalUnpaidAmount6MonthString = TotalUnpaidAmount6MonthString + _doGetBillingOfficeDebtSummaryList[i].UnpaidAmount6Month;
//                    TotalUnpaidDetail6MonthString = TotalUnpaidDetail6MonthString + _doGetBillingOfficeDebtSummaryList[i].UnpaidDetail6Month;

//                    Add_DebtActualTableGridBlankLine(i + 1, _doGetBillingOfficeDebtSummaryList[i], _doOfficeDataDo);
//                }
//                //                TotalTargetAmountAllString = (TotalTargetAmountAllString / _doGetBillingOfficeDebtSummaryList.length);
//                //                TotalTargetDetailAllString = (TotalTargetDetailAllString / _doGetBillingOfficeDebtSummaryList.length);
//            };

//            Add_DebtActualTableGridTotalLine(TotalUnpaidAmountString, TotalUnpaidDetailString, TotalTargetAmountAllString
//                , TotalTargetDetailAllString, TotalAmountCompareToTargetAllString, TotalDetailCompareToTargetAllString
//                , TotalUnpaidAmount2MonthString, TotalUnpaidDetail2MonthString, TotalTargetAmount2MonthString
//                , TotalTargetDetail2MonthString, TotalAmountCompareToTarget2MonthString, TotalDetailCompareToTarget2MonthString
//                , TotalUnpaidAmount6MonthString, TotalUnpaidDetail6MonthString);

//            master_event.ScrollWindow("#divDebtActualTable");
//        }
//        else if (controls != undefined) {
//            VaridateCtrl(controls, controls);
//        }
//    });
//}

function change_MonthYear() {
    $("#divDebtActualSummary").show();
    $("#divDebtActualTable").show();

    $("#divListOfUnPaidBillingTargetByBillingOffice").hide();
    $("#divListOfUnPaidInvoiceByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTargetLabel1").hide();
    $("#divListOfUnPaidByBillingTargetLabel2").hide();
    // delete all value on screen of
    ResetDebtActualTableValue();
    ResetListOfUnPaidBillingTargetByBillingOfficeValue();
    ResetListOfUnPaidInvoiceByBillingTargetValue();
    ResetListOfUnPaidByBillingTargetValue();

    _RawdtpMonthYear = $("#cboMonthYear").val();

    // check all input on Server
    var obj = GetUserAdjustData();

    $("#ICS030_DebtActualTableGrid").LoadDataToGrid(grdDebtActualTableGrid, 0, true, "/Income/ICS030_LoadGetBillingOfficeDebtSummaryToGrid",
                                        obj, "ICS030_DebtActualTableData", false,

                                        function (result, controls, isWarning) {
                                            if (controls != undefined) {
                                                VaridateCtrl(controls, controls);
                                            }
                                            else {

                                                ajax_method.CallScreenController("/Income/ICS030_LoadGetBillingOfficeDebtSummaryData", obj, function (result, controls, isWarning) {
                                                    if (result != undefined) {

                                                        conYes = result.conYes;
                                                        conNo = result.conNo;

                                                        _doGetBillingOfficeDebtSummaryList = result.doGetBillingOfficeDebtSummaryList;
                                                        _doNewGetBillingOfficeDebtSummaryList = result.doNewGetBillingOfficeDebtSummaryList;
                                                        _doOfficeDataDo = result.doOfficeDataDo;

                                                        // Comment by Jirawat Jannet
                                                        //if (_doGetBillingOfficeDebtSummaryList != null) {
                                                        //    Add_DebtActualTableGridTotal(result.doTotalBillingOfficeDebt);
                                                        //    master_event.ScrollWindow("#divDebtActualTable");
                                                        //}

                                                        // Add by Jirawat Jannet
                                                        if (_doNewGetBillingOfficeDebtSummaryList != null) {
                                                            Add_DebtActualTableGridTotalNew(result.doTotalBillingOfficeDebtLocal);
                                                            Add_DebtActualTableGridTotalNew(result.doTotalBillingOfficeDebtUs);

                                                            var LastRowIndex1 = grdDebtActualTableGrid.getRowsNum() - 2;
                                                            var LastRowIndex2 = grdDebtActualTableGrid.getRowsNum() - 1;


                                                            grdDebtActualTableGrid.cells2(LastRowIndex1, 1).cell.style.display = 'none';
                                                            grdDebtActualTableGrid.cells2(LastRowIndex2, 0).cell.style.display = 'none';
                                                            grdDebtActualTableGrid.cells2(LastRowIndex2, 1).cell.style.display = 'none';

                                                            grdDebtActualTableGrid.cells2(LastRowIndex1, 0).setValue('<b style=\'font-size:9pt\'>' + ICS030_ViewBag.lblTotal + '</b>');
                                                            grdDebtActualTableGrid.cells2(LastRowIndex1, 0).cell.colSpan = 2;
                                                            grdDebtActualTableGrid.cells2(LastRowIndex1, 0).cell.rowSpan = 2;
                                                            grdDebtActualTableGrid.cells2(LastRowIndex1, 0).cell.style.verticalAlign = 'middle';
                                                            grdDebtActualTableGrid.cells2(LastRowIndex1, 0).cell.style.textAlign = 'right';


                                                            grdDebtActualTableGrid.cells2(LastRowIndex1, 2).cell.style.verticalAlign = 'middle';
                                                            grdDebtActualTableGrid.cells2(LastRowIndex2, 2).cell.style.verticalAlign = 'middle';



                                                            master_event.ScrollWindow("#divDebtActualTable");
                                                        }
                                                    }
                                                    else if (controls != undefined) {
                                                        VaridateCtrl(controls, controls);
                                                    }
                                                });

                                            }

                                            // add by jirawat jannet @ 2016-10-10
                                            // set table rowspan
                                            setGridRowSpan(grdDebtActualTableGrid);

                                            
                                        }
                                        , null
                                    );

}

// Add by Jirawat Jannet @ 2016-10-10
function setGridRowSpan(grid) {

    // hide column before set row span
    var rowIndex = 1;
    var groupIndex = 1;
    for (var i = 0 ; i < grid.getRowsNum() ; i++) {
        
        if ((rowIndex - 1) % 4 == 0) {
            var billingColIndex = grid.getColIndexById('BillingOffice');
            var noIndex = grid.getColIndexById('No');


            grid.cells2(i, billingColIndex).cell.rowSpan = 4;
            //grid.cells2(i, billingColIndex).cell.className = 'customHeader';
            grid.cells2(i, billingColIndex).cell.style.backgroundColor = '#d1ecfa';

            grid.cells2(i, noIndex).cell.rowSpan = 4;
            //grid.cells2(i, noIndex).cell.className = 'customHeader';
            grid.cells2(i, noIndex).cell.style.backgroundColor = '#d1ecfa';
            grid.cells2(i, noIndex).setValue(groupIndex);

            groupIndex = groupIndex + 1;
        } else {
            var billingColIndex = grid.getColIndexById('BillingOffice');
            var noIndex = grid.getColIndexById('No');

            grid.cells2(i, billingColIndex).cell.style.display = 'none';
            grid.cells2(i, noIndex).cell.style.display = 'none';
        }

        if (rowIndex % 4 == 1 || rowIndex % 4 == 3) {
            var currencyIndex = grid.getColIndexById('Currency');
            grid.cells2(i, currencyIndex).cell.rowSpan = 2;
            grid.cells2(i, currencyIndex).cell.style.verticalAlign = 'middle';
        } else {
            var currencyIndex = grid.getColIndexById('Currency');
            grid.cells2(i, currencyIndex).cell.style.display = 'none';
        }


        rowIndex = rowIndex + 1;
    }

}

function change_MonthYear_FirstTime() {
    $("#divDebtActualSummary").show();
    $("#divDebtActualTable").show();

    $("#divListOfUnPaidBillingTargetByBillingOffice").hide();
    $("#divListOfUnPaidInvoiceByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTargetLabel1").hide();
    $("#divListOfUnPaidByBillingTargetLabel2").hide();

    _RawdtpMonthYear = $("#cboMonthYear").val();

    // check all input on Server
    var obj = GetUserAdjustData();

    grdDebtActualTableGrid = $("#ICS030_DebtActualTableGrid").LoadDataToGridWithInitial(0, false, true, "/Income/ICS030_LoadGetBillingOfficeDebtSummaryToGrid",
                                obj, "ICS030_DebtActualTableData", false, null, null,

                                function (result, controls, isWarning) {
                                    if (controls != undefined) {
                                        VaridateCtrl(controls, controls);
                                    }
                                    else {

                                        ajax_method.CallScreenController("/Income/ICS030_LoadGetBillingOfficeDebtSummaryData", obj, function (result, controls, isWarning) {
                                            if (result != undefined) {

                                                conYes = result.conYes;
                                                conNo = result.conNo;

                                                _doGetBillingOfficeDebtSummaryList = result.doGetBillingOfficeDebtSummaryList;
                                                _doNewGetBillingOfficeDebtSummaryList = result.doNewGetBillingOfficeDebtSummaryList;
                                                _doOfficeDataDo = result.doOfficeDataDo;

                                                // Comment by Jirawat Jannet
                                                //if (_doGetBillingOfficeDebtSummaryList != null) {
                                                //    Add_DebtActualTableGridTotal(result.doTotalBillingOfficeDebt);
                                                //    master_event.ScrollWindow("#divDebtActualTable");
                                                //}

                                                // Add by Jirawat Jannet
                                                if (_doNewGetBillingOfficeDebtSummaryList != null) {
                                                    Add_DebtActualTableGridTotalNew(result.doTotalBillingOfficeDebtLocal);
                                                    Add_DebtActualTableGridTotalNew(result.doTotalBillingOfficeDebtUs);

                                                    var LastRowIndex1 = grdDebtActualTableGrid.getRowsNum() - 2;
                                                    var LastRowIndex2 = grdDebtActualTableGrid.getRowsNum() - 1;


                                                    grdDebtActualTableGrid.cells2(LastRowIndex1, 1).cell.style.display = 'none';
                                                    grdDebtActualTableGrid.cells2(LastRowIndex2, 0).cell.style.display = 'none';
                                                    grdDebtActualTableGrid.cells2(LastRowIndex2, 1).cell.style.display = 'none';

                                                    grdDebtActualTableGrid.cells2(LastRowIndex1, 0).setValue('<b style=\'font-size:9pt\'>' + ICS030_ViewBag.lblTotal + '</b>');
                                                    grdDebtActualTableGrid.cells2(LastRowIndex1, 0).cell.colSpan = 2;
                                                    grdDebtActualTableGrid.cells2(LastRowIndex1, 0).cell.rowSpan = 2;
                                                    grdDebtActualTableGrid.cells2(LastRowIndex1, 0).cell.style.verticalAlign = 'middle';
                                                    grdDebtActualTableGrid.cells2(LastRowIndex1, 0).cell.style.textAlign = 'right';


                                                    grdDebtActualTableGrid.cells2(LastRowIndex1, 2).cell.style.verticalAlign = 'middle';
                                                    grdDebtActualTableGrid.cells2(LastRowIndex2, 2).cell.style.verticalAlign = 'middle';



                                                    master_event.ScrollWindow("#divDebtActualTable");
                                                }
                                            }
                                            else if (controls != undefined) {
                                                VaridateCtrl(controls, controls);
                                            }
                                        });

                                    }


                                    // add by jirawat jannet @ 2016-10-10
                                    // set table rowspan
                                    setGridRowSpan(grdDebtActualTableGrid);

                                }

                            );

    SpecialGridControl(grdDebtActualTableGrid, ["BillingOffice"]);
    BindOnLoadedEvent(grdDebtActualTableGrid, function () {

        var colBillingOffice = grdDebtActualTableGrid.getColIndexById('BillingOffice');
        var colBillingOfficeCode = grdDebtActualTableGrid.getColIndexById('BillingOfficeCode');
        var colBillingOfficeName = grdDebtActualTableGrid.getColIndexById('BillingOfficeName');
        var colDisableLinkOfficeFlag = grdDebtActualTableGrid.getColIndexById('DisableLinkOfficeFlag');

        var billingOffice = "";
        var billingOfficeCode = "";
        var billingOfficeName = "";
        var strDisableLinkOfficeFlag = "";
        var tempLink = "";

        for (var i = 0; i < grdDebtActualTableGrid.getRowsNum() ; i++) {

            if (i % 4 == 0) {
                var rowId = grdDebtActualTableGrid.getRowId(i);

                billingOffice = grdDebtActualTableGrid.cells2(i, colBillingOffice).getValue();
                billingOfficeCode = grdDebtActualTableGrid.cells2(i, colBillingOfficeCode).getValue();
                billingOfficeName = grdDebtActualTableGrid.cells2(i, colBillingOfficeName).getValue();
                strDisableLinkOfficeFlag = grdDebtActualTableGrid.cells2(i, colDisableLinkOfficeFlag).getValue();

                DisableLinkOfficeFlag = strDisableLinkOfficeFlag == "1";

                tempLink = '<a href="#" onclick="DebtActualTableOfficeClick(\'' + i + '\',\'' + billingOfficeCode + '\',\'' + billingOfficeName + '\')">' + billingOfficeName + '</a>';

                // if select first index of combo = this month
                // when not select first month then gen no link
                if ($("#cboMonthYear")[0][0].selected == false || DisableLinkOfficeFlag == true) {
                    tempLink = billingOfficeName;
                }

                grdDebtActualTableGrid.cells2(i, colBillingOffice).setValue(billingOfficeCode + '<BR> ' + tempLink);
            }
        }


        grdDebtActualTableGrid.setSizes();
    });
}
//End Modify

//Add by Jutarat A. on 06032012
function Add_DebtActualTableGridTotal(_doTotalBillingOfficeDebt) {
    if (_doTotalBillingOfficeDebt != undefined && _doTotalBillingOfficeDebt != null) {
        CheckFirstRowIsEmpty(grdDebtActualTableGrid, true);

        AddNewRow(grdDebtActualTableGrid, ["",
                '<b style=\'font-size:9pt\'>' + ICS030_ViewBag.lblTotal + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalUnpaidAmountString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalUnpaidDetailString + '</b>',
                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalTargetAmountAllString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalTargetDetailAllString + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TargetAmountAllString
                + '<BR>' + _doTotalBillingOfficeDebt.TargetDetailAllString + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalUnpaidAmount2MonthString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalUnpaidDetail2MonthString + '</b>',
                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalTargetAmount2MonthString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalTargetDetail2MonthString + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TargetAmount2MonthString
                + '<BR>' + _doTotalBillingOfficeDebt.TargetDetail2MonthString + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalUnpaidAmount6MonthString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalUnpaidDetail6MonthString + '</b>'
                                                ]);
        grdDebtActualTableGrid.setSizes();
    }
}
//End Add

// Add by Jirawat Jannet @ 2016-10-10
function Add_DebtActualTableGridTotalNew(_doTotalBillingOfficeDebt) {

    AddNewRow(grdDebtActualTableGrid, ["",
                '<b style=\'font-size:9pt\'>' + ICS030_ViewBag.lblTotal + '</b>',
                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.Currency + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalUnpaidAmountString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalUnpaidDetailString + '</b>',
                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalTargetAmountAllString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalTargetDetailAllString + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TargetAmountAllString
                + '<BR>' + _doTotalBillingOfficeDebt.TargetDetailAllString + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalUnpaidAmount2MonthString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalUnpaidDetail2MonthString + '</b>',
                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalTargetAmount2MonthString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalTargetDetail2MonthString + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TargetAmount2MonthString
                + '<BR>' + _doTotalBillingOfficeDebt.TargetDetail2MonthString + '</b>',

                '<b style=\'font-size:9pt\'>' + _doTotalBillingOfficeDebt.TotalUnpaidAmount6MonthString
                + '<BR>' + _doTotalBillingOfficeDebt.TotalUnpaidDetail6MonthString + '</b>'
    ]);
}

// Grid Function
function Add_DebtActualTableGridBlankLine(intRow, _doGetBillingOfficeDebtSummaryList, _doOfficeDataDo) {

    var tempLink;
    //Modified by budd, support multi-language
    //var tempLink = '<a href="#" onclick="DebtActualTableOfficeClick(\'' + intRow + '\',\'' + _doGetBillingOfficeDebtSummaryList.BillingOfficeCode + '\',\'' + _doGetBillingOfficeDebtSummaryList.BiilingOfficeNameEN + '\')">' + _doGetBillingOfficeDebtSummaryList.BiilingOfficeNameEN + '</a>';
    tempLink = '<a href="#" onclick="DebtActualTableOfficeClick(\'' + intRow + '\',\'' + _doGetBillingOfficeDebtSummaryList.BillingOfficeCode + '\',\'' + _doGetBillingOfficeDebtSummaryList.BiilingOfficeName + '\')">' + _doGetBillingOfficeDebtSummaryList.BiilingOfficeName + '</a>';

    var OfficeFlag;
    DisableLinkOfficeFlag = true;

    for (var i = 0; i < _doOfficeDataDo.length; ++i) {

        if (_doGetBillingOfficeDebtSummaryList.BillingOfficeCode == _doOfficeDataDo[i].OfficeCode) {
            DisableLinkOfficeFlag = false;
            break;
        }
    }

    // if select first index of combo = this month
    // when not select first month then gen no link
    if ($("#cboMonthYear")[0][0].selected == false || DisableLinkOfficeFlag == true) {
        tempLink = _doGetBillingOfficeDebtSummaryList.BiilingOfficeName;
    }

    var targetAmountAll = "";
    var targetDetailAll = "";
    var targetAmount2Month = "";
    var targetDetail2Month = "";

    if (_doGetBillingOfficeDebtSummaryList.TargetAmountAll > 0) {
        targetAmountAll = number_format(parseFloat(((_doGetBillingOfficeDebtSummaryList.UnpaidAmount * 100) / _doGetBillingOfficeDebtSummaryList.TargetAmountAll)), 1, '.', ',').toString();
    }
    if (_doGetBillingOfficeDebtSummaryList.TargetDetailAll > 0) {
        targetDetailAll = number_format(parseFloat(((_doGetBillingOfficeDebtSummaryList.UnpaidDetail * 100) / _doGetBillingOfficeDebtSummaryList.TargetDetailAll)), 1, '.', ',').toString();
    }
    if (_doGetBillingOfficeDebtSummaryList.TargetAmount2Month > 0) {
        targetAmount2Month = number_format(parseFloat(((_doGetBillingOfficeDebtSummaryList.UnpaidAmount2Month * 100) / _doGetBillingOfficeDebtSummaryList.TargetAmount2Month)), 1, '.', ',').toString();
    }
    if (_doGetBillingOfficeDebtSummaryList.TargetDetail2Month > 0) {
        targetDetail2Month = number_format(parseFloat(((_doGetBillingOfficeDebtSummaryList.UnpaidDetail2Month * 100) / _doGetBillingOfficeDebtSummaryList.TargetDetail2Month)), 1, '.', ',').toString();
    }

    CheckFirstRowIsEmpty(grdDebtActualTableGrid, true);

    AddNewRow(grdDebtActualTableGrid, [intRow,
    _doGetBillingOfficeDebtSummaryList.BillingOfficeCode
    + '<BR> ' + tempLink,

    _doGetBillingOfficeDebtSummaryList.UnpaidAmountString
    + '<BR>' + _doGetBillingOfficeDebtSummaryList.UnpaidDetailString,
    _doGetBillingOfficeDebtSummaryList.TargetAmountAllString
    + '<BR>' + _doGetBillingOfficeDebtSummaryList.TargetDetailAllString,
    targetAmountAll
    + '<BR>' + targetDetailAll,
    //    '(1) ' + _doGetBillingOfficeDebtSummaryList.AmountCompareToTargetAllString
    //+ '<BR>(2) ' + _doGetBillingOfficeDebtSummaryList.DetailCompareToTargetAllString,

    _doGetBillingOfficeDebtSummaryList.UnpaidAmount2MonthString
    + '<BR>' + _doGetBillingOfficeDebtSummaryList.UnpaidDetail2MonthString,
    _doGetBillingOfficeDebtSummaryList.TargetAmount2MonthString
    + '<BR>' + _doGetBillingOfficeDebtSummaryList.TargetDetail2MonthString,
    targetAmount2Month
    + '<BR>' + targetDetail2Month,
    //     '(1) ' + _doGetBillingOfficeDebtSummaryList.AmountCompareToTarget2MonthString
    //+ '<BR>(2) ' + _doGetBillingOfficeDebtSummaryList.DetailCompareToTarget2MonthString,

    _doGetBillingOfficeDebtSummaryList.UnpaidAmount6MonthString
    + '<BR>' + _doGetBillingOfficeDebtSummaryList.UnpaidDetail6MonthString
                                             ]);
    grdDebtActualTableGrid.setSizes();
    //_doGetBillingOfficeDebtSummaryList(ConvertDateObject(doGetMoneyCollectionManagementInfo.ReceiptDate)),

    //OldestDelayMonth

}

function Add_DebtActualTableGridTotalLine(TotalUnpaidAmountString, TotalUnpaidDetailString, TotalTargetAmountAllString
    , TotalTargetDetailAllString, TotalAmountCompareToTargetAllString, TotalDetailCompareToTargetAllString
    , TotalUnpaidAmount2MonthString, TotalUnpaidDetail2MonthString, TotalTargetAmount2MonthString
    , TotalTargetDetail2MonthString, TotalAmountCompareToTarget2MonthString, TotalDetailCompareToTarget2MonthString
    , TotalUnpaidAmount6MonthString, TotalUnpaidDetail6MonthString) {

    var targetAmountAll = "";
    var targetDetailAll = "";
    var targetAmount2Month = "";
    var targetDetail2Month = "";

    if (TotalTargetAmountAllString > 0) {
        targetAmountAll = number_format(parseFloat(((TotalUnpaidAmountString * 100) / TotalTargetAmountAllString)), 1, '.', ',').toString();
    }
    if (TotalTargetDetailAllString > 0) {
        targetDetailAll = number_format(parseFloat(((TotalUnpaidDetailString * 100) / TotalTargetDetailAllString)), 1, '.', ',').toString();
    }
    if (TotalTargetAmount2MonthString > 0) {
        targetAmount2Month = number_format(parseFloat(((TotalUnpaidAmount2MonthString * 100) / TotalTargetAmount2MonthString)), 1, '.', ',').toString();
    }
    if (TotalTargetDetail2MonthString > 0) {
        targetDetail2Month = number_format(parseFloat(((TotalUnpaidDetail2MonthString * 100) / TotalTargetDetail2MonthString)), 1, '.', ',').toString();
    }

    CheckFirstRowIsEmpty(grdDebtActualTableGrid, true);


    AddNewRow(grdDebtActualTableGrid, ["",
    '<b style=\'font-size:9pt\'>' + ICS030_ViewBag.lblTotal + '</b>',

    '<b style=\'font-size:9pt\'>' + number_format(parseFloat(TotalUnpaidAmountString), 2, '.', ',')
    + '<BR>' + number_format(parseFloat(TotalUnpaidDetailString), 0, '.', ',') + '</b>',
    '<b style=\'font-size:9pt\'>' + number_format(parseFloat(TotalTargetAmountAllString), 2, '.', ',')
    + '<BR>' + number_format(parseFloat(TotalTargetDetailAllString), 0, '.', ',') + '</b>',

    '<b style=\'font-size:9pt\'>' + targetAmountAll
    + '<BR>' + targetDetailAll + '</b>',

    '<b style=\'font-size:9pt\'>' + number_format(parseFloat(TotalUnpaidAmount2MonthString), 2, '.', ',')
    + '<BR>' + number_format(parseFloat(TotalUnpaidDetail2MonthString), 0, '.', ',') + '</b>',
    '<b style=\'font-size:9pt\'>' + number_format(parseFloat(TotalTargetAmount2MonthString), 2, '.', ',')
    + '<BR>' + number_format(parseFloat(TotalTargetDetail2MonthString), 0, '.', ',') + '</b>',

    '<b style=\'font-size:9pt\'>' + targetAmount2Month
    + '<BR>' + targetDetail2Month + '</b>',

    '<b style=\'font-size:9pt\'>' + number_format(parseFloat(TotalUnpaidAmount6MonthString), 2, '.', ',')
    + '<BR>' + number_format(parseFloat(TotalUnpaidDetail6MonthString), 0, '.', ',') + '</b>'
                                             ]);
    grdDebtActualTableGrid.setSizes();
}

function DebtActualTableOfficeClick(intRow, strOfficeCode, strOfficeName) {
    _strOfficeCode = strOfficeCode;
    _strOfficeName = strOfficeName;
    

    //$("divDebtActualSummary").show();
    //$("divDebtActualTable").show();

    $("#divListOfUnPaidBillingTargetByBillingOffice").show();
    $("#divListOfUnPaidInvoiceByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTargetLabel1").hide();
    $("#divListOfUnPaidByBillingTargetLabel2").hide();

    // delete all value on screen of
    //ResetDebtActualTableValue();
    ResetListOfUnPaidBillingTargetByBillingOfficeValue();
    ResetListOfUnPaidInvoiceByBillingTargetValue();
    ResetListOfUnPaidByBillingTargetValue();

    $("#txtUnPaidBillingBillingOffice").val(strOfficeName);
    $("#txtUnPaidBillingBillingOfficeCode").val(strOfficeCode);

    // check all input on Server
    var obj = GetUserAdjustData(); 
    $("#ICS030_ListOfUnPaidBillingTargetByBillingOfficeGrid").LoadDataToGrid(grdListOfUnPaidBillingTargetByBillingOfficeGrid
        , ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Income/ICS030_LoadGetBillingTargetDebtSummaryByOfficeData", obj, "doGetBillingTargetDebtSummaryByOffice", false,
                    function (result, controls, isWarning) {
                        if (controls != undefined) {
                            VaridateCtrl(controls, controls);
                        }
                        else {
                            document.getElementById('divListOfUnPaidBillingTargetByBillingOfficeBody').scrollIntoView();
                        }
                    },
                    function (result, controls, isWarning) {
                        if (isWarning == undefined) {
                            $("#divListOfUnPaidBillingTargetByBillingOfficeBody").show();
                        }
                    });
}

function UnPaidBillingTargetUnpaidInvoiceClick(rowIndex, billingTargetCode) {
    _UnPaidBLByBLOffice_RowIndex = rowIndex;
    _strBillingTargetCode = billingTargetCode;
   
    $("#divListOfUnPaidInvoiceByBillingTarget").show();
    $("#divListOfUnPaidByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTargetLabel1").hide();
    $("#divListOfUnPaidByBillingTargetLabel2").hide();

    // delete all value on screen of
    //ResetDebtActualTableValue();
    //ResetListOfUnPaidBillingTargetByBillingOfficeValue();
    ResetListOfUnPaidInvoiceByBillingTargetValue();
    ResetListOfUnPaidByBillingTargetValue();

    
    grdListOfUnPaidBillingTargetByBillingOfficeGrid.selectRow(rowIndex);
    var strBillingTargetDebtSummaryByOffice = grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(rowIndex, grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById("ToJson")).getValue();
    var objBillingTargetDebtSummaryByOffice = JSON.parse(htmlDecode(strBillingTargetDebtSummaryByOffice));

    $("#txtUnPaidInvoiceBillingTargetCode").val(billingTargetCode);
    $("#txtUnPaidInvoiceBillingOffice").val(_strOfficeName);
    $("#txtUnPaidInvoiceBillingOfficeCode").val(_strOfficeCode);

    $("#txtUnPaidInvoiceBillingClientNameEN").val(objBillingTargetDebtSummaryByOffice.BillingClientNameEN);
    $("#txtUnPaidInvoiceBillingClientNameLC").val(objBillingTargetDebtSummaryByOffice.BillingClientNameLC);
    $("#txtUnPaidInvoiceBillingClientAddressEN").val(objBillingTargetDebtSummaryByOffice.BillingClientAddressEN);
    $("#txtUnPaidInvoiceBillingClientAddressLC").val(objBillingTargetDebtSummaryByOffice.BillingClientAddressLC);
    $("#txtUnPaidInvoiceBillingClientTelNo").val(objBillingTargetDebtSummaryByOffice.BillingClientTelNo);
    $("#txtUnPaidInvoiceBillingClientContactPerson").val(objBillingTargetDebtSummaryByOffice.ContactPersonName);

    // check all input on Server
    var obj = GetUserAdjustData();
    $("#ICS030_ListOfUnPaidInvoiceByBillingTargetGrid").LoadDataToGrid(grdListOfUnPaidInvoiceByBillingTargetGrid
        , ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Income/ICS030_LoadGetUnpaidInvoiceDebtSummaryByBillingTargetData", obj, "doGetUnpaidInvoiceDebtSummaryByBillingTarget", false,
                    function (result, controls, isWarning) {
                        if (controls != undefined) {
                            VaridateCtrl(controls, controls);
                        }
                        else {
                            document.getElementById('divListOfUnPaidInvoiceByBillingTargetBody').scrollIntoView();
                        }
                    },
                    function (result, controls, isWarning) {
                        if (isWarning == undefined) {
                            $("#divListOfUnPaidInvoiceByBillingTargetBody").show();
                        }
                    });
}

function UnPaidBillingTargetUnpaidDetailClick(rowIndex, billingTargetCode) {
    _UnPaidBLByBLOffice_RowIndex = rowIndex;
    _strBillingTargetCode = billingTargetCode;

    $("#divListOfUnPaidInvoiceByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTarget").show();
    $("#divListOfUnPaidByBillingTargetLabel1").show();
    $("#divListOfUnPaidByBillingTargetLabel2").hide();
    
    // delete all value on screen of
    //ResetDebtActualTableValue();
    //ResetListOfUnPaidBillingTargetByBillingOfficeValue();
    ResetListOfUnPaidInvoiceByBillingTargetValue();
    ResetListOfUnPaidByBillingTargetValue();

    grdListOfUnPaidBillingTargetByBillingOfficeGrid.selectRow(rowIndex);
    var strBillingTargetDebtSummaryByOffice = grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(rowIndex, grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById("ToJson")).getValue();
    var objBillingTargetDebtSummaryByOffice = JSON.parse(htmlDecode(strBillingTargetDebtSummaryByOffice));

    $("#txtUnPaidByBillingBillingTargetCode").val(_strBillingTargetCode);
    $("#txtUnPaidByBillingBillingOffice").val(_strOfficeName);
    $("#txtUnPaidByBillingBillingOfficeCode").val(_strOfficeCode);
    $("#txtUnPaidByBillingBillingClientNameEN").val(objBillingTargetDebtSummaryByOffice.BillingClientNameEN);
    $("#txtUnPaidByBillingBillingClientNameLC").val(objBillingTargetDebtSummaryByOffice.BillingClientNameLC);
    $("#txtUnPaidByBillingBillingClientAddressEN").val(objBillingTargetDebtSummaryByOffice.BillingClientAddressEN);
    $("#txtUnPaidByBillingBillingClientAddressLC").val(objBillingTargetDebtSummaryByOffice.BillingClientAddressLC);
    $("#txtUnPaidByBillingBillingClientTelNo").val(objBillingTargetDebtSummaryByOffice.BillingClientTelNo);
    $("#txtUnPaidByBillingBillingClientContactPerson").val(objBillingTargetDebtSummaryByOffice.ContactPersonName);

    // check all input on Server
    var obj = GetUserAdjustData();
    $("#ICS030_ListOfUnPaidByBillingTargetGrid").LoadDataToGrid(grdListOfUnPaidByBillingTargetGrid
        , ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Income/ICS030_LoadGetUnpaidDetailDebtSummaryByBillingTargetData", obj, "doGetUnpaidDetailDebtSummary", false,
                    function (result, controls, isWarning) {
                        if (controls != undefined) {
                            VaridateCtrl(controls, controls);
                        }
                        else {
                            document.getElementById('divListOfUnPaidByBillingTargetBody').scrollIntoView();
                        }
                    },
                    function (result, controls, isWarning) {
                        if (isWarning == undefined) {
                            $("#divListOfUnPaidByBillingTargetBody").show();
                        }
                    });
}

function ListOfUnPaidInvoiceNoOfBillingClick(rowIndex, InvoiceNo, InvoiceOCC) {
    //Set global var
    _strInvoiceNo = InvoiceNo;
    _strInvoiceOCC = InvoiceOCC;

    //$("divDebtActualSummary").show();
    //$("divDebtActualTable").show();
    //$("#divListOfUnPaidBillingTargetByBillingOffice").show();
    //$("#divListOfUnPaidInvoiceByBillingTarget").show();
    $("#divListOfUnPaidByBillingTarget").show();
    $("#divListOfUnPaidByBillingTargetLabel1").hide();
    $("#divListOfUnPaidByBillingTargetLabel2").show();
    // delete all value on screen of
    //ResetDebtActualTableValue();
    //ResetListOfUnPaidBillingTargetByBillingOfficeValue();
    //ResetListOfUnPaidInvoiceByBillingTargetValue();
    ResetListOfUnPaidByBillingTargetValue();

    //Get BillingTargetDebtSummaryByOffice
    var officeRowIndex = grdListOfUnPaidBillingTargetByBillingOfficeGrid.getRowIndex(grdListOfUnPaidBillingTargetByBillingOfficeGrid.getSelectedId());
    var strBillingTargetDebtSummaryByOffice = grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(officeRowIndex, grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById("ToJson")).getValue();
    var objBillingTargetDebtSummaryByOffice = JSON.parse(htmlDecode(strBillingTargetDebtSummaryByOffice));

    $("#txtUnPaidByBillingBillingTargetCode").val(_strBillingTargetCode);
    $("#txtUnPaidByBillingBillingOffice").val(_strOfficeName);
    $("#txtUnPaidByBillingBillingOfficeCode").val(_strOfficeCode);
    $("#txtUnPaidByBillingBillingClientNameEN").val(objBillingTargetDebtSummaryByOffice.BillingClientNameEN);
    $("#txtUnPaidByBillingBillingClientNameLC").val(objBillingTargetDebtSummaryByOffice.BillingClientNameLC);
    $("#txtUnPaidByBillingBillingClientAddressEN").val(objBillingTargetDebtSummaryByOffice.BillingClientAddressEN);
    $("#txtUnPaidByBillingBillingClientAddressLC").val(objBillingTargetDebtSummaryByOffice.BillingClientAddressLC);
    $("#txtUnPaidByBillingBillingClientTelNo").val(objBillingTargetDebtSummaryByOffice.BillingClientTelNo);
    $("#txtUnPaidByBillingBillingClientContactPerson").val(objBillingTargetDebtSummaryByOffice.ContactPersonName);

    //Load data to grid
    var obj = GetUserAdjustData();
    $("#ICS030_ListOfUnPaidByBillingTargetGrid").LoadDataToGrid(grdListOfUnPaidByBillingTargetGrid
        , ROWS_PER_PAGE_FOR_VIEWPAGE, true, "/Income/ICS030_LoadGetUnpaidDetailDebtSummaryByInvoiceData", obj, "doGetUnpaidDetailDebtSummary", false,
                    function (result, controls, isWarning) {
                        if (controls != undefined) {
                            VaridateCtrl(controls, controls);
                        }
                        else {
                            document.getElementById('divListOfUnPaidByBillingTargetBody').scrollIntoView();
                        }
                    },
                    function (result, controls, isWarning) {
                        if (isWarning == undefined) {
                            $("#divListOfUnPaidByBillingTargetBody").show();
                        }
                    });
}


function btnOpenICS032FromUnPaidInvoiceGrid(rowId) {
    var arr1 = new Array();
    var arr2 = new Array();
    var arr3 = new Array();

    //Get BillingTargetDebtSummaryByOffice
    var officeRowIndex = _UnPaidBLByBLOffice_RowIndex; //grdListOfUnPaidBillingTargetByBillingOfficeGrid.getRowIndex(grdListOfUnPaidBillingTargetByBillingOfficeGrid.getSelectedId());
    var strBillingTargetDebtSummaryByOffice = grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(officeRowIndex, grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById("ToJson")).getValue();
    var objBillingTargetDebtSummaryByOffice = JSON.parse(htmlDecode(strBillingTargetDebtSummaryByOffice));

    if (objBillingTargetDebtSummaryByOffice != undefined && objBillingTargetDebtSummaryByOffice != null) {
        arr1.push(objBillingTargetDebtSummaryByOffice);
    }


    //Get UnpaidInvoiceDebtSummaryByBillingTarget
    var intRow = grdListOfUnPaidInvoiceByBillingTargetGrid.getRowIndex(rowId);
    var strUnpaidInvoiceDebtSummaryByBillingTarget = grdListOfUnPaidInvoiceByBillingTargetGrid.cells2(intRow, grdListOfUnPaidInvoiceByBillingTargetGrid.getColIndexById("ToJson")).getValue();
    var objUnpaidInvoiceDebtSummaryByBillingTarget = JSON.parse(htmlDecode(strUnpaidInvoiceDebtSummaryByBillingTarget));

    if (objUnpaidInvoiceDebtSummaryByBillingTarget != undefined && objUnpaidInvoiceDebtSummaryByBillingTarget != null) {
        arr2.push(objUnpaidInvoiceDebtSummaryByBillingTarget);
    };


    //Open screen
    ajax_method.CallScreenController("/Income/ICS030_CheckForICS032", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {

                    var tt = {
                        strBillingOfficeCode: _strOfficeCode,
                        strBillingOfficeName: _strOfficeName,
                        strInvoiceNo: _strInvoiceNo,
                        strInvoiceOCC: _strInvoiceOCC,
                        doBillingTargetDebtSummaryList: arr1,
                        doGetUnpaidInvoiceDebtSummaryByBillingTargetList: arr2,
                        doGetUnpaidDetailDebtSummaryByBillingCodeList: arr3,
                        strOpenFromListofUnpaidInvoiceByBillingTarget: 'Y'
                    };

                    var obj = { "data": tt };

                    ajax_method.CallScreenControllerWithAuthority("/Income/ICS032", obj, true);
                } else if (result == "2") {
                    /* --- Get Message --- */
                    var obj = {
                        module: "Common",
                        code: "MSG0049"
                    };
                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                        OpenErrorMessageDialog(result.Code, result.Message,
                        function () {},null);
                    });

                } else if (result == "3") {
                    /* --- Get Message --- */
                    var obj = {
                        module: "Common",
                        code: "MSG0053"
                    };
                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                        OpenErrorMessageDialog(result.Code, result.Message,
                        function () {},
                        null);
                    });
                }
            }
            else {
                VaridateCtrl(controls, controls);
            }
        });
}

function btnOpenICS032FromUnPaidByBillingGrid(rowId) {
    var arr1 = new Array();
    var arr2 = new Array();
    var arr3 = new Array();
    
    //Get BillingTargetDebtSummaryByOffice
    var officeRowIndex = _UnPaidBLByBLOffice_RowIndex; //grdListOfUnPaidBillingTargetByBillingOfficeGrid.getRowIndex(grdListOfUnPaidBillingTargetByBillingOfficeGrid.getSelectedId());
    var strBillingTargetDebtSummaryByOffice = grdListOfUnPaidBillingTargetByBillingOfficeGrid.cells2(officeRowIndex, grdListOfUnPaidBillingTargetByBillingOfficeGrid.getColIndexById("ToJson")).getValue();
    var objBillingTargetDebtSummaryByOffice = JSON.parse(htmlDecode(strBillingTargetDebtSummaryByOffice));

    if (objBillingTargetDebtSummaryByOffice != undefined && objBillingTargetDebtSummaryByOffice != null) {
        arr1.push(objBillingTargetDebtSummaryByOffice);        
    };

    //Get Unpiad detail debt summary
    var rowIndex = grdListOfUnPaidByBillingTargetGrid.getRowIndex(rowId);
    var strUnpaidDetailDebtSummary = grdListOfUnPaidByBillingTargetGrid.cells2(rowIndex, grdListOfUnPaidByBillingTargetGrid.getColIndexById("ToJson")).getValue();
    var objUnpaidDetailDebtSummary = JSON.parse(htmlDecode(strUnpaidDetailDebtSummary));

    if (objUnpaidDetailDebtSummary != undefined && objUnpaidDetailDebtSummary != null) {
        //Set global var
        _strInvoiceNo = objUnpaidDetailDebtSummary.InvoiceNo;
        _strInvoiceOCC = objUnpaidDetailDebtSummary.InvoiceOCC;
        arr3.push(objUnpaidDetailDebtSummary);
    };

    ajax_method.CallScreenController("/Income/ICS030_CheckForICS032", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {
                    var obj = {
                        strBillingOfficeCode: _strOfficeCode,
                        strBillingOfficeName: _strOfficeName,
                        strInvoiceNo: _strInvoiceNo,
                        strInvoiceOCC: _strInvoiceOCC,
                        doBillingTargetDebtSummaryList: arr1,
                        doGetUnpaidInvoiceDebtSummaryByBillingTargetList: arr2,
                        doGetUnpaidDetailDebtSummaryByBillingCodeList: arr3,
                        strOpenFromListofUnpaidInvoiceByBillingTarget: 'N'
                    };

                    ajax_method.CallScreenControllerWithAuthority("/Income/ICS032", obj, true);
                } else if (result == "2") {
                    /* --- Get Message --- */
                    var obj = {
                        module: "Common",
                        code: "MSG0049"
                    };
                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                        OpenErrorMessageDialog(result.Code, result.Message,
                        function () {

                        },
                        null);
                    });
                } else if (result == "3") {
                    /* --- Get Message --- */
                    var obj = {
                        module: "Common",
                        code: "MSG0053"
                    };
                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                        OpenErrorMessageDialog(result.Code, result.Message,
                        function () {

                        },
                        null);
                    });
                }
            }
            else {
                VaridateCtrl(controls, controls);
            }
        });
}

function InitialPage() {
    $("#divDebtActualTable").show();
    $("#divListOfUnPaidBillingTargetByBillingOffice").hide();
    $("#divListOfUnPaidInvoiceByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTargetLabel1").hide();
    $("#divListOfUnPaidByBillingTargetLabel2").hide();

    // example
    // Date
    //$("#dtpCustomerAcceptanceDate").InitialDate();
    //InitialDateFromToControl("#dptAdjustBillingPeriodDateFrom", "#dptAdjustBillingPeriodDateTo");
    //Text Input
    //$("#txtSelSeparateFromInvoiceNo").attr("maxlength", 12);
    // Number
    //$("#txtBillingAmount").BindNumericBox(10, 2, 0, 9999999999.99);
    //setVisableTable(conNo);
    //setFormMode(conModeInit);

    //    var exd = new Date();
    //    var intDay = exd.getDate();

    //    $("cboMonthYear").index(1);
}

// create all send to server data for check mendatory and save (in case all input data is ok)
function GetUserAdjustData() {

    var returnObj = {
        RawdtpMonthYear: _RawdtpMonthYear,
        //intMonth : _intMonth,
        //intYear : _intYear,

        strOfficeCode: _strOfficeCode,
        strOfficeName: _strOfficeName,

        strBillingTargetCode: _strBillingTargetCode,

        strInvoiceNo: _strInvoiceNo,
        strInvoiceOCC: _strInvoiceOCC
    };

    return returnObj;

}

// Clear Screen
function ClearScreenToInitStage() {

    $("#divDebtActualSummary").show();
    $("#divDebtActualTable").show();

    $("#divListOfUnPaidBillingTargetByBillingOffice").hide();
    $("#divListOfUnPaidInvoiceByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTarget").hide();
    $("#divListOfUnPaidByBillingTargetLabel1").hide();
    $("#divListOfUnPaidByBillingTargetLabel2").hide();

    // delete all value on screen of
    ResetDebtActualTableValue();
    ResetListOfUnPaidBillingTargetByBillingOfficeValue();
    ResetListOfUnPaidInvoiceByBillingTargetValue();
    ResetListOfUnPaidByBillingTargetValue();

    $("#divDebtActualTable").clearForm();
    $("#divListOfUnPaidBillingTargetByBillingOffice").clearForm();
    $("#divListOfUnPaidInvoiceByBillingTarget").clearForm();
    $("#divListOfUnPaidByBillingTarget").clearForm();

    CloseWarningDialog();
}

function number_format(number, decimals, dec_point, thousands_sep) {
    if (number == '') {
        number = '0'
    }

    if (number == undefined) {
        number = '0'
    }

    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals), sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec); return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}