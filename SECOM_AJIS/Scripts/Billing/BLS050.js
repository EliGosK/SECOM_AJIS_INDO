
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
/// <reference path="../number-functions.js" />

var grdCancelBillingDetailGrid;
var grdIssueBillingDetailGrid;

var forchk_cboAdjustmentType;
var forchk_BillingAmountAdj;
var forchk_dptAdjustBillingPeriodDateFrom;
var forchk_dptAdjustBillingPeriodDateTo;

var C_ISSUE_INV_NORMAL;
var C_LASTOCC;

var BLS050ConfirmWarningMessageList;


$(document).ready(function () {
    // ..ROWS_PER_PAGE_FOR_SEARCHPAGE
    // insert case not require ROWS_PER_PAGE_FOR_SEARCHPAGE


    // Initial grid 1
    if ($.find("#BLS050_CancelBillingDetailGrid").length > 0) {

        grdCancelBillingDetailGrid = $("#BLS050_CancelBillingDetailGrid").InitialGrid(999, false, "/Billing/BLS050_InitialCancelBillingDetailGrid", function () {

            SpecialGridControl(grdCancelBillingDetailGrid, ["BLS050_CancelBillingDetailGrid", "Chk"]);
            BindOnLoadedEvent(grdCancelBillingDetailGrid, function () {

                var colInx = grdCancelBillingDetailGrid.getColIndexById('Button');

                for (var i = 0; i < grdCancelBillingDetailGrid.getRowsNum(); i++) {

                    var rowId = grdCancelBillingDetailGrid.getRowId(i);
                    //-----------------------------------------
                    // Col 1
                    var clt1 = "#" + GenerateGridControlID("chkDel", rowId);
                    var checkboxColInx = grdCancelBillingDetailGrid.getColIndexById("Chk");
                    grdCancelBillingDetailGrid.cells2(i, checkboxColInx).setValue(GenerateCheckBox("chkDel", rowId, $(clt1).val(), true));
                    BindGridCheckBoxClickEvent("chkDel", rowId, function (rowId, checked) {
                        chkCancelBillingDetailGrid(rowId, checked);
                    });

                }

                grdCancelBillingDetailGrid.setSizes();
            });
        });

    }

    // Initial grid 2
    if ($.find("#BLS050_IssueBillingDetailGrid").length > 0) {

        grdIssueBillingDetailGrid = $("#BLS050_IssueBillingDetailGrid").InitialGrid(999, false, "/Billing/BLS050_InitialIssueBillingDetailGrid", function () {

            SpecialGridControl(grdIssueBillingDetailGrid, ["BLS050_IssueBillingDetailGrid", "Billingtype", "FirstFeeFlag", "ContractOCC", "From", "Billingamount", "Issueinvoice", "Paymentmethod", "BillingdetailinvoiceformatBilling", "Expectedissueautotransferdate", "Del", "BillingTypeGroup"]);

        });

    }

    //Init Object Event
    // 1 Div Panel Body
    $("#btnRetrieve").click(btn_Retrieve_click);
    $("#btnClear").click(function () {
        setAllObjectToConfirmMode(conNo);
        ClearMainForm();
        CleardivSpecifyProcessType();
        setVisableTable(0);
        setFormMode(conModeInit);
        DeleteAllRow(grdCancelBillingDetailGrid);
        DeleteAllRow(grdIssueBillingDetailGrid);
    });
    $("#btnSelectProcess").click(btn_Select_Process_click);

    $("#rdoReCreateBillingDetail").change(rdoReCreateBillingDetail_Select);
    $("#rdoCancelBillingDetail").change(rdoCancelBillingDetail_Select);
    $("#rdoForceCreateBillingDetail").change(rdoForceCreateBillingDetail_Select);
    $("#rdoRegisterAdjustOnNextPeriodAmount").change(rdoRegisterAdjustOnNextPeriodAmount_Select);

    // tt
    //$("#rdoRegister").change(rdoRegister_Select);
    //$("#rdoDelete").change(rdoDelete_Select);

    $("#ProcessType").change(ProcessType_change);

    //Initial Page
    InitialPage();
});

// grid function

function ProcessType_change() {
    var testVal = $("#BillingAmountAdj").NumericCurrency().val();
    var processTypeVal = $("#ProcessType").val();
    if (processTypeVal == "") {
        verModeRadio2 = "";
        setDisableWhenSelectRdoDelete(conYes);

    } else if (processTypeVal == "REGISTER") {
        rdoRegister_Select();
    } else if (processTypeVal == "DELETE") {
        rdoDelete_Select();
    }

    
}

function BindGridObjectClickEvent(id, row_id, row_idx, func) {
    var ctrl = "#" + GenerateGridControlID(id, row_id);

    $(ctrl).unbind("focus");
    $(ctrl).focus(function () {
        if (this.className.indexOf("row-image-disabled") < 0) {
            if (typeof (func) == "function") {
                func(row_id);
            }
        }
        return false;
    });
}

function BindGridObjectSelectEvent(id, row_id, row_idx, func) {
    var ctrl = "#" + GenerateGridControlID(id, row_id);

    $(ctrl).unbind("change");
    $(ctrl).change(function () {
        if (this.className.indexOf("row-image-disabled") < 0) {
            if (typeof (func) == "function") {
                func(row_id);
            }
        }
        return false;
    });
}

function BindGridDateTimePickerLeaveEvent(row_id) {
    var ctrl = "#" + GenerateGridControlID("dtpFrom", row_id) + ", #" + GenerateGridControlID("dtpTo", row_id);
    $(ctrl).bind('BlurAfterSetDate', function (event) {
        calculateBillingAmount(row_id);
    });
}


function AddRowIfClickOnLastObjectOnBillingBlankLine(row_id) {

    row_idc = grdIssueBillingDetailGrid.getRowId(grdIssueBillingDetailGrid.getRowsNum() - 1);
    if (row_id == row_idc) {
        Add_BillingBlankLine();

    }
}

function cboBillingtypeSelect(row_id) {

    //var C_BILLING_TYPE_GROUP_DEPOSIT = '0';
    //var C_BILLING_TYPE_GROUP_CONTINUES = '1';
    //var C_BILLING_TYPE_GROUP_SALE = '2';
    //var C_BILLING_TYPE_GROUP_INSTALL = '3';
    //var C_BILLING_TYPE_GROUP_DIFF_AMOUNT = '4';
    //var C_BILLING_TYPE_GROUP_OTHER = '5';

    var strBillingTypeGroup = '';

    var rowId = row_id;
    var clt1 = "#" + GenerateGridControlID("cboBillingtype", rowId);
    var clt2 = "#" + GenerateGridControlID("dtpTo", rowId);
    var clt4 = "#" + GenerateGridControlID("txtBillingamount", rowId);
    var cltBillingTypeGroup = "#" + GenerateGridControlID("txtBillingTypeGroup", rowId);
    $(cltBillingTypeGroup).val('');


    // tt
    var clt5 = "#" + GenerateGridControlID("cboBillingdetailinvoiceformatBilling", rowId);
    var cboIssueInvoiceId = "#" + GenerateGridControlID("cboIssueinvoice", rowId);
    var cboContractOCCId = "#" + GenerateGridControlID("cboContractOCC", rowId);

    var obj = {
        BillingTypeCode: $(clt1).val()
    };

    if ($(clt1).val() != '') {
        $(clt1).SetDisabled(true);
        ajax_method.CallScreenController("/Billing/BLS050_GetBillingTypeGroup", obj, function (result, controls, isWarning) {
            $(clt1).SetDisabled(false);
            if (result != undefined) {
                strBillingTypeGroup = result.BillingTypeGroup;
                $(cltBillingTypeGroup).val(strBillingTypeGroup);

                if (strBillingTypeGroup == BLS050_ViewBag.C_BILLING_TYPE_GROUP_CONTINUES || strBillingTypeGroup == BLS050_ViewBag.C_BILLING_TYPE_GROUP_DIFF_AMOUNT) {
                    $(clt2).EnableDatePicker(true);
                    calculateBillingAmount(row_id);
                } else {
                    //$(clt2).InitialDate();
                    $(clt2).val('');
                    $(clt2).EnableDatePicker(false);
                    //clear txtBillingamount value if not auto calculate and change date period
                    //$(clt4).val('');
                }

                if (strBillingTypeGroup == BLS050_ViewBag.C_BILLING_TYPE_GROUP_SALE) {
                    if ($(clt5 + ' option').length >= 2) {
                        $(clt5)[0].selectedIndex = 1;
                    }
                }
                else {
                    if ($(clt5 + ' option').length >= 1) {
                        $(clt5)[0].selectedIndex = 0;
                    }
                }

                if ($(cboIssueInvoiceId).val() == BLS050_ViewBag.C_ISSUE_INV_NOT_ISSUE || strBillingTypeGroup == BLS050_ViewBag.C_BILLING_TYPE_GROUP_SALE) {
                    $(clt5).attr("disabled", true);
                }
                else {
                    $(clt5).attr("disabled", false);
                }
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }

            if (result.OCCMode) {
                $(cboContractOCCId).val('');
                $(cboContractOCCId).attr("disabled", false);
            }
            else {
                $(cboContractOCCId).val(C_LASTOCC);
                $(cboContractOCCId).attr("disabled", true);
            }
        });
    } else {
        $(clt2).EnableDatePicker(true);
        calculateBillingAmount(row_id);
        
        if ($(cboIssueInvoiceId).val() == BLS050_ViewBag.C_ISSUE_INV_NOT_ISSUE) {
            $(clt5).attr("disabled", true);
        }
        else {
            $(clt5).attr("disabled", false);
        }
        $(clt5)[0].selectedIndex = 0;
        $(cboContractOCCId).val('');
    }


    row_idc = grdIssueBillingDetailGrid.getRowId(grdIssueBillingDetailGrid.getRowsNum() - 1);
    if (row_id == row_idc) {
        Add_BillingBlankLine();
    }
}
function cboIssueinvoiceSelect(row_id) {

    //var C_ISSUE_INV_NORMAL = '0';
    //var C_ISSUE_INV_REALTIME = '1';
    //var C_ISSUE_INV_NOT_ISSUE = '2';

    //var C_PAYMENT_METHOD_AUTO_TRANFER = '1'
    //var C_PAYMENT_METHOD_BANK_TRANSFER = '0'
    //var C_PAYMENT_METHOD_CREDIT_CARD = '2'
    //var C_PAYMENT_METHOD_MESSENGER = '3'

    var d = new Date();

    var rowId = row_id;
    var clt1 = "#" + GenerateGridControlID("cboIssueinvoice", rowId);
    var clt2 = "#" + GenerateGridControlID("cboBillingdetailinvoiceformatBilling", rowId);
    var clt3 = "#" + GenerateGridControlID("cboPaymentmethod", rowId);
    var clt4 = "#" + GenerateGridControlID("dtpExpectedissueautotransferdate", rowId);
    var cltBillingTypeGroup = "#" + GenerateGridControlID("txtBillingTypeGroup", rowId);

    if ($(clt1).val() == BLS050_ViewBag.C_ISSUE_INV_NOT_ISSUE || $(cltBillingTypeGroup).val() == BLS050_ViewBag.C_BILLING_TYPE_GROUP_SALE) {
        $(clt2).attr("disabled", true);
    } else {
        $(clt2).attr("disabled", false);
    }

    if ($(clt1).val() != BLS050_ViewBag.C_ISSUE_INV_NORMAL) {
        if ($(clt3).val() == BLS050_ViewBag.C_PAYMENT_METHOD_BANK_TRANSFER ||
        $(clt3).val() == BLS050_ViewBag.C_PAYMENT_METHOD_MESSENGER) {
            $(clt4).SetDate(d);
            //$(clt4).EnableDatePicker(false);

        } else {
            //$(clt4).InitialDate();
            $(clt4).val('');
            $(clt4).EnableDatePicker(true);
        }
    } else {
        //$(clt4).InitialDate();
        $(clt4).val('');
        $(clt4).EnableDatePicker(true);
    }

    row_idc = grdIssueBillingDetailGrid.getRowId(grdIssueBillingDetailGrid.getRowsNum() - 1);
    if (row_id == row_idc) {
        Add_BillingBlankLine();
    }

}
function cboPaymentmethodSelect(row_id) {

    //var C_ISSUE_INV_NORMAL = '0';
    //var C_ISSUE_INV_REALTIME = '1';
    //var C_ISSUE_INV_NOT_ISSUE = '2';

    //var C_PAYMENT_METHOD_AUTO_TRANFER = '1'
    //var C_PAYMENT_METHOD_BANK_TRANSFER = '0'
    //var C_PAYMENT_METHOD_CREDIT_CARD = '2'
    //var C_PAYMENT_METHOD_MESSENGER = '3'

    var d = new Date();

    var rowId = row_id;
    var clt1 = "#" + GenerateGridControlID("cboIssueinvoice", rowId);
    var clt2 = "#" + GenerateGridControlID("cboBillingdetailinvoiceformatBilling", rowId);
    var clt3 = "#" + GenerateGridControlID("cboPaymentmethod", rowId);
    var clt4 = "#" + GenerateGridControlID("dtpExpectedissueautotransferdate", rowId);

    var cltBillingTypeGroup = "#" + GenerateGridControlID("txtBillingTypeGroup", rowId);

    if ($(clt1).val() == BLS050_ViewBag.C_ISSUE_INV_NOT_ISSUE || $(cltBillingTypeGroup).val() == BLS050_ViewBag.C_BILLING_TYPE_GROUP_SALE) {
        $(clt2).attr("disabled", true);
    } else {
        $(clt2).attr("disabled", false);
    }

    if ($(clt1).val() != BLS050_ViewBag.C_ISSUE_INV_NORMAL) {
        if ($(clt3).val() == BLS050_ViewBag.C_PAYMENT_METHOD_BANK_TRANSFER ||
        $(clt3).val() == BLS050_ViewBag.C_PAYMENT_METHOD_MESSENGER) {
            $(clt4).SetDate(d);
            //$(clt4).EnableDatePicker(true);

        } else {
            //$(clt4).InitialDate();
            $(clt4).val('');
            $(clt4).EnableDatePicker(true);
        }
    } else {
        //$(clt4).InitialDate();
        $(clt4).val('');
        $(clt4).EnableDatePicker(true);
    }

    row_idc = grdIssueBillingDetailGrid.getRowId(grdIssueBillingDetailGrid.getRowsNum() - 1);
    if (row_id == row_idc) {
        Add_BillingBlankLine();
    }
}

function cboBillingdetailinvoiceformatBillingSelect(row_id) {
    var row_idc = grdIssueBillingDetailGrid.getRowId(grdIssueBillingDetailGrid.getRowsNum() - 1);
    if (row_id == row_idc) {
        Add_BillingBlankLine();
    }
}

//Rename function name dtpFromLeave to CalculateBillingAmount
function calculateBillingAmount(rowId) {
    //    var C_BILLING_TYPE_GROUP_DEPOSIT = '0';
    //    var C_BILLING_TYPE_GROUP_CONTINUES = '1';
    //    var C_BILLING_TYPE_GROUP_SALE = '2';
    //    var C_BILLING_TYPE_GROUP_INSTALL = '3';
    //    var C_BILLING_TYPE_GROUP_DIFF_AMOUNT = '4';
    //    var C_BILLING_TYPE_GROUP_OTHER = '5';


    //var rowId = row_id;
    var clt1 = "#" + GenerateGridControlID("cboBillingtype", rowId);
    var clt2 = "#" + GenerateGridControlID("dtpFrom", rowId);
    var clt3 = "#" + GenerateGridControlID("dtpTo", rowId);
    var clt4 = "#" + GenerateGridControlID("txtBillingamount", rowId);
    var cltBillingTypeGroup = "#" + GenerateGridControlID("txtBillingTypeGroup", rowId);

    if ($(cltBillingTypeGroup).val() == BLS050_ViewBag.C_BILLING_TYPE_GROUP_CONTINUES && $(clt2).val() != '' && $(clt3).val() != '') {
        var obj = {
            dtpFrom: $(clt2).val(),
            dtpTo: $(clt3).val(),
            BillingTypeCode: $(clt1).val()
        };
        ajax_method.CallScreenController("/Billing/BLS050_CalculateBillingAmount", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                //$(clt4).val(number_format(parseFloat(result), 2, '.', ','));

                var newVal = (new Number(result)).numberFormat("#,##0.00");
                $(clt4).val(newVal);
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });
    }
    //    else {
    //        $(clt4).val('9999.99');
    //        if ($(clt4).val() == '') {
    //            $(clt4).val('0.00');
    //        }
    //    }
    //    row_idc = grdIssueBillingDetailGrid.getRowId(grdIssueBillingDetailGrid.getRowsNum() - 1);
    //    if (row_id == row_idc) {
    //        Add_BillingBlankLine();
    //    }
}

// function dtpToLeave(row_id) {

//     row_idc = grdIssueBillingDetailGrid.getRowId(grdIssueBillingDetailGrid.getRowsNum() - 1);
//     if (row_id == row_idc) {
//         Add_BillingBlankLine();
//     }
// 
//}
function Add_BillingBlankLine() {

    var rowHead = $('#BLS050_IssueBillingDetailGrid_grid').find('tr')[0];

    // Add content to header
    //var testHtml = '<select id="testSelectCol"><option value="1">Test 1</option><option value="2">Test 2</option><option value="3">Test 3</option></select>';
    //$('#BLS050_IssueBillingDetailGrid_grid').find("tr:eq(1)").find("td:eq(1)").html(testHtml)
    //$("#testSelectCol").change(function () {
    //    var val = $(this).val();
    //    alert(val);
    //});


    CheckFirstRowIsEmpty(grdIssueBillingDetailGrid, true);
    AddNewRow(grdIssueBillingDetailGrid, ["",
                                       "",
                                       "",
                                       "",
                                       "",
                                       "",
                                       "",
                                       "",
                                       "",
                                       ""]);

    var row_idx = grdIssueBillingDetailGrid.getRowsNum() - 1;
    var rowId = grdIssueBillingDetailGrid.getRowId(row_idx);

    var defStringVal = "";
    var defNumVal = 0.00;
    var defDateVal = "";

    GenerateGridCombobox(grdIssueBillingDetailGrid, rowId, "cboBillingtype", "Billingtype"
                    , "/Billing/BLS050_GetComboBoxBillingType", defStringVal, true);
    BindGridObjectSelectEvent("cboBillingtype", rowId, row_idx + 1, cboBillingtypeSelect);

    var chkFirstFee = "#" + GenerateGridControlID("chkFirstFee", rowId);
    var chkFirstFeeColInx = grdIssueBillingDetailGrid.getColIndexById("FirstFeeCheckBox");
    grdIssueBillingDetailGrid.cells(rowId, chkFirstFeeColInx).setValue(GenerateCheckBox2("chkFirstFee", rowId, "", true, false));
    grdIssueBillingDetailGrid.cells(rowId, chkFirstFeeColInx).setAttribute("title", " ");
    
    GenerateGridCombobox(grdIssueBillingDetailGrid, rowId, "cboContractOCC", "ContractOCC"
                    , "/Billing/BLS050_GetComboBoxContractOCC", null, true);
    //BindGridObjectSelectEvent("cboContractOCC", rowId, row_idx + 1, cboIssueinvoiceSelect);

    GenerateGridDateTimePickerFromToSingleCell(grdIssueBillingDetailGrid, rowId, "From"
                    , "dtpFrom", defDateVal, "dtpTo", defDateVal, true);
    BindGridDateTimePickerLeaveEvent(rowId);

    GenerateNumericCurrencyControl(grdIssueBillingDetailGrid, "txtBillingamount", rowId, "Billingamount", BLS050_UIProcessObj.MonthlyBillingAmountCurrencyType); // add by jirawat jannet

    //GenerateNumericBox2(grdIssueBillingDetailGrid, "txtBillingamount", rowId, "Billingamount", defNumVal, 10, 2, 0, 9999999999.99, 0, true);
    //BindGridObjectClickEvent("txtBillingamount", rowId, row_idx + 1, calculateBillingAmount);


    GenerateGridCombobox(grdIssueBillingDetailGrid, rowId, "cboIssueinvoice", "Issueinvoice"
                    , "/Billing/BLS050_GetComboBoxIssueInvoice", BLS050_ViewBag.C_ISSUE_INV_NORMAL, true);
    BindGridObjectSelectEvent("cboIssueinvoice", rowId, row_idx + 1, cboIssueinvoiceSelect);

    GenerateGridCombobox(grdIssueBillingDetailGrid, rowId, "cboPaymentmethod", "Paymentmethod"
                    , "/Billing/BLS050_GetComboBoxPaymentMethod", defStringVal, true);
    BindGridObjectSelectEvent("cboPaymentmethod", rowId, row_idx + 1, cboPaymentmethodSelect);

    GenerateGridCombobox(grdIssueBillingDetailGrid, rowId, "cboBillingdetailinvoiceformatBilling", "BillingdetailinvoiceformatBilling"
                    , "/Billing/BLS050_GetComboBoxBillingDetilsInvoiceFormat", defStringVal, true);
    BindGridObjectSelectEvent("cboBillingdetailinvoiceformatBilling", rowId, row_idx + 1, cboBillingdetailinvoiceformatBillingSelect);

    GenerateGridDateTimePicker(grdIssueBillingDetailGrid, rowId, "dtpExpectedissueautotransferdate", "Expectedissueautotransferdate", defStringVal, true, "75%");
    //BindGridObjectClickEvent("dtpExpectedissueautotransferdate", rowId, row_idx + 1, AddRowIfClickOnLastObjectOnBillingBlankLine);

    GenerateRemoveButton(grdIssueBillingDetailGrid, "btnRemove", rowId, "Del", true);
    BindGridButtonClickEvent("btnRemove", rowId, btnRemoveByBillingDetailGrid);

    //Billing type group
    GenerateTextBox(grdIssueBillingDetailGrid, "txtBillingTypeGroup", rowId, "BillingTypeGroup", "", true);

    grdIssueBillingDetailGrid.setSizes();

}

// Creat by Jirawat Jannet 2016-08-23
function GenerateNumericCurrencyControl(grid, id, row_id, col_id, currency) {
    var col = grid.getColIndexById(col_id);
    var fid = GenerateGridControlID(id, row_id);

    var obj = {
        id: fid,
        currency: currency
    };
    ajax_method.CallScreenController("/Billing/BLS050_GenerateCurrencyNumericTextBox", obj, function (result, controls, isWarning) {
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
function Add_CancelBillingDetailLine(doBillingDetailForCancel) {

    var TempStartDateToEndDate = '';
    if (ConvertDateToTextFormat(ConvertDateObject(doBillingDetailForCancel.BillingStartDate)) == '' ||
    ConvertDateToTextFormat(ConvertDateObject(doBillingDetailForCancel.BillingEndDate)) == '') {
        TempStartDateToEndDate = ConvertDateToTextFormat(ConvertDateObject(doBillingDetailForCancel.BillingStartDate)) +
                                        ConvertDateToTextFormat(ConvertDateObject(doBillingDetailForCancel.BillingEndDate))
    } else {
        TempStartDateToEndDate = ConvertDateToTextFormat(ConvertDateObject(doBillingDetailForCancel.BillingStartDate)) +
                                        " ~ " + ConvertDateToTextFormat(ConvertDateObject(doBillingDetailForCancel.BillingEndDate))
    }

    //Add by Jutarat A. on 29072013
    var strFirstFeeFlag = null;
    if (doBillingDetailForCancel.FirstFeeFlag != null) {
        strFirstFeeFlag = doBillingDetailForCancel.FirstFeeFlag.toString();
    }
    //End Add

    CheckFirstRowIsEmpty(grdCancelBillingDetailGrid, true);
    AddNewRow(grdCancelBillingDetailGrid, ["",
                                       doBillingDetailForCancel.InvoiceNo,
                                       doBillingDetailForCancel.InvoiceOCC,
                                       doBillingDetailForCancel.BillingDetailNo,
                                       doBillingDetailForCancel.BillingTypeCode + ': ' + doBillingDetailForCancel.BillingTypeNameString,
                                       doBillingDetailForCancel.PaymentStatus, // + '-' + doBillingDetailForCancel.PaymentStatusNameString,
                                       TempStartDateToEndDate,
                                       //doBillingDetailForCancel.BillingAmount,
                                       doBillingDetailForCancel.BillingAmountCurrencyType,
                                       ConvertDateToTextFormat(ConvertDateObject(doBillingDetailForCancel.BillingStartDate)),
                                       ConvertDateToTextFormat(ConvertDateObject(doBillingDetailForCancel.BillingEndDate)),
                                       doBillingDetailForCancel.BillingTypeCode,
                                       strFirstFeeFlag, //Add by Jutarat A. on 29072013
                                       ""
                                       ]);

    var row_idx = grdCancelBillingDetailGrid.getRowsNum() - 1;
    var row_id = grdCancelBillingDetailGrid.getRowId(row_idx);

    GenerateCheckBox("chkDel", row_id, "", true);
    BindGridCheckBoxClickEvent("chkDel", row_id, function (rowId, checked) {
        chkCancelBillingDetailGrid(rowId, checked);
    });

    grdCancelBillingDetailGrid.cellById(row_id,
                    grdCancelBillingDetailGrid.getColIndexById("Paymentstatus")
                    ).setAttribute("title", doBillingDetailForCancel.PaymentStatusNameString);

    // set all cancel row to white..
    // White "#ffffff"
    grdCancelBillingDetailGrid.setRowColor(row_id, "#ffffff");

    grdCancelBillingDetailGrid.setSizes();

}

function btnRemoveByBillingDetailGrid(rowId) {
    //Check Box Mark Delete Process
    if (grdIssueBillingDetailGrid.getRowsNum() > 1) {
        DeleteRow(grdIssueBillingDetailGrid, rowId);
        var rowId = grdIssueBillingDetailGrid.getRowId(grdIssueBillingDetailGrid.getRowsNum() - 1);
        //BindGridObjectClickEvent("dtpExpectedissueautotransferdate", rowId, grdIssueBillingDetailGrid.getRowsNum(), AddRowIfClickOnLastObjectOnBillingBlankLine);
        BindGridObjectSelectEvent("cboBillingtype", rowId, grdIssueBillingDetailGrid.getRowsNum(), cboBillingtypeSelect);
        BindGridDateTimePickerLeaveEvent(rowId);
        //BindGridObjectLeaveEvent("txtBillingamount", rowId, grdIssueBillingDetailGrid.getRowsNum(), calculateBillingAmount);
        BindGridObjectSelectEvent("cboIssueinvoice", rowId, grdIssueBillingDetailGrid.getRowsNum(), cboIssueinvoiceSelect);
        BindGridObjectSelectEvent("cboPaymentmethod", rowId, grdIssueBillingDetailGrid.getRowsNum(), cboPaymentmethodSelect);
        BindGridObjectSelectEvent("cboBillingdetailinvoiceformatBilling", rowId, grdIssueBillingDetailGrid.getRowsNum(), cboBillingdetailinvoiceformatBillingSelect);
    }

}

function chkCancelBillingDetailGrid(rowId, checked) {

    if (checked) {
        // bright orange "#fff1cc"
        grdCancelBillingDetailGrid.setRowColor(rowId, "#fff1cc");
    } else {
        // White "#ffffff"
        grdCancelBillingDetailGrid.setRowColor(rowId, "#ffffff");
    }

}

function InitialPage() {


    $("#BillingCode").attr("maxlength", 9);
    $("#BillingOCC").attr("maxlength", 2);

    $("#rdoReCreateBillingDetail").attr("disabled", true);
    $("#rdoCancelBillingDetail").attr("disabled", true);
    $("#rdoForceCreateBillingDetail").attr("disabled", true);
    $("#rdoRegisterAdjustOnNextPeriodAmount").attr("disabled", true);

    $("#btnSelectProcess").attr("disabled", true);

    //$("#rdoDelete").attr("disabled", false);

    InitialDateFromToControl("#dptAdjustBillingPeriodDateFrom", "#dptAdjustBillingPeriodDateTo");
    $("#BillingAmountAdj").BindNumericBox(12, 2, 0, 999999999999.99);

    setVisableTable(0);
    setFormMode(conModeInit);
}

// Form Mode Section

var conModeInit = 0;
var conModeView = 1;
var conModeEdit = 2;
var conModeConfirm = 9;

var conModeRadio1rdoReCreateBillingDetail = 1;
var conModeRadio1rdoCancelBillingDetail = 2;
var conModeRadio1rdoForceCreateBillingDetail = 3;
var conModeRadio1rdoRegisterAdjustOnNextPeriodAmount = 4;

var conModeRadio2rdoRegister = 1;
var conModeRadio2rdoDelete = 2;

var conNo = 0;
var conYes = 1;

var verModeRadio1 = 1;
var verModeRadio2 = 1;

function setFormMode(intMode) {
    // ModeInit
    if (intMode == conModeInit) {

        verModeRadio1 = conModeRadio1rdoReCreateBillingDetail;
        verModeRadio2 = conModeRadio2rdoRegister;

        register_command.SetCommand(null);
        reset_command.SetCommand(null);

        confirm_command.SetCommand(null);
        back_command.SetCommand(null);
        setVisableBillCode(conYes);
    }

    // ModeView = 1;

    if (intMode == conModeView) {
        register_command.SetCommand(btn_Register_click); // 
        reset_command.SetCommand(btn_Reset_click); //

        confirm_command.SetCommand(null);
        back_command.SetCommand(null);

    }

    // ModeEdit = 2;

    if (intMode == conModeEdit) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }

    // ModeConfirm = 9;

    if (intMode == conModeConfirm) {
        register_command.SetCommand(null);
        reset_command.SetCommand(null);
        confirm_command.SetCommand(btn_Confirm_click);
        back_command.SetCommand(btn_Back_click);
    }
}


function ConfirmWarningMessageBox() {
    if (BLS050ConfirmWarningMessageList == undefined || BLS050ConfirmWarningMessageList.length == 0) {
        //Goto confirm mode
        setAllObjectToConfirmMode(conYes);
        setFormMode(conModeEdit);

        // Hide delete button in grid
        var colBtnRemoveIdx1 = grdIssueBillingDetailGrid.getColIndexById("Del");
        grdIssueBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, true);

        if (CheckFirstRowIsEmpty(grdIssueBillingDetailGrid) == false) {
            for (var i = 0; i < grdIssueBillingDetailGrid.getRowsNum(); i++) {
                var row_id = grdIssueBillingDetailGrid.getRowId(i);
                var clt1 = "#" + GenerateGridControlID("cboBillingtype", row_id);

                if ($(clt1).val() == '') {
                    DeleteRow(grdIssueBillingDetailGrid, row_id);
                    i = i - 1;
                }
            }
        }
    }
    else {
        var msg = BLS050ConfirmWarningMessageList.shift();

        var obj = {
            module: "Billing",
            code: msg.Code,
            param: msg.Params
        };
        
        call_ajax_method_json("/Shared/GetMessage", obj, function (data) {
            OpenOkCancelDialog(data.Code, data.Message, function () {
                ConfirmWarningMessageBox();
            }, null);
        });
    }
}

// Mode Event
function btn_Register_click() {
    command_control.CommandControlMode(false); //Add by Jutarat A. on 25122013

    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Billing/BLS050_Register", obj, function (result, controls, isWarning) {
        if (result != undefined) {
            if (result == "1" || result.ResultFlag == "1") {
                //Pass (may be with warning message), need confirmation to enter in confirm mode
                BLS050ConfirmWarningMessageList = result.ConfirmMessageID;
                BLS050WarningMessageList = result.WarningMessage;
                ConfirmWarningMessageBox();
            }
        }
        else if (controls != undefined) {
            VaridateCtrl(controls, controls);
        }

        command_control.CommandControlMode(true); //Add by Jutarat A. on 25122013
    });

    /*

    if((forchk_cboAdjustmentType != $("#cboAdjustmentType").val()
    || forchk_BillingAmountAdj != $("#BillingAmountAdj").val()
    || forchk_dptAdjustBillingPeriodDateFrom != $("#dptAdjustBillingPeriodDateFrom").val()
    || forchk_dptAdjustBillingPeriodDateTo != $("#dptAdjustBillingPeriodDateTo").val())

    && forchk_cboAdjustmentType != null) // case not have adjust-on-next-period data in database 
    {
    //--- data in ‘adjust-on-next-period’ section was change ---
    //---------------------------------------
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Billing/BLS050_Register", obj, function (result, controls, isWarning) {
    if (result != undefined) {
    if (result == "1") {

    // --- Get Message --- 
    //---------------------------------------
    var obj = {
    module: "Billing",
    code: "MSG6036"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
    OpenOkCancelDialog(result.Code, result.Message,
    function () {

    //---------------------------------------
    setAllObjectToConfirmMode(conYes);
    setFormMode(conModeEdit);

    // Hide delete button in grid
    var colBtnRemoveIdx1 = grdIssueBillingDetailGrid.getColIndexById("Del");
    grdIssueBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, true);

    if (CheckFirstRowIsEmpty(grdIssueBillingDetailGrid) == false) {
    for (var i = 0; i < grdIssueBillingDetailGrid.getRowsNum(); i++) {
    var row_id = grdIssueBillingDetailGrid.getRowId(i);
    var clt1 = "#" + GenerateGridControlID("cboBillingtype", row_id);

    if ($(clt1).val() == '') {
    DeleteRow(grdIssueBillingDetailGrid, row_id);
    i = i - 1;
    }

    }
    }
    //---------------------------------------
    },
    null);
    });
    //---------------------------------------
    }
    }
    else if (controls != undefined) {
    VaridateCtrl(controls, controls);
    }
    });
    //---------------------------------------
    } else
    {
    var obj = GetUserAdjustData();
    ajax_method.CallScreenController("/Billing/BLS050_Register", obj, function (result, controls, isWarning) {
    if (result != undefined) {
    if (result == "1") {

    setAllObjectToConfirmMode(conYes);
    setFormMode(conModeEdit);
    // Hide delete button in grid
    var colBtnRemoveIdx1 = grdIssueBillingDetailGrid.getColIndexById("Del");
    grdIssueBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, true);

    if (CheckFirstRowIsEmpty(grdIssueBillingDetailGrid) == false) {
    for (var i = 0; i < grdIssueBillingDetailGrid.getRowsNum(); i++) {
    var row_id = grdIssueBillingDetailGrid.getRowId(i);
    var clt1 = "#" + GenerateGridControlID("cboBillingtype", row_id);

    if ($(clt1).val() == '') {
    DeleteRow(grdIssueBillingDetailGrid, row_id);
    i = i - 1;
    }

    }
    }
    }

    if (result == "MSG6035") {
 
    // --- Get Message --- 
    var obj = {
    module: "Billing",
    code: "MSG6035"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
    OpenOkCancelDialog(result.Code, result.Message,
    function () {

    setAllObjectToConfirmMode(conYes);
    setFormMode(conModeEdit);
    // Hide delete button in grid
    var colBtnRemoveIdx1 = grdIssueBillingDetailGrid.getColIndexById("Del");
    grdIssueBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, true);

    if (CheckFirstRowIsEmpty(grdIssueBillingDetailGrid) == false) {
    for (var i = 0; i < grdIssueBillingDetailGrid.getRowsNum(); i++) {
    var row_id = grdIssueBillingDetailGrid.getRowId(i);
    var clt1 = "#" + GenerateGridControlID("cboBillingtype", row_id);

    if ($(clt1).val() == '') {
    DeleteRow(grdIssueBillingDetailGrid, row_id);
    i = i - 1;
    }

    }
    }

    },
    null);
    });

    }

    if (result == "MSG6036") {

    // --- Get Message --- 
    var obj = {
    module: "Billing",
    code: "MSG6036"
    };
    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
    OpenOkCancelDialog(result.Code, result.Message,
    function () {

    setAllObjectToConfirmMode(conYes);
    setFormMode(conModeEdit);
    // Hide delete button in grid
    var colBtnRemoveIdx1 = grdIssueBillingDetailGrid.getColIndexById("Del");
    grdIssueBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, true);

    if (CheckFirstRowIsEmpty(grdIssueBillingDetailGrid) == false) {
    for (var i = 0; i < grdIssueBillingDetailGrid.getRowsNum(); i++) {
    var row_id = grdIssueBillingDetailGrid.getRowId(i);
    var clt1 = "#" + GenerateGridControlID("cboBillingtype", row_id);

    if ($(clt1).val() == '') {
    DeleteRow(grdIssueBillingDetailGrid, row_id);
    i = i - 1;
    }

    }
    }

    },
    null);
    });

    }

    }
    else if (controls != undefined) {
    VaridateCtrl(controls, controls);
    }
    });
    }
    */

}
function GetUserAdjustData() {

    var arr1 = new Array();
    var arr2 = new Array();

    var header = {
        strContractCode: $("#BillingCode").val(),
        strBillingOCC: $("#BillingOCC").val(),
        rdoProcessTypeSpe: verModeRadio1
    };

    if (CheckFirstRowIsEmpty(grdCancelBillingDetailGrid) == false) {

        var idxColFirstFeeFlag = grdCancelBillingDetailGrid.getColIndexById("FirstFeeFlag"); //Add by Jutarat A. on 29072013
        for (var i = 0; i < grdCancelBillingDetailGrid.getRowsNum(); i++) {

            var row_id = grdCancelBillingDetailGrid.getRowId(i);
            // custom object in grid use object name
            var clt1 = "#" + GenerateGridControlID("chkDel", row_id);
            // non custom object in grid use name in XML
            var clt2 = "#" + GenerateGridControlID("Invoiceno", row_id);
            var clt3 = "#" + GenerateGridControlID("Runningno", row_id);
            var clt4 = "#" + GenerateGridControlID("Billingtype", row_id);
            var clt5 = "#" + GenerateGridControlID("Paymentstatus", row_id);
            var clt6 = "#" + GenerateGridControlID("Billingperiod", row_id);
            var clt7 = "#" + GenerateGridControlID("Billingamount", row_id);

            var clt8 = "#" + GenerateGridControlID("DateStart", row_id);
            var clt9 = "#" + GenerateGridControlID("DateEnd", row_id);
            var clt10 = "#" + GenerateGridControlID("BillingtypeCode", row_id);
            var obj1 = {

                bolDel: $(clt1).prop('checked'),
                strInvoiceno: grdCancelBillingDetailGrid.cells
                                (row_id, grdCancelBillingDetailGrid.getColIndexById("Invoiceno")).getValue(),
                strInvoiceOCC: grdCancelBillingDetailGrid.cells
                                (row_id, grdCancelBillingDetailGrid.getColIndexById("InvoiceOCC")).getValue(),
                strRunningno: grdCancelBillingDetailGrid.cells
                                (row_id, grdCancelBillingDetailGrid.getColIndexById("Runningno")).getValue(),
                strBillingtype: grdCancelBillingDetailGrid.cells
                                (row_id, grdCancelBillingDetailGrid.getColIndexById("Billingtype")).getValue(),
                strPaymentstatus: grdCancelBillingDetailGrid.cells
                                (row_id, grdCancelBillingDetailGrid.getColIndexById("Paymentstatus")).getValue(),
                strBillingperiod: grdCancelBillingDetailGrid.cells
                                (row_id, grdCancelBillingDetailGrid.getColIndexById("Billingperiod")).getValue(),
                strBillingamount: grdCancelBillingDetailGrid.cells
                                (row_id, grdCancelBillingDetailGrid.getColIndexById("Billingamount")).getValue(),
                strDateStart: grdCancelBillingDetailGrid.cells
                                                (row_id, grdCancelBillingDetailGrid.getColIndexById("DateStart")).getValue(),
                strDateEnd: grdCancelBillingDetailGrid.cells
                                                (row_id, grdCancelBillingDetailGrid.getColIndexById("DateEnd")).getValue(),
                strBillingtypeCode: grdCancelBillingDetailGrid.cells
                                                (row_id, grdCancelBillingDetailGrid.getColIndexById("BillingtypeCode")).getValue(),
                FirstFeeFlag: grdCancelBillingDetailGrid.cells(row_id, idxColFirstFeeFlag).getValue() //Add by Jutarat A. on 29072013
            };
            arr1.push(obj1);
        }
    }

    var detail1 = arr1;

    if (CheckFirstRowIsEmpty(grdIssueBillingDetailGrid) == false) {
        for (var i = 0; i < grdIssueBillingDetailGrid.getRowsNum(); i++) {

            var row_id = grdIssueBillingDetailGrid.getRowId(i);

            var clt1 = "#" + GenerateGridControlID("cboBillingtype", row_id);
            var clt2 = "#" + GenerateGridControlID("dtpFrom", row_id);
            var clt3 = "#" + GenerateGridControlID("dtpTo", row_id);
            var clt4 = "#" + GenerateGridControlID("txtBillingamount", row_id);
            var clt42 = "#" + GenerateGridControlID("txtBillingamount", row_id + "CurrencyType");
            var clt5 = "#" + GenerateGridControlID("cboIssueinvoice", row_id);
            var clt6 = "#" + GenerateGridControlID("cboPaymentmethod", row_id);
            var clt7 = "#" + GenerateGridControlID("cboBillingdetailinvoiceformatBilling", row_id);
            var clt8 = "#" + GenerateGridControlID("dtpExpectedissueautotransferdate", row_id);
            var chkFirstFeeFlagId = "#" + GenerateGridControlID("chkFirstFee", row_id);
            var cboContractOCCId = "#" + GenerateGridControlID("cboContractOCC", row_id);

            // if use mouse loss focus on numreic text box may get <blank> input
            // in that case set to 0.00
            if ($(clt4).val() == '') {
                $(clt4).val('0.00')
            }

            var obj2 = {

                strBillingtype: $(clt1).val(),
                dtpFrom: $(clt2).val(),
                dtpTo: $(clt3).val(),
                intBillingamount: $(clt4).val(),
                initBillingamountCurrency: $(clt42).val(),
                strIssueinvoice: $(clt5).val(),
                strPaymentmethod: $(clt6).val(),
                strBillingdetailinvoiceformat: $(clt7).val(),
                dtpExpectedissueautotransferdate: $(clt8).val(),
                FirstFeeFlag: $(chkFirstFeeFlagId).is(":checked"),
                ContractOCC: $(cboContractOCCId).val(),

                strBillingtypeID: GenerateGridControlID("cboBillingtype", row_id),
                dtpFromID: GenerateGridControlID("dtpFrom", row_id),
                dtpToID: GenerateGridControlID("dtpTo", row_id),
                intBillingamountID: GenerateGridControlID("txtBillingamount", row_id),
                initBillingamountCurrencyID: GenerateGridControlID("txtBillingamount", row_id + "CurrencyType"),
                strIssueinvoiceID: GenerateGridControlID("cboIssueinvoice", row_id),
                strPaymentmethodID: GenerateGridControlID("cboPaymentmethod", row_id),
                strBillingdetailinvoiceformatID: GenerateGridControlID("cboBillingdetailinvoiceformatBilling", row_id),
                dtpExpectedissueautotransferdateID: GenerateGridControlID("dtpExpectedissueautotransferdate", row_id),
                chkFirstFeeFlagId: GenerateGridControlID("chkFirstFee", row_id),
                cboContractOCCId: GenerateGridControlID("cboContractOCC", row_id)
            };

            VaridateCtrl([
                obj2.strBillingtypeID,
                obj2.dtpFromID,
                obj2.dtpToID,
                obj2.intBillingamountID,
                obj2.initBillingamountCurrencyID,
                obj2.strIssueinvoiceID,
                obj2.strPaymentmethodID,
                obj2.strBillingdetailinvoiceformatID,
                obj2.dtpExpectedissueautotransferdateID,
                obj2.chkFirstFeeFlagId,
                obj2.cboContractOCCId
            ], null);

            arr2.push(obj2);
        }
    }

    var detail2 = arr2;

    // if use mouse loss focus on numreic text box may get <blank> input
    // in that case set to 0.00

    if ($("#BillingAmountAdj").val() == '') {
        $("#BillingAmountAdj").val('0.00')
    }
    if ($("#BillingAmountAdj").val() == '0') {
        $("#BillingAmountAdj").val('0.00')
    }
    var detail3 = {

        rdoProcessTypeAdj: verModeRadio2,
        cboAdjustmentType: $("#cboAdjustmentType").val(),
        intBillingAmountAdj: $("#BillingAmountAdj").val(),
        intBillingAmountAdjCurrency: $("#BillingAmountAdj").NumericCurrencyValue(),
        dptAdjustBillingPeriodDateFrom: $("#dptAdjustBillingPeriodDateFrom").val(),
        dptAdjustBillingPeriodDateTo: $("#dptAdjustBillingPeriodDateTo").val()

    };

    var returnObj = {
        Header: header,
        Detail1: detail1,
        Detail2: detail2,
        Detail3: detail3
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
    //            setAllObjectToConfirmMode(conNo);
    //            ClearMainForm();
    //            CleardivSpecifyProcessType();
    //            setVisableTable(0);
    //            setFormMode(conModeInit);
    //            DeleteAllRow(grdCancelBillingDetailGrid);
    //            DeleteAllRow(grdIssueBillingDetailGrid);
    //        },
    //        null);
    //    });


    setAllObjectToConfirmMode(conNo);
    ClearMainForm();
    CleardivSpecifyProcessType();
    setVisableTable(0);
    setFormMode(conModeInit);
    DeleteAllRow(grdCancelBillingDetailGrid);
    DeleteAllRow(grdIssueBillingDetailGrid);

}
function btn_Approve_click() {

}
function btn_Reject_click() {
}
function btn_Return_click() {
}
function btn_Close_click() {
}
function btn_Confirm_click() {
    command_control.CommandControlMode(false); //Add by Jutarat A. on 25122013

    // save data
    ajax_method.CallScreenController("/Billing/BLS050_Confirm", "",
        function (result, controls, isWarning) {
            if (result != undefined) {
                if (result != "0") {
                    // goto confirm state



                    // goto confirm state
                    setAllObjectToConfirmMode(conNo);
                    ClearMainForm();
                    CleardivSpecifyProcessType();
                    setVisableTable(0);
                    setFormMode(conModeInit);
                    DeleteAllRow(grdCancelBillingDetailGrid);
                    DeleteAllRow(grdIssueBillingDetailGrid);

                    // Show delete button in grid
                    var colBtnRemoveIdx1 = grdIssueBillingDetailGrid.getColIndexById("Del");
                    grdIssueBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, false);
                    // Concept by P'Leing
                    SetFitColumnForBackAction(grdIssueBillingDetailGrid, "TempSpan");



                    // Success popup
                    var objMsg = {
                        module: "Common",
                        code: "MSG0046"
                    };
                    call_ajax_method_json("/Shared/GetMessage", objMsg, function (resultMsg) {
                        OpenInformationMessageDialog(resultMsg.Code, resultMsg.Message, function () {

                            // tt

                            // "result.Detail2 != undefined" = case delete that have no real time print 
                            // and have no invoice send back

                            if (result.strFilePath != '') {
                                if (result.strFilePath != undefined) {
                                    var key = ajax_method.GetKeyURL(null);
                                    var url = ajax_method.GenerateURL("/Billing/BLS050_GetInvoiceReport?k=" + key + "&fileName=" + result.strFilePath);
                                    window.open(url, "", "scrollbars=no,menubar=yes,height=1024,width=1024,resizable=yes,toolbar=no,location=no,status=yes");
                                }
                            }

                        });
                    });
                }
            }
            else {
                VaridateCtrl(controls, controls);
            }

            command_control.CommandControlMode(true); //Add by Jutarat A. on 25122013
        });

}
function btn_Back_click() {
    setAllObjectToConfirmMode(conNo);
    setFormMode(conModeView);

    $("#rdoReCreateBillingDetail").attr("disabled", true);
    $("#rdoCancelBillingDetail").attr("disabled", true);
    $("#rdoForceCreateBillingDetail").attr("disabled", true);
    $("#rdoRegisterAdjustOnNextPeriodAmount").attr("disabled", true);

    $("#btnSelectProcess").attr("disabled", true);

    // Show delete button in grid
    var colBtnRemoveIdx1 = grdIssueBillingDetailGrid.getColIndexById("Del");
    grdIssueBillingDetailGrid.setColumnHidden(colBtnRemoveIdx1, false);
    // Concept by P'Leing
    SetFitColumnForBackAction(grdIssueBillingDetailGrid, "TempSpan");

    // restore blank row incase input < 3 row
    if (CheckFirstRowIsEmpty(grdIssueBillingDetailGrid) == false) {
        if (grdIssueBillingDetailGrid.getRowsNum() < 3) {
            var tempgrdIssueBillingDetailGridgetRowsNum = grdIssueBillingDetailGrid.getRowsNum()
            for (var i = 0; i < 3 - tempgrdIssueBillingDetailGridgetRowsNum; i++) {
                Add_BillingBlankLine();
            }
        }
    }
}

// Enable Obj On Screen

// Visable Obj On Screen
function setVisableTable(intMode) {

    if (intMode == conModeRadio1rdoReCreateBillingDetail) {
        $("#divSpecifyProcessType").show();
        $("#divCancelBillingDetail").show();
        $("#divIssueBillingDetail").show();
        $("#divAdjustOnNextPreiodAmount").show();
    }
    else if (intMode == conModeRadio1rdoCancelBillingDetail) {
        $("#divSpecifyProcessType").show();
        $("#divCancelBillingDetail").show();
        $("#divIssueBillingDetail").hide();
        $("#divAdjustOnNextPreiodAmount").hide();
    }
    else if (intMode == conModeRadio1rdoForceCreateBillingDetail) {
        $("#divSpecifyProcessType").show();
        $("#divCancelBillingDetail").hide();
        $("#divIssueBillingDetail").show();
        $("#divAdjustOnNextPreiodAmount").hide();
    }
    else if (intMode == conModeRadio1rdoRegisterAdjustOnNextPeriodAmount) {
        $("#divSpecifyProcessType").show();
        $("#divCancelBillingDetail").hide();
        $("#divIssueBillingDetail").hide();
        $("#divAdjustOnNextPreiodAmount").show();
    }
    else {
        $("#divSpecifyProcessType").show();
        $("#divCancelBillingDetail").hide();
        $("#divIssueBillingDetail").hide();
        $("#divAdjustOnNextPreiodAmount").hide();
    }
}

function setVisableBillCode(intMode) {

    if (intMode == conYes) {
        $("#BillingCode").attr("readonly", false);
        $("#BillingOCC").attr("readonly", false);
        $("#btnRetrieve").attr("disabled", false);
        $("#btnClear").attr("disabled", false);
    }
    else {
        $("#BillingCode").attr("readonly", true);
        $("#BillingOCC").attr("readonly", true);
        $("#btnRetrieve").attr("disabled", true);

    }
}

function setDisableWhenSelectRdoDelete(intMode) {

    if (intMode == conYes) {

        $("#cboAdjustmentType").attr("disabled", true);
        $("#BillingAmountAdj").attr("readonly", true);
        $("#BillingAmountAdj").NumericCurrency().attr("disabled", true);
        $("#dptAdjustBillingPeriodDateFrom").EnableDatePicker(false);
        $("#dptAdjustBillingPeriodDateTo").EnableDatePicker(false);

       
    }
    else {
        $("#cboAdjustmentType").attr("disabled", false);
        $("#BillingAmountAdj").attr("readonly", false);
        $("#BillingAmountAdj").NumericCurrency().attr("disabled", false);
        $("#dptAdjustBillingPeriodDateFrom").EnableDatePicker(true);
        $("#dptAdjustBillingPeriodDateTo").EnableDatePicker(true);

        //if (BLS050_UIProcessObj.IsAdjustBillingPeriodAmountCurrentNull) {
        //    $("#BillingAmountAdj").NumericCurrency().attr("disabled", false);
        //} else {
        //    $("#BillingAmountAdj").NumericCurrency().attr("disabled", true);
        //}
    }
}

// Clear Obj On Screen
function ClearMainForm() {

    $("#frmMainForm").clearForm();
}

function CleardivSpecifyProcessType() {
    $("#BillingCode").val("");
    $("#BillingOCC").val("");
    $("#btnRetrieve").attr("disabled", false);
    $("#btnClear").attr("disabled", false);

    $("#BillingClientNameEN").val("");
    $("#BillingClientNameLC").val("");
    $("#CurrentPaymentMethod").val("");

    $("#rdoReCreateBillingDetail").attr("checked", true);

    $("#rdoReCreateBillingDetail").attr("disabled", true);
    $("#rdoCancelBillingDetail").attr("disabled", true);
    $("#rdoForceCreateBillingDetail").attr("disabled", true);
    $("#rdoRegisterAdjustOnNextPeriodAmount").attr("disabled", true);

    $("#btnSelectProcess").attr("disabled", true);
}

function CleardivCancelBillingDetail() {

    DeleteAllRow(grdCancelBillingDetailGrid);

}

function CleardivIssueBillingDetail() {
    DeleteAllRow(grdIssueBillingDetailGrid);
}

function CleardivAdjustOnNextPreiodAmount() {
    //$("#rdoRegister").attr("checked", true);

    $("#cboAdjustmentType").val("");
    $("#BillingAmount").val("");

    $("#dptAdjustBillingPeriodDateFrom").val("");
    $("#dptAdjustBillingPeriodDateTo").val("");
}


function setAllObjectToConfirmMode(intMode) {
    if (intMode == conYes) {
        $("#divSpecifyProcessType").SetViewMode(true);
        $("#divCancelBillingDetail").SetViewMode(true);
        $("#divIssueBillingDetail").SetViewMode(true);
        $("#divAdjustOnNextPreiodAmount").SetViewMode(true);

        $("#divSpecifyProcessType").ResetToNormalControl(false);
        $("#divCancelBillingDetail").ResetToNormalControl(false);
        $("#divIssueBillingDetail").ResetToNormalControl(false);
        $("#divAdjustOnNextPreiodAmount").ResetToNormalControl(false);

    } else {

        $("#divSpecifyProcessType").SetViewMode(false);
        $("#divCancelBillingDetail").SetViewMode(false);
        $("#divIssueBillingDetail").SetViewMode(false);
        $("#divAdjustOnNextPreiodAmount").SetViewMode(false);

        $("#divSpecifyProcessType").ResetToNormalControl(true);
        $("#divCancelBillingDetail").ResetToNormalControl(true);
        $("#divIssueBillingDetail").ResetToNormalControl(true);
        $("#divAdjustOnNextPreiodAmount").ResetToNormalControl(true);

    }

}


// tt
function RegenerateProcessTypeCbo(mode) {

    // mode 1 => -select-, register
    // mode 2 => regiter, delete


    if (mode == 1) {
        $("#ProcessType option").remove();

        $('#ProcessType').append($('<option></option>').val("").html($("#lblSelect").val()));
        $('#ProcessType').append($('<option></option>').val("REGISTER").html($("#lblRegister").val()));
    } else if (mode == 2) {
        $("#ProcessType option").remove();

        $('#ProcessType').append($('<option></option>').val("REGISTER").html($("#lblRegister").val()));
        $('#ProcessType').append($('<option></option>').val("DELETE").html($("#lblDelete").val()));

    }

}

// On Form Event

function btn_Retrieve_click() {

    var obj = {
        ContractCode: $("#BillingCode").val(),
        BillingOCC: $("#BillingOCC").val()
    };

    ajax_method.CallScreenController("/Billing/BLS050_RetrieveData", obj, function (result, controls) {
        if (controls != undefined) {
            VaridateCtrl(controls, controls);
            return;
        }
        else if (result != undefined
            && result._doBLS050GetBillingBasic != null
            && result._doBLS050GetTbt_BillingTargetForView != null
            && result._doBLS050GetBillingDetailForCancelList != null) {

            $("#BillingClientNameEN").val(result._doBLS050GetTbt_BillingTargetForView.FullNameEN);
            $("#BillingClientNameLC").val(result._doBLS050GetTbt_BillingTargetForView.FullNameLC);
            $("#CurrentPaymentMethod").val(result._doBLS050GetBillingBasic.PaymentMethodNameString);
            for (var i = 0; i < result._doBLS050GetBillingDetailForCancelList.length; ++i) {
                Add_CancelBillingDetailLine(result._doBLS050GetBillingDetailForCancelList[i]);
            }

            // add by jirawat jannet
            //if (result._doBLS050GetBillingDetailForCancelList.MonthlyBillingAmountCurrency == null) {
            //    BLS050_UIProcessObj.MonthlyBillingAmountCurrency = '01';
            //} else {
            //    BLS050_UIProcessObj.MonthlyBillingAmountCurrency = result._doBLS050GetBillingBasic.MonthlyBillingAmountCurrency;
            //}

            BLS050_UIProcessObj.MonthlyBillingAmountCurrencyType = '1';

            //////////////////////

            setVisableBillCode(conNo);

            $("#BillingCode").val(result._doBLS050GetBillingBasic.ContractCodeShort)

            // tt --
            if (result._doBLS050GetBillingBasic.AdjustType == null
            || result._doBLS050GetBillingBasic.AdjustType == undefined) {

                RegenerateProcessTypeCbo(1); // 1 ==>  -select-,register

                verModeRadio2 = "";
                $("#cboAdjustmentType").attr("disabled", true);
                $("#BillingAmountAdj").attr("readonly", true);
                $("#BillingAmountAdj").NumericCurrency().attr("disabled", true);
                $("#dptAdjustBillingPeriodDateFrom").EnableDatePicker(false);
                $("#dptAdjustBillingPeriodDateTo").EnableDatePicker(false);

                //$("#rdoDelete").attr("disabled", true);
                //$("#rdoRegister").attr("checked", true);
                //verModeRadio2 = conModeRadio2rdoRegister;
                //setDisableWhenSelectRdoDelete(conNo);
            } else {

                RegenerateProcessTypeCbo(2);  // 2 ==> register, delete

                verModeRadio2 = conModeRadio2rdoRegister;
                setDisableWhenSelectRdoDelete(conNo);

                //$("#rdoDelete").attr("disabled", false);
                //$("#rdoDelete").attr("checked", true);
                //verModeRadio2 = conModeRadio2rdoDelete;
                //setDisableWhenSelectRdoDelete(conYes);
            }
            // tt --

            $("#cboAdjustmentType").val(result._doBLS050GetBillingBasic.AdjustType);
            $("#BillingAmountAdj").val(number_format(parseFloat(result._doBLS050GetBillingBasic.AdjustBillingPeriodAmount), 2, '.', ','));

            if (result._doBLS050GetBillingBasic.AdjustBillingPeriodAmountCurrencyType == null) {
                BLS050_UIProcessObj.IsAdjustBillingPeriodAmountCurrentNull = true;
            } else {
                BLS050_UIProcessObj.IsAdjustBillingPeriodAmountCurrentNull = false;
            }
            
           
            $("#BillingAmountAdj").NumericCurrency().val(result._doBLS050GetBillingBasic.AdjustBillingPeriodAmountCurrencyType);
            $("#dptAdjustBillingPeriodDateFrom").val(ConvertDateToTextFormat(ConvertDateObject(result._doBLS050GetBillingBasic.AdjustBillingPeriodStartDate)));
            $("#dptAdjustBillingPeriodDateTo").val(ConvertDateToTextFormat(ConvertDateObject(result._doBLS050GetBillingBasic.AdjustBillingPeriodEndDate)));

            forchk_cboAdjustmentType = result._doBLS050GetBillingBasic.AdjustType;
            forchk_BillingAmountAdj = number_format(parseFloat(result._doBLS050GetBillingBasic.AdjustBillingPeriodAmount), 2, '.', ',');
            forchk_dptAdjustBillingPeriodDateFrom = ConvertDateToTextFormat(ConvertDateObject(result._doBLS050GetBillingBasic.AdjustBillingPeriodStartDate));
            forchk_dptAdjustBillingPeriodDateTo = ConvertDateToTextFormat(ConvertDateObject(result._doBLS050GetBillingBasic.AdjustBillingPeriodEndDate));

            $("#rdoReCreateBillingDetail").attr("disabled", false);
            $("#rdoCancelBillingDetail").attr("disabled", false);
            $("#rdoForceCreateBillingDetail").attr("disabled", false);
            $("#rdoRegisterAdjustOnNextPeriodAmount").attr("disabled", false);

            $("#btnSelectProcess").attr("disabled", false);

            // alway last run don't swap sequence
            C_ISSUE_INV_NORMAL = result.C_ISSUE_INV_NORMAL;
            C_LASTOCC = result.LastOCC;
        }
        else {
            // after show Error MSG Layer on Right on Screen then go to Init Mode
            setFormMode(conModeInit);
            VaridateCtrl(controls, controls);
        }
    });

}

function btn_Select_Process_click() {

    $("#btnClear").attr("disabled", true);

    if (verModeRadio1 == conModeRadio1rdoReCreateBillingDetail) {
        var obj = GetUserAdjustData();
        ajax_method.CallScreenController("/Billing/BLS050_ChkOperateOrSpCreatePermission", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {

                    setFormMode(conModeView);
                    setVisableTable(verModeRadio1);

                    $("#rdoReCreateBillingDetail").attr("disabled", true);
                    $("#rdoCancelBillingDetail").attr("disabled", true);
                    $("#rdoForceCreateBillingDetail").attr("disabled", true);
                    $("#rdoRegisterAdjustOnNextPeriodAmount").attr("disabled", true);

                    $("#btnSelectProcess").attr("disabled", true);

                    // force reload internal grid combo temp
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboBillingtype");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboIssueinvoice");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboPaymentmethod");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboBillingdetailinvoiceformatBilling");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboContractOCC");

                    var chkFirstFeeColInx = grdIssueBillingDetailGrid.getColIndexById("FirstFeeCheckBox");
                    grdIssueBillingDetailGrid.setColumnHidden(chkFirstFeeColInx, true);
                    $("#imgFirstFeeFlag").hide();

                    Add_BillingBlankLine();
                    Add_BillingBlankLine();
                    Add_BillingBlankLine();

                    grdIssueBillingDetailGrid.setSizes();
                    // alway last run don't swap sequence

                }
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });

    }

    if (verModeRadio1 == conModeRadio1rdoCancelBillingDetail) {
        var obj = GetUserAdjustData();
        ajax_method.CallScreenController("/Billing/BLS050_ChkDelPermission", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {

                    setFormMode(conModeView);
                    setVisableTable(verModeRadio1);

                    $("#rdoReCreateBillingDetail").attr("disabled", true);
                    $("#rdoCancelBillingDetail").attr("disabled", true);
                    $("#rdoForceCreateBillingDetail").attr("disabled", true);
                    $("#rdoRegisterAdjustOnNextPeriodAmount").attr("disabled", true);

                    $("#btnSelectProcess").attr("disabled", true);

                } else {
                    /* --- Get Message --- */
                    var obj = {
                        module: "Billing",
                        code: "MSG6023"
                    };
                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                        OpenErrorMessageDialog(result.Code, result.Message,
                            function () {

                            },
                            null);
                    });
                }
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });
    }

    if (verModeRadio1 == conModeRadio1rdoForceCreateBillingDetail) {
        var obj = GetUserAdjustData();
        ajax_method.CallScreenController("/Billing/BLS050_ChkOperateOrSpCreatePermission", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {

                    setFormMode(conModeView);
                    setVisableTable(verModeRadio1);

                    $("#rdoReCreateBillingDetail").attr("disabled", true);
                    $("#rdoCancelBillingDetail").attr("disabled", true);
                    $("#rdoForceCreateBillingDetail").attr("disabled", true);
                    $("#rdoRegisterAdjustOnNextPeriodAmount").attr("disabled", true);

                    $("#btnSelectProcess").attr("disabled", true);

                    // force reload internal grid combo temp
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboBillingtype");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboIssueinvoice");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboPaymentmethod");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboBillingdetailinvoiceformatBilling");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboContractOCC");

                    var chkFirstFeeColInx = grdIssueBillingDetailGrid.getColIndexById("FirstFeeCheckBox");
                    grdIssueBillingDetailGrid.setColumnHidden(chkFirstFeeColInx, false);
                    $("#imgFirstFeeFlag").show();

                    Add_BillingBlankLine();
                    Add_BillingBlankLine();
                    Add_BillingBlankLine();

                    grdIssueBillingDetailGrid.setSizes();
                    // alway last run don't swap sequence

                } else {
                    /* --- Get Message --- */
                    var obj = {
                        module: "Billing",
                        code: "MSG6087"
                    };
                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                        OpenErrorMessageDialog(result.Code, result.Message,
                            function () {

                            },
                            null);
                    });
                }

            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });

    }

    if (verModeRadio1 == conModeRadio1rdoRegisterAdjustOnNextPeriodAmount) {
        var obj = GetUserAdjustData();
        ajax_method.CallScreenController("/Billing/BLS050_ChkOperatePermission", obj, function (result, controls, isWarning) {
            if (result != undefined) {
                if (result == "1") {

                    setFormMode(conModeView);
                    setVisableTable(verModeRadio1);

                    $("#rdoReCreateBillingDetail").attr("disabled", true);
                    $("#rdoCancelBillingDetail").attr("disabled", true);
                    $("#rdoForceCreateBillingDetail").attr("disabled", true);
                    $("#rdoRegisterAdjustOnNextPeriodAmount").attr("disabled", true);

                    $("#btnSelectProcess").attr("disabled", true);

                    // force reload internal grid combo temp
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboBillingtype");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboIssueinvoice");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboPaymentmethod");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboBillingdetailinvoiceformatBilling");
                    ClearGridComboboxCache(grdIssueBillingDetailGrid, "cboContractOCC");

                    Add_BillingBlankLine();
                    Add_BillingBlankLine();
                    Add_BillingBlankLine();

                    grdIssueBillingDetailGrid.setSizes();
                    // alway last run don't swap sequence

                } else {
                    /* --- Get Message --- */
                    var obj = {
                        module: "Billing",
                        code: "MSG6087"
                    };
                    call_ajax_method_json("/Shared/GetMessage", obj, function (result) {
                        OpenErrorMessageDialog(result.Code, result.Message,
                            function () {

                            },
                            null);
                    });
                }
            }
            else if (controls != undefined) {
                VaridateCtrl(controls, controls);
            }
        });

    }
}

// Radio Select
function rdoReCreateBillingDetail_Select() {
    verModeRadio1 = conModeRadio1rdoReCreateBillingDetail;
}
function rdoCancelBillingDetail_Select() {
    verModeRadio1 = conModeRadio1rdoCancelBillingDetail;
}
function rdoForceCreateBillingDetail_Select() {
    verModeRadio1 = conModeRadio1rdoForceCreateBillingDetail;
}
function rdoRegisterAdjustOnNextPeriodAmount_Select() {
    verModeRadio1 = conModeRadio1rdoRegisterAdjustOnNextPeriodAmount;
}
function rdoRegister_Select() {
    verModeRadio2 = conModeRadio2rdoRegister;
    setDisableWhenSelectRdoDelete(conNo);
}
function rdoDelete_Select() {
    verModeRadio2 = conModeRadio2rdoDelete;
    setDisableWhenSelectRdoDelete(conYes);
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


