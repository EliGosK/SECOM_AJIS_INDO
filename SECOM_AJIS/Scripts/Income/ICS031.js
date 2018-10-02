
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

var pageRow = ROWS_PER_PAGE_FOR_VIEWPAGE;
var grdSetUnpaidTargetGrid;
var _doGetDebtTargetList;
var _detailInputDatas;


var grdMoneyCollectionManagementInformationGrid;
var _doGetMoneyCollectionManagementInfoList;

$(document).ready(function () {
    // ..

    // Initial grid 1

    if ($.find("#ICS031_SetUnpaidTargetGrid").length > 0) {

        grdSetUnpaidTargetGrid = $("#ICS031_SetUnpaidTargetGrid").InitialGrid(0, false, "/Income/ICS031_InitialSetUnpaidTargetGrid", function () {

            SpecialGridControl(grdSetUnpaidTargetGrid, ["ICS031_SetUnpaidTargetGrid", "AllUnpaidTargetAmount", "AllUnpaidTargetNoOfBillingDetail", "UnpaidOverTargetAmount", "UnpaidOverTargetNoOfBillingDetail"]);
            BindOnLoadedEvent(grdSetUnpaidTargetGrid, function () {


                for (var i = 0; i < grdSetUnpaidTargetGrid.getRowsNum(); i++) {

                    var rowId = grdSetUnpaidTargetGrid.getRowId(i);
                    //-----------------------------------------
                    // Col 1
                    //var ColIndex = grdByBillingDetailGrid.getColIndexById("ContractCode");
                    //var val = GetValueFromLinkType(grdByBillingDetailGrid, i, ColIndex);
                    // Col 4
                    var clt4 = "#" + GenerateGridControlID("txtAllUnpaidTargetAmount", rowId);
                    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtAllUnpaidTargetAmount", rowId, "AllUnpaidTargetAmount", $(clt4).val(), 12, 2, 1, 999999999999.99, 0, true);

                    // Col 5
                    var clt5 = "#" + GenerateGridControlID("txtAllUnpaidTargetNoOfBillingDetail", rowId);
                    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtAllUnpaidTargetNoOfBillingDetail", rowId, "AllUnpaidTargetNoOfBillingDetail", $(clt5).val(), 12, 0, 1, 999999999999, 0, true);

                    // Col 6
                    var clt6 = "#" + GenerateGridControlID("txtUnpaidOverTargetAmount", rowId);
                    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtUnpaidOverTargetAmount", rowId, "UnpaidOverTargetAmount", $(clt6).val(), 12, 2, 1, 999999999999.99, 0, true);

                    // Col 7
                    var clt7 = "#" + GenerateGridControlID("txtUnpaidOverTargetNoOfBillingDetail", rowId);
                    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtUnpaidOverTargetNoOfBillingDetail", rowId, "UnpaidOverTargetNoOfBillingDetail", $(clt7).val(), 12, 0, 1, 999999999999, 0, true);

                }

                grdSetUnpaidTargetGrid.setSizes();
            });

            //Init Object Event
            // 1 Div Panel Body
            //$("#btnSearch").click(btn_Search_click);
            //$("#btnClear").click(btn_Clear_click);
            //Initial Page
            InitialPage();
        });

    }


    
});

// Grid Function

function Add_SetUnpaidTargetBlankLine(detailInputData) {

    CheckFirstRowIsEmpty(grdSetUnpaidTargetGrid, true);

    AddNewRow(grdSetUnpaidTargetGrid, [detailInputData.No,
                                            detailInputData.BillingOfficeCode,
                                            detailInputData.BillingOfficeName,
                                            detailInputData.CurrencyTypeName,
                                            "",
                                            "",
                                            "",
                                            ""
                                            ]);


    var row_idx = grdSetUnpaidTargetGrid.getRowsNum() - 1;
    var rowId = grdSetUnpaidTargetGrid.getRowId(row_idx);

    var defStringVal = "";
    var defNumVal = 0;
    var defDateVal = "";
    accounting.toFixed(detailInputData.AllUnpaidTargetAmount, 2)
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtAllUnpaidTargetAmount", rowId, "AllUnpaidTargetAmount", accounting.toFixed(detailInputData.AllUnpaidTargetAmount, 2), 12, 2, 1, 999999999999.99, 0, true);
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtAllUnpaidTargetNoOfBillingDetail", rowId, "AllUnpaidTargetNoOfBillingDetail", detailInputData.AllUnpaidTargetBillingDetail, 12, 0, 1, 999999999999, 0, true);
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtUnpaidOverTargetAmount", rowId, "UnpaidOverTargetAmount", accounting.toFixed(detailInputData.UnpaidOverTargetAmount, 2), 12, 2, 1, 999999999999.99, 0, true);
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtUnpaidOverTargetNoOfBillingDetail", rowId, "UnpaidOverTargetNoOfBillingDetail", detailInputData.UnpaidOverBillingDetail, 12, 0, 1, 999999999999, 0, true);

    grdSetUnpaidTargetGrid.setSizes();
 

}
function Add_SetUnpaidTargetTotalLine() {

    //ICS031_ViewBag.currencyLocal
    //ICS031_ViewBag.currencyUs

    var TotalAmountAll;
    var TotalAmountAllUsd;

    var TotalAmount2Month;
    var TotalAmount2MonthUsd;

    var TotalDetailAll;
    var TotalDetailAllUsd;

    var TotalDetail2Month;
    var TotalDetail2MonthUsd;

    TotalAmountAll = 0;
    TotalAmountAllUsd = 0;

    TotalAmount2Month = 0;
    TotalAmount2MonthUsd = 0;

    TotalDetailAll = 0;
    TotalDetailAllUsd = 0;

    TotalDetail2Month = 0;
    TotalDetail2MonthUsd = 0;

    CheckFirstRowIsEmpty(grdSetUnpaidTargetGrid, true);

    for (var i = 0; i < grdSetUnpaidTargetGrid.getRowsNum(); i++) {
        
        var rowId = grdSetUnpaidTargetGrid.getRowId(i);
        //-----------------------------------------
        // Col 1
        //var ColIndex = grdByBillingDetailGrid.getColIndexById("ContractCode");
        //var val = GetValueFromLinkType(grdByBillingDetailGrid, i, ColIndex);
        // Col 4
        var rCurrency = grdSetUnpaidTargetGrid.cells
                                    (rowId, grdSetUnpaidTargetGrid.getColIndexById("Currency")).getValue()
        var clt4 = "#" + GenerateGridControlID("txtAllUnpaidTargetAmount", rowId);
        if ($(clt4).val() != '') {
            if (rCurrency == ICS031_ViewBag.currencyLocal) {
                TotalAmountAll = TotalAmountAll + parseFloat($(clt4).NumericValue());
            }
            else {
                TotalAmountAllUsd = TotalAmountAllUsd + parseFloat($(clt4).NumericValue());
            }
        }
        // Col 5
        var clt5 = "#" + GenerateGridControlID("txtAllUnpaidTargetNoOfBillingDetail", rowId);
        if ($(clt5).val() != '') {
            if (rCurrency == ICS031_ViewBag.currencyLocal) {
                TotalDetailAll = TotalDetailAll + parseInt($(clt5).NumericValue());
            } else {
                TotalDetailAllUsd = TotalDetailAllUsd + parseInt($(clt5).NumericValue());
            }
        }
        // Col 6
        var clt6 = "#" + GenerateGridControlID("txtUnpaidOverTargetAmount", rowId);
        if ($(clt6).val() != '') {
            if (rCurrency == ICS031_ViewBag.currencyLocal) {
                TotalAmount2Month = TotalAmount2Month + parseFloat($(clt6).NumericValue());
            } else {
                TotalAmount2MonthUsd = TotalAmount2MonthUsd + parseFloat($(clt6).NumericValue());
            }
        }
        // Col 7
        var clt7 = "#" + GenerateGridControlID("txtUnpaidOverTargetNoOfBillingDetail", rowId);
        if ($(clt7).val() != '') {
            if (rCurrency == ICS031_ViewBag.currencyLocal) {
                TotalDetail2Month = TotalDetail2Month + parseInt($(clt7).NumericValue());
            } else {
                TotalDetail2MonthUsd = TotalDetail2MonthUsd + parseInt($(clt7).NumericValue());
            }
        }
    }


    var noIndex = grdSetUnpaidTargetGrid.getColIndexById('No');
    var billingColIndex = grdSetUnpaidTargetGrid.getColIndexById('BillingOfficeCode');
    var billingColNameIndex = grdSetUnpaidTargetGrid.getColIndexById('BillingOfficeName');

    //ICS031_ViewBag.currencyLocal
    //ICS031_ViewBag.currencyUs
    AddNewRow(grdSetUnpaidTargetGrid, ["<b>" + ICS031_ViewBag.lblTotal + "</b>",
                                            "",
                                            "",
                                            ICS031_ViewBag.currencyLocal,
                                            "",
                                            "",
                                            "",
                                            ""
                                            ]);

    var rowId = grdSetUnpaidTargetGrid.getRowId(i);

    grdSetUnpaidTargetGrid.cells2(i, noIndex).cell.colSpan = 3;
    grdSetUnpaidTargetGrid.cells2(i, noIndex).cell.rowSpan = 2;
    grdSetUnpaidTargetGrid.cells2(i, billingColIndex).cell.style.display = 'none';
    grdSetUnpaidTargetGrid.cells2(i, billingColNameIndex).cell.style.display = 'none';

    grdSetUnpaidTargetGrid.cells2(i, noIndex).cell.style.textAlign = 'right';
    grdSetUnpaidTargetGrid.cells2(i, noIndex).cell.style.verticalAlign = 'middle';

    i++;

    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtAllUnpaidTargetAmount", rowId, "AllUnpaidTargetAmount", number_format(TotalAmountAll, 2, '.', ','), 20, 2, 1, 99999999999999999999.99, 0, false);
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtAllUnpaidTargetNoOfBillingDetail", rowId, "AllUnpaidTargetNoOfBillingDetail", number_format(TotalDetailAll, 0, '.', ','), 20, 0, 1, 99999999999999999999, 0, false);
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtUnpaidOverTargetAmount", rowId, "UnpaidOverTargetAmount", number_format(TotalAmount2Month, 2, '.', ','), 20, 2, 1, 99999999999999999999.99, 0, false);
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtUnpaidOverTargetNoOfBillingDetail", rowId, "UnpaidOverTargetNoOfBillingDetail", number_format(TotalDetail2Month, 0, '.', ','), 20, 0, 1, 99999999999999999999, 0, false);

    AddNewRow(grdSetUnpaidTargetGrid, ["<b>" + ICS031_ViewBag.lblTotal + "</b>",
                                            "",
                                            "",
                                            ICS031_ViewBag.currencyUs,
                                            "",
                                            "",
                                            "",
                                            ""
    ]);

    rowId = grdSetUnpaidTargetGrid.getRowId(i);

    grdSetUnpaidTargetGrid.cells2(i, noIndex).cell.style.display = 'none';
    grdSetUnpaidTargetGrid.cells2(i, billingColIndex).cell.style.display = 'none';
    grdSetUnpaidTargetGrid.cells2(i, billingColNameIndex).cell.style.display = 'none';

    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtAllUnpaidTargetAmount", rowId, "AllUnpaidTargetAmount", number_format(TotalAmountAllUsd, 2, '.', ','), 20, 2, 1, 99999999999999999999.99, 0, false);
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtAllUnpaidTargetNoOfBillingDetail", rowId, "AllUnpaidTargetNoOfBillingDetail", number_format(TotalDetailAllUsd, 0, '.', ','), 20, 0, 1, 99999999999999999999, 0, false);
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtUnpaidOverTargetAmount", rowId, "UnpaidOverTargetAmount", number_format(TotalAmount2MonthUsd, 2, '.', ','), 20, 2, 1, 99999999999999999999.99, 0, false);
    GenerateNumericBox2(grdSetUnpaidTargetGrid, "txtUnpaidOverTargetNoOfBillingDetail", rowId, "UnpaidOverTargetNoOfBillingDetail", number_format(TotalDetail2MonthUsd, 0, '.', ','), 20, 0, 1, 99999999999999999999, 0, false);


    grdSetUnpaidTargetGrid.setSizes();
}


function Update_SetUnpaidTargetTotalLine() {
    var TotalAmountAll;
    var TotalAmountAllUsd;

    var TotalAmount2Month;
    var TotalAmount2MonthUsd;

    var TotalDetailAll;
    var TotalDetailAllUsd;

    var TotalDetail2Month;
    var TotalDetail2MonthUsd;

    TotalAmountAll = 0;
    TotalAmountAllUsd = 0;

    TotalAmount2Month = 0;
    TotalAmount2MonthUsd = 0;

    TotalDetailAll = 0;
    TotalDetailAllUsd = 0;

    TotalDetail2Month = 0;
    TotalDetail2MonthUsd = 0;

    CheckFirstRowIsEmpty(grdSetUnpaidTargetGrid, true);

    for (var i = 0; i < grdSetUnpaidTargetGrid.getRowsNum() - 2 ; i++) {

        var rowId = grdSetUnpaidTargetGrid.getRowId(i);
        //-----------------------------------------
        // Col 1
        //var ColIndex = grdByBillingDetailGrid.getColIndexById("ContractCode");
        //var val = GetValueFromLinkType(grdByBillingDetailGrid, i, ColIndex);
        // Col 4
        var rCurrency = grdSetUnpaidTargetGrid.cells
                                    (rowId, grdSetUnpaidTargetGrid.getColIndexById("Currency")).getValue()
        var clt4 = "#" + GenerateGridControlID("txtAllUnpaidTargetAmount", rowId);
        if ($(clt4).val() != '') {
            if (rCurrency == ICS031_ViewBag.currencyLocal) {
                TotalAmountAll = TotalAmountAll + parseFloat($(clt4).NumericValue());
            }
            else {
                TotalAmountAllUsd = TotalAmountAllUsd + parseFloat($(clt4).NumericValue());
            }
        }
        // Col 5
        var clt5 = "#" + GenerateGridControlID("txtAllUnpaidTargetNoOfBillingDetail", rowId);
        if ($(clt5).val() != '') {
            if (rCurrency == ICS031_ViewBag.currencyLocal) {
                TotalDetailAll = TotalDetailAll + parseInt($(clt5).NumericValue());
            } else {
                TotalDetailAllUsd = TotalDetailAllUsd + parseInt($(clt5).NumericValue());
            }
        }
        // Col 6
        var clt6 = "#" + GenerateGridControlID("txtUnpaidOverTargetAmount", rowId);
        if ($(clt6).val() != '') {
            if (rCurrency == ICS031_ViewBag.currencyLocal) {
                TotalAmount2Month = TotalAmount2Month + parseFloat($(clt6).NumericValue());
            } else {
                TotalAmount2MonthUsd = TotalAmount2MonthUsd + parseFloat($(clt6).NumericValue());
            }
        }
        // Col 7
        var clt7 = "#" + GenerateGridControlID("txtUnpaidOverTargetNoOfBillingDetail", rowId);
        if ($(clt7).val() != '') {
            if (rCurrency == ICS031_ViewBag.currencyLocal) {
                TotalDetail2Month = TotalDetail2Month + parseInt($(clt7).NumericValue());
            } else {
                TotalDetail2MonthUsd = TotalDetail2MonthUsd + parseInt($(clt7).NumericValue());
            }
        }
    }



    // update total in last rows
    var localRowId = grdSetUnpaidTargetGrid.getRowId(grdSetUnpaidTargetGrid.getRowsNum() - 2);
    var usRowId = grdSetUnpaidTargetGrid.getRowId(grdSetUnpaidTargetGrid.getRowsNum() - 1);


    $("#" + GenerateGridControlID("txtAllUnpaidTargetAmount", localRowId)).val(number_format(TotalAmountAll, 2, '.', ','));
    $("#" + GenerateGridControlID("txtAllUnpaidTargetAmount", usRowId)).val(number_format(TotalAmountAllUsd, 2, '.', ','));

    $("#" + GenerateGridControlID("txtAllUnpaidTargetNoOfBillingDetail", localRowId)).val(number_format(TotalDetailAll, 0, '.', ','));
    $("#" + GenerateGridControlID("txtAllUnpaidTargetNoOfBillingDetail", usRowId)).val(number_format(TotalDetailAllUsd, 0, '.', ','));

    $("#" + GenerateGridControlID("txtUnpaidOverTargetAmount", localRowId)).val(number_format(TotalAmount2Month, 2, '.', ','));
    $("#" + GenerateGridControlID("txtUnpaidOverTargetAmount", usRowId)).val(number_format(TotalAmount2MonthUsd, 2, '.', ','));

    $("#" + GenerateGridControlID("txtUnpaidOverTargetNoOfBillingDetail", localRowId)).val(number_format(TotalDetail2Month, 0, '.', ','));
    $("#" + GenerateGridControlID("txtUnpaidOverTargetNoOfBillingDetail", usRowId)).val(number_format(TotalDetail2MonthUsd, 0, '.', ','));

    //var clt4 = "#" + GenerateGridControlID("txtAllUnpaidTargetAmount", rowId);
    //$(clt4).val(number_format(TotalAmountAll, 2, '.', ','));

    // Col 5
    //var clt5 = "#" + GenerateGridControlID("txtAllUnpaidTargetNoOfBillingDetail", rowId);
    //$(clt5).val(number_format(TotalDetailAll, 0, '.', ','));

    // Col 6
    //var clt6 = "#" + GenerateGridControlID("txtUnpaidOverTargetAmount", rowId);
    //$(clt6).val(number_format(TotalAmount2Month, 2, '.', ','));

    // Col 7
    //var clt7 = "#" + GenerateGridControlID("txtUnpaidOverTargetNoOfBillingDetail", rowId);
    //$(clt7).val(number_format(TotalDetail2Month, 0, '.', ','));

    grdSetUnpaidTargetGrid.setSizes();
}

function number_format(number, decimals, dec_point, thousands_sep) {

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

function InitialPage() {

    // example
    // Date
    //$("#dtpCustomerAcceptanceDate").InitialDate();
    //InitialDateFromToControl("#dptAdjustBillingPeriodDateFrom", "#dptAdjustBillingPeriodDateTo");
    //Text Input
    //$("#txtSelSeparateFromInvoiceNo").attr("maxlength", 12);
    // Number
    //$("#txtBillingAmount").BindNumericBox(10, 2, 0, 9999999999.99);
    //setVisableTable(conNo);
    setFormMode(conModeInit);
    LoadData();

}
// Form Mode Section

var conModeInit = 0;
var conModeView = 1;
var conModeEdit = 2;
var conModeConfirm = 9;

var conNo = 0;
var conYes = 1;

var bolViewMode = false;

function setFormMode(intMode) {
    // ModeInit
    if (intMode == conModeInit) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (intMode == conModeView) {
        // ModeView = 1;
        register_command.SetCommand(btn_Register_click);
        reset_command.SetCommand(btn_Reset_click);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
    }
    else if (intMode == conModeEdit) {
        // ModeEdit = 2;
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }
    else if (intMode == conModeConfirm) {
        // ModeConfirm = 9;
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        approve_command.SetCommand(null);
        reject_command.SetCommand(null);
        return_command.SetCommand(null);
        close_command.SetCommand(null);
        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }
}

function LoadData() {
    // check all input on Server
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS031_SearchData", obj, function (result, controls, isWarning) {
        if (result != undefined) {

            // goto Idel state
            // Wired design
            setFormMode(conModeView);
            bolViewMode = true;

            _doGetDebtTargetList = result.doGetDebtTargetList;
            _detailInputDatas = result.detailInputDatas;


            // Comment by jirwat jannet @ 2016-10-13
            //if (_doGetDebtTargetList != null) {
            //    for (var i = 0; i < _doGetDebtTargetList.length; ++i) {
            //        Add_SetUnpaidTargetBlankLine(_doGetDebtTargetList[i]);
            //    }
            //};

            // Add data grid rows by Jirawat Jannet @2016-10-13
            if (_detailInputDatas != null) {
                for (var i = 0; i < _detailInputDatas.length; i++) {
                    Add_SetUnpaidTargetBlankLine(_detailInputDatas[i]);
                }
            }

            InitDataGridMergeUI(grdSetUnpaidTargetGrid);

            Add_SetUnpaidTargetTotalLine();

            //            $("#dtpExpectedCollectDateFrom").attr("disabled", true);
            //            $("#dtpExpectedCollectDateTo").attr("disabled", true);
            //            $("#chklCollectionArea").attr("disabled", true);

            //            $("#btnSearch").attr("disabled", true);
            //            $("#btnClear").attr("disabled", false);

        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

function InitDataGridMergeUI(grid) {
    // hide column before set row span
    var rowIndex = 1;
    var groupIndex = 1;
    for (var i = 0 ; i < grid.getRowsNum() ; i++) {

        if ((rowIndex - 1) % 2 == 0) {
            var noIndex = grid.getColIndexById('No');
            var billingColIndex = grid.getColIndexById('BillingOfficeCode');
            var billingColNameIndex = grid.getColIndexById('BillingOfficeName');


            grid.cells2(i, noIndex).cell.rowSpan = 2;
            grid.cells2(i, billingColIndex).cell.rowSpan = 2;
            grid.cells2(i, billingColNameIndex).cell.rowSpan = 2;

            groupIndex = groupIndex + 1;
        } else {
            var noIndex = grid.getColIndexById('No');
            var billingColIndex = grid.getColIndexById('BillingOfficeCode');
            var billingColNameIndex = grid.getColIndexById('BillingOfficeName');

            grid.cells2(i, noIndex).cell.style.display = 'none';
            grid.cells2(i, billingColIndex).cell.style.display = 'none';
            grid.cells2(i, billingColNameIndex).cell.style.display = 'none';
        }

        rowIndex = rowIndex + 1;
    }
}

function btn_Search_click() {
    // check all input on Server
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS031_SearchData", obj, function (result, controls, isWarning) {
        if (result != undefined) {

            // goto Idel state
            // Wired design
            setVisableTable(conYes);
            setFormMode(conModeInit);
            bolViewMode = true;

            _doGetMoneyCollectionManagementInfoList = result.doGetMoneyCollectionManagementInfo;
            if (_doGetMoneyCollectionManagementInfoList != null) {
                for (var i = 0; i < _doGetMoneyCollectionManagementInfoList.length; ++i) {
                    Add_MoneyCollectionManagementInformationBlankLine(_doGetMoneyCollectionManagementInfoList[i]);
                }
            };

            $("#dtpExpectedCollectDateFrom").attr("disabled", true);
            $("#dtpExpectedCollectDateTo").attr("disabled", true);
            $("#chklCollectionArea").attr("disabled", true);

            $("#btnSearch").attr("disabled", true);
            $("#btnClear").attr("disabled", false);

            $("#btnDownloadFile").attr("disabled", false);

        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });
}

// Mode Event
function btn_Register_click() {

    Update_SetUnpaidTargetTotalLine();
    // check all input on Server
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Income/ICS031_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "1") {
                // goto confirm state

                setFormMode(conModeEdit);

                bolViewMode = true;
                //$("#divSetUnpaidTarget").SetViewMode(true);
                SetViewMode031(true, true);
                // Hide delete button in grid
                //                var colBtnRemoveIdx1 = grdByBillingDetailGrid.getColIndexById("Del");
                //                grdByBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, true);

            }
            else {
                VaridateCtrl(controls, controls);
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }
    });

}

// create all send to server data for check mendatory and save (in case all input data is ok)
function GetUserAdjustData() {

    var arr1 = new Array();

    if (CheckFirstRowIsEmpty(grdSetUnpaidTargetGrid) == false) {

        for (var i = 0; i < grdSetUnpaidTargetGrid.getRowsNum() - 2; i++) {

            var row_id = grdSetUnpaidTargetGrid.getRowId(i);

            var clt4 = "#" + GenerateGridControlID("txtAllUnpaidTargetAmount", row_id);
            var clt5 = "#" + GenerateGridControlID("txtAllUnpaidTargetNoOfBillingDetail", row_id);
            var clt6 = "#" + GenerateGridControlID("txtUnpaidOverTargetAmount", row_id);
            var clt7 = "#" + GenerateGridControlID("txtUnpaidOverTargetNoOfBillingDetail", row_id);
            // non custom object in grid use object name
            // use column name in XML not declare ass txt, dtp ect

            var obj1 = {

                txtBillingOfficeCode: grdSetUnpaidTargetGrid.cells
                                    (row_id, grdSetUnpaidTargetGrid.getColIndexById("BillingOfficeCode")).getValue(),
                txtCurrency: grdSetUnpaidTargetGrid.cells
                                    (row_id, grdSetUnpaidTargetGrid.getColIndexById("Currency")).getValue(),
                txtAmountAll: parseFloat($(clt4).NumericValue()),
                txtDetailAll: parseInt($(clt5).NumericValue()),
                txtAmount2Month: parseFloat($(clt6).NumericValue()),
                txtDetail2Month: parseInt($(clt7).NumericValue())

                // example non clustom grid getvalue
                //strBillingamount: grdMoneyCollectionManagementInformationGrid.cells
                //(row_id, grdMoneyCollectionManagementInformationGrid.getColIndexById("Billingamount")).getValue(),
            };
            arr1.push(obj1);
        }
    }

    var detail1 = arr1;

    var returnObj = {
        Detail1: detail1
    };

    return returnObj;

}


function btn_Reset_click() {
//    /* --- Get Message --- */
//    var obj = {
//        module: "Common",
//        code: "MSG0038"
//    };
//    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
//        OpenOkCancelDialog(result.Code, result.Message,
//        function () {

            $("#divSetUnpaidTarget").SetViewMode(false);
            $("#divSetUnpaidTarget").ResetToNormalControl(true);

            DeleteAllRow(grdSetUnpaidTargetGrid);
            _doGetDebtTargetList = null;

            LoadData();
//        },
//        null);
//    });
}

function btn_Confirm_click() {

    // save data
    ajax_method.CallScreenController("/Income/ICS031_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {
                    // goto confirm state

                    // Success
                    var objMsg = {
                        module: "Income",
                        code: "MSG7008"
                    };
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {
                            //
                        });
                    });

                    $("#divSetUnpaidTarget").SetViewMode(false);
                    $("#divSetUnpaidTarget").ResetToNormalControl(true);

                    DeleteAllRow(grdSetUnpaidTargetGrid);
                    _doGetDebtTargetList = null;

                    LoadData();
                }
            }
            else {
                VaridateCtrl(controls, controls);
            }
        });

}
function btn_Back_click() {

    setFormMode(conModeView);
    bolViewMode = false;

    $("#divSetUnpaidTarget").SetViewMode(false);

    $("#divSetUnpaidTarget").ResetToNormalControl(true);

    //    // Show delete button in grid
    //    var colBtnRemoveIdx1 = grdByBillingDetailGrid.getColIndexById("Del");
    //    grdByBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, false);
    //    // Concept by P'Leing
    //    SetFitColumnForBackAction(grdByBillingDetailGrid, "TempSpan");

}

// Clear Screen
function ClearScreenToInitStage() {

}
// Enable Obj On Screen

// Visable Obj On Screen
function setVisableTable(intMode) {

    if (intMode == conYes) {
        $("#divResaultList").show();
    }
    else if (intMode == conNo) {
        $("#divResaultList").hide();
    }
    else {
        $("#divResaultList").hide();
    };

}

function SetViewMode031(isview, editonly) {
    // Modified by Natthavat S. (02/11/2011)
    // Remark: Modified to support radio button
    $("#divSetUnpaidTarget").find("input[type=text],input[type=checkbox],input[type=radio],textarea,button,select,span[class=label-remark],div[class=label-remark],a").each(function () {
        if ($(this).attr("id") == undefined) {
            var isctrl = true;
            var tag = this.tagName.toLowerCase();
            if ((tag == "input") || tag == "a") {
                isctrl = false;
            }

            if (isctrl) {
                if (isview) {
                    $(this).hide();
                }
                else {
                    $(this).show();
                }
            }
        }
        else {

            var div = "div" + $(this).attr("id");
            var tag = this.tagName.toLowerCase();
            var readonly = this.readOnly;
            var klass = $(this).attr("class");

            /* --- Merge --- */
            var disabled = this.disabled;
            /* ------------- */

            if (isview) {
                if (tag == "input" && $(this).attr("type") == "checkbox") {
                    $(this).attr("disabled", true);
                } else if (tag == "input" && $(this).attr("type") == "radio") {
                    $(this).attr("disabled", true);
                }
                else {
                    $(this).hide();
                    var unitDate = $("#unitDate" + $(this).attr("id"));
                    if (unitDate.length > 0) {
                        unitDate.hide();
                    }

                    if (tag == "input" || tag == "select" || tag == "textarea" || tag == "a") {
                        /* --- For Datetime Picker --- */
                        $(this).parent().find("img").hide();

                        var css = "label-readonly-view";
                        if (readonly == false || readonly == undefined || editonly == true)
                            css = "label-edit-view";

                        /* --- Merge --- */
                        if (disabled == true)
                            css = "label-readonly-view";
                        /* ------------- */

                        // for 031 only
                        if (klass == "numeric-box") {

                            if (css == "label-edit-view") {
                                css = "label-edit-view-number";
                            } else if (css == "label-readonly-view") {
                                css = "label-readonly-view-number";
                            }
                        }
                        // end for 031 only

                        var val = $(this).val();
                        if (tag == "select") {
                            if (val != "") {
                                val = $(this).find(":selected").text();
                            }
                        }

                        //Add by Jutarat A. (15/02/2012) 
                        //For support link
                        if (tag == "a") {
                            css = "label-readonly-view";
                            val = $(this).text();
                        }
                        //End Add

                        if ($.trim(val) == "") {
                            val = "-";
                            //                        var unit = $("#unit" + $(this).attr("id"));
                            //                        if (unit.length > 0) {
                            //                            unit.hide();
                            //                        }
                        }

                        if (tag == "textarea") {
                            val = val.replace(/\n/g, "<br/>");
                        }

                        /* --- Merge --- */
                        if (typeof (val) == "string" && val != undefined) {
                            val = ConvertSSH(val);
                        }
                        /* ------------- */

                        $(this).parent().find("#" + div).remove();
                        $(this).parent().append("<div id='" + div + "' class='" + css + "'>" + val + "</div>");
                    }
                }
            }
            else {
                if (tag == "input" && $(this).attr("type") == "checkbox") {
                    $(this).removeAttr("disabled");
                } else if (tag == "input" && $(this).attr("type") == "radio") {
                    $(this).removeAttr("disabled");
                }
                else {
                    $(this).show();
                    var unitlst = ["unitDate", "unit"];
                    for (var ui = 0; ui < unitlst.length; ui++) {
                        var unit = $("#" + unitlst[ui] + $(this).attr("id"));
                        if (unit.length > 0) {
                            unit.show();
                        }
                    }

                    if (tag == "input" || tag == "select" || tag == "textarea" || tag == "a") {
                        /* --- For Datetime Picker --- */
                        $(this).parent().find("img").show();
                        $(this).parent().find("#" + div).remove();
                    }
                }
            }
        }
    });
}

// Add by Jirawat Jannet @ 2016-10-13
// Generate textbox with currenc combobox
function GenerateNumericCurrencyControl(grid, id, row_id, col_id, currency) {
    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);

    var obj = {
        id: fid,
        currency: currency
    };
    ///Income/ICS031_Register
    ajax_method.CallScreenController("/Billing/ICS031_GenerateCurrencyNumericTextBox", obj, function (result, controls, isWarning) {
        var txt = result;
        grid.cells(row_id, col).setValue(txt);
        fid = "#" + fid;
        $(fid).BindNumericBox(12, 2, 0, 999999999999.99);
        var dVal = parseFloat($(fid).val());

        $(fid).val(accounting.toFixed(dVal, 2));
        $(fid).NumericCurrency().attr('disabled', false);

        $(fid).focus(function () {
            grid.selectRowById(row_id);
        });
    });
}